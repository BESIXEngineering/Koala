Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry

Namespace Koala
    Public Class LoadCase
        Inherits GH_KoalaComponent
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("LoadCase", "LoadCase", "Definition of load cases",
                "Libraries", New EsaObjectType() {EsaObjectType.LoadCase})
        End Sub

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
            pManager.AddTextParameter("Name", "Name", "Load case name (e.g. LC1-selfweight)", GH_ParamAccess.item)
            pManager.AddTextParameter("Type", "Type", "Load case type (SW, permanent, variable, or dynamic)", GH_ParamAccess.item, "permanent")
            'It might be nicer to do this with Enums, but current component outputs Strings
            'pManager.AddParameter(New Param_Enum("Type", "Type", GH_ParamAccess.item, LoadCaseType.permanent))
            pManager.AddTextParameter("LoadGroup", "LoadGroup", "Name of the belonging load group (e.g. LG1)", GH_ParamAccess.item)

        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
            pManager.AddTextParameter("LoadCases", "LoadCases", "LoadCase data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
            Dim SE_loadcases(2) As String

            Dim lcasename As String = ""
            Dim lcasetype As String = "permanent"
            'Dim lcasetype As LoadCaseType
            'Dim lcasetypeIdx As Integer = 1
            Dim lcasegroup As String = ""

            If (Not DA.GetData(0, lcasename)) Then Return
            DA.GetData(0, lcasename)
            DA.GetData(1, lcasetype)
            'DA.GetData(1, lcasetypeIdx)
            'lcasetype = CType(lcasetypeIdx, LoadCaseType)
            If (Not DA.GetData(0, lcasegroup)) Then Return
            DA.GetData(2, lcasegroup)

            'Check and process input data
            If String.IsNullOrEmpty(lcasename) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Name can't be empty")
                Return
            End If

            If String.IsNullOrEmpty(lcasegroup) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "LoadGroup can't be empty")
                Return
            End If

            SE_loadcases(0) = lcasename
            SE_loadcases(1) = lcasetype
            'SE_loadcases(1) = GetEnumDescription(lcasetype)
            SE_loadcases(2) = lcasegroup

            DA.SetDataList(0, SE_loadcases)
        End Sub

        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.LoadCase_new
            End Get
        End Property

        Public Overrides ReadOnly Property ComponentGuid As Guid
            Get
                Return New Guid("3871da1d-b9f6-4b21-9842-4a8c2601a3b3")
            End Get
        End Property
    End Class
End Namespace
