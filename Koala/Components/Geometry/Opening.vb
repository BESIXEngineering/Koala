Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry


Namespace Koala

    Public Class Opening
        Inherits GH_KoalaComponent

        Dim nodeIdx As Long = 0
        ReadOnly allNodes As New List(Of SENode)()

        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("Opening", "Opening",
                "Create an opening in a 2D member. Members and nodes are numbered continuously regardless of the input data tree structure.",
                "Structure", New EsaObjectType() {EsaObjectType.Node, EsaObjectType.Opening})
        End Sub

        Public Overrides ReadOnly Property Exposure As GH_Exposure
            Get
                Return GH_Exposure.primary
            End Get
        End Property

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddCurveParameter("ClosedCurve", "ClosedCurve", "Closed curve defining opening", GH_ParamAccess.item)
            pManager.AddTextParameter("Surface", "Surface", "Name of surface in which to create to opening", GH_ParamAccess.item)
            pManager.AddTextParameter("NodePrefix", "NodePrefix", "Prefix of nodes defining opening", GH_ParamAccess.item, "NO")
            pManager.AddNumberParameter("Tolerance", "Tolerance", "tolerance for geometry check", GH_ParamAccess.item, 0.001)
            pManager.AddTextParameter("OpeningPrefix", "OpeningPrefix", "Prefix of the name of the opening", GH_ParamAccess.item, "O")
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("Nodes", "Nodes", "Output node data", GH_ParamAccess.tree)
            pManager.AddTextParameter("Opening", "Opening", "Output opening data", GH_ParamAccess.list)
        End Sub

        Protected Overrides Sub BeforeSolveInstance()
            MyBase.BeforeSolveInstance()
            nodeIdx = 0
            allNodes.Clear()
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            Dim inputCurve As Curve = Nothing
            Dim surfaceName As String = ""
            Dim nodePrefix As String = "NO"
            Dim tolerance As Double = 0.001
            Dim memberNamePrefix As String = "O"

            If (Not DA.GetData(0, inputCurve)) Then Return
            If (Not DA.GetData(1, surfaceName)) Then Return
            If (Not DA.GetData(2, nodePrefix)) Then Return
            If (Not DA.GetData(3, tolerance)) Then Return
            If (Not DA.GetData(4, memberNamePrefix)) Then Return

            Dim SE_NodeTree As New GH_Structure(Of GH_String)()
            Dim SE_member(2) As String

            Dim basePath As GH_Path = DA.ParameterTargetPath(0)

            ' set member name and surface
            NameIndex += 1
            Dim memberName As String = String.Format("{0}{1}", memberNamePrefix, NameIndex)
            SE_member(0) = memberName
            SE_member(1) = surfaceName

            'check if the curve is a closed, planar curve
            If Not inputCurve.IsClosed Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid curve: curve is not closed")
                Return
            End If
            If Not inputCurve.IsPlanar(tolerance) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid curve: curve is not planar")
                Return
            End If

            Dim segment As Rhino.Geometry.Curve
            Dim segments() As Rhino.Geometry.Curve
            Dim segmentCount As Long
            Dim arrPoints As New Rhino.Collections.Point3dList

            Dim boundaryShape As String = ""

            'get all curve segments
            If inputCurve.IsCircle() Then
                ReDim Preserve segments(0)
                segments.SetValue(inputCurve, 0)
                segmentCount = 1
            Else
                segments = inputCurve.DuplicateSegments
                segmentCount = segments.Count
            End If

            For i As Integer = 0 To segmentCount - 1
                segment = segments(i)

                If segment.GetLength() < tolerance Then
                    Continue For 'iterate to next edge
                End If

                'Get type of curve and nodelist
                Dim edgeType As String = ""
                GetTypeAndNodes(segment, edgeType, arrPoints)

                Dim edgeNodeNames As String = ""

                For j As Integer = 0 To arrPoints.Count - 2 ' skip last point because SCIA Engineer will automatically go the the first point of the next edge, or close the curve

                    Dim nodePath As GH_Path = basePath.AppendElement(nodeIdx)

                    nodeIdx += 1
                    Dim nodeName = String.Format("{0}{1}", nodePrefix, nodeIdx)

                    'create the node, then add it to the edge information
                    Dim node As New SENode With {
                                            .Name = nodeName,
                                            .Point = arrPoints(j)
                                         }
                    allNodes.Add(node)
                    SE_NodeTree.Append(New GH_String(node.Name), nodePath)
                    SE_NodeTree.Append(New GH_String(node.Point.X), nodePath)
                    SE_NodeTree.Append(New GH_String(node.Point.Y), nodePath)
                    SE_NodeTree.Append(New GH_String(node.Point.Z), nodePath)

                    'in case of closed curve for SCIA Engineer > don't add the last point
                    If edgeNodeNames = "" Then
                        edgeNodeNames = nodeName
                        'ElseIf inode < arrPoints.Count OrElse (inode = arrPoints.Count And edgeType = "Circle") Then
                        '    edgeNodeNames = edgeNodeNames & ";" & currentnode.Value.Name
                    Else
                        edgeNodeNames = edgeNodeNames & ";" & nodeName
                    End If
                Next j

                'add edge information to the BoundaryShape string
                If boundaryShape = "" Then
                    boundaryShape = edgeType + ";" + edgeNodeNames
                Else
                    boundaryShape = boundaryShape + " | " + edgeType + ";" + edgeNodeNames
                End If
            Next i

            'store the edge description in the array
            SE_member(2) = boundaryShape

            DA.SetDataTree(0, SE_NodeTree)
            DA.SetDataList(1, SE_member)
        End Sub

        '<Custom additional code> 
        Private Sub GetTypeAndNodes(ByRef edge As Rhino.Geometry.Curve, ByRef EdgeType As String, ByRef arrPoints As Rhino.Collections.Point3dList)

            Dim arc As Rhino.Geometry.Arc
            Dim circle As Rhino.Geometry.Circle
            Dim nurbscurve As Rhino.Geometry.NurbsCurve

            If edge.IsCircle() Then 'Opening defined in XML as circle by 3 points
                EdgeType = "Circle"
                edge.TryGetCircle(circle)
                arrPoints.Clear()
                arrPoints.Add(circle.PointAt(0))
                arrPoints.Add(circle.PointAt(Math.PI / 2))
                arrPoints.Add(circle.PointAt(Math.PI))
                arrPoints.Add(circle.PointAt(0)) ' close the circle, last point will be ignored

            ElseIf edge.IsArc() Then
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
                Return My.Resources.Opening
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("066f1144-e421-4a47-ba9b-af0ab4ba82b8")
            End Get
        End Property
    End Class

End Namespace