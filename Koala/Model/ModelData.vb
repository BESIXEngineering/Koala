Imports Koala.Koala

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

    Public Nodes As List(Of Node)
    Public BeamInternalNodes(,) As String
    Public Beams(,) As String
    Public Surfaces(,) As String

    Public LoadPanels(,) As String
    Public Openings(,) As String
    Public SlabInternalEdges(,) As String

    Public RidgidArms(,) As String

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
        Dim i As Long
        For i = 0 To Surfaces.GetLength(0) - 1
            If Surfaces(i, 0) = name Then
                objectType = EsaObjectType.Member2D
                TryGetEdgeMemberType = True
                Exit Function
            End If
        Next

        For i = 0 To Openings.GetLength(0) - 1
            If Openings(i, 0) = name Then
                objectType = EsaObjectType.Opening
                TryGetEdgeMemberType = True
                Exit Function
            End If
        Next

        For i = 0 To SlabInternalEdges.GetLength(0) - 1
            If SlabInternalEdges(i, 0) = name Then
                objectType = EsaObjectType.InternalEdge2D
                TryGetEdgeMemberType = True
                Exit Function
            End If
        Next

        TryGetEdgeMemberType = False
    End Function

End Class
