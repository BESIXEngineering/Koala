Imports System.Collections.Generic
Imports System.Windows.Forms
Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala

    Public Class CreateXML2
        Inherits GH_Component
        Implements IGH_VariableParameterComponent

        ''' <summary>
        ''' Dictionary of variable parameter groups
        ''' </summary>
        Private ReadOnly VariableParameterDict As New Dictionary(Of EsaObjectCategory, EsaObjectType()) From {
            {EsaObjectCategory.Library, New EsaObjectType() {
                EsaObjectType.ProjectInfo,
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

        'Structure VariableParameterDefinition
        '    Dim Category As VariableParameterCategory
        '    Dim Name As String
        'End Structure


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

            ' TODO: Enum param options are not maintained when reopening a saved GH file in VariableParameterComponent
            Dim idx As Integer
            idx = pManager.AddParameter(New Param_Enum("StructureType", "Type of analysis model", GH_ParamAccess.item, AnalysisModelStructuralType.GeneralXYZ, False))
            pManager.Param(idx).Optional = True
            idx = pManager.AddParameter(New Param_Enum("UILanguage", "UI language", GH_ParamAccess.item, UILanguage.EN, False))
            pManager.Param(idx).Optional = True
            idx = pManager.AddTextParameter("Materials", "Materials", "Materials: Conctrete, Steel, Timber", GH_ParamAccess.list)
            pManager.Param(idx).Optional = True
            idx = pManager.AddNumberParameter("MeshSize", "MeshSize", "Size of mesh", GH_ParamAccess.item)
            pManager.Param(idx).Optional = True
            idx = pManager.AddNumberParameter("Scale", "Scale", "Scale", GH_ParamAccess.item, 1)
            pManager.Param(idx).Optional = True
            idx = pManager.AddBooleanParameter("RemDuplNodes", "RemDuplNodes", "Output filename", GH_ParamAccess.item, False)
            pManager.Param(idx).Optional = True
            idx = pManager.AddNumberParameter("Tolerance", "Tolerance", "Tolerance for duplicity nodes", GH_ParamAccess.item, 0.001)
            pManager.Param(idx).Optional = True
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
            Dim structureTypeString As String = ""
            Dim UILanguage As UILanguage = UILanguage.EN

            Dim materials = New List(Of String)
            Dim meshSize As Double = -1 ' negative mesh size will be ignored when creating xml

            Dim scale As Double = 1
            Dim remDuplNodes As Boolean = False
            Dim tolerance As Double = 0.001

            Dim i As Integer = 0

            DA.GetData(0, run)
            If (Not DA.GetData(1, fileName)) Then Return
            If (DA.GetData(2, i)) Then
                If Not [Enum].IsDefined(GetType(AnalysisModelStructuralType), i) Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid StructuralType")
                Else
                    structureTypeString = i.ToString
                End If
            End If

            If (DA.GetData(3, i)) Then UILanguage = CType(i, UILanguage)

            DA.GetDataList(4, materials)
            DA.GetData(5, meshSize)
            DA.GetData(6, scale)
            DA.GetData(7, remDuplNodes)
            DA.GetData(8, tolerance)

            If run = False Then
                Exit Sub
            End If

            CreateXMLFile(fileName, structureTypeString, materials, GetEnumDescription(UILanguage), meshSize,
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
                          GetInputText(DA, EsaObjectType.ProjectInfo),
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
            Dim paramName As String = GetEnumDescription(oType)
            Dim idx As Integer = Params.IndexOfInputParam(paramName)

            GetInputText = New List(Of String)
            If idx > -1 Then
                DA.GetDataList(idx, GetInputText)
            End If
        End Function

        ''' <summary>
        ''' Check if all object types of the category are input parameters
        ''' </summary>
        ''' <param name="cat"></param>
        ''' <returns>-1 if no object type are input, 0 if some are input and 1 if all are input</returns>
        Private Function AreTypesInput(cat As EsaObjectCategory) As Integer
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
                Dim catIsChecked As Integer = AreTypesInput(cat)
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