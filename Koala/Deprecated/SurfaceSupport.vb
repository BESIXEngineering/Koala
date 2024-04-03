Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala

    <System.Obsolete("Use 'SurfaceSupport2' instead which makes better use of Grasshopper's default data matching algorithm.")>
    Public Class SurfaceSupport
        Inherits GH_KoalaComponent
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("SurfaceSupport", "SurfaceSupport",
                "SurfaceSupport description",
                "Structure", New EsaObjectType() {EsaObjectType.SurfaceSupport})
        End Sub

        Public Overrides ReadOnly Property Exposure As GH_Exposure
            Get
                Return GH_Exposure.hidden
            End Get
        End Property

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddTextParameter("SurfList", "SurfList", "List of 2D member names where to apply load", GH_ParamAccess.list)
            pManager.AddTextParameter("Subsoil", "Subsoil", "Definition of subsoil", GH_ParamAccess.item, "Subsoil0")
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

            Dim SurfList = New List(Of String)
            Dim Subsoil As String = "Subsoil0"

            If (Not DA.GetDataList(Of String)(0, SurfList)) Then Return
            If (Not DA.GetData(1, Subsoil)) Then Return

            Dim SE_surfsupports(SurfList.Count, 2)
            Dim FlatList As New List(Of System.Object)()
            'a load consists of: load case, surface name, coord. system (GCS/LCS), direction (X, Y, Z), value (kN/m)

            For Each item In SurfList
                FlatList.Add(item.Trim())
                NameIndex += 1
                FlatList.Add("SurfaceSupport" & NameIndex.ToString())
                FlatList.Add(Subsoil)
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
                Return New Guid("5938152c-0d94-4751-aa17-c4aa491c5cd2")
            End Get
        End Property
    End Class

End Namespace