Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala

    Public Class SurfaceEdgeSupport
        Inherits GH_KoalaComponent

        Const DefaultNamePrefix As String = "SSE"
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("Surface Edge Support", "EdgeSupport",
                "Support on surface edge",
                "Structure", New EsaObjectType() {EsaObjectType.SurfaceEdgeSupport})
        End Sub

        Public Overrides ReadOnly Property Exposure As GH_Exposure
            Get
                Return GH_Exposure.secondary
            End Get
        End Property

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddTextParameter("Edge", "Edge", "Definition of parent member name, parent member type (if not present in the 'CreateXML' model) and index of edge. Example: S1;SURFACE;2 or O1;OPENING;1 or NE1;INTERNAL EDGE", GH_ParamAccess.item)
            pManager.AddTextParameter("NamePrefix", "NamePrefix", "Object name prefix", GH_ParamAccess.item, DefaultNamePrefix)

            pManager.AddParameter(New Param_Enum("Tx", "Translation in X axis", GH_ParamAccess.item, DegreeOfFreedomForTranslation.Rigid))
            pManager.AddParameter(New Param_Enum("Ty", "Translation in Y axis", GH_ParamAccess.item, DegreeOfFreedomForTranslation.Rigid))
            pManager.AddParameter(New Param_Enum("Tz", "Translation in Z axis", GH_ParamAccess.item, DegreeOfFreedomForTranslation.Rigid))
            pManager.AddParameter(New Param_Enum("Rx", "Rotation around X axis", GH_ParamAccess.item, DegreeOfFreedomSupport.Rigid))
            pManager.AddParameter(New Param_Enum("Ry", "Rotation around Y axis", GH_ParamAccess.item, DegreeOfFreedomSupport.Rigid))
            pManager.AddParameter(New Param_Enum("Rz", "Rotation around Z axis", GH_ParamAccess.item, DegreeOfFreedomSupport.Rigid))

            pManager.AddNumberParameter("StiffnessTx", "StiffnessTx", "Stiffness for Tx in MN/m^2", GH_ParamAccess.item, 0.0)
            pManager.AddNumberParameter("StiffnessTy", "StiffnessTy", "Stiffness for Ty in MN/m^2", GH_ParamAccess.item, 0.0)
            pManager.AddNumberParameter("StiffnessTz", "StiffnessTz", "Stiffness for Tz in MN/m^2", GH_ParamAccess.item, 0.0)
            pManager.AddNumberParameter("StiffnessRx", "StiffnessRx", "Stiffness for Rx in MNm/m/rad", GH_ParamAccess.item, 0.0)
            pManager.AddNumberParameter("StiffnessRy", "StiffnessRy", "Stiffness for Ry in MNm/m/rad", GH_ParamAccess.item, 0.0)
            pManager.AddNumberParameter("StiffnessRz", "StiffnessRz", "Stiffness for Rz in MNm/m/rad", GH_ParamAccess.item, 0.0)

            pManager.AddTextParameter("FunctionTx", "FunctionTx", "Name of the non-linear stiffness function for Tx in MNm", GH_ParamAccess.item, "")
            pManager.AddTextParameter("FunctionTy", "FunctionTy", "Name of the non-linear stiffness function for Ty in MNm", GH_ParamAccess.item, "")
            pManager.AddTextParameter("FunctionTz", "FunctionTz", "Name of the non-linear stiffness function for Tz in MNm", GH_ParamAccess.item, "")
            pManager.AddTextParameter("FunctionRx", "FunctionRx", "Name of the non-linear stiffness function for Rx in MNm/rad", GH_ParamAccess.item, "")
            pManager.AddTextParameter("FunctionRy", "FunctionRy", "Name of the non-linear stiffness function for Ry in MNm/rad", GH_ParamAccess.item, "")
            pManager.AddTextParameter("FunctionRz", "FunctionRz", "Name of the non-linear stiffness function for Rz in MNm/rad", GH_ParamAccess.item, "")

            pManager.AddParameter(New Param_Enum("CoordSystem", "Coordinate system", GH_ParamAccess.item, CoordSystem.GCS))
            pManager.AddParameter(New Param_Enum("CoordDefinition", "Coordinate definition of position along edge", GH_ParamAccess.item, CoordinateDefinition.Rela))
            pManager.AddNumberParameter("Position1", "Position1", "Start position of support on edge", GH_ParamAccess.item, 0)
            pManager.AddNumberParameter("Position2", "Position2", "End position of support on edge", GH_ParamAccess.item, 1)
            pManager.AddParameter(New Param_Enum("Origin", "Origin of position along edge", GH_ParamAccess.item, Origin.fromStart))
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("EdgeSupport", "EdgeSupport", "Surface EdgeSupport data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
            Dim edgeDefinition As String = ""
            Dim namePrefix As String = DefaultNamePrefix

            Dim Tx As DegreeOfFreedomForTranslation = DegreeOfFreedomForTranslation.Rigid
            Dim Ty As DegreeOfFreedomForTranslation = DegreeOfFreedomForTranslation.Rigid
            Dim Tz As DegreeOfFreedomForTranslation = DegreeOfFreedomForTranslation.Rigid
            Dim Rx As DegreeOfFreedomSupport = DegreeOfFreedomSupport.Rigid
            Dim Ry As DegreeOfFreedomSupport = DegreeOfFreedomSupport.Rigid
            Dim Rz As DegreeOfFreedomSupport = DegreeOfFreedomSupport.Rigid
            Dim TxStiffness As Double
            Dim TyStiffness As Double
            Dim TzStiffness As Double
            Dim RxStiffness As Double
            Dim RyStiffness As Double
            Dim RzStiffness As Double
            Dim TxFunction As String = ""
            Dim TyFunction As String = ""
            Dim TzFunction As String = ""
            Dim RxFunction As String = ""
            Dim RyFunction As String = ""
            Dim RzFunction As String = ""

            Dim system As CoordSystem = CoordSystem.GCS
            Dim CoordDefinition As CoordinateDefinition = CoordinateDefinition.Rela
            Dim Position1 As Double = 0.0
            Dim Position2 As Double = 1.0
            Dim Origin As Origin = Origin.fromStart
            Dim i As Integer

            If (Not DA.GetData(Of String)(0, edgeDefinition)) Then Return
            DA.GetData(1, namePrefix)

            If DA.GetData(2, i) Then Tx = CType(i, DegreeOfFreedomForTranslation)
            If DA.GetData(3, i) Then Ty = CType(i, DegreeOfFreedomForTranslation)
            If DA.GetData(4, i) Then Tz = CType(i, DegreeOfFreedomForTranslation)
            If DA.GetData(5, i) Then Rx = CType(i, DegreeOfFreedomSupport)
            If DA.GetData(6, i) Then Ry = CType(i, DegreeOfFreedomSupport)
            If DA.GetData(7, i) Then Rz = CType(i, DegreeOfFreedomSupport)

            DA.GetData(Of Double)(8, TxStiffness)
            DA.GetData(Of Double)(9, TyStiffness)
            DA.GetData(Of Double)(10, TzStiffness)
            DA.GetData(Of Double)(11, RxStiffness)
            DA.GetData(Of Double)(12, RyStiffness)
            DA.GetData(Of Double)(13, RzStiffness)

            DA.GetData(Of String)(14, TxFunction)
            DA.GetData(Of String)(15, TyFunction)
            DA.GetData(Of String)(16, TzFunction)
            DA.GetData(Of String)(17, RxFunction)
            DA.GetData(Of String)(18, RyFunction)
            DA.GetData(Of String)(19, RzFunction)

            If DA.GetData(20, i) Then system = CType(i, CoordSystem)

            If DA.GetData(21, i) Then CoordDefinition = CType(i, CoordinateDefinition)
            DA.GetData(22, Position1)
            DA.GetData(23, Position2)
            If DA.GetData(24, i) Then Origin = CType(i, Origin)

            Dim referenceobj As String, referencetype As String
            Dim supportedgeIdx As Integer = 1

            Dim FlatList As New List(Of System.Object)()

            Dim edgeDefinitionParts = edgeDefinition.Split(";")
            referenceobj = edgeDefinitionParts(0).Trim()

            If edgeDefinitionParts.Length = 1 Then
                supportedgeIdx = 1
                referencetype = ""
            Else
                ' If no 2D member type is given, but only the edge
                If Integer.TryParse(edgeDefinitionParts(1), supportedgeIdx) Then
                    referencetype = ""
                Else
                    referencetype = edgeDefinitionParts(1).Trim()

                    If edgeDefinitionParts.Length > 2 Then
                        If Not Integer.TryParse(edgeDefinitionParts(2), supportedgeIdx) Then
                            supportedgeIdx = 1
                        End If
                    End If
                End If

                referencetype = referencetype.Trim().ToUpperInvariant()

                If referencetype <> "" And referencetype <> "INTERNAL EDGE" And referencetype <> "SURFACE" And referencetype <> "OPENING" Then
                    Throw New ArgumentException("Invalid edge reference type. Must be SURFACE, OPENING or INTERNAL EDGE")
                End If
            End If

            If referencetype = "INTERNAL EDGE" Then
                supportedgeIdx = 1
            End If

            FlatList.Add(referenceobj)
            FlatList.Add(referencetype)
            FlatList.Add(supportedgeIdx)
            NameIndex += 1
            FlatList.Add(namePrefix & NameIndex.ToString())

            FlatList.Add(GetEnumDescription(Tx))
            FlatList.Add(GetEnumDescription(Ty))
            FlatList.Add(GetEnumDescription(Tz))
            FlatList.Add(GetEnumDescription(Rx))
            FlatList.Add(GetEnumDescription(Ry))
            FlatList.Add(GetEnumDescription(Rz))
            FlatList.Add(TxStiffness * 1000000.0)
            FlatList.Add(TyStiffness * 1000000.0)
            FlatList.Add(TzStiffness * 1000000.0)
            FlatList.Add(RxStiffness * 1000000.0)
            FlatList.Add(RyStiffness * 1000000.0)
            FlatList.Add(RzStiffness * 1000000.0)
            FlatList.Add(TxFunction)
            FlatList.Add(TyFunction)
            FlatList.Add(TzFunction)
            FlatList.Add(RxFunction)
            FlatList.Add(RyFunction)
            FlatList.Add(RzFunction)

            FlatList.Add(GetEnumDescription(system))
            FlatList.Add(GetEnumDescription(CoordDefinition))
            FlatList.Add(Position1)
            FlatList.Add(Position2)
            FlatList.Add(GetEnumDescription(Origin))

            DA.SetDataList(0, FlatList)
        End Sub


        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.LineSupport

            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("61dc16e7-8e8b-40da-8a64-2b9686281a4a")
            End Get
        End Property
    End Class

End Namespace