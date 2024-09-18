Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics.Eventing.Reader
Imports System.IO
Imports System.Windows.Forms
Imports Eto.Forms
Imports Grasshopper.Kernel.Geometry.SpatialTrees
Imports Grasshopper.Kernel.Parameters
Imports Grasshopper.Kernel.Types.Transforms
Imports Koala.Koala
Module HelperTools

#Region "Enum options"

    Public Sub AddOptionstoMenuDOFTransition(menuItem As Param_Integer)
        menuItem.AddNamedValue("Free", 0)
        menuItem.AddNamedValue("Rigid", 1)
        menuItem.AddNamedValue("Flexible", 2)
        menuItem.AddNamedValue("Rigid press only", 3)
        menuItem.AddNamedValue("Rigid tension only", 4)
        menuItem.AddNamedValue("Flexible press only", 5)
        menuItem.AddNamedValue("Flexible tension only", 6)
        menuItem.AddNamedValue("Nonlinear", 7)
    End Sub

    Public Sub AddOptionstoMenuDOFRotation(menuItem As Param_Integer)
        menuItem.AddNamedValue("Free", 0)
        menuItem.AddNamedValue("Rigid", 1)
        menuItem.AddNamedValue("Flexible", 2)
        menuItem.AddNamedValue("Nonlinear", 7)
    End Sub

    Public Sub AddOptionsToMenuLanguage(menuItem As Param_Integer)
        menuItem.AddNamedValue("English", 0)
        menuItem.AddNamedValue("Nederlands", 1)
        menuItem.AddNamedValue("Français", 2)
        menuItem.AddNamedValue("Deutsch", 3)
        menuItem.AddNamedValue("Čeština", 4)
        menuItem.AddNamedValue("Slovenčina", 5)

    End Sub
    Public Function GetStringForLanguage(item As Integer) As String
        Select Case item
            Case 0
                Return "English"
            Case 1
                Return "Nederlands"
            Case 2
                Return "Français"
            Case 3
                Return "Deutsch"
            Case 4
                Return "Čeština"
            Case 5
                Return "Slovenčina"
            Case Else
                Return "English"
        End Select
    End Function

    Public Function GetNumberForUILangauge(item As String) As Long
        Select Case item
            Case "English"
                Return 0
            Case "Nederlands"
                Return 1
            Case "Français"
                Return 2
            Case "Deutsch"
                Return 3
            Case "Čeština"
                Return 4
            Case "Slovenčina"
                Return 5
            Case Else
                Return 0
        End Select
    End Function

    Public Sub AddOptionsToMenuCalculationType(menuItem As Param_Integer)
        menuItem.AddNamedValue("LIN", 0)
        menuItem.AddNamedValue("NEL", 1)
        menuItem.AddNamedValue("STB", 2)
        menuItem.AddNamedValue("EIG", 3)
    End Sub

    Public Function GetStringForCalculationType(item As Integer) As String
        Select Case item
            Case 0
                Return "LIN"
            Case 1
                Return "NEL"
            Case 2
                Return "STB"
            Case 3
                Return "EIG"
            Case Else
                Return "LIN"
        End Select
    End Function

    Public Sub AddOptionstoMenuMemberSystemLine(menuItem As Param_Integer)
        menuItem.AddNamedValue("Centre", 1)
        menuItem.AddNamedValue("Top", 2)
        menuItem.AddNamedValue("Bottom", 4)
        menuItem.AddNamedValue("Left", 8)
        menuItem.AddNamedValue("Top left", 10)
        menuItem.AddNamedValue("Bottom left", 12)
        menuItem.AddNamedValue("Right", 16)
        menuItem.AddNamedValue("Top right", 18)
        menuItem.AddNamedValue("Bottom right", 20)

    End Sub

    Public Function GetStringForMemberSystemLineOrPlane(item As Integer) As String
        Select Case item
            Case 1
                Return "Centre"
            Case 2
                Return "Top"
            Case 4
                Return "Bottom"
            Case 8
                Return "Left"
            Case 12
                Return "Top left"
            Case 14
                Return "Bottom left"
            Case 16
                Return "Right"
            Case 18
                Return "Top right"
            Case 20
                Return "Bottom right"
            Case Else
                Return "Centre"
        End Select
    End Function

    Public Sub AddOptionstoMenuMemberSystemPlane(menuItem As Param_Integer)
        menuItem.AddNamedValue("Centre", 1)
        menuItem.AddNamedValue("Top", 2)
        menuItem.AddNamedValue("Bottom", 4)
    End Sub



    Public Sub AddOptionstoMenuFEMNLType2D(menuItem As Param_Integer)
        menuItem.AddNamedValue("none", 0)
        menuItem.AddNamedValue("Press only", 1)
        menuItem.AddNamedValue("Membrane", 2)
    End Sub

    Public Function GetStringForFEMNLType2D(item As Integer) As String
        Select Case item
            Case 0
                Return "none"
            Case 1
                Return "Press only"
            Case 2
                Return "Membrane"
            Case Else
                Return "none"
        End Select
    End Function

    Public Sub AddOptionstoMenuStructureType(menuItem As Param_Integer)
        menuItem.AddNamedValue("Beam", 0)
        menuItem.AddNamedValue("Truss XZ", 1)
        menuItem.AddNamedValue("Frame XZ", 2)
        menuItem.AddNamedValue("Truss XYZ", 3)
        menuItem.AddNamedValue("Frame XYZ", 4)
        menuItem.AddNamedValue("Grid XY", 5)
        menuItem.AddNamedValue("Plate XY", 6)
        menuItem.AddNamedValue("Wall XY", 7)
        menuItem.AddNamedValue("General XYZ", 8)
    End Sub

    Public Function GetStringForStructureType(item As Integer) As String
        Select Case item
            Case 0
                Return "Beam"
            Case 1
                Return "Truss XZ"
            Case 2
                Return "Frame XZ"
            Case 3
                Return "Truss XYZ"
            Case 4
                Return "Frame XYZ"
            Case 5
                Return "Grid XY"
            Case 6
                Return "Plate XY"
            Case 7
                Return "Wall XY"
            Case 8
                Return "General XYZ"
            Case Else
                Return "General XYZ"
        End Select
    End Function

    Public Function GetStringForDOF(item As Integer) As String
        Select Case item
            Case 0
                Return "Free"
            Case 1
                Return "Rigid"
            Case 2
                Return "Flexible"
            Case 3
                Return "Rigid press only"
            Case 4
                Return "Ridig tension only"
            Case 5
                Return "Flexible press only"
            Case 6
                Return "Flexible tension only"

            Case 7
                Return "Nonlinear"
            Case Else
                Return "Free"
        End Select

    End Function

    Public Sub AddOptionsToMenuBeamFEMtype(menuItem As Param_Integer)
        menuItem.AddNamedValue("standard", 0)
        menuItem.AddNamedValue("axial force only", 1)
    End Sub

    Public Function GetStringForBeamFEMtype(item As Integer) As String
        Select Case item
            Case 0
                Return "standard"
            Case 1
                Return "axial force only"
            Case Else
                Return "standard"
        End Select
    End Function

    Public Sub AddOptionsToMenuBeamType(menuItem As Param_Integer)
        menuItem.AddNamedValue("general", 0)
        menuItem.AddNamedValue("beam", 1)
        menuItem.AddNamedValue("column", 2)
        menuItem.AddNamedValue("gable column", 3)
        menuItem.AddNamedValue("secondary column", 4)
        menuItem.AddNamedValue("rafter", 5)
        menuItem.AddNamedValue("purlin", 6)
        menuItem.AddNamedValue("roof bracing", 7)
        menuItem.AddNamedValue("wall bracing", 8)
        menuItem.AddNamedValue("girt", 9)
        menuItem.AddNamedValue("truss chord", 10)
        menuItem.AddNamedValue("truss diagonal", 11)
        menuItem.AddNamedValue("plate rib", 12)
        menuItem.AddNamedValue("beam slab", 13)
    End Sub
    Public Function GetStringForBeamType(item As Integer) As String
        Select Case item
            Case 0
                Return "general"
            Case 1
                Return "beam"
            Case 2
                Return "column"
            Case 3
                Return "gable column"
            Case 4
                Return "secondary column"
            Case 5
                Return "rafter"
            Case 6
                Return "purlin"
            Case 7
                Return "roof bracing"
            Case 8
                Return "wall bracing"
            Case 9
                Return "girt"
            Case 10
                Return "truss chord"
            Case 11
                Return "truss diagonal"
            Case 12
                Return "plate rib"
            Case 13
                Return "beam slab"
            Case Else
                Return "general"
        End Select
    End Function
    Public Sub AddOptionsToMenuCrosslinkType(menuItem As Param_Integer)
        menuItem.AddNamedValue("Fixed", 0)
        menuItem.AddNamedValue("Hinged", 1)
    End Sub

    Public Function GetStringForCrosslinkType(item As Integer) As String
        Select Case item
            Case 0
                Return "Fixed"
            Case 1
                Return "Hinged"
            Case 2
                Return "Coupler"
            Case Else
                Return "Fixed"
        End Select
    End Function


    Public Sub AddOptionstoMenuLinCombiType(menuitem As Param_Integer)
        menuitem.AddNamedValue("Envelope - ultimate", 0)
        menuitem.AddNamedValue("Envelope - serviceability", 1)
        menuitem.AddNamedValue("Linear - ultimate", 2)
        menuitem.AddNamedValue("Linear - serviceability", 3)
        menuitem.AddNamedValue("EN-ULS (STR/GEO) Set B", 4)
        menuitem.AddNamedValue("EN-Accidental 1", 5)
        menuitem.AddNamedValue("EN-Accidental 2", 6)
        menuitem.AddNamedValue("EN-Seismic", 7)
        menuitem.AddNamedValue("EN-SLS Characteristic", 8)
        menuitem.AddNamedValue("EN-SLS Frequent", 9)
        menuitem.AddNamedValue("EN-SLS Quasi-permanent", 10)
        menuitem.AddNamedValue("EN-ULS (STR/GEO) Set C", 11)
    End Sub

    Public Function GetStringForLinCombiType(item As Integer)
        Select Case item
            Case 0
                Return "Envelope - ultimate"
            Case 1
                Return "Envelope - serviceability"
            Case 2
                Return "Linear - ultimate"
            Case 3
                Return "Linear - serviceability"
            Case 4
                Return "EN-ULS (STR/GEO) Set B"
            Case 5
                Return "EN-Accidental 1"
            Case 6
                Return "EN-Accidental 2"
            Case 7
                Return "EN-Seismic"
            Case 8
                Return "EN-SLS Characteristic"
            Case 9
                Return "EN-SLS Frequent"
            Case 10
                Return "EN-SLS Quasi-permanent"
            Case 11
                Return "EN-ULS (STR/GEO) Set C"
            Case Else
                Return "Linear - ultimate"
        End Select
    End Function

    Public Sub AddOptionstomenuItemNLCombiType(menuitem As Param_Integer)
        menuitem.AddNamedValue("Ultimate", 0)
        menuitem.AddNamedValue("Serviceability", 1)
    End Sub

    Public Function GetStringForNLCombiType(item As Integer) As String
        Select Case item
            Case 0
                Return "Ultimate"
            Case 1
                Return "Serviceability"
            Case Else
                Return "Ultimate"
        End Select
    End Function

    Public Sub AddOptionsToMenuDistributionOfLoad(menuitem As Param_Integer)
        menuitem.AddNamedValue("Uniform", 0)
        menuitem.AddNamedValue("Trapez", 1)
    End Sub

    Public Function GetStringFromDistributionOfLoad(item As Integer) As String
        Select Case item
            Case 0
                Return "Uniform"
            Case 1
                Return "Trapez"
            Case Else
                Return "Uniform"
        End Select
    End Function

    Public Sub AddOptionsToMenuDistributionOfSurfaceLoad(menuitem As Param_Integer)
        menuitem.AddNamedValue("Uniform", 0)
        menuitem.AddNamedValue("DirectionX", 1)
        menuitem.AddNamedValue("DirectionY", 2)
    End Sub

    Public Function GetStringFromDistributionOfSurfaceLoad(item As Integer) As String
        Select Case item
            Case 0
                Return "Uniform"
            Case 1
                Return "DirectionX"
            Case 2
                Return "DirectionY"
            Case Else
                Return "Uniform"
        End Select
    End Function

    Public Sub AddOptionsToMenuDirection(menuitem As Param_Integer)
        menuitem.AddNamedValue("X", 0)
        menuitem.AddNamedValue("Y", 1)
        menuitem.AddNamedValue("Z", 2)
    End Sub

    Public Function GetStringFromDirection(item As Integer) As String
        Select Case item
            Case 0
                Return "X"
            Case 1
                Return "Y"
            Case 2
                Return "Z"
            Case Else
                Return "Z"
        End Select
    End Function


    Public Sub AddOptionsToMenuDirectionMoment(menuitem As Param_Integer)
        menuitem.AddNamedValue("Mx", 0)
        menuitem.AddNamedValue("My", 1)
        menuitem.AddNamedValue("Mz", 2)
    End Sub

    Public Function GetStringFromDirectionMoment(item As Integer) As String
        Select Case item
            Case 0
                Return "Mx"
            Case 1
                Return "My"
            Case 2
                Return "Mz"
            Case Else
                Return "Mz"
        End Select
    End Function

    Public Sub AddOptionsToMenuOrigin(menuitem As Param_Integer)
        menuitem.AddNamedValue("From start", 0)
        menuitem.AddNamedValue("From end", 1)
    End Sub

    Public Function GetStringFromOrigin(item As Integer) As String
        Select Case item
            Case 0
                Return "From start"
            Case 1
                Return "From end"
            Case Else
                Return "From start"
        End Select
    End Function


    Public Sub AddOptionsToMenuCoordDefinition(menuitem As Param_Integer)
        menuitem.AddNamedValue("Rela", 0)
        menuitem.AddNamedValue("Abso", 1)
    End Sub

    Public Function GetStringFromCoordDefinition(item As Integer) As String
        Select Case item
            Case 0
                Return "Rela"
            Case 1
                Return "Abso"
            Case Else
                Return "Rela"
        End Select
    End Function

    Public Sub AddOptionsToMenuCoordSysLine(menuitem As Param_Integer)
        menuitem.AddNamedValue("GCS - Length", 0)
        menuitem.AddNamedValue("GCS - Projection", 1)
        menuitem.AddNamedValue("LCS", 2)
    End Sub

    Public Function GetStringFromCoordSysLine(item As Integer) As String
        Select Case item
            Case 0
                Return "GCS - Length"
            Case 1
                Return "GCS - Projection"
            Case 2
                Return "LCS"
            Case 3
                Return "Member LCS"
            Case 4
                Return "GCS"
            Case Else
                Return "GCS - Length"
        End Select
    End Function

    Public Sub AddOptionsToMenuThermalDistribution(menuitem As Param_Integer)
        menuitem.AddNamedValue("Constant", 0)
        menuitem.AddNamedValue("Linear", 1)
    End Sub


    Public Function GetStringFromMenuThermalDistribution(item As Integer) As String
        Select Case item
            Case 0
                Return "Constant"
            Case 1
                Return "Linear"

            Case Else
                Return "Constant"
        End Select
    End Function

    Public Sub AddOptionsToMenuCoordSysPoint(menuitem As Param_Integer)
        menuitem.AddNamedValue("GCS", 0)
        menuitem.AddNamedValue("LCS", 1)
    End Sub

    Public Function GetStringFromCoordSysPoint(item As Integer) As String
        Select Case item
            Case 0
                Return "GCS"
            Case 1
                Return "LCS"
            Case Else
                Return "GCS"
        End Select
    End Function


    Public Sub AddOptionsToMenuValidity(menuitem As Param_Integer)
        menuitem.AddNamedValue("All", 0)
        menuitem.AddNamedValue("-Z", 1)
        menuitem.AddNamedValue("+Z", 2)
        menuitem.AddNamedValue("From-to", 3)
        menuitem.AddNamedValue("Z=0", 4)
        menuitem.AddNamedValue("-Z (incl. 0)", 5)
        menuitem.AddNamedValue("+Z (incl. 0)", 6)
    End Sub


    Public Function GetStringFromValidity(item As Integer) As String
        Select Case item
            Case 0
                Return "All"
            Case 1
                Return "-Z"
            Case 2
                Return "+Z"
            Case 3
                Return "From-to"
            Case 4
                Return "Z=0"
            Case 5
                Return "-Z (incl. 0)"
            Case 6
                Return "+Z (incl. 0)"
            Case Else
                Return "All"
        End Select
    End Function


    Public Sub AddOptionsToMenuSelection(menuitem As Param_Integer)
        menuitem.AddNamedValue("Auto", 0)
        menuitem.AddNamedValue("Select", 1)

    End Sub
    Public Function GetStringFromMenuSelection(item As Integer) As String
        Select Case item
            Case 0
                Return "Auto"
            Case 1
                Return "Select"
            Case Else
                Return "Auto"
        End Select
    End Function


    Public Sub AddOptionsToMenuCoordSysFreeLine(menuitem As Param_Integer)
        menuitem.AddNamedValue("GCS - Length", 0)
        menuitem.AddNamedValue("GCS - Projection", 1)
        menuitem.AddNamedValue("Member LCS", 3)
    End Sub
    Public Sub AddOptionsToMenuBeamNLTypePT(menuitem As Param_Integer)
        menuitem.AddNamedValue("Press only", 0)
        menuitem.AddNamedValue("Tension only", 1)
    End Sub

    Public Function GetStringFromBeamNLTypePT(item As Integer) As String
        Select Case item
            Case 0
                Return "Press only"
            Case 1
                Return "Tension only"
            Case Else
                Return "Press only"
        End Select
    End Function
    Public Sub AddOptionsToMenuCoordSysFreePoint(menuitem As Param_Integer)
        menuitem.AddNamedValue("GCS", 4)
        menuitem.AddNamedValue("Member LCS", 3)
    End Sub

    Public Sub AddOptionsToMenuBeamNLGapDirection(menuitem As Param_Integer)
        menuitem.AddNamedValue("Both directions", 0)
        menuitem.AddNamedValue("Press only", 1)
        menuitem.AddNamedValue("Tension only", 1)
    End Sub

    Public Function GetStringFromBeamNLGapDirection(item As Integer) As String
        Select Case item
            Case 0
                Return "Both directions"
            Case 1
                Return "Press only"
            Case 2
                Return "Tension only"
            Case Else
                Return "Both directions"
        End Select
    End Function
    Public Sub AddOptionsToMenuBeamNLGapPosition(menuitem As Param_Integer)
        menuitem.AddNamedValue("Begin", 0)
        menuitem.AddNamedValue("End", 1)

    End Sub

    Public Function GetStringFromBeamNLGapPosition(item As Integer) As String
        Select Case item
            Case 0
                Return "Begin"
            Case 1
                Return "End"
            Case Else
                Return "Begin"
        End Select
    End Function


    Public Sub AddOptionsToMenuBeamNLLimitForceType(menuitem As Param_Integer)
        menuitem.AddNamedValue("Buckling ( results zero )", 0)
        menuitem.AddNamedValue("Plastic yielding", 1)

    End Sub

    Public Function GetStringFromBeamNLLimitForceType(item As Integer) As String
        Select Case item
            Case 0
                Return "Buckling ( results zero )"
            Case 1
                Return "Plastic yielding"
            Case Else
                Return "Buckling ( results zero )"
        End Select
    End Function
    Public Sub AddOptionsToMenuBeamNLLimnitForceDirection(menuitem As Param_Integer)
        menuitem.AddNamedValue("Limit compression", 0)
        menuitem.AddNamedValue("Limit tension", 1)

    End Sub

    Public Function GetStringFromBeamNLLimnitForceDirection(item As Integer) As String
        Select Case item
            Case 0
                Return "Limit compression"
            Case 1
                Return "Limit tension"
            Case Else
                Return "Limit compression"
        End Select
    End Function


    Public Sub AddOptionsToMenuHingePosition(menuitem As Param_Integer)
        menuitem.AddNamedValue("Begin", 0)
        menuitem.AddNamedValue("End", 1)
        menuitem.AddNamedValue("Both", 2)

    End Sub

    Public Function GetStringFromHingePosition(item As Integer) As String
        Select Case item
            Case 0
                Return "Begin"
            Case 1
                Return "End"
            Case 2
                Return "Both"
            Case Else
                Return "Both"
        End Select
    End Function

    Public Sub AddOptionstoMenuSwapOrientation(menuItem As Param_Integer)
        menuItem.AddNamedValue("No", 0)
        menuItem.AddNamedValue("Yes", 1)
    End Sub

    Public Function GetStringFromSwapOrientation(item As Integer) As String
        Select Case item
            Case 0
                Return "No"
            Case 1
                Return "Yes"
            Case Else
                Return "No"
        End Select
    End Function

    Public Sub AddOptionsToMenuPanelType(menuItem As Param_Integer)
        menuItem.AddNamedValue("To panel nodes", 0)
        menuItem.AddNamedValue("To panel edges", 1)
        menuItem.AddNamedValue("To panel edges and beams", 2)
    End Sub
    Public Function GetStringForPanelType(item As Integer) As String
        Select Case item
            Case 0
                Return "To panel nodes"
            Case 1
                Return "To panel edges"
            Case 2
                Return "To panel edges and beams"

            Case Else
                Return "To panel edges and beams"
        End Select
    End Function


    Public Sub AddOptionsToMenuTransferMethod(menuItem As Param_Integer)
        menuItem.AddNamedValue("Accurate(FEM),fixed link with beams", 0)
        menuItem.AddNamedValue("Standard", 1)
        menuItem.AddNamedValue("Accurate(FEM),hinged link with beams", 2)
        menuItem.AddNamedValue("Tributary area", 3)
    End Sub
    Public Function GetStringForTransferMethod(item As Integer) As String
        Select Case item
            Case 0
                Return "Accurate(FEM),fixed link with beams"
            Case 1
                Return "Standard"
            Case 2
                Return "Accurate(FEM),hinged link with beams"
            Case 3
                Return "Tributary area"
            Case Else
                Return "Standard"
        End Select
    End Function

    Sub AddOptionsToMenuSupportingMembersValidity(menuItem As Param_Integer)
        menuItem.AddNamedValue("All", 0)
        menuItem.AddNamedValue("-Z", 1)
        menuItem.AddNamedValue("+Z", 2)
    End Sub
    Public Function GetStringForSupportingMembersValidity(item As Integer) As String
        Select Case item
            Case 0
                Return "All"
            Case 1
                Return "-Z"
            Case 2
                Return "+Z"

            Case Else
                Return "All"
        End Select
    End Function

    Public Sub AddOptionsToMenuTransferDirection(menuItem As Param_Integer)
        menuItem.AddNamedValue("X (LCS panel)", 0)
        menuItem.AddNamedValue("Y (LCS panel)", 1)
        menuItem.AddNamedValue("all (LCS panel)", 2)
    End Sub
    Public Function GetStringForTransferDirection(item As Integer) As String
        Select Case item
            Case 0
                Return "X (LCS panel)"
            Case 1
                Return "Y (LCS panel)"
            Case 2
                Return "all (LCS panel)"

            Case Else
                Return "all (LCS panel)"
        End Select
    End Function

    Public Sub AddOptionsToMenuNLFunctionType(menuItem As Param_Integer)
        menuItem.AddNamedValue("Translation", 0)
        menuItem.AddNamedValue("Rotation", 1)
        menuItem.AddNamedValue("Nonlinear subsoil", 2)
    End Sub
    Public Function GetStringForitemNLFunctionType(item As Integer) As String
        Select Case item
            Case 0
                Return "Translation"
            Case 1
                Return "Rotation"
            Case 2
                Return "Nonlinear subsoil"
            Case Else
                Return "Translation"
        End Select
    End Function


    Public Sub AddOptionsToMenuNLFunctionEndType(menuItem As Param_Integer)
        menuItem.AddNamedValue("Rigid", 0)
        menuItem.AddNamedValue("Free", 1)
        menuItem.AddNamedValue("Flexible", 2)
    End Sub

    Public Function GetStringForitemNLFunctionEndType(item As Integer) As String
        Select Case item
            Case 0
                Return "Rigid"
            Case 1
                Return "Free"
            Case 2
                Return "Flexible"
            Case Else
                Return "Rigid"
        End Select
    End Function

    Public Sub AddOptionstoMenuC1ztype(menuItem As Param_Integer)
        menuItem.AddNamedValue("Flexible", 0)
        menuItem.AddNamedValue("Nonlinear function", 1)
    End Sub

    Public Function GetStringForC1ztype(item As Integer) As String
        Select Case item
            Case 0
                Return "Flexible"
            Case 1
                Return "Nonlinear function"

            Case Else
                Return "Flexible"
        End Select
    End Function


    Public Sub AddOptionsToMenuBeamNLCableInitialMesh(menuitem As Param_Integer)
        menuitem.AddNamedValue("Straight", 0)
        menuitem.AddNamedValue("Calculated", 1)

    End Sub

    Public Function GetStringFromBeamNLCableInitialMesh(item As Integer) As String
        Select Case item
            Case 0
                Return "Straight"
            Case 1
                Return "Calculated"
            Case Else
                Return "Straight"
        End Select
    End Function
#End Region

    Private Function UnflattenObjectData(dataIn As List(Of String), valuesPerObject As Integer, name As String) As String(,)
        UnflattenObjectData = Nothing

        If (dataIn IsNot Nothing) Then

            Dim objectCount As Integer = dataIn.Count / valuesPerObject

            If objectCount > 0 Then
                Dim dataOut(objectCount - 1, valuesPerObject - 1) As String

                Rhino.RhinoApp.WriteLine("Number of " & name & " objects: " & objectCount)
                For i = 0 To objectCount - 1
                    For j = 0 To valuesPerObject - 1
                        dataOut(i, j) = dataIn(j + i * valuesPerObject)
                    Next j
                Next i

                UnflattenObjectData = dataOut
            End If
        End If

    End Function

    Private Sub OpenContainerAndTable(ByRef oSB As Text.StringBuilder, type As Koala.EsaObjectType)
        OpenContainer(oSB, type)
        oSB.AppendLine("<table id=""" & TableIds(type) & """ t=""" & TableTypes(type) & """>")
    End Sub
    Private Sub OpenContainer(ByRef oSB As Text.StringBuilder, type As Koala.EsaObjectType)
        oSB.AppendLine("")
        oSB.AppendLine("<container id=""" & ContainerIds(type) & """ t=""" & ContainerTypes(type) & """>")
    End Sub


    Private Sub CloseContainerAndTable(ByRef oSB As Text.StringBuilder)
        oSB.AppendLine("</table>")
        oSB.AppendLine("</container>")
    End Sub

    Private Function ConCat_ht(h, t)
        ConCat_ht = "<h" & h & " t=""" & t & """/>"
    End Function
    Private Function ConCat_hh(h, N)
        ConCat_hh = "<h" & h & " h=""" & N & """/>"
    End Function
    Private Function ConCat_pv(p, v)
        ConCat_pv = "<p" & p & " v=""" & v & """/>"
    End Function
    Private Function ConCat_pvt(p, v, t)
        ConCat_pvt = "<p" & p & " v=""" & v & """ t=""" & t & """/>"
    End Function
    Private Function ConCat_pvx(p, v, x)
        ConCat_pvx = "<p" & p & " v=""" & v & """ x=""" & x & """/>"
    End Function
    Private Function ConCat_pnx(p, n, x)
        ConCat_pnx = "<p" & p & " n=""" & n & """ x=""" & x & """/>"
    End Function

    Private Function ConCat_pv1v2v3(p, v1, v2, v3)
        ConCat_pv1v2v3 = "<p" & p & " v1=""" & v1 & """ v2=""" & v2 & """ v3=""" & v3 & """/>"
    End Function
    Private Function ConCat_pv1v2v3x(p, v1, v2, v3, x)
        ConCat_pv1v2v3x = "<p" & p & " v1=""" & v1 & """ v2=""" & v2 & """ v3=""" & v3 & """ x=""" & x & """/>"
    End Function
    Private Function ConCat_pv1v2x(p, v1, v2, x)
        ConCat_pv1v2x = "<p" & p & " v1=""" & v1 & """ v2=""" & v2 & """ x=""" & x & """/>"
    End Function

    Private Function ConCat_pn(p, N)
        ConCat_pn = "<p" & p & " n=""" & N & """/>"
    End Function
    Private Function ConCat_pin(p, i, N)
        ConCat_pin = "<p" & p & " i=""" & i & """ n=""" & N & """/>"
    End Function
    Private Function ConCat_opentable(p, t)
        ConCat_opentable = "<p" & p & " t=""" & t & """>"
    End Function
    Private Function ConCat_closetable(p)
        ConCat_closetable = "</p" & p & ">"
    End Function
    Private Function ConCat_row(id)
        ConCat_row = "<row id=""" & CStr(id) & """>"
    End Function

    Private Function ConCat_pvt_enum(Of T)(p As Integer, value As String) As String
        Dim enumValue = Koala.GetEnum(Of T)(value)
        ConCat_pvt_enum = ConCat_pvt(p.ToString, Convert.ToInt32(enumValue), Koala.GetEnumDescription(enumValue))
    End Function

    '<Custom additional code> 

    Public Sub CreateXMLFile(FileName As String, StructureType As String, MaterialTypes As List(Of String), UILanguage As String, scale As Double, meshSize As Double, RemDuplNodes As Boolean, Tolerance As Double,
                             projectInfo As List(Of String), in_selections As List(Of String), in_layers As List(Of String), in_materials As List(Of String), in_sections As List(Of String),
                             in_nodes As List(Of String), in_beams As List(Of String), in_surfaces As List(Of String),
                             in_openings As List(Of String), in_nodesupports As List(Of String), in_edgesupports As List(Of String), in_lcases As List(Of String), in_lgroups As List(Of String),
                             in_mgroups As List(Of String), in_mcombis As List(Of String), in_spectra As List(Of String),
                             in_lloads As List(Of String), in_sloads As List(Of String),
                             in_fploads As List(Of String), in_flloads As List(Of String), in_fsloads As List(Of String), in_hinges As List(Of String), in_edgeLoads As List(Of String), in_pointLoadsPoints As List(Of String), in_pointLoadsBeams As List(Of String),
                             in_LinCombinations As List(Of String), in_NonLinCombinations As List(Of String), in_StabCombinations As List(Of String), in_ResultClass As List(Of String),
                             in_CrossLinks As List(Of String), in_presstensionElem As List(Of String), in_gapElem As List(Of String), in_limitforceElem As List(Of String),
                             in_BeamLineSupport As List(Of String), in_PointSupportOnBeam As List(Of String), in_Subsoils As List(Of String), in_SurfaceSupports As List(Of String), in_loadpanels As List(Of String), in_pointMomentPoint As List(Of String),
                             in_pointMomentBeam As List(Of String), in_lineMomentBeam As List(Of String), in_lineMomentEdge As List(Of String), in_freePointMoment As List(Of String), in_nonlinearfunctions As List(Of String),
                             in_slabinternalEdges As List(Of String), in_RigidArms As List(Of String), in_Cables As List(Of String), in_BeamInternalNodes As List(Of String), in_LineHiges As List(Of String),
                             in_ThermalLoadBeams As List(Of String), in_ThermalLoadSurfaces As List(Of String), in_ArbitraryProfiles As List(Of String),
                             in_IntegationStrips As List(Of String), in_SectionOn1D As List(Of String), in_SectionOn2D As List(Of String),
                             in_AveragingStrips As List(Of String))

        Dim i As Long

        ''If LCSType = 0 > Standard definition of LCS with an angle > LCSParam1 is the angle in radian
        ''If LCSType = 2 > Definition of LCS through a vector for local Z > LCSParam1/2/3 are the X, Y, Z components of the vector

        Dim stopWatch As New System.Diagnostics.Stopwatch()
        Dim time_elapsed As Double
        Dim objstream As String

        'initialize stopwatch
        stopWatch.Start()

        'show that it's busy
        Rhino.RhinoApp.WriteLine("")
        Rhino.RhinoApp.WriteLine("===== KOALA SCIA Engineer plugin - model creation =====")

        'Define the StringBuilder capacity
        Dim oSB As New Text.StringBuilder(100) 'required for fast string building

        Rhino.RhinoApp.WriteLine("Generating model data...")

        'get model data (de-serialize)
        '=============================
        ' Keep track of all unique nodes in the model
        Dim nodeMap = New Dictionary(Of String, Node)()
        Dim allNodes As New List(Of Node)

        Dim SE_Nodes = UnflattenObjectData(in_nodes, 4, "node")
        Dim SE_IntNodes = UnflattenObjectData(in_BeamInternalNodes, 5, "beam internal node")

        If (SE_Nodes IsNot Nothing) Then
            For i = 0 To SE_Nodes.GetLength(0) - 1
                AddToNodeMap(nodeMap, allNodes, SE_Nodes(i, 0), SE_Nodes(i, 1), SE_Nodes(i, 2), SE_Nodes(i, 3), Nothing, Tolerance, RemDuplNodes)
            Next i
        End If

        If (SE_IntNodes IsNot Nothing) Then
            For i = 0 To SE_IntNodes.GetLength(0) - 1
                AddToNodeMap(nodeMap, allNodes, SE_IntNodes(i, 0), SE_IntNodes(i, 1), SE_IntNodes(i, 2), SE_IntNodes(i, 3), SE_IntNodes(i, 4), Tolerance, RemDuplNodes)
            Next i
        End If

        Dim materials = New List(Of Material)
        If (in_materials IsNot Nothing) Then
            For Each materialDef In in_materials
                materials.Add(New Material(materialDef))
            Next
        End If

        Dim model As New ModelData With {
            .Scale = scale,
            .MeshSize = meshSize,
            .UILanguage = UILanguage,
            .StructureType = StructureType,
            .Sections = UnflattenObjectData(in_sections, 4, "section"),
            .MaterialTypes = MaterialTypes,
            .ProjectInfo = projectInfo,
            .Selections = UnflattenObjectData(in_selections, 2, "selections"),
            .Nodes = allNodes,
            .NodeMap = nodeMap,
            .Beams = UnflattenObjectData(in_beams, 14, "beam"),
            .Surfaces = UnflattenObjectData(in_surfaces, 12, "surface"),
            .Layers = UnflattenObjectData(in_layers, 3, "layer"),
            .Materials = materials,
            .LoadPanels = UnflattenObjectData(in_loadpanels, 8, "load panel"),
            .Openings = UnflattenObjectData(in_openings, 3, "opening"),
            .SlabInternalEdges = UnflattenObjectData(in_slabinternalEdges, 3, "slab internal edge"),
            .RigidArms = UnflattenObjectData(in_RigidArms, 5, "rigid arm"),
            .NodeSupports = UnflattenObjectData(in_nodesupports, 22, "node support"),
            .BeamPointSupports = UnflattenObjectData(in_PointSupportOnBeam, 26, "beam point support"),
            .BeamLineSupports = UnflattenObjectData(in_BeamLineSupport, 25, "beam line support"),
            .SurfaceSupports = UnflattenObjectData(in_SurfaceSupports, 3, "surface support"),
            .SurfaceEdgeSupports = UnflattenObjectData(in_edgesupports, 27, "surface edge support"),
            .LoadCases = UnflattenObjectData(in_lcases, 3, "load case"),
            .LoadGroups = UnflattenObjectData(in_lgroups, 3, "load group"),
            .MassGroups = UnflattenObjectData(in_mgroups, 3, "mass group"),
            .MassCombinations = UnflattenObjectData(in_mcombis, 2, "mass combination"),
            .SeismicSpectra = UnflattenObjectData(in_spectra, 4, "seismic spectra"),
            .NodePointLoads = UnflattenObjectData(in_pointLoadsPoints, 6, "point load"),
            .NodePointMoments = UnflattenObjectData(in_pointMomentPoint, 5, "point moment"),
            .BeamPointLoads = UnflattenObjectData(in_pointLoadsBeams, 12, "beam point load"),
            .BeamPointMoments = UnflattenObjectData(in_pointMomentBeam, 10, "beam point moment"),
            .BeamLineLoads = UnflattenObjectData(in_lloads, 13, "beam line load"),
            .BeamLineMoments = UnflattenObjectData(in_lineMomentBeam, 11, "beam line moment"),
            .EdgeLoads = UnflattenObjectData(in_edgeLoads, 15, "edge load"),
            .EdgeMoments = UnflattenObjectData(in_lineMomentEdge, 13, "edge moment"),
            .SurfaceLoads = UnflattenObjectData(in_sloads, 5, "surface load"),
            .FreePointLoads = UnflattenObjectData(in_fploads, 11, "free point load"),
            .FreePointMoments = UnflattenObjectData(in_freePointMoment, 11, "free point moment"),
            .FreeLineLoads = UnflattenObjectData(in_flloads, 11, "free line load"),
            .FreeSurfaceLoads = UnflattenObjectData(in_fsloads, 12, "free surface load"),
            .BeamThermalLoads = UnflattenObjectData(in_ThermalLoadBeams, 12, "beam thermal load"),
            .SurfaceThermalLoads = UnflattenObjectData(in_ThermalLoadSurfaces, 6, "thermal surface load"),
            .Hinges = UnflattenObjectData(in_hinges, 21, "hinge"),
            .CrossLinks = UnflattenObjectData(in_CrossLinks, 3, "cross link"),
            .LineHinges = UnflattenObjectData(in_LineHiges, 14, "line hinge"),
            .LinearCombinations = UnflattenObjectData(in_LinCombinations, 3, "linear combination"),
            .NonLinearCombinations = UnflattenObjectData(in_NonLinCombinations, 4, "non-linear combination"),
            .StabilityCombinations = UnflattenObjectData(in_StabCombinations, 2, "stability combination"),
            .ResultClasses = UnflattenObjectData(in_ResultClass, 4, "result class"),
            .GapElements = UnflattenObjectData(in_gapElem, 4, "gap element"),
            .PretensionElements = UnflattenObjectData(in_presstensionElem, 2, "pretension element"),
            .LimitForceElements = UnflattenObjectData(in_limitforceElem, 4, "limit force element"),
            .Cables = UnflattenObjectData(in_Cables, 6, "cable"),
            .Subsoils = UnflattenObjectData(in_Subsoils, 9, "subsoil"),
            .NonLinearFunctions = UnflattenObjectData(in_nonlinearfunctions, 5, "non-linear function"),
            .ArbitraryProfiles = UnflattenObjectData(in_ArbitraryProfiles, 9, "arbitrary profile"),
            .IntegrationStrips = UnflattenObjectData(in_IntegationStrips, 17, "integration strip"),
            .AveragingStrips = UnflattenObjectData(in_AveragingStrips, 14, "averaging strip"),
            .SectionOn1D = UnflattenObjectData(in_SectionOn1D, 8, "section on 1d"),
            .SectionOn2D = UnflattenObjectData(in_SectionOn2D, 6, "section on 2d")
        }

        'write the XML file
        '---------------------------------------------------
        Rhino.RhinoApp.WriteLine("Creating the XML file string in memory...")

        Dim fileNameXMLdef As String
        fileNameXMLdef = Path.GetFileName(FileName) + ".def"

        Call WriteXMLFile(oSB, model, fileNameXMLdef)

        Rhino.RhinoApp.Write("Writing to file: " & FileName & "...")

        objstream = oSB.ToString()
        System.IO.File.WriteAllText(FileName, objstream)

        Dim XmlDefOutputPath As String
        XmlDefOutputPath = FileName + ".def"
        Dim xmlDef As String
        xmlDef = My.Resources.koala_xml
        System.IO.File.WriteAllText(XmlDefOutputPath, xmlDef)

        Rhino.RhinoApp.Write(" Done." & Convert.ToChar(13))

        'stop stopwatch
        stopWatch.Stop()
        time_elapsed = stopWatch.ElapsedMilliseconds
        Rhino.RhinoApp.WriteLine("Done in " + CStr(time_elapsed) + " ms.")
    End Sub


    Private Sub WriteXMLFile(ByRef oSB As Text.StringBuilder, modelData As ModelData, fileNameXMLdef As String)

        'global variables for this component
        '-----------------------------------
        ' required until free loads geometry's definition is language-neutral in SCIA Engineer's XML
        Dim UILanguageNumber As Long = GetNumberForUILangauge(modelData.UILanguage)

        Dim i As Long
        'write XML header information -----------------------------------------------------

        oSB.AppendLine("<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>")
        oSB.AppendLine("<project xmlns=""http://www.scia.cz"">")
        oSB.AppendLine("<def uri=""" & fileNameXMLdef & """/>")

        If Not String.IsNullOrEmpty(modelData.StructureType) Or modelData.MaterialTypes.Count <> 0 Or modelData.ProjectInfo.Count >= 5 Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.ProjectData)

            'header
            oSB.AppendLine("<h>")
            oSB.AppendLine(ConCat_ht("0", "Structure"))
            oSB.AppendLine(ConCat_ht("1", "Project"))
            oSB.AppendLine(ConCat_ht("2", "Part"))
            oSB.AppendLine(ConCat_ht("3", "Description"))
            oSB.AppendLine(ConCat_ht("4", "Author"))
            oSB.AppendLine(ConCat_ht("5", "Date"))
            oSB.AppendLine(ConCat_ht("6", "Concrete"))
            oSB.AppendLine(ConCat_ht("7", "Steel"))
            oSB.AppendLine(ConCat_ht("8", "Timber"))
            oSB.AppendLine(ConCat_ht("9", "Steel fibre concrete"))
            oSB.AppendLine(ConCat_ht("10", "Other material"))
            oSB.AppendLine(ConCat_ht("11", "Aluminium"))
            oSB.AppendLine(ConCat_ht("12", "Masonry"))
            oSB.AppendLine(ConCat_ht("13", "Functionality"))
            oSB.AppendLine("</h>")

            'data
            oSB.AppendLine("<obj id=""1"">")
            Select Case modelData.StructureType
                Case "Beam"
                    oSB.AppendLine(ConCat_pvt("0", 0, "Beam"))
                Case "Truss XZ"
                    oSB.AppendLine(ConCat_pvt("0", 1, "Truss XZ"))
                Case "Frame XZ"
                    oSB.AppendLine(ConCat_pvt("0", 2, "Frame XZ"))
                Case "Truss XYZ"
                    oSB.AppendLine(ConCat_pvt("0", 3, "Truss XYZ"))
                Case "Frame XYZ"
                    oSB.AppendLine(ConCat_pvt("0", 4, "Frame XYZ"))
                Case "Grid XY"
                    oSB.AppendLine(ConCat_pvt("0", 5, "Grid XY"))
                Case "Plate XY"
                    oSB.AppendLine(ConCat_pvt("0", 6, "Plate XY"))
                Case "Wall XY"
                    oSB.AppendLine(ConCat_pvt("0", 7, "Wall XY"))
                Case "General XYZ"
                    oSB.AppendLine(ConCat_pvt("0", 8, "General XYZ"))
            End Select

            If modelData.ProjectInfo.Count > 0 Then
                For i = 0 To Math.Min(modelData.ProjectInfo.Count - 1, 4)
                    If modelData.ProjectInfo(i) IsNot Nothing Then
                        oSB.AppendLine(ConCat_pv((i + 1).ToString, modelData.ProjectInfo(i)))
                    End If
                Next
            End If

            If modelData.MaterialTypes.Count <> 0 Then
                Dim materialEnums As New List(Of Koala.ProjectMaterialType)
                For Each material In modelData.MaterialTypes
                    If Not String.IsNullOrEmpty(material) Then
                        Dim matEnum = Koala.GetEnum(Of Koala.ProjectMaterialType)(material)
                        materialEnums.Add(matEnum)
                    End If
                Next
                If materialEnums.Count > 0 Then
                    oSB.AppendLine(ConCat_pv("6", IIf(materialEnums.Contains(Koala.ProjectMaterialType.Concrete), "1", "0")))
                    oSB.AppendLine(ConCat_pv("7", IIf(materialEnums.Contains(Koala.ProjectMaterialType.Steel), "1", "0")))
                    oSB.AppendLine(ConCat_pv("8", IIf(materialEnums.Contains(Koala.ProjectMaterialType.Timber), "1", "0")))
                    oSB.AppendLine(ConCat_pv("9", IIf(materialEnums.Contains(Koala.ProjectMaterialType.SteelFibreConcrete), "1", "0")))
                    oSB.AppendLine(ConCat_pv("10", IIf(materialEnums.Contains(Koala.ProjectMaterialType.Other), "1", "0")))
                    oSB.AppendLine(ConCat_pv("11", IIf(materialEnums.Contains(Koala.ProjectMaterialType.Aluminium), "1", "0")))
                    oSB.AppendLine(ConCat_pv("12", IIf(materialEnums.Contains(Koala.ProjectMaterialType.Masonry), "1", "0")))
                End If
                'Else
                '    oSB.AppendLine(ConCat_pv("6", "1"))
                '    oSB.AppendLine(ConCat_pv("7", "1"))
                '    oSB.AppendLine(ConCat_pv("8", "0"))
                '    oSB.AppendLine(ConCat_pv("9", "0"))
            End If

            'Don't define this in Koala, but in the esa template that you wish to update
            'oSB.AppendLine(ConCat_pv("13", "PrDEx_InitialStress, PrDEx_Subsoil,PrDEx_InitialDeformationsAndCurvature, PrDEx_SecondOrder, PrDEx_Nonlinearity, PrDEx_BeamLocalNonlinearity,PrDEx_SupportNonlinearity, PrDEx_StabilityAnalysis"))
            'If (projectInfo.Count <> 0) Then
            '    oSB.AppendLine(ConCat_pv("14", projectInfo(5)))
            '    oSB.AppendLine(ConCat_pv("15", projectInfo(6)))
            'Else
            '    oSB.AppendLine(ConCat_pv("14", "EC - EN"))
            '    oSB.AppendLine(ConCat_pv("15", "Standard EN"))
            'End If
            oSB.AppendLine("</obj>")

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.Layers IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.Layer)
            Call WriteLayerHeaders(oSB)

            For i = 0 To modelData.Layers.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... layer: " + Str(i))
                End If
                Call WriteLayer(oSB, i, modelData.Layers)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.MeshSize > 0 Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.MeshSetup)

            'header
            oSB.AppendLine("<h>")
            oSB.AppendLine(ConCat_ht("0", "Name"))
            oSB.AppendLine(ConCat_ht("1", "Average size of 2d element/curved element"))
            oSB.AppendLine(ConCat_ht("2", "Average number of tiles of 1d element"))
            oSB.AppendLine(ConCat_ht("3", "Division on haunches and arbitrary members"))
            oSB.AppendLine(ConCat_ht("4", "Minimal length of beam element"))
            oSB.AppendLine(ConCat_ht("5", "Maximal length of beam element"))
            oSB.AppendLine("</h>")

            'data
            oSB.AppendLine("<obj id=""1"">")
            oSB.AppendLine(ConCat_pv("1", modelData.MeshSize))
            oSB.AppendLine("</obj>")

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.Materials IsNot Nothing Then
            Dim concreteMaterials = modelData.Materials.Where(Function(m) m.Type = MaterialType.Concrete_EC_EN1)
            Dim steelMaterials = modelData.Materials.Where(Function(m) m.Type = MaterialType.Steel_EC1)

            If concreteMaterials.Any Then
                OpenContainer(oSB, Koala.EsaObjectType.Material)
                oSB.AppendLine("<table id=""D0AFF031-9D60-4737-8D7C-24140478CBE7"" t=""EP_MaterialEC.EP_MaterialCrtEC_EN.1"">")

                'headers
                oSB.AppendLine("<h>")
                oSB.AppendLine(ConCat_ht("0", "Name"))
                oSB.AppendLine(ConCat_ht("2", "Material type"))
                oSB.AppendLine(ConCat_ht("3", "Thermal expansion"))
                oSB.AppendLine(ConCat_ht("4", "Unit mass"))
                oSB.AppendLine(ConCat_ht("5", "Density in fresh state"))
                oSB.AppendLine(ConCat_ht("6", "E modulus"))
                oSB.AppendLine(ConCat_ht("7", "Poisson coeff."))
                oSB.AppendLine(ConCat_ht("8", "Independent G modulus"))
                oSB.AppendLine(ConCat_ht("9", "G modulus"))
                oSB.AppendLine(ConCat_ht("10", "Log. decrement (non-uniform damping only)"))
                oSB.AppendLine(ConCat_ht("11", "Colour"))
                oSB.AppendLine(ConCat_ht("12", "Specific heat"))
                oSB.AppendLine(ConCat_ht("13", "Thermal conductivity"))
                oSB.AppendLine(ConCat_ht("14", "Order in code"))
                oSB.AppendLine(ConCat_ht("15", "Stone diameter (dg)"))
                oSB.AppendLine(ConCat_ht("16", "Cement class"))
                oSB.AppendLine(ConCat_ht("17", "Price per unit"))
                oSB.AppendLine(ConCat_ht("18", "Characteristic compressive cylinder strength fck(28)"))
                oSB.AppendLine(ConCat_ht("19", "Calculated depended values"))
                oSB.AppendLine(ConCat_ht("20", "Mean compressive strength  fcm(28)"))
                oSB.AppendLine(ConCat_ht("21", "fcm(28) - fck(28)"))
                oSB.AppendLine(ConCat_ht("22", "Mean tensile strength fctm(28)"))
                oSB.AppendLine(ConCat_ht("23", "fctk 0,05(28)"))
                oSB.AppendLine(ConCat_ht("24", "fctk 0,95(28)"))
                oSB.AppendLine(ConCat_ht("25", "Design compressive strength - persistent (fcd = fck / gamma c_p)"))
                oSB.AppendLine(ConCat_ht("26", "Design compressive strength - accidental (fcd = fck / gamma c_a)"))
                oSB.AppendLine(ConCat_ht("27", "Strain at reaching maximum strength eps c2"))
                oSB.AppendLine(ConCat_ht("28", "Ultimate strain eps cu2"))
                oSB.AppendLine(ConCat_ht("29", "Strain at reaching maximum strength eps c3"))
                oSB.AppendLine(ConCat_ht("30", "Ultimate strain eps cu3"))
                oSB.AppendLine(ConCat_ht("31", "n"))
                oSB.AppendLine(ConCat_ht("32", "Type of aggregate"))
                oSB.AppendLine(ConCat_ht("33", "Measured values of mean compressive strength (influence of ageing)"))
                oSB.AppendLine(ConCat_ht("34", "Type of diagram"))
                oSB.AppendLine("</h>")

                'data
                For Each concrete In concreteMaterials
                    Call WriteConcrete(oSB, concrete)
                Next

                Call CloseContainerAndTable(oSB)
            End If

            If steelMaterials.Any Then
                OpenContainer(oSB, Koala.EsaObjectType.Material)
                oSB.AppendLine("<table id=""62418600-E01A-4580-ADF7-FD8CCAC8D2A8"" t=""EP_MaterialEC.EP_MaterialSteelEC.1"">")

                'headers
                oSB.AppendLine("<h>")
                oSB.AppendLine(ConCat_ht("0", "Name"))
                oSB.AppendLine(ConCat_ht("2", "Material type"))
                oSB.AppendLine(ConCat_ht("3", "Thermal expansion"))
                oSB.AppendLine(ConCat_ht("4", "Unit mass"))
                oSB.AppendLine(ConCat_ht("5", "E modulus"))
                oSB.AppendLine(ConCat_ht("6", "Poisson coeff."))
                oSB.AppendLine(ConCat_ht("7", "Independent G modulus"))
                oSB.AppendLine(ConCat_ht("8", "G modulus"))
                oSB.AppendLine(ConCat_ht("9", "Log. decrement (non-uniform damping only)"))
                oSB.AppendLine(ConCat_ht("10", "Colour"))
                oSB.AppendLine(ConCat_ht("11", "Thermal expansion (for fire resistance)"))
                oSB.AppendLine(ConCat_ht("12", "Specific heat"))
                oSB.AppendLine(ConCat_ht("13", "Thermal conductivity"))
                oSB.AppendLine(ConCat_ht("14", "Ultimate strength"))
                oSB.AppendLine(ConCat_ht("15", "Yield strength"))
                oSB.AppendLine(ConCat_ht("16", "Price per unit"))
                oSB.AppendLine(ConCat_ht("17", "Lower limit"))
                oSB.AppendLine(ConCat_ht("18", "Upper limit"))
                oSB.AppendLine(ConCat_ht("19", "Fu (range)"))
                oSB.AppendLine(ConCat_ht("20", "Fy (range)"))
                oSB.AppendLine("</h>")

                'data
                For Each steel In steelMaterials
                    Call WriteSteel(oSB, steel)
                Next

                Call CloseContainerAndTable(oSB)
            End If
        End If

        If modelData.Sections IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.CrossSection)
            Call WriteSectionHeaders(oSB)

            For i = 0 To modelData.Sections.GetLength(0) - 1
                Call WriteSection(oSB, modelData.Sections(i, 0), modelData.Sections(i, 1), modelData.Sections(i, 2), modelData.Sections(i, 3))
            Next i

            Call CloseContainerAndTable(oSB)
        End If

        ' Non-linear functions need to be defined at the start in order to be referenced by other members
        If modelData.NonLinearFunctions IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.NonLinearFunction)
            Call WriteNonlinearFunctionHeaders(oSB)

            For i = 0 To modelData.NonLinearFunctions.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... NL function: " + Str(i))
                End If
                Call WriteNonlinearFunction(oSB, i, modelData.NonLinearFunctions)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.Nodes IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.Node)
            Call WriteNodeHeaders(oSB)

            Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... nodes")
            For Each node In modelData.Nodes
                Call WriteNode(oSB, modelData, node)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.Beams IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.Member1D)
            Call WriteBeamHeaders(oSB)

            For i = 0 To modelData.Beams.GetLength(0) - 1
                If i > 0 And (i Mod 500 = 0) Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... beam: " + Str(i))
                End If
                Call WriteBeam(oSB, i, modelData)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.Surfaces IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.Member2D)
            Call WriteSurfaceHeaders(oSB)

            For i = 0 To modelData.Surfaces.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... surface: " + Str(i))
                End If
                Call WriteSurface(oSB, i, modelData)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.LoadPanels IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.LoadPanel)
            Call WriteLoadPanelHeaders(oSB)

            For i = 0 To modelData.LoadPanels.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... surface: " + Str(i))
                End If
                Call WriteLoadPanels(oSB, i, modelData)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.Openings IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.Opening)
            Call WriteOpeningHeaders(oSB)

            For i = 0 To modelData.Openings.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... opening: " + Str(i))
                End If
                Call WriteOpening(oSB, i, modelData)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.ArbitraryProfiles IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.ArbitraryProfile)
            Call WriteArbitraryProfileHeaders(oSB)

            For i = 0 To modelData.ArbitraryProfiles.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... arbitrary profile: " + Str(i))
                End If
                Call WriteArbitraryProfile(oSB, i, modelData.ArbitraryProfiles)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.SlabInternalEdges IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.InternalEdge2D)
            Call WriteInternalEdgeHeaders(oSB)

            For i = 0 To modelData.SlabInternalEdges.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... SlabInternalEdge: " + Str(i))
                End If
                Call WriteInternalEdge(oSB, i, modelData)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.NodeSupports IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.NodeSupport)
            Call WriteNodeSupportHeaders(oSB)

            For i = 0 To modelData.NodeSupports.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... nodal supports: " + Str(i))
                End If
                Call WriteNodeSupport(oSB, i, modelData)

            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.BeamLineSupports IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.BeamLineSupport)
            Call WriteBeamLineSupportHeaders(oSB)

            For i = 0 To modelData.BeamLineSupports.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... beam line supports: " + Str(i))
                End If
                Call WriteBeamLineSupport(oSB, i, modelData.BeamLineSupports)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.BeamPointSupports IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.BeamPointSupport)
            Call WriteBeamPointSupportHeaders(oSB)

            For i = 0 To modelData.BeamPointSupports.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... edge supports: " + Str(i))
                End If
                Call WriteBeamPointSupport(oSB, i, modelData.BeamPointSupports)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.SurfaceEdgeSupports IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.SurfaceEdgeSupport)
            Call WriteEdgeSupportHeaders(oSB)

            For i = 0 To modelData.SurfaceEdgeSupports.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... edge supports: " + Str(i))
                End If
                Call WriteEdgeSupport(oSB, i, modelData)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        'Define subsoil before surface supports!
        If modelData.Subsoils IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.Subsoil)
            Call WriteSubsoilHeaders(oSB)

            For i = 0 To modelData.Subsoils.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... subsoil: " + Str(i))
                End If
                Call WriteSubsoil(oSB, i, modelData.Subsoils)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.SurfaceSupports IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.SurfaceSupport)
            Call WriteSurfaceSupportHeaders(oSB)

            For i = 0 To modelData.SurfaceSupports.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... surface support: " + Str(i))
                End If
                Call WriteSurfaceSupport(oSB, i, modelData.SurfaceSupports)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.RigidArms IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.RigidArm)
            Call WriteRigidArmHeaders(oSB)

            For i = 0 To modelData.RigidArms.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... rigid arm: " + Str(i))
                End If
                Call WriteRigidArm(oSB, i, modelData.RigidArms)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.Hinges IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.Hinge)
            Call WriteHingeHeaders(oSB)

            For i = 0 To modelData.Hinges.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... hinge: " + Str(i))
                End If
                Call WriteHinge(oSB, i, modelData.Hinges)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.LineHinges IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.LineHinge)
            Call WriteLineHingeHeaders(oSB)

            For i = 0 To modelData.LineHinges.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... LineHinge: " + Str(i))
                End If
                Call WriteLineHinge(oSB, i, modelData.LineHinges)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.CrossLinks IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.CrossLink)
            Call WriteCrossLinkHeaders(oSB)

            For i = 0 To modelData.CrossLinks.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... crosslink: " + Str(i))
                End If
                Call WriteCrossLink(oSB, i, modelData.CrossLinks)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        'Define load cases before mass groups!
        If modelData.LoadCases IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.LoadCase)
            Call WriteLoadCaseHeaders(oSB)

            For i = 0 To modelData.LoadCases.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... load case: " + Str(i))
                End If
                Call WriteLoadCase(oSB, i, modelData.LoadCases)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.LoadGroups IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.LoadGroup)
            Call WriteLoadGroupHeaders(oSB)

            For i = 0 To modelData.LoadGroups.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... load group: " + Str(i))
                End If
                Call WriteLoadGroup(oSB, i, modelData.LoadGroups)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.LinearCombinations IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.LinearCombination)
            Call WriteLinCombinationHeaders(oSB)

            For i = 0 To modelData.LinearCombinations.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... combination: " + Str(i))
                End If
                Call WriteLinCombination(oSB, i, modelData.LinearCombinations)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.NonLinearCombinations IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.NonLinearCombination)
            Call WriteNonLinCombinationHeaders(oSB)

            For i = 0 To modelData.NonLinearCombinations.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... NL combination: " + Str(i))
                End If
                Call WriteNonLinCombination(oSB, i, modelData.NonLinearCombinations)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.StabilityCombinations IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.StabilityCombination)
            Call WriteStabilityCombinationHeaders(oSB)

            For i = 0 To modelData.StabilityCombinations.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... stability combination: " + Str(i))
                End If

                Call WriteStabilityCombination(oSB, i, modelData.StabilityCombinations)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        'Define Result classes after load cases and combinations!
        If modelData.ResultClasses IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.ResultClass)
            Call WriteResultClassHeaders(oSB)

            For i = 0 To modelData.ResultClasses.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... stability combination: " + Str(i))
                End If

                Call WriteResultclass(oSB, i, modelData.ResultClasses)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        'Define Mass Groups after load cases!
        If modelData.MassGroups IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.MassGroup)
            Call WriteMassGroupHeader(oSB)

            For i = 0 To modelData.MassGroups.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XMl file string in memory... mass group: " + Str(i))
                End If

                Call WriteMassGroup(oSB, i, modelData.MassGroups)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        'Define Mass combinations after mass groups!
        If modelData.MassCombinations IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.MassCombination)
            Call WriteMassCombinationHeader(oSB)

            For i = 0 To modelData.MassCombinations.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... mass combination: " + Str(i))
                End If

                Call WriteMassCombinations(oSB, i, modelData.MassCombinations)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.SeismicSpectra IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.SeismicSpectrum)
            Call WriteSeismicSpectrumHeader(oSB)

            For i = 0 To modelData.SeismicSpectra.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... seismic spectrum: " + Str(i))
                End If

                Call WriteSeismicSpectrum(oSB, i, modelData.SeismicSpectra)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.NodePointLoads IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.PointLoadNode)
            Call WritePointLoadsPointHeaders(oSB)

            For i = 0 To modelData.NodePointLoads.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... point load node: " + Str(i))
                End If
                Call WritePointLoadsPoint(oSB, i, modelData)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.NodePointMoments IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.PointMomentNode)
            Call WriteMomentLoadsPointHeaders(oSB)

            For i = 0 To modelData.NodePointMoments.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... moment in node: " + Str(i))
                End If
                Call WriteMomentLoadsPoint(oSB, i, modelData)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.BeamPointLoads IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.PointLoadBeam)
            Call WritePointLoadsBeamHeaders(oSB)

            For i = 0 To modelData.BeamPointLoads.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... beam point load: " + Str(i))
                End If
                Call WritePointLoadsBeam(oSB, i, modelData.BeamPointLoads)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.BeamPointMoments IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.PointMomentBeam)
            Call WriteMomentLoadsBeamHeaders(oSB)

            For i = 0 To modelData.BeamPointMoments.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... point moment on beam: " + Str(i))
                End If
                Call WriteMomentLoadsBeam(oSB, i, modelData.BeamPointMoments)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.BeamLineLoads IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.LineLoadBeam)
            Call WriteLineLoadBeamHeaders(oSB)

            For i = 0 To modelData.BeamLineLoads.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... beam line load: " + Str(i))
                End If
                Call WriteLineLoadBeam(oSB, i, modelData.BeamLineLoads)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.BeamLineMoments IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.LineMomentBeam)
            Call WriteLineMomentLoadBeamHeaders(oSB)

            For i = 0 To modelData.BeamLineMoments.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... beam line moment: " + Str(i))
                End If
                Call WriteLineMomentLoadBeam(oSB, i, modelData.BeamLineMoments)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.EdgeLoads IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.LineLoadEdge)
            Call WriteEdgeLoadHeaders(oSB)

            For i = 0 To modelData.EdgeLoads.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... edge load: " + Str(i))
                End If
                Call WriteEdgeLoad(oSB, i, modelData.EdgeLoads)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.EdgeMoments IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.LineMomentEdge)
            Call WriteMomentLineLoadOnEdgeHeaders(oSB)

            For i = 0 To modelData.EdgeMoments.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... edge moment: " + Str(i))
                End If
                Call WriteMomentLineLoadOnEdge(oSB, i, modelData.EdgeMoments)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.SurfaceLoads IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.SurfaceLoad)
            Call WriteSurfaceLoadHeaders(oSB)

            For i = 0 To modelData.SurfaceLoads.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... surface load: " + Str(i))
                End If
                Call WriteSurfaceLoad(oSB, i, modelData.SurfaceLoads)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.FreePointLoads IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.FreePointLoad)
            Call WriteFreePointLoadHeaders(oSB)

            For i = 0 To modelData.FreePointLoads.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... free point load: " + Str(i))
                End If
                Call WriteFreePointLoad(oSB, modelData.Scale, i, modelData.FreePointLoads)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.FreePointMoments IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.FreePointMoment)
            Call WriteFreePointMomentLoadHeaders(oSB)

            For i = 0 To modelData.FreePointMoments.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... free point moment: " + Str(i))
                End If
                Call WriteFreePointMomentLoad(oSB, modelData.Scale, i, modelData.FreePointMoments)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.FreeLineLoads IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.FreeLineLoad)
            Call WriteFreeLineLoadHeaders(oSB)

            For i = 0 To modelData.FreeLineLoads.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... free line load: " + Str(i))
                End If
                Call WriteFreeLineLoad(oSB, modelData.Scale, i, modelData.FreeLineLoads)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.FreeSurfaceLoads IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.FreeSurfaceLoad)
            Call WriteFreeSurfaceLoadHeaders(oSB)

            For i = 0 To modelData.FreeSurfaceLoads.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... free surface load: " + Str(i))
                End If
                Call WriteFreeSurfaceLoad(oSB, modelData.Scale, i, modelData.FreeSurfaceLoads, UILanguageNumber)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.BeamThermalLoads IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.ThermalLoad1D)
            Call WriteBeamThermalLoadHeaders(oSB)

            For i = 0 To modelData.BeamThermalLoads.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... beam thermal load: " + Str(i))
                End If
                Call WriteBeamThermalLoad(oSB, i, modelData.BeamThermalLoads)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.SurfaceThermalLoads IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.ThermalLoad2D)
            Call WriteSurfaceThermalLoadHeaders(oSB)

            For i = 0 To modelData.SurfaceThermalLoads.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... surface thermal load: " + Str(i))
                End If
                Call WriteSurfaceThermalLoad(oSB, i, modelData.SurfaceThermalLoads)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.PretensionElements IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.PreTensionElement)
            Call WritePressTensionOnlyBeamNLHeaders(oSB)

            For i = 0 To modelData.PretensionElements.GetLength(0) - 1
                If i > 0 And (i Mod 500 = 0) Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... PretensionElement: " + Str(i))
                End If
                Call WritePressTensionOnlyBeamNL(oSB, i, modelData.PretensionElements)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.GapElements IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.GapElement)
            Call WriteGapLocalBeamNLHeaders(oSB)

            For i = 0 To modelData.GapElements.GetLength(0) - 1
                If i > 0 And (i Mod 500 = 0) Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... GapElement: " + Str(i))
                End If
                Call WriteGapLocalBeamNL(oSB, i, modelData.GapElements)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.Cables IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.Cable)
            Call WriteCableBeamNLHeaders(oSB)

            For i = 0 To modelData.Cables.GetLength(0) - 1
                If i > 0 And (i Mod 500 = 0) Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... cable: " + Str(i))
                End If
                Call WriteCableBeamNL(oSB, i, modelData.Cables)
            Next

            Call CloseContainerAndTable(oSB)
        End If


        If modelData.LimitForceElements IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.LimitForceElement)
            Call WriteLimitForceBeamNLHeaders(oSB)

            For i = 0 To modelData.LimitForceElements.GetLength(0) - 1
                If i > 0 And (i Mod 500 = 0) Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... LimitForceElement: " + Str(i))
                End If
                Call WriteLimitForceBeamNL(oSB, i, modelData.LimitForceElements)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.IntegrationStrips IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.IntegrationStrip)
            Call WriteIntegrationStripHeaders(oSB)

            For i = 0 To modelData.IntegrationStrips.GetLength(0) - 1
                If i > 0 And (i Mod 500 = 0) Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... integration strip: " + Str(i))
                End If
                Call WriteIntegrationStrip(oSB, i, modelData.IntegrationStrips)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.SectionOn1D IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.SectionOn1D)
            Call WriteSectionOn1DHeaders(oSB)

            For i = 0 To modelData.SectionOn1D.GetLength(0) - 1
                If i > 0 And (i Mod 500 = 0) Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... section on 1D: " + Str(i))
                End If
                Call WriteSectionOn1D(oSB, i, modelData.SectionOn1D)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.SectionOn2D IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.SectionOn2D)
            Call WriteSectionOn2DHeaders(oSB)

            For i = 0 To modelData.SectionOn2D.GetLength(0) - 1
                If i > 0 And (i Mod 500 = 0) Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... section on 2D: " + Str(i))
                End If
                Call WriteSectionOn2D(oSB, i, modelData.SectionOn2D)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        If modelData.AveragingStrips IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.AveragingStrip)
            Call WriteAveragingStripHeaders(oSB)

            For i = 0 To modelData.AveragingStrips.GetLength(0) - 1
                If i > 0 And (i Mod 500 = 0) Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... averaging strip: " + Str(i))
                End If
                Call WriteAveragingStrip(oSB, i, modelData.AveragingStrips)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        ' Write selection at end to make sure that referenced objects already exist in model
        If modelData.Selections IsNot Nothing Then
            Call OpenContainerAndTable(oSB, Koala.EsaObjectType.Selection)
            Call WriteSelectionHeaders(oSB)

            For i = 0 To modelData.Selections.GetLength(0) - 1
                If i > 0 And i Mod 100 = 0 Then
                    Rhino.RhinoApp.WriteLine("Creating the XML file string in memory... selection: " + Str(i))
                End If
                Call WriteSelection(oSB, i, modelData)
            Next

            Call CloseContainerAndTable(oSB)
        End If

        'close XML file--------------------------------------------------------------------
        oSB.AppendLine("</project>")

    End Sub

    Private Sub WriteSelectionHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Selected objects (Type.Name)"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteSelection(oSB As Text.StringBuilder, i As Long, modelData As ModelData)
        Dim selections = modelData.Selections

        oSB.AppendLine("<obj nm=""" & selections(i, 0).Trim & """>")
        oSB.AppendLine(ConCat_pv("0", selections(i, 0).Trim))

        Dim objDefinitionParts = selections(i, 1).Split(";")
        If objDefinitionParts.Length Mod 2 <> 0 Then
            Throw New ArgumentException("Invalid selected object definition: " & selections(i, 0))
        End If

        If objDefinitionParts.Length > 1 Then
            Dim j As Integer
            For j = 0 To objDefinitionParts.Length - 1 Step 2
                Dim objName As String = objDefinitionParts(j).Trim
                Dim objType As EsaObjectType = Koala.GetEnum(Of EsaObjectType)(objDefinitionParts(j + 1))
                If objType = EsaObjectType.Undefined Then
                    objType = modelData.FindObjectTypeByName(objName)
                End If
                If objType = EsaObjectType.Undefined Then
                    Throw New Exception("Failed to determine type of object " & objName & " for selection " & selections(i, 0))
                End If
                oSB.AppendLine(ConCat_pvx("1", ContainerTypes(objType) & "." & objName, j / 2))
            Next
        End If

        oSB.AppendLine("</obj>")
    End Sub


    Private Sub WriteLayerHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Comment"))
        oSB.AppendLine(ConCat_ht("2", "Structural model only"))
        oSB.AppendLine(ConCat_ht("3", "Current used activity"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteLayer(oSB As Object, i As Long, sE_layers(,) As Object)
        oSB.AppendLine("<obj id = """ & i.ToString() & """" & " nm=""" & sE_layers(i, 0) & """>")
        oSB.AppendLine(ConCat_pv("0", sE_layers(i, 0)))
        oSB.AppendLine(ConCat_pv("1", sE_layers(i, 1)))
        Select Case sE_layers(i, 2)
            Case "yes"
                oSB.AppendLine(ConCat_pv("2", "1"))
            Case "no"
                oSB.AppendLine(ConCat_pv("2", "0"))
            Case Else
                oSB.AppendLine(ConCat_pv("2", "0"))
        End Select
        oSB.AppendLine(ConCat_pv("3", "1"))
        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteConcrete(oSB As Object, material As Material)
        oSB.AppendLine("<obj nm=""" & Trim(material.Name) & """>")
        oSB.AppendLine(ConCat_pv("0", Trim(material.Name)))
        oSB.AppendLine(ConCat_pvt("2", "0", "Concrete"))

        Dim value As String = Nothing
        If material.Properties.TryGetValue(Material.MaterialProperty.ThermalExpansion, value) Then
            oSB.AppendLine(ConCat_pv("3", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.Density, value) Then
            oSB.AppendLine(ConCat_pv("4", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.E, value) Then
            oSB.AppendLine(ConCat_pv("6", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.Poisson, value) Then
            oSB.AppendLine(ConCat_pv("7", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.G, value) Then
            oSB.AppendLine(ConCat_pv("8", "1"))
            oSB.AppendLine(ConCat_pv("9", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.SpecificHeat, value) Then
            oSB.AppendLine(ConCat_pv("12", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.ThermalConductivity, value) Then
            oSB.AppendLine(ConCat_pv("13", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.AggregateSize, value) Then
            oSB.AppendLine(ConCat_pv("15", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.CementClass, value) Then
            oSB.AppendLine(ConCat_pvt_enum(Of Koala.CementClass)(16, value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.CompressiveStrength, value) Then
            oSB.AppendLine(ConCat_pv("18", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.AggregateType, value) Then
            oSB.AppendLine(ConCat_pvt_enum(Of Koala.TypeOfAggregate)(32, value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.DiagramType, value) Then
            oSB.AppendLine(ConCat_pvt_enum(Of Koala.TypeOfDiagram)(34, value))
        End If

        'todo add support for additional concrete properties
        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteSteel(oSB As Object, material As Material)
        oSB.AppendLine("<obj nm=""" & Trim(material.Name) & """>")
        oSB.AppendLine(ConCat_pv("0", Trim(material.Name)))
        oSB.AppendLine(ConCat_pvt("2", "0", "Steel"))

        Dim value As String = Nothing
        If material.Properties.TryGetValue(Material.MaterialProperty.ThermalExpansion, value) Then
            oSB.AppendLine(ConCat_pv("3", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.Density, value) Then
            oSB.AppendLine(ConCat_pv("4", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.E, value) Then
            oSB.AppendLine(ConCat_pv("5", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.Poisson, value) Then
            oSB.AppendLine(ConCat_pv("6", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.G, value) Then
            oSB.AppendLine(ConCat_pv("7", "1"))
            oSB.AppendLine(ConCat_pv("8", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.SpecificHeat, value) Then
            oSB.AppendLine(ConCat_pv("12", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.ThermalConductivity, value) Then
            oSB.AppendLine(ConCat_pv("13", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.UltimateStrength, value) Then
            oSB.AppendLine(ConCat_pv("14", value))
        End If

        If material.Properties.TryGetValue(Material.MaterialProperty.YieldStrength, value) Then
            oSB.AppendLine(ConCat_pv("15", value))
        End If

        'todo add support for additional steel properties
        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteNodeHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Coord X"))
        oSB.AppendLine(ConCat_ht("2", "Coord Y"))
        oSB.AppendLine(ConCat_ht("3", "Coord Z"))
        oSB.AppendLine(ConCat_ht("4", "Linked node"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteNode(ByRef oSB As Text.StringBuilder, modelData As ModelData, node As Node) 'write 1 node to the XML stream
        oSB.AppendLine("<obj nm=""" & Trim(node.Name) & """>")
        oSB.AppendLine(ConCat_pv("0", Trim(node.Name)))
        oSB.AppendLine(ConCat_pv("1", CStr(node.X * modelData.Scale)))
        oSB.AppendLine(ConCat_pv("2", CStr(node.Y * modelData.Scale)))
        oSB.AppendLine(ConCat_pv("3", CStr(node.Z * modelData.Scale)))
        If node.LinkedTo IsNot Nothing Then
            oSB.AppendLine(ConCat_pv("4", "to " & CStr(node.LinkedTo)))
        End If
        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteBeamHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Beg. node"))
        oSB.AppendLine(ConCat_ht("2", "End node"))
        oSB.AppendLine(ConCat_ht("3", "Layer"))
        oSB.AppendLine(ConCat_ht("4", "Cross-section"))
        oSB.AppendLine(ConCat_ht("5", "FEM type"))
        oSB.AppendLine(ConCat_ht("6", "Member system-line at"))
        oSB.AppendLine(ConCat_ht("7", "ey"))
        oSB.AppendLine(ConCat_ht("8", "ez"))
        oSB.AppendLine(ConCat_ht("9", "Table of geometry"))
        oSB.AppendLine(ConCat_ht("10", "Type"))
        oSB.AppendLine(ConCat_ht("11", "LCS"))
        oSB.AppendLine(ConCat_ht("12", "LCS Rotation"))
        oSB.AppendLine(ConCat_ht("13", "X"))
        oSB.AppendLine(ConCat_ht("14", "Y"))
        oSB.AppendLine(ConCat_ht("15", "Z"))
        oSB.AppendLine(ConCat_ht("16", "System lengths and buckling settings"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteBeam(ByRef oSB As Text.StringBuilder, ibeam As Integer, modelData As ModelData) 'write 1 beam to the XML stream
        'a beam consists of: Name, Section, Layer, LineShape, LCSType, LCSParam1, LCSParam2, LCSParam3
        Dim beams = modelData.Beams
        Dim LineType As String
        Dim i As Integer

        oSB.AppendLine("<obj nm=""" & beams(ibeam, 0) & """>")
        oSB.AppendLine(ConCat_pv("0", beams(ibeam, 0))) 'Name

        Dim ShapeAndNodes As String() = beams(ibeam, 3).Split(New Char() {";"c})
        LineType = ShapeAndNodes.ElementAt(0)

        Dim nodeStart = modelData.GetNodeName(ShapeAndNodes.ElementAt(1))
        Dim nodeEnd = modelData.GetNodeName(ShapeAndNodes.Last())

        oSB.AppendLine(ConCat_pn("1", nodeStart)) 'Beg. node
        oSB.AppendLine(ConCat_pn("2", nodeEnd)) 'End node

        oSB.AppendLine(ConCat_pn("3", beams(ibeam, 2))) 'layer
        oSB.AppendLine(ConCat_pn("4", beams(ibeam, 1))) 'Cross-Section


        Select Case beams(ibeam, 9)
            Case "standard"
                oSB.AppendLine(ConCat_pvt("5", "0", "standard"))
            Case "axial force only"
                oSB.AppendLine(ConCat_pvt("5", "1", "axial force only"))
            Case Else
                oSB.AppendLine(ConCat_pvt("5", "0", "standard"))
        End Select

        Select Case beams(ibeam, 10)
            Case "Centre"
                oSB.AppendLine(ConCat_pvt("6", "1", "Centre"))
            Case "Top"
                oSB.AppendLine(ConCat_pvt("6", "2", "Top"))
            Case "Bottom"
                oSB.AppendLine(ConCat_pvt("6", "4", "Bottom"))
            Case "Left"
                oSB.AppendLine(ConCat_pvt("6", "8", "Left"))
            Case "Top left"
                oSB.AppendLine(ConCat_pvt("6", "10", "Top left"))
            Case "Bottom left"
                oSB.AppendLine(ConCat_pvt("6", "12", "Bottom left"))
            Case "Right"
                oSB.AppendLine(ConCat_pvt("6", "16", "Right"))
            Case "Top right"
                oSB.AppendLine(ConCat_pvt("6", "18", "Top right"))
            Case "Bottom right"
                oSB.AppendLine(ConCat_pvt("6", "20", "Bottom right"))
            Case Else
                oSB.AppendLine(ConCat_pvt("6", "1", "Centre"))
        End Select

        oSB.AppendLine(ConCat_pv("7", beams(ibeam, 11))) 'ey
        oSB.AppendLine(ConCat_pv("8", beams(ibeam, 12))) 'ez

        If LineType = "Arc" Then
            Dim middleNode = modelData.GetNodeName(ShapeAndNodes.ElementAt(2))
            oSB.AppendLine(ConCat_opentable("9", ""))
            'Table of Geometry
            oSB.AppendLine("<h>")
            oSB.AppendLine(ConCat_ht("1", "Node"))
            oSB.AppendLine(ConCat_ht("2", "Edge"))
            oSB.AppendLine("</h>")

            oSB.AppendLine(ConCat_row(0))
            oSB.AppendLine(ConCat_pn("1", nodeStart))
            oSB.AppendLine(ConCat_pv("2", "1"))
            oSB.AppendLine("</row>")
            oSB.AppendLine(ConCat_row(1))
            oSB.AppendLine(ConCat_pn("1", middleNode))
            oSB.AppendLine("</row>")
            oSB.AppendLine(ConCat_row(2))
            oSB.AppendLine(ConCat_pn("1", nodeEnd))
            oSB.AppendLine("</row>")
            oSB.AppendLine(ConCat_closetable("9"))
        ElseIf LineType = "Polyline" Then
            oSB.AppendLine(ConCat_opentable("9", ""))
            'Table of Geometry
            oSB.AppendLine("<h>")
            oSB.AppendLine(ConCat_ht("1", "Node"))
            oSB.AppendLine(ConCat_ht("2", "Edge"))
            oSB.AppendLine("</h>")
            For i = 1 To ShapeAndNodes.Count - 2
                oSB.AppendLine(ConCat_row(i - 1))
                oSB.AppendLine(ConCat_pn("1", modelData.GetNodeName(ShapeAndNodes.ElementAt(i))))
                oSB.AppendLine(ConCat_pv("2", "0"))
                oSB.AppendLine("</row>")
            Next i
            oSB.AppendLine(ConCat_row(i - 1))
            oSB.AppendLine(ConCat_pn("1", modelData.GetNodeName(ShapeAndNodes.ElementAt(i))))
            oSB.AppendLine("</row>")

            oSB.AppendLine(ConCat_closetable("9"))
        ElseIf LineType = "Line" Then 'line
            oSB.AppendLine(ConCat_opentable("9", ""))
            'Table of Geometry
            oSB.AppendLine("<h>")
            oSB.AppendLine(ConCat_ht("1", "Node"))
            oSB.AppendLine(ConCat_ht("2", "Edge"))
            oSB.AppendLine("</h>")
            oSB.AppendLine(ConCat_row(0))
            oSB.AppendLine(ConCat_pn("1", nodeStart))
            oSB.AppendLine(ConCat_pv("2", "0"))
            oSB.AppendLine("</row>")
            oSB.AppendLine(ConCat_row(1))
            oSB.AppendLine(ConCat_pn("1", nodeEnd))
            oSB.AppendLine("</row>")
            oSB.AppendLine(ConCat_closetable("9"))
        ElseIf LineType = "Spline" Then
            oSB.AppendLine(ConCat_opentable("9", ""))
            'Table of Geometry
            oSB.AppendLine("<h>")
            oSB.AppendLine(ConCat_ht("1", "Node"))
            oSB.AppendLine(ConCat_ht("2", "Edge"))
            oSB.AppendLine("</h>")
            oSB.AppendLine(ConCat_row(0))
            oSB.AppendLine(ConCat_pn("1", nodeStart))
            oSB.AppendLine(ConCat_pv("2", "7"))
            oSB.AppendLine("</row>")
            For i = 2 To ShapeAndNodes.Count - 1
                oSB.AppendLine(ConCat_row(i - 1))
                oSB.AppendLine(ConCat_pn("1", modelData.GetNodeName(ShapeAndNodes.ElementAt(i))))
                oSB.AppendLine("</row>")
            Next i
            oSB.AppendLine(ConCat_closetable("9"))
        End If
        Select Case beams(ibeam, 8)
            Case "general"
                oSB.AppendLine(ConCat_pvt("10", "0", "general (0)")) 'type
            Case "column"
                oSB.AppendLine(ConCat_pvt("10", "2", "column (100)"))
            Case "gable column"
                oSB.AppendLine(ConCat_pvt("10", "3", "gable column (70)"))
            Case "secondary column"
                oSB.AppendLine(ConCat_pvt("10", "4", "secondary column (60)"))
            Case "rafter"
                oSB.AppendLine(ConCat_pvt("10", "5", "rafter (90)"))
            Case "purlin"
                oSB.AppendLine(ConCat_pvt("10", "6", "purlin (0)"))
            Case "roof bracing"
                oSB.AppendLine(ConCat_pvt("10", "7", "roof bracing (0)"))
            Case "wall bracing"
                oSB.AppendLine(ConCat_pvt("10", "8", "wall bracing (0)"))
            Case "girt"
                oSB.AppendLine(ConCat_pvt("10", "9", "girt (0)"))
            Case "truss chord"
                oSB.AppendLine(ConCat_pvt("10", "10", "truss chord (95)"))
            Case "truss diagonal"
                oSB.AppendLine(ConCat_pvt("10", "11", "truss diagonal (90)"))
            Case "plate rib"
                oSB.AppendLine(ConCat_pvt("10", "12", "plate rib (92)"))
            Case "beam slab"
                oSB.AppendLine(ConCat_pvt("10", "13", "beam slab (99)"))
            Case Else
                oSB.AppendLine(ConCat_pvt("10", "0", "general (0)")) 'type
        End Select



        If beams(ibeam, 4) = 0 Then 'standard LCS
            oSB.AppendLine(ConCat_pv("11", beams(ibeam, 4))) 'LCS type
            oSB.AppendLine(ConCat_pv("12", beams(ibeam, 5))) 'LCS rotation
        Else 'LCS by Z vector
            oSB.AppendLine(ConCat_pv("11", beams(ibeam, 4))) 'LCS type
            oSB.AppendLine(ConCat_pv("13", beams(ibeam, 5))) 'X
            oSB.AppendLine(ConCat_pv("14", beams(ibeam, 6))) 'Y
            oSB.AppendLine(ConCat_pv("15", beams(ibeam, 7))) 'Z
        End If

        If beams(ibeam, 13) >= 0 Then 'Buckling group
            oSB.AppendLine(ConCat_pv("16", beams(ibeam, 13)))
        End If

        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WritePressTensionOnlyBeamNLHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Reference table"))
        oSB.AppendLine(ConCat_ht("2", "Type"))
        oSB.AppendLine("</h>")
    End Sub


    Private Sub WritePressTensionOnlyBeamNL(ByRef oSB, i, beamnlocalnonlin(,))

        oSB.AppendLine("<obj nm=""" & "PTBNL" & Trim(Str(i)) & """>")
        oSB.AppendLine(ConCat_pv("0", "PTBNL" & Trim(Str(i)))) 'Name
        'write beam name as reference table
        oSB.AppendLine("<p1 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("2", beamnlocalnonlin(i, 0)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p1>")
        'end of reference table
        Select Case beamnlocalnonlin(i, 1)
            Case "Press only"
                oSB.AppendLine(ConCat_pvt("2", "0", "Press only"))
            Case "Tension only"
                oSB.AppendLine(ConCat_pvt("2", "1", "Tension only"))
        End Select
        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteLimitForceBeamNLHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Reference table"))
        oSB.AppendLine(ConCat_ht("2", "Type"))
        oSB.AppendLine(ConCat_ht("3", "Direction"))
        oSB.AppendLine(ConCat_ht("4", "Type"))
        oSB.AppendLine(ConCat_ht("5", "Marginal force"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteLimitForceBeamNL(ByRef oSB, i, LimitForce(,))
        oSB.AppendLine("<obj nm=""" & "LFBNL" & Trim(Str(i)) & """>")
        oSB.AppendLine(ConCat_pv("0", "LFBNL" & Trim(Str(i)))) 'Name
        'write beam name as reference table
        oSB.AppendLine("<p1 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("2", LimitForce(i, 0)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p1>")
        'end of reference table
        oSB.AppendLine(ConCat_pvt("2", "2", "Limit force"))
        Select Case LimitForce(i, 1)
            Case "Limit tension"
                oSB.AppendLine(ConCat_pvt("3", "0", "Limit tension"))
            Case "Limit compression"
                oSB.AppendLine(ConCat_pvt("3", "1", "Limit compression"))
        End Select
        Select Case LimitForce(i, 2)
            Case "Buckling"
                oSB.AppendLine(ConCat_pvt("4", "0", "Buckling ( results zero )"))
            Case "Tension only"
                oSB.AppendLine(ConCat_pvt("4", "1", "Plastic yielding"))
        End Select
        oSB.AppendLine(ConCat_pv("5", LimitForce(i, 3)))

        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteGapLocalBeamNLHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Reference table"))
        oSB.AppendLine(ConCat_ht("2", "Type"))
        oSB.AppendLine(ConCat_ht("3", "Type"))
        oSB.AppendLine(ConCat_ht("4", "Displacement"))
        oSB.AppendLine(ConCat_ht("5", "Position"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteGapLocalBeamNL(ByRef oSB, igap, gaps(,))
        oSB.AppendLine("<obj nm=""" & "GBNL" & Trim(Str(igap)) & """>")
        oSB.AppendLine(ConCat_pv("0", "GBNL" & Trim(Str(igap)))) 'Name
        'write beam name as reference table
        oSB.AppendLine("<p1 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("2", gaps(igap, 0)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p1>")
        'end of reference table
        oSB.AppendLine(ConCat_pvt("2", "3", "Gap"))
        Select Case gaps(igap, 1)
            Case "Press only"
                oSB.AppendLine(ConCat_pvt("3", "0", "Press only"))
            Case "Tension only"
                oSB.AppendLine(ConCat_pvt("3", "1", "Tension only"))
            Case "Both directions"
                oSB.AppendLine(ConCat_pvt("3", "2", "Both directions"))
        End Select
        oSB.AppendLine(ConCat_pv("4", gaps(igap, 2)))
        Select Case gaps(igap, 3)
            Case "Begin"
                oSB.AppendLine(ConCat_pvt("5", "0", "Begin"))
            Case "End"
                oSB.AppendLine(ConCat_pvt("5", "1", "End"))
        End Select
        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteCableBeamNLHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Reference table"))
        oSB.AppendLine(ConCat_ht("2", "Type"))
        oSB.AppendLine(ConCat_ht("3", "Initial mesh"))
        oSB.AppendLine(ConCat_ht("4", "Self weight"))
        oSB.AppendLine(ConCat_ht("5", "Normal force"))
        oSB.AppendLine(ConCat_ht("6", "Pn"))
        oSB.AppendLine(ConCat_ht("7", "Alpha x"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteCableBeamNL(ByRef oSB, icable, cables(,))
        oSB.AppendLine("<obj nm=""" & "CBNL" & Trim(Str(icable)) & """>")
        oSB.AppendLine(ConCat_pv("0", "CBNL" & Trim(Str(icable)))) 'Name
        'write beam name as reference table
        oSB.AppendLine("<p1 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("2", cables(icable, 0)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p1>")
        'end of reference table
        oSB.AppendLine(ConCat_pvt("2", "5", "Cable"))
        Select Case cables(icable, 1)

            Case "Straight"
                oSB.AppendLine(ConCat_pvt("3", "0", "Straight"))
            Case "Calculated"
                oSB.AppendLine(ConCat_pvt("3", "1", "Calculated"))


        End Select
        If (cables(icable, 2)) Then
            oSB.AppendLine(ConCat_pv("4", 1))
        Else
            oSB.AppendLine(ConCat_pv("4", 0))
        End If
        oSB.AppendLine(ConCat_pv("5", cables(icable, 3)))
        oSB.AppendLine(ConCat_pv("6", cables(icable, 4)))
        oSB.AppendLine(ConCat_pv("7", cables(icable, 5)))


        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteSurfaceHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Layer"))
        oSB.AppendLine(ConCat_ht("2", "Type"))
        oSB.AppendLine(ConCat_ht("3", "Material"))
        oSB.AppendLine(ConCat_ht("4", "FEM nonlinear model"))
        oSB.AppendLine(ConCat_ht("5", "Thickness type"))
        oSB.AppendLine(ConCat_ht("6", "Direction"))
        oSB.AppendLine(ConCat_ht("7", "Thickness"))
        oSB.AppendLine(ConCat_ht("8", "Point 1"))
        oSB.AppendLine(ConCat_ht("9", "Member system-plane at"))
        oSB.AppendLine(ConCat_ht("10", "Eccentricity z"))
        oSB.AppendLine(ConCat_ht("11", "Table of geometry"))
        oSB.AppendLine(ConCat_ht("12", "Internal nodes"))
        oSB.AppendLine(ConCat_ht("13", "Swap orientation"))
        oSB.AppendLine(ConCat_ht("14", "LCS angle"))
        oSB.AppendLine(ConCat_ht("15", "Element type"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteSurface(ByRef osb As Text.StringBuilder, isurface As Integer, modelData As ModelData)
        Dim row_id As Long, inode As Long
        Dim iedge As Long
        Dim edges() As String, nodes() As String
        Dim surfaces = modelData.Surfaces

        osb.AppendLine("<obj nm=""" & surfaces(isurface, 0) & """>")
        osb.AppendLine(ConCat_pv("0", surfaces(isurface, 0)))
        osb.AppendLine(ConCat_pn("1", surfaces(isurface, 4))) 'layer
        Select Case surfaces(isurface, 1)
            Case "Shell"
                osb.AppendLine(ConCat_pv("2", "2")) 'Shell type
            Case "Plate"
                osb.AppendLine(ConCat_pv("2", "0")) 'Plate type
            Case "Wall"
                osb.AppendLine(ConCat_pv("2", "1")) 'wall type
            Case Else
                osb.AppendLine(ConCat_pv("2", "0")) 'Plate type
        End Select

        osb.AppendLine(ConCat_pn("3", surfaces(isurface, 2))) 'material
        Select Case surfaces(isurface, 9)
            Case "none"
                osb.AppendLine(ConCat_pvt("4", "0", "none"))
            Case "Press only"
                osb.AppendLine(ConCat_pvt("4", "1", "Press only"))
            Case "Membrane"
                osb.AppendLine(ConCat_pvt("4", "2", "Membrane"))
            Case Else
                osb.AppendLine(ConCat_pvt("4", "0", "none"))
        End Select

        Dim thickness() As String
        thickness = Split(surfaces(isurface, 3), "|")

        If thickness.Length = 1 Then ' if we've only given a single thickness value, just consider it as a constant thickness slab
            osb.AppendLine(ConCat_pvt("5", "0", "constant"))
            osb.AppendLine(ConCat_pv("7", CStr(thickness(0) / 1000))) 'thickness
        Else
            Dim thicknessType As String
            thicknessType = thickness(0)
            Select Case thicknessType
                Case "constant"
                    osb.AppendLine(ConCat_pvt("5", "0", "constant"))
                    Dim thck As Double
                    thck = thickness(1) / 1000
                    osb.AppendLine(ConCat_pv("7", CStr(thck))) 'thickness
                Case "variable"
                    osb.AppendLine(ConCat_pvt("5", "1", "variable"))
                    Dim variableThicknessType As String
                    variableThicknessType = thickness(1)

                    Select Case variableThicknessType
                        Case "Global X"
                            osb.AppendLine(ConCat_pvt("6", "1", "Global X"))
                        Case "Global Y"
                            osb.AppendLine(ConCat_pvt("6", "2", "Global Y"))
                        Case "Global Z"
                            osb.AppendLine(ConCat_pvt("6", "3", "Global Z"))
                        Case "Local X"
                            osb.AppendLine(ConCat_pvt("6", "4", "Local X"))
                        Case "Local Y"
                            osb.AppendLine(ConCat_pvt("6", "5", "Local Y"))
                        Case "Variable in two directions"
                            osb.AppendLine(ConCat_pvt("6", "6", "Variable in two directions"))
                        Case "Radial"
                            osb.AppendLine(ConCat_pvt("6", "7", "Radial"))
                        Case "Variable in 4 pt."
                            osb.AppendLine(ConCat_pvt("6", "8", "Variable in 4 pt."))

                    End Select
                    Dim thicknessProperties() As String
                    Dim i As Long
                    Dim thicknesses As New List(Of Double)
                    Dim thicknessNodes As New List(Of String)
                    If variableThicknessType = "Radial" Then
                        For i = 2 To thickness.Count() - 1
                            osb.AppendLine(ConCat_pvx("7", thickness(i) / 1000, i - 2)) 'value in mm
                        Next
                    Else
                        For i = 2 To thickness.Count() - 1
                            thicknessProperties = Split(thickness(i), ";")
                            thicknesses.Add(thicknessProperties(0))
                            thicknessNodes.Add(thicknessProperties(1))
                        Next
                        For i = 0 To thicknesses.Count() - 1
                            osb.AppendLine(ConCat_pvx("7", thicknesses(i) / 1000, i)) 'value in mm
                        Next
                        For i = 0 To thicknessNodes.Count() - 1
                            osb.AppendLine(ConCat_pnx("8", thicknessNodes(i), i))
                        Next
                    End If


            End Select
        End If

        Select Case surfaces(isurface, 7)
            Case "Centre"
                osb.AppendLine(ConCat_pvt("9", "1", "Centre"))
            Case "Top"
                osb.AppendLine(ConCat_pvt("9", "2", "Top"))
            Case "Bottom"
                osb.AppendLine(ConCat_pvt("9", "4", "Bottom"))
            Case Else
                osb.AppendLine(ConCat_pvt("9", "1", "Centre"))
        End Select

        osb.AppendLine(ConCat_pv("10", surfaces(isurface, 8)))

        'table of geometry
        osb.AppendLine(ConCat_opentable("11", ""))
        osb.AppendLine("<h>")
        osb.AppendLine(ConCat_ht("0", "Closed curve"))
        osb.AppendLine(ConCat_ht("1", "Node"))
        osb.AppendLine(ConCat_ht("2", "Edge"))
        osb.AppendLine("</h>")

        'loop through all edges
        row_id = 0
        edges = Strings.Split(surfaces(isurface, 5), "|")

        For iedge = 0 To edges.Count - 1
            inode = 0
            osb.AppendLine(ConCat_row(row_id))
            osb.AppendLine(ConCat_pv("0", "1")) 'Closed curve
            osb.AppendLine(ConCat_pn("1", modelData.GetNodeName(Trim(Split(edges(iedge), ";")(1))))) 'first node
            Select Case Strings.Trim(Strings.Split(edges(iedge), ";")(0)) 'curve type
                Case "Line"
                    osb.AppendLine(ConCat_pvt("2", "0", "Line"))
                Case "Arc"
                    osb.AppendLine(ConCat_pvt("2", "1", "Circle arc"))
                Case "Spline"
                    osb.AppendLine(ConCat_pvt("2", "7", "Spline"))
                Case "Circle"
                    osb.AppendLine(ConCat_pvt("2", "2", "Circle by centre and vertex"))
            End Select

            While inode < UBound(Split(edges(iedge), ";")) - 1
                inode = inode + 1
                row_id = row_id + 1
                osb.AppendLine("</row>")
                osb.AppendLine(ConCat_row(row_id))
                If Split(edges(iedge), ";")(1 + inode).Contains("[") Then
                    Dim Normal As String
                    Normal = Split(edges(iedge), ";")(1 + inode).Replace(",", ";")
                    osb.AppendLine(ConCat_pin("1", "-1", Normal))
                Else
                    osb.AppendLine(ConCat_pn("1", Trim(Split(edges(iedge), ";")(1 + inode))))
                End If

            End While
            osb.AppendLine("</row>")
            row_id = row_id + 1

        Next


        osb.AppendLine(ConCat_closetable("11"))

        'loop through all nodes
        row_id = 0
        If Not String.IsNullOrEmpty(surfaces(isurface, 6)) Then
            nodes = Strings.Split(surfaces(isurface, 6), ";")
            'internal nodes
            If nodes.Count <> 0 And Not String.IsNullOrEmpty(nodes(0)) Then
                osb.AppendLine(ConCat_opentable("12", ""))
                osb.AppendLine("<h>")
                osb.AppendLine(ConCat_ht("0", "Node"))
                osb.AppendLine("</h>")

                For inode = 0 To nodes.Count - 1
                    osb.AppendLine(ConCat_row(row_id))
                    osb.AppendLine(ConCat_pn("0", modelData.GetNodeName(Trim(nodes(inode)))))
                    osb.AppendLine("</row>")
                    row_id += 1

                Next inode
            End If

            osb.AppendLine(ConCat_closetable("12"))
        End If

        osb.AppendLine(ConCat_pv("13", surfaces(isurface, 10)))
        osb.AppendLine(ConCat_pv("14", surfaces(isurface, 11)))
        osb.AppendLine(ConCat_pvt("15", "1", "Standard"))

        osb.AppendLine("</obj>")

    End Sub

    Private Sub WriteLoadPanelHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Layer"))
        oSB.AppendLine(ConCat_ht("2", "Table of geometry"))
        oSB.AppendLine(ConCat_ht("3", "Element type"))
        oSB.AppendLine(ConCat_ht("4", "Panel type"))
        oSB.AppendLine(ConCat_ht("5", "Load transfer direction"))
        oSB.AppendLine(ConCat_ht("6", "Transfer in X [%]"))
        oSB.AppendLine(ConCat_ht("7", "Transfer in Y [%]"))
        oSB.AppendLine(ConCat_ht("8", "Load transfer method"))
        oSB.AppendLine(ConCat_ht("9", "Selection of entities"))
        oSB.AppendLine(ConCat_ht("10", "Swap orientation"))
        oSB.AppendLine(ConCat_ht("11", "LCS angle"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteLoadPanels(ByRef osb As Text.StringBuilder, iloadpanel As Integer, modelData As ModelData) 'write 1 surface to the XML stream
        Dim row_id As Long, inode As Long
        Dim iedge As Long
        Dim edges() As String
        Dim loadPanels = modelData.LoadPanels

        osb.AppendLine("<obj nm=""" & loadPanels(iloadpanel, 0) & """>")
        osb.AppendLine(ConCat_pv("0", loadPanels(iloadpanel, 0))) ' name
        osb.AppendLine(ConCat_pn("1", loadPanels(iloadpanel, 1))) 'layer

        'table of geometry
        osb.AppendLine(ConCat_opentable("2", ""))
        osb.AppendLine("<h>")
        osb.AppendLine(ConCat_ht("0", "Closed curve"))
        osb.AppendLine(ConCat_ht("1", "Node"))
        osb.AppendLine(ConCat_ht("2", "Edge"))
        osb.AppendLine("</h>")

        'loop through all edges
        row_id = 0
        edges = Strings.Split(loadPanels(iloadpanel, 2), "|")

        For iedge = 0 To edges.Count - 1
            inode = 0
            osb.AppendLine(ConCat_row(row_id))
            osb.AppendLine(ConCat_pv("0", "1")) 'Closed curve
            Dim nodeName As String = Trim(Split(edges(iedge), ";")(1))
            osb.AppendLine(ConCat_pn("1", modelData.GetNodeName(nodeName))) 'first node
            Select Case Strings.Trim(Strings.Split(edges(iedge), ";")(0)) 'curve type
                Case "Line"
                    osb.AppendLine(ConCat_pvt("2", "0", "Line"))
                Case "Arc"
                    osb.AppendLine(ConCat_pvt("2", "1", "Circle arc"))
                Case "Spline"
                    osb.AppendLine(ConCat_pvt("2", "7", "Spline"))
                Case "Circle"
                    osb.AppendLine(ConCat_pvt("2", "2", "Circle by centre and vertex"))
            End Select

            While inode < UBound(Split(edges(iedge), ";")) - 1
                inode = inode + 1
                row_id = row_id + 1
                osb.AppendLine("</row>")
                osb.AppendLine(ConCat_row(row_id))
                If Split(edges(iedge), ";")(1 + inode).Contains("[") Then
                    Dim Normal As String
                    Normal = Split(edges(iedge), ";")(1 + inode).Replace(",", ";")
                    osb.AppendLine(ConCat_pin("1", "-1", Normal))
                Else
                    osb.AppendLine(ConCat_pn("1", Trim(Split(edges(iedge), ";")(1 + inode))))
                End If
            End While
            osb.AppendLine("</row>")
            row_id = row_id + 1
        Next
        osb.AppendLine(ConCat_closetable("2"))

        osb.AppendLine(ConCat_pvt("3", "5", "Load panel"))
        osb.AppendLine(ConCat_pvt_enum(Of Koala.LoadPanelType)(4, loadPanels(iloadpanel, 3)))
        osb.AppendLine(ConCat_pvt_enum(Of Koala.LoadPanelTransferDirection)(5, loadPanels(iloadpanel, 4)))

        osb.AppendLine(ConCat_pv("6", "50"))
        osb.AppendLine(ConCat_pv("7", "50"))
        'osb.AppendLine(ConCat_ht("6", "Transfer in X [%]"))
        'osb.AppendLine(ConCat_ht("7", "Transfer in Y [%]"))

        osb.AppendLine(ConCat_pvt_enum(Of Koala.LoadPanelTransferMethod)(8, loadPanels(iloadpanel, 5)))
        osb.AppendLine(ConCat_pvt("9", "0", "All")) 'selection of entities - -Z +Z all

        osb.AppendLine(ConCat_pv("10", loadPanels(iloadpanel, 6)))
        osb.AppendLine(ConCat_pv("11", loadPanels(iloadpanel, 7)))

        osb.AppendLine("</obj>")

    End Sub

    Private Sub WriteOpeningHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Reference table"))
        oSB.AppendLine(ConCat_ht("1", "Name"))
        oSB.AppendLine(ConCat_ht("2", "2D Member"))
        oSB.AppendLine(ConCat_ht("3", "Table of geometry"))
        oSB.AppendLine(ConCat_ht("4", "Material"))
        oSB.AppendLine(ConCat_ht("5", "Thickness"))
        oSB.AppendLine(ConCat_ht("6", "Table of geometry"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteOpening(ByRef osb, iopening, modelData) 'write 1 opening to the XML stream
        Dim row_id As Long, inode As Long
        Dim iedge As Long
        Dim edges() As String
        Dim openings = modelData.Openings

        osb.AppendLine("<obj nm=""" & openings(iopening, 0) & """>")
        'write surface name as reference table
        osb.AppendLine("<p0 t="""">")
        osb.AppendLine("<h>")
        osb.AppendLine("<h0 t=""Member Type""/>")
        osb.AppendLine("<h1 t=""Member Type Name""/>")
        osb.AppendLine("<h2 t=""Member Name""/>")
        osb.AppendLine("</h>")
        osb.AppendLine("<row id=""0"">")
        osb.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member2D)))
        osb.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member2D)))
        osb.AppendLine(ConCat_pv("2", openings(iopening, 1)))
        osb.AppendLine("</row>")
        osb.AppendLine("</p0>")
        'end of reference table

        osb.AppendLine(ConCat_pv("1", openings(iopening, 0))) 'name
        osb.AppendLine(ConCat_opentable("3", ""))
        'table of geometry
        osb.AppendLine("<h>")
        osb.AppendLine(ConCat_ht("0", "Closed curve"))
        osb.AppendLine(ConCat_ht("1", "Node"))
        osb.AppendLine(ConCat_ht("2", "Edge"))
        osb.AppendLine("</h>")

        'Loop through all edges
        row_id = 0
        edges = Strings.Split(openings(iopening, 2), "|")

        For iedge = 0 To edges.Count - 1
            inode = 0
            osb.AppendLine(ConCat_row(row_id))
            osb.AppendLine(ConCat_pv("0", "1")) 'Closed curve
            osb.AppendLine(ConCat_pn("1", modelData.GetNodeName(Trim(Split(edges(iedge), ";")(1))))) 'first node
            Select Case Strings.Trim(Strings.Split(edges(iedge), ";")(0)) 'curve type
                Case "Line"
                    osb.AppendLine(ConCat_pvt("2", "0", "Line"))
                Case "Arc"
                    osb.AppendLine(ConCat_pvt("2", "1", "Circle arc"))
                Case "Circle"
                    osb.AppendLine(ConCat_pvt("2", "4", "Circle by 3 pts"))
                Case "Spline"
                    osb.AppendLine(ConCat_pvt("2", "7", "Spline"))
            End Select

            While inode < UBound(Split(edges(iedge), ";")) - 1
                inode = inode + 1
                row_id = row_id + 1
                osb.AppendLine("</row>")
                osb.AppendLine(ConCat_row(row_id))
                osb.AppendLine(ConCat_pn("1", Trim(Split(edges(iedge), ";")(1 + inode))))

            End While
            osb.AppendLine("</row>")
            row_id = row_id + 1

        Next

        osb.AppendLine(ConCat_closetable("3"))
        osb.AppendLine("</obj>")

    End Sub
    Private Sub WriteInternalEdgeHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "2D Member"))
        oSB.AppendLine(ConCat_ht("2", "Shape"))
        oSB.AppendLine(ConCat_ht("3", "Table of geometry"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteInternalEdge(ByRef osb As Text.StringBuilder, iInternalEdge As Integer, modelData As ModelData)
        Dim slabInternalEdges = modelData.SlabInternalEdges

        osb.AppendLine("<obj nm=""" & slabInternalEdges(iInternalEdge, 0) & """>")

        osb.AppendLine(ConCat_pv("0", slabInternalEdges(iInternalEdge, 0))) ' name
        osb.AppendLine(ConCat_pn("1", slabInternalEdges(iInternalEdge, 1))) ' 2D member

        Dim nodeStart As String, nodeEnd As String, MiddleNode As String

        Dim ShapeAndNodes As String() = slabInternalEdges(iInternalEdge, 2).Split(New Char() {";"c})
        Dim LineType = ShapeAndNodes.ElementAt(0)
        nodeStart = modelData.GetNodeName(ShapeAndNodes.ElementAt(1))
        nodeEnd = modelData.GetNodeName(ShapeAndNodes.Last())
        Dim i As Long = 0
        ' shape
        osb.AppendLine(ConCat_pv("2", LineType)) ' Shape
        osb.AppendLine(ConCat_opentable("3", "")) 'table of geometry
        If LineType = "Arc" Then

            MiddleNode = modelData.GetNodeName(ShapeAndNodes.ElementAt(2))

            'Table of Geometry
            osb.AppendLine("<h>")
            osb.AppendLine(ConCat_ht("1", "Node"))
            osb.AppendLine(ConCat_ht("2", "Edge"))
            osb.AppendLine("</h>")

            osb.AppendLine(ConCat_row(0))
            osb.AppendLine(ConCat_pn("1", nodeStart))
            osb.AppendLine(ConCat_pv("2", "1"))
            osb.AppendLine("</row>")
            osb.AppendLine(ConCat_row(1))
            osb.AppendLine(ConCat_pn("1", MiddleNode))
            osb.AppendLine("</row>")
            osb.AppendLine(ConCat_row(2))
            osb.AppendLine(ConCat_pn("1", nodeEnd))
            osb.AppendLine("</row>")

        ElseIf LineType = "Polyline" Then

            'Table of Geometry
            osb.AppendLine("<h>")
            osb.AppendLine(ConCat_ht("1", "Node"))
            osb.AppendLine(ConCat_ht("2", "Edge"))
            osb.AppendLine("</h>")
            For i = 1 To ShapeAndNodes.Count - 2
                osb.AppendLine(ConCat_row(i - 1))
                osb.AppendLine(ConCat_pn("1", modelData.GetNodeName(ShapeAndNodes.ElementAt(i))))
                osb.AppendLine(ConCat_pv("2", "0"))
                osb.AppendLine("</row>")
            Next i
            osb.AppendLine(ConCat_row(i - 1))
            osb.AppendLine(ConCat_pn("1", modelData.GetNodeName(ShapeAndNodes.ElementAt(i))))
            osb.AppendLine("</row>")


        ElseIf LineType = "Line" Then 'line

            'Table of Geometry
            osb.AppendLine("<h>")
            osb.AppendLine(ConCat_ht("1", "Node"))
            osb.AppendLine(ConCat_ht("2", "Edge"))
            osb.AppendLine("</h>")
            osb.AppendLine(ConCat_row(0))
            osb.AppendLine(ConCat_pn("1", nodeStart))
            osb.AppendLine(ConCat_pv("2", "0"))
            osb.AppendLine("</row>")
            osb.AppendLine(ConCat_row(1))
            osb.AppendLine(ConCat_pn("1", nodeEnd))
            osb.AppendLine("</row>")

        ElseIf LineType = "Spline" Then

            'Table of Geometry
            osb.AppendLine("<h>")
            osb.AppendLine(ConCat_ht("1", "Node"))
            osb.AppendLine(ConCat_ht("2", "Edge"))
            osb.AppendLine("</h>")
            osb.AppendLine(ConCat_row(0))
            osb.AppendLine(ConCat_pn("1", nodeStart))
            osb.AppendLine(ConCat_pv("2", "7"))
            osb.AppendLine("</row>")
            For i = 2 To ShapeAndNodes.Count - 1
                osb.AppendLine(ConCat_row(i - 1))
                osb.AppendLine(ConCat_pn("1", modelData.GetNodeName(ShapeAndNodes.ElementAt(i))))
                osb.AppendLine("</row>")
            Next i

        End If
        osb.AppendLine(ConCat_closetable("3"))

        osb.AppendLine("</obj>")

    End Sub

    Private Sub WriteRigidArmHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Master"))
        oSB.AppendLine(ConCat_ht("2", "Slave"))
        oSB.AppendLine(ConCat_ht("1", "Hinge on master"))
        oSB.AppendLine(ConCat_ht("2", "Hinge on slave"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteRigidArm(ByRef osb, iRigidArm, RigidArms(,))

        osb.AppendLine("<obj nm=""" & RigidArms(iRigidArm, 0) & """>")

        osb.appendline(ConCat_pv("0", RigidArms(iRigidArm, 0)))
        osb.AppendLine(ConCat_pn("1", RigidArms(iRigidArm, 1)))
        osb.appendline(ConCat_pn("2", RigidArms(iRigidArm, 2)))
        If (RigidArms(iRigidArm, 3)) Then
            osb.appendline(ConCat_pv("3", "1"))
        Else
            osb.appendline(ConCat_pv("3", "0"))
        End If
        If (RigidArms(iRigidArm, 4)) Then
            osb.appendline(ConCat_pv("4", "1"))
        Else
            osb.appendline(ConCat_pv("4", "0"))
        End If


        osb.AppendLine("</obj>")

    End Sub

    Private Sub WriteSectionHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Catalog ID"))
        oSB.AppendLine(ConCat_ht("2", "Catalog item"))
        oSB.AppendLine(ConCat_ht("3", "Parameters"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteSection(ByRef oSB, sectionname, sectioncode, sectiondef, sectionmat) 'write 1 profile: hot-rolled steel or concrete

        Dim sectiontype As String
        Dim formtype As String
        Dim formcode As Long
        oSB.AppendLine("<obj nm=""" & sectionname & """>")
        oSB.AppendLine(ConCat_pv("0", sectionname)) 'Name

        formtype = Strings.UCase(Strings.Left(sectioncode, 4))

        If formtype = "FORM" Then
            'steel rolled profiles
            formcode = Strings.Split(sectioncode, "-")(1)

            oSB.AppendLine(ConCat_pv("1", "EP_CssLib.EP_ProfLib_Rolled.1"))
            oSB.AppendLine(ConCat_pv("2", formcode))
            oSB.AppendLine("<p3 t="""">")
            oSB.AppendLine("<h>")
            oSB.AppendLine(ConCat_ht("0", "Name"))
            oSB.AppendLine(ConCat_ht("1", "Material"))
            oSB.AppendLine(ConCat_ht("5", "Rolled section"))
            oSB.AppendLine("</h>")
            oSB.AppendLine("<row id=""0"">")
            oSB.AppendLine(ConCat_pv("0", "Material"))
            oSB.AppendLine(ConCat_pn("1", sectionmat))
            oSB.AppendLine("</row>")
            oSB.AppendLine("<row id=""1"">")
            oSB.AppendLine(ConCat_pv("5", sectiondef & "|" & formcode))
            oSB.AppendLine("</row>")
            oSB.AppendLine("</p3>")

        Else
            Dim catalogueLib As String
            Dim catalogueItem As String
            sectiontype = Strings.UCase(Strings.Left(sectiondef, 4))

            If formtype = "CONC" Then
                catalogueLib = "EP_CssLib.EP_ProfLib_Concrete.1"
                Select Case sectiontype
                    Case "RECT" '> full rectangle
                        catalogueItem = "0"
                    Case "ISEC" '> Isection
                        catalogueItem = "1"
                    Case "TSEC" '> Tsection
                        catalogueItem = "2"
                    Case "LSEC" '> L section
                        catalogueItem = "3"
                    Case "CIRC" '> full circle
                        catalogueItem = "4"
                    Case "OVAL" '> oval
                        catalogueItem = "5"
                    Case "LREV" '> L rev 
                        catalogueItem = "6"
                    Case Else
                        Throw New ArgumentException("Invalid Concrete section type " & sectiontype)
                End Select

            ElseIf formtype = "TIMB" Then
                catalogueLib = "EP_CssLib.EP_ProfLib_Timber.1"

                Select Case sectiontype
                    Case "RECT" '> full rectangle
                        catalogueItem = "0"
                    Case "CIRC" '> full circle
                        catalogueItem = "1"
                    Case "TSEC" '> Tsection
                        catalogueItem = "4"
                    Case "ISEC" '> Isection
                        catalogueItem = "5"
                        'Case "LSEC" '> L section
                        '    catalogueItem = "3"'
                        'Case "LREV" '> L rev 
                        '    catalogueItem = "6" '
                        'Case "OVAL" '> oval
                        '    catalogueItem = "5"'
                        'Case "DREC" '> DoubleRectangle
                        '    catalogueItem = "7"
                        'Case "TREC" '> TripleRectangle
                        '    catalogueItem = "8"
                        'Case "ISSH" '> ISectionWithHaunch
                        '    catalogueItem = "9"
                        'Case "CSEC" '> CSection
                        '    catalogueItem = "10"
                        'Case "USEC" '> USection
                        '    catalogueItem = "11"
                        'Case "PIPE" '> Pipe
                        '    catalogueItem = "12"
                        'Case "POLY" '> Polygon
                        '    catalogueItem = "13"
                        'Case "XSEC" '> XSection
                        '    catalogueItem = "14"
                        'Case "ZSEC" '> ZSection
                        '    catalogueItem = "15"
                        'Case "SBOX" '> Box
                        '    catalogueItem = "16"
                        'Case "DBOX" '> DoubleBox
                        '    catalogueItem = "17"
                        'Case "TRAP" '> Trapezoid
                        '    catalogueItem = "28"
                        'Case "ISAH" '> ISectionWithHaunchAsymmetric
                        '    catalogueItem = "29"
                        'Case "TSSH" '> TSectionWithHaunch
                        '    catalogueItem = "30"
                        'Case "SREP" '> RectangleWithPlates
                        '    catalogueItem = "31"
                        'Case "DREP" '> DoubleRectangleWithPlates
                        '    catalogueItem = "32"
                    Case Else
                        Throw New ArgumentException("Invalid Timber section type " & sectiontype)
                End Select
            Else
                catalogueLib = "EP_CssLib.EP_ProfLib_GeomThinWalled.1"

                Select Case sectiontype
                    Case "IROS" '> IRolled
                        catalogueItem = "0"
                    Case "TUBE" '> Tube
                        catalogueItem = "1"
                    Case "PIPE" '> Pipe
                        catalogueItem = "2"
                    Case "ANGL" '> Angle
                        catalogueItem = "3"
                    Case "CHAN" '> Channel
                        catalogueItem = "4"
                    Case "TTEE" '> TTee
                        catalogueItem = "5"
                    Case "RECT" '> full rectangle
                        catalogueItem = "6"
                    Case "CIRC" '> full circle
                        catalogueItem = "7"
                    Case "IROA" '> IRolledAsymmetric
                        catalogueItem = "8"
                        'Case "ZZEE" '> ZZee
                        '    catalogueItem = "24"
                        'Case "CFCH" '> ColdFormedChannel
                        '    catalogueItem = "25"
                        'Case "CFCL" '> ColdFormedChannelWithLips
                        '    catalogueItem = "26"
                        'Case "CFZE" '> ColdFormedZee
                        '    catalogueItem = "27"
                    Case Else
                        Throw New ArgumentException("Invalid Thin-Walled section type " & sectiontype)
                End Select
            End If

            Dim dimensionValues As Double() = Array.ConvertAll(Split(Mid(sectiondef, 5), "x"), New Converter(Of String, Double)(AddressOf Double.Parse))
            Dim dimensionNameString As String

            Select Case sectiontype
                Case "RECT" '> full rectangle
                    dimensionNameString = "H;B"
                Case "ISEC" '> Isection
                    dimensionNameString = "H;Bh;Bs;ts;th;s"
                Case "TSEC" '> Tsection
                    dimensionNameString = "H;B;th;sh"
                Case "LSEC" '> L section
                    dimensionNameString = "H;B;th;sh"
                Case "LREV" '> L rev 
                    dimensionNameString = "H;B;th;sh"
                Case "CIRC" '> full circle
                    dimensionNameString = "D"
                Case "OVAL" '> oval
                    dimensionNameString = "H;B"
                Case "DREC" '> DoubleRectangle
                    dimensionNameString = "tha;Ba;a"
                Case "TREC" '> TripleRectangle
                    dimensionNameString = "tha;Ba;a"
                Case "ISSH" '> ISectionWithHaunch
                    dimensionNameString = "H;B;ta;s;ts;th"
                Case "CSEC" '> CSection
                    dimensionNameString = "A;tha1;tha2;B;thb1;thb2"
                Case "USEC" '> USection
                    dimensionNameString = "H;B;th;sh"
                Case "PIPE" '> Pipe
                    dimensionNameString = "D;t"
                Case "POLY" '> Polygon
                    dimensionNameString = "R;n"
                Case "XSEC" '> XSection
                    dimensionNameString = "A;tha;B;thb"
                Case "ZSEC" '> ZSection
                    dimensionNameString = "A;tha;B;thb1;thb2;C"
                Case "SBOX" '> Box
                    dimensionNameString = "A;tha;B;thb1;thb2"
                Case "DBOX" '> DoubleBox
                    dimensionNameString = "Ba;ha;Bb;hb"
                Case "IROS" '> IRolled
                    dimensionNameString = "H;B;t;s;R"
                Case "IROA" '> IRolledAsymmetric
                    dimensionNameString = "H;s;Bt;Bb;tt;tb;R"
                Case "TUBE" '> Tube
                    dimensionNameString = "H;B;s;R;r1"
                Case "ANGL" '> Angle
                    dimensionNameString = "H;B;t;R;R1"
                Case "CHAN" '> Channel
                    dimensionNameString = "H;B;t;s;R"
                Case "TTEE" '> TTee
                    dimensionNameString = "H;B;t;s;R"
                Case "ZZEE" '> ZZee
                    dimensionNameString = "H;B;t;s;R;R1"
                Case "CFCH" '> ColdFormedChannel
                    dimensionNameString = "H;B;s;r"
                Case "CFCL" '> ColdFormedChannelWithLips
                    dimensionNameString = "H;B;s;r;c"
                Case "CFZE" '> ColdFormedZee
                    dimensionNameString = "H;B;s;r"
                Case "TRAP" '> Trapezoid
                    dimensionNameString = "H;Bh;Bs"
                Case "ISAH" '> ISectionWithHaunchAsymmetric
                    dimensionNameString = "H;Bt;tt;tth;Bb;tb;tbh;s"
                Case "TSSH" '> TSectionWithHaunch
                    dimensionNameString = "H;Bh;Bs;Bw;th"
                Case "SREP" '> RectangleWithPlates
                    dimensionNameString = "tha;Ba;thb;Bb"
                Case "DREP" '> DoubleRectangleWithPlates
                    dimensionNameString = "Ba;ha;Bb;hb"
                Case Else
                    Throw New ArgumentException("Invalid section type " & sectiontype)
            End Select

            oSB.AppendLine(ConCat_pv("1", catalogueLib))
            oSB.AppendLine(ConCat_pv("2", catalogueItem))
            oSB.AppendLine("<p3 t="""">")
            oSB.AppendLine("<h>")
            oSB.AppendLine(ConCat_ht("0", "Name"))
            oSB.AppendLine(ConCat_ht("1", "Material"))
            oSB.AppendLine(ConCat_ht("4", "Length"))
            oSB.AppendLine("</h>")
            oSB.AppendLine("<row id=""0"">")
            oSB.AppendLine(ConCat_pv("0", "Material"))
            oSB.AppendLine(ConCat_pn("1", sectionmat))
            oSB.AppendLine("</row>")

            Dim dimensionNames As String() = Split(dimensionNameString, ";")
            If dimensionNames.Length < dimensionValues.Length Then
                Throw New ArgumentException("Expected the following dimensions for section type " & sectiontype & ": " & dimensionNameString)
            End If

            For i As Integer = 1 To dimensionNames.Length
                oSB.AppendLine("<row id=""" & i.ToString() & """>")
                oSB.AppendLine(ConCat_pv("0", dimensionNames(i - 1)))
                oSB.AppendLine(ConCat_pv("4", dimensionValues(i - 1) / 1000))
                oSB.AppendLine("</row>")
            Next

            oSB.AppendLine("</p3>")
        End If

        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteNonlinearFunctionHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Type"))
        oSB.AppendLine(ConCat_ht("2", "Positive end"))
        oSB.AppendLine(ConCat_ht("3", "Negative end"))
        oSB.AppendLine(ConCat_ht("4", "u / F"))
        oSB.AppendLine(ConCat_ht("5", "fi / M"))
        oSB.AppendLine(ConCat_ht("6", "u / F"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteNonlinearFunction(ByRef oSB, i, SE_NonlinearFunctions(,)) 'write 1 nodal support to the XML stream


        oSB.AppendLine("<obj nm=""" & SE_NonlinearFunctions(i, 0) & """>")
        oSB.AppendLine(ConCat_pv("0", SE_NonlinearFunctions(i, 0)))
        Select Case (SE_NonlinearFunctions(i, 1))
            Case "Translation"
                oSB.AppendLine(ConCat_pvt("1", "0", "Translation"))
            Case "Rotation"
                oSB.AppendLine(ConCat_pvt("1", "1", "Rotation"))
            Case "Nonlinear subsoil"
                oSB.AppendLine(ConCat_pvt("1", "2", "Nonlinear subsoil"))
            Case Else
                oSB.AppendLine(ConCat_pvt("1", "0", "Translation"))
        End Select

        Select Case (SE_NonlinearFunctions(i, 3))
            Case "Rigid"
                oSB.AppendLine(ConCat_pvt("2", "0", "Rigid"))
            Case "Free"
                oSB.AppendLine(ConCat_pvt("2", "1", "Free"))
            Case "Flexible"
                oSB.AppendLine(ConCat_pvt("2", "2", "Flexible"))
            Case Else
                oSB.AppendLine(ConCat_pvt("2", "0", "Rigid"))
        End Select

        Select Case (SE_NonlinearFunctions(i, 3))
            Case "Rigid"
                oSB.AppendLine(ConCat_pvt("3", "0", "Rigid"))
            Case "Free"
                oSB.AppendLine(ConCat_pvt("3", "1", "Free"))
            Case "Flexible"
                oSB.AppendLine(ConCat_pvt("3", "2", "Flexible"))
            Case Else
                oSB.AppendLine(ConCat_pvt("3", "0", "Rigid"))
        End Select

        Dim parts As String() = SE_NonlinearFunctions(i, 4).Split(New Char() {"|"c})

        Dim x As String
        Dim y As String
        Dim j As Long = 0
        Select Case (SE_NonlinearFunctions(i, 1))
            Case "Translation"
                For Each item In parts
                    x = item.Split(";")(0)
                    y = item.Split(";")(1)
                    oSB.AppendLine(ConCat_pv1v2x("4", x, y, j))
                    j += 1
                Next item
            Case "Rotation"
                For Each item In parts
                    x = item.Split(";")(0)
                    y = item.Split(";")(1)
                    oSB.AppendLine(ConCat_pv1v2x("5", x, y, j))
                    j += 1
                Next item
            Case "Nonlinear subsoil"
                For Each item In parts
                    x = item.Split(";")(0)
                    y = item.Split(";")(1)
                    oSB.AppendLine(ConCat_pv1v2x("6", x, y, j))
                    j += 1
                Next item

        End Select

        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteNodeSupportHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Reference Table"))
        'oSB.AppendLine(ConCat_hh("1", "Node"))
        oSB.AppendLine(ConCat_ht("2", "X"))
        oSB.AppendLine(ConCat_ht("3", "Y"))
        oSB.AppendLine(ConCat_ht("4", "Z"))
        oSB.AppendLine(ConCat_ht("5", "Rx"))
        oSB.AppendLine(ConCat_ht("6", "Ry"))
        oSB.AppendLine(ConCat_ht("7", "Rz"))
        oSB.AppendLine(ConCat_ht("8", "Stiffness X"))
        oSB.AppendLine(ConCat_ht("9", "Stiffness Y"))
        oSB.AppendLine(ConCat_ht("10", "Stiffness Z"))
        oSB.AppendLine(ConCat_ht("11", "Stiffness Rx"))
        oSB.AppendLine(ConCat_ht("12", "Stiffness Ry"))
        oSB.AppendLine(ConCat_ht("13", "Stiffness Rz"))
        oSB.AppendLine(ConCat_ht("14", "Angle [deg]"))
        oSB.AppendLine(ConCat_ht("15", "Function X"))
        oSB.AppendLine(ConCat_ht("16", "Function Y"))
        oSB.AppendLine(ConCat_ht("17", "Function Z"))
        oSB.AppendLine(ConCat_ht("18", "Function Rx"))
        oSB.AppendLine(ConCat_ht("19", "Function Ry"))
        oSB.AppendLine(ConCat_ht("20", "Function Rz"))
        oSB.AppendLine(ConCat_ht("21", "System"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteNodeSupport(ByRef oSB As Text.StringBuilder, isupport As Integer, modelData As ModelData) 'write 1 nodal support to the XML stream
        Dim supports = modelData.NodeSupports

        oSB.AppendLine("<obj nm=""" & supports(isupport, 1) & """>")
        oSB.AppendLine(ConCat_pv("0", supports(isupport, 1))) 'Support name
        'write beam name as reference table
        oSB.AppendLine("<p1 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Node)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Node)))
        oSB.AppendLine(ConCat_pv("2", modelData.GetNodeName(supports(isupport, 0))))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p1>")
        'End Of reference table

        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomForTranslation)(2, supports(isupport, 2)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomForTranslation)(3, supports(isupport, 3)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomForTranslation)(4, supports(isupport, 4)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomSupport)(5, supports(isupport, 5)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomSupport)(6, supports(isupport, 6)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomSupport)(7, supports(isupport, 7)))

        oSB.AppendLine(ConCat_pv("8", supports(isupport, 8)))
        oSB.AppendLine(ConCat_pv("9", supports(isupport, 9)))
        oSB.AppendLine(ConCat_pv("10", supports(isupport, 10)))
        oSB.AppendLine(ConCat_pv("11", supports(isupport, 11)))
        oSB.AppendLine(ConCat_pv("12", supports(isupport, 12)))
        oSB.AppendLine(ConCat_pv("13", supports(isupport, 13)))
        'Angle
        oSB.AppendLine(ConCat_pv("14", supports(isupport, 21)))
        '15-20 NL functions
        oSB.AppendLine(ConCat_pin("15", "1", supports(isupport, 14)))
        oSB.AppendLine(ConCat_pin("16", "1", supports(isupport, 15)))
        oSB.AppendLine(ConCat_pin("17", "1", supports(isupport, 16)))
        oSB.AppendLine(ConCat_pin("18", "1", supports(isupport, 17)))
        oSB.AppendLine(ConCat_pin("19", "1", supports(isupport, 18)))
        oSB.AppendLine(ConCat_pin("20", "1", supports(isupport, 19)))
        'System
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.CoordSystemNodeSupport)(21, supports(isupport, 20)))
        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteEdgeSupportHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Reference table"))
        oSB.AppendLine(ConCat_ht("2", "Edge"))
        oSB.AppendLine(ConCat_ht("3", "X"))
        oSB.AppendLine(ConCat_ht("4", "Y"))
        oSB.AppendLine(ConCat_ht("5", "Z"))
        oSB.AppendLine(ConCat_ht("6", "Rx"))
        oSB.AppendLine(ConCat_ht("7", "Ry"))
        oSB.AppendLine(ConCat_ht("8", "Rz"))
        oSB.AppendLine(ConCat_ht("9", "Stiffness X"))
        oSB.AppendLine(ConCat_ht("10", "Stiffness Y"))
        oSB.AppendLine(ConCat_ht("11", "Stiffness Z"))
        oSB.AppendLine(ConCat_ht("12", "Stiffness Rx"))
        oSB.AppendLine(ConCat_ht("13", "Stiffness Ry"))
        oSB.AppendLine(ConCat_ht("14", "Stiffness Rz"))
        oSB.AppendLine(ConCat_ht("15", "System"))
        oSB.AppendLine(ConCat_ht("16", "Coord. definition"))
        oSB.AppendLine(ConCat_ht("17", "Position x1"))
        oSB.AppendLine(ConCat_ht("18", "Position x2"))
        oSB.AppendLine(ConCat_ht("19", "Origin"))
        oSB.AppendLine(ConCat_ht("20", "Function X"))
        oSB.AppendLine(ConCat_ht("21", "Function Y"))
        oSB.AppendLine(ConCat_ht("22", "Function Z"))
        oSB.AppendLine(ConCat_ht("23", "Function Rx"))
        oSB.AppendLine(ConCat_ht("24", "Function Ry"))
        oSB.AppendLine(ConCat_ht("25", "Function Rz"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteEdgeSupport(ByRef oSB As Text.StringBuilder, isupport As Integer, modelData As ModelData) 'write 1 edge support to the XML stream
        Dim supports = modelData.SurfaceEdgeSupports
        oSB.AppendLine("<obj nm=""" & supports(isupport, 3) & """>")
        oSB.AppendLine(ConCat_pv("0", supports(isupport, 3))) 'Support name

        'write surface name as reference table
        oSB.AppendLine("<p1 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")

        Dim objType As EsaObjectType = EsaObjectType.Member2D

        If Not modelData.TryGetEdgeMemberType(supports(isupport, 0), objType) Then
            'different reference depending whether it's towards a surface or an opening
            Select Case supports(isupport, 1)
                Case "SURFACE"
                    objType = EsaObjectType.Member2D
                Case "OPENING"
                    objType = EsaObjectType.Opening
                Case "INTERNAL EDGE"
                    objType = EsaObjectType.InternalEdge2D
                Case Else
                    objType = EsaObjectType.Member2D
            End Select
        End If

        oSB.AppendLine(ConCat_pv("0", ContainerIds(objType)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(objType)))
        oSB.AppendLine(ConCat_pv("2", supports(isupport, 0)))

        oSB.AppendLine("</row>")
        oSB.AppendLine("</p1>")
        'end of reference table

        oSB.AppendLine(ConCat_pvt("2", CStr(CLng(supports(isupport, 2)) - 1), supports(isupport, 2))) 'Edge number minus 1 is the index

        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomForTranslation)(3, supports(isupport, 4)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomForTranslation)(4, supports(isupport, 5)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomForTranslation)(5, supports(isupport, 6)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomSupport)(6, supports(isupport, 7)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomSupport)(7, supports(isupport, 8)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomSupport)(8, supports(isupport, 9)))

        oSB.AppendLine(ConCat_pv("9", supports(isupport, 10)))
        oSB.AppendLine(ConCat_pv("10", supports(isupport, 11)))
        oSB.AppendLine(ConCat_pv("11", supports(isupport, 12)))
        oSB.AppendLine(ConCat_pv("12", supports(isupport, 13)))
        oSB.AppendLine(ConCat_pv("13", supports(isupport, 14)))
        oSB.AppendLine(ConCat_pv("14", supports(isupport, 15)))

        oSB.AppendLine(ConCat_pvt_enum(Of Koala.CoordSystem)(15, supports(isupport, 22)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.CoordinateDefinition)(16, supports(isupport, 23)))
        oSB.AppendLine(ConCat_pv("17", supports(isupport, 24)))
        oSB.AppendLine(ConCat_pv("18", supports(isupport, 25)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.Origin)(19, supports(isupport, 26)))

        oSB.AppendLine(ConCat_pin("20", "1", supports(isupport, 16)))
        oSB.AppendLine(ConCat_pin("21", "1", supports(isupport, 17)))
        oSB.AppendLine(ConCat_pin("22", "1", supports(isupport, 18)))
        oSB.AppendLine(ConCat_pin("23", "1", supports(isupport, 19)))
        oSB.AppendLine(ConCat_pin("24", "1", supports(isupport, 20)))
        oSB.AppendLine(ConCat_pin("25", "1", supports(isupport, 21)))

        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteBeamLineSupportHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Reference table"))
        oSB.AppendLine(ConCat_ht("2", "Type"))
        oSB.AppendLine(ConCat_ht("3", "X"))
        oSB.AppendLine(ConCat_ht("4", "Y"))
        oSB.AppendLine(ConCat_ht("5", "Z"))
        oSB.AppendLine(ConCat_ht("6", "Rx"))
        oSB.AppendLine(ConCat_ht("7", "Ry"))
        oSB.AppendLine(ConCat_ht("8", "Rz"))
        oSB.AppendLine(ConCat_ht("9", "Stiffness X"))
        oSB.AppendLine(ConCat_ht("10", "Stiffness Y"))
        oSB.AppendLine(ConCat_ht("11", "Stiffness Z"))
        oSB.AppendLine(ConCat_ht("12", "Stiffness Rx"))
        oSB.AppendLine(ConCat_ht("13", "Stiffness Ry"))
        oSB.AppendLine(ConCat_ht("14", "Stiffness Rz"))
        oSB.AppendLine(ConCat_ht("15", "System"))
        oSB.AppendLine(ConCat_ht("16", "Coord. definition"))
        oSB.AppendLine(ConCat_ht("17", "Position x1"))
        oSB.AppendLine(ConCat_ht("18", "Position x2"))
        oSB.AppendLine(ConCat_ht("19", "Origin"))
        oSB.AppendLine(ConCat_ht("20", "Function X"))
        oSB.AppendLine(ConCat_ht("21", "Function Y"))
        oSB.AppendLine(ConCat_ht("22", "Function Z"))
        oSB.AppendLine(ConCat_ht("23", "Function Rx"))
        oSB.AppendLine(ConCat_ht("24", "Function Ry"))
        oSB.AppendLine(ConCat_ht("25", "Function Rz"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteBeamLineSupport(ByRef oSB, isupport, supports(,)) 'write 1 edge support to the XML stream
        oSB.AppendLine("<obj nm=""" & supports(isupport, 1) & """>")
        oSB.AppendLine(ConCat_pv("0", supports(isupport, 1))) 'Support name

        'write beam name as reference table
        oSB.AppendLine("<p1 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("2", supports(isupport, 0)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p1>")
        'end of reference table

        'support type
        oSB.AppendLine(ConCat_pvt("2", 0, "Line"))

        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomForTranslation)(3, supports(isupport, 2)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomForTranslation)(4, supports(isupport, 3)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomForTranslation)(5, supports(isupport, 4)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomSupport)(6, supports(isupport, 5)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomSupport)(7, supports(isupport, 6)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomSupport)(8, supports(isupport, 7)))

        oSB.AppendLine(ConCat_pv("9", supports(isupport, 8)))
        oSB.AppendLine(ConCat_pv("10", supports(isupport, 9)))
        oSB.AppendLine(ConCat_pv("11", supports(isupport, 10)))
        oSB.AppendLine(ConCat_pv("12", supports(isupport, 11)))
        oSB.AppendLine(ConCat_pv("13", supports(isupport, 12)))
        oSB.AppendLine(ConCat_pv("14", supports(isupport, 13)))

        oSB.AppendLine(ConCat_pvt_enum(Of Koala.CoordSystem)(15, supports(isupport, 20)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.CoordinateDefinition)(16, supports(isupport, 21)))
        oSB.AppendLine(ConCat_pv("17", supports(isupport, 22)))
        oSB.AppendLine(ConCat_pv("18", supports(isupport, 23)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.Origin)(19, supports(isupport, 24)))

        oSB.AppendLine(ConCat_pin("20", "1", supports(isupport, 14)))
        oSB.AppendLine(ConCat_pin("21", "1", supports(isupport, 15)))
        oSB.AppendLine(ConCat_pin("22", "1", supports(isupport, 16)))
        oSB.AppendLine(ConCat_pin("23", "1", supports(isupport, 17)))
        oSB.AppendLine(ConCat_pin("24", "1", supports(isupport, 18)))
        oSB.AppendLine(ConCat_pin("25", "1", supports(isupport, 19)))
        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteBeamPointSupportHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Reference table"))
        oSB.AppendLine(ConCat_ht("2", "Type"))
        oSB.AppendLine(ConCat_ht("3", "X"))
        oSB.AppendLine(ConCat_ht("4", "Y"))
        oSB.AppendLine(ConCat_ht("5", "Z"))
        oSB.AppendLine(ConCat_ht("6", "Rx"))
        oSB.AppendLine(ConCat_ht("7", "Ry"))
        oSB.AppendLine(ConCat_ht("8", "Rz"))
        oSB.AppendLine(ConCat_ht("9", "Stiffness X"))
        oSB.AppendLine(ConCat_ht("10", "Stiffness Y"))
        oSB.AppendLine(ConCat_ht("11", "Stiffness Z"))
        oSB.AppendLine(ConCat_ht("12", "Stiffness Rx"))
        oSB.AppendLine(ConCat_ht("13", "Stiffness Ry"))
        oSB.AppendLine(ConCat_ht("14", "Stiffness Rz"))
        oSB.AppendLine(ConCat_ht("15", "System"))
        oSB.AppendLine(ConCat_ht("16", "Coord. definition"))
        oSB.AppendLine(ConCat_ht("17", "Position x"))
        oSB.AppendLine(ConCat_ht("18", "Origin"))
        oSB.AppendLine(ConCat_ht("19", "Repeat (n)"))
        oSB.AppendLine(ConCat_ht("20", "Delta x"))
        oSB.AppendLine(ConCat_ht("21", "Function X"))
        oSB.AppendLine(ConCat_ht("22", "Function Y"))
        oSB.AppendLine(ConCat_ht("23", "Function Z"))
        oSB.AppendLine(ConCat_ht("24", "Function Rx"))
        oSB.AppendLine(ConCat_ht("25", "Function Ry"))
        oSB.AppendLine(ConCat_ht("26", "Function Rz"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteBeamPointSupport(ByRef oSB, isupport, supports(,)) 'write 1 edge support to the XML stream
        oSB.AppendLine("<obj nm=""" & supports(isupport, 1) & """>")
        oSB.AppendLine(ConCat_pv("0", supports(isupport, 1))) 'Support name

        'write beam name as reference table
        oSB.AppendLine("<p1 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("2", supports(isupport, 0)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p1>")
        'end of reference table

        'support type
        oSB.AppendLine(ConCat_pvt("2", 0, "Standard"))

        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomForTranslation)(3, supports(isupport, 2)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomForTranslation)(4, supports(isupport, 3)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomForTranslation)(5, supports(isupport, 4)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomSupport)(6, supports(isupport, 5)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomSupport)(7, supports(isupport, 6)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomSupport)(8, supports(isupport, 7)))

        oSB.AppendLine(ConCat_pv("9", supports(isupport, 8)))
        oSB.AppendLine(ConCat_pv("10", supports(isupport, 9)))
        oSB.AppendLine(ConCat_pv("11", supports(isupport, 10)))
        oSB.AppendLine(ConCat_pv("12", supports(isupport, 11)))
        oSB.AppendLine(ConCat_pv("13", supports(isupport, 12)))
        oSB.AppendLine(ConCat_pv("14", supports(isupport, 13)))

        oSB.AppendLine(ConCat_pvt_enum(Of Koala.CoordSystem)(15, supports(isupport, 20)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.CoordinateDefinition)(16, supports(isupport, 21)))
        oSB.AppendLine(ConCat_pv("17", supports(isupport, 22)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.Origin)(18, supports(isupport, 23)))
        oSB.AppendLine(ConCat_pv("19", supports(isupport, 24)))
        oSB.AppendLine(ConCat_pv("20", supports(isupport, 25)))

        oSB.AppendLine(ConCat_pin("21", "1", supports(isupport, 14)))
        oSB.AppendLine(ConCat_pin("22", "1", supports(isupport, 15)))
        oSB.AppendLine(ConCat_pin("23", "1", supports(isupport, 16)))
        oSB.AppendLine(ConCat_pin("24", "1", supports(isupport, 17)))
        oSB.AppendLine(ConCat_pin("25", "1", supports(isupport, 18)))
        oSB.AppendLine(ConCat_pin("26", "1", supports(isupport, 19)))

        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteSubsoilHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Decription"))
        oSB.AppendLine(ConCat_ht("2", "C1x"))
        oSB.AppendLine(ConCat_ht("3", "C1y"))
        oSB.AppendLine(ConCat_ht("4", "C1z"))
        oSB.AppendLine(ConCat_ht("5", "Stiffness"))
        oSB.AppendLine(ConCat_ht("6", "C2x"))
        oSB.AppendLine(ConCat_ht("7", "C2y"))
        oSB.AppendLine(ConCat_ht("8", "Nonlinear function C1z"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteSubsoil(ByRef oSB, i, subsoil(,)) 'write 1 edge support to the XML stream

        oSB.AppendLine("<obj nm=""" & subsoil(i, 0) & """>")
        oSB.AppendLine(ConCat_pv("0", subsoil(i, 0)))
        oSB.AppendLine(ConCat_pv("1", subsoil(i, 1)))
        oSB.AppendLine(ConCat_pv("2", subsoil(i, 2)))
        oSB.AppendLine(ConCat_pv("3", subsoil(i, 3)))

        Select Case subsoil(i, 7)
            Case "Flexible"
                oSB.AppendLine(ConCat_pvt("4", "0", "Flexible"))
            Case "Nonlinear function"
                oSB.AppendLine(ConCat_pvt("4", "1", "Nonlinear Function"))
            Case Else
                oSB.AppendLine(ConCat_pvt("4", "0", "Flexible"))
        End Select

        oSB.AppendLine(ConCat_pv("5", subsoil(i, 4)))
        oSB.AppendLine(ConCat_pv("6", subsoil(i, 5)))
        oSB.AppendLine(ConCat_pv("7", subsoil(i, 6)))
        ' Subsoil NLinear function definition requires the NLinear function to be defined FIRST in the XML or esa project!
        oSB.AppendLine(ConCat_pin("8", "1", subsoil(i, 8)))
        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteSurfaceSupportHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Reference Table"))
        oSB.AppendLine(ConCat_ht("2", "Type"))
        oSB.AppendLine(ConCat_ht("3", "Subsoil"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteSurfaceSupport(ByRef oSB, i, surfacesupport(,))

        oSB.AppendLine("<obj nm=""" & surfacesupport(i, 1) & """>")
        oSB.AppendLine(ConCat_pv("0", surfacesupport(i, 1)))

        'write beam name as reference table
        oSB.AppendLine("<p1 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member2D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member2D)))
        oSB.AppendLine(ConCat_pv("2", surfacesupport(i, 0)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p1>")
        'end of reference table
        oSB.AppendLine(ConCat_pvt("2", "0", "Individual"))
        oSB.AppendLine(ConCat_pn("3", surfacesupport(i, 2)))
        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteLoadGroupHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Load")) '0: Permanent, 1: Variable
        oSB.AppendLine(ConCat_ht("2", "Relation"))
        oSB.AppendLine(ConCat_ht("3", "Type"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteLoadGroup(ByRef oSB, igroup, groups(,)) 'write 1 load group to the XML stream
        oSB.AppendLine("<obj id=""" & Trim(Str(igroup)) & """ nm=""" & groups(igroup, 0) & """>")
        oSB.AppendLine(ConCat_pv("0", groups(igroup, 0)))

        Dim loadTypeDescr = CStr(groups(igroup, 1))
        Dim loadTypeParts = loadTypeDescr.Split("|")

        Dim loadType As LoadGroupLoadType = Koala.GetEnum(Of LoadGroupLoadType)(loadTypeParts(0))
        oSB.AppendLine(ConCat_pvt("1", Convert.ToInt32(loadType), Koala.GetEnumDescription(loadType)))

        If loadType = LoadGroupLoadType.Variable Then
            oSB.AppendLine(ConCat_pvt_enum(Of Koala.LoadGroupRelation)(2, groups(igroup, 2)))

            If loadTypeParts.Length > 1 Then
                Dim variableLoadType As Koala.LoadGroupVariableLoadType = Koala.GetEnum(Of LoadGroupVariableLoadType)(loadTypeParts(1))
                oSB.AppendLine(ConCat_pvt("3", Convert.ToInt32(variableLoadType), Koala.GetEnumDescription(variableLoadType)))
            End If

        ElseIf loadType = LoadGroupLoadType.Seismic Then
            Dim relation As Koala.LoadGroupRelation = Koala.GetEnum(Of LoadGroupRelation)(groups(igroup, 2))

            If relation = LoadGroupRelation.Exclusive Or relation = LoadGroupRelation.Together Then
                oSB.AppendLine(ConCat_pvt("2", Convert.ToInt32(relation), Koala.GetEnumDescription(relation)))
            End If
        End If

        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteLinCombinationHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Type"))
        oSB.AppendLine(ConCat_ht("2", " Load cases"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteLinCombination(ByRef oSB, icombi, combinations(,)) 'write 1 combination to the XML stream

        oSB.AppendLine("<obj id=""" & Trim(Str(icombi)) & """ nm=""" & combinations(icombi, 0) & """>")
        oSB.AppendLine(ConCat_pv("0", combinations(icombi, 0)))
        Select Case combinations(icombi, 1)
            Case "Envelope - ultimate"
                oSB.AppendLine(ConCat_pvt("1", "0", "Envelope - ultimate"))
            Case "Envelope - serviceability"
                oSB.AppendLine(ConCat_pvt("1", "1", "Envelope - serviceability"))
            Case "Linear - ultimate"
                oSB.AppendLine(ConCat_pvt("1", "2", "Linear - ultimate"))
            Case "Linear - serviceability"
                oSB.AppendLine(ConCat_pvt("1", "3", "Linear - serviceability"))
            Case "EN-ULS (STR/GEO) Set B"
                oSB.AppendLine(ConCat_pvt("1", "4", "EN-ULS (STR/GEO) Set B"))
            Case "EN-Accidental 1"
                oSB.AppendLine(ConCat_pvt("1", "5", "EN-Accidental 1"))
            Case "EN-Accidental 2"
                oSB.AppendLine(ConCat_pvt("1", "6", "EN-Accidental 2"))
            Case "EN-Seismic"
                oSB.AppendLine(ConCat_pvt("1", "7", "EN-Seismic"))
            Case "EN-SLS Characteristic"
                oSB.AppendLine(ConCat_pvt("1", "8", "EN-SLS Characteristic"))
            Case "EN-SLS Frequent"
                oSB.AppendLine(ConCat_pvt("1", "9", "EN-SLS Frequent"))
            Case "EN-SLS Quasi-permanent"
                oSB.AppendLine(ConCat_pvt("1", "10", "EN-SLS Quasi-permanent"))
            Case "EN-ULS (STR/GEO) Set C"
                oSB.AppendLine(ConCat_pvt("1", "11", "EN-ULS (STR/GEO) Set C"))
        End Select
        Dim parts As String() = combinations(icombi, 2).Split(New Char() {";"c})
        Dim Name As String
        Dim Coeff As String
        Dim i As Long = 0
        oSB.AppendLine(ConCat_opentable("2", ""))
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Coeff."))
        oSB.AppendLine(ConCat_ht("2", "Load case ID"))
        oSB.AppendLine("</h>")
        For Each item In parts
            Coeff = item.Split("*")(0)
            Name = item.Split("*")(1)
            oSB.AppendLine(ConCat_row(i))
            oSB.AppendLine(ConCat_pv("0", Name))
            oSB.appendline(ConCat_pv("1", Coeff))
            oSB.appendline(ConCat_pin("2", i + 1, Name))
            oSB.AppendLine("</row>")
            i += 1
        Next item
        oSB.AppendLine(ConCat_closetable("2"))
        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteNonLinCombinationHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Type"))
        oSB.AppendLine(ConCat_ht("2", " Load cases"))
        oSB.AppendLine(ConCat_ht("3", "Description"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteNonLinCombination(ByRef oSB, icombi, combinations(,)) 'write 1 nonlinear combination to the XML stream

        oSB.AppendLine("<obj id=""" & Trim(Str(icombi)) & """ nm=""" & combinations(icombi, 0) & """>")
        oSB.AppendLine(ConCat_pv("0", combinations(icombi, 0)))
        Select Case combinations(icombi, 1)
            Case "Ultimate"
                oSB.AppendLine(ConCat_pvt("1", "0", "Ultimate"))
            Case "Serviceability"
                oSB.AppendLine(ConCat_pvt("1", "1", "Serviceability"))
        End Select
        Dim parts As String() = combinations(icombi, 2).Split(New Char() {";"c})
        Dim Name As String
        Dim Coeff As String
        Dim i As Long = 0
        oSB.AppendLine(ConCat_opentable("2", ""))
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Coeff."))
        oSB.AppendLine(ConCat_ht("2", "Load case ID"))
        'description
        oSB.AppendLine("</h>")
        For Each item In parts
            Coeff = item.Split("*")(0)
            Name = item.Split("*")(1)
            oSB.AppendLine(ConCat_row(i))
            oSB.AppendLine(ConCat_pv("0", Name))
            oSB.appendline(ConCat_pv("1", Coeff))
            oSB.appendline(ConCat_pin("2", i + 1, Name))
            oSB.AppendLine("</row>")
            i += 1
        Next item
        oSB.AppendLine(ConCat_closetable("2"))
        oSB.AppendLine(ConCat_pv("3", combinations(icombi, 3)))
        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteStabilityCombinationHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", " Load cases"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteResultClassHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.Appendline(ConCat_ht("0", "Name"))
        oSB.Appendline(ConCat_ht("1", "Case"))
        oSB.Appendline(ConCat_ht("2", "AllType"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteStabilityCombination(ByRef oSB, icombi, combinations(,)) 'write 1 nonlinear combination to the XML stream
        oSB.AppendLine("<obj id=""" & Trim(Str(icombi)) & """ nm=""" & combinations(icombi, 0) & """>")
        oSB.AppendLine(ConCat_pv("0", combinations(icombi, 0)))
        Dim parts As String() = combinations(icombi, 1).Split(New Char() {";"c})
        Dim Name As String
        Dim Coeff As String
        Dim i As Long = 0
        oSB.AppendLine(ConCat_opentable("1", ""))
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Coeff."))
        oSB.AppendLine(ConCat_ht("2", "Load case ID"))
        oSB.AppendLine("</h>")
        For Each item In parts
            Coeff = item.Split("*")(0)
            Name = item.Split("*")(1)
            oSB.AppendLine(ConCat_row(i))
            oSB.AppendLine(ConCat_pv("0", Name))
            oSB.appendline(ConCat_pv("1", Coeff))
            oSB.appendline(ConCat_pin("2", i + 1, Name))
            oSB.AppendLine("</row>")
            i += 1
        Next item
        oSB.AppendLine(ConCat_closetable("1"))
        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteResultclass(ByRef oSB, iclass, classes(,)) 'write 1 result class to the XML stream
        oSB.AppendLine("<obj id=""" & Trim(Str(iclass)) & """ nm=""" & classes(iclass, 0) & """>")
        oSB.AppendLine(ConCat_pv("0", classes(iclass, 0)))

        Dim names As String() = classes(iclass, 1).Split(New Char() {";"c})
        Dim types As String() = classes(iclass, 2).Split(New Char() {";"c})
        Dim subtypes As String() = classes(iclass, 3).Split(New Char() {";"c})
        Dim i As Integer = 0
        Dim p_id As Integer = 0

        oSB.AppendLine(ConCat_opentable("1", ""))

        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Load case ID"))
        oSB.AppendLine(ConCat_ht("1", "Load combi ID"))
        'oSB.AppendLine(ConCat_ht("2", "Concrete combi ID"))
        oSB.AppendLine(ConCat_ht("2", "Nonlinear combi ID"))
        'oSB.AppendLine(ConCat_ht("4", "Mass combi ID"))
        'oSB.AppendLine(ConCat_ht("5", "Stability combi ID"))
        'oSB.AppendLine(ConCat_ht("6", "Case_InxLc"))
        oSB.AppendLine(ConCat_ht("3", "Case_Type"))
        'oSB.AppendLine(ConCat_ht("8", "Case_Model"))
        oSB.AppendLine(ConCat_ht("4", "Case_tUS"))
        oSB.AppendLine("</h>")

        For Each item In names
            oSB.AppendLine(ConCat_row(i))
            If types(i) = "1001" Then 'loadcases and linear combinations
                If subtypes(i) = "-1" Then
                    p_id = 0 'loadcases
                ElseIf subtypes(i) = "0" Or subtypes(i) = 1 Then
                    p_id = 1 'linear combinations
                End If
            ElseIf types(i) = "1002" Then
                p_id = 2 'nonlinear combinations
            End If

            oSB.AppendLine(ConCat_pn(p_id, item))
            oSB.AppendLine(ConCat_pv("3", types(i)))
            oSB.AppendLine(ConCat_pv("4", subtypes(i)))
            oSB.AppendLine("</row>")
            i += 1
        Next

        oSB.AppendLine(ConCat_closetable("1"))

        Dim all_type_names() As String = {"Load case",
                                          "Ultimate combination",
                                          "Serviceability combination",
                                          "Nonlinear ultimate combination",
                                          "Nonlinear serviceability combination"}

        Dim all_type_types() As String = {"1001",
                                          "1001",
                                          "1001",
                                          "1002",
                                          "1002"}

        Dim all_type_tus() As String = {"-1",
                                        "0",
                                        "1",
                                        "0",
                                        "1"}

        Dim all_type_filter() As String = {"0",
                                           "0",
                                           "0",
                                           "1",
                                           "2"}

        oSB.AppendLine(ConCat_opentable("2", ""))
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "All Type Name"))
        oSB.AppendLine(ConCat_ht("2", "All Type All"))
        oSB.AppendLine(ConCat_ht("3", "All Type Type"))
        oSB.AppendLine(ConCat_ht("4", "All Type tUS"))
        oSB.AppendLine(ConCat_ht("5", "All Type FilterType1"))
        oSB.AppendLine(ConCat_ht("6", "All Type FilterType2"))
        oSB.AppendLine(ConCat_ht("7", "All Type Size"))
        oSB.AppendLine("</h>")

        For i = 0 To all_type_names.Length - 1
            oSB.AppendLine(ConCat_row(i))
            oSB.AppendLine(ConCat_pv("0", all_type_names(i)))
            oSB.AppendLine(ConCat_pv("2", "0"))
            oSB.AppendLine(ConCat_pv("3", all_type_types(i)))
            oSB.AppendLine(ConCat_pv("4", all_type_tus(i)))
            oSB.AppendLine(ConCat_pv("5", all_type_filter(i)))
            oSB.AppendLine(ConCat_pv("6", "0"))
            oSB.AppendLine("</row>")
        Next

        oSB.AppendLine(ConCat_closetable("2"))

        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteLoadCaseHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Action type")) '0: Permanent, 1: Variable
        oSB.AppendLine(ConCat_ht("2", "Load type")) '0: Self-weight, 1: Standard, 2: Primary
        oSB.AppendLine(ConCat_ht("3", "Direction")) '0: -Z, 1: +Z, 2: -Y etc.
        oSB.AppendLine(ConCat_ht("4", "Load group"))
        oSB.AppendLine(ConCat_ht("5", "Specification"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteLoadCase(ByRef oSB, icase, cases(,)) 'write 1 load case to the XML stream

        oSB.AppendLine("<obj id=""" & Trim(Str(icase)) & """ nm=""" & cases(icase, 0) & """>")
        oSB.AppendLine(ConCat_pv("0", cases(icase, 0)))

        Dim lctype As Koala.LoadCaseType = Koala.GetEnum(Of LoadCaseType)(cases(icase, 1))
        Select Case lctype
            Case Koala.LoadCaseType.SW
                oSB.AppendLine(ConCat_pvt("1", "0", "Permanent"))
                oSB.AppendLine(ConCat_pvt("2", "0", "Self weight"))
                oSB.AppendLine(ConCat_pvt("3", "0", "-Z"))

            Case Koala.LoadCaseType.Permanent
                oSB.AppendLine(ConCat_pvt("1", "0", "Permanent"))
                oSB.AppendLine(ConCat_pvt("2", "1", "Standard"))

            Case Koala.LoadCaseType.Variable
                oSB.AppendLine(ConCat_pvt("1", "1", "Variable"))
                oSB.AppendLine(ConCat_pvt("2", "0", "Static"))

            Case Koala.LoadCaseType.Seismic
                oSB.AppendLine(ConCat_pvt("1", "1", "Variable"))
                oSB.AppendLine(ConCat_pvt("2", "1", "Dynamic"))
                'Parameters to add for earthquake load cases :
                'Specification 'seismicity'
                'Mass Combination to consider
                'Response spectrum
                'Factor X
                'Factor Y
                'Factor Z
                'acceleration factor (default 1)
                'overturning reference level (default 0)
                'method (4 options)
                'Superposition type
                'Required total mass ratio
                'Required minimall mass ratio
                'Use dominant mode (no=0, yes=1)

        End Select

        oSB.AppendLine(ConCat_pn("4", cases(icase, 2)))

        If lctype = Koala.LoadCaseType.Seismic Then
            oSB.AppendLine(ConCat_pvt("5", "100", "Seismicity"))
        End If

        oSB.AppendLine("</obj>")

    End Sub
    Private Sub WriteMassGroupHeader(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Load case"))
        oSB.AppendLine(ConCat_ht("2", "Keep masses up-to-date-with loads"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteMassGroup(ByRef oSB, igroup, groups(,)) 'write 1 mass group to the XML stream

        oSB.AppendLine("<obj id=""" & Trim(Str(igroup)) & """ nm=""" & groups(igroup, 0) & """>")
        oSB.AppendLine(ConCat_pv("0", groups(igroup, 0)))
        oSB.Appendline(ConCat_pn("1", groups(igroup, 1)))
        oSB.AppendLine(ConCat_pv("2", groups(igroup, 2)))
        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteMassCombinationHeader(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Load cases"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteMassCombinations(ByRef oSB, icombi, combinations(,)) 'write 1 mass combination to the XML stream
        oSB.AppendLine("<obj id=""" & Trim(Str(icombi)) & """ nm=""" & combinations(icombi, 0) & """>")
        'Write combination tables
        oSB.AppendLine(ConCat_pv("0", combinations(icombi, 0)))
        Dim parts As String() = combinations(icombi, 1).Split(New Char() {";"c})
        Dim Name As String
        Dim Coeff As String
        Dim i As Integer = 0
        oSB.AppendLine(ConCat_opentable("1", ""))
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Coeff."))
        oSB.AppendLine(ConCat_ht("2", "Load case ID"))
        oSB.AppendLine("</h>")
        For Each item In parts
            Coeff = item.Split("*")(0)
            Name = item.Split("*")(1)
            oSB.AppendLine(ConCat_row(i))
            oSB.AppendLine(ConCat_pv("0", Name))
            oSB.appendline(ConCat_pv("1", Coeff))
            oSB.appendline(ConCat_pin("2", i + 1, Name))
            oSB.AppendLine("</row>")
            i += 1
        Next item
        oSB.AppendLine(ConCat_closetable("1"))
        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteSeismicSpectrumHeader(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Freq./Period/Accel."))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteSeismicSpectrum(ByRef oSB, ispectrum, spectrums(,)) 'write 1 seismic spectrum to the XML stream
        oSB.AppendLine("<obj id=""" & Trim(Str(ispectrum)) & """ nm=""" & spectrums(ispectrum, 0) & """>")

        'Write the freq/wavelength/accel data
        oSB.AppendLine(ConCat_pv("0", spectrums(ispectrum, 0)))

        Dim freqs As String() = Split(spectrums(ispectrum, 1), ";")
        Dim wavelengths As String() = Split(spectrums(ispectrum, 2), ";")
        Dim accelerations As String() = Split(spectrums(ispectrum, 3), ";")
        Dim i As Integer = 0
        For Each item In freqs
            oSB.AppendLine(ConCat_pv1v2v3x("1", freqs(i), wavelengths(i), accelerations(i), i))
            i += 1
        Next item
        oSB.AppendLine("</obj>")
    End Sub
    Private Sub WriteLineLoadBeamHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Load case"))
        oSB.AppendLine(ConCat_ht("2", "Reference Table"))
        oSB.AppendLine(ConCat_ht("3", "Direction"))
        oSB.AppendLine(ConCat_ht("4", "Distribution"))
        oSB.AppendLine(ConCat_ht("5", "Value - P@1"))
        oSB.AppendLine(ConCat_ht("6", "Value - P@2"))
        oSB.AppendLine(ConCat_ht("7", "System"))
        oSB.AppendLine(ConCat_ht("8", "Location"))
        oSB.AppendLine(ConCat_ht("9", "Position x1"))
        oSB.AppendLine(ConCat_ht("10", "Position x2"))
        oSB.AppendLine(ConCat_ht("11", "Coord. definition"))
        oSB.AppendLine(ConCat_ht("12", "Origin"))
        oSB.AppendLine(ConCat_ht("13", "Eccentricity ey"))
        oSB.AppendLine(ConCat_ht("14", "Eccentricity ez"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteLineLoadBeam(ByRef oSB, iload, loads(,)) 'write 1 line load to the XML stream

        oSB.AppendLine("<obj id=""" & Trim(Str(iload)) & """ nm=""" & "LL" & Trim(Str(iload)) & """>")
        oSB.AppendLine(ConCat_pv("0", "LL" & Trim(Str(iload))))
        oSB.AppendLine(ConCat_pn("1", loads(iload, 0)))
        'write beam name as reference table
        oSB.AppendLine("<p2 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("2", loads(iload, 1)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p2>")
        'end of reference table

        'direction

        Select Case loads(iload, 3)
            Case "X"
                oSB.AppendLine(ConCat_pvt("3", "0", "X"))
            Case "Y"
                oSB.AppendLine(ConCat_pvt("3", "1", "Y"))
            Case "Z"
                oSB.AppendLine(ConCat_pvt("3", "2", "Z"))
        End Select
        'distribution & values
        Select Case loads(iload, 4)
            Case "Uniform"
                oSB.AppendLine(ConCat_pvt("4", "0", loads(iload, 4)))
            Case "Trapez"
                oSB.AppendLine(ConCat_pvt("4", "1", loads(iload, 4)))
            Case Else
                oSB.AppendLine(ConCat_pvt("4", "0", loads(iload, 4)))
        End Select

        'load value
        oSB.AppendLine(ConCat_pv("5", loads(iload, 5) * 1000))
        oSB.AppendLine(ConCat_pv("6", loads(iload, 6) * 1000))
        'axis definition

        Select Case loads(iload, 2)
            Case "GCS"
                oSB.AppendLine(ConCat_pvt("7", "0", "GCS"))
            Case "LCS"
                oSB.AppendLine(ConCat_pvt("7", "1", "LCS"))
        End Select
        'p8 would be the "location": length or projection (only for GCS)

        'p9 would be the starting position for the load > default = 0.0
        'p10 would be the end position for the load > default = 1.0
        oSB.AppendLine(ConCat_pv("9", loads(iload, 8)))
        oSB.AppendLine(ConCat_pv("10", loads(iload, 9)))
        'p11 would be the definition of relative or absolute coordinates
        Select Case loads(iload, 7)
            Case "Rela"
                oSB.AppendLine(ConCat_pvt("11", "1", "Rela"))
            Case "Abso"
                oSB.AppendLine(ConCat_pvt("11", "0", "Abso"))
        End Select
        'p12 would be the indication of where the coordinates start: From start or From end
        Select Case loads(iload, 10)
            Case "From start"
                oSB.AppendLine(ConCat_pvt("12", "0", "From start"))
            Case "From end"
                oSB.AppendLine(ConCat_pvt("12", "1", "From end"))
        End Select
        'p13 & p14 would be the ey and ez eccentricities for the line load
        oSB.AppendLine(ConCat_pv("13", loads(iload, 11)))
        oSB.AppendLine(ConCat_pv("14", loads(iload, 12)))
        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteBeamThermalLoadHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Load case"))
        oSB.AppendLine(ConCat_ht("2", "Reference Table"))
        oSB.AppendLine(ConCat_ht("3", "Distribution"))
        oSB.AppendLine(ConCat_ht("4", "Delta"))
        oSB.AppendLine(ConCat_ht("5", "+y - Left delta"))
        oSB.AppendLine(ConCat_ht("6", "-y - Right delta"))
        oSB.AppendLine(ConCat_ht("7", "+z - Top delta"))
        oSB.AppendLine(ConCat_ht("8", "-z - Bottom delta"))
        oSB.AppendLine(ConCat_ht("9", "Coord. definition"))
        oSB.AppendLine(ConCat_ht("10", "Position x1"))
        oSB.AppendLine(ConCat_ht("11", "Position x2"))
        oSB.AppendLine(ConCat_ht("12", "Origin"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteBeamThermalLoad(ByRef oSB, iload, loads(,)) 'write 1 line load to the XML stream

        oSB.AppendLine("<obj id=""" & Trim(Str(iload)) & """ nm=""" & "BTLB" & Trim(Str(iload)) & """>")
        oSB.AppendLine(ConCat_pv("0", "BTLB" & Trim(Str(iload))))


        'write beam name as reference table
        oSB.AppendLine("<p1 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("2", loads(iload, 1)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p1>")
        'end of reference table

        oSB.AppendLine(ConCat_pn("2", loads(iload, 0)))

        'direction

        Select Case loads(iload, 2)
            Case "Constant"
                oSB.AppendLine(ConCat_pvt("3", "0", "Constant"))
            Case "Linear"
                oSB.AppendLine(ConCat_pvt("3", "1", "Linear"))
        End Select

        oSB.AppendLine(ConCat_pv("4", loads(iload, 3)))
        oSB.AppendLine(ConCat_pv("5", loads(iload, 4)))
        oSB.AppendLine(ConCat_pv("6", loads(iload, 5)))
        oSB.AppendLine(ConCat_pv("7", loads(iload, 6)))
        oSB.AppendLine(ConCat_pv("8", loads(iload, 7)))

        Select Case loads(iload, 8)
            Case "Rela"
                oSB.AppendLine(ConCat_pvt("9", "1", "Rela"))
            Case "Abso"
                oSB.AppendLine(ConCat_pvt("9", "0", "Abso"))
        End Select
        'p12 would be the indication of where the coordinates start: From start or From end

        oSB.AppendLine(ConCat_pv("10", loads(iload, 9)))
        oSB.AppendLine(ConCat_pv("11", loads(iload, 10)))

        Select Case loads(iload, 11)
            Case "From start"
                oSB.AppendLine(ConCat_pvt("12", "0", "From start"))
            Case "From end"
                oSB.AppendLine(ConCat_pvt("12", "1", "From end"))
        End Select

        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteLineMomentLoadBeamHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Load case"))
        oSB.AppendLine(ConCat_ht("2", "Reference Table"))
        oSB.AppendLine(ConCat_ht("3", "Direction"))
        oSB.AppendLine(ConCat_ht("4", "Distribution"))
        oSB.AppendLine(ConCat_ht("5", "Value - M@1"))
        oSB.AppendLine(ConCat_ht("6", "Value - M@2"))
        oSB.AppendLine(ConCat_ht("7", "System"))
        oSB.AppendLine(ConCat_ht("8", "Location"))
        oSB.AppendLine(ConCat_ht("9", "Position x1"))
        oSB.AppendLine(ConCat_ht("10", "Position x2"))
        oSB.AppendLine(ConCat_ht("11", "Coord. definition"))
        oSB.AppendLine(ConCat_ht("12", "Origin"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteLineMomentLoadBeam(ByRef oSB, iload, loads(,)) 'write 1 line load to the XML stream

        oSB.AppendLine("<obj id=""" & Trim(Str(iload)) & """ nm=""" & "LMLB" & Trim(Str(iload)) & """>")
        oSB.AppendLine(ConCat_pv("0", "LMLB" & Trim(Str(iload))))
        oSB.AppendLine(ConCat_pn("1", loads(iload, 0)))
        'write beam name as reference table
        oSB.AppendLine("<p2 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("2", loads(iload, 1)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p2>")
        'end of reference table

        'direction

        Select Case loads(iload, 3)
            Case "Mx"
                oSB.AppendLine(ConCat_pvt("3", "0", "Mx"))
            Case "My"
                oSB.AppendLine(ConCat_pvt("3", "1", "My"))
            Case "Mz"
                oSB.AppendLine(ConCat_pvt("3", "2", "Mz"))
        End Select
        'distribution & values
        Select Case loads(iload, 4)
            Case "Uniform"
                oSB.AppendLine(ConCat_pvt("4", "0", loads(iload, 4)))
            Case "Trapez"
                oSB.AppendLine(ConCat_pvt("4", "1", loads(iload, 4)))
            Case Else
                oSB.AppendLine(ConCat_pvt("4", "0", loads(iload, 4)))
        End Select

        'load value
        oSB.AppendLine(ConCat_pv("5", loads(iload, 5) * 1000))
        oSB.AppendLine(ConCat_pv("6", loads(iload, 6) * 1000))
        'axis definition

        Select Case loads(iload, 2)
            Case "GCS"
                oSB.AppendLine(ConCat_pvt("7", "0", "GCS"))
            Case "LCS"
                oSB.AppendLine(ConCat_pvt("7", "1", "LCS"))
        End Select
        'p8 would be the "location": length or projection (only for GCS)

        'p9 would be the starting position for the load > default = 0.0
        'p10 would be the end position for the load > default = 1.0
        oSB.AppendLine(ConCat_pv("9", loads(iload, 8)))
        oSB.AppendLine(ConCat_pv("10", loads(iload, 9)))
        'p11 would be the definition of relative or absolute coordinates
        Select Case loads(iload, 7)
            Case "Rela"
                oSB.AppendLine(ConCat_pvt("11", "1", "Rela"))
            Case "Abso"
                oSB.AppendLine(ConCat_pvt("11", "0", "Abso"))
        End Select
        'p12 would be the indication of where the coordinates start: From start or From end
        Select Case loads(iload, 10)
            Case "From start"
                oSB.AppendLine(ConCat_pvt("12", "0", "From start"))
            Case "From end"
                oSB.AppendLine(ConCat_pvt("12", "1", "From end"))
        End Select

        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteEdgeLoadHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Load case"))
        oSB.AppendLine(ConCat_ht("2", "Reference Table"))
        oSB.AppendLine(ConCat_ht("3", "Direction"))
        oSB.AppendLine(ConCat_ht("4", "Distribution"))
        oSB.AppendLine(ConCat_ht("5", "Value - P@1"))
        oSB.AppendLine(ConCat_ht("6", "Value - P@2"))
        oSB.AppendLine(ConCat_ht("7", "System"))
        oSB.AppendLine(ConCat_ht("8", "Location"))
        oSB.AppendLine(ConCat_ht("9", "Position x1"))
        oSB.AppendLine(ConCat_ht("10", "Position x2"))
        oSB.AppendLine(ConCat_ht("11", "Coord. definition"))
        oSB.AppendLine(ConCat_ht("12", "Origin"))
        oSB.AppendLine(ConCat_ht("13", "Eccentricity ey"))
        oSB.AppendLine(ConCat_ht("14", "Eccentricity ez"))
        oSB.AppendLine(ConCat_ht("15", "Edge"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteEdgeLoad(ByRef oSB, iload, loads(,)) 'write 1 line load on surface ede  to the XML stream
        oSB.AppendLine("<obj id=""" & Trim(Str(iload)) & """ nm=""" & "ESL" & Trim(Str(iload)) & """>")
        oSB.AppendLine(ConCat_pv("0", "ESL" & Trim(Str(iload))))
        oSB.AppendLine(ConCat_pn("1", loads(iload, 0)))
        'write surafec name as reference table
        oSB.AppendLine("<p2 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        Select Case (loads(iload, 14).ToString().ToLower())
            Case "edge", "surface"
                oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member2D)))
                oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member2D)))
            Case "internal", "internal edge"
                oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.InternalEdge2D)))
                oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.InternalEdge2D)))
            Case "opening"
                oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Opening)))
                oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Opening)))
            Case Else
                oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member2D)))
                oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member2D)))
        End Select

        oSB.AppendLine(ConCat_pv("2", loads(iload, 1)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p2>")
        'end of reference table

        'direction
        Select Case loads(iload, 4)
            Case "X"
                oSB.AppendLine(ConCat_pvt("3", "0", "X"))
            Case "Y"
                oSB.AppendLine(ConCat_pvt("3", "1", "Y"))
            Case "Z"
                oSB.AppendLine(ConCat_pvt("3", "2", "Z"))
        End Select
        'distribution & values
        Select Case loads(iload, 5)
            Case "Uniform"
                oSB.AppendLine(ConCat_pvt("4", "0", loads(iload, 5)))
            Case "Trapez"
                oSB.AppendLine(ConCat_pvt("4", "1", loads(iload, 5)))
            Case Else
                oSB.AppendLine(ConCat_pvt("4", "0", loads(iload, 5)))
        End Select

        'load value
        oSB.AppendLine(ConCat_pv("5", loads(iload, 6) * 1000))
        oSB.AppendLine(ConCat_pv("6", loads(iload, 7) * 1000))
        'axis definition

        Select Case loads(iload, 3)
            Case "GCS - Length"
                oSB.AppendLine(ConCat_pvt("7", "0", "GCS"))
                oSB.AppendLine(ConCat_pvt("8", "0", "Length"))
            Case "GCS - Projection"
                oSB.AppendLine(ConCat_pvt("7", "0", "GCS"))
                oSB.AppendLine(ConCat_pvt("8", "1", "Projection"))
            Case "LCS"
                oSB.AppendLine(ConCat_pvt("7", "1", "LCS"))
            Case Else
                oSB.AppendLine(ConCat_pvt("7", "1", "LCS"))
        End Select

        oSB.AppendLine(ConCat_pv("9", loads(iload, 9)))
        oSB.AppendLine(ConCat_pv("10", loads(iload, 10)))
        'p11 would be the definition of relative or absolute coordinates
        Select Case loads(iload, 8)
            Case "Rela"
                oSB.AppendLine(ConCat_pvt("11", "1", "Rela"))
            Case "Abso"
                oSB.AppendLine(ConCat_pvt("11", "0", "Abso"))
        End Select
        'p12 would be the indication of where the coordinates start: From start or From end
        Select Case loads(iload, 11)
            Case "From start"
                oSB.AppendLine(ConCat_pvt("12", "0", "From start"))
            Case "From end"
                oSB.AppendLine(ConCat_pvt("12", "1", "From end"))
        End Select
        'p13 & p14 would be the ey and ez eccentricities for the line load
        oSB.AppendLine(ConCat_pv("13", loads(iload, 12)))
        oSB.AppendLine(ConCat_pv("14", loads(iload, 13)))
        'edge
        oSB.AppendLine(ConCat_pvt("15", loads(iload, 2) - 1, loads(iload, 2)))
        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteMomentLineLoadOnEdgeHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Load case"))
        oSB.AppendLine(ConCat_ht("2", "Reference Table"))
        oSB.AppendLine(ConCat_ht("3", "Direction"))
        oSB.AppendLine(ConCat_ht("4", "Distribution"))
        oSB.AppendLine(ConCat_ht("5", "Value - M@1"))
        oSB.AppendLine(ConCat_ht("6", "Value - M@2"))
        oSB.AppendLine(ConCat_ht("7", "System"))
        oSB.AppendLine(ConCat_ht("8", "Location"))
        oSB.AppendLine(ConCat_ht("9", "Position x1"))
        oSB.AppendLine(ConCat_ht("10", "Position x2"))
        oSB.AppendLine(ConCat_ht("11", "Coord. definition"))
        oSB.AppendLine(ConCat_ht("12", "Origin"))
        oSB.AppendLine(ConCat_ht("13", "Edge"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteMomentLineLoadOnEdge(ByRef oSB, iload, loads(,)) 'write 1 line load on surface ede  to the XML stream
        oSB.AppendLine("<obj id=""" & Trim(Str(iload)) & """ nm=""" & "ESLM" & Trim(Str(iload)) & """>")
        oSB.AppendLine(ConCat_pv("0", "ESLM" & Trim(Str(iload))))
        oSB.AppendLine(ConCat_pn("1", loads(iload, 0)))
        'write surafec name as reference table
        oSB.AppendLine("<p2 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        Select Case (loads(iload, 12).ToString().ToLower())
            Case "edge", "surface"
                oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member2D)))
                oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member2D)))
            Case "internal", "internal edge"
                oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.InternalEdge2D)))
                oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.InternalEdge2D)))
            Case "opening"
                oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Opening)))
                oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Opening)))
            Case Else
                oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member2D)))
                oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member2D)))
        End Select
        oSB.AppendLine(ConCat_pv("2", loads(iload, 1)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p2>")
        'end of reference table

        'direction

        Select Case loads(iload, 4)
            Case "Mx"
                oSB.AppendLine(ConCat_pvt("3", "0", "Mx"))
            Case "My"
                oSB.AppendLine(ConCat_pvt("3", "1", "My"))
            Case "Mz"
                oSB.AppendLine(ConCat_pvt("3", "2", "Mz"))
        End Select
        'distribution & values
        Select Case loads(iload, 5)
            Case "Uniform"
                oSB.AppendLine(ConCat_pvt("4", "0", loads(iload, 5)))
            Case "Trapez"
                oSB.AppendLine(ConCat_pvt("4", "1", loads(iload, 5)))
            Case Else
                oSB.AppendLine(ConCat_pvt("4", "0", loads(iload, 5)))
        End Select

        'load value
        oSB.AppendLine(ConCat_pv("5", loads(iload, 6) * 1000))
        oSB.AppendLine(ConCat_pv("6", loads(iload, 7) * 1000))
        'axis definition

        Select Case loads(iload, 3)
            Case "GCS - Length"
                oSB.AppendLine(ConCat_pvt("7", "0", "GCS"))
                oSB.AppendLine(ConCat_pvt("8", "0", "Length"))
            Case "GCS - Projection"
                oSB.AppendLine(ConCat_pvt("7", "0", "GCS"))
                oSB.AppendLine(ConCat_pvt("8", "1", "Projection"))
            Case "LCS"
                oSB.AppendLine(ConCat_pvt("7", "1", "LCS"))
            Case Else
                oSB.AppendLine(ConCat_pvt("7", "1", "LCS"))
        End Select

        oSB.AppendLine(ConCat_pv("9", loads(iload, 9)))
        oSB.AppendLine(ConCat_pv("10", loads(iload, 10)))
        'p11 would be the definition of relative or absolute coordinates
        Select Case loads(iload, 8)
            Case "Rela"
                oSB.AppendLine(ConCat_pvt("11", "1", "Rela"))
            Case "Abso"
                oSB.AppendLine(ConCat_pvt("11", "0", "Abso"))
        End Select
        'p12 would be the indication of where the coordinates start: From start or From end
        Select Case loads(iload, 11)
            Case "From start"
                oSB.AppendLine(ConCat_pvt("12", "0", "From start"))
            Case "From end"
                oSB.AppendLine(ConCat_pvt("12", "1", "From end"))
        End Select

        'edge
        oSB.AppendLine(ConCat_pvt("13", loads(iload, 2) - 1, loads(iload, 2)))
        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteSurfaceLoadHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Load case"))
        oSB.AppendLine(ConCat_ht("2", "Reference Table"))
        oSB.AppendLine(ConCat_ht("3", "Direction"))
        oSB.AppendLine(ConCat_ht("4", "Value"))
        oSB.AppendLine(ConCat_ht("5", "System"))
        oSB.AppendLine(ConCat_ht("6", "Location"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteSurfaceLoad(ByRef oSB, iload, loads(,)) 'write 1 surface load to the XML stream

        oSB.AppendLine("<obj id=""" & Trim(Str(iload)) & """ nm=""" & "SF" & Trim(Str(iload)) & """>")
        oSB.AppendLine(ConCat_pv("0", "SF" & Trim(Str(iload))))
        oSB.AppendLine(ConCat_pn("1", loads(iload, 0)))
        'write beam name as reference table
        oSB.AppendLine("<p2 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member2D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member2D)))
        oSB.AppendLine(ConCat_pv("2", loads(iload, 1)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p2>")
        'end of reference table

        'direction
        Select Case loads(iload, 3)
            Case "X"
                oSB.AppendLine(ConCat_pvt("3", "0", "X"))
            Case "Y"
                oSB.AppendLine(ConCat_pvt("3", "1", "Y"))
            Case "Z"
                oSB.AppendLine(ConCat_pvt("3", "2", "Z"))
        End Select
        'load value
        oSB.AppendLine(ConCat_pv("4", loads(iload, 4) * 1000))
        'coordinate system & "location"
        Select Case loads(iload, 2)
            Case "GCS - Length"
                oSB.AppendLine(ConCat_pvt("5", "0", "GCS"))
                oSB.AppendLine(ConCat_pvt("6", "0", "Length"))
            Case "GCS - Projection"
                oSB.AppendLine(ConCat_pvt("5", "0", "GCS"))
                oSB.AppendLine(ConCat_pvt("6", "1", "Projection"))
            Case "LCS"
                oSB.AppendLine(ConCat_pvt("5", "1", "LCS"))
        End Select
        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteSurfaceThermalLoadHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Reference Table"))
        oSB.AppendLine(ConCat_ht("2", "Load case"))
        oSB.AppendLine(ConCat_ht("3", "Distribution"))
        oSB.AppendLine(ConCat_ht("4", "Delta"))
        oSB.AppendLine(ConCat_ht("5", "+z - Top delta"))
        oSB.AppendLine(ConCat_ht("6", "-z - Bottom delta"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteSurfaceThermalLoad(ByRef oSB, iload, loads(,)) 'write 1 surface load to the XML stream

        oSB.AppendLine("<obj id=""" & Trim(Str(iload)) & """ nm=""" & "STLS" & Trim(Str(iload)) & """>")
        oSB.AppendLine(ConCat_pv("0", "STLS" & Trim(Str(iload))))

        'write beam name as reference table
        oSB.AppendLine("<p1 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member2D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member2D)))
        oSB.AppendLine(ConCat_pv("2", loads(iload, 1)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p1>")
        'end of reference table
        oSB.AppendLine(ConCat_pn("2", loads(iload, 0)))
        'distribution
        Select Case loads(iload, 2)
            Case "Constant"
                oSB.AppendLine(ConCat_pvt("3", "0", "Constant"))
            Case "Linear"
                oSB.AppendLine(ConCat_pvt("3", "1", "Linear"))
        End Select
        'load value
        oSB.AppendLine(ConCat_pv("4", loads(iload, 3)))
        oSB.AppendLine(ConCat_pv("5", loads(iload, 4)))
        oSB.AppendLine(ConCat_pv("6", loads(iload, 5)))

        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WritePointLoadsPointHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Load case"))
        oSB.AppendLine(ConCat_ht("1", "Name"))
        oSB.AppendLine(ConCat_ht("2", "Reference Table"))
        oSB.AppendLine(ConCat_ht("3", "Direction"))
        oSB.AppendLine(ConCat_ht("4", "System"))
        oSB.AppendLine(ConCat_ht("5", "Value - F"))
        oSB.AppendLine(ConCat_ht("6", "Angle [deg]"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WritePointLoadsPoint(ByRef oSB As Text.StringBuilder, iload As Integer, modelData As ModelData)

        Dim loads = modelData.NodePointLoads
        oSB.AppendLine("<obj id=""" & Trim(Str(iload)) & """ nm=""" & "PLP" & Trim(Str(iload)) & """>")

        oSB.AppendLine(ConCat_pn("0", loads(iload, 0)))
        oSB.AppendLine(ConCat_pv("1", "PLP" & Trim(Str(iload))))
        'write beam name as reference table
        oSB.AppendLine("<p2 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Node)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Node)))
        oSB.AppendLine(ConCat_pv("2", modelData.GetNodeName(loads(iload, 1))))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p2>")
        'end of reference table

        'direction
        Select Case loads(iload, 3)
            Case "X"
                oSB.AppendLine(ConCat_pvt("3", "0", "X"))
            Case "Y"
                oSB.AppendLine(ConCat_pvt("3", "1", "Y"))
            Case "Z"
                oSB.AppendLine(ConCat_pvt("3", "2", "Z"))
        End Select
        'coordinate system
        Select Case loads(iload, 2)
            Case "GCS"
                oSB.AppendLine(ConCat_pvt("4", "0", "GCS"))
            Case "LCS"
                oSB.AppendLine(ConCat_pvt("4", "1", "LCS"))
        End Select
        'load value
        oSB.AppendLine(ConCat_pv("5", loads(iload, 4) * 1000))
        'angle
        oSB.AppendLine(ConCat_pv("6", loads(iload, 5)))
        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteMomentLoadsPointHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Load case"))
        oSB.AppendLine(ConCat_ht("1", "Name"))
        oSB.AppendLine(ConCat_ht("2", "Reference Table"))
        oSB.AppendLine(ConCat_ht("3", "Direction"))
        oSB.AppendLine(ConCat_ht("4", "System"))
        oSB.AppendLine(ConCat_ht("5", "Value - M"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteMomentLoadsPoint(ByRef oSB As Text.StringBuilder, iload As Integer, modelData As ModelData)

        Dim loads = modelData.NodePointMoments
        oSB.AppendLine("<obj id=""" & Trim(Str(iload)) & """ nm=""" & "MLP" & Trim(Str(iload)) & """>")


        oSB.AppendLine(ConCat_pn("0", loads(iload, 0)))
        oSB.AppendLine(ConCat_pv("1", "MLP" & Trim(Str(iload))))
        'write beam name as reference table
        oSB.AppendLine("<p2 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Node)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Node)))
        oSB.AppendLine(ConCat_pv("2", modelData.GetNodeName(loads(iload, 1))))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p2>")
        'end of reference table

        'direction
        Select Case loads(iload, 3)
            Case "Mx"
                oSB.AppendLine(ConCat_pvt("3", "0", "Mx"))
            Case "My"
                oSB.AppendLine(ConCat_pvt("3", "1", "My"))
            Case "Mz"
                oSB.AppendLine(ConCat_pvt("3", "2", "Mz"))
        End Select
        'coordinate system
        Select Case loads(iload, 2)
            Case "GCS"
                oSB.AppendLine(ConCat_pvt("4", "0", "GCS"))
            Case "LCS"
                oSB.AppendLine(ConCat_pvt("4", "1", "LCS"))
        End Select
        'load value
        oSB.AppendLine(ConCat_pv("5", loads(iload, 4) * 1000))

        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WritePointLoadsBeamHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Load case"))
        oSB.AppendLine(ConCat_ht("1", "Name"))
        oSB.AppendLine(ConCat_ht("2", "Reference Table"))
        oSB.AppendLine(ConCat_ht("3", "Direction"))
        oSB.AppendLine(ConCat_ht("4", "System"))
        oSB.AppendLine(ConCat_ht("5", "Value - F"))
        oSB.AppendLine(ConCat_ht("6", "Coord. definition"))
        oSB.AppendLine(ConCat_ht("7", "Position x"))
        oSB.AppendLine(ConCat_ht("8", "Origin"))
        oSB.AppendLine(ConCat_ht("9", "Repeat (n)"))
        oSB.AppendLine(ConCat_ht("10", "Eccentricity ey"))
        oSB.AppendLine(ConCat_ht("11", "Eccentricity ez"))
        oSB.AppendLine(ConCat_ht("12", "Delta x"))
        oSB.AppendLine("</h>")
    End Sub

    Sub WritePointLoadsBeam(ByRef oSB, iload, loads(,))

        oSB.AppendLine("<obj id=""" & Trim(Str(iload)) & """ nm=""" & "PLB" & Trim(Str(iload)) & """>")
        oSB.AppendLine(ConCat_pn("0", loads(iload, 0)))
        oSB.AppendLine(ConCat_pv("1", "PLB" & Trim(Str(iload))))
        'write beam name as reference table
        oSB.AppendLine("<p2 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("2", loads(iload, 1)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p2>")
        'end of reference table

        'direction
        Select Case loads(iload, 3)
            Case "X"
                oSB.AppendLine(ConCat_pvt("3", "0", "X"))
            Case "Y"
                oSB.AppendLine(ConCat_pvt("3", "1", "Y"))
            Case "Z"
                oSB.AppendLine(ConCat_pvt("3", "2", "Z"))
        End Select
        'coordinate system
        Select Case loads(iload, 2)
            Case "GCS"
                oSB.AppendLine(ConCat_pvt("4", "0", "GCS"))
            Case "LCS"
                oSB.AppendLine(ConCat_pvt("4", "1", "LCS"))
        End Select
        'load value
        oSB.AppendLine(ConCat_pv("5", loads(iload, 4) * 1000))
        Select Case loads(iload, 5)
            Case "Rela"
                oSB.AppendLine(ConCat_pvt("6", "1", "Rela"))
            Case "Abso"
                oSB.AppendLine(ConCat_pvt("6", "0", "Abso"))
        End Select
        oSB.AppendLine(ConCat_pv("7", loads(iload, 6)))
        Select Case loads(iload, 7)
            Case "From start"
                oSB.AppendLine(ConCat_pvt("8", "0", "From start"))
            Case "From end"
                oSB.AppendLine(ConCat_pvt("8", "1", "From end"))
        End Select


        oSB.AppendLine(ConCat_pv("9", loads(iload, 8)))
        oSB.AppendLine(ConCat_pv("10", loads(iload, 9)))
        oSB.AppendLine(ConCat_pv("11", loads(iload, 10)))
        oSB.AppendLine(ConCat_pv("12", loads(iload, 11)))

        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteMomentLoadsBeamHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Load case"))
        oSB.AppendLine(ConCat_ht("1", "Name"))
        oSB.AppendLine(ConCat_ht("2", "Reference Table"))
        oSB.AppendLine(ConCat_ht("3", "Direction"))
        oSB.AppendLine(ConCat_ht("4", "System"))
        oSB.AppendLine(ConCat_ht("5", "Value - F"))
        oSB.AppendLine(ConCat_ht("6", "Coord. definition"))
        oSB.AppendLine(ConCat_ht("7", "Position x"))
        oSB.AppendLine(ConCat_ht("8", "Origin"))
        oSB.AppendLine(ConCat_ht("9", "Repeat (n)"))
        oSB.AppendLine(ConCat_ht("10", "Delta x"))
        oSB.AppendLine("</h>")
    End Sub

    Sub WriteMomentLoadsBeam(ByRef oSB, iload, loads(,))

        oSB.AppendLine("<obj id=""" & Trim(Str(iload)) & """ nm=""" & "MLB" & Trim(Str(iload)) & """>")
        oSB.AppendLine(ConCat_pn("0", loads(iload, 0)))
        oSB.AppendLine(ConCat_pv("1", "MLB" & Trim(Str(iload))))
        'write beam name as reference table
        oSB.AppendLine("<p2 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("2", loads(iload, 1)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p2>")
        'end of reference table

        'direction
        Select Case loads(iload, 3)
            Case "Mx"
                oSB.AppendLine(ConCat_pvt("3", "0", "Mx"))
            Case "My"
                oSB.AppendLine(ConCat_pvt("3", "1", "My"))
            Case "Mz"
                oSB.AppendLine(ConCat_pvt("3", "2", "Mz"))
        End Select
        'coordinate system
        Select Case loads(iload, 2)
            Case "GCS"
                oSB.AppendLine(ConCat_pvt("4", "0", "GCS"))
            Case "LCS"
                oSB.AppendLine(ConCat_pvt("4", "1", "LCS"))
        End Select
        'load value
        oSB.AppendLine(ConCat_pv("5", loads(iload, 4) * 1000))
        Select Case loads(iload, 5)
            Case "Rela"
                oSB.AppendLine(ConCat_pvt("6", "1", "Rela"))
            Case "Abso"
                oSB.AppendLine(ConCat_pvt("6", "0", "Abso"))
        End Select
        oSB.AppendLine(ConCat_pv("7", loads(iload, 6)))
        Select Case loads(iload, 7)
            Case "From start"
                oSB.AppendLine(ConCat_pvt("8", "0", "From start"))
            Case "From end"
                oSB.AppendLine(ConCat_pvt("8", "1", "From end"))
        End Select


        oSB.AppendLine(ConCat_pv("9", loads(iload, 8)))
        oSB.AppendLine(ConCat_pv("10", loads(iload, 9)))


        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteFreePointLoadHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Load case"))
        oSB.AppendLine(ConCat_ht("1", "Name"))
        oSB.AppendLine(ConCat_ht("2", "Direction"))
        oSB.AppendLine(ConCat_ht("3", "Validity"))
        oSB.AppendLine(ConCat_ht("4", "Select"))
        oSB.AppendLine(ConCat_ht("5", "Value - F"))
        oSB.AppendLine(ConCat_ht("6", "Coord X"))
        oSB.AppendLine(ConCat_ht("7", "Coord Y"))
        oSB.AppendLine(ConCat_ht("8", "Coord Z"))
        oSB.AppendLine(ConCat_ht("9", "System"))
        oSB.AppendLine(ConCat_ht("10", "Selected objects"))
        oSB.AppendLine(ConCat_ht("11", "Validity from"))
        oSB.AppendLine(ConCat_ht("12", "Validity to"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteFreePointLoad(ByRef oSB, scale, iload, loads(,)) 'write 1 free point load to the XML stream
        'a free point load consists of:
        'Load Case, Selection, Validity, coord sys (GCS/LCS), direction (X, Y, Z), value (kN), PointX, PointY, PointZ

        oSB.AppendLine("<obj id=""" & Trim(Str(iload)) & """ nm=""" & "FF" & Trim(Str(iload)) & """>")
        oSB.AppendLine(ConCat_pn("0", loads(iload, 0)))
        oSB.AppendLine(ConCat_pv("1", "FF" & Trim(Str(iload))))
        'direction
        Select Case loads(iload, 4)
            Case "X"
                oSB.AppendLine(ConCat_pvt("2", "0", "X"))
            Case "Y"
                oSB.AppendLine(ConCat_pvt("2", "1", "Y"))
            Case "Z"
                oSB.AppendLine(ConCat_pvt("2", "2", "Z"))
        End Select
        'validity
        Select Case loads(iload, 1)
            Case "All"
                oSB.AppendLine(ConCat_pvt("3", "0", "All"))
            Case "-Z"
                oSB.AppendLine(ConCat_pvt("3", "1", "-Z"))
            Case "+Z"
                oSB.AppendLine(ConCat_pvt("3", "2", "+Z"))
            Case "From-to"
                oSB.AppendLine(ConCat_pvt("3", "3", "From-to"))
            Case "Z=0"
                oSB.AppendLine(ConCat_pvt("3", "4", "Z=0"))
            Case "-Z"
                oSB.AppendLine(ConCat_pvt("3", "5", "-Z (incl. 0)"))
            Case "+Z"
                oSB.AppendLine(ConCat_pvt("3", "6", "+Z (incl. 0)"))
        End Select
        'selection
        oSB.AppendLine(ConCat_pvt("4", "0", "Auto"))
        'load value
        oSB.AppendLine(ConCat_pv("5", loads(iload, 5) * 1000))
        'point X, Y, Z coordinates
        oSB.AppendLine(ConCat_pv("6", loads(iload, 6) * scale))
        oSB.AppendLine(ConCat_pv("7", loads(iload, 7) * scale))
        oSB.AppendLine(ConCat_pv("8", loads(iload, 8) * scale))
        'coordinate system
        Select Case loads(iload, 3)
            Case "GCS"
                oSB.AppendLine(ConCat_pvt("9", "0", "GCS"))
            Case "Member LCS"
                oSB.AppendLine(ConCat_pvt("9", "1", "Member LCS"))
        End Select
        oSB.AppendLine(ConCat_pv("10", ""))
        oSB.AppendLine(ConCat_pv("11", loads(iload, 9)))
        oSB.AppendLine(ConCat_pv("12", loads(iload, 10)))
        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteFreePointMomentLoadHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Load case"))
        oSB.AppendLine(ConCat_ht("1", "Name"))
        oSB.AppendLine(ConCat_ht("2", "Direction"))
        oSB.AppendLine(ConCat_ht("3", "Validity"))
        oSB.AppendLine(ConCat_ht("4", "Select"))
        oSB.AppendLine(ConCat_ht("5", "Value - F"))
        oSB.AppendLine(ConCat_ht("6", "Coord X"))
        oSB.AppendLine(ConCat_ht("7", "Coord Y"))
        oSB.AppendLine(ConCat_ht("8", "Coord Z"))
        oSB.AppendLine(ConCat_ht("9", "System"))
        oSB.AppendLine(ConCat_ht("10", "Selected objects"))
        oSB.AppendLine(ConCat_ht("11", "Validity from"))
        oSB.AppendLine(ConCat_ht("12", "Validity to"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteFreePointMomentLoad(ByRef oSB, scale, iload, loads(,)) 'write 1 free point load to the XML stream
        'a free point load consists of:
        'Load Case, Selection, Validity, coord sys (GCS/LCS), direction (X, Y, Z), value (kN), PointX, PointY, PointZ

        oSB.AppendLine("<obj id=""" & Trim(Str(iload)) & """ nm=""" & "FMP" & Trim(Str(iload)) & """>")
        oSB.AppendLine(ConCat_pn("0", loads(iload, 0)))
        oSB.AppendLine(ConCat_pv("1", "FMP" & Trim(Str(iload))))
        'direction
        Select Case loads(iload, 4)
            Case "Mx"
                oSB.AppendLine(ConCat_pvt("2", "0", "Mx"))
            Case "My"
                oSB.AppendLine(ConCat_pvt("2", "1", "My"))
            Case "Mz"
                oSB.AppendLine(ConCat_pvt("2", "2", "Mz"))
        End Select
        'validity
        Select Case loads(iload, 1)
            Case "All"
                oSB.AppendLine(ConCat_pvt("3", "0", "All"))
            Case "-Z"
                oSB.AppendLine(ConCat_pvt("3", "1", "-Z"))
            Case "+Z"
                oSB.AppendLine(ConCat_pvt("3", "2", "+Z"))
            Case "From-to"
                oSB.AppendLine(ConCat_pvt("3", "3", "From-to"))
            Case "Z=0"
                oSB.AppendLine(ConCat_pvt("3", "4", "Z=0"))
            Case "-Z"
                oSB.AppendLine(ConCat_pvt("3", "5", "-Z (incl. 0)"))
            Case "+Z"
                oSB.AppendLine(ConCat_pvt("3", "6", "+Z (incl. 0)"))
        End Select
        'selection
        oSB.AppendLine(ConCat_pvt("4", "0", "Auto"))
        'load value
        oSB.AppendLine(ConCat_pv("5", loads(iload, 5) * 1000))
        'point X, Y, Z coordinates
        oSB.AppendLine(ConCat_pv("6", loads(iload, 6) * scale))
        oSB.AppendLine(ConCat_pv("7", loads(iload, 7) * scale))
        oSB.AppendLine(ConCat_pv("8", loads(iload, 8) * scale))
        'coordinate system
        Select Case loads(iload, 3)
            Case "GCS"
                oSB.AppendLine(ConCat_pvt("9", "0", "GCS"))
            Case "Member LCS"
                oSB.AppendLine(ConCat_pvt("9", "1", "Member LCS"))
        End Select
        oSB.AppendLine(ConCat_pv("10", ""))
        oSB.AppendLine(ConCat_pv("11", loads(iload, 9)))
        oSB.AppendLine(ConCat_pv("12", loads(iload, 10)))


        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteFreeLineLoadHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Load case"))
        oSB.AppendLine(ConCat_ht("1", "Name"))
        oSB.AppendLine(ConCat_ht("2", "Direction"))
        oSB.AppendLine(ConCat_ht("3", "Distribution"))
        oSB.AppendLine(ConCat_ht("4", "Value - P@1"))
        oSB.AppendLine(ConCat_ht("5", "Value - P@2"))
        oSB.AppendLine(ConCat_ht("6", "Validity"))
        oSB.AppendLine(ConCat_ht("7", "Select"))
        oSB.AppendLine(ConCat_ht("8", "System"))
        oSB.AppendLine(ConCat_ht("9", "Location"))
        oSB.AppendLine(ConCat_ht("10", "Table of geometry"))
        oSB.AppendLine(ConCat_ht("11", "Selected objects"))
        oSB.AppendLine(ConCat_ht("12", "Validity from"))
        oSB.AppendLine(ConCat_ht("13", "Validity to"))
        oSB.AppendLine("</h>")
    End Sub


    Private Sub WriteFreeLineLoad(ByRef oSB, scale, iload, loads(,)) 'write 1 free line load to the XML stream
        'a free line load consists of:
        'load case, validity, selection, coord. system (GCS/LCS), direction (X, Y, Z), value (kN/m), LineShape


        oSB.AppendLine("<obj id=""" & Trim(Str(iload)) & """ nm=""" & "FL" & Trim(Str(iload)) & """>")
        oSB.AppendLine(ConCat_pn("0", loads(iload, 0)))
        oSB.AppendLine(ConCat_pv("1", "FL" & Trim(Str(iload))))
        'direction
        Select Case loads(iload, 4)
            Case "X"
                oSB.AppendLine(ConCat_pvt("2", "0", "X"))
            Case "Y"
                oSB.AppendLine(ConCat_pvt("2", "1", "Y"))
            Case "Z"
                oSB.AppendLine(ConCat_pvt("2", "2", "Z"))
        End Select

        'distribution
        Select Case loads(iload, 5)
            Case "Uniform"
                oSB.AppendLine(ConCat_pvt("3", "0", loads(iload, 5)))
            Case "Trapez"
                oSB.AppendLine(ConCat_pvt("3", "1", loads(iload, 5)))
            Case Else
                oSB.AppendLine(ConCat_pvt("3", "0", loads(iload, 5)))
        End Select

        'load value
        oSB.AppendLine(ConCat_pv("4", loads(iload, 6) * 1000))
        oSB.AppendLine(ConCat_pv("5", loads(iload, 7) * 1000))
        'validity
        Select Case loads(iload, 1)
            Case "All"
                oSB.AppendLine(ConCat_pvt("6", "0", "All"))
            Case "-Z"
                oSB.AppendLine(ConCat_pvt("6", "1", "-Z"))
            Case "+Z"
                oSB.AppendLine(ConCat_pvt("6", "2", "+Z"))
            Case "From-to"
                oSB.AppendLine(ConCat_pvt("6", "3", "From-to"))
            Case "Z=0"
                oSB.AppendLine(ConCat_pvt("6", "4", "Z=0"))
            Case "-Z"
                oSB.AppendLine(ConCat_pvt("6", "5", "-Z (incl. 0)"))
            Case "+Z"
                oSB.AppendLine(ConCat_pvt("6", "6", "+Z (incl. 0)"))
        End Select

        ' selection
        Dim selection As Koala.Selection = Koala.GetEnum(Of Koala.Selection)(loads(iload, 2))
        Select Case selection
            Case Koala.Selection.Select
                oSB.AppendLine(ConCat_pvt("7", "1", "Select"))
            Case Else
                oSB.AppendLine(ConCat_pvt("7", "0", "Auto"))
        End Select

        'coordinate system
        Select Case loads(iload, 3)
            Case "GCS - Length"
                oSB.AppendLine(ConCat_pvt("8", "0", "GCS"))
                oSB.AppendLine(ConCat_pvt("9", "0", "Length"))
            Case "GCS - Projection"
                oSB.AppendLine(ConCat_pvt("8", "0", "GCS"))
                oSB.AppendLine(ConCat_pvt("9", "1", "Projection"))
            Case "Member LCS"
                oSB.AppendLine(ConCat_pvt("8", "1", "Member LCS"))
        End Select

        'table of geometry

        Dim lineParts As String() = loads(iload, 8).Split(";")
        Dim lineType As String = lineParts(0).ToLower().Trim()
        Dim pointCount As Long = (lineParts.Length - 1) / 3

        If Not lineType.Equals("line") And Not lineType.Equals("polyline") Then
            Throw New ArgumentException("FreeLineLoad only supports line or polyline geometry")
        End If

        oSB.AppendLine(ConCat_opentable("10", ""))

        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Node"))
        oSB.AppendLine(ConCat_ht("1", "Point definition"))
        oSB.AppendLine(ConCat_ht("2", "Coord X"))
        oSB.AppendLine(ConCat_ht("3", "Coord Y"))
        oSB.AppendLine(ConCat_ht("4", "Coord Z"))
        oSB.AppendLine(ConCat_ht("5", "Edge"))
        oSB.AppendLine("</h>")

        Dim row_id As Long = 0
        For i As Long = 0 To pointCount - 1
            oSB.AppendLine(ConCat_row(row_id))
            If i = pointCount - 1 Then
                oSB.AppendLine(ConCat_pv("0", "End"))
            Else
                oSB.AppendLine(ConCat_pv("0", "Head"))
            End If
            oSB.AppendLine(ConCat_pvt("1", "0", "Standard"))
            oSB.AppendLine(ConCat_pv("2", Trim(lineParts(i * 3 + 1) * scale))) 'node X
            oSB.AppendLine(ConCat_pv("3", Trim(lineParts(i * 3 + 2) * scale))) 'node Y
            oSB.AppendLine(ConCat_pv("4", Trim(lineParts(i * 3 + 3) * scale))) 'node Z
            Select Case lineType 'curve type - only "Line by 2 pts" or polyline is supported by SCIA Engineer
                Case "Arc"
                    oSB.AppendLine(ConCat_pvt("5", "1", "Circle arc")) 'not supported in SE
                Case "Spline"
                    oSB.AppendLine(ConCat_pvt("5", "7", "Spline")) 'not supported in SE
                Case Else
                    oSB.AppendLine(ConCat_pvt("5", "0", "Line"))
            End Select
            oSB.AppendLine("</row>")
            row_id += 1
        Next

        oSB.AppendLine(ConCat_closetable("10"))
        oSB.AppendLine(ConCat_pv("11", ""))
        oSB.AppendLine(ConCat_pv("12", loads(iload, 9)))
        oSB.AppendLine(ConCat_pv("13", loads(iload, 10)))

        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteFreeSurfaceLoadHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Load case"))
        oSB.AppendLine(ConCat_ht("1", "Name"))
        oSB.AppendLine(ConCat_ht("2", "Direction"))
        oSB.AppendLine(ConCat_ht("3", "Distribution"))
        oSB.AppendLine(ConCat_ht("4", "q"))
        oSB.AppendLine(ConCat_ht("5", "q1"))
        oSB.AppendLine(ConCat_ht("6", "q2"))
        oSB.AppendLine(ConCat_ht("7", "Validity"))
        oSB.AppendLine(ConCat_ht("8", "Select"))
        oSB.AppendLine(ConCat_ht("9", "System"))
        oSB.AppendLine(ConCat_ht("10", "Location"))
        oSB.AppendLine(ConCat_ht("11", "Table of geometry"))
        oSB.AppendLine(ConCat_ht("12", "Selected objects"))
        oSB.AppendLine(ConCat_ht("13", "Validity from"))
        oSB.AppendLine(ConCat_ht("14", "Validity to"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteFreeSurfaceLoad(ByRef oSB, scale, iload, loads(,), UILanguageNumber) 'write 1 free surface load to the XML stream
        'a free line load consists of:
        'load case, validity, selection, coord. system (GCS/LCS), direction (X, Y, Z), value (kN/m), BoundaryShape

        'oSB.AppendLine(ConCat_ht("0", "Load case"))
        'oSB.AppendLine(ConCat_ht("1", "Name"))
        'oSB.AppendLine(ConCat_ht("2", "Direction"))
        'oSB.AppendLine(ConCat_ht("3", "Distribution"))
        'oSB.AppendLine(ConCat_ht("4", "q"))
        'oSB.AppendLine(ConCat_ht("5", "q1"))
        'oSB.AppendLine(ConCat_ht("6", "q2"))
        'oSB.AppendLine(ConCat_ht("7", "Validity"))
        'oSB.AppendLine(ConCat_ht("8", "Select"))
        'oSB.AppendLine(ConCat_ht("9", "System"))
        'oSB.AppendLine(ConCat_ht("10", "Location"))
        'oSB.AppendLine(ConCat_ht("11", "Table of geometry"))
        'oSB.AppendLine(ConCat_ht("12", "Selected objects"))
        'oSB.AppendLine(ConCat_ht("13", "Validity from"))
        'oSB.AppendLine(ConCat_ht("14", "Validity to"))

        Dim BoundaryShape As String
        Dim LineShape As String
        Dim row_id As Long

        oSB.AppendLine("<obj id=""" & Trim(Str(iload)) & """ nm=""" & "FL" & Trim(Str(iload)) & """>")
        oSB.AppendLine(ConCat_pn("0", loads(iload, 0)))
        oSB.AppendLine(ConCat_pv("1", "FL" & Trim(Str(iload))))
        'direction
        Dim direction As Koala.Direction = Koala.GetEnum(Of Koala.Direction)(loads(iload, 4))
        Select Case direction
            Case Koala.Direction.X
                oSB.AppendLine(ConCat_pvt("2", "0", "X"))
            Case Koala.Direction.Y
                oSB.AppendLine(ConCat_pvt("2", "1", "Y"))
            Case Else
                oSB.AppendLine(ConCat_pvt("2", "2", "Z"))
        End Select
        'distribution
        Dim distribution As Koala.DistributionOfSurfaceLoad = Koala.GetEnum(Of Koala.DistributionOfSurfaceLoad)(loads(iload, 5))
        Select Case distribution
            Case Koala.DistributionOfSurfaceLoad.DirectionX
                oSB.AppendLine(ConCat_pvt("3", "1", "Dir X"))
            Case Koala.DistributionOfSurfaceLoad.DirectionY
                oSB.AppendLine(ConCat_pvt("3", "2", "Dir Y"))
            Case Else
                oSB.AppendLine(ConCat_pvt("3", "0", "Uniform"))
        End Select
        'load value(s)
        Select Case distribution
            Case Koala.DistributionOfSurfaceLoad.DirectionX, Koala.DistributionOfSurfaceLoad.DirectionY
                oSB.AppendLine(ConCat_pv("5", loads(iload, 6) * 1000))
                oSB.AppendLine(ConCat_pv("6", loads(iload, 7) * 1000))
            Case Else
                oSB.AppendLine(ConCat_pv("4", loads(iload, 6) * 1000))
        End Select

        'validity
        Dim validity As Koala.Validity = Koala.GetEnum(Of Koala.Validity)(loads(iload, 1))
        Select Case validity
            Case Koala.Validity.ZNeg
                oSB.AppendLine(ConCat_pvt("7", "1", "-Z"))
            Case Koala.Validity.ZPos
                oSB.AppendLine(ConCat_pvt("7", "2", "+Z"))
            Case Koala.Validity.FromTo
                oSB.AppendLine(ConCat_pvt("7", "3", "From-to"))
            Case Koala.Validity.ZZero
                oSB.AppendLine(ConCat_pvt("7", "4", "Z=0"))
            Case Koala.Validity.ZNegOrZero
                oSB.AppendLine(ConCat_pvt("7", "5", "-Z (incl. 0)"))
            Case Koala.Validity.ZPosOrZero
                oSB.AppendLine(ConCat_pvt("7", "6", "+Z (incl. 0)"))
            Case Else
                oSB.AppendLine(ConCat_pvt("7", "0", "All"))
        End Select

        'selection (loads(iload,2))
        Dim selection As Koala.Selection = Koala.GetEnum(Of Koala.Selection)(loads(iload, 2))
        Select Case selection
            Case Koala.Selection.Select
                oSB.AppendLine(ConCat_pvt("8", "1", "Select"))
            Case Else
                oSB.AppendLine(ConCat_pvt("8", "0", "Auto"))
        End Select

        'coordinate system
        Dim coordSys As Koala.CoordSystemFreeLoad = Koala.GetEnum(Of Koala.CoordSystemFreeLoad)(loads(iload, 3))
        Select Case coordSys
            Case Koala.CoordSystemFreeLoad.GCSProjection
                oSB.AppendLine(ConCat_pvt("9", "0", "GCS"))
                oSB.AppendLine(ConCat_pvt("10", "1", "Projection"))
            Case Koala.CoordSystemFreeLoad.MemberLCS
                oSB.AppendLine(ConCat_pvt("9", "1", "Member LCS"))
            Case Else
                oSB.AppendLine(ConCat_pvt("9", "0", "GCS"))
                oSB.AppendLine(ConCat_pvt("10", "0", "Length"))
        End Select

        'table of geometry

        oSB.AppendLine(ConCat_opentable("11", ""))

        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Node"))
        oSB.AppendLine(ConCat_ht("1", "Point definition"))
        oSB.AppendLine(ConCat_ht("2", "Coord X"))
        oSB.AppendLine(ConCat_ht("3", "Coord Y"))
        oSB.AppendLine(ConCat_ht("4", "Coord Z"))
        oSB.AppendLine(ConCat_ht("5", "Edge"))
        oSB.AppendLine("</h>")

        BoundaryShape = loads(iload, 8)
        row_id = 0

        For Each LineShape In BoundaryShape.Split("|")

            oSB.AppendLine(ConCat_row(row_id))
            oSB.AppendLine(ConCat_pv("0", "Head"))
            oSB.AppendLine(ConCat_pvt("1", "0", "Standard"))
            oSB.AppendLine(ConCat_pv("2", Trim(Split(LineShape, ";")(1) * scale))) 'first node X
            oSB.AppendLine(ConCat_pv("3", Trim(Split(LineShape, ";")(2) * scale))) 'first node Y
            oSB.AppendLine(ConCat_pv("4", Trim(Split(LineShape, ";")(3) * scale))) 'first node Z
            Select Case Strings.Trim(Strings.Split(LineShape, ";")(0)) 'curve type - only "Line by 2 pts" is supported by SCIA Engineer
                Case "Line"
                    If UILanguageNumber = "0" Then oSB.AppendLine(ConCat_pv("5", "Line")) 'English
                    If UILanguageNumber = "1" Then oSB.AppendLine(ConCat_pv("5", "Lijn")) 'Dutch
                    If UILanguageNumber = "2" Then oSB.AppendLine(ConCat_pv("5", "Ligne")) 'French
                    If UILanguageNumber = "3" Then oSB.AppendLine(ConCat_pv("5", "Linie")) 'German
                    If UILanguageNumber = "4" Then oSB.AppendLine(ConCat_pv("5", "Přímka")) 'Czech
                    If UILanguageNumber = "5" Then oSB.AppendLine(ConCat_pv("5", "Čiara")) 'Slovak
                Case "Arc"
                    oSB.AppendLine(ConCat_pv("5", "Circle arc")) 'not supported in SE
                Case "Spline"
                    oSB.AppendLine(ConCat_pv("5", "Spline")) 'not supported in SE
            End Select

            oSB.AppendLine("</row>")

            row_id += 1
        Next LineShape

        oSB.AppendLine(ConCat_closetable("11"))

        'table of selection
        Dim selectionNames As String = loads(iload, 11)

        If Not String.IsNullOrEmpty(selectionNames) Then
            oSB.AppendLine(ConCat_opentable("12", ""))

            oSB.AppendLine("<h>")
            oSB.AppendLine(ConCat_ht("0", "Type"))
            oSB.AppendLine(ConCat_ht("1", "Type human name"))
            oSB.AppendLine(ConCat_ht("2", "Id"))
            oSB.AppendLine(ConCat_ht("3", "Name"))
            oSB.AppendLine("</h>")

            row_id = 0
            Dim member As String
            For Each member In selectionNames.Split("|")
                oSB.AppendLine(ConCat_row(row_id))
                Dim memberParts As String() = member.Split(";")

                If memberParts.Length <> 3 Then
                    Throw New ArgumentException("Invalid FreeSurfaceLoad SelectedMember2D string")
                End If

                'different reference depending whether it's towards a surface or an opening
                Select Case memberParts(1).ToLower.Trim
                    Case "opening"
                        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Opening)))
                        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Opening)))
                    Case Else
                        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member2D)))
                        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member2D)))
                End Select
                oSB.AppendLine(ConCat_pv("2", memberParts(2).Trim))
                oSB.AppendLine(ConCat_pv("3", memberParts(0).Trim))
                oSB.AppendLine("</row>")
                row_id += 1
            Next member

            oSB.AppendLine(ConCat_closetable("12"))
        Else
            ' oSB.AppendLine(ConCat_pv("12", ""))
        End If

        ' validity from and to
        oSB.AppendLine(ConCat_pv("13", loads(iload, 9)))
        oSB.AppendLine(ConCat_pv("14", loads(iload, 10)))

        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteHingeHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Reference table"))
        oSB.AppendLine(ConCat_ht("2", "Position"))
        oSB.AppendLine(ConCat_ht("3", "ux"))
        oSB.AppendLine(ConCat_ht("4", "uy"))
        oSB.AppendLine(ConCat_ht("5", "uz"))
        oSB.AppendLine(ConCat_ht("6", "fix"))
        oSB.AppendLine(ConCat_ht("7", "fiy"))
        oSB.AppendLine(ConCat_ht("8", "fiz"))
        oSB.AppendLine(ConCat_ht("9", "Stiff - ux"))
        oSB.AppendLine(ConCat_ht("10", "Stiff - uy"))
        oSB.AppendLine(ConCat_ht("11", "Stiff - uz"))
        oSB.AppendLine(ConCat_ht("12", "Stiff - fix"))
        oSB.AppendLine(ConCat_ht("13", "Stiff - fiy"))
        oSB.AppendLine(ConCat_ht("14", "Stiff - fiz"))
        oSB.AppendLine(ConCat_ht("15", "Function X"))
        oSB.AppendLine(ConCat_ht("16", "Function Y"))
        oSB.AppendLine(ConCat_ht("17", "Function Z"))
        oSB.AppendLine(ConCat_ht("18", "Function Rx"))
        oSB.AppendLine(ConCat_ht("19", "Function Ry"))
        oSB.AppendLine(ConCat_ht("20", "Function Rz"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteHinge(ByRef oSB, ihinge, hinges(,)) 'write 1 hinge to the XML stream
        oSB.AppendLine("<obj nm=""" & hinges(ihinge, 1) & """>")
        oSB.AppendLine(ConCat_pv("0", hinges(ihinge, 1))) 'Hinge name
        'write beam name as reference table
        oSB.AppendLine("<p1 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("2", hinges(ihinge, 0)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p1>")
        'end of reference table

        oSB.AppendLine(ConCat_pvt_enum(Of Koala.BeamEnd)(2, hinges(ihinge, 2)))

        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomHinge)(3, hinges(ihinge, 3)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomHinge)(4, hinges(ihinge, 4)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomHinge)(5, hinges(ihinge, 5)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomHinge)(6, hinges(ihinge, 6)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomHinge)(7, hinges(ihinge, 7)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DegreeOfFreedomHinge)(8, hinges(ihinge, 8)))

        oSB.AppendLine(ConCat_pv("9", hinges(ihinge, 9)))
        oSB.AppendLine(ConCat_pv("10", hinges(ihinge, 10)))
        oSB.AppendLine(ConCat_pv("11", hinges(ihinge, 11)))
        oSB.AppendLine(ConCat_pv("12", hinges(ihinge, 12)))
        oSB.AppendLine(ConCat_pv("13", hinges(ihinge, 13)))
        oSB.AppendLine(ConCat_pv("14", hinges(ihinge, 14)))

        oSB.AppendLine(ConCat_pin("15", "1", hinges(ihinge, 15)))
        oSB.AppendLine(ConCat_pin("16", "1", hinges(ihinge, 16)))
        oSB.AppendLine(ConCat_pin("17", "1", hinges(ihinge, 17)))
        oSB.AppendLine(ConCat_pin("18", "1", hinges(ihinge, 18)))
        oSB.AppendLine(ConCat_pin("19", "1", hinges(ihinge, 19)))
        oSB.AppendLine(ConCat_pin("20", "1", hinges(ihinge, 20)))

        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteCrossLinkHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Type"))
        oSB.AppendLine(ConCat_ht("2", "1st member"))
        oSB.AppendLine(ConCat_ht("3", "2st member"))
        oSB.AppendLine("</h>")
    End Sub

    Sub WriteCrossLink(ByRef oSB, icorsslink, crosslink(,)) 'write 1 hinge to the XML stream
        oSB.AppendLine("<obj nm=""CRL" & icorsslink & """>")
        oSB.AppendLine(ConCat_pv("0", "CRL" & icorsslink)) 'Cross0-link name

        Select Case crosslink(icorsslink, 0)
            Case "Fixed"
                oSB.AppendLine(ConCat_pvt("1", "0", "Fixed"))
            Case "Hinged"
                oSB.AppendLine(ConCat_pvt("1", "1", "Hinged"))
            Case "Coupler"
                oSB.AppendLine(ConCat_pvt("1", "2", "Coupler"))
        End Select
        oSB.AppendLine(ConCat_pv("2", crosslink(icorsslink, 1)))
        oSB.AppendLine(ConCat_pv("3", crosslink(icorsslink, 2)))

        oSB.AppendLine("</obj>")
    End Sub

    Private Sub WriteLineHingeHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Reference table"))
        oSB.AppendLine(ConCat_ht("2", "Edge"))
        oSB.AppendLine(ConCat_ht("3", "Coord. definition"))
        oSB.AppendLine(ConCat_ht("4", "Position x1"))
        oSB.AppendLine(ConCat_ht("5", "Position x2"))
        oSB.AppendLine(ConCat_ht("6", "Origin"))
        oSB.AppendLine(ConCat_ht("7", "ux"))
        oSB.AppendLine(ConCat_ht("8", "uy"))
        oSB.AppendLine(ConCat_ht("9", "uz"))
        oSB.AppendLine(ConCat_ht("10", "fix"))
        oSB.AppendLine(ConCat_ht("11", "Stiff - ux"))
        oSB.AppendLine(ConCat_ht("12", "Stiff - uy"))
        oSB.AppendLine(ConCat_ht("13", "Stiff - uz"))
        oSB.AppendLine(ConCat_ht("14", "Stiff - fix"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteLineHinge(ByRef oSB, ihinge, hinges(,)) 'write 1 hinge to the XML stream
        Dim tt As String

        oSB.AppendLine("<obj nm=""LHE" & ihinge & """>")
        oSB.AppendLine(ConCat_pv("0", "LHE" & ihinge)) 'Hinge name
        'write beam name as reference table
        oSB.AppendLine("<p1 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member2D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member2D)))
        oSB.AppendLine(ConCat_pv("2", hinges(ihinge, 0)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p1>")
        'end of reference table
        oSB.AppendLine(ConCat_pvt("2", hinges(ihinge, 1) - 1, hinges(ihinge, 1)))


        Select Case hinges(ihinge, 2)
            Case "Rela"
                oSB.AppendLine(ConCat_pvt("3", "1", "Rela"))
            Case "Abso"
                oSB.AppendLine(ConCat_pvt("3", "0", "Abso"))
        End Select
        oSB.AppendLine(ConCat_pv("4", hinges(ihinge, 3)))
        oSB.AppendLine(ConCat_pv("5", hinges(ihinge, 4)))

        Select Case hinges(ihinge, 5)
            Case "From start"
                oSB.AppendLine(ConCat_pvt("6", "0", "From start"))
            Case "From end"
                oSB.AppendLine(ConCat_pvt("6", "1", "From end"))
        End Select
        'p13 & p14 would be the ey and ez eccentricities for the line load



        tt = GetStringForDOF(hinges(ihinge, 6))
        oSB.AppendLine(ConCat_pvt("7", hinges(ihinge, 6), tt))
        tt = GetStringForDOF(hinges(ihinge, 7))
        oSB.AppendLine(ConCat_pvt("8", hinges(ihinge, 7), tt))
        tt = GetStringForDOF(hinges(ihinge, 8))
        oSB.AppendLine(ConCat_pvt("9", hinges(ihinge, 8), tt))
        tt = GetStringForDOF(hinges(ihinge, 9))
        oSB.AppendLine(ConCat_pvt("10", hinges(ihinge, 9), tt))
        oSB.AppendLine(ConCat_pv("11", hinges(ihinge, 10)))
        oSB.AppendLine(ConCat_pv("12", hinges(ihinge, 11)))
        oSB.AppendLine(ConCat_pv("13", hinges(ihinge, 12)))
        oSB.AppendLine(ConCat_pv("14", hinges(ihinge, 13)))

        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteArbitraryProfileHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Reference Table"))
        oSB.AppendLine(ConCat_ht("2", "Coord. definition"))
        oSB.AppendLine(ConCat_ht("3", "Cross-section"))
        oSB.AppendLine(ConCat_ht("4", "Spans table"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteArbitraryProfile(ByRef oSB, idx, aprofiles(,)) 'write 1 ArbitraryProfile to the XML stream
        oSB.AppendLine("<obj nm=""" & aprofiles(idx, 1) & """>")

        'Name
        oSB.AppendLine(ConCat_pv("0", aprofiles(idx, 1)))

        'Object reference table
        oSB.AppendLine("<p1 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("2", aprofiles(idx, 0)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p1>")

        'Coordinate definition
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.CoordinateDefinition)(2, aprofiles(idx, 3)))
        'Section
        oSB.AppendLine(ConCat_pv("3", aprofiles(idx, 2)))
        'Span reference table
        oSB.AppendLine("<p4 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""length 1""/>")
        oSB.AppendLine("<h1 t=""Type of Css(1)""/>")
        oSB.AppendLine("<h2 t=""Cross-section1(1)""/>")
        oSB.AppendLine("<h3 t=""Cross-section2(1)""/>")
        oSB.AppendLine("<h4 t=""Css 1 param(1)""/>")
        oSB.AppendLine("<h5 t=""Css 2 param(1)""/>")
        oSB.AppendLine("<h6 t=""Alignment(1)""/>")
        oSB.AppendLine("</h>")

        Dim SpanLengths = Split(aprofiles(idx, 4), ";")
        Dim SpanTypes = Split(aprofiles(idx, 5), ";")
        Dim SpanCss1 = Split(aprofiles(idx, 6), ";")
        Dim SpanCss2 = Split(aprofiles(idx, 7), ";")
        Dim SpanAlignments = Split(aprofiles(idx, 8), ";")

        Dim SpanCount As Integer = New Integer() {
            SpanLengths.Length,
            SpanTypes.Length,
            SpanCss1.Length,
            SpanCss2.Length,
            SpanAlignments.Length
        }.Min()

        For i As Integer = 0 To SpanCount - 1
            oSB.AppendLine("<row id=""" & i & """>")
            ' span length
            oSB.AppendLine(ConCat_pv("0", SpanLengths(i).Trim()))
            ' span Css type
            oSB.AppendLine(ConCat_pvt_enum(Of Koala.ArbitraryProfileSpanType)(1, SpanTypes(i)))
            ' span css 1 and 2
            oSB.AppendLine(ConCat_pn("2", SpanCss1(i).Trim()))
            oSB.AppendLine(ConCat_pn("3", SpanCss2(i).Trim()))
            ' haunch parameters
            If Koala.MatchesEnum(SpanTypes(i), Koala.ArbitraryProfileSpanType.ParametricHaunch) Then 'SpanTypes(i).Trim() = "ParametricHaunch" Then
                oSB.AppendLine(ConCat_pvt("4", "0", "from DB"))
                oSB.AppendLine(ConCat_pvt("5", "0", "from DB"))
            End If
            ' span alignment
            oSB.AppendLine(ConCat_pvt_enum(Of Koala.ArbitraryProfileAlignment)(6, SpanAlignments(i)))

            oSB.AppendLine("</row>")
        Next

        oSB.AppendLine("</p4>")

        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteIntegrationStripHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "UniqueID"))
        oSB.AppendLine(ConCat_ht("2", "Create meshnodes"))
        oSB.AppendLine(ConCat_ht("3", "Effective width geometry"))
        oSB.AppendLine(ConCat_ht("4", "Effective width definition"))
        oSB.AppendLine(ConCat_ht("5", "Width (total)"))
        oSB.AppendLine(ConCat_ht("6", "No. of thickness (total)"))
        oSB.AppendLine(ConCat_ht("7", "Coord X"))
        oSB.AppendLine(ConCat_ht("8", "Coord Y"))
        oSB.AppendLine(ConCat_ht("9", "Coord Z"))
        oSB.AppendLine(ConCat_ht("10", "Coord X"))
        oSB.AppendLine(ConCat_ht("11", "Coord Y"))
        oSB.AppendLine(ConCat_ht("12", "Coord Z"))
        oSB.AppendLine(ConCat_ht("13", "Length"))
        oSB.AppendLine(ConCat_ht("14", "Shape"))
        oSB.AppendLine(ConCat_ht("15", "2D member"))
        oSB.AppendLine(ConCat_ht("16", "Table of geometry"))
        oSB.AppendLine(ConCat_ht("17", "Width left"))
        oSB.AppendLine(ConCat_ht("18", "Width right"))
        oSB.AppendLine(ConCat_ht("19", "No. of thickness left"))
        oSB.AppendLine(ConCat_ht("20", "No. of thickness right"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteIntegrationStrip(ByRef oSB, iIntegrationStrip, integrationStrips(,)) 'write 1 integration strip to the XML stream
        'a beam consists of: Name, Section, Layer, LineShape, LCSType, LCSParam1, LCSParam2, LCSParam3

        oSB.AppendLine("<obj nm=""" & integrationStrips(iIntegrationStrip, 0) & """>")
        oSB.AppendLine(ConCat_pv("0", integrationStrips(iIntegrationStrip, 0))) 'Name
        'oSB.AppendLine(ConCat_pv("1", integrationStrips(iIntegrationStrip, 1))) 'UniqueId

        Dim mesh_str As String = integrationStrips(iIntegrationStrip, 2)
        Dim mesh_int As Integer = Convert.ToInt32(Convert.ToBoolean(mesh_str))
        oSB.AppendLine(ConCat_pv("2", mesh_int)) 'Create meshnodes

        oSB.AppendLine(ConCat_pvt_enum(Of Koala.EffectiveWidthGeometry)(3, integrationStrips(iIntegrationStrip, 3)))
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.EffectiveWidthDefinition)(4, integrationStrips(iIntegrationStrip, 4)))

        If integrationStrips(iIntegrationStrip, 4) = "Number of thickness" Then
            oSB.AppendLine(ConCat_pv("6", integrationStrips(iIntegrationStrip, 6))) 'No. of thickness (total)
        Else ' = "Width"
            oSB.AppendLine(ConCat_pv("5", integrationStrips(iIntegrationStrip, 5))) 'Width
        End If

        Dim x1 As String = integrationStrips(iIntegrationStrip, 7)
        Dim y1 As String = integrationStrips(iIntegrationStrip, 8)
        Dim z1 As String = integrationStrips(iIntegrationStrip, 9)
        Dim x2 As String = integrationStrips(iIntegrationStrip, 10)
        Dim y2 As String = integrationStrips(iIntegrationStrip, 11)
        Dim z2 As String = integrationStrips(iIntegrationStrip, 12)

        oSB.AppendLine(ConCat_pv("7", x1)) 'X
        oSB.AppendLine(ConCat_pv("8", y1)) 'Y
        oSB.AppendLine(ConCat_pv("9", z1)) 'Z
        oSB.AppendLine(ConCat_pv("10", x2)) 'X
        oSB.AppendLine(ConCat_pv("11", y2)) 'Y
        oSB.AppendLine(ConCat_pv("12", z2)) 'Z

        oSB.AppendLine(ConCat_pv("13", integrationStrips(iIntegrationStrip, 13))) 'Length
        oSB.AppendLine(ConCat_pv("14", "Line")) 'Shape

        oSB.AppendLine(ConCat_pn("15", integrationStrips(iIntegrationStrip, 14))) '2D Member

        ' Reference Table
        oSB.AppendLine(ConCat_opentable("16", ""))
        'Table of Geometry
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("1", "Node"))
        oSB.AppendLine(ConCat_ht("2", "Edge"))
        oSB.AppendLine("</h>")
        oSB.AppendLine(ConCat_row(0))
        oSB.AppendLine(ConCat_pn("1", String.Format("[{0}; {1}; {2}]", x1, y1, z1)))
        oSB.appendline(ConCat_pv("2", "0")) ' Line
        oSB.AppendLine("</row>")
        oSB.AppendLine(ConCat_row(1))
        oSB.AppendLine(ConCat_pn("1", String.Format("[{0}; {1}; {2}]", x2, y2, z2)))
        oSB.AppendLine("</row>")
        oSB.AppendLine(ConCat_closetable("16"))

        If (integrationStrips(iIntegrationStrip, 4) = "Number of thickness") And (integrationStrips(iIntegrationStrip, 3) = "Constant nonsymmetric") Then
            oSB.AppendLine(ConCat_pv("19", integrationStrips(iIntegrationStrip, 15))) 'thickness Left
            oSB.AppendLine(ConCat_pv("20", integrationStrips(iIntegrationStrip, 16))) 'Thickness rigth
        ElseIf (integrationStrips(iIntegrationStrip, 4) = "Width") And (integrationStrips(iIntegrationStrip, 3) = "Constant nonsymmetric") Then
            oSB.AppendLine(ConCat_pv("17", integrationStrips(iIntegrationStrip, 15))) 'width Left
            oSB.AppendLine(ConCat_pv("18", integrationStrips(iIntegrationStrip, 16))) 'width rigth
        End If


        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteSectionOn1DHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Reference Table"))
        oSB.AppendLine(ConCat_ht("1", "Name"))
        oSB.AppendLine(ConCat_ht("2", "UniqueID"))
        oSB.AppendLine(ConCat_ht("3", "Coord. definition"))
        oSB.AppendLine(ConCat_ht("4", "Position x"))
        oSB.AppendLine(ConCat_ht("5", "Origin"))
        oSB.AppendLine(ConCat_ht("6", "Repeat (n)"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteSectionOn1D(ByRef oSB, iSectionOnBeam, sectionOnBeams(,))
        oSB.AppendLine("<obj nm=""" & sectionOnBeams(iSectionOnBeam, 1) & """>")

        'write surface name as reference table
        oSB.AppendLine("<p0 t="""">")
        oSB.AppendLine("<h>")
        oSB.AppendLine("<h0 t=""Member Type""/>")
        oSB.AppendLine("<h1 t=""Member Type Name""/>")
        oSB.AppendLine("<h2 t=""Member Name""/>")
        oSB.AppendLine("</h>")
        oSB.AppendLine("<row id=""0"">")
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member1D)))
        oSB.AppendLine(ConCat_pv("2", sectionOnBeams(iSectionOnBeam, 0)))
        oSB.AppendLine("</row>")
        oSB.AppendLine("</p0>")
        'end of reference table

        oSB.AppendLine(ConCat_pv("1", sectionOnBeams(iSectionOnBeam, 1))) 'Name
        'oSB.AppendLine(ConCat_pv("1", sectionOnBeams(iSectionOnBeam, 2))) 'UniqueID
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.CoordinateDefinition)(3, sectionOnBeams(iSectionOnBeam, 3)))
        oSB.AppendLine(ConCat_pv("4", sectionOnBeams(iSectionOnBeam, 4))) 'Position
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.Origin)(5, sectionOnBeams(iSectionOnBeam, 5)))

        Dim repeat As Int32 = Convert.ToInt32(sectionOnBeams(iSectionOnBeam, 6))
        oSB.AppendLine(ConCat_pv("6", repeat)) 'Repeat (n)
        If repeat > 1 Then
            oSB.AppendLine(ConCat_pv("7", sectionOnBeams(iSectionOnBeam, 7))) 'Delta x
        End If

        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteSectionOn2DHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Name"))
        oSB.AppendLine(ConCat_ht("1", "Draw"))
        oSB.AppendLine(ConCat_ht("2", "Direction of cut"))
        oSB.AppendLine(ConCat_ht("3", "Coord X"))
        oSB.AppendLine(ConCat_ht("4", "Coord Y"))
        oSB.AppendLine(ConCat_ht("5", "Coord Z"))
        oSB.AppendLine(ConCat_ht("6", "Coord X"))
        oSB.AppendLine(ConCat_ht("7", "Coord Y"))
        oSB.AppendLine(ConCat_ht("8", "Coord Z"))
        oSB.AppendLine(ConCat_ht("9", "Layer"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteSectionOn2D(ByRef oSB, iSection, sectionOn2D(,))
        'Name
        oSB.AppendLine("<obj nm=""" & sectionOn2D(iSection, 0) & """>")
        oSB.AppendLine(ConCat_pv("0", sectionOn2D(iSection, 0)))
        'Draw
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.DrawDiagram)(1, sectionOn2D(iSection, 2)))
        'Direction
        Dim direction As Rhino.Geometry.Vector3d
        If Not Grasshopper.Kernel.GH_Convert.ToVector3d(sectionOn2D(iSection, 3), direction, Grasshopper.Kernel.GH_Conversion.Both) Then
            direction = Rhino.Geometry.Vector3d.ZAxis
        End If
        oSB.AppendLine(ConCat_pv1v2v3("2", direction.X, direction.Y, direction.Z))

        'Vertex 1
        Dim vertex1 As Rhino.Geometry.Point3d
        If Not Grasshopper.Kernel.GH_Convert.ToPoint3d(sectionOn2D(iSection, 4), vertex1, Grasshopper.Kernel.GH_Conversion.Both) Then
            Throw New ArgumentException("Invalid SectionOn2D Vertex1")
        End If
        oSB.AppendLine(ConCat_pv("3", vertex1.X))
        oSB.AppendLine(ConCat_pv("4", vertex1.Y))
        oSB.AppendLine(ConCat_pv("5", vertex1.Z))

        'Vertex 2
        Dim vertex2 As Rhino.Geometry.Point3d
        If Not Grasshopper.Kernel.GH_Convert.ToPoint3d(sectionOn2D(iSection, 5), vertex2, Grasshopper.Kernel.GH_Conversion.Both) Then
            Throw New ArgumentException("Invalid SectionOn2D Vertex2")
        End If
        oSB.AppendLine(ConCat_pv("6", vertex2.X))
        oSB.AppendLine(ConCat_pv("7", vertex2.Y))
        oSB.AppendLine(ConCat_pv("8", vertex2.Z))

        ' Layer
        oSB.AppendLine(ConCat_pn("9", sectionOn2D(iSection, 1)))
        oSB.AppendLine("</obj>")

    End Sub

    Private Sub WriteAveragingStripHeaders(ByRef oSB)
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Reference Table"))
        oSB.AppendLine(ConCat_ht("1", "Name"))
        oSB.AppendLine(ConCat_ht("2", "UniqueID"))
        oSB.AppendLine(ConCat_ht("3", "Type"))
        oSB.AppendLine(ConCat_ht("4", "Direction"))
        oSB.AppendLine(ConCat_ht("5", "Width"))
        oSB.AppendLine(ConCat_ht("6", "Length"))
        oSB.AppendLine(ConCat_ht("7", "Angle"))
        oSB.AppendLine(ConCat_ht("8", "Coord X"))
        oSB.AppendLine(ConCat_ht("9", "Coord Y"))
        oSB.AppendLine(ConCat_ht("10", "Coord Z"))
        oSB.AppendLine(ConCat_ht("11", "Coord x"))
        oSB.AppendLine(ConCat_ht("12", "Coord y"))
        oSB.AppendLine(ConCat_ht("13", "Coord z"))
        oSB.AppendLine("</h>")
    End Sub

    Private Sub WriteAveragingStrip(ByRef oSB, iAveragingStrip, averagingStrips(,)) 'write 1 averaging strip to the XML stream
        'a beam consists of: Name, Section, Layer, LineShape, LCSType, LCSParam1, LCSParam2, LCSParam3

        oSB.AppendLine("<obj nm=""" & averagingStrips(iAveragingStrip, 1) & """>") 'obj Name

        ' Reference Table
        oSB.AppendLine(ConCat_opentable("0", ""))
        'Table of Geometry
        oSB.AppendLine("<h>")
        oSB.AppendLine(ConCat_ht("0", "Member Type"))
        oSB.AppendLine(ConCat_ht("1", "Member Type Name"))
        'oSB.AppendLine(ConCat_ht("2", "Member Id"))
        oSB.AppendLine(ConCat_ht("3", "Member Name"))
        oSB.AppendLine("</h>")
        oSB.AppendLine(ConCat_row(0))
        oSB.AppendLine(ConCat_pv("0", ContainerIds(EsaObjectType.Member2D)))
        oSB.AppendLine(ConCat_pv("1", ContainerTypes(EsaObjectType.Member2D)))
        oSB.appendline(ConCat_pv("3", averagingStrips(iAveragingStrip, 0))) ' 2D Member to check
        oSB.AppendLine("</row>")
        oSB.AppendLine(ConCat_closetable("0"))


        oSB.AppendLine(ConCat_pv("1", averagingStrips(iAveragingStrip, 1))) 'Name
        'oSB.AppendLine(ConCat_pv("2", integrationStrips(iIntegrationStrip, 1))) 'UniqueId
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.AveragingStripType)(3, averagingStrips(iAveragingStrip, 3)))  'Create Type
        oSB.AppendLine(ConCat_pvt_enum(Of Koala.AveragingStripDirection)(4, averagingStrips(iAveragingStrip, 4)))  ' Direction
        oSB.AppendLine(ConCat_pv("5", averagingStrips(iAveragingStrip, 5))) 'Width

        Dim x1 As String = averagingStrips(iAveragingStrip, 8)
        Dim y1 As String = averagingStrips(iAveragingStrip, 10)
        Dim z1 As String = averagingStrips(iAveragingStrip, 12)

        ' In case of Strip not a Point
        If averagingStrips(iAveragingStrip, 9) IsNot Nothing Then
            Dim x2 As String = averagingStrips(iAveragingStrip, 9)
            Dim y2 As String = averagingStrips(iAveragingStrip, 11)
            Dim z2 As String = averagingStrips(iAveragingStrip, 13)

            oSB.AppendLine(ConCat_pvx("8", x1, "0")) 'X
            oSB.AppendLine(ConCat_pvx("8", x2, "1")) 'X
            oSB.AppendLine(ConCat_pvx("9", y1, "0")) 'Y
            oSB.AppendLine(ConCat_pvx("9", y2, "1")) 'Y
            oSB.AppendLine(ConCat_pvx("10", z1, "0")) 'Z
            oSB.AppendLine(ConCat_pvx("10", z2, "1")) 'Z
        Else  ' In case of Point not Strip

            oSB.AppendLine(ConCat_pv("6", averagingStrips(iAveragingStrip, 6))) 'Length
            oSB.AppendLine(ConCat_pv("7", averagingStrips(iAveragingStrip, 7))) 'Angle
            oSB.AppendLine(ConCat_pv("8", x1)) 'X
            oSB.AppendLine(ConCat_pv("9", y1)) 'Y
            oSB.AppendLine(ConCat_pv("10", z1)) 'Z

        End If

        oSB.AppendLine("</obj>")

    End Sub



    Function WrapIfRequired(Filename As String) As String
        Dim containsSpaces = False
        Dim ContainsQuotes = False

        Dim FilePath = Filename.Trim
        If FilePath.Contains(" ") Then containsSpaces = True
        If FilePath.Substring(0, 1) = """" Then ContainsQuotes = True
        If FilePath.Substring(FilePath.Length - 1, 1) = """" And ContainsQuotes = False Then ContainsQuotes = True

        If containsSpaces = True And ContainsQuotes = False Then
            'Only wrap if it contains spaces in string and not already quoted
            Return """" & FilePath & """"
        Else
            Return FilePath
        End If
    End Function

    Public Function RunCalculationWithEsaXML(FileName As String, ESAXMLPath As String, CalcType As String, TemplateName As String, OutputFile As String, ByRef time_elapsed As Double, SESavedProject As String) As String

        Dim stopWatch As New System.Diagnostics.Stopwatch()


        'initialize stopwatch
        stopWatch.Start()

        Rhino.RhinoApp.WriteLine("")
        Rhino.RhinoApp.WriteLine("===== KOALA SCIA Engineer plugin - running analysis =====")
        Dim strOut As String
        'run ESA_XML
        '---------------------------------------------------
        Try
            Dim myProcess As New System.Diagnostics.Process
            Dim ESAXMLArgs As String
            Dim strErr As String, intExit As Integer

            Dim ExportType As String = ""
            Dim ExportTypeString As String

            ExportType = OutputFile.Split(".").Last
            Select Case ExportType
                Case "tst"
                    ExportTypeString = "-tTST -o"
                Case "xlsx"
                    ExportTypeString = "-sd -tXLSX -o"
                Case "rtf"
                    ExportTypeString = "-sd -tRTF -o"

                Case "pdf"
                    ExportTypeString = "-sd -tPDF -o"
                Case "dds"
                    ExportTypeString = "-sd -tDDS -o"
                Case "txt"
                    ExportTypeString = "-tTXT -o"
                Case "XML"
                    ExportTypeString = "-tXML -o"
                Case Else
                    ExportTypeString = "-tTXT -o"
            End Select
            '.\ESA_XML.exe LIN 'C:\TEMP\MakaraHala\SCIAtemplate.ESA' 'C:\TEMP\MakaraHala\KoalaHall.xml' -sd -tXLSX -oC:\TEMP\MakaraHala\Output.xlsx
            myProcess.StartInfo.FileName = ESAXMLPath

            TemplateName = WrapIfRequired(TemplateName)
            FileName = WrapIfRequired(FileName)

            If Not SESavedProject = "" Then
                Dim MultiProcesFile = System.IO.Path.GetDirectoryName(SESavedProject)
                MultiProcesFile += "\runParameters.txt"
                'FileCopy(TemplateName, SESavedProject)
                Dim Parameters = CalcType & " " & TemplateName & " " & FileName & " -tESA -o" & SESavedProject & Environment.NewLine
                If Not OutputFile = "" Then
                    Parameters += CalcType & " " & TemplateName & " " & FileName & " " & ExportTypeString & OutputFile & Environment.NewLine
                End If

                System.IO.File.WriteAllText(MultiProcesFile, Parameters)
                ESAXMLArgs = "CMD" & " " & MultiProcesFile
                'File.Delete(MultiProcesFile)
            Else
                ESAXMLArgs = CalcType & " " & TemplateName & " " & FileName
                If Not OutputFile = "" Then
                    ESAXMLArgs += " " & ExportTypeString & OutputFile
                End If
            End If





            myProcess.StartInfo.Arguments = ESAXMLArgs
            myProcess.StartInfo.UseShellExecute = False
            'myProcess.StartInfo.RedirectStandardOutput = True
            'myProcess.StartInfo.RedirectStandardError = True
            'myProcess.StartInfo.CreateNoWindow = True

            Rhino.RhinoApp.WriteLine("Starting SCIA Engineer...")
            Rhino.RhinoApp.WriteLine("Arguments: " & ESAXMLArgs)
            Rhino.RhinoApp.WriteLine("Please wait...")

            myProcess.Start()

            'Dim calculationOutputReport As StreamReader
            'calculationOutputReport = myProcess.StandardOutput
            'Dim calculationOutputReportString As String
            'calculationOutputReportString = calculationOutputReport.ReadToEnd()
            myProcess.WaitForExit()
            'Dim calculationLogFile As String
            'calculationLogFile = System.IO.Path.GetDirectoryName(OutputFile) + "\CalculationLog.txt"
            'System.IO.File.WriteAllText(calculationLogFile, calculationOutputReportString)
            'calculationLogFile = System.IO.Path.GetDirectoryName(OutputFile) + "\CalculationLog.txt"
            'System.IO.File.WriteAllText(calculationLogFile, calculationOutputReportString)
            intExit = myProcess.ExitCode
            Rhino.RhinoApp.Write("SCIA Engineer finished with exit code: " & intExit)
            'output anything that could come out of SCIA Engineer
            'standard out
            strOut = ""
            Select Case intExit
                Case 0
                    Rhino.RhinoApp.WriteLine(" - Succeeded")
                    strOut = " Calculation Succeeded"
                Case 1
                    Rhino.RhinoApp.WriteLine(" - Unable To initialize MFC")
                    strOut = "Unable To initialize MFC"
                Case 2
                    Rhino.RhinoApp.WriteLine(" - Missing arguments")
                    strOut = "Missing arguments in commandline"
                Case 3
                    Rhino.RhinoApp.WriteLine(" - Invalid arguments")
                    strOut = "Invalid arguments in command line"
                Case 4
                    Rhino.RhinoApp.WriteLine(" - Unable To open ProjectFile")
                    strOut = "Unable To open ProjectFile"
                Case 5
                    Rhino.RhinoApp.WriteLine(" - Calculation failed")
                    strOut = "Calculation failed"
                Case 6
                    Rhino.RhinoApp.WriteLine(" - Unable To initialize application environment")
                    strOut = "Unable To initialize application environment"
                Case 7
                    Rhino.RhinoApp.WriteLine(" - Error during update ProjectFile By XMLUpdateFile")
                    strOut = "Error during update ProjectFile By XMLUpdateFile"
                Case 8
                    Rhino.RhinoApp.WriteLine(" - Error during create export outputs")
                    strOut = "Error during create export outputs"
                Case 9
                    Rhino.RhinoApp.WriteLine(" - Error during create XML outputs")
                    strOut = "Error during create XML outputs"
                Case 99
                    Rhino.RhinoApp.WriteLine(" - Error during update ProjectFile By XLSX Update")
                    strOut = "Error during update ProjectFile By XLSX Update"
                Case Else
                    Rhino.RhinoApp.WriteLine(" - Unknown exit code")
                    strOut = "Unknown exit code"
            End Select


            'strOut = myProcess.StandardOutput.ReadToEnd
            If strOut Is "" Then
            Else
                Rhino.RhinoApp.WriteLine("SCIA Engineer output message: " & strOut)

            End If
            'standard error
            strErr = ""
            'strErr = myProcess.StandardOutput.ReadToEnd
            If strErr <> "" Then Rhino.RhinoApp.WriteLine("SCIA Engineer error message: " & strErr)

            ' DA.SetData(1, OutputFile)
        Catch ex As Exception
            Rhino.RhinoApp.WriteLine("Encountered error launching esa_xml.exe: " & ex.Message)
            strOut = "Encountered error launching esa_xml.exe: " & ex.Message
        End Try


        'stop stopwatch
        stopWatch.Stop()
        time_elapsed = stopWatch.ElapsedMilliseconds
        Rhino.RhinoApp.WriteLine("Done in " + Str(time_elapsed) + " ms.")
        Return strOut
    End Function

End Module
