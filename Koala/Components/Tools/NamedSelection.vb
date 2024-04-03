Imports System.Collections.Generic
Imports Grasshopper.Kernel


Namespace Koala

    Public Class NamedSelection
        Inherits GH_KoalaComponent

        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("Selection", "Selection",
                "Saved selection of SCIA objects",
                "General", New EsaObjectType() {EsaObjectType.Selection})
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
            pManager.AddTextParameter("Name", "Name", "Name of the saved selection", GH_ParamAccess.item)
            pManager.AddTextParameter("ObjectNames", "ObjectNames", "Names of the SCIA objects to add to the selection (e.g. B1, N5, S2)", GH_ParamAccess.list)
            pManager.AddParameter(New Param_Enum("ObjectTypes", "Type of each object; the type of object is automatically determined if an object with the given name is defined in the same XML export file; if not, you should specify the correct type yourself.", GH_ParamAccess.list, EsaObjectType.Undefined))
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddGenericParameter("Selection", "Selection", "Selection definitions", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
            Dim name As String = ""
            Dim objectNames = New List(Of String)
            Dim objectTypes = New List(Of Integer)
            Dim i As Integer

            If (Not DA.GetData(0, name)) Then Return
            If (Not DA.GetDataList(1, objectNames)) Then Return
            If (Not DA.GetDataList(2, objectTypes)) Then Return

            Dim namesAndTypesParts As New List(Of String)

            For i = 0 To objectNames.Count - 1
                namesAndTypesParts.Add(objectNames(i).Trim)

                Dim objType As EsaObjectType
                If Not objectTypes.Any() Then
                    objType = EsaObjectType.Undefined
                Else
                    objType = CType(objectTypes(Math.Min(objectTypes.Count - 1, i)), EsaObjectType)
                End If

                namesAndTypesParts.Add(GetEnumDescription(objType))
            Next

            Dim SE_selection As New List(Of String) From {
                name,
                String.Join(";", namesAndTypesParts)
            }

            DA.SetDataList(0, SE_selection)
        End Sub


        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.SaveSelection
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("f1e2765c-b3a3-4d4b-b4a2-01d8a1d8228d")
            End Get
        End Property
    End Class

End Namespace