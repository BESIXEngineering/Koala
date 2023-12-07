Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Types
Imports Rhino.Geometry


Namespace Koala
    Public Class AveragingStrip
        Inherits GH_KoalaComponent

        Const DefaultNamePrefix As String = "RS"
        Public NameIndex As Integer = 0
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("Averaging Strip", "Averaging Strip",
                       "Create an Averaging Strip on 2D Member",
                       "Structure", New EsaObjectType() {EsaObjectType.AveragingStrip})
        End Sub

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            Dim leftIdx As Integer
            Dim rightIdx As Integer

            pManager.AddTextParameter("2DMemberName", "2DMemberName", "Name of the 2D member where to put integration strip", GH_ParamAccess.item)
            pManager.AddGeometryParameter("Position", "Position", "Point or Line geometry on 2D Member", GH_ParamAccess.item)
            pManager.AddParameter(New Param_Enum("Direction", "Direction", GH_ParamAccess.item, AveragingStripDirection.longitudinal))
            pManager.AddNumberParameter("Width", "Width", "Width", GH_ParamAccess.item, 1.0)
            pManager.AddNumberParameter("Length", "Length", "Length", GH_ParamAccess.item, 1.0)
            pManager.AddNumberParameter("Angle", "Angle", "Angle", GH_ParamAccess.item, 0.0)
            pManager.AddTextParameter("AveragingStripPrefix", "AveragingStripPrefix", "Averaging Strip name prefix", GH_ParamAccess.item, DefaultNamePrefix)


        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("AveragingStrip", "AveragingStrip", "", GH_ParamAccess.list)
        End Sub

        Protected Overrides Sub BeforeSolveInstance()
            NameIndex = 0
            MyBase.BeforeSolveInstance()
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            NameIndex += 1
            Dim SE_averagingStrip(13) As String

            Dim Member2D As String = ""
            Dim averagingStripGeometry As IGH_GeometricGoo = Nothing
            Dim averaginStripLine As GH_Curve
            Dim averagingStripPoint As GH_Point
            ' Dim avergingStripType As AveragingStripType
            Dim avergingStripDirection As AveragingStripDirection
            Dim directionIdx As Integer = 0
            Dim Width As Double = 0
            Dim Length As Double = 0
            Dim Angle As Double = 0
            Dim NamePrefix As String = DefaultNamePrefix

            If (Not DA.GetData(0, Member2D)) Then Return
            DA.GetData(0, Member2D)
            If (Not DA.GetData(1, averagingStripGeometry)) Then Return
            DA.GetData(1, averagingStripGeometry)

            'DA.GetData(2, avergingStripType)
            'avergingStripType = CType(avergingStripType, AveragingStripType)

            DA.GetData(2, directionIdx)
            avergingStripDirection = CType(directionIdx, AveragingStripDirection)

            DA.GetData(3, Width)
            DA.GetData(4, Length)
            DA.GetData(5, Angle)
            DA.GetData(6, NamePrefix)

            ' Check and process input data
            If String.IsNullOrEmpty(Member2D) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "2D Member can't be empty")
                Return
            End If

            If String.IsNullOrEmpty(NamePrefix) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "NamePrefix can't be empty")
                Return
            End If

            SE_averagingStrip(0) = Member2D
            SE_averagingStrip(1) = NamePrefix & NameIndex.ToString()
            ' SE_averagingStrip(3) = GetEnumDescription(avergingStripType)
            SE_averagingStrip(4) = GetEnumDescription(avergingStripDirection)
            SE_averagingStrip(5) = Width.ToString()
            SE_averagingStrip(6) = Length.ToString()
            SE_averagingStrip(7) = Angle.ToString()


            If TypeOf averagingStripGeometry Is GH_Curve Then

                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Warn: second point of Averaging Strip might not be set in SCIA. This is a bug...")

                averaginStripLine = DirectCast(averagingStripGeometry, GH_Curve)

                Dim aveLineRhino As Curve = Nothing
                averaginStripLine.CastTo(Of Curve)(aveLineRhino)

                'get points of the line
                'Declarations to work with RhinoCommon objects
                Dim arrPoints As New Rhino.Collections.Point3dList
                Dim lineType As String = ""
                Dim lineShape As String

                'extract geometry from the curve
                GetTypeAndNodes(aveLineRhino, lineType, arrPoints)

                lineShape = lineType
                If lineType <> "Line" Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Could not recognize the geometry of the inputted curves: """ & lineType & """. Only straight lines are supported.")
                    Return
                End If

                'Dim begIdx As Integer = (NameIndex - 1) * 2
                'Dim endIdx As Integer = (NameIndex - 1) * 2 + 1
                Dim beginNode As Rhino.Geometry.Point3d = arrPoints(0)
                Dim endNode As Rhino.Geometry.Point3d = arrPoints(1)

                SE_averagingStrip(3) = GetEnumDescription(AveragingStripType.strip)

                SE_averagingStrip(8) = beginNode.X.ToString()
                SE_averagingStrip(9) = endNode.X.ToString()

                SE_averagingStrip(10) = beginNode.Y.ToString()
                SE_averagingStrip(11) = endNode.Y.ToString()

                SE_averagingStrip(12) = beginNode.Z.ToString()
                SE_averagingStrip(13) = endNode.Z.ToString()

            ElseIf TypeOf averagingStripGeometry Is GH_Point Then
                averagingStripPoint = DirectCast(averagingStripGeometry, GH_Point)

                Dim avePointRhino As Point3d
                averagingStripGeometry.CastTo(Of Point3d)(avePointRhino)

                SE_averagingStrip(3) = GetEnumDescription(AveragingStripType.point)

                SE_averagingStrip(8) = avePointRhino.X.ToString()
                SE_averagingStrip(10) = avePointRhino.Y.ToString()
                SE_averagingStrip(12) = avePointRhino.Z.ToString()

                'TODO: Make global to local coord
            End If


            DA.SetDataList(0, SE_averagingStrip)
        End Sub


        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.AveragingStrip
            End Get
        End Property

        Public Overrides ReadOnly Property Exposure As GH_Exposure
            Get
                Return GH_Exposure.quarternary
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("CFBC3ABA-D6CC-4E6B-903F-CAE905DA1707")
            End Get
        End Property
    End Class
End Namespace