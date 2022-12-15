Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry


Namespace Koala

    Public Class LoadPanel
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
            MyBase.New("Load Panel", "LoadPanel",
                "Create a 2D load panel. Members and nodes are numbered continuously regardless of the input data tree structure.",
                "Structure", New EsaObjectType() {EsaObjectType.Node, EsaObjectType.LoadPanel})
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
            pManager.AddBrepParameter("Surface", "Surface", "Load panel surface", GH_ParamAccess.item)
            pManager.AddTextParameter("Layer", "Layer", "Layer name", GH_ParamAccess.item, "LoadPanels")
            pManager.AddParameter(New Param_Enum("PanelType", "Load panel type", GH_ParamAccess.item, LoadPanelType.ToPanelNode))
            pManager.AddParameter(New Param_Enum("TransferDirection", "Load panel transfer direction", GH_ParamAccess.item, LoadPanelTransferDirection.Both))
            'pManager.AddTextParameter("TransferDirectionRatio", "TransferDirectionRatio", "Transfer Direction Ratio X|Y e.g. 50|50", GH_ParamAccess.item, "50|50")
            pManager.AddParameter(New Param_Enum("TransferMethod", "Load panel transfer method", GH_ParamAccess.item, LoadPanelTransferMethod.Standard))
            pManager.AddTextParameter("NodePrefix", "NodePrefix", "Node name prefix", GH_ParamAccess.item, "LPN")
            pManager.AddNumberParameter("Tolerance", "Tolerance", "Tolerance for duplicity nodes", GH_ParamAccess.item, 0.001)
            pManager.AddBooleanParameter("RemDuplNodes", "RemDuplNodes", "Set True if you want to remove duplicate nodes", GH_ParamAccess.item, False)
            pManager.AddTextParameter("MemberPrefix", "MemberPrefix", "Load panel name prefix", GH_ParamAccess.item, "LP")
            pManager.AddBooleanParameter("SwapOrientation", "SwapOrientation", "Swap orientation of surface?", GH_ParamAccess.item, False)
            pManager.AddNumberParameter("LCSangle", "LCSangle", "LCS angle [deg]", GH_ParamAccess.item, 0)
            'pManager.AddIntegerParameter("SupportingMembersValidity", "SupportingMembersValidity", "Validity of supporting members", GH_ParamAccess.list, 0)
            'AddOptionsToMenuSupportingMembersValidity(pManager.Param(10))
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("Nodes", "Nodes", "Output node data", GH_ParamAccess.tree)
            pManager.AddTextParameter("LoadPanel", "LoadPanel", "Output load panel data", GH_ParamAccess.list)
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
            Rhino.RhinoApp.WriteLine("KoalaLoadPanel: Done in " + Str(time_elapsed) + " ms.")
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            Dim brep As Brep = Nothing
            Dim layerName As String = "LoadPanels"
            Dim panelType As LoadPanelType = LoadPanelType.ToPanelNode
            Dim transferDirection As LoadPanelTransferDirection = LoadPanelTransferDirection.Both
            Dim transferMethod As LoadPanelTransferMethod = LoadPanelTransferMethod.Standard
            Dim nodePrefix As String = "LPN"
            Dim tolerance As Double = 0.001
            Dim remDuplNodes As Boolean = False
            Dim memberNamePrefix As String = "LP"
            Dim swapOrientation As Boolean = False
            Dim angleLCS As Double = 0.0

            Dim i As Integer = 0

            If (Not DA.GetData(0, brep)) Then Return
            If (Not DA.GetData(1, layerName)) Then Return
            If DA.GetData(2, i) Then panelType = CType(i, LoadPanelType)
            If DA.GetData(3, i) Then transferDirection = CType(i, LoadPanelTransferDirection)
            If DA.GetData(4, i) Then transferMethod = CType(i, LoadPanelTransferMethod)
            If (Not DA.GetData(5, nodePrefix)) Then Return
            If (Not DA.GetData(6, tolerance)) Then Return
            If (Not DA.GetData(7, remDuplNodes)) Then Return
            If (Not DA.GetData(8, memberNamePrefix)) Then Return
            If (Not DA.GetData(9, swapOrientation)) Then Return
            If (Not DA.GetData(10, angleLCS)) Then Return

            angleLCS *= Math.PI / 180

            ' Initialize output
            Dim SE_NodeTree As New GH_Structure(Of GH_String)()
            Dim SE_member(7) As String

            Dim basePath As GH_Path = DA.ParameterTargetPath(0)

            'check if the surface can be handled to SCIA Engineer: non-planar surfaces may have a maximum of 4 boundary edges
            Dim warnings As New List(Of String)
            Dim boundary As Rhino.Geometry.Curve = Member2D.GetBrepBoundary(brep)
            Dim segments() As Rhino.Geometry.Curve = Member2D.GetBoundarySegments(boundary, tolerance, warnings)

            If warnings.Any Then
                For Each w In warnings
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, w)
                Next w
            End If

            memberIdx += 1
            Dim memberName As String = String.Format("{0}{1}", memberNamePrefix, memberIdx)

            SE_member(0) = memberName
            SE_member(1) = layerName
            SE_member(2) = Member2D.ProcessSurfaceBoundary(segments, remDuplNodes, tolerance, allNodes, nodeIdx, nodePrefix, basePath, SE_NodeTree)
            SE_member(3) = GetEnumDescription(panelType)
            SE_member(4) = GetEnumDescription(transferDirection)
            SE_member(5) = GetEnumDescription(transferMethod)
            SE_member(6) = If(swapOrientation, "1", "0")
            SE_member(7) = angleLCS

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
                Return My.Resources.LoadPanel
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("2afa25dc-a287-4455-8621-0cd8609c607e")
            End Get
        End Property
    End Class

End Namespace