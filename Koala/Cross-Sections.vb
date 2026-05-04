Imports Grasshopper.Kernel


Namespace Koala

    Public Class Cross_Sections
        Inherits GH_KoalaComponent
        Public Sub New()
            MyBase.New("CrossSection", "CrossSection",
                "Definition of catalogue and parametric cross sections",
                "Libraries", New EsaObjectType() {EsaObjectType.CrossSection})
        End Sub

        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddTextParameter("Sections", "Sections", "Name;Formcode;Profile;Material e.g.: CS1; FORM-2; RHS200/150/8.0; S 235", GH_ParamAccess.list, "CS1; FORM-2; RHS200/150/8.0; S 235")
        End Sub

        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("CrossSection", "CrossSection", "CrossSection data", GH_ParamAccess.list)
        End Sub

        Protected Overrides ReadOnly Property HelpDescription As String
            Get
                Return "<p>" &
                    "Get the cross-section definition parts from a cross section string." &
                    "The cross-section string comprises 4 parts separeted by semi-colon (Name;Formcode;Profile;Material):" &
                    "</p>" &
                    "<ul>" &
                        "<li>Name: Name of the section (e.g. CS1)</li>" &
                        "<li>Formcode: Code for the section form (e.g. FORM-2)</li>" &
                        "<li>Profile: Section definition (e.g. RHS200/150/8.0)</li>" &
                        "<li>Material: Material of the section (e.g. S 235)</li>" &
                    "</ul>" &
                    "<p>The Profile definition allows to specify additional properties by appending a json-object-like string.<br /> E.g. property multipliers can be added as such: RHS200/150/8.0{kA:3.0, kIy:2.0}</p>" &
                    "<p>Supported properties: [MULTIPLIERS] kA, kAy, kAz, kIx, kIy, kIz</p>" &
                    "<h3>Catalogue sections</h3>" &
                    "<p>Define formcode FORM-1, FORM-2, etc. (as defined by SCIA) and the name of a profile available in the matching SCIA catalogue.</p>" &
                    "<p>Examples:</p>" &
                    "<ul><li>CS1; FORM-1; IPE180; S 355</li><li>CS2; FORM-2; RHS200/150/8.0; S 235</li></ul>" &
                    "<h3>Parametric sections</h3>" &
                    "<p>Define formcode CONC, STEE or TIMB and a section definition comprising the shape type and all required parameter values.</p>" &
                    "<p>Examples:</p>" &
                    "<ul><li>CS3; CONC; RECT300x500; C30/37</li><li>CS4; STEE; PIPE200x10; S 235</li><li>CS5; TIMB; CIRC300; C14 (EN 338)</li></ul>" &
                    "<h3>Numerical sections</h3>" &
                    "<p>Define formcode NUME and a section definition comprising the cross section properties in a json-object-like string format. Property values are given in millimeter-based units (like in SCIA interface).</p>" &
                    "<p>Supported properties are: A, Ay, Az, It, Iy, Iz, Iw, Wely, Welz, Wply, Wplz, dy, dz, cYUCS, cZUCS, α, AL, AD, Mply+, Mply-, Mplz+, Mplz-, Rgy, Rgz, βy, βz, IYLCS, IZLCS, IYZLCS.</p>" &
                    "<p>Example:</p>" &
                    "<ul><li>CS6; NUME; {A: 5000, Iy:3.7e+7, Iz:1.4e+7, Rgy:82.84, Rgz:49.82}; S235</li></ul>"
            End Get
        End Property

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