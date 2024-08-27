Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry

Namespace Koala
    Public Class LoadGroup
        Inherits GH_KoalaComponent
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("LoadGroup", "LoadGroup", "Definition of Load Groups", "Libraries",
                       New EsaObjectType() {EsaObjectType.LoadGroup})
        End Sub

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
            pManager.AddTextParameter("Name", "Name", "Name of the LoadGroup (e.g. LG1)", GH_ParamAccess.item)
            pManager.AddTextParameter("Type", "Type", "LoadGroup type (permanent, variable, accidental, or seismic)", GH_ParamAccess.item, "permanent")
            pManager.AddTextParameter("Relation", "Relation", "LoadGroup relation (standard, exclusive, or together)", GH_ParamAccess.item, "N/A")
            'It might be nicer to do this with Enums, but current component outputs Strings
            'pManager.AddParameter(New Param_Enum("Type", "Type", GH_ParamAccess.item, LoadGroupType.variable))
            'pManager.AddParameter(New Param_Enum("Relation", "Relation", GH_ParamAccess.item, LoadGroupRelation.standard))
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
            pManager.AddTextParameter("LoadGroups", "LoadGroups", "LoadGroup data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
            Dim SE_lgroups(2) As String

            Dim lgroupname As String = ""
            Dim lgrouptype As String = "permanent"
            Dim lgrouprelation As String = "N/A"
            'Dim lgrouptype As LoadGroupType
            'Dim lgrouptypeIdx As Integer = 1
            'Dim lgrouprelation As LoadGroupRelation
            'Dim lgrouprelationIdx As Integer = 0

            If (Not DA.GetData(0, lgroupname)) Then Return
            DA.GetData(0, lgroupname)
            DA.GetData(1, lgrouptype)
            DA.GetData(2, lgrouprelation)
            'DA.GetData(1, lgrouptypeIdx)
            'lgrouptype = CType(lgrouptypeIdx, LoadGroupType)
            'DA.GetData(2, lgrouprelationIdx)
            'lgrouprelation = CType(lgrouprelationIdx, LoadGroupRelation)

            'Check input data
            If String.IsNullOrEmpty(lgroupname) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Name can't be empty")
                Return
            End If

            'Check coherence between Type and Relation inputs + adapt where needed (a warning will be given)
            If Strings.LCase(lgrouptype) = "permanent" Then
                lgrouprelation = "N/A"
            End If

            If Strings.LCase(lgrouptype) = "accidental" Then
                lgrouprelation = "exclusive"
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "LG type 'accidental' requires LG relation 'exclusive'")
            End If

            If Strings.LCase(lgrouptype) = "seismic" And Strings.LCase(lgrouprelation) = "standard" Then
                lgrouprelation = "exclusive"
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "LG relation 'standard' is incompatible with LG type 'seismic'")
            End If

            SE_lgroups(0) = lgroupname
            SE_lgroups(1) = lgrouptype
            SE_lgroups(2) = lgrouprelation

            DA.SetDataList(0, SE_lgroups)

        End Sub

        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                Return My.Resources.LoadGroup_new
            End Get
        End Property

        Public Overrides ReadOnly Property ComponentGuid As Guid
            Get
                Return New Guid("ec03dfed-a115-477c-8653-c919a4709270")
            End Get
        End Property
    End Class
End Namespace

