Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala

    Public Class SurfaceSupport2
        Inherits GH_KoalaComponent

        Const DefaultNamePrefix As String = "SSS"
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("Surface Support", "SurfaceSupport",
                "Surface support based on subsoil",
                "Structure", New EsaObjectType() {EsaObjectType.SurfaceSupport})
        End Sub

        Public Overrides ReadOnly Property Exposure As GH_Exposure
            Get
                Return GH_Exposure.secondary
            End Get
        End Property

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddTextParameter("Surfaces", "Surfaces", "Names of surfaces where to apply supports (can be multiple surfaces separated by semi-colon ';')", GH_ParamAccess.item)
            pManager.AddTextParameter("NamePrefix", "NamePrefix", "Object name prefix", GH_ParamAccess.item, DefaultNamePrefix)
            pManager.AddTextParameter("Subsoil", "Subsoil", "Name of supporting subsoil", GH_ParamAccess.item, "Subsoil0")
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("SurfaceSupports", "SurfaceSupports", "SurfaceSupport data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            Dim surfaces As String = ""
            Dim namePrefix As String = DefaultNamePrefix
            Dim subsoil As String = "Subsoil0"

            If (Not DA.GetData(Of String)(0, surfaces)) Then Return
            DA.GetData(1, namePrefix)
            If (Not DA.GetData(2, subsoil)) Then Return

            Dim FlatList As New List(Of System.Object)()

            'initialize some variables
            For Each surfaceName As String In Split(surfaces, ";")
                If (String.IsNullOrWhiteSpace(surfaceName)) Then
                    Continue For
                End If

                FlatList.Add(surfaceName.Trim())
                NameIndex += 1
                FlatList.Add(namePrefix & NameIndex.ToString())
                FlatList.Add(subsoil)
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
                Return My.Resources.SurfaceSupport
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("5938152c-0d94-4751-aa17-c4aa491c5ce3")
            End Get
        End Property
    End Class

End Namespace