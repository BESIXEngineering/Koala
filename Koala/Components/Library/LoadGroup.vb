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
            pManager.AddParameter(New Param_Enum("LoadType", "LoadGroup load type", GH_ParamAccess.item, LoadGroupLoadType.Permanent))
            pManager.AddParameter(New Param_Enum("Relation", "LoadGroup relation", GH_ParamAccess.item, LoadGroupRelation.Undefined))
            pManager.AddParameter(New Param_Enum("VariableType", "LoadGroup variable load type", GH_ParamAccess.item, LoadGroupVariableLoadType.Undefined))
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
            Dim lgrouptype As LoadGroupLoadType = LoadGroupLoadType.Permanent
            Dim lgrouprelation As LoadGroupRelation = LoadGroupRelation.Undefined
            Dim lgroupvarloadtype As LoadGroupVariableLoadType = LoadGroupVariableLoadType.Undefined

            If (Not DA.GetData(0, lgroupname)) Then Return
            DA.GetData(0, lgroupname)
            Dim i As Integer
            If DA.GetData(1, i) Then lgrouptype = CType(i, LoadGroupLoadType)
            If DA.GetData(2, i) Then lgrouprelation = CType(i, LoadGroupRelation)
            If DA.GetData(3, i) Then lgroupvarloadtype = CType(i, LoadGroupVariableLoadType)

            'Check input data
            If String.IsNullOrEmpty(lgroupname) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Name can't be empty")
                Return
            End If

            'Check coherence between Type and Relation inputs + adapt where needed (a warning will be given)
            If lgrouptype = LoadGroupLoadType.Permanent Then
                lgrouprelation = LoadGroupRelation.Undefined
            End If

            If lgrouptype = LoadGroupLoadType.Accidental Then
                If lgrouprelation <> LoadGroupRelation.Exclusive And lgrouprelation <> LoadGroupRelation.Undefined Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "LG type 'accidental' requires LG relation 'exclusive'")
                End If
                lgrouprelation = LoadGroupRelation.Exclusive
            End If

            If lgrouptype = LoadGroupLoadType.Seismic And lgrouprelation = LoadGroupRelation.Standard Then
                lgrouprelation = LoadGroupRelation.Exclusive
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "LG relation 'standard' is incompatible with LG type 'seismic'")
            End If

            SE_lgroups(0) = lgroupname
            If lgrouptype = LoadGroupLoadType.Variable And lgroupvarloadtype <> LoadGroupVariableLoadType.Undefined Then
                SE_lgroups(1) = GetEnumDescription(lgrouptype) & "|" & GetEnumDescription(lgroupvarloadtype)
            Else
                SE_lgroups(1) = GetEnumDescription(lgrouptype)
            End If
            SE_lgroups(2) = GetEnumDescription(lgrouprelation)

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

