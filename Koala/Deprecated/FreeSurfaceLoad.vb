Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala
    <System.Obsolete("Obsolete. Use version 3 instead which supports a non-uniform load distribution.")>
    Public Class FreeSurfaceLoad
        Inherits GH_KoalaComponent
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("FreeSurfaceLoad", "FreeSurfaceLoad",
                "FreeSurfaceLoad description",
                "Load", New EsaObjectType() {EsaObjectType.FreeSurfaceLoad})
        End Sub

        Public Overrides ReadOnly Property Exposure As GH_Exposure
            Get
                Return GH_Exposure.hidden
            End Get
        End Property

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddTextParameter("LoadCase", "LoadCase", "Name of load case", GH_ParamAccess.item, "LC2")
            pManager.AddParameter(New Param_Enum("Validity", "Validity", GH_ParamAccess.item, Validity.All))
            pManager.AddParameter(New Param_Enum("Selection", "Selection", GH_ParamAccess.item, Selection.Auto))
            pManager.AddParameter(New Param_Enum("CoordSys", "Coordinate system", GH_ParamAccess.item, CoordSystemFreeLoad.GCSLength))
            pManager.AddParameter(New Param_Enum("Direction", "Direction of load", GH_ParamAccess.item, Direction.Z))
            pManager.AddNumberParameter("LoadValue", "LoadValue", "Value of Load in KN/m", GH_ParamAccess.item, -1)
            pManager.AddCurveParameter("Boundaries", "Boundaries", "List of lines", GH_ParamAccess.list)
            pManager.AddNumberParameter("ValidityFrom", "ValidityFrom", "Validity From in m", GH_ParamAccess.item, 0)
            pManager.AddNumberParameter("ValidityTo", "ValidityTo", "Validity To in m", GH_ParamAccess.item, 0)
            'pManager.AddTextParameter("Selected2Dmembers", "Selected2Dmembers", "Selected 2D members as list if Selection is put as Selected", GH_ParamAccess.list, {})
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("FreeSurfaceloads", "FreeSurfaceloads", "", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            'note: only straight segments are supported in SCIA Engineer's XML today (SE 18.1.3035) > limitation of Koala as well

            Dim LoadCase As String = "LC1"
            Dim LoadValue As Double = -1.0
            Dim Boundaries = New List(Of Curve)
            Dim i As Integer


            Dim ValidityFrom As Double = 0.0
            Dim ValidityTo As Double = 0.0

            If (Not DA.GetData(0, LoadCase)) Then Return
            If (Not DA.GetData(1, i)) Then Return
            Dim validity As Validity = CType(i, Validity)
            If (Not DA.GetData(2, i)) Then Return
            Dim selection As Selection = CType(i, Selection)
            If (Not DA.GetData(3, i)) Then Return
            Dim coordSys As CoordSystemFreeLoad = CType(i, CoordSystemFreeLoad)
            If (Not DA.GetData(4, i)) Then Return
            Dim direction As Direction = CType(i, Direction)
            If (Not DA.GetData(5, LoadValue)) Then Return
            If (Not DA.GetDataList(Of Curve)(6, Boundaries)) Then Return
            If (Not DA.GetData(7, ValidityFrom)) Then Return
            If (Not DA.GetData(8, ValidityTo)) Then Return

            Dim j As Long

            Dim SE_fsloads(Boundaries.Count, 11)
            Dim FlatList As New List(Of System.Object)()
            'a free surface load consists of: load case, validity, selection, coord. system (GCS/LCS), direction (X, Y, Z), value (kN/m^2), BoundaryShape

            Dim itemcount As Long
            Dim item As Rhino.Geometry.Curve
            Dim segments() As Rhino.Geometry.Curve
            Dim segment As Rhino.Geometry.Curve

            Dim BoundaryShape As String, LineShape As String

            'initialize some variables
            itemcount = 0

            'create load data
            '=================

            For Each item In Boundaries

                segments = item.DuplicateSegments

                BoundaryShape = ""

                For Each segment In segments

                    'Get full geometry description
                    LineShape = GetClosedBoundaryShape(segment)

                    If LineShape.Split(";")(0) <> "Line" Then
                        Rhino.RhinoApp.WriteLine("KOALA: only straight line segments are supported for free line loads. Different geometries will be skipped.")
                        Continue For
                    End If

                    If BoundaryShape = "" Then
                        BoundaryShape = LineShape
                    Else
                        BoundaryShape = BoundaryShape & " | " & LineShape
                    End If

                Next segment

                SE_fsloads(itemcount, 0) = LoadCase
                SE_fsloads(itemcount, 1) = GetEnumDescription(validity)
                SE_fsloads(itemcount, 2) = GetEnumDescription(selection)
                SE_fsloads(itemcount, 3) = GetEnumDescription(coordSys)
                SE_fsloads(itemcount, 4) = GetEnumDescription(direction)
                SE_fsloads(itemcount, 5) = "Uniform"
                SE_fsloads(itemcount, 6) = LoadValue
                SE_fsloads(itemcount, 7) = LoadValue
                SE_fsloads(itemcount, 8) = BoundaryShape
                SE_fsloads(itemcount, 9) = ValidityFrom
                SE_fsloads(itemcount, 10) = ValidityTo
                SE_fsloads(itemcount, 11) = ""
                itemcount += 1
            Next

            'Flatten data for export as simple list

            FlatList.Clear()

            For i = 0 To itemcount - 1
                For j = 0 To 11
                    FlatList.Add(SE_fsloads(i, j))
                Next j
            Next i



            DA.SetDataList(0, FlatList)


        End Sub

        '<Custom additional code> 
        Private Function GetClosedBoundaryShape(ByRef curve As Rhino.Geometry.Curve) As String

            Dim arrPoint As Rhino.Geometry.Point3d
            Dim arrPoints As New Rhino.Collections.Point3dList

            Dim EdgeType As String
            Dim edgenodelist As String

            Dim i As Long

            EdgeType = ""
            edgenodelist = ""

            'Get type of curve and nodelist
            GetTypeAndNodes(curve, EdgeType, arrPoints)
            i = 0
            For Each arrPoint In arrPoints
                'don't add the last point to the list!
                If i >= arrPoints.Count - 1 Then
                    Exit For
                End If

                If edgenodelist = "" Then
                    edgenodelist = arrPoint.X & ";" & arrPoint.Y & ";" & arrPoint.Z
                Else
                    edgenodelist = edgenodelist & ";" & arrPoint.X & ";" & arrPoint.Y & ";" & arrPoint.Z
                End If

                i += 1

            Next arrPoint

            GetClosedBoundaryShape = EdgeType + ";" + edgenodelist

        End Function

        Private Sub GetTypeAndNodes(ByRef edge As Rhino.Geometry.Curve, ByRef EdgeType As String, ByRef arrPoints As Rhino.Collections.Point3dList)

            Dim arc As Rhino.Geometry.Arc
            Dim nurbscurve As Rhino.Geometry.NurbsCurve

            If edge.IsArc() Then
                EdgeType = "Arc"
                'convert to arc
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
            Else
                EdgeType = "Spline"
                'convert to Nurbs curve to get the Edit points
                nurbscurve = edge.ToNurbsCurve
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
                Return My.Resources.FreeSurfaceLoad

            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("8bb2139d-3082-480f-8c0e-de049cc89821")
            End Get
        End Property
    End Class

End Namespace