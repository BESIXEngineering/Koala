﻿Imports Grasshopper.Kernel

Public Class KoalaInfo
    Inherits GH_AssemblyInfo

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "Koala"
        End Get
    End Property
    Public Overrides ReadOnly Property Icon As System.Drawing.Bitmap
        Get
            'Return a 24x24 pixel bitmap to represent this GHA library.
            Return Nothing
        End Get
    End Property
    Public Overrides ReadOnly Property Description As String
        Get
            'Return a short string describing the purpose of this GHA library.
            Return "Plugin which convert Grasshopper model to SCIA Engineer application"
        End Get
    End Property
    Public Overrides ReadOnly Property Id As System.Guid
        Get
            Return New System.Guid("66f67903-c5e2-422e-9746-4b7cfe843dbb")
        End Get
    End Property

    Public Overrides ReadOnly Property AuthorName As String
        Get
            'Return a string identifying you or your company.
            Return "Originally developed by Jaroslav Broz at Strawberrylab for SCIA. Currently maintained by Kelvin Roovers at BESIX."
        End Get
    End Property
    Public Overrides ReadOnly Property AuthorContact As String
        Get
            'Return a string representing your preferred contact details.
            Return "jara.broz@gmail.com"
        End Get
    End Property
End Class
