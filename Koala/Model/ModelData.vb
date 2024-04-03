Imports Koala.Koala
Imports Rhino.DocObjects

Public Class ModelData
    Public NodeMap As Dictionary(Of String, Node)

    Public Scale As Double
    Public MeshSize As Double
    Public UILanguage As String
    Public StructureType As String

    Public Sections(,) As String
    Public Layers(,) As String
    Public Materials As List(Of String)
    Public ProjectInfo As List(Of String)
    Public Selections(,) As String

    Public Nodes As List(Of Node)
    Public BeamInternalNodes(,) As String
    Public Beams(,) As String
    Public Surfaces(,) As String

    Public LoadPanels(,) As String
    Public Openings(,) As String
    Public SlabInternalEdges(,) As String

    Public RigidArms(,) As String

    Public NodeSupports(,) As String
    Public BeamPointSupports(,) As String
    Public BeamLineSupports(,) As String
    Public SurfaceEdgeSupports(,) As String
    Public SurfaceSupports(,) As String


    Public LoadCases(,) As String
    Public LoadGroups(,) As String

    Public NodePointLoads(,) As String
    Public NodePointMoments(,) As String

    Public BeamPointLoads(,) As String
    Public BeamPointMoments(,) As String
    Public BeamLineLoads(,) As String
    Public BeamLineMoments(,) As String

    Public EdgeLoads(,) As String
    Public EdgeMoments(,) As String

    Public SurfaceLoads(,) As String

    Public FreePointLoads(,) As String
    Public FreePointMoments(,) As String
    Public FreeLineLoads(,) As String
    Public FreeSurfaceLoads(,) As String

    Public BeamThermalLoads(,) As String
    Public SurfaceThermalLoads(,) As String

    Public Hinges(,) As String
    Public CrossLinks(,) As String
    Public LineHinges(,) As String

    Public LinearCombinations(,) As String
    Public NonLinearCombinations(,) As String
    Public StabilityCombinations(,) As String

    Public GapElements(,) As String
    Public PretensionElements(,) As String
    Public LimitForceElements(,) As String
    Public Cables(,) As String

    Public Subsoils(,) As String
    Public NonLinearFunctions(,) As String

    Public ArbitraryProfiles(,) As String
    Public IntegrationStrips(,) As String
    Public AveragingStrips(,) As String
    Public SectionOnBeams(,) As String

    ''' <summary>
    ''' Find the matching name of the node as mapped in the model
    ''' </summary>
    Public Function GetNodeName(name As String) As String
        If Not NodeMap.ContainsKey(name) Then
            Throw New ArgumentException("Node not defined in model: " & name)
        End If
        GetNodeName = NodeMap(name).Name
    End Function

    Public Function TryGetEdgeMemberType(name As String, ByRef objectType As EsaObjectType) As Boolean
        If IsInCollection(name, Surfaces) Then
            objectType = EsaObjectType.Member2D
            TryGetEdgeMemberType = True

        ElseIf IsInCollection(name, Openings) Then
            objectType = EsaObjectType.Opening
            TryGetEdgeMemberType = True

        ElseIf IsInCollection(name, SlabInternalEdges) Then
            objectType = EsaObjectType.InternalEdge2D
            TryGetEdgeMemberType = True

        Else
            TryGetEdgeMemberType = False

        End If
    End Function

    Private Function IsInCollection(name As String, collection As String(,), Optional nameIdx As Integer = 0) As Boolean
        If collection Is Nothing Or String.IsNullOrEmpty(name) Then
            IsInCollection = False
            Exit Function
        End If

        For i As Long = 0 To collection.GetLength(0) - 1
            If collection(i, nameIdx) = name Then
                IsInCollection = True
                Exit Function
            End If
        Next

        IsInCollection = False
    End Function

    Public Function FindObjectTypeByName(name As String) As EsaObjectType
        If Nodes IsNot Nothing And Nodes.Any(Function(x) x.Name = name) Then
            FindObjectTypeByName = EsaObjectType.Node

        ElseIf IsInCollection(name, Beams) Then
            FindObjectTypeByName = EsaObjectType.Member1D

        ElseIf IsInCollection(name, Surfaces) Then
            FindObjectTypeByName = EsaObjectType.Member2D

        ElseIf IsInCollection(name, LoadPanels) Then
            FindObjectTypeByName = EsaObjectType.LoadPanel

        ElseIf IsInCollection(name, Openings) Then
            FindObjectTypeByName = EsaObjectType.Opening

        ElseIf IsInCollection(name, SlabInternalEdges) Then
            FindObjectTypeByName = EsaObjectType.InternalEdge2D

        ElseIf IsInCollection(name, NodeSupports, 1) Then
            FindObjectTypeByName = EsaObjectType.NodeSupport

        ElseIf IsInCollection(name, BeamLineSupports, 1) Then
            FindObjectTypeByName = EsaObjectType.BeamLineSupport

        ElseIf IsInCollection(name, BeamPointSupports, 1) Then
            FindObjectTypeByName = EsaObjectType.BeamPointSupport

        ElseIf IsInCollection(name, SurfaceEdgeSupports, 1) Then
            FindObjectTypeByName = EsaObjectType.SurfaceEdgeSupport

        ElseIf IsInCollection(name, SurfaceSupports, 1) Then
            FindObjectTypeByName = EsaObjectType.SurfaceSupport

        ElseIf IsInCollection(name, Subsoils) Then
            FindObjectTypeByName = EsaObjectType.Subsoil

        ElseIf IsInCollection(name, Hinges, 1) Then
            FindObjectTypeByName = EsaObjectType.Hinge

        ElseIf IsInCollection(name, RigidArms) Then
            FindObjectTypeByName = EsaObjectType.RigidArm

        Else
            FindObjectTypeByName = EsaObjectType.Undefined

        End If
    End Function
End Class
