Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala

    Public Class BeamPointSupport
        Inherits GH_KoalaComponent

        Const DefaultNamePrefix As String = "SB"
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("Beam Point Support", "BeamPointSupport",
                "Point support on 1D member",
                 "Structure", New EsaObjectType() {EsaObjectType.BeamPointSupport})
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
            pManager.AddTextParameter("Beams", "Beams", "Names of beams where to apply supports (can be multiple beams separated by semi-colon ';')", GH_ParamAccess.item)
            pManager.AddTextParameter("NamePrefix", "NamePrefix", "Object name prefix", GH_ParamAccess.item, DefaultNamePrefix)

            pManager.AddParameter(New Param_Enum("Tx", "Translation in X axis", GH_ParamAccess.item, DegreeOfFreedomForTranslation.Rigid))
            pManager.AddParameter(New Param_Enum("Ty", "Translation in Y axis", GH_ParamAccess.item, DegreeOfFreedomForTranslation.Rigid))
            pManager.AddParameter(New Param_Enum("Tz", "Translation in Z axis", GH_ParamAccess.item, DegreeOfFreedomForTranslation.Rigid))
            pManager.AddParameter(New Param_Enum("Rx", "Rotation around X axis", GH_ParamAccess.item, DegreeOfFreedomSupport.Rigid))
            pManager.AddParameter(New Param_Enum("Ry", "Rotation around Y axis", GH_ParamAccess.item, DegreeOfFreedomSupport.Rigid))
            pManager.AddParameter(New Param_Enum("Rz", "Rotation around Z axis", GH_ParamAccess.item, DegreeOfFreedomSupport.Rigid))

            pManager.AddNumberParameter("StiffnessTx", "StiffnessTx", "Stiffness for Tx in MN/m", GH_ParamAccess.item, 0.0)
            pManager.AddNumberParameter("StiffnessTy", "StiffnessTy", "Stiffness for Ty in MN/m", GH_ParamAccess.item, 0.0)
            pManager.AddNumberParameter("StiffnessTz", "StiffnessTz", "Stiffness for Tz in MN/m", GH_ParamAccess.item, 0.0)
            pManager.AddNumberParameter("StiffnessRx", "StiffnessRx", "Stiffness for Rx in MNm/rad", GH_ParamAccess.item, 0.0)
            pManager.AddNumberParameter("StiffnessRy", "StiffnessRy", "Stiffness for Ry in MNm/rad", GH_ParamAccess.item, 0.0)
            pManager.AddNumberParameter("StiffnessRz", "StiffnessRz", "Stiffness for Rz in MNm/rad", GH_ParamAccess.item, 0.0)

            pManager.AddTextParameter("FunctionTx", "FunctionTx", "Name of the non-linear stiffness function for Tx in MNm", GH_ParamAccess.item, "")
            pManager.AddTextParameter("FunctionTy", "FunctionTy", "Name of the non-linear stiffness function for Ty in MNm", GH_ParamAccess.item, "")
            pManager.AddTextParameter("FunctionTz", "FunctionTz", "Name of the non-linear stiffness function for Tz in MNm", GH_ParamAccess.item, "")
            pManager.AddTextParameter("FunctionRx", "FunctionRx", "Name of the non-linear stiffness function for Rx in MNm/rad", GH_ParamAccess.item, "")
            pManager.AddTextParameter("FunctionRy", "FunctionRy", "Name of the non-linear stiffness function for Ry in MNm/rad", GH_ParamAccess.item, "")
            pManager.AddTextParameter("FunctionRz", "FunctionRz", "Name of the non-linear stiffness function for Rz in MNm/rad", GH_ParamAccess.item, "")

            pManager.AddParameter(New Param_Enum("CoordSystem", "Coordinate system", GH_ParamAccess.item, CoordSystem.GCS))
            pManager.AddParameter(New Param_Enum("CoordDefinition", "Coordinate definition of position along member 1D", GH_ParamAccess.item, CoordinateDefinition.Rela))
            pManager.AddNumberParameter("PositionX", "PositionX", "Start position of support on edge", GH_ParamAccess.item, 0.5)
            pManager.AddParameter(New Param_Enum("Origin", "Origin of position along member 1D", GH_ParamAccess.item, Origin.fromStart))
            pManager.AddIntegerParameter("Repeat", "Repeat", "Repeat", GH_ParamAccess.item, 1)
            pManager.AddNumberParameter("DeltaX", "DeltaX", "DeltaX", GH_ParamAccess.item, 0.1)

        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("BeamPointSupport", "BeamPointSupport", "BeamPointSupport data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
            Dim beams As String = ""
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
            Dim PositionX As Double = 0.0
            Dim Repeat As Integer = 1
            Dim Origin As Origin = Origin.fromStart
            Dim DeltaX As Double = 0.01
            Dim i As Integer

            If (Not DA.GetData(Of String)(0, beams)) Then Return
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
            DA.GetData(22, PositionX)
            If DA.GetData(23, i) Then Origin = CType(i, Origin)
            DA.GetData(24, Repeat)
            DA.GetData(25, DeltaX)

            Dim FlatList As New List(Of System.Object)()
            For Each beamName As String In Split(beams, ";")
                If (String.IsNullOrWhiteSpace(beamName)) Then
                    Continue For
                End If

                FlatList.Add(beamName.Trim())
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
                FlatList.Add(PositionX)
                FlatList.Add(GetEnumDescription(Origin))
                FlatList.Add(Repeat)
                FlatList.Add(DeltaX)
            Next

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
                Return My.Resources.PointSupportOnBeam
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("192c50e9-147c-4b09-9682-d8ef829524a8")
            End Get
        End Property
    End Class

End Namespace