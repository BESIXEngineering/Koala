﻿Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Parameters
Imports Rhino.Geometry


Namespace Koala

    ''' <summary>
    ''' Obsolete. Use "Member1D" instead which makes better use of Grasshopper's default data matching algorithm.
    ''' </summary>
    <System.Obsolete>
    Public Class Beams
        Inherits GH_KoalaComponent
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("Beams", "Beams",
                "Beams description",
                "Structure", New EsaObjectType() {EsaObjectType.Node, EsaObjectType.Member1D})
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
            pManager.AddCurveParameter("Curves", "Curves", "List of definiton curves for beams", GH_ParamAccess.list)
            pManager.AddVectorParameter("ZVectors", "ZVectors", "List of zvectors", GH_ParamAccess.list)
            pManager.Param(1).Optional = True
            pManager.AddTextParameter("Sections", "Sections", "Cross-sections", GH_ParamAccess.list, "CS1")
            pManager.AddTextParameter("Layers", "Layers", "Definition of layers", GH_ParamAccess.list, "Beams")
            pManager.AddTextParameter("NodePrefix", "NodePrefix", "Node prefix", GH_ParamAccess.item, "NB")
            pManager.AddNumberParameter("Tolerance", "Tolerance", "Tolerance for duplicity nodes", GH_ParamAccess.item, 0.001)
            pManager.AddBooleanParameter("RemDuplNodes", "RemDuplNodes", "Set True if you want to remove duplicate nodes", GH_ParamAccess.item, False)
            pManager.AddIntegerParameter("StructuralType", "StructuralType", "Type:  Right click and select from options or choose number: general - 0, beam - 1, column - 2, gable column - 3, secondary column - 4,rafter-5, purlin- 6,roof bracing-7,
            wall bracing - 8, girt - 9, truss chord- 10, truss diagonal - 11,plate rib - 12, beam slab - 13", GH_ParamAccess.list, 0)
            AddOptionsToMenuBeamType(pManager.Param(7))
            pManager.AddIntegerParameter("FEMtype", "FEM type", "Element type for FEM analysis: Right click and select from options or choose number: standard - 0,axial force only - 1", GH_ParamAccess.list, 0)
            pManager.AddIntegerParameter("MemberSystemLine", "MemberSystemLine", "Member system line at: Right click and select from options or choose number: Centre - 1,Top - 2,  Bottom - 4,Left - 8, Top left - 10, Bottom left - 12, Right - 16, Top right - 18,Bottom right - 20 ", GH_ParamAccess.list, 1)
            AddOptionstoMenuMemberSystemLine(pManager.Param(9))
            pManager.AddNumberParameter("ey", "ey", "Eccentricity of load in y axis", GH_ParamAccess.list, 0)
            pManager.AddNumberParameter("ez", "ez", "Eccentricity of load in z axis", GH_ParamAccess.list, 0)
            pManager.AddTextParameter("BeamNamePrefix", "BeamNamePrefix", "Beam name prefix", GH_ParamAccess.item, "B")


        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("Nodes", "Nodes", "Output nodes", GH_ParamAccess.list)
            pManager.AddTextParameter("Beams", "Beams", "Output beams", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            Dim stopWatch As New System.Diagnostics.Stopwatch()
            Dim time_elapsed As Double

            Dim j As Long
            Dim ivector As Long
            Dim i As Integer
            Dim currentnode As Long

            Dim Curves = New List(Of Curve)
            Dim NodePrefix As String = "N"
            Dim zvectors = New List(Of Vector3d)
            Dim layers = New List(Of String)
            Dim sections = New List(Of String)
            Dim tolerance As Double
            Dim RemDuplNodes As Boolean
            Dim StructuralType As String = "general"
            Dim StructuralTypes = New List(Of Integer)
            Dim FEMtype As String = "standard"
            Dim FEMtypes = New List(Of Integer)
            Dim MemberSystemLine As String = "Centre"
            Dim MemberSystemLines = New List(Of Integer)
            Dim ey As Double = 0.0
            Dim eys = New List(Of Double)
            Dim ez As Double = 0.0
            Dim ezs = New List(Of Double)
            Dim BeamNamePrefix As String = "B"

            If (Not DA.GetDataList(Of Curve)(0, Curves)) Then Return
            DA.GetDataList(Of Vector3d)(1, zvectors)
            If (Not DA.GetDataList(Of String)(2, sections)) Then Return
            If (Not DA.GetDataList(Of String)(3, layers)) Then Return
            If (Not DA.GetData(Of String)(4, NodePrefix)) Then Return
            If (Not DA.GetData(Of Double)(5, tolerance)) Then Return
            If (Not DA.GetData(Of Boolean)(6, RemDuplNodes)) Then Return
            DA.GetDataList(Of Integer)(7, StructuralTypes)
            DA.GetDataList(Of Integer)(8, FEMtypes)
            DA.GetDataList(Of Integer)(9, MemberSystemLines)
            DA.GetDataList(Of Double)(10, eys)
            DA.GetDataList(Of Double)(11, ezs)
            DA.GetData(Of String)(12, BeamNamePrefix)

            Dim SE_nodes(100000, 3) As String 'a node consists of: Name, X, Y, Z > make the array a dynamic list later
            Dim FlatNodeList As New List(Of String)()

            Dim SE_beams(Curves.Count, 13) As String

            Dim FlatBeamList As New List(Of String)()
            'a beam consists of: Name, Section, Layer, LineShape, LCSType, LCSParam1, LCSParam2, LCSParam3
            'If LCSType = 0 > Standard definition of LCS with an angle > LCSParam1 is the angle in radian
            'If LCSType = 2 > Definition of LCS through a vector for local Z > LCSParam1/2/3 are the X, Y, Z components of the vector

            Dim beam_material As Long

            Dim nodecount As Long, beamcount As Long
            Dim curvecount As Long

            Dim LineShape As String, LineType As String

            'Declarations to work with RhinoCommon objects
            Dim arrPoint As Rhino.Geometry.Point3d
            Dim arrPoints As New Rhino.Collections.Point3dList
            Dim vVector As Rhino.Geometry.Vector3d

            'initialize stopwatch
            stopWatch.Start()

            'initialize some variables
            beam_material = 1
            nodecount = 0
            beamcount = 0
            LineType = ""

            'loop through all segments
            '===========================

            curvecount = Curves.Count
            'input data check

            'Z vectors
            If zvectors.Count = 0 Then
                ' No defined list of Z vectors ! - assigning default SCIA Engineer LCS to all Curves
                vVector.X = 0
                vVector.Y = 0
                vVector.Z = 0
                For i = 1 To curvecount
                    zvectors.Add(vVector)
                Next
            End If

            ''check nr of z vectors
            'If curvecount < zvectors.count Then
            '    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Too many Z vectors are defined. They will be ignored.")
            'ElseIf curvecount > zvectors.count Then
            '    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Less Z vectors are defined than beams. The last defined Z vector will be used for the extra beams")
            'End If

            ''check nr of layers
            'If curvecount < layers.count Then
            '    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Too many layers are defined. They will be ignored.")
            'ElseIf curvecount > layers.count Then
            '    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Less layers are defined than beams. The last defined layer will be used for the extra beams")
            'End If

            ''check nr of sections
            'If curvecount < sections.count Then
            '    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Too many  sections are defined. They will be ignored.")
            'ElseIf curvecount > sections.count Then
            '    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Less sections are defined than beams. The last defined section will be used for the extra beams")
            'End If

            ''check maxStructuralTypes of sections
            'If curvecount < StructuralTypes.Count Then
            '    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Too many  StructuralTypes are defined. They will be ignored.")
            'ElseIf curvecount > StructuralTypes.Count Then
            '    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Less StructuralTypes are defined than beams. The last defined section will be used for the extra beams")
            'End If

            ''check maxFEMTypes of sections
            'If curvecount < FEMtypes.Count Then
            '    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Too many  FEMtypes are defined. They will be ignored.")
            'ElseIf curvecount > FEMtypes.Count Then
            '    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Less FEMtypes are defined than beams. The last defined section will be used for the extra beams")
            'End If

            ''check max of sections
            'If curvecount < eys.Count Then
            '    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Too many  eccentricities ey are defined. They will be ignored.")
            'ElseIf curvecount > eys.Count Then
            '    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Less eccentricities ey are defined than beams. The last defined section will be used for the extra beams")
            'End If

            ''check maxStructuralTypes of sections
            'If curvecount < ezs.Count Then
            '    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Too many  eccentricities ez are defined. They will be ignored.")
            'ElseIf curvecount > ezs.Count Then
            '    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Less eccentricities ez are defined than beams. The last defined section will be used for the extra beams")
            'End If

            For i = 0 To curvecount - 1

                'extract geometry from the curve
                GetTypeAndNodes(Curves(i), LineType, arrPoints)

                LineShape = LineType
                If LineType <> "Line" And LineType <> "Arc" And LineType <> "Polyline" And LineType <> "Spline" Then 'And LineType <> "Circle" Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Could not recognize the geometry of the inputted curves: """ & LineType & """. Only straight lines & circle arcs are supported. Beam" & BeamNamePrefix + i.ToString() & "will not be created.")
                    Continue For
                End If

                SE_beams(i, 0) = BeamNamePrefix + (i + 1).ToString()
                SE_beams(i, 1) = sections(Math.Min(sections.Count - 1, i))
                SE_beams(i, 2) = layers(Math.Min(layers.Count - 1, i))

                'create the new nodes

                For Each arrPoint In arrPoints
                    'check if node at these coordinates already exists
                    If RemDuplNodes Then
                        currentnode = GetExistingNode(arrPoint, SE_nodes, nodecount, tolerance)
                    Else
                        currentnode = -1
                    End If

                    If currentnode = -1 Then
                        'create it
                        SE_nodes(nodecount, 0) = NodePrefix & (nodecount + 1)
                        SE_nodes(nodecount, 1) = arrPoint.X
                        SE_nodes(nodecount, 2) = arrPoint.Y
                        SE_nodes(nodecount, 3) = arrPoint.Z

                        currentnode = nodecount
                        nodecount += 1
                    End If

                    'add the node to the line shape
                    LineShape = LineShape & ";" & SE_nodes(currentnode, 0)

                Next arrPoint

                SE_beams(i, 3) = LineShape

                ' add LCS definition if present
                ivector = Math.Min(zvectors.Count - 1, i)

                If zvectors(ivector).IsZero Then 'no Z Vector defined
                    SE_beams(i, 4) = 0
                    SE_beams(i, 5) = 0 'default beam LCS orientation
                Else
                    SE_beams(i, 4) = 2 'assign beam LCS based on Z-vector
                    SE_beams(i, 5) = zvectors(ivector).X
                    SE_beams(i, 6) = zvectors(ivector).Y
                    SE_beams(i, 7) = zvectors(ivector).Z
                End If

                SE_beams(i, 8) = GetStringForBeamType(StructuralTypes(Math.Min(StructuralTypes.Count - 1, i)))
                SE_beams(i, 9) = GetStringForBeamFEMtype(FEMtypes(Math.Min(FEMtypes.Count - 1, i)))
                SE_beams(i, 10) = GetStringForMemberSystemLineOrPlane(MemberSystemLines(Math.Min(MemberSystemLines.Count - 1, i)))

                SE_beams(i, 11) = eys(Math.Min(eys.Count - 1, i))
                SE_beams(i, 12) = ezs(Math.Min(ezs.Count - 1, i))
                SE_beams(i, 13) = -1  'Buckling group index

                beamcount += 1

            Next i 'next curve

            'Flatten data for export as simple list

            FlatNodeList.Clear()

            For i = 0 To nodecount - 1
                For j = 0 To 3
                    FlatNodeList.Add(SE_nodes(i, j))
                Next j
            Next i
            DA.SetDataList(0, FlatNodeList)


            FlatBeamList.Clear()

            For i = 0 To beamcount - 1
                If SE_beams(i, 0) Is Nothing Then
                    Continue For
                End If
                FlatBeamList.Add(SE_beams(i, 0))
                FlatBeamList.Add(SE_beams(i, 1))
                FlatBeamList.Add(SE_beams(i, 2))
                FlatBeamList.Add(SE_beams(i, 3))
                FlatBeamList.Add(SE_beams(i, 4))
                FlatBeamList.Add(SE_beams(i, 5))
                FlatBeamList.Add(SE_beams(i, 6))
                FlatBeamList.Add(SE_beams(i, 7))
                FlatBeamList.Add(SE_beams(i, 8))
                FlatBeamList.Add(SE_beams(i, 9))
                FlatBeamList.Add(SE_beams(i, 10))
                FlatBeamList.Add(SE_beams(i, 11))
                FlatBeamList.Add(SE_beams(i, 12))
                FlatBeamList.Add(SE_beams(i, 13))

            Next i
            DA.SetDataList(1, FlatBeamList)

            'stop stopwatch
            stopWatch.Stop()
            time_elapsed = stopWatch.ElapsedMilliseconds
            'rhino.RhinoApp.WriteLine("KoalaBeams: Done in " + str(time_elapsed) + " ms.")
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
                Return New Guid("720a3890-2fba-4ae1-96a2-971e3bfe8dec")
            End Get
        End Property


        Private Function GetStringForBeamFEMtype(item As Integer) As String
            Select Case item
                Case 0
                    Return "standard"
                Case 1
                    Return "axial force only"
                Case Else
                    Return "standard"
            End Select
        End Function

        Private Sub AddOptionsToMenuBeamType(menuItem As Param_Integer)
            menuItem.AddNamedValue("general", 0)
            menuItem.AddNamedValue("beam", 1)
            menuItem.AddNamedValue("column", 2)
            menuItem.AddNamedValue("gable column", 3)
            menuItem.AddNamedValue("secondary column", 4)
            menuItem.AddNamedValue("rafter", 5)
            menuItem.AddNamedValue("purlin", 6)
            menuItem.AddNamedValue("roof bracing", 7)
            menuItem.AddNamedValue("wall bracing", 8)
            menuItem.AddNamedValue("girt", 9)
            menuItem.AddNamedValue("truss chord", 10)
            menuItem.AddNamedValue("truss diagonal", 11)
            menuItem.AddNamedValue("plate rib", 12)
            menuItem.AddNamedValue("beam slab", 13)
        End Sub

        Private Function GetStringForBeamType(item As Integer) As String
            Select Case item
                Case 0
                    Return "general"
                Case 1
                    Return "beam"
                Case 2
                    Return "column"
                Case 3
                    Return "gable column"
                Case 4
                    Return "secondary column"
                Case 5
                    Return "rafter"
                Case 6
                    Return "purlin"
                Case 7
                    Return "roof bracing"
                Case 8
                    Return "wall bracing"
                Case 9
                    Return "girt"
                Case 10
                    Return "truss chord"
                Case 11
                    Return "truss diagonal"
                Case 12
                    Return "plate rib"
                Case 13
                    Return "beam slab"
                Case Else
                    Return "general"
            End Select
        End Function
    End Class

End Namespace