Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala

    Public Class FreePointMoment
        Inherits GH_KoalaComponent

        Public Sub New()
            MyBase.New("FreePointMoment", "FreePointMoment", "Free Point Moment",
                "Load", New EsaObjectType() {EsaObjectType.FreePointMoment})
        End Sub

        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddTextParameter("LoadCase", "LoadCase", "Name of load case", GH_ParamAccess.item, "LC2")
            pManager.AddParameter(New Param_Enum("Validity", "Validity", GH_ParamAccess.item, Validity.All))
            pManager.AddParameter(New Param_Enum("Selection", "Selection", GH_ParamAccess.item, Selection.Auto))
            pManager.AddParameter(New Param_Enum("CoordSys", "Coordinate system", GH_ParamAccess.item, CoordSystemFreePointLoad.GCS))
            pManager.AddParameter(New Param_Enum("Direction", "Direction of load", GH_ParamAccess.item, MomentDirection.Mz))
            pManager.AddNumberParameter("LoadValue", "LoadValue", "Value of Load in KNm", GH_ParamAccess.item, -1)
            pManager.AddPointParameter("Points", "Points", "List of points", GH_ParamAccess.list)
            pManager.AddNumberParameter("ValidityFrom", "ValidityFrom", "Validity From in m", GH_ParamAccess.item, 0)
            pManager.AddNumberParameter("ValidityTo", "ValidityTo", "Validity To in m", GH_ParamAccess.item, 0)
            'pManager.AddTextParameter("Selected2Dmembers", "Selected2Dmembers", "Selected 2D members as list if Selection is put as Selected", GH_ParamAccess.list, {})
        End Sub

        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("FreePointMoment", "FreePointMoment", "FreePointMoment data", GH_ParamAccess.list)
        End Sub

        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
            Dim LoadCase As String = ""
            Dim LoadValue As Double = -1.0
            Dim Points = New List(Of Point3d)
            Dim i As Integer

            Dim ValidityFrom As Double = 0.0
            Dim ValidityTo As Double = 0.0

            If (Not DA.GetData(0, LoadCase)) Then Return

            If (Not DA.GetData(1, i)) Then Return
            Dim validity As Validity = CType(i, Validity)

            If (Not DA.GetData(2, i)) Then Return
            Dim selection As Selection = CType(i, Selection)

            If (Not DA.GetData(3, i)) Then Return
            Dim coordSys As CoordSystemFreePointLoad
            If i = 4 Then  ' for backward compatibility, original default value was set to 4
                coordSys = CoordSystemFreePointLoad.GCS
            Else
                coordSys = CType(i, CoordSystemFreePointLoad)
            End If

            If (Not DA.GetData(4, i)) Then Return
            Dim direction As MomentDirection = CType(i, MomentDirection)

            If (Not DA.GetData(5, LoadValue)) Then Return
            If (Not DA.GetDataList(Of Point3d)(6, Points)) Then Return
            If (Not DA.GetData(7, ValidityFrom)) Then Return
            If (Not DA.GetData(8, ValidityTo)) Then Return

            Dim j As Long

            Dim SE_fploads(Points.Count, 11)
            Dim FlatList As New List(Of System.Object)()
            'a load consists of: load case, validity, selection, coord. system (GCS/LCS), direction (X, Y, Z), value (kN/m), X, Y, Z

            Dim itemcount As Long
            Dim item As Rhino.Geometry.Point3d

            'initialize some variables
            itemcount = 0

            'create load data
            '=================
            For Each item In Points
                SE_fploads(itemcount, 0) = LoadCase
                SE_fploads(itemcount, 1) = GetEnumDescription(validity)
                SE_fploads(itemcount, 2) = GetEnumDescription(selection)
                SE_fploads(itemcount, 3) = GetEnumDescription(coordSys)
                SE_fploads(itemcount, 4) = GetEnumDescription(direction)
                SE_fploads(itemcount, 5) = LoadValue
                SE_fploads(itemcount, 6) = item.X
                SE_fploads(itemcount, 7) = item.Y
                SE_fploads(itemcount, 8) = item.Z
                SE_fploads(itemcount, 9) = ValidityFrom
                SE_fploads(itemcount, 10) = ValidityTo
                itemcount = itemcount + 1
            Next

            'Flatten data for export as simple list
            For i = 0 To itemcount - 1
                For j = 0 To 10
                    FlatList.Add(SE_fploads(i, j))
                Next j
            Next i
            DA.SetDataList(0, FlatList)
        End Sub


        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                Return My.Resources.FreePointMoment
            End Get
        End Property

        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("190bab7f-0553-497c-b710-4ef6e28a6ffa")
            End Get
        End Property
    End Class

End Namespace