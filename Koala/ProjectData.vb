Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala

    Public Class ProjectData
        Inherits GH_KoalaComponent
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("ProjectData", "ProjectData",
                "Main project data",
                "General", New EsaObjectType() {EsaObjectType.ProjectData})
        End Sub

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            Dim idx As Integer
            idx = pManager.AddTextParameter("Project name", "ProjectName", "Name of the project", GH_ParamAccess.item)
            pManager.Param(idx).Optional = True
            idx = pManager.AddTextParameter("Part name", "PartName", "Name of the project part", GH_ParamAccess.item)
            pManager.Param(idx).Optional = True
            idx = pManager.AddTextParameter("Description", "Description", "Project description", GH_ParamAccess.item)
            pManager.Param(idx).Optional = True
            idx = pManager.AddTextParameter("Author", "Author", "Project author", GH_ParamAccess.item)
            pManager.Param(idx).Optional = True
            idx = pManager.AddTextParameter("Date", "Date", "Date", GH_ParamAccess.item, DateTime.Now.ToLocalTime.ToString("dd-MM-yyyy"))
            pManager.Param(idx).Optional = True

            idx = pManager.AddParameter(New Param_Enum("StructureType", "Type of analysis model", GH_ParamAccess.item, AnalysisModelStructuralType.GeneralXYZ, False))
            pManager.Param(idx).Optional = True
            idx = pManager.AddParameter(New Param_Enum("UILanguage", "UI language", GH_ParamAccess.item, UILanguage.EN, False))
            pManager.Param(idx).Optional = True
            idx = pManager.AddTextParameter("Materials", "Materials", "Materials: Concrete, Steel, Timber, Aluminium, Masonry, SteelFibreConcrete, Other", GH_ParamAccess.list)
            pManager.Param(idx).Optional = True
            idx = pManager.AddNumberParameter("MeshSize", "MeshSize", "Size of mesh", GH_ParamAccess.item)
            pManager.Param(idx).Optional = True
            idx = pManager.AddNumberParameter("Scale", "Scale", "Scale", GH_ParamAccess.item, 1)
            pManager.Param(idx).Optional = True
            idx = pManager.AddBooleanParameter("RemDuplNodes", "RemDuplNodes", "Output filename", GH_ParamAccess.item, False)
            pManager.Param(idx).Optional = True
            idx = pManager.AddNumberParameter("Tolerance", "Tolerance", "Tolerance to determine duplicate nodes", GH_ParamAccess.item, 0.001)
            pManager.Param(idx).Optional = True

            'pManager.AddTextParameter("NationalCode", "NationalCode", "National code", GH_ParamAccess.item, "EC - EN")
            'pManager.Param(5).Optional = True
            'pManager.AddTextParameter("NationalAnnex", "NationalAnnex", "National annex", GH_ParamAccess.item, "Standard EN")
            'pManager.Param(6).Optional = True
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("ProjectData", "ProjectData", "Project data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            Dim projectName As String = Nothing
            Dim part As String = Nothing
            Dim description As String = Nothing
            Dim author As String = Nothing
            Dim dateOfCreation As String = Nothing

            Dim structureTypeString As String = Nothing
            Dim UILanguageString As String = Nothing

            Dim materials = New List(Of String)
            Dim materialString = Nothing

            Dim meshSize As Double = -1 ' negative mesh size will be ignored when creating xml

            Dim scale As Double = 1
            Dim remDuplNodes As Boolean = False
            Dim tolerance As Double = 0.001

            Dim i As Integer = 0

            'Dim NationalCode As String = "EC - EN"
            'Dim NationalAnnex As String = "Standard EN"
            DA.GetData(0, projectName)
            DA.GetData(1, part)
            DA.GetData(2, description)
            DA.GetData(3, author)
            DA.GetData(4, dateOfCreation)
            'DA.GetData(Of String)(5, NationalCode)
            'DA.GetData(Of String)(6, NationalAnnex)

            If (DA.GetData(5, i)) Then
                structureTypeString = GetEnumDescription(CType(i, AnalysisModelStructuralType))
            End If

            If (DA.GetData(6, i)) Then
                If Not [Enum].IsDefined(GetType(UILanguage), i) Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid StructuralType")
                Else
                    UILanguageString = GetEnumDescription(CType(i, UILanguage))
                End If
            End If

            If DA.GetDataList(7, materials) AndAlso materials.Count > 0 Then
                Dim validMaterials As New List(Of String)
                For Each mat In materials
                    If Not String.IsNullOrWhiteSpace(mat) Then
                        Dim matEnum = GetEnum(Of Material)(mat)
                        validMaterials.Add(GetEnumDescription(matEnum))
                    End If
                Next
                If validMaterials.Count > 0 Then materialString = String.Join(";", validMaterials)
            End If

            If DA.GetData(8, meshSize) Then meshSize = If(meshSize < 0, -1, meshSize)
            DA.GetData(9, scale)
            DA.GetData(10, remDuplNodes)
            DA.GetData(11, tolerance)


            Dim FlatList As New List(Of System.Object)()
            FlatList.Clear()

            FlatList.Add(projectName)
            FlatList.Add(part)
            FlatList.Add(description)
            FlatList.Add(author)
            FlatList.Add(dateOfCreation)
            'FlatList.Add(NationalCode)
            'FlatList.Add(NationalAnnex)
            FlatList.Add(structureTypeString)
            FlatList.Add(UILanguageString)
            FlatList.Add(materialString)
            FlatList.Add(meshSize)
            FlatList.Add(scale)
            FlatList.Add(If(remDuplNodes, "1", "0"))
            FlatList.Add(tolerance)

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
                Return My.Resources.ProjectInfo
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("af8c8fc3-897e-47d3-ab39-5e86d465bd43")
            End Get
        End Property
    End Class

End Namespace