Imports System.Collections.Generic
Imports Grasshopper.Kernel


Namespace Koala

    Public Class NonlinearFunction
        Inherits GH_KoalaComponent

        Public Sub New()
            MyBase.New("NonlinearFunction", "NLFunction",
                "Add a Nonlinear Function",
                "Libraries", New EsaObjectType() {EsaObjectType.NonLinearFunction})
        End Sub

        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddTextParameter("Name", " Name", "Name of nonlinear function", GH_ParamAccess.item, "NLF1")
            pManager.AddParameter(New Param_Enum("Type", "Type of function", GH_ParamAccess.item, NLFunctionType.Translation))
            pManager.AddParameter(New Param_Enum("PositiveEnd", "Type of Positive end", GH_ParamAccess.item, NLFunctionEndType.Rigid))
            pManager.AddParameter(New Param_Enum("NegativeEnd", "Type of Negative end", GH_ParamAccess.item, NLFunctionEndType.Rigid))
            pManager.AddTextParameter("GraphPoints", " GraphPoints", "GraphPoints", GH_ParamAccess.list, {"-1;0"})
        End Sub

        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("NonLinearFunction", "NonLinearFunction", "NonLinearFunction data", GH_ParamAccess.list)
        End Sub

        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)

            Dim Name As String = "NFL1"
            Dim i As Integer
            Dim Points As New List(Of String)

            If (Not DA.GetData(0, Name)) Then Return

            If (Not DA.GetData(1, i)) Then Return
            Dim NLType As NLFunctionType = CType(i, NLFunctionType)

            If (Not DA.GetData(2, i)) Then Return
            Dim PosEnd As NLFunctionEndType = CType(i, NLFunctionEndType)

            If (Not DA.GetData(3, i)) Then Return
            Dim NegEnd As NLFunctionEndType = CType(i, NLFunctionEndType)

            If (Not DA.GetDataList(4, Points)) Then Return

            Dim FlatList As New List(Of Object)()
            Dim GraphPoints As String = ""

            For i = 0 To Points.Count - 1
                If (Not i = Points.Count - 1) Then
                    GraphPoints += Points(i) + "|"
                Else
                    GraphPoints += Points(i)
                End If
            Next i

            FlatList.Add(Name)
            FlatList.Add(GetEnumDescription(NLType))
            FlatList.Add(GetEnumDescription(PosEnd))
            FlatList.Add(GetEnumDescription(NegEnd))
            FlatList.Add(GraphPoints)

            DA.SetDataList(0, FlatList)
        End Sub

        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                Return My.Resources.NonLinearFunction
            End Get
        End Property

        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("d7abd3f8-a1aa-4e46-b450-53ea26148591")
            End Get
        End Property

    End Class
End Namespace