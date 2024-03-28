Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry


Namespace Koala

    Public Class InternalEdge2D
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
            MyBase.New("Internal Edge on 2D Member", "InternalEdgeOn2DMember",
                "Create an internal edge on a 2D member. Members and nodes are numbered continuously regardless of the input data tree structure.",
                "Structure", New EsaObjectType() {EsaObjectType.Node, EsaObjectType.InternalEdge2D})
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
            pManager.AddCurveParameter("Line", "Line", "Line on the 2D member", GH_ParamAccess.item)
            pManager.AddTextParameter("MemberName", "MemberName", "Name of the 2D member where to put internal edge", GH_ParamAccess.item)
            pManager.AddTextParameter("EdgePrefix", "EdgePrefix", "Internal Edge Name Prefix", GH_ParamAccess.item, "ES")
            pManager.AddTextParameter("NodePrefix", "NodePrefix", "Node prefix", GH_ParamAccess.item, "NEN")
            pManager.AddNumberParameter("Tolerance", "Tolerance", "Tolerance for duplicity nodes", GH_ParamAccess.item, 0.001)
            pManager.AddBooleanParameter("RemDuplNodes", "RemDuplNodes", "Set True if you want to remove duplicate nodes", GH_ParamAccess.item, False)
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("Nodes", "Nodes", "Output node data", GH_ParamAccess.tree)
            pManager.AddTextParameter("Internal Edge", "InternalEdge2D", "InternalEdge2D output data", GH_ParamAccess.list)
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

            Dim line As Curve = Nothing
            Dim surfaceName As String = ""
            Dim edgePrefix As String = "ES"
            Dim nodePrefix As String = "NES"
            Dim tolerance As Double
            Dim remDuplNodes As Boolean

            If Not DA.GetData(0, line) Then Return
            If Not DA.GetData(1, surfaceName) Then Return
            If Not DA.GetData(2, edgePrefix) Then Return
            If Not DA.GetData(3, nodePrefix) Then Return
            If Not DA.GetData(4, tolerance) Then Return
            If Not DA.GetData(5, remDuplNodes) Then Return

            Dim SE_NodeTree As New GH_Structure(Of GH_String)()
            Dim SE_member(2) As String

            Dim arrPoints As New Rhino.Collections.Point3dList
            Dim lineShape As String, lineType As String = "Line"

            'extract geometry from the curve
            GetTypeAndNodes(line, lineType, arrPoints)
            lineShape = lineType

            NameIndex += 1
            Dim memberName As String = String.Format("{0}{1}", edgePrefix, NameIndex)
            SE_member(0) = memberName
            SE_member(1) = surfaceName

            Dim basePath As GH_Path = DA.ParameterTargetPath(0)

            For Each arrPoint In arrPoints

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
                End If

                'add the node to the line shape
                lineShape = lineShape & ";" & currentnode.Value.Name

            Next arrPoint

            SE_member(2) = lineShape

            DA.SetDataTree(0, SE_NodeTree)
            DA.SetDataList(1, SE_member)
        End Sub


        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.InternalSlab
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("53bff1f3-590e-4baa-9dff-20ab1c42da42")
            End Get
        End Property
    End Class

End Namespace