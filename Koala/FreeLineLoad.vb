Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala

    Public Class FreeLineLoad
        Inherits GH_Component
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("FreeLineLoad", "FreeLineLoad",
                "MyComponent1 description",
                "Koala", "Load")
        End Sub

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddTextParameter("LoadCase", "LoadCase", "Name of load case", GH_ParamAccess.item, "LC2")
            pManager.AddParameter(New Param_Enum("Validity", "Validity", GH_ParamAccess.item, Validity.All))
            pManager.AddParameter(New Param_Enum("Selection", "Selection", GH_ParamAccess.item, Selection.Auto))
            pManager.AddParameter(New Param_Enum("CoordSys", "Coordinate system", GH_ParamAccess.item, CoordSystemFreeLoad.GCSLength))
            pManager.AddParameter(New Param_Enum("Direction", "Direction of load", GH_ParamAccess.item, Direction.Z))
            pManager.AddParameter(New Param_Enum("Distribution", "Distribution of the surface load", GH_ParamAccess.item, DistributionOfLineLoad.Uniform))
            pManager.AddNumberParameter("LoadValue1", "LoadValue1", "Value of Load in KN/m", GH_ParamAccess.item, -1.0)
            pManager.AddNumberParameter("LoadValue2", "LoadValue2", "Value of Load in KN/m", GH_ParamAccess.item, -1.0)
            pManager.AddCurveParameter("Lines", "Lines", "List of lines", GH_ParamAccess.list)
            pManager.AddNumberParameter("ValidityFrom", "ValidityFrom", "Validity From in m", GH_ParamAccess.item, 0)
            pManager.AddNumberParameter("ValidityTo", "ValidityTo", "Validity To in m", GH_ParamAccess.item, 0)
            'pManager.AddTextParameter("Selected2Dmembers", "Selected2Dmembers", "Selected 2D members as list if Selection is put as Selected", GH_ParamAccess.list, {})
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)

            pManager.AddTextParameter("FreeLineloads", "FreeLineloads", "", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            Dim LoadCase As String = ""
            Dim validity As Validity = Validity.All
            Dim selection As Selection = Selection.Auto
            Dim coordSys As CoordSystemFreeLoad = CoordSystemFreeLoad.GCSLength
            Dim direction As Direction = Direction.Z
            Dim distribution As DistributionOfLineLoad = DistributionOfLineLoad.Uniform
            Dim LoadValue1 As Double = -1.0
            Dim LoadValue2 As Double = -1.0
            Dim Lines = New List(Of Curve)
            Dim i As Integer
            Dim ValidityFrom As Double = 0.0
            Dim ValidityTo As Double = 0.0

            If (Not DA.GetData(0, LoadCase)) Then Return

            If DA.GetData(1, i) Then validity = CType(i, Validity)
            If DA.GetData(2, i) Then selection = CType(i, Selection)
            If DA.GetData(3, i) Then coordSys = CType(i, CoordSystemFreeLoad)
            If DA.GetData(4, i) Then direction = CType(i, Direction)
            If DA.GetData(5, i) Then distribution = CType(i, DistributionOfLineLoad)

            If (Not DA.GetData(6, LoadValue1)) Then Return
            Select Case distribution
                Case DistributionOfLineLoad.Trapez
                    DA.GetData(7, LoadValue2)
                Case Else
                    LoadValue2 = LoadValue1
            End Select
            If (Not DA.GetDataList(Of Curve)(8, Lines)) Then Return
            If (Not DA.GetData(9, ValidityFrom)) Then Return
            If (Not DA.GetData(10, ValidityTo)) Then Return

            Dim j As Long

            Dim SE_flloads(Lines.Count, 11)
            Dim FlatList As New List(Of System.Object)()
            'a free line load consists of: load case, validity, selection, coord. system (GCS/LCS), direction (X, Y, Z), value (kN/m), LineShape

            Dim itemcount As Long
            Dim item As Rhino.Geometry.Curve
            Dim BoundaryShape As String

            'initialize some variables
            itemcount = 0

            'create load data
            '=================

            For Each item In Lines
                BoundaryShape = GetBoundaryShape(item)
                Dim shapeType As String = BoundaryShape.Split(";")(0).ToLower

                If shapeType <> "line" And shapeType <> "polyline" Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Only lines or polylines are supported for free line loads. Different geometries will be skipped.")
                    Continue For
                End If

                SE_flloads(itemcount, 0) = LoadCase
                SE_flloads(itemcount, 1) = GetEnumDescription(Validity)
                SE_flloads(itemcount, 2) = GetEnumDescription(Selection)
                SE_flloads(itemcount, 3) = GetEnumDescription(CoordSys)
                SE_flloads(itemcount, 4) = GetEnumDescription(Direction)
                SE_flloads(itemcount, 5) = GetEnumDescription(Distribution)
                SE_flloads(itemcount, 6) = LoadValue1
                SE_flloads(itemcount, 7) = LoadValue2
                SE_flloads(itemcount, 8) = BoundaryShape
                SE_flloads(itemcount, 9) = ValidityFrom
                SE_flloads(itemcount, 10) = ValidityTo
                itemcount += 1
            Next

            'Flatten data for export as simple list

            FlatList.Clear()

            For i = 0 To itemcount - 1
                For j = 0 To 10
                    FlatList.Add(SE_flloads(i, j))
                Next j
            Next i
            DA.SetDataList(0, FlatList)


        End Sub

        '<Custom additional code> 
        Private Function GetBoundaryShape(ByRef curve As Rhino.Geometry.Curve) As String

            Dim arrPoint As Rhino.Geometry.Point3d
            Dim arrPoints As New Rhino.Collections.Point3dList

            Dim EdgeType As String
            Dim edgenodelist As String

            EdgeType = ""
            edgenodelist = ""

            'Get type of curve and nodelist
            GetTypeAndNodes(curve, EdgeType, arrPoints)
            For Each arrPoint In arrPoints
                If edgenodelist = "" Then
                    edgenodelist = arrPoint.X & ";" & arrPoint.Y & ";" & arrPoint.Z
                Else
                    edgenodelist = edgenodelist & ";" & arrPoint.X & ";" & arrPoint.Y & ";" & arrPoint.Z
                End If
            Next arrPoint

            GetBoundaryShape = EdgeType + ";" + edgenodelist

        End Function

        Private Sub GetTypeAndNodes(ByRef edge As Rhino.Geometry.Curve, ByRef EdgeType As String, ByRef arrPoints As Rhino.Collections.Point3dList)

            If edge.IsArc() Then
                EdgeType = "Arc"
                'convert to arc
                Dim arc As Rhino.Geometry.Arc = Nothing
                edge.TryGetArc(arc)
                arrPoints.Clear()
                arrPoints.Add(arc.StartPoint)
                arrPoints.Add(arc.MidPoint)
                arrPoints.Add(arc.EndPoint)

            ElseIf edge.IsLinear() Then
                EdgeType = "Line"
                arrPoints.Clear()
                arrPoints.Add(edge.PointAtStart)
                arrPoints.Add(edge.PointAtEnd)

            ElseIf edge.IsPolyline() Then
                EdgeType = "Polyline"
                Dim polyline As Rhino.Geometry.Polyline = Nothing
                edge.TryGetPolyline(polyline)
                arrPoints.Clear()
                For Each point In polyline
                    arrPoints.Add(point)
                Next point

            Else
                EdgeType = "Spline"
                'convert to Nurbs curve to get the Edit points
                Dim nurbscurve As Rhino.Geometry.NurbsCurve = edge.ToNurbsCurve
                arrPoints = nurbscurve.GrevillePoints
            End If

        End Sub



        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.FreeLineLoad
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("82294a3a-b741-4dd0-99bb-b02c0365020a")
            End Get
        End Property
    End Class

End Namespace