Imports System.Collections.Generic
Imports Grasshopper.Kernel
Imports Rhino.Geometry

Namespace Koala
    Public Class SectionOn2D
        Inherits GH_KoalaComponent
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("Section on 2D", "Section on 2D",
                       "Create a section on a 2D Member",
                       "Structure", New EsaObjectType() {EsaObjectType.SectionOn2D})
        End Sub

        Const DefaultNamePrefix As String = "SE"

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddTextParameter("SectionPrefix", "SectionPrefix", "Section on 2D name prefix", GH_ParamAccess.item, DefaultNamePrefix)
            pManager.AddTextParameter("Layer", "Layer", "Layer name", GH_ParamAccess.item, "SectionOn2D")
            pManager.AddParameter(New Param_Enum("Draw", "Draw diagram", GH_ParamAccess.item, DrawDiagram.ZDirection))
            pManager.AddVectorParameter("DirectionOfCut", "DirectionOfCut", "Direction of cut", GH_ParamAccess.item, New Vector3d(0, 0, 1))
            pManager.AddLineParameter("Line", "Line", "Line position of section on 2D", GH_ParamAccess.item)
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("SectionOn2D", "SectionOn2D", "Section on 2D data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            Dim SE_sectionOn2D As New List(Of String)

            Dim namePrefix As String = DefaultNamePrefix
            Dim layer As String = ""
            Dim draw As DrawDiagram = DrawDiagram.ZDirection
            Dim directionOfCut As Vector3d
            Dim location As Line

            If Not DA.GetData(0, namePrefix) Then Return
            If Not DA.GetData(1, layer) Then layer = "SectionOn2D"

            Dim idx As Integer = 0
            If DA.GetData(2, idx) Then draw = CType(idx, DrawDiagram)

            DA.GetData(3, directionOfCut)
            DA.GetData(4, location)

            ' Check and process input data
            If String.IsNullOrEmpty(namePrefix) Then
                namePrefix = DefaultNamePrefix
            End If
            If String.IsNullOrEmpty(layer) Then
                layer = "Layer1"
            End If

            NameIndex += 1
            Dim memberName As String = String.Format("{0}{1}", namePrefix, NameIndex)

            SE_sectionOn2D.Add(memberName)
            SE_sectionOn2D.Add(layer)
            SE_sectionOn2D.Add(GetEnumDescription(draw))

            Dim direction As String = directionOfCut.ToString()
            Dim vertex1 As String = location.From.ToString()
            Dim vertex2 As String = location.To.ToString()

            SE_sectionOn2D.Add(direction)
            SE_sectionOn2D.Add(vertex1)
            SE_sectionOn2D.Add(vertex2)

            DA.SetDataList(0, SE_sectionOn2D)

        End Sub


        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.SectionOn2D
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
                Return New Guid("26729DF8-C139-4B53-8216-AFC9F2F0690E")
            End Get
        End Property
    End Class
End Namespace