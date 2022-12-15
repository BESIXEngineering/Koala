Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala

    Public Class ArbitraryProfile
        Inherits GH_KoalaComponent

        Const DefaultNamePrefix As String = "AP"
        Public NameIndex As Integer = 0

        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("ArbitraryProfile", "ArbitraryProfile",
                "Define a SCIA Arbitrary Profile for a 1D member. NOTE: SCIA XML doesn't support setting H and B parameters of ParametricHaunch spans.",
                 "Structure", New EsaObjectType() {EsaObjectType.ArbitraryProfile})
        End Sub

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            Dim pIdx As Integer

            pManager.AddTextParameter("Beams", "Beams", "Names of the beams on which the ArbitraryProfile is placed (can be multiple beams separated by semi-colon ';')", GH_ParamAccess.item)
            pManager.AddTextParameter("NamePrefix", "NamePrefix", "Object name prefix", GH_ParamAccess.item, DefaultNamePrefix)
            pManager.AddTextParameter("Section", "Section", "Cross section name of the ArbitraryProfile object", GH_ParamAccess.item)
            pManager.AddParameter(New Param_Enum("CoordDefinition", "CoordDefinition", GH_ParamAccess.item, ArbitraryProfileCoordDefinition.Rela))

            pIdx = pManager.AddNumberParameter("Length", "Length", "Length of each span (if relative, should sum up to 1)", GH_ParamAccess.list)
            pManager.Param(pIdx).Optional = True

            pManager.AddParameter(New Param_Enum("TypeOfCss", "TypeOfCss", GH_ParamAccess.list, ArbitraryProfileSpanType.Prismatic))
            pManager.AddTextParameter("Css1", "Css1", "Name of Cross section 1 for each span", GH_ParamAccess.list)
            pManager.AddTextParameter("Css2", "Css2", "Name of Cross section 2 for each span (if type is TwoCss)", GH_ParamAccess.list)
            pManager.AddParameter(New Param_Enum("Alignment", "Cross section alignment for each span", GH_ParamAccess.list, ArbitraryProfileAlignment.Undefined))
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("ArbitraryProfiles", "ArbitraryProfiles", "", GH_ParamAccess.list)
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
            Dim ArbitraryProfiles = New List(Of String)

            Dim Beams As String = ""
            Dim NamePrefix As String = DefaultNamePrefix
            Dim Section As String

            Dim CoordDefinitionIdx As Integer = 1
            Dim CoordDefinition As ArbitraryProfileCoordDefinition

            Dim Lengths = New List(Of Double)
            Dim TypeOfCss = New List(Of Integer)
            Dim Css1 = New List(Of String)
            Dim Css2 = New List(Of String)
            Dim Alignments = New List(Of Integer)

            If (Not DA.GetData(0, Beams)) Then Return
            DA.GetData(0, Beams)
            DA.GetData(1, NamePrefix)
            If (Not DA.GetData(2, Section)) Then Return

            DA.GetData(3, CoordDefinitionIdx)
            CoordDefinition = CType(CoordDefinitionIdx, ArbitraryProfileCoordDefinition)

            DA.GetDataList(4, Lengths)
            DA.GetDataList(5, TypeOfCss)
            If (Not DA.GetDataList(6, Css1)) Then Return
            If (Not DA.GetDataList(7, Css2)) Then Return

            DA.GetDataList(8, Alignments)

            ' Check and process input data
            If String.IsNullOrEmpty(Beams) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Beams can't be empty")
                Return
            End If

            If String.IsNullOrEmpty(NamePrefix) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "NamePrefix can't be empty")
                Return
            End If

            If String.IsNullOrEmpty(Section) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Section can't be empty")
                Return
            End If

            ' Iterate over each span and collect the span parameters
            Dim spanCount As Integer = Css1.Count 'New Integer() {Lengths.Count, Css1.Count, Css2.Count}.Min()

            Dim LengthValues(spanCount - 1) As String
            Dim TypeOfCssValues(spanCount - 1) As String
            Dim Css1Values(spanCount - 1) As String
            Dim Css2Values(spanCount - 1) As String
            Dim AlignmentsValues(spanCount - 1) As String

            Dim SpanLength As Double
            If CoordDefinition = ArbitraryProfileCoordDefinition.Rela Then
                SpanLength = 1.0 / spanCount
            Else
                SpanLength = 1.0
            End If

            Dim SpanTypeOfCss As ArbitraryProfileSpanType = ArbitraryProfileSpanType.Prismatic
            Dim SpanAlignmentValue As ArbitraryProfileAlignment = ArbitraryProfileAlignment.Undefined

            For i As Integer = 0 To spanCount - 1
                Css1Values(i) = Css1(i)

                If i < TypeOfCss.Count Then
                    SpanTypeOfCss = CType(TypeOfCss(i), ArbitraryProfileSpanType)
                End If
                TypeOfCssValues(i) = SpanTypeOfCss.ToString()

                If SpanTypeOfCss = ArbitraryProfileSpanType.Prismatic Or i >= Css2.Count Then
                    Css2Values(i) = Css1(i)
                Else
                    Css2Values(i) = Css2(i)
                End If

                If i < Lengths.Count Then
                    SpanLength = Lengths(i)
                End If
                LengthValues(i) = SpanLength.ToString()

                If i < Alignments.Count Then
                    SpanAlignmentValue = CType(Alignments(i), ArbitraryProfileAlignment)
                End If
                AlignmentsValues(i) = SpanAlignmentValue.ToString()
            Next
            'Flatten data for export as simple list
            Dim FlatList As New List(Of Object)()

            FlatList.Clear()


            For Each beamName As String In Split(Beams, ";")
                If (String.IsNullOrWhiteSpace(beamName)) Then
                    Continue For
                End If

                FlatList.Add(beamName.Trim())

                NameIndex += 1
                FlatList.Add(NamePrefix & NameIndex.ToString())
                FlatList.Add(Section)
                FlatList.Add(CoordDefinition.ToString())
                FlatList.Add(Join(LengthValues, ";"))
                FlatList.Add(Join(TypeOfCssValues, ";"))
                FlatList.Add(Join(Css1Values, ";"))
                FlatList.Add(Join(Css2Values, ";"))
                FlatList.Add(Join(AlignmentsValues, ";"))
            Next

            DA.SetDataList(0, FlatList)
        End Sub


        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.ArbitraryProfile
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("3e039d2b-6dff-41a4-8f55-475c4ad7bf37")
            End Get
        End Property
    End Class

End Namespace