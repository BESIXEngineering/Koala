
Imports Grasshopper.Kernel

Namespace Koala
    Public MustInherit Class GH_KoalaComponent
        Inherits GH_Component

        Public Property ObjectTypes As EsaObjectType()

        Public Sub New(name As String, nickname As String, description As String, subcategory As String, _objectTypes As EsaObjectType())
            MyBase.New(name, nickname, description, "Koala", subcategory)
            ObjectTypes = _objectTypes
        End Sub

    End Class
End Namespace
