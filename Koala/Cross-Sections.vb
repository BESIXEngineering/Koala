﻿Imports System.Collections.Generic
Imports Grasshopper.GUI.HTML
Imports Grasshopper.Kernel
Imports Rhino.Geometry


Namespace Koala

    Public Class Cross_Sections
        Inherits GH_KoalaComponent
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("CrossSections", "CrossSections",
                "CrossSections description",
                "Libraries", New EsaObjectType() {EsaObjectType.CrossSection})
        End Sub

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddTextParameter("Sections", "Sections", "Name;Formcode;Profile;Material e.g.: CS1; FORM-2; RHS200/150/8.0; S 235", GH_ParamAccess.list, "CS1; FORM-2; RHS200/150/8.0; S 235")


        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("CrossSection", "CrossSection", "CrossSection data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)


            Dim i As Long
            Dim Sections = New List(Of String)


            If (Not DA.GetDataList(Of String)(0, Sections)) Then Return

            Dim SE_sections(Sections.Count, 3) As String
            Dim FlatList As New List(Of System.Object)()
            'a section consists of: Profile name, section definition, material

            Dim item As String
            Dim itemcount As Long
            Dim sectionname As String, sectiondef As String, sectionmat As String, sectioncode As String

            'initialize some variables
            itemcount = 0

            'identify section information in the strings
            For Each item In Sections
                If (item IsNot "") Then
                    sectionname = item.Split(";")(0)
                    sectioncode = item.Split(";")(1)
                    sectiondef = item.Split(";")(2)
                    sectionmat = item.Split(";")(3)
                    SE_sections(itemcount, 0) = sectionname.Trim
                    SE_sections(itemcount, 1) = sectioncode.Trim
                    SE_sections(itemcount, 2) = sectiondef.Trim
                    SE_sections(itemcount, 3) = sectionmat.Trim

                    itemcount += 1
                End If
            Next

            'Flatten data for export as simple list
            FlatList.Clear()

            For i = 0 To itemcount - 1
                FlatList.Add(SE_sections(i, 0))
                FlatList.Add(SE_sections(i, 1))
                FlatList.Add(SE_sections(i, 2))
                FlatList.Add(SE_sections(i, 3))
            Next i
            DA.SetDataList(0, FlatList)


        End Sub

        Protected Overrides Function HtmlHelp_Source() As String

            Dim url As String = “https://jara-broz.gitbook.io/koala/manual/in-depth-component-reference/libraries/cross-sections“
            'run default browser with help
            Process.Start(url)

            Return "GOTO:https://jara-broz.gitbook.io/koala/manual/in-depth-component-reference/libraries/cross-sections"
            'Return "https://jara-broz.gitbook.io/koala/manual/in-depth-component-reference/libraries/cross-sections"
        End Function



        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.Cross_section

            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("f12a737f-c444-497b-9fd8-f3e5f9bff8eb")
            End Get
        End Property
    End Class

End Namespace