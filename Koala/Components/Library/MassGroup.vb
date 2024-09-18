Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala

    Public Class MassGroup
        Inherits GH_KoalaComponent
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("MassGroup", "MassGroup",
                "Definition of Mass Groups",
                "Libraries", New EsaObjectType() {EsaObjectType.MassGroup})
        End Sub

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddTextParameter("Name", "Name", "Name of the MassGroup (e.g. MG1)", GH_ParamAccess.item, "MG1")
            pManager.AddTextParameter("Base Lc Name", "BaseLc", "Base LoadCase for the MG (e.g. LC1)", GH_ParamAccess.item, "LC1")
            pManager.AddBooleanParameter("Update", "Update", "Update", GH_ParamAccess.item, True)
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("MassGroups", "MassGroups", "", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            Dim SE_mgroups(2) As String

            Dim mgroupname As String = "MG1"
            Dim mgrouplcase As String = "LC1"
            Dim updateflag As Boolean = True

            DA.GetData(0, mgroupname)
            DA.GetData(1, mgrouplcase)
            DA.GetData(2, updateflag)

            SE_mgroups(0) = mgroupname
            SE_mgroups(1) = mgrouplcase
            SE_mgroups(2) = CInt(updateflag)

            DA.SetDataList(0, SE_mgroups)
        End Sub

        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                Return My.Resources.MassGroup
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("96d6e5cb-9441-4499-bd74-9c3cb195a68e")
            End Get
        End Property
    End Class
End Namespace