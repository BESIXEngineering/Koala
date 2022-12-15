Imports System.Collections.Generic
Imports System.Windows.Forms
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry


Namespace Koala

    Public Class CreateXML2
        Inherits GH_Component
        Implements IGH_VariableParameterComponent

        ''' <summary>
        ''' Dictionary of variable parameter groups
        ''' </summary>
        Private ReadOnly VariableParameterDict As New Dictionary(Of EsaObjectCategory, EsaObjectType()) From {
            {EsaObjectCategory.Project, New EsaObjectType() {
                EsaObjectType.ProjectData}},
            {EsaObjectCategory.Library, New EsaObjectType() {
                EsaObjectType.Layer,
                EsaObjectType.CrossSection}},
            {EsaObjectCategory.Structure0D, New EsaObjectType() {
                EsaObjectType.Node}},
            {EsaObjectCategory.Structure1D, New EsaObjectType() {
                EsaObjectType.Member1D,
                EsaObjectType.ArbitraryProfile,
                EsaObjectType.InternalNode1D}},
            {EsaObjectCategory.Structure2D, New EsaObjectType() {
                EsaObjectType.Member2D,
                EsaObjectType.LoadPanel,
                EsaObjectType.Opening,
                EsaObjectType.InternalEdge2D}},
            {EsaObjectCategory.BoundaryCondition, New EsaObjectType() {
                EsaObjectType.NodeSupport,
                EsaObjectType.BeamPointSupport,
                EsaObjectType.BeamLineSupport,
                EsaObjectType.EdgeSupport,
                EsaObjectType.SurfaceSupport,
                EsaObjectType.Subsoil,
                EsaObjectType.Hinge,
                EsaObjectType.LineHinge,
                EsaObjectType.CrossLink,
                EsaObjectType.RigidArm}},
            {EsaObjectCategory.LoadCase, New EsaObjectType() {
                EsaObjectType.LoadCase,
                EsaObjectType.LoadGroup,
                EsaObjectType.LinearCombination,
                EsaObjectType.NonLinearCombination,
                EsaObjectType.StabilityCombination}},
            {EsaObjectCategory.PointLoad, New EsaObjectType() {
                EsaObjectType.PointLoadPoint,
                EsaObjectType.PointMomentPoint,
                EsaObjectType.PointLoadBeam,
                EsaObjectType.PointMomentBeam,
                EsaObjectType.FreePointLoad,
                EsaObjectType.FreePointMoment}},
            {EsaObjectCategory.LineLoad, New EsaObjectType() {
                EsaObjectType.LineLoadBeam,
                EsaObjectType.LineMomentBeam,
                EsaObjectType.LineLoadEdge,
                EsaObjectType.LineMomentEdge,
                EsaObjectType.FreeLineLoad}},
            {EsaObjectCategory.SurfaceLoad, New EsaObjectType() {
                EsaObjectType.SurfaceLoad,
                EsaObjectType.FreeSurfaceLoad}},
            {EsaObjectCategory.ThermalLoad, New EsaObjectType() {
                EsaObjectType.ThermalLoad1D,
                EsaObjectType.ThermalLoad2D}},
            {EsaObjectCategory.NonLinear, New EsaObjectType() {
                EsaObjectType.NonLinearFunction,
                EsaObjectType.PreTensionElement,
                EsaObjectType.GapElement,
                EsaObjectType.LimitForceElement,
                EsaObjectType.Cable}}
        }

        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("CreateXML", "CreateXML",
                "Create the SciaXML file that can be used to update a model in SCIA Engineer",
                "Koala", "General")
        End Sub

        Private ReadOnly FixedInputParamCount As Integer = 9

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddBooleanParameter("Run", "Run", "Set true to create XML file", GH_ParamAccess.item, False)
            pManager.AddTextParameter("FileName", "FileName", "Output filename", GH_ParamAccess.item)
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("filename", "filename", "Name of the output XML file", GH_ParamAccess.item)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
            Dim run As Boolean = False
            Dim fileName As String = ""

            DA.GetData(0, run)
            If (Not DA.GetData(1, fileName)) Then Return

            If run = False Then
                Exit Sub
            End If

            ' Split project data into original CreateXML input
            Dim projectInfo As New List(Of String)
            Dim structureTypeString As String = ""
            Dim uilanguage As UILanguage = UILanguage.EN
            Dim materials As New List(Of String)
            Dim meshSize As Double = -1
            Dim scale As Double = 1
            Dim remDuplNodes As Boolean = False
            Dim tolerance As Double = 0.001

            Dim projectData = GetInputData(Of IGH_Goo)(DA, EsaObjectType.ProjectData)
            Dim dataCount As Integer = projectData.Count
            If dataCount > 0 Then

                Dim stringValue As String = ""

                For i As Integer = 0 To 4
                    Dim infoValue As String = Nothing
                    If i < dataCount AndAlso projectData(i) IsNot Nothing AndAlso projectData(i).CastTo(stringValue) Then
                        infoValue = stringValue
                    End If
                    projectInfo.Add(infoValue)
                Next

                If dataCount > 5 AndAlso projectData(5) IsNot Nothing Then
                    If projectData(5).CastTo(stringValue) AndAlso Not String.IsNullOrEmpty(stringValue) Then
                        Dim structuralType = GetEnum(Of AnalysisModelStructuralType)(stringValue)
                        structureTypeString = GetEnumDescription(structuralType)
                    End If
                End If

                If dataCount > 6 AndAlso projectData(6) IsNot Nothing Then
                    If projectData(6).CastTo(stringValue) AndAlso Not String.IsNullOrEmpty(stringValue) Then
                        uilanguage = GetEnum(Of UILanguage)(stringValue)
                    End If
                End If

                If dataCount > 7 AndAlso projectData(7) IsNot Nothing Then
                    If projectData(7).CastTo(stringValue) AndAlso Not String.IsNullOrEmpty(stringValue) Then
                        materials = (From s In Split(stringValue, ";")
                                     Where Not String.IsNullOrWhiteSpace(s)
                                     Select s.Trim).ToList
                    End If
                End If

                If dataCount > 8 AndAlso projectData(8) IsNot Nothing Then
                    If projectData(8).CastTo(stringValue) Then
                        If Not Double.TryParse(stringValue, meshSize) Then
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid ProjectData mesh size")
                        End If
                    End If
                End If

                If dataCount > 9 AndAlso projectData(9) IsNot Nothing Then
                    If Not projectData(9).CastTo(scale) AndAlso scale <= 0 Then
                        scale = 1
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid ProjectData scale")
                    End If
                End If

                If dataCount > 10 AndAlso projectData(10) IsNot Nothing Then
                    If projectData(10).CastTo(stringValue) Then
                        remDuplNodes = stringValue = "1"
                    End If
                End If

                If dataCount > 11 AndAlso projectData(11) IsNot Nothing Then
                    If Not projectData(11).CastTo(tolerance) AndAlso tolerance < 0 Then
                        tolerance = 0.001
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid ProjectData tolerance")
                    End If
                End If
            End If

            CreateXMLFile(fileName, structureTypeString, materials, GetEnumDescription(uilanguage), meshSize,
                          GetInputText(DA, EsaObjectType.CrossSection),
                          GetInputText(DA, EsaObjectType.Node),
                          GetInputText(DA, EsaObjectType.Member1D),
                          GetInputText(DA, EsaObjectType.Member2D),
                          GetInputText(DA, EsaObjectType.Opening),
                          GetInputText(DA, EsaObjectType.NodeSupport),
                          GetInputText(DA, EsaObjectType.EdgeSupport),
                          GetInputText(DA, EsaObjectType.LoadCase),
                          GetInputText(DA, EsaObjectType.LoadGroup),
                          GetInputText(DA, EsaObjectType.LineLoadBeam),
                          GetInputText(DA, EsaObjectType.SurfaceLoad),
                          GetInputText(DA, EsaObjectType.FreePointLoad),
                          GetInputText(DA, EsaObjectType.FreeLineLoad),
                          GetInputText(DA, EsaObjectType.FreeSurfaceLoad),
                          GetInputText(DA, EsaObjectType.Hinge),
                          GetInputText(DA, EsaObjectType.LineLoadEdge),
                          GetInputText(DA, EsaObjectType.PointLoadPoint),
                          GetInputText(DA, EsaObjectType.PointLoadBeam),
                          scale,
                          GetInputText(DA, EsaObjectType.LinearCombination),
                          GetInputText(DA, EsaObjectType.NonLinearCombination),
                          GetInputText(DA, EsaObjectType.StabilityCombination),
                          GetInputText(DA, EsaObjectType.CrossLink),
                          GetInputText(DA, EsaObjectType.PreTensionElement),
                          GetInputText(DA, EsaObjectType.GapElement),
                          GetInputText(DA, EsaObjectType.LimitForceElement),
                          projectInfo,
                          GetInputText(DA, EsaObjectType.Layer),
                          GetInputText(DA, EsaObjectType.BeamLineSupport),
                          GetInputText(DA, EsaObjectType.BeamPointSupport),
                          GetInputText(DA, EsaObjectType.Subsoil),
                          GetInputText(DA, EsaObjectType.SurfaceSupport),
                          GetInputText(DA, EsaObjectType.LoadPanel),
                          GetInputText(DA, EsaObjectType.PointMomentPoint),
                          GetInputText(DA, EsaObjectType.PointMomentBeam),
                          GetInputText(DA, EsaObjectType.LineMomentBeam),
                          GetInputText(DA, EsaObjectType.LineMomentEdge),
                          GetInputText(DA, EsaObjectType.FreePointMoment),
                          GetInputText(DA, EsaObjectType.NonLinearFunction),
                          remDuplNodes,
                          tolerance,
                          GetInputText(DA, EsaObjectType.InternalEdge2D),
                          GetInputText(DA, EsaObjectType.RigidArm),
                          GetInputText(DA, EsaObjectType.Cable),
                          GetInputText(DA, EsaObjectType.InternalNode1D),
                          GetInputText(DA, EsaObjectType.LineHinge),
                          GetInputText(DA, EsaObjectType.ThermalLoad1D),
                          GetInputText(DA, EsaObjectType.ThermalLoad2D),
                          GetInputText(DA, EsaObjectType.ArbitraryProfile))

            DA.SetData(0, fileName)
        End Sub

        Private Function GetInputText(DA As IGH_DataAccess, oType As EsaObjectType) As List(Of String)
            Return GetInputData(Of String)(DA, oType)
        End Function

        Private Function GetInputData(Of T)(DA As IGH_DataAccess, oType As EsaObjectType) As List(Of T)
            Dim paramName As String = GetEnumDescription(oType)
            Dim idx As Integer = Params.IndexOfInputParam(paramName)

            GetInputData = New List(Of T)
            If idx > -1 Then
                DA.GetDataList(idx, GetInputData)
            End If
        End Function

        ''' <summary>
        ''' Check if all object types of the category are input parameters
        ''' </summary>
        ''' <param name="cat"></param>
        ''' <returns>-1 if no object type are input, 0 if some are input and 1 if all are input</returns>
        Private Function AreEsaTypesInput(cat As EsaObjectCategory) As Integer
            If Not VariableParameterDict(cat).Any() Then Return -1

            Dim status As Integer = 0
            For Each t As EsaObjectType In VariableParameterDict(cat)
                Dim idx = Params.IndexOfInputParam(GetEnumDescription(t))
                If idx = -1 Then
                    If status = 0 Then
                        status = -1
                    ElseIf status = 1 Then
                        Return 0
                    End If
                Else
                    If status = 0 Then
                        status = 1
                    ElseIf status = -1 Then
                        Return 0
                    End If
                End If
            Next
            Return status
        End Function
        'Public Overrides Function Write(ByVal writer As GH_IO.Serialization.GH_IWriter) As Boolean
        '    Return MyBase.Write(writer)
        'End Function
        'Public Overrides Function Read(ByVal reader As GH_IO.Serialization.GH_IReader) As Boolean
        '    Return MyBase.Read(reader)
        'End Function

        Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)

            For Each cat As EsaObjectCategory In VariableParameterDict.Keys.OrderBy(Function(x) Convert.ToInt32(x))
                Dim catIsChecked As Integer = AreEsaTypesInput(cat)
                Dim catName As String = GetEnumDescription(cat)
                Dim item As ToolStripMenuItem = Menu_AppendItem(menu, catName, AddressOf Menu_ToggleCategoryClicked, True)
                item.CheckState = If(catIsChecked = -1, CheckState.Unchecked, If(catIsChecked = 0, CheckState.Indeterminate, CheckState.Checked))
                item.ToolTipText = "Add all objects of category " & catName
                item.Tag = cat

                ' Add a sub item for each type in the category
                For Each oType In VariableParameterDict(cat).OrderBy(Function(x) Convert.ToInt32(x))
                    Dim subName As String = GetEnumDescription(oType)
                    Dim subitem As New ToolStripMenuItem(subName, Nothing, AddressOf Menu_ToggleObjectTypeClicked) With {
                        .Checked = Params.IndexOfInputParam(subName) <> -1,
                        .Tag = oType
                    }
                    item.DropDownItems.Add(subitem)
                Next
            Next

        End Sub

        ''' <summary>
        ''' Add/remove a single input parameter
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub Menu_ToggleObjectTypeClicked(sender As Object, e As EventArgs)
            Dim item As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
            Dim oType As EsaObjectType = CType(item.Tag, EsaObjectType)
            Dim remove As Boolean = item.Checked
            item.Checked = Not item.Checked


            If remove Then
                Dim paramIdx = Params.IndexOfInputParam(GetEnumDescription(oType))

                If paramIdx > -1 Then
                    ExpireSolution(True)

                    Dim toRemove = Params.Input(paramIdx)
                    Dim doc As GH_Document = OnPingDocument()
                    doc.UndoUtil.RecordGenericObjectEvent("Remove Parameter", Me)

                    Params.UnregisterInputParameter(toRemove)
                    Params.OnParametersChanged()
                    VariableParameterMaintenance()
                End If

            Else
                Dim paramName As String = ""
                Dim paramIdx = ShouldCreateInputParameter(oType, paramName)

                If paramIdx <> -1 Then

                    ExpireSolution(True)
                    Dim doc As GH_Document = OnPingDocument()
                    doc.UndoUtil.RecordGenericObjectEvent("Add Parameter", Me)

                    CreateInputParameter(paramName, paramIdx)

                    Params.OnParametersChanged()
                    VariableParameterMaintenance()
                End If
            End If

        End Sub

        ''' <summary>
        ''' Add/remove all input parameters matching a category
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub Menu_ToggleCategoryClicked(sender As Object, e As EventArgs)
            Dim item As ToolStripMenuItem = CType(sender, ToolStripMenuItem)
            Dim category As EsaObjectCategory = CType(item.Tag, EsaObjectCategory)
            Dim remove As Boolean = (item.CheckState = CheckState.Checked)
            item.CheckState = If(remove, CheckState.Unchecked, CheckState.Checked)

            For Each subitem In item.DropDownItems
                subitem.Checked = item.Checked
            Next subitem

            If remove Then

                Dim paramsToRemove As New List(Of IGH_Param)
                For Each oType As EsaObjectType In VariableParameterDict(category)
                    Dim paramIdx = Params.IndexOfInputParam(GetEnumDescription(oType))
                    If paramIdx > -1 Then
                        paramsToRemove.Add(Params.Input(paramIdx))
                    End If
                Next

                If paramsToRemove.Any Then
                    ExpireSolution(True)
                    Dim doc As GH_Document = OnPingDocument()
                    doc.UndoUtil.RecordGenericObjectEvent("Remove Parameters", Me)

                    For Each p In paramsToRemove
                        Params.UnregisterInputParameter(p)
                    Next

                    Params.OnParametersChanged()
                    VariableParameterMaintenance()
                End If

            Else
                Dim paramAdded As Boolean = False
                For Each oType As EsaObjectType In VariableParameterDict(category)

                    Dim paramName As String = ""
                    Dim paramIdx = ShouldCreateInputParameter(oType, paramName)

                    If paramIdx <> -1 Then
                        If Not paramAdded Then
                            paramAdded = True
                            ExpireSolution(True)
                            Dim doc As GH_Document = OnPingDocument()
                            doc.UndoUtil.RecordGenericObjectEvent("Add Parameters", Me)
                        End If

                        CreateInputParameter(paramName, paramIdx)
                    End If
                Next

                If paramAdded Then
                    Params.OnParametersChanged()
                    VariableParameterMaintenance()
                End If
            End If

        End Sub


#Region "Variable parameter interface"
        Public Function CanInsertParameter(side As GH_ParameterSide, index As Integer) As Boolean Implements IGH_VariableParameterComponent.CanInsertParameter
            CanInsertParameter = False
        End Function

        Public Function CanRemoveParameter(side As GH_ParameterSide, index As Integer) As Boolean Implements IGH_VariableParameterComponent.CanRemoveParameter
            CanRemoveParameter = (side = GH_ParameterSide.Input) AndAlso (index >= FixedInputParamCount)
        End Function

        Public Function CreateParameter(side As GH_ParameterSide, index As Integer) As IGH_Param Implements IGH_VariableParameterComponent.CreateParameter
            CreateParameter = New Grasshopper.Kernel.Parameters.Param_String With {
                .Name = GH_ComponentParamServer.InventUniqueNickname("ABCD", Params.Input),
                .Access = GH_ParamAccess.list,
                .Optional = True,
                .MutableNickName = False
            }
        End Function

        Public Function DestroyParameter(side As GH_ParameterSide, index As Integer) As Boolean Implements IGH_VariableParameterComponent.DestroyParameter
            DestroyParameter = (side = GH_ParameterSide.Input) AndAlso (index >= FixedInputParamCount)
        End Function

        Public Sub VariableParameterMaintenance() Implements IGH_VariableParameterComponent.VariableParameterMaintenance
            ' Perform parameter maintenance here!
            ' ExpireSolution(True)
        End Sub

        Private Function CreateInputParameter(index As Integer, name As String, Optional description As String = "") As IGH_Param
            Dim param As IGH_Param = CreateParameter(GH_ParameterSide.Input, index)
            param.Name = name
            param.NickName = name
            If Not String.IsNullOrEmpty(description) Then param.Description = description

            CreateInputParameter = param
        End Function

        ''' <summary>
        ''' Create input parameter if not yet existing
        ''' </summary>
        Private Function CreateInputParameter(name As String, idx As Integer) As IGH_Param
            Dim description As String = "Flattened data list of " & name
            Dim newInputParam As IGH_Param = CreateInputParameter(idx, name, description)
            Params.RegisterInputParam(newInputParam, idx)
            ' By default flatten the input parameter data
            newInputParam.DataMapping = GH_DataMapping.Flatten

            CreateInputParameter = newInputParam
        End Function

        ''' <summary>
        ''' Determine the index at which to create the new input parameter; returns -1 if the parameter already exists.
        ''' </summary>
        ''' <param name="oType"></param>
        ''' <param name="name"></param>
        Private Function ShouldCreateInputParameter(oType As EsaObjectType, ByRef name As String) As Integer
            name = GetEnumDescription(oType)
            Dim paramIdx = Params.IndexOfInputParam(name)

            If paramIdx <> -1 Then Return -1

            paramIdx = Params.Input.Count

            ' Find target index of parameter to maintain Enum order
            Dim other As EsaObjectType
            Dim currIdx As Integer = CInt(oType)

            For i As Integer = FixedInputParamCount To paramIdx - 1
                If [Enum].TryParse(Params.Input(i).Name, other) Then
                    If (currIdx < CInt(other)) Then
                        paramIdx = i
                        Exit For
                    End If
                End If
            Next i

            Return paramIdx
        End Function

#End Region


        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.CreateXML
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("f0d324f8-c648-4b3b-bb9e-16330a2f4a9e")
            End Get
        End Property
    End Class

End Namespace