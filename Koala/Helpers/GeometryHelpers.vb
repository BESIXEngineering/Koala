Partial Module HelperTools
    Private Sub AddToNodeMap(ByRef nameMap As Dictionary(Of String, Node),
                             ByRef nodeList As List(Of Node),
                             name As String,
                             nodeX As Double, nodeY As Double, nodeZ As Double,
                             member1D As String,
                             tolerance As Double,
                             removeDuplicates As Boolean)
        Dim node As New Node With {
            .Name = name,
            .X = nodeX,
            .Y = nodeY,
            .Z = nodeZ,
            .LinkedTo = member1D
        }

        ' If the name of the node is already present, 
        ' either check whether the coordinates of the nodes match (and do nothing)
        ' or throw an error
        If nameMap.ContainsKey(name) Then
            Dim existingNode = nameMap(name)
            If NodesOverlap(existingNode, node, tolerance) Then
                ' Ignore the node in case of a duplicate declaration
                If member1D Is Nothing Or existingNode.LinkedTo = member1D Then
                    Exit Sub
                ElseIf existingNode.LinkedTo Is Nothing Then
                    existingNode.LinkedTo = member1D
                    Exit Sub
                End If
            End If
            Throw New Exception("Model contains duplicate node name " & name)
        End If

        ' Add the node to the name map in order to correct all references to it
        nameMap.Add(name, node)

        ' Remove overlapping nodes, if required
        If removeDuplicates Then
            For Each existingNode As Node In nodeList
                If NodesOverlap(existingNode, node, tolerance) Then
                    If member1D IsNot Nothing Then
                        If existingNode.LinkedTo IsNot Nothing And existingNode.LinkedTo <> member1D Then
                            Throw New Exception("Can't merge nodes linked to different members: " & name & " to " & member1D & ", " & existingNode.Name & " to " & existingNode.LinkedTo)
                        End If
                        existingNode.LinkedTo = member1D
                    End If
                    nameMap(name) = existingNode
                    Rhino.RhinoApp.WriteLine(name & " is duplicate and is removed")
                    ' Don't add the duplicate node to the node list
                    Exit Sub
                End If
            Next
        End If

        nodeList.Add(node)
    End Sub

    Private Function NodesOverlap(p1 As Node, p2 As Node, tolerance As Double) As Boolean
        NodesOverlap = NodesOverlap(p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z, tolerance)
    End Function

    Private Function NodesOverlap(p1 As Rhino.Geometry.Point3d, x2 As Double, y2 As Double, z2 As Double, tolerance As Double) As Boolean
        NodesOverlap = NodesOverlap(p1.X, p1.Y, p1.Z, x2, y2, z2, tolerance)
    End Function

    Private Function NodesOverlap(x1 As Double, y1 As Double, z1 As Double, x2 As Double, y2 As Double, z2 As Double, tolerance As Double) As Boolean
        If tolerance < 0 Then
            NodesOverlap = False
        Else
            NodesOverlap = Math.Abs(x1 - x2) <= tolerance And Math.Abs(y1 - y2) <= tolerance And Math.Abs(z1 - z2) <= tolerance
        End If
    End Function

    Public Function GetExistingNode(arrPoint As Rhino.Geometry.Point3d, nodes As List(Of Koala.SENode), epsilon As Double) As Koala.SENode?
        'Start with node not found, loop through all the nodes until one is found within tolerance
        'Not in use now, as it's quite slow compared to within SCIA Engineer
        GetExistingNode = Nothing
        Rhino.RhinoApp.WriteLine("Searching node")
        For Each node In nodes
            If Math.Abs(arrPoint.X - node.Point.X) < epsilon Then
                If Math.Abs(arrPoint.Y - node.Point.Y) < epsilon Then
                    If Math.Abs(arrPoint.Z - node.Point.Z) < epsilon Then
                        GetExistingNode = node
                        Exit For
                    End If
                End If
            End If
        Next node
    End Function

    Public Function GetExistingNode(arrPoint As Rhino.Geometry.Point3d, nodes(,) As String, nnodes As Long, epsilon As Double)
        Dim currentnode
        'Start with node not found, loop through all the nodes until one is found within tolerance
        'Not in use now, as it's quite slow compared to within SCIA Engineer
        GetExistingNode = -1
        currentnode = 0

        If nnodes Mod 50 = 0 And nnodes > 100 Then
            Rhino.RhinoApp.WriteLine("Searching node " & CStr(nnodes))
            'rhino.Display.DrawEventArgs
        End If

        While GetExistingNode = -1 And currentnode < nnodes
            If NodesOverlap(arrPoint, nodes(currentnode, 1), nodes(currentnode, 2), nodes(currentnode, 3), epsilon) Then
                GetExistingNode = currentnode
            Else
                currentnode += 1
            End If
        End While
    End Function

    Public Sub GetTypeAndNodes(ByRef line As Rhino.Geometry.Curve, ByRef LineType As String, ByRef arrPoints As Rhino.Collections.Point3dList)
        Dim arc As Rhino.Geometry.Arc
        Dim nurbscurve As Rhino.Geometry.NurbsCurve
        Dim i As Integer

        If line.IsCircle Then
            Dim circle As Rhino.Geometry.Circle
            line.TryGetCircle(circle)
            LineType = "Circle"
            arrPoints.Clear()
            Dim PointOnCircle As Rhino.Geometry.Point3d
            PointOnCircle = circle.PointAt(0.0)
            arrPoints.Add(PointOnCircle)
            arrPoints.Add(circle.Center)

        ElseIf line.IsLinear() Then
            LineType = "Line"
            arrPoints.Clear()
            arrPoints.Add(line.PointAtStart)
            arrPoints.Add(line.PointAtEnd)

        ElseIf line.IsPolyline Then
            LineType = "Polyline"
            arrPoints.Clear()
            Dim polyline As Rhino.Geometry.Polyline = Nothing
            line.TryGetPolyline(polyline)
            For i = 0 To polyline.Count - 1
                arrPoints.Add(polyline.ElementAt(i))
            Next i

        ElseIf line.IsArc() Then
            LineType = "Arc"
            'convert to arc
            line.TryGetArc(arc)
            arrPoints.Clear()
            arrPoints.Add(arc.StartPoint)
            arrPoints.Add(arc.MidPoint)
            arrPoints.Add(arc.EndPoint)
            'Dim arc As Rhino.Geometry.Arc

        Else
            LineType = "Spline"
            'convert to Nurbs curve to get the Edit points
            nurbscurve = line.ToNurbsCurve
            arrPoints = nurbscurve.GrevillePoints
        End If
    End Sub
End Module

