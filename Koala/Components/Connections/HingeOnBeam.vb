Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino
Imports Rhino.Geometry


Namespace Koala

    Public Class Hinge
        Inherits GH_KoalaComponent

        Const DefaultNamePrefix As String = "H"

        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("HingeOnBeam", "Hinge",
                "Hinge on beam",
                "Structure", New EsaObjectType() {EsaObjectType.Hinge})
        End Sub

        Public Overrides ReadOnly Property Exposure As GH_Exposure
            Get
                Return GH_Exposure.tertiary
            End Get
        End Property

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddTextParameter("Beams", "Beams", "Names of beams where to apply hinges (can be multiple beams separated by semi-colon ';')", GH_ParamAccess.item)
            pManager.AddTextParameter("NamePrefix", "NamePrefix", "Object name prefix", GH_ParamAccess.item, DefaultNamePrefix)

            pManager.AddParameter(New Param_Enum("Position", "Position of the hinge", GH_ParamAccess.item, BeamEnd.Both))

            pManager.AddParameter(New Param_Enum("Tx", "Translation in X axis", GH_ParamAccess.item, DegreeOfFreedomHinge.Rigid))
            pManager.AddParameter(New Param_Enum("Ty", "Translation in Y axis", GH_ParamAccess.item, DegreeOfFreedomHinge.Rigid))
            pManager.AddParameter(New Param_Enum("Tz", "Translation in Z axis", GH_ParamAccess.item, DegreeOfFreedomHinge.Rigid))
            pManager.AddParameter(New Param_Enum("Rx", "Rotation around X axis", GH_ParamAccess.item, DegreeOfFreedomHinge.Rigid))
            pManager.AddParameter(New Param_Enum("Ry", "Rotation around Y axis", GH_ParamAccess.item, DegreeOfFreedomHinge.Rigid))
            pManager.AddParameter(New Param_Enum("Rz", "Rotation around Z axis", GH_ParamAccess.item, DegreeOfFreedomHinge.Rigid))

            pManager.AddNumberParameter("StiffnessTx", "StiffnessTx", "Stiffness for Tx in MN/m", GH_ParamAccess.item, 0.0)
            pManager.AddNumberParameter("StiffnessTy", "StiffnessTy", "Stiffness for Ty in MN/m", GH_ParamAccess.item, 0.0)
            pManager.AddNumberParameter("StiffnessTz", "StiffnessTz", "Stiffness for Tz in MN/m", GH_ParamAccess.item, 0.0)
            pManager.AddNumberParameter("StiffnessRx", "StiffnessRx", "Stiffness for Rx in MNm/rad", GH_ParamAccess.item, 0.0)
            pManager.AddNumberParameter("StiffnessRy", "StiffnessRy", "Stiffness for Ry in MNm/rad", GH_ParamAccess.item, 0.0)
            pManager.AddNumberParameter("StiffnessRz", "StiffnessRz", "Stiffness for Rz in MNm/rad", GH_ParamAccess.item, 0.0)

            pManager.AddTextParameter("FunctionTx", "FunctionTx", "WARN: Not supported by SCIA, first available NL function is used!!! Name of the non-linear stiffness function for Tx in MNm", GH_ParamAccess.item, "")
            pManager.AddTextParameter("FunctionTy", "FunctionTy", "WARN: Not supported by SCIA, first available NL function is used!!! Name of the non-linear stiffness function for Ty in MNm", GH_ParamAccess.item, "")
            pManager.AddTextParameter("FunctionTz", "FunctionTz", "WARN: Not supported by SCIA, first available NL function is used!!! Name of the non-linear stiffness function for Tz in MNm", GH_ParamAccess.item, "")
            pManager.AddTextParameter("FunctionRx", "FunctionRx", "WARN: Not supported by SCIA, first available NL function is used!!! Name of the non-linear stiffness function for Rx in MNm/rad", GH_ParamAccess.item, "")
            pManager.AddTextParameter("FunctionRy", "FunctionRy", "WARN: Not supported by SCIA, first available NL function is used!!! Name of the non-linear stiffness function for Ry in MNm/rad", GH_ParamAccess.item, "")
            pManager.AddTextParameter("FunctionRz", "FunctionRz", "WARN: Not supported by SCIA, first available NL function is used!!! Name of the non-linear stiffness function for Rz in MNm/rad", GH_ParamAccess.item, "")
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("Hinge", "Hinge", "Hinge data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            Dim beams As String = ""
            Dim namePrefix As String = DefaultNamePrefix
            Dim position As BeamEnd = BeamEnd.Both
            Dim Tx As DegreeOfFreedomHinge = DegreeOfFreedomHinge.Rigid
            Dim Ty As DegreeOfFreedomHinge = DegreeOfFreedomHinge.Rigid
            Dim Tz As DegreeOfFreedomHinge = DegreeOfFreedomHinge.Rigid
            Dim Rx As DegreeOfFreedomHinge = DegreeOfFreedomHinge.Rigid
            Dim Ry As DegreeOfFreedomHinge = DegreeOfFreedomHinge.Rigid
            Dim Rz As DegreeOfFreedomHinge = DegreeOfFreedomHinge.Rigid
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
            Dim i As Integer

            If (Not DA.GetData(0, beams)) Then Return

            If String.IsNullOrEmpty(beams) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Beams can't be empty")
                Return
            End If

            DA.GetData(1, namePrefix)
            If DA.GetData(2, i) Then position = CType(i, BeamEnd)

            If DA.GetData(3, i) Then Tx = CType(i, DegreeOfFreedomHinge)
            If DA.GetData(4, i) Then Ty = CType(i, DegreeOfFreedomHinge)
            If DA.GetData(5, i) Then Tz = CType(i, DegreeOfFreedomHinge)
            If DA.GetData(6, i) Then Rx = CType(i, DegreeOfFreedomHinge)
            If DA.GetData(7, i) Then Ry = CType(i, DegreeOfFreedomHinge)
            If DA.GetData(8, i) Then Rz = CType(i, DegreeOfFreedomHinge)

            DA.GetData(Of Double)(9, TxStiffness)
            DA.GetData(Of Double)(10, TyStiffness)
            DA.GetData(Of Double)(11, TzStiffness)
            DA.GetData(Of Double)(12, RxStiffness)
            DA.GetData(Of Double)(13, RyStiffness)
            DA.GetData(Of Double)(14, RzStiffness)

            DA.GetData(Of String)(15, TxFunction)
            DA.GetData(Of String)(16, TyFunction)
            DA.GetData(Of String)(17, TzFunction)
            DA.GetData(Of String)(18, RxFunction)
            DA.GetData(Of String)(19, RyFunction)
            DA.GetData(Of String)(20, RzFunction)

            Dim FlatList As New List(Of System.Object)()
            FlatList.Clear()

            For Each beamName As String In Split(beams, ";")
                If (String.IsNullOrWhiteSpace(beamName)) Then
                    Continue For
                End If

                FlatList.Add(beamName.Trim())
                NameIndex += 1
                FlatList.Add(namePrefix & NameIndex.ToString())

                FlatList.Add(GetEnumDescription(position))

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
                Return My.Resources.Hinge
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("f8356149-2e03-4367-ae9e-cd45ba0070f7")
            End Get
        End Property
    End Class

End Namespace