Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala
    Public Structure SENode
        'a node consists of: Name, X, Y, Z
        Dim Point As Rhino.Geometry.Point3d
        Dim Name As String
    End Structure

    Public Class Member0D
        Inherits GH_KoalaComponent

        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("Node", "Node",
                "Create a point. Nodes are numbered continuously regardless of the input data tree structure.",
                "Structure", New EsaObjectType() {EsaObjectType.Node})
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
            pManager.AddPointParameter("Point", "Point", "Point", GH_ParamAccess.item)
            pManager.AddTextParameter("NodePrefix", "NodePrefix", "Prefix for nodes", GH_ParamAccess.item, "N")
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("Node", "Node", "Output node data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
            Dim point As Point3d
            Dim nodePrefix As String = "N"

            If (Not DA.GetData(0, point)) Then Return
            If (Not DA.GetData(1, nodePrefix)) Then Return

            Dim SE_member(3) As String 'a node consists of: Name, X, Y, Z

            NameIndex += 1
            Dim memberName As String = String.Format("{0}{1}", nodePrefix, NameIndex)

            SE_member(0) = memberName
            SE_member(1) = point.X
            SE_member(2) = point.Y
            SE_member(3) = point.Z

            DA.SetDataList(0, SE_member)
        End Sub


        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.Node

            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("b03f4962-1eb8-40df-93cd-a662b2c4761f")
            End Get
        End Property
    End Class

End Namespace