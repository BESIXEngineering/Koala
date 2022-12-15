Imports System.Collections.Generic
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Data
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry


Namespace Koala

    Public Class Member1D
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
            MyBase.New("1D Member", "1DMember",
                       "Create a 1D member. Members and nodes are numbered continuously regardless of the input data tree structure.",
                       "Structure", New EsaObjectType() {EsaObjectType.Node, EsaObjectType.Member1D})
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
            pManager.AddCurveParameter("Curve", "Curve", "Member curve", GH_ParamAccess.item)
            pManager.AddVectorParameter("ZVector", "ZVector", "Member local z vector", GH_ParamAccess.item)
            pManager.Param(1).Optional = True
            pManager.AddTextParameter("Section", "Section", "Cross-section name", GH_ParamAccess.item, "CS1")
            pManager.AddTextParameter("Layer", "Layer", "Layer name", GH_ParamAccess.item, "Beams")
            pManager.AddTextParameter("NodePrefix", "NodePrefix", "Node prefix", GH_ParamAccess.item, "NB")
            pManager.AddNumberParameter("Tolerance", "Tolerance", "Tolerance to determine duplicate nodes", GH_ParamAccess.item, 0.001)
            pManager.AddBooleanParameter("RemDuplNodes", "RemDuplNodes", "Set True if you want to remove duplicate nodes", GH_ParamAccess.item, False)
            pManager.AddParameter(New Param_Enum("StructuralType", "StructuralType", GH_ParamAccess.item, BeamType.General))
            pManager.AddParameter(New Param_Enum("FEMtype", "Element type for FEM analysis", GH_ParamAccess.item, BeamFEMType.Standard))
            pManager.AddParameter(New Param_Enum("MemberSystemLine", "MemberSystemLine", GH_ParamAccess.item, MemberSystemLine.Centre))
            pManager.AddNumberParameter("ey", "ey", "Eccentricity of load in y axis", GH_ParamAccess.item, 0)
            pManager.AddNumberParameter("ez", "ez", "Eccentricity of load in z axis", GH_ParamAccess.item, 0)
            pManager.AddTextParameter("MemberPrefix", "MemberPrefix", "Member name prefix", GH_ParamAccess.item, "B")
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("Nodes", "Nodes", "Output node data", GH_ParamAccess.tree)
            pManager.AddTextParameter("1D Member", "1DMember", "Output 1D member data", GH_ParamAccess.list)
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
            Dim time_elapsed As Double
            'stop stopwatch
            stopWatch.Stop()
            time_elapsed = stopWatch.ElapsedMilliseconds
            Rhino.RhinoApp.WriteLine("Koala1DMembers: Done in " + Str(time_elapsed) + " ms.")
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            Dim i As Integer = 0
            Dim curve As Curve = Nothing
            Dim nodePrefix As String = "N"
            Dim zvector As Vector3d = Nothing
            Dim layer As String = "Beams"
            Dim section As String = "CS1"
            Dim tolerance As Double = 0.001
            Dim remDuplNodes As Boolean = False
            Dim structuralType As BeamType = BeamType.General
            Dim FEMtype As BeamFEMType = BeamFEMType.Standard
            Dim memberSystemLine As MemberSystemLine = MemberSystemLine.Centre
            Dim ey As Double = 0.0
            Dim ez As Double = 0.0
            Dim memberNamePrefix As String = "B"

            If (Not DA.GetData(0, curve)) Then Return
            DA.GetData(1, zvector)
            If (Not DA.GetData(2, section)) Then Return
            If (Not DA.GetData(3, layer)) Then Return
            If (Not DA.GetData(4, nodePrefix)) Then Return
            If (Not DA.GetData(5, tolerance)) Then Return
            If (Not DA.GetData(6, remDuplNodes)) Then Return
            If DA.GetData(7, i) Then structuralType = CType(i, BeamType)
            If DA.GetData(8, i) Then FEMtype = CType(i, BeamFEMType)
            If DA.GetData(9, i) Then memberSystemLine = CType(i, MemberSystemLine)
            DA.GetData(10, ey)
            DA.GetData(11, ez)
            DA.GetData(12, memberNamePrefix)

            Dim SE_NodeTree As New GH_Structure(Of GH_String)()
            Dim SE_member(12) As String

            Dim basePath As GH_Path = DA.ParameterTargetPath(0)

            'a beam consists of: Name, Section, Layer, LineShape, LCSType, LCSParam1, LCSParam2, LCSParam3
            'If LCSType = 0 > Standard definition of LCS with an angle > LCSParam1 is the angle in radian
            'If LCSType = 2 > Definition of LCS through a vector for local Z > LCSParam1/2/3 are the X, Y, Z components of the vector

            'Declarations to work with RhinoCommon objects
            Dim arrPoint As Rhino.Geometry.Point3d
            Dim arrPoints As New Rhino.Collections.Point3dList

            'initialize some variables
            Dim lineType As String = ""
            Dim lineShape As String

            'input data check
            'Z vectors
            If zvector = Nothing Then
                ' No defined Z vector ! - assigning default SCIA Engineer LCS
                zvector = Vector3d.Zero
            End If

            memberIdx += 1
            Dim memberName As String = String.Format("{0}{1}", memberNamePrefix, memberIdx)

            'extract geometry from the curve
            GetTypeAndNodes(curve, lineType, arrPoints)

            lineShape = lineType
            If lineType <> "Line" And lineType <> "Arc" And lineType <> "Polyline" And lineType <> "Spline" Then 'And LineType <> "Circle" Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Could not recognize the geometry of the inputted curves: """ & lineType & """. Only straight lines & circle arcs are supported. Beam" & memberName & "will not be created.")
                Return
            End If

            SE_member(0) = memberName
            SE_member(1) = section
            SE_member(2) = layer

            'create the new nodes
            For Each arrPoint In arrPoints
                Dim currentnode As SENode? = Nothing
                Dim nodePath As GH_Path = basePath.AppendElement(nodeIdx)

                ' Find whether the node already exists, if not, create it
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

            SE_member(3) = lineShape

            ' add LCS definition if present
            If zvector.IsZero Then 'no Z Vector defined
                SE_member(4) = 0
                SE_member(5) = 0 'default beam LCS orientation
            Else
                SE_member(4) = 2 'assign beam LCS based on Z-vector
                SE_member(5) = zvector.X
                SE_member(6) = zvector.Y
                SE_member(7) = zvector.Z
            End If

            SE_member(8) = GetEnumDescription(structuralType)
            SE_member(9) = GetEnumDescription(FEMtype)
            SE_member(10) = GetEnumDescription(memberSystemLine)

            SE_member(11) = ey
            SE_member(12) = ez

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
                Return My.Resources.Beam
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("720e3890-2fba-fae1-96a2-971e3bfe8dec")
            End Get
        End Property
    End Class

End Namespace