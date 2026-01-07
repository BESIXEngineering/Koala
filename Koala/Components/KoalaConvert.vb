Imports System.Windows.Forms
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Types

Namespace Koala

    ''' <summary>
    ''' Component with very rudimentary support for converting arbitrary objects to Koala model data.
    ''' The input object must support Koala conversion, i.e. have a ToKoala() method that returns a compatible List(Of Dictionary(Of String, String)) object.
    ''' Each dictionary in the list represents one object, with key-value pairs for the object properties.
    ''' The "ObjectType" key is mandatory and must match one of the EsaObjectType enum values. The other keys are used to specify the object properties.
    ''' The component solution will extract the property values in the correct order for each object type and output them as a data tree of strings (one branch per object).
    ''' By using Dictionary(Of String, String) as the data exchange structure, it becomes easy to include future changes in object types or properties.
    ''' The output parameters can be added/removed manually or automatically based on the object types detected in the input data.
    ''' </summary>
    Public Class KoalaConvert
        Inherits GH_KoalaComponent
        Implements IGH_VariableParameterComponent

        Public Overrides ReadOnly Property ObjectTypes As EsaObjectType()
            Get
                ' Returns all EsaObjectType values of which Params.Output contains a parameter with Name matching the enum description
                Dim outputParamNames As New HashSet(Of String)(Params.Output.Select(Function(p) p.Name))
                Return [Enum].GetValues(GetType(EsaObjectType)).Cast(Of EsaObjectType)().Where(Function(t) outputParamNames.Contains(GetEnumDescription(t))).ToArray()
            End Get
        End Property

        Public Sub New()
            MyBase.New("KoalaConvert", "KoalaConvert",
                "Convert an object to Koala model data. The object should support Koala conversion, 
                i.e. have a ToKoala() method that returns a compatible List<Dictionary<string, string>> object.",
                "General")
        End Sub

        Private ReadOnly FixedOutputParamCount As Integer = 0
        Private ReadOnly AvailableTypes As New HashSet(Of EsaObjectType)

        Public Overrides Sub AddedToDocument(document As GH_Document)
            MyBase.AddedToDocument(document)
            If Params.Output.Count <= FixedOutputParamCount Then AutoAddOutput()
        End Sub

        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddGenericParameter("Data", "D", "Data to convert", GH_ParamAccess.item)
        End Sub

        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
        End Sub

        Protected Overrides Sub BeforeSolveInstance()
            MyBase.BeforeSolveInstance()
            AvailableTypes.Clear()
        End Sub

        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
            Dim dataGoo As IGH_Goo = Nothing


            If (Not DA.GetData(0, dataGoo)) Then Return
            If (dataGoo Is Nothing) Then Return

            ' Check whether dataGoo has the ToKoala method
            Dim projectData As List(Of Dictionary(Of String, String)) = Nothing

            ' Use reflection to check for ToKoala() method
            Dim sourceObj As Object = dataGoo
            Dim toKoalaMethod = sourceObj.GetType().GetMethod("ToKoala", Type.EmptyTypes)
            If toKoalaMethod Is Nothing Then
                ' Try the nested ScriptVariable type
                sourceObj = dataGoo.ScriptVariable()
                If sourceObj IsNot Nothing Then
                    toKoalaMethod = sourceObj.GetType().GetMethod("ToKoala", Type.EmptyTypes)
                End If
            End If

            If toKoalaMethod Is Nothing Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input data does not support Koala conversion (missing ToKoala method)")
                Return
            End If

            Dim returnType = toKoalaMethod.ReturnType
            ' Check if return type is List(Of Dictionary(Of String, String))
            If returnType.IsGenericType AndAlso
                returnType.GetGenericTypeDefinition() = GetType(List(Of )) Then
                Dim listArg = returnType.GetGenericArguments()(0)
                If listArg.IsGenericType AndAlso
                    listArg.GetGenericTypeDefinition() = GetType(Dictionary(Of ,)) Then
                    Dim dictArgs = listArg.GetGenericArguments()
                    If dictArgs(0) Is GetType(String) AndAlso dictArgs(1) Is GetType(String) Then
                        ' Call the method
                        Try
                            Dim result = toKoalaMethod.Invoke(sourceObj, Nothing)
                            projectData = TryCast(result, List(Of Dictionary(Of String, String)))
                            If projectData Is Nothing Then
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "ToKoala method did not return the expected type List(Of Dictionary(Of String, String))")
                                Return
                            End If
                        Catch ex As Exception
                            ' In case of failure, show the full exception chain to the user
                            Dim msg As String = "Error calling ToKoala method: "
                            While ex IsNot Nothing
                                msg &= Environment.NewLine & "-> " & ex.GetType().Name & ": " & ex.Message
                                ex = ex.InnerException
                            End While
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, msg)
                            Return
                        End Try
                    End If
                End If
            End If

            ' Determine object types
            Dim typeIndexDict As New Dictionary(Of EsaObjectType, List(Of Integer))()

            If projectData IsNot Nothing Then
                For i As Integer = 0 To projectData.Count - 1
                    Dim dict = projectData(i)
                    Dim typeStr As String = Nothing
                    If dict IsNot Nothing AndAlso dict.TryGetValue("ObjectType", typeStr) Then
                        Dim oType As EsaObjectType
                        If [Enum].TryParse(typeStr, True, oType) AndAlso [Enum].IsDefined(GetType(EsaObjectType), oType) Then
                            If Not typeIndexDict.ContainsKey(oType) Then
                                AvailableTypes.Add(oType)
                                typeIndexDict(oType) = New List(Of Integer)()
                            End If
                            typeIndexDict(oType).Add(i)
                        Else
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Could not match ObjectType '{typeStr}' to a valid EsaObjectType at index {i}.")
                        End If
                    Else
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Missing 'ObjectType' key in dictionary at index {i}.")
                    End If
                Next
            End If

            ' Get list of output strings for each output parameter / object type
            ' Inform user if object data is available for which there exist no output parameters
            For Each oType In typeIndexDict.Keys
                Dim paramIdx = Params.IndexOfOutputParam(GetEnumDescription(oType))
                If paramIdx = -1 Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, $"No output parameter for object type {oType}. Use the context menu to add it.")
                Else
                    Dim param As IGH_Param = Params.Output(paramIdx)
                    Dim pathBase As Data.GH_Path = DA.ParameterTargetPath(paramIdx).AppendElement(DA.ParameterTargetIndex(paramIdx))

                    Dim texts As New Data.GH_Structure(Of GH_String)

                    Dim indices = typeIndexDict(oType)
                    Dim c As Integer = 0
                    For Each idx In indices
                        Dim dict = projectData(idx)
                        ' Convert dictionary to XML string
                        Dim objectData As List(Of GH_String) = DictionaryToObjectData(oType, dict)
                        texts.AppendRange(objectData, pathBase.AppendElement(c))
                        c += 1
                    Next
                    DA.SetDataTree(paramIdx, texts)
                End If
            Next
        End Sub

        ''' <summary>
        ''' Converts a dictionary representing an object to a list of GH_String values, depending on the object type.
        ''' The order and selection of values matches the output of the corresponding component for the object type.
        ''' </summary>
        Private Function DictionaryToObjectData(oType As EsaObjectType, dict As Dictionary(Of String, String)) As IEnumerable(Of GH_String)
            Dim result As New List(Of GH_String)

            If dict Is Nothing Then Return result

            Dim keys As String()

            Select Case oType
                Case EsaObjectType.Undefined
                    Return result

                Case EsaObjectType.ProjectData
                    keys = {"ProjectName", "Part", "Description", "Author", "DateOfCreation", "StructureType", "UILanguage", "Materials", "MeshSize", "Scale", "RemoveDuplicateNodes", "Tolerance"}

                Case EsaObjectType.Layer
                    keys = {"Name", "Description", "IsStructuralOnly"}

                Case EsaObjectType.CrossSection
                    keys = {"Name", "Form", "Description", "Material"}

                Case EsaObjectType.Node
                    keys = {"Name", "X", "Y", "Z"}

                Case EsaObjectType.Member1D
                    keys = {"Name", "Section", "Layer", "Shape", "LcsType", "LcsX", "LcsY", "LcsZ", "StructuralType", "FEMType", "Alignment", "ey", "ez", "BucklingGroupIndex"}

                Case EsaObjectType.Member2D
                    keys = {"Name", "StructuralType", "Material", "Thickness", "Layer", "Shape", "InternalNodes", "Alignment", "ez", "FEMType", "SwapOrientation", "LcsAngle"}

                'Case EsaObjectType.MeshSetup
                Case EsaObjectType.Material
                    keys = {"Definition"}

                Case EsaObjectType.Selection
                    keys = {"Name", "ObjectNamesAndTypes"}

                Case EsaObjectType.LoadPanel
                    keys = {"Name", "Layer", "Shape", "PanelType", "TransferDirection", "TransferMethod", "SwapOrientation", "LcsAngle"}

                Case EsaObjectType.Opening
                    keys = {"Name", "Surface", "Shape"}

                Case EsaObjectType.ArbitraryProfile
                    keys = {"Beam", "Name", "Section", "CoordDefinition", "Lengths", "TypesOfCss", "Css1", "Css2", "Alignments"}

                Case EsaObjectType.InternalNode1D
                    keys = {"Name", "X", "Y", "Z", "Beam"}

                Case EsaObjectType.InternalEdge2D
                    keys = {"Name", "Surface", "Shape"}

                Case EsaObjectType.NodeSupport
                    keys = {"Node", "Name", "Tx", "Ty", "Tz", "Rx", "Ry", "Rz", "SpringX", "SpringY", "SpringZ", "SpringRx", "SpringRy", "SpringRz", "NonLinearX", "NonLinearY", "NonLinearZ", "NonLinearRx", "NonLinearRy", "NonLinearRz", "CoordinateSystem", "Angle"}

                Case EsaObjectType.SurfaceEdgeSupport
                    keys = {"Surface", "SurfaceType", "Edge", "Name", "Tx", "Ty", "Tz", "Rx", "Ry", "Rz", "SpringX", "SpringY", "SpringZ", "SpringRx", "SpringRy", "SpringRz", "CoordinateSystem", "CoordinateDefinition", "Position1", "Position2", "Origin"}

                Case EsaObjectType.BeamLineSupport
                    keys = {"Beam", "Name", "Tx", "Ty", "Tz", "Rx", "Ry", "Rz", "SpringX", "SpringY", "SpringZ", "SpringRx", "SpringRy", "SpringRz", "NonLinearX", "NonLinearY", "NonLinearZ", "NonLinearRx", "NonLinearRy", "NonLinearRz", "CoordinateSystem", "CoordinateDefinition", "Position1", "Position2", "Origin"}

                Case EsaObjectType.BeamPointSupport
                    keys = {"Beam", "Name", "Tx", "Ty", "Tz", "Rx", "Ry", "Rz", "SpringX", "SpringY", "SpringZ", "SpringRx", "SpringRy", "SpringRz", "NonLinearX", "NonLinearY", "NonLinearZ", "NonLinearRx", "NonLinearRy", "NonLinearRz", "CoordinateSystem", "CoordinateDefinition", "Position", "Origin", "Repeat", "Spacing"}

                Case EsaObjectType.SurfaceSupport
                    keys = {"Surface", "Name", "Subsoil"}

                Case EsaObjectType.Subsoil
                    keys = {"Name", "Description", "C1x", "C1y", "C1z", "C2x", "C2y", "C1zType", "NonlinearFunction"}

                Case EsaObjectType.Hinge
                    keys = {"Beam", "Name", "Position", "Tx", "Ty", "Tz", "Rx", "Ry", "Rz", "SpringX", "SpringY", "SpringZ", "SpringRx", "SpringRy", "SpringRz", "NonLinearX", "NonLinearY", "NonLinearZ", "NonLinearRx", "NonLinearRy", "NonLinearRz"}

                Case EsaObjectType.LineHinge
                    keys = {"Surface", "Edge", "CoordinateDefinition", "Position1", "Position2", "Origin", "Tx", "Ty", "Tz", "Rx", "SpringX", "SpringY", "SpringZ", "SpringRx"}

                Case EsaObjectType.CrossLink
                    keys = {"Type", "Member1", "Member2"}

                Case EsaObjectType.RigidArm
                    keys = {"Name", "MasterNode", "SlaveNode", "HingeOnMaster", "HingeOnSlave"}

                Case EsaObjectType.LoadCase
                    keys = {"Name", "Type", "LoadGroup"}

                Case EsaObjectType.LoadGroup
                    keys = {"Name", "Type", "Relation"}

                Case EsaObjectType.LinearCombination
                    keys = {"Name", "Type", "Content"}

                Case EsaObjectType.NonLinearCombination
                    keys = {"Name", "Type", "Content", "Description"}

                Case EsaObjectType.StabilityCombination
                    keys = {"Name", "Content"}

                Case EsaObjectType.ResultClass
                    keys = {"Name", "CaseNames", "CaseTypes", "CaseSubTypes"}

                Case EsaObjectType.MassGroup
                    keys = {"Name", "LoadCase", "UpdateFlag"}

                Case EsaObjectType.MassCombination
                    keys = {"Name", "Content"}

                Case EsaObjectType.SeismicSpectrum
                    keys = {"Name", "Frequencies", "WaveLengths", "Accelerations"}

                Case EsaObjectType.PointLoadNode
                    keys = {"LoadCase", "Node", "CoordinateSystem", "Direction", "Value", "Angle"}

                Case EsaObjectType.PointMomentNode
                    keys = {"LoadCase", "Node", "CoordinateSystem", "Direction", "Value"}

                Case EsaObjectType.PointLoadBeam
                    keys = {"LoadCase", "Beam", "CoordinateSystem", "Direction", "Value", "CoordinateDefinition", "Position", "Origin", "Repeat", "Ey", "Ez", "Spacing"}

                Case EsaObjectType.PointMomentBeam
                    keys = {"LoadCase", "Beam", "CoordinateSystem", "Direction", "Value", "CoordinateDefinition", "Position", "Origin", "Repeat", "Spacing"}

                Case EsaObjectType.FreePointLoad
                    keys = {"LoadCase", "Validity", "Selection", "CoordinateSystem", "Direction", "Value", "X", "Y", "Z", "ValidityFrom", "ValidityTo"}

                Case EsaObjectType.FreePointMoment
                    keys = {"LoadCase", "Validity", "Selection", "CoordinateSystem", "Direction", "Value", "X", "Y", "Z", "ValidityFrom", "ValidityTo"}

                Case EsaObjectType.LineLoadBeam
                    keys = {"LoadCase", "Beam", "CoordinateSystem", "Direction", "Distribution", "Value1", "Value2", "CoordinateDefinition", "Position1", "Position2", "Origin", "Ey", "Ez"}

                Case EsaObjectType.LineMomentBeam
                    keys = {"LoadCase", "Beam", "CoordinateSystem", "Direction", "Distribution", "Value1", "Value2", "CoordinateDefinition", "Position1", "Position2", "Origin"}

                Case EsaObjectType.LineLoadEdge
                    keys = {"LoadCase", "Surface", "Edge", "CoordinateSystem", "Direction", "Distribution", "Value1", "Value2", "CoordinateDefinition", "Position1", "Position2", "Origin", "Ey", "Ez", "SurfaceType"}

                Case EsaObjectType.LineMomentEdge
                    keys = {"LoadCase", "Surface", "Edge", "CoordinateSystem", "Direction", "Distribution", "Value1", "Value2", "CoordinateDefinition", "Position1", "Position2", "Origin", "SurfaceType"}

                Case EsaObjectType.FreeLineLoad
                    keys = {"LoadCase", "Validity", "Selection", "CoordinateSystem", "Direction", "Distribution", "Value1", "Value2", "Shape", "ValidityFrom", "ValidityTo"}

                Case EsaObjectType.SurfaceLoad
                    keys = {"LoadCase", "Surface", "CoordinateSystem", "Direction", "Value"}

                Case EsaObjectType.FreeSurfaceLoad
                    keys = {"LoadCase", "Validity", "Selection", "CoordinateSystem", "Direction", "Distribution", "Value", Nothing, "Shape", "ValidityFrom", "ValidityTo", "Member"}

                Case EsaObjectType.ThermalLoad1D
                    keys = {"LoadCase", "Beam", "Distribution", "Delta", "RightDelta", "LeftDelta", "TopDelta", "BottomDelta", "CoordinateDefinition", "Position1", "Position2", "Origin"}

                Case EsaObjectType.ThermalLoad2D
                    keys = {"LoadCase", "Surface", "Distribution", "Delta", "TopDelta", "BottomDelta"}

                Case EsaObjectType.NonLinearFunction
                    keys = {"Name", "Type", "PositiveEnd", "NegativeEnd", "Points"}

                Case EsaObjectType.PreTensionElement
                    keys = {"Name", "Type"}

                Case EsaObjectType.GapElement
                    keys = {"Name", "Type", "Displacement", "Position"}

                Case EsaObjectType.LimitForceElement
                    keys = {"Name", "Direction", "Type", "MarginalForce"}

                Case EsaObjectType.Cable
                    keys = {"Name", "InitialMesh", "SelfWeight", "NormalForce", "Pn", "Alphax"}

                Case EsaObjectType.IntegrationStrip
                    keys = {"Name", Nothing, "Mesh", "EffectiveWidthGeometry", "EffectiveWidthDefinition", "Width", "Thickness", "X0", "Y0", "Z0", "X1", "Y1", "Z1", "Length", "Surface", "LeftLength", "RightLength"}

                Case EsaObjectType.AveragingStrip
                    keys = {"Surface", "Name", Nothing, "Type", "Direction", "Width", "Length", "Angle", "X0", "X1", "Y0", "Y1", "Z0", "Z1"}

                Case EsaObjectType.SectionOn1D
                    keys = {"Beam", "Name", Nothing, "CoordinateDefinition", "Position", "Origin", "Repeat", "Spacing"}

                Case EsaObjectType.SectionOn2D
                    keys = {"Surface", "Layer", "DrawDiagram", "Direction", "From", "To"}

                Case Else
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"Conversion not implemented for object type {oType}")
                    Return result
            End Select

            Dim missingKeys As New List(Of String)

            For Each key In keys
                If key Is Nothing Then
                    result.Add(New GH_String(String.Empty))
                    Continue For
                End If

                Dim val As String = Nothing
                Dim found As Boolean = False
                ' Try case-sensitive lookup first, then case-insensitive
                If dict.TryGetValue(key, val) Then
                    found = True
                Else
                    For Each k In dict.Keys
                        If String.Equals(k, key, StringComparison.OrdinalIgnoreCase) Then
                            val = dict(k)
                            found = True
                            Exit For
                        End If
                    Next
                End If
                If found Then
                    result.Add(New GH_String(val))
                Else
                    result.Add(New GH_String(String.Empty))
                    missingKeys.Add(key)
                End If
            Next

            ' Inform user about missing object keys
            If missingKeys.Count > 0 Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Missing property keys for {oType} object: {String.Join(", ", missingKeys)}")
            End If

            Return result
        End Function

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
        ''' Detect all object types available in the current input data and add them as output parameters
        ''' </summary>
        Private Sub AutoAddOutput()
            If AvailableTypes.Count > 0 Then
                AddParametersForEsaObjectTypes(AvailableTypes.ToArray())
            End If
        End Sub

        ''' <summary>
        ''' Check if all object types of the category are input parameters
        ''' </summary>
        ''' <param name="cat"></param>
        ''' <returns>-1 if no object type are input, 0 if some are input and 1 if all are input</returns>
        Private Function AreEsaTypesOutput(cat As EsaObjectCategory) As Integer
            If Not CreateXML2.VariableParameterDict(cat).Any() Then Return -1

            Dim status As Integer = 0
            For Each t As EsaObjectType In CreateXML2.VariableParameterDict(cat)
                Dim idx = Params.IndexOfOutputParam(GetEnumDescription(t))
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

        Protected Overrides Sub AppendAdditionalComponentMenuItems(ByVal menu As System.Windows.Forms.ToolStripDropDown)

            Dim autoAddItem As ToolStripMenuItem = Menu_AppendItem(menu, "Autodetect output parameters", AddressOf Menu_AutoAddClicked, True)
            Dim selectItem As ToolStripMenuItem = Menu_AppendItem(menu, "Select output parameters")

            For Each cat As EsaObjectCategory In CreateXML2.VariableParameterDict.Keys.OrderBy(Function(x) Convert.ToInt32(x))
                Dim catIsChecked As Integer = AreEsaTypesOutput(cat)
                Dim catName As String = GetEnumDescription(cat)
                Dim catItem As New ToolStripMenuItem(catName, Nothing, AddressOf Menu_ToggleCategoryClicked) With {
                        .CheckState = If(catIsChecked = -1, CheckState.Unchecked, If(catIsChecked = 0, CheckState.Indeterminate, CheckState.Checked)),
                        .ToolTipText = "Add all objects of category " & catName,
                        .Tag = cat
                    }
                selectItem.DropDownItems.Add(catItem)

                ' Add a sub item for each type in the category
                For Each oType In CreateXML2.VariableParameterDict(cat).OrderBy(Function(x) Convert.ToInt32(x))
                    Dim subName As String = GetEnumDescription(oType)
                    Dim subitem As New ToolStripMenuItem(subName, Nothing, AddressOf Menu_ToggleObjectTypeClicked) With {
                        .Checked = Params.IndexOfOutputParam(subName) <> -1,
                        .Tag = oType
                    }
                    catItem.DropDownItems.Add(subitem)
                Next
            Next

        End Sub

        Private Sub Menu_AutoAddClicked(sender As Object, e As EventArgs)
            AutoAddOutput()
            ExpireSolution(True)
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
                RemoveParameterForEsaObjectType(oType)
            Else
                AddParameterForEsaObjectType(oType)
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
                RemoveParametersForEsaObjectTypes(CreateXML2.VariableParameterDict(category))
            Else
                AddParametersForEsaObjectTypes(CreateXML2.VariableParameterDict(category))
            End If

        End Sub

        Private Sub AddParameterForEsaObjectType(objectType As EsaObjectType)
            Dim paramName As String = ""
            Dim paramIdx = ShouldCreateOutputParameter(objectType, paramName)

            If paramIdx <> -1 Then

                ExpireSolution(True)
                Dim doc As GH_Document = OnPingDocument()
                doc.UndoUtil.RecordGenericObjectEvent("Add Parameter", Me)

                CreateOutputParameter(paramName, paramIdx)

                Params.OnParametersChanged()
                VariableParameterMaintenance()
            End If
        End Sub

        Private Sub RemoveParameterForEsaObjectType(objectType As EsaObjectType)
            Dim paramIdx = Params.IndexOfOutputParam(GetEnumDescription(objectType))

            If paramIdx > -1 Then
                ExpireSolution(True)

                Dim toRemove = Params.Output(paramIdx)
                Dim doc As GH_Document = OnPingDocument()
                doc.UndoUtil.RecordGenericObjectEvent("Remove Parameter", Me)

                Params.UnregisterOutputParameter(toRemove)
                Params.OnParametersChanged()
                VariableParameterMaintenance()
            End If
        End Sub

        Private Sub AddParametersForEsaObjectTypes(objectTypes As ICollection(Of EsaObjectType))
            Dim paramAdded As Boolean = False
            For Each oType As EsaObjectType In objectTypes

                Dim paramName As String = ""
                Dim paramIdx = ShouldCreateOutputParameter(oType, paramName)

                If paramIdx <> -1 Then
                    If Not paramAdded Then
                        paramAdded = True
                        ExpireSolution(True)
                        Dim doc As GH_Document = OnPingDocument()
                        doc.UndoUtil.RecordGenericObjectEvent("Add Parameters", Me)
                    End If

                    CreateOutputParameter(paramName, paramIdx)
                End If
            Next

            If paramAdded Then
                Params.OnParametersChanged()
                VariableParameterMaintenance()
            End If
        End Sub

        Private Sub RemoveParametersForEsaObjectTypes(objectTypes As EsaObjectType())
            Dim paramsToRemove As New List(Of IGH_Param)
            For Each oType As EsaObjectType In objectTypes
                Dim paramIdx = Params.IndexOfOutputParam(GetEnumDescription(oType))
                If paramIdx > -1 Then
                    paramsToRemove.Add(Params.Output(paramIdx))
                End If
            Next

            If paramsToRemove.Any Then
                ExpireSolution(True)
                Dim doc As GH_Document = OnPingDocument()
                doc.UndoUtil.RecordGenericObjectEvent("Remove Parameters", Me)

                For Each p In paramsToRemove
                    Params.UnregisterOutputParameter(p)
                Next

                Params.OnParametersChanged()
                VariableParameterMaintenance()
            End If
        End Sub

#Region "Variable parameter interface"
        Public Function CanInsertParameter(side As GH_ParameterSide, index As Integer) As Boolean Implements IGH_VariableParameterComponent.CanInsertParameter
            CanInsertParameter = False
        End Function

        Public Function CanRemoveParameter(side As GH_ParameterSide, index As Integer) As Boolean Implements IGH_VariableParameterComponent.CanRemoveParameter
            CanRemoveParameter = (side = GH_ParameterSide.Output) AndAlso (index >= FixedOutputParamCount)
        End Function

        Public Function CreateParameter(side As GH_ParameterSide, index As Integer) As IGH_Param Implements IGH_VariableParameterComponent.CreateParameter
            CreateParameter = New Grasshopper.Kernel.Parameters.Param_String With {
                .Name = GH_ComponentParamServer.InventUniqueNickname("ABCD", Params.Output),
                .Access = GH_ParamAccess.tree,
                .Optional = True,
                .MutableNickName = False
            }
        End Function

        Public Function DestroyParameter(side As GH_ParameterSide, index As Integer) As Boolean Implements IGH_VariableParameterComponent.DestroyParameter
            DestroyParameter = (side = GH_ParameterSide.Output) AndAlso (index >= FixedOutputParamCount)
        End Function

        Public Sub VariableParameterMaintenance() Implements IGH_VariableParameterComponent.VariableParameterMaintenance
            ' Perform parameter maintenance here!
            ' ExpireSolution(True)
        End Sub

        Private Function CreateOutputParameter(index As Integer, name As String, Optional description As String = "") As IGH_Param
            Dim param As IGH_Param = CreateParameter(GH_ParameterSide.Output, index)
            param.Name = name
            param.NickName = name
            If Not String.IsNullOrEmpty(description) Then param.Description = description

            CreateOutputParameter = param
        End Function

        ''' <summary>
        ''' Create input parameter if not yet existing
        ''' </summary>
        Private Function CreateOutputParameter(name As String, idx As Integer) As IGH_Param
            Dim description As String = "Data tree of " & name
            Dim newParam As IGH_Param = CreateOutputParameter(idx, name, description)
            Params.RegisterOutputParam(newParam, idx)

            CreateOutputParameter = newParam
        End Function

        ''' <summary>
        ''' Determine the index at which to create the new input parameter; returns -1 if the parameter already exists.
        ''' </summary>
        ''' <param name="oType"></param>
        ''' <param name="name"></param>
        Private Function ShouldCreateOutputParameter(oType As EsaObjectType, ByRef name As String) As Integer
            name = GetEnumDescription(oType)
            Dim paramIdx = Params.IndexOfOutputParam(name)

            If paramIdx <> -1 Then Return -1

            paramIdx = Params.Output.Count

            ' Find target index of parameter to maintain Enum order
            Dim other As EsaObjectType
            Dim currIdx As Integer = CInt(oType)

            For i As Integer = FixedOutputParamCount To paramIdx - 1
                If [Enum].TryParse(Params.Output(i).Name, other) Then
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
                Return My.Resources.Koala
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("a2c324f8-c648-4b3b-bb9e-16330a2f4a9b")
            End Get
        End Property
    End Class

End Namespace