
Imports Grasshopper.Kernel

Namespace Koala
    Public MustInherit Class GH_KoalaComponent
        Inherits GH_Component

        Public NameIndex As Long = 0
        Private ReadOnly _objectTypes As EsaObjectType()

        Public Overridable ReadOnly Property ObjectTypes As EsaObjectType()
            Get
                Return _objectTypes
            End Get
        End Property

        Protected Sub New(name As String, nickname As String, description As String, subcategory As String)
            MyBase.New(name, nickname, description, "Koala", subcategory)
        End Sub

        Public Sub New(name As String, nickname As String, description As String, subcategory As String, compObjectTypes As EsaObjectType())
            Me.New(name, nickname, description, subcategory)
            _objectTypes = compObjectTypes
        End Sub

        Protected Overrides Sub BeforeSolveInstance()
            NameIndex = 0
            MyBase.BeforeSolveInstance()
        End Sub

    End Class
End Namespace
