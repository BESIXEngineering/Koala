Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala

    Public Class MassCombination
        Inherits GH_KoalaComponent
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("MassCombination", "MassCombination",
                "Definition of Mass Combinations",
                "Libraries", New EsaObjectType() {EsaObjectType.MassCombination})
        End Sub

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddTextParameter("MCombiName", "MCombiName", "Name of the Mass COmbination", GH_ParamAccess.item, "MC1")
            pManager.AddTextParameter("MCombiContent", "MCombiContent", "Content of the combination, e.g. 1.0*MG1;1.0*MG2", GH_ParamAccess.item, "1.0*MG1;1.0*MG2")
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("MassCombinations", "MassCombinations", "Mass Combination data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            Dim SE_mcombinations(1) As String

            Dim MassCombinationNames As String = "MC1"
            Dim MassCombinationContent As String = "1*LC2"

            DA.GetData(0, MassCombinationNames)
            DA.GetData(1, MassCombinationContent)

            SE_mcombinations(0) = MassCombinationNames
            SE_mcombinations(1) = MassCombinationContent

            DA.SetDataList(0, SE_mcombinations)

        End Sub

        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                Return My.Resources.MassCombination
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("031aa4f7-bb94-448a-b6fd-9ab4c917266b")
            End Get
        End Property
    End Class
End Namespace