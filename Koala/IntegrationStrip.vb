Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala
    Public Class IntegrationStrip
        Inherits GH_KoalaComponent

        Const DefaultNamePrefix As String = "CM"
        Public NameIndex As Integer = 0
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("Intergation Strip", "Intergation Strip",
                       "Create an Intergation Strip on 2D Member",
                       "Structure", New EsaObjectType() {EsaObjectType.IntegrationStrip})
        End Sub

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            Dim leftIdx As Integer
            Dim rightIdx As Integer

            pManager.AddTextParameter("2DMemberName", "2DMemberName", "Name of the 2D member where to put integration strip", GH_ParamAccess.item)
            pManager.AddCurveParameter("Line", "Line", "Integration Strip Line", GH_ParamAccess.item)
            pManager.AddBooleanParameter("Create Mesh Node", "Create Mesh Node", "Set True if you want to create new nodes", GH_ParamAccess.item, False)
            pManager.AddParameter(New Param_Enum("Effective Width Geometry", "Effective Width Geometry", GH_ParamAccess.item, EffectiveWidthGeometry.symm))
            pManager.AddParameter(New Param_Enum("Effective Width Definition", "Effective Width Definition", GH_ParamAccess.item, EffectiveWidthDefinition.width))
            pManager.AddNumberParameter("Width", "Width", "Width (total) [m]", GH_ParamAccess.item, 1.0)
            pManager.AddNumberParameter("Thickness", "Thickness", "No. of thickness (total)", GH_ParamAccess.item, 1.0)
            pManager.AddTextParameter("IntegrationStripPrefix", "IntegrationStripPrefix", "Integration Strip name prefix", GH_ParamAccess.item, DefaultNamePrefix)

            ' Possibly hide it if symmetrical width definition
            leftIdx = pManager.AddNumberParameter("Left", "Left", "In case of nonsymerical Width Definition the length of the left side", GH_ParamAccess.item)
            pManager.Param(leftIdx).Optional = True
            rightIdx = pManager.AddNumberParameter("Right", "Right", "In case of nonsymerical Width Definition the length of the right side", GH_ParamAccess.item)
            pManager.Param(rightIdx).Optional = True
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("IntegrationStrip", "IntegrationStrip", "", GH_ParamAccess.list)
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
            Dim SE_integrationStrip(16) As String

            Dim Member2D As String = ""
            Dim integrationStripLine As Curve = Nothing
            Dim mesh As Boolean = False
            Dim geomIdx As Integer = 0
            Dim geom As EffectiveWidthGeometry
            Dim defIdx As Integer = 0
            Dim def As EffectiveWidthDefinition
            Dim Width As Double = 0
            Dim Thickness As Double = 0
            Dim Left As Double = 0
            Dim Right As Double = 0
            Dim NamePrefix As String = DefaultNamePrefix

            If (Not DA.GetData(0, Member2D)) Then Return
            DA.GetData(0, Member2D)
            If (Not DA.GetData(1, integrationStripLine)) Then Return
            DA.GetData(1, integrationStripLine)

            DA.GetData(2, mesh)

            DA.GetData(3, geomIdx)
            geom = CType(geomIdx, EffectiveWidthGeometry)
            DA.GetData(4, defIdx)
            def = CType(defIdx, EffectiveWidthDefinition)

            DA.GetData(5, Width)
            DA.GetData(6, Thickness)
            DA.GetData(7, NamePrefix)
            DA.GetData(8, Left)
            DA.GetData(9, Right)


            ' Check and process input data
            If String.IsNullOrEmpty(Member2D) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "2D Member can't be empty")
                Return
            End If

            If String.IsNullOrEmpty(NamePrefix) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "NamePrefix can't be empty")
                Return
            End If

            SE_integrationStrip(0) = NamePrefix & NameIndex.ToString()
            ' SE_integrationStrip(1) = UniqueID
            SE_integrationStrip(2) = mesh.ToString()
            SE_integrationStrip(3) = GetEnumDescription(geom)
            SE_integrationStrip(4) = GetEnumDescription(def)
            SE_integrationStrip(5) = Width.ToString()
            SE_integrationStrip(6) = Thickness.ToString()

            'get points of the line
            'Declarations to work with RhinoCommon objects
            Dim arrPoints As New Rhino.Collections.Point3dList
            Dim lineType As String = ""
            Dim lineShape As String

            'extract geometry from the curve
            GetTypeAndNodes(integrationStripLine, lineType, arrPoints)

            lineShape = lineType
            If lineType <> "Line" Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Could not recognize the geometry of the inputted curves: """ & lineType & """. Only straight lines are supported.")
                Return
            End If

            Dim begIdx As Integer = (NameIndex - 1) * 2
            Dim endIdx As Integer = (NameIndex - 1) * 2 + 1
            Dim beginnigNode As Rhino.Geometry.Point3d = arrPoints(0)
            Dim endNode As Rhino.Geometry.Point3d = arrPoints(1)

            SE_integrationStrip(7) = beginnigNode.X.ToString()
            SE_integrationStrip(8) = beginnigNode.Y.ToString()
            SE_integrationStrip(9) = beginnigNode.Z.ToString()

            SE_integrationStrip(10) = endNode.X.ToString()
            SE_integrationStrip(11) = endNode.Y.ToString()
            SE_integrationStrip(12) = endNode.Z.ToString()

            SE_integrationStrip(13) = integrationStripLine.GetLength().ToString()
            SE_integrationStrip(14) = Member2D

            SE_integrationStrip(15) = Left.ToString()
            SE_integrationStrip(16) = Right.ToString()

            DA.SetDataList(0, SE_integrationStrip)
        End Sub


        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.IntegrationStrip
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
                Return New Guid("CFBC3ABA-D6CC-4E6B-903F-CAE905DA1706")
            End Get
        End Property
    End Class
End Namespace