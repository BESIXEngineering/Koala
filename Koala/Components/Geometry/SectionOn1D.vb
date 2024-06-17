Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry

Namespace Koala
    Public Class SectionOn1D
        Inherits GH_KoalaComponent
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("Section on 1D", "Section on 1D",
                       "Create a section on 1D Member",
                       "Structure", New EsaObjectType() {EsaObjectType.SectionOn1D})
        End Sub

        Const DefaultNamePrefix As String = "SB"

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddTextParameter("1DMemberName", "1DMemberName", "Name of the 1D member where to put a section", GH_ParamAccess.item)
            pManager.AddTextParameter("SectionPrefix", "SectionPrefix", "Section on 1D name prefix", GH_ParamAccess.item, DefaultNamePrefix)
            pManager.AddParameter(New Param_Enum("Coord. Definition", "Coord. Definition", GH_ParamAccess.item, CoordinateDefinition.Rela))
            pManager.AddNumberParameter("Position", "Position", "Position along the 1D Member where to put the section", GH_ParamAccess.item, 0.5)
            pManager.AddParameter(New Param_Enum("Origin", "Origin", GH_ParamAccess.item, Origin.fromStart))
            pManager.AddNumberParameter("Repeat (n)", "Repeat (n)", "Number of times to repeat the section", GH_ParamAccess.item, 1)
            pManager.AddNumberParameter("Delta x", "Delta x", "In case of repeated sections the distance between the sections", GH_ParamAccess.item, 0.1)
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("SectionOn1D", "SectionOn1D", "Section on 1D data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            NameIndex += 1

            Dim SE_sectionOnBeam(7) As String

            Dim Member1D As String = ""
            Dim NamePrefix As String = DefaultNamePrefix
            Dim CoordIdx As Integer = 0
            Dim Coord As CoordinateDefinition
            Dim Position As Double = 0
            Dim originIdx As Integer = 0
            Dim origin As Origin
            Dim Repeat As Double = 0
            Dim Delta As Double = 0

            If (Not DA.GetData(0, Member1D)) Then Return
            DA.GetData(0, Member1D)
            DA.GetData(1, NamePrefix)
            DA.GetData(2, CoordIdx)
            Coord = CType(CoordIdx, CoordinateDefinition)
            DA.GetData(3, Position)
            DA.GetData(4, originIdx)
            origin = CType(originIdx, Origin)
            DA.GetData(5, Repeat)
            DA.GetData(6, Delta)

            ' Check and process input data
            If String.IsNullOrEmpty(Member1D) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "1D Member can't be empty")
                Return
            End If

            SE_sectionOnBeam(0) = Member1D
            SE_sectionOnBeam(1) = NamePrefix & NameIndex.ToString()
            ' SE_sectionOnBeam(2) = UniqueID
            SE_sectionOnBeam(3) = GetEnumDescription(Coord)
            SE_sectionOnBeam(4) = Position.ToString()
            SE_sectionOnBeam(5) = GetEnumDescription(origin)
            SE_sectionOnBeam(6) = Repeat.ToString()
            SE_sectionOnBeam(7) = Delta.ToString()

            DA.SetDataList(0, SE_sectionOnBeam)

        End Sub


        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.SectionOnBeam
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
                Return New Guid("86629DF8-C139-4B53-8216-AFC9F2F0694C")
            End Get
        End Property
    End Class
End Namespace