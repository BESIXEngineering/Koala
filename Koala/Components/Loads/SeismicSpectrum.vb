Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Rhino.Geometry
Namespace Koala
    Public Class SeismicSpectrum
        Inherits GH_KoalaComponent
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("SeismicSpectrum", "SeismicSpectrum", "Define a Seismic Spectrum",
                "Libraries", New EsaObjectType() {EsaObjectType.SeismicSpectrum})
        End Sub

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_InputParamManager)
            pManager.AddTextParameter("Name", "Name", "Spectrum name", GH_ParamAccess.item)
            pManager.AddTextParameter("Frequency", "Frequency", "List of the frequencies in Hz ('f1;f2;f3;...' Not required if wavelengths are provided)", GH_ParamAccess.item, "")
            pManager.AddTextParameter("Wavelength", "Wavelength", "List of the wavelengths in s ('L1;L2;L3;...' Not required if frequencies are provided)", GH_ParamAccess.item, "")
            pManager.AddTextParameter("Acceleration", "Acceleration", "List of accelerations in m/s^2 ('a1;a2;a3;...')", GH_ParamAccess.item, "")

        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_OutputParamManager)
            pManager.AddTextParameter("Spectra", "Spectra", "Seismic spectra data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
            Dim SE_spectra(3) As String

            Dim specname As String = ""
            Dim freq As String = ""
            Dim wavelength As String = ""
            Dim acceleration As String = ""

            If (Not DA.GetData(0, specname)) Then Return
            DA.GetData(0, specname)
            DA.GetData(1, freq)
            DA.GetData(2, wavelength)
            If (Not DA.GetData(3, acceleration)) Then Return
            DA.GetData(3, acceleration)

            'Check and process input data
            If String.IsNullOrEmpty(specname) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Name can't be empty")
                Return
            End If

            If String.IsNullOrEmpty(freq) And String.IsNullOrEmpty(wavelength) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Frequency and wavelength can't both be empty")
                Return
            End If

            If String.IsNullOrEmpty(acceleration) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Acceleration can't be empty")
                Return
            End If

            If String.IsNullOrEmpty(freq) Then
                Dim wavelength_items() As String = Split(wavelength, ";")
                Dim freq_items(wavelength_items.Length - 1) As String
                Dim itemcount As Long = 0

                If wavelength_items.Length <> Split(acceleration, ";").Length Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Acceleration list length does not match")
                    Return
                End If

                For Each item In wavelength_items
                    freq_items(itemcount) = Math.Round(1 / wavelength_items(itemcount), 2)
                    itemcount += 1
                Next

                freq = Join(freq_items, ";")

            ElseIf String.IsNullOrEmpty(wavelength) Then
                Dim freq_items() As String = Split(freq, ";")
                Dim wavelength_items(freq_items.Length - 1) As String
                Dim itemcount As Long = 0

                If freq_items.Length <> Split(acceleration, ";").Length Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Acceleration list length does not match")
                    Return
                End If

                For Each item In freq_items
                    wavelength_items(itemcount) = Math.Round(1 / freq_items(itemcount), 2)
                    itemcount += 1
                Next

                wavelength = Join(wavelength_items, ";")

            Else
                Dim wavelength_items() As String = Split(wavelength, ";")
                Dim freq_items() As String = Split(freq, ";")

                If freq_items.Length <> wavelength_items.Length Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Frequency and wavelength list lengths do not match")
                    Return
                End If

                If (wavelength_items.Length <> Split(acceleration, ";").Length) Or (wavelength_items.Length <> Split(acceleration, ";").Length) Then
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Acceleration list length does not match")
                    Return
                End If
            End If

            SE_spectra(0) = specname
            SE_spectra(1) = freq
            SE_spectra(2) = wavelength
            SE_spectra(3) = acceleration

            DA.SetDataList(0, SE_spectra)

        End Sub

        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.SeismicSpectrum
            End Get
        End Property

        Public Overrides ReadOnly Property ComponentGuid As Guid
            Get
                Return New Guid("3a8d2466-d802-4e4a-abbe-71ace28c029e")
            End Get
        End Property
    End Class
End Namespace