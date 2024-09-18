Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.ApplicationSettings
Imports Rhino.Geometry

Namespace Koala
    Public Class ResultClass
        Inherits GH_KoalaComponent
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("ResultClass", "ResultClass", "Definition of Result Classes",
                "Libraries", New EsaObjectType() {EsaObjectType.ResultClass})
        End Sub

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
            pManager.AddTextParameter("Name", "Name", "Result class name", GH_ParamAccess.item, "")
            pManager.AddTextParameter("LoadCases", "LoadCases", "Load case names (e.g. 'LC1;LC2;LC3;...')", GH_ParamAccess.item, "")
            pManager.AddTextParameter("ULS", "ULS", "ULS (linear) combination names (e.g. 'ULS1;ULS2;ULS3;...)", GH_ParamAccess.item, "")
            pManager.AddTextParameter("SLS", "SLS", "SLS (linear) combination names (e.g. 'SLS1;SLS2;SLS3;...')", GH_ParamAccess.item, "")
            pManager.AddTextParameter("NL ULS", "NL ULS", "ULS (non-linear) combination names (e.g. 'NLU1;NLU2,NLU3;...')", GH_ParamAccess.item, "")
            pManager.AddTextParameter("NL SLS", "NL SLS", "SLS (non-linear) combination names (e.g. 'NLS1;NLS2;NLS3;...')", GH_ParamAccess.item, "")
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
            pManager.AddTextParameter("ResultClasses", "ResultClasses", "Result classes data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As Grasshopper.Kernel.IGH_DataAccess)
            Dim SE_resultclasses(3) As String

            Dim rcname As String = ""
            Dim lcname As String = ""
            Dim ULScomname As String = ""
            Dim SLScomname As String = ""
            Dim NLULScomname As String = ""
            Dim NLSLScomname As String = ""

            Dim rcpartname As String = ""
            Dim rccasetype As String = ""
            Dim rccasetus As String = ""

            Dim len_parts(4) As Integer
            Dim partname As New List(Of String)()
            Dim casetype As New List(Of String)()
            Dim casetus As New List(Of String)()

            If (Not DA.GetData(0, rcname)) Then Return
            DA.GetData(0, rcname)
            DA.GetData(1, lcname)
            DA.GetData(2, ULScomname)
            DA.GetData(3, SLScomname)
            DA.GetData(4, NLULScomname)
            DA.GetData(5, NLSLScomname)

            'check input data
            If String.IsNullOrEmpty(rcname) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Name can't be empty")
                Return
            End If

            len_parts(0) = Split(lcname, ";").Length
            len_parts(1) = Split(ULScomname, ";").Length
            len_parts(2) = Split(SLScomname, ";").Length
            len_parts(3) = Split(NLULScomname, ";").Length
            len_parts(4) = Split(NLSLScomname, ";").Length

            If String.IsNullOrEmpty(lcname) And
                    String.IsNullOrEmpty(ULScomname) And
                    String.IsNullOrEmpty(SLScomname) And
                    String.IsNullOrEmpty(NLULScomname) And
                    String.IsNullOrEmpty(NLSLScomname) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "At least one set of load cases or combinations is expected")
                Return
            End If

            Dim lcparts() As String = Split(lcname, ";")
            Dim ULScomparts() As String = Split(ULScomname, ";")
            Dim SLScomparts() As String = Split(SLScomname, ";")
            Dim NLULScomparts() As String = Split(NLULScomname, ";")
            Dim NLSLScomparts() As String = Split(NLSLScomname, ";")

            If Not String.IsNullOrEmpty(lcname) Then
                For i = 0 To len_parts(0) - 1
                    partname.Add(lcparts(i))
                    casetype.Add("1001")
                    casetus.Add("-1")
                Next
            End If

            If Not String.IsNullOrEmpty(ULScomname) Then
                For i = 0 To len_parts(1) - 1
                    partname.Add(ULScomparts(i))
                    casetype.Add("1001")
                    casetus.Add("0")
                Next
            End If

            If Not String.IsNullOrEmpty(SLScomname) Then
                For i = 0 To len_parts(2) - 1
                    partname.Add(SLScomparts(i))
                    casetype.Add("1001")
                    casetus.Add("1")
                Next
            End If

            If Not String.IsNullOrEmpty(NLULScomname) Then
                For i = 0 To len_parts(3) - 1
                    partname.Add(NLULScomparts(i))
                    casetype.Add("1002")
                    casetus.Add("0")
                Next
            End If

            If Not String.IsNullOrEmpty(NLSLScomname) Then
                For i = 0 To len_parts(2) - 1
                    partname.Add(NLSLScomparts(i))
                    casetype.Add("1002")
                    casetus.Add("1")
                Next
            End If

            rcpartname = Join(partname.ToArray(), ";")
            rccasetype = Join(casetype.ToArray(), ";")
            rccasetus = Join(casetus.ToArray(), ";")

            SE_resultclasses(0) = rcname
            SE_resultclasses(1) = rcpartname
            SE_resultclasses(2) = rccasetype
            SE_resultclasses(3) = rccasetus

            DA.SetDataList(0, SE_resultclasses)
        End Sub

        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.ResultClass
            End Get
        End Property

        Public Overrides ReadOnly Property ComponentGuid As Guid
            Get
                Return New Guid("5554cb3d-9303-4044-9820-b523806ebb63")
            End Get
        End Property
    End Class
End Namespace
