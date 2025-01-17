Imports System.Collections.Generic
Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala
    ''' <summary>
    ''' Improved implementation of the FreeSurfaceLoad component to include the option to set a non-uniform distribution with 3 load values.
    ''' </summary>
    Public Class FreeSurfaceLoad3
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
                "FreeSurfaceLoad with option for distributed load",
                "Load", New EsaObjectType() {EsaObjectType.FreeSurfaceLoad})
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
            pManager.AddParameter(New Param_Enum("Distribution", "Distribution of the surface load", GH_ParamAccess.item, DistributionOfSurfaceLoad.Uniform))

            pManager.AddNumberParameter("LoadValue1", "LoadValue1", "Value of Load in KN/m", GH_ParamAccess.item, -1)
            pManager.AddNumberParameter("LoadValue2", "LoadValue2", "Value of second Load in KN/m (if not uniform distribution)", GH_ParamAccess.item, -1)
            pManager.AddNumberParameter("LoadValue3", "LoadValue3", "Value of third Load in KN/m (if 3 point distribution)", GH_ParamAccess.item, -1)

            pManager.AddCurveParameter("Boundary", "Boundary", "Boundary curve", GH_ParamAccess.item)
            pManager.AddNumberParameter("ValidityFrom", "ValidityFrom", "Validity From in m", GH_ParamAccess.item, 0)
            pManager.AddNumberParameter("ValidityTo", "ValidityTo", "Validity To in m", GH_ParamAccess.item, 0)

            pManager.AddTextParameter("Members2D", "Members2D", "If Selection = Select, give the selected 2D members in format Name;TYPE;Id separated by '|'. 
TYPE can for the moment only be SURFACE. Xml update unfortunately only works for the member Id (the object Id of the member in the Xml export). 
Example: 'S1;SURFACE;1 | S2;SURFACE;4'", GH_ParamAccess.item, "")
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("FreeSurfaceLoad", "FreeSurfaceLoad", "FreeSurfaceLoad data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            'note: only straight segments are supported in SCIA Engineer's XML today (SE 18.1.3035) > limitation of Koala as well
            Dim LoadCase As String = "LC1"
            Dim LoadValue1 As Double = -1.0
            Dim LoadValue2 As Double = -1.0
            Dim LoadValue3 As Double = -1.0
            Dim LoadValueString As String

            Dim boundary As Curve = Nothing
            Dim i As Integer

            Dim ValidityFrom As Double = 0.0
            Dim ValidityTo As Double = 0.0

            Dim selected2DMembers As String = ""

            If (Not DA.GetData(0, LoadCase)) Then Return
            If (Not DA.GetData(1, i)) Then Return
            Dim validity As Validity = CType(i, Validity)
            If (Not DA.GetData(2, i)) Then Return
            Dim selection As Selection = CType(i, Selection)
            If (Not DA.GetData(3, i)) Then Return
            Dim coordSys As CoordSystemFreeLoad = CType(i, CoordSystemFreeLoad)
            If (Not DA.GetData(4, i)) Then Return
            Dim direction As Direction = CType(i, Direction)
            If (Not DA.GetData(5, i)) Then Return
            Dim distribution As DistributionOfSurfaceLoad = CType(i, DistributionOfSurfaceLoad)

            If (Not DA.GetData(6, LoadValue1)) Then Return
            Select Case distribution
                Case DistributionOfSurfaceLoad.DirectionX, DistributionOfSurfaceLoad.DirectionY
                    DA.GetData(7, LoadValue2)
                    LoadValueString = $"{LoadValue1};{LoadValue2}"
                Case DistributionOfSurfaceLoad.ThreePoints
                    DA.GetData(7, LoadValue2)
                    DA.GetData(8, LoadValue3)
                    LoadValueString = $"{LoadValue1};{LoadValue2};{LoadValue3}"
                Case Else
                    LoadValueString = $"{LoadValue1}"
            End Select

            If Not DA.GetData(9, boundary) Then Return
            If (Not DA.GetData(10, ValidityFrom)) Then Return
            If (Not DA.GetData(11, ValidityTo)) Then Return

            If (Not DA.GetData(12, selected2DMembers)) Then Return

            Dim SE_fsloads(11)
            'a free surface load consists of: load case, validity, selection, coord. system (GCS/LCS), direction (X, Y, Z), distribution (uniform | dirX | dirY), 1 to 3 values (kN/m^2), BoundaryShape

            Dim segments() As Rhino.Geometry.Curve
            Dim segment As Rhino.Geometry.Curve

            Dim BoundaryShape As String, LineShape As String

            'create load data
            '=================

            segments = boundary.DuplicateSegments

            BoundaryShape = ""

            For Each segment In segments

                'Get full geometry description
                LineShape = GetClosedBoundaryShape(segment)

                If LineShape.Split(";")(0) <> "Line" Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
                                      "Only straight line segments are supported for free surface loads. Different geometries will be skipped.")
                    Continue For
                End If

                If BoundaryShape = "" Then
                    BoundaryShape = LineShape
                Else
                    BoundaryShape = BoundaryShape & " | " & LineShape
                End If

            Next segment

            ' Process the selected Member2D input.
            ' Should be format Name;TYPE;Id
            ' Example: S1;SURFACE;1  or  O2;OPENENING;2
            ' Xml update only works for the member Id (the object Id of the member in the Xml export), not for the member name (which is hence redundant)
            Dim selectedMemberString As String = ""
            Try
                If Not String.IsNullOrEmpty(selected2DMembers) Then
                    Dim memberStrings As List(Of String) = New List(Of String)

                    For Each m In Split(selected2DMembers, "|")
                        If (String.IsNullOrWhiteSpace(m)) Then
                            Continue For
                        End If
                        Dim parts As String() = Split(m, ";")
                        ' Assume that only the name is given
                        Dim memberString As String = ""

                        If parts.Length = 1 Then
                            parts(0) = parts(0).Trim
                            Dim id As Integer = Math.Max(1, CInt(parts(0).Substring(1)))
                            memberString = String.Format("{0};SURFACE;{1}", parts(0), id)
                            'If parts(0).StartsWith("o", StringComparison.OrdinalIgnoreCase) Then
                            '    memberString = String.Format("{0};OPENING;{1}", parts(0), id)
                            'Else
                            '    memberString = String.Format("{0};SURFACE;{1}", parts(0), id)
                            'End If
                        ElseIf parts.Length = 3 Then
                            Select Case parts(1).ToLower.Trim
                                'Case "opening"
                                '    memberString = String.Format("{0};OPENING;{1}", parts(0).Trim, CInt(parts(2)))
                                Case Else
                                    memberString = String.Format("{0};SURFACE;{1}", parts(0).Trim, CInt(parts(2)))
                            End Select
                        Else
                            Throw New ArgumentException("Invalid member string format; must be in format of 'Name;SURFACE;Id'")
                        End If
                        memberStrings.Add(memberString)
                    Next
                    selectedMemberString = String.Join(" | ", memberStrings)
                End If

            Catch ex As Exception
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid member string: " & ex.Message)
                Return
            End Try

            SE_fsloads(0) = LoadCase
            SE_fsloads(1) = GetEnumDescription(validity)
            SE_fsloads(2) = GetEnumDescription(selection)
            SE_fsloads(3) = GetEnumDescription(coordSys)
            SE_fsloads(4) = GetEnumDescription(direction)
            SE_fsloads(5) = GetEnumDescription(distribution)
            SE_fsloads(6) = LoadValueString
            SE_fsloads(7) = ""
            SE_fsloads(8) = BoundaryShape
            SE_fsloads(9) = ValidityFrom
            SE_fsloads(10) = ValidityTo
            SE_fsloads(11) = selectedMemberString

            DA.SetDataList(0, SE_fsloads)

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
                Return New Guid("9499bfdb-1ab4-4202-95a8-bb73d18a3a91")
            End Get
        End Property
    End Class

End Namespace