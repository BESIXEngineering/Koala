Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry


Namespace Koala

    Public Class Member2D
        Inherits GH_KoalaComponent

        Dim memberIdx As Long = 0
        Dim nodeIdx As Long = 0
        ReadOnly allNodes As New List(Of SENode)()
        ReadOnly stopWatch As New System.Diagnostics.Stopwatch()

        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("2D Member", "2DMember",
                "Create a 2D member. Members and nodes are numbered continuously regardless of the input data tree structure.",
                "Structure", New EsaObjectType() {EsaObjectType.Node, EsaObjectType.Member2D})
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
            pManager.AddBrepParameter("Surface", "Surface", "Member 2D surface", GH_ParamAccess.item)
            pManager.AddTextParameter("Material", "Material", "Material", GH_ParamAccess.item, "C20/25")
            pManager.AddTextParameter("Thickness", "Thickness", "Thickness description comprising the thickness type and thickness value(s) (in mm). Example: constant|100 or variable|Global X|N3;150|N4;300", GH_ParamAccess.item)
            pManager.AddTextParameter("Layer", "Layer", "Layer name", GH_ParamAccess.item, "Surfaces")
            pManager.AddTextParameter("InternalNodes", "InternalNodes", "Names of the Internal Nodes, separated by a semicolon ';'", GH_ParamAccess.item, "")
            pManager.Param(4).Optional = True
            pManager.AddTextParameter("NodePrefix", "NodePrefix", "Node name prefix", GH_ParamAccess.item, "N2D")
            pManager.AddNumberParameter("Tolerance", "Tolerance", "Tolerance for duplicity nodes", GH_ParamAccess.item, 0.001)
            pManager.AddBooleanParameter("RemDuplNodes", "RemDuplNodes", "Set True if you want to remove duplicate nodes", GH_ParamAccess.item, False)
            pManager.AddParameter(New Param_Enum("MemberSystemPlane", "System plane of the member", GH_ParamAccess.item, MemberSystemPlane.Centre))
            pManager.AddNumberParameter("Eccentricity z", "Eccentricity z", "Eccentricity of the plane", GH_ParamAccess.item, 0.0)
            pManager.AddParameter(New Param_Enum("FEM nonlinear model", "FEM type of non-linear model", GH_ParamAccess.item, SlabFEMType.None))
            pManager.AddTextParameter("MemberPrefix", "MemberPrefix", "Member name prefix", GH_ParamAccess.item, "S")
            pManager.AddBooleanParameter("SwapOrientation", "SwapOrientation", "Swap orientation of surface?", GH_ParamAccess.item, False)
            pManager.AddNumberParameter("LCSangle", "LCSangle", "LCS angle [deg]", GH_ParamAccess.item, 0)
            'pManager.AddTextParameter("StructuralType", "StructuralType", "Type of element: Plate, Wall, Shell", GH_ParamAccess.item, "Plate")
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("Nodes", "Nodes", "Output node data", GH_ParamAccess.tree)
            pManager.AddTextParameter("2D Member", "2DMember", "Output 2D member data", GH_ParamAccess.list)
        End Sub


        Protected Overrides Sub BeforeSolveInstance()
            MyBase.BeforeSolveInstance()
            memberIdx = 0
            nodeIdx = 0
            allNodes.Clear()
            'initialize stopwatch
            stopWatch.Start()
        End Sub

        Protected Overrides Sub AfterSolveInstance()
            MyBase.AfterSolveInstance()
            allNodes.Clear()
            'stop stopwatch
            stopWatch.Stop()
            Dim time_elapsed As Double = stopWatch.ElapsedMilliseconds
            Rhino.RhinoApp.WriteLine("Koala2DMembers: Done in " + Str(time_elapsed) + " ms.")
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            Dim brep As Brep = Nothing
            Dim material As String = "C20/25"
            Dim thickness As String = ""
            Dim surfLayer As String = "Surfaces"
            Dim internalNodes As String = ""
            Dim nodePrefix As String = "NS"
            Dim tolerance As Double = 0.001
            Dim remDuplNodes As Boolean = False
            Dim memberPlane As MemberSystemPlane = MemberSystemPlane.Centre
            Dim eccentricityZ As Double = 0.0
            Dim FEMNLType As SlabFEMType = SlabFEMType.None
            Dim memberNamePrefix As String = "S"
            Dim swapOrientation As Boolean = False
            Dim angleLCS As Double = 0.0

            Dim i As Integer = 0

            If (Not DA.GetData(0, brep)) Then Return
            If (Not DA.GetData(1, material)) Then Return
            If (Not DA.GetData(2, thickness)) Then Return
            If (Not DA.GetData(3, surfLayer)) Then Return
            DA.GetData(4, internalNodes)
            If (Not DA.GetData(5, nodePrefix)) Then Return
            If (Not DA.GetData(6, tolerance)) Then Return
            If (Not DA.GetData(7, remDuplNodes)) Then Return
            If DA.GetData(8, i) Then memberPlane = CType(i, MemberSystemPlane)
            If (Not DA.GetData(9, eccentricityZ)) Then Return
            If DA.GetData(10, i) Then FEMNLType = CType(i, SlabFEMType)
            If (Not DA.GetData(11, memberNamePrefix)) Then Return
            If (Not DA.GetData(12, swapOrientation)) Then Return
            If (Not DA.GetData(13, angleLCS)) Then Return

            angleLCS *= Math.PI / 180

            ' Initialize output
            Dim SE_NodeTree As New GH_Structure(Of GH_String)()
            Dim SE_member(11) As String

            Dim basePath As GH_Path = DA.ParameterTargetPath(0)

            'check if the surface can be handled to SCIA Engineer: non-planar surfaces may have a maximum of 4 boundary edges
            Dim warnings As New List(Of String)
            Dim boundary As Rhino.Geometry.Curve = GetBrepBoundary(brep)
            Dim segments() As Rhino.Geometry.Curve = GetBoundarySegments(boundary, tolerance, warnings)

            If warnings.Any Then
                For Each w In warnings
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, w)
                Next w
            End If

            ' Dim fullSurf As Rhino.Geometry.Surface = brep.Faces(0)
            Dim surfType As String
            Dim isPlanar As Boolean = boundary.IsPlanar(tolerance) ' (fullSurf.IsPlanar(tolerance))
            If segments.Count <= 4 And Not isPlanar Then
                'shells, max 4 edges
                surfType = "Shell"
            ElseIf isPlanar Then
                'plates
                surfType = "Plate"
            Else
                'not supported: shell with more than 4 edges
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Non-planar surface with " & segments.Count & " (>4) edges is not supported: will be skipped.
Tip: subdivide the brep into individual faces (with max. 4 edges per face)")
                Return
            End If

            memberIdx += 1
            Dim memberName As String = String.Format("{0}{1}", memberNamePrefix, memberIdx)

            SE_member(0) = memberName
            SE_member(1) = surfType
            SE_member(2) = material
            SE_member(3) = thickness
            SE_member(4) = surfLayer
            SE_member(5) = ProcessSurfaceBoundary(segments, remDuplNodes, tolerance, allNodes, nodeIdx, nodePrefix, basePath, SE_NodeTree)
            SE_member(6) = internalNodes
            SE_member(7) = GetEnumDescription(memberPlane)
            SE_member(8) = eccentricityZ
            SE_member(9) = GetEnumDescription(FEMNLType)
            SE_member(10) = If(swapOrientation, "1", "0")
            SE_member(11) = angleLCS

            DA.SetDataTree(0, SE_NodeTree)
            DA.SetDataList(1, SE_member)
        End Sub

        Public Shared Function GetBrepBoundary(ByVal brep As Brep) As Rhino.Geometry.Curve
            'check if the surface can be handled to SCIA Engineer: non-planar surfaces may have a maximum of 4 boundary edges
            'get surface naked edges = unsorted list of edges
            Dim nakededges() As Rhino.Geometry.Curve = brep.DuplicateNakedEdgeCurves(True, False)
            'join all, take the first curve and explode back to segment; this should now be properly sorted
            GetBrepBoundary = Rhino.Geometry.Curve.JoinCurves(nakededges)(0)
        End Function

        Public Shared Function GetBoundarySegments(ByVal boundary As Curve, ByVal tolerance As Double, ByRef warnings As List(Of String)) As Rhino.Geometry.Curve()
            Dim segments() As Rhino.Geometry.Curve

            If boundary.IsCircle() Then
                ReDim Preserve segments(0)
                segments.SetValue(boundary, 0)
            Else
                segments = boundary.DuplicateSegments
            End If

            ' Remove too short edges
            Dim segmentCount As Integer = segments.Length
            segments = (From s In segments
                        Where s.GetLength() > tolerance).ToArray()

            If segmentCount > segments.Length Then
                warnings.Add("Skipping edge shorter than tolerance")
            End If
            GetBoundarySegments = segments
        End Function

        Public Shared Function ProcessSurfaceBoundary(ByVal segments As Rhino.Geometry.Curve(),
                                               ByVal remDuplNodes As Boolean,
                                               ByVal tolerance As Double,
                                               ByRef allNodes As List(Of SENode),
                                               ByRef nodeIdx As Long,
                                               ByVal nodePrefix As String,
                                               ByVal basePath As GH_Path,
                                               ByRef SE_NodeTree As GH_Structure(Of GH_String)) As String

            Dim edge As Rhino.Geometry.Curve
            Dim arrPoint As Rhino.Geometry.Point3d
            Dim arrPoints As New Rhino.Collections.Point3dList
            Dim edgeNodeNames As String

            Dim inode As Long
            Dim firstNode As SENode? = Nothing
            Dim closedSurface As Boolean = False
            Dim boundaryShape As String = ""
            Dim lastNodeName As String = ""

            For Each edge In segments

                'Get type of curve segments and nodelist
                Dim edgeType As String = ""
                GetTypeAndNodes(edge, edgeType, arrPoints)

                edgeNodeNames = ""
                inode = 0

                For Each arrPoint In arrPoints
                    inode += 1

                    'for edges 2 and beyond, skip the first node
                    If lastNodeName <> "" And inode = 1 Then
                        edgeNodeNames = lastNodeName
                    Else
                        'check if the surface is closed: the new point is identical to the first node
                        If firstNode IsNot Nothing AndAlso arrPoint.DistanceToSquared(firstNode.Value.Point) < tolerance * tolerance Then
                            closedSurface = True
                            Exit For
                        Else
                            Dim nodePath As GH_Path = basePath.AppendElement(nodeIdx)
                            Dim currentnode As SENode? = Nothing

                            'check if a node already exists at these coordinates
                            If remDuplNodes Then
                                currentnode = GetExistingNode(arrPoint, allNodes, tolerance)
                            End If

                            If currentnode Is Nothing Then
                                nodeIdx += 1
                                currentnode = New SENode With {
                                    .Name = String.Format("{0}{1}", nodePrefix, nodeIdx),
                                    .Point = arrPoint
                                }
                                allNodes.Add(currentnode.Value)
                                SE_NodeTree.Append(New GH_String(currentnode.Value.Name), nodePath)
                                SE_NodeTree.Append(New GH_String(currentnode.Value.Point.X), nodePath)
                                SE_NodeTree.Append(New GH_String(currentnode.Value.Point.Y), nodePath)
                                SE_NodeTree.Append(New GH_String(currentnode.Value.Point.Z), nodePath)
                                'store the position of the first node to later check if the surface is closed
                                If firstNode Is Nothing Then firstNode = currentnode
                            End If

                            'in case of closed curve for SCIA Engineer > don't add the last point
                            If edgeNodeNames = "" Then
                                edgeNodeNames = currentnode.Value.Name
                            ElseIf inode < arrPoints.Count OrElse (inode = arrPoints.Count And edgeType = "Circle") Then
                                edgeNodeNames = edgeNodeNames & ";" & currentnode.Value.Name
                            End If

                            lastNodeName = currentnode.Value.Name
                        End If
                    End If
                Next arrPoint

                'add edge information to the BoundaryShape string
                If boundaryShape = "" Then
                    If edgeType = "Circle" Then
                        Dim circle As Rhino.Geometry.Circle
                        edge.TryGetCircle(circle)
                        boundaryShape = edgeType + ";" + edgeNodeNames + ";" + "[" + (circle.Center.X + circle.Normal.X).ToString() + "," + (circle.Center.Y + circle.Normal.Y).ToString() + "," + (circle.Center.Z + circle.Normal.Z).ToString() + "]"
                    Else
                        boundaryShape = edgeType + ";" + edgeNodeNames
                    End If
                Else
                    boundaryShape = boundaryShape + " | " + edgeType + ";" + edgeNodeNames
                End If

                'don't go to next edge if the surface is closed
                If closedSurface Then Exit For
            Next edge

            ProcessSurfaceBoundary = boundaryShape
        End Function

        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                Return My.Resources._2DMember
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("241d3b0f-3cf5-4ba4-9ebb-9cc86888baf8")
            End Get
        End Property
    End Class

End Namespace