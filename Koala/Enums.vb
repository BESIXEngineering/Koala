Imports System.ComponentModel
Imports System.Reflection
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Parameters

Namespace Koala
    Module EnumExtensions

        Public Class Param_Enum
            Inherits Param_Integer

            Public Sub New(_name As String, _description As String, _access As GH_ParamAccess, _default As [Enum], Optional setPersistant As Boolean = True)
                MyBase.New()
                Name = _name
                NickName = _name
                Description = _description
                Access = _access

                For Each item As [Enum] In [Enum].GetValues(_default.GetType)
                    Dim s As String = GetEnumDescription(item)
                    AddNamedValue(s, Convert.ToInt32(item))
                    Description += " | " & s & "=" & Convert.ToInt32(item).ToString()
                Next
                If setPersistant Then
                    SetPersistentData(Convert.ToInt32(_default))
                End If
            End Sub
        End Class

        'Public Sub AddEnumOptions(Of T)(param As Param_Integer)
        '    For Each item As [Enum] In [Enum].GetValues(GetType(T))
        '        Dim s As String = GetEnumDescription(item)
        '        param.AddNamedValue(s, Convert.ToInt32(item))
        '        param.Description += " | " & s & "=" & Convert.ToInt32(item).ToString()
        '    Next
        'End Sub

        Public Function GetEnumDescription(ByVal e As [Enum]) As String
            Dim fi As FieldInfo = e.GetType().GetField(e.ToString())
            Dim attr() As DescriptionAttribute =
                          DirectCast(fi.GetCustomAttributes(GetType(DescriptionAttribute),
                          False), DescriptionAttribute())

            If attr.Length > 0 Then
                Return attr(0).Description
            Else
                Return e.ToString()
            End If
        End Function

        ''' <summary>
        ''' Return the enum value of a given type matching a string.
        ''' The string can represent the integer value of the enum, its name or its description.
        ''' A case-insensitive match is performed.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Function GetEnum(Of T)(ByVal name As String) As [Enum]
            ' Check if string represents an integer.
            ' If so, return the matching enum.
            ' If not find the enum with Name or Description matching the given name.
            Dim i As Integer
            name = name.Trim
            If Not String.IsNullOrEmpty(name) AndAlso Integer.TryParse(name, i) Then
                If [Enum].IsDefined(GetType(T), i) Then
                    Return [Enum].Parse(GetType(T), name)
                End If
            Else
                name = name.ToLower
                For Each item As [Enum] In [Enum].GetValues(GetType(T))
                    If name = item.ToString.ToLower Then Return item
                    Dim descr As String = GetEnumDescription(item)
                    If name = descr.ToLower Then Return item
                Next
            End If
            Throw New ArgumentException(String.Format("Invalid {0}: '{1}'", GetType(T).Name, name))
            'Return [Enum].GetValues(GetType(T)).Cast(Of [Enum]).First
        End Function

        ''' <summary>
        ''' Check whether a string value matches an Enum.
        ''' Match is true if the string value matches the Enum's integer value, name or description.
        ''' A case-insensitive match is performed.
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="enumValue"></param>
        ''' <returns></returns>
        Public Function MatchesEnum(ByVal value As String, ByVal enumValue As [Enum]) As Boolean
            Dim i As Integer
            value = value.Trim
            If Not String.IsNullOrEmpty(value) AndAlso Integer.TryParse(value, i) Then
                MatchesEnum = Convert.ToInt32(enumValue) = i
            Else
                value = value.ToLower
                MatchesEnum = (value = enumValue.ToString.ToLower) OrElse (value = GetEnumDescription(enumValue).ToLower)
            End If
        End Function
    End Module

    Public Enum AnalysisModelStructuralType
        <Description("Beam")>
        Beam = 0
        <Description("Truss XZ")>
        TrussXZ = 1
        <Description("Frame XZ")>
        FrameXZ = 2
        <Description("Truss XYZ")>
        TrussXYZ = 3
        <Description("Frame XYZ")>
        FrameXYZ = 4
        <Description("Grid XY")>
        GridXY = 5
        <Description("Plate XY")>
        PlateXY = 6
        <Description("Wall XY")>
        WallXY = 7
        <Description("General XYZ")>
        GeneralXYZ = 8
    End Enum

    Public Enum ArbitraryProfileCoordDefinition
        Abso = 0
        Rela = 1
    End Enum

    Public Enum ArbitraryProfileAlignment
        <Description("default")>
        Undefined = 0
        <Description("centre line")>
        CentreLine = 1
        <Description("top surface")>
        TopSurface = 2
        <Description("bottom surface")>
        BottomSurface = 3
        <Description("left surface")>
        LeftSurface = 4
        <Description("right surface")>
        RightSurface = 5
        <Description("top left")>
        TopLeft = 6
        <Description("top right")>
        TopRight = 7
        <Description("bottom left")>
        BottomLeft = 8
        <Description("bottom right")>
        BottomRight = 9
    End Enum

    Public Enum ArbitraryProfileSpanType
        <Description("prismatic")>
        Prismatic = 0
        <Description("param. haunch")>
        ParametricHaunch = 1
        <Description("two Css")>
        TwoCss = 2
    End Enum

    Public Enum BeamEnd
        <Description("Begin")>
        NodeAtStart = 0
        <Description("End")>
        NodeAtEnd = 1
        <Description("Both")>
        Both = 2
    End Enum

    Public Enum BeamFEMType
        <Description("standard")>
        Standard = 0
        <Description("axial force only")>
        AxialForceOnly = 1
    End Enum

    Public Enum BeamType
        <Description("general")>
        General = 0
        <Description("beam")>
        Beam = 1
        <Description("column")>
        Column = 2
        <Description("gable column")>
        GableColumn = 3
        <Description("secondary column")>
        SecondaryColumn = 4
        <Description("rafter")>
        Rafter = 5
        <Description("purlin")>
        Purlin = 6
        <Description("roof bracing")>
        RoofBracing = 7
        <Description("wall bracing")>
        WallBracing = 8
        <Description("girt")>
        Girt = 9
        <Description("truss chord")>
        TrussChord = 10
        <Description("truss diagonal")>
        TrussDiagonal = 11
        <Description("plate rib")>
        PlateRib = 12
        <Description("beam slab")>
        BeamSlab = 13
    End Enum

    Public Enum CoordSystemFreeLoad
        <Description("GCS - Length")>
        GCSLength = 0
        <Description("GCS - Projection")>
        GCSProjection = 1
        <Description("Member LCS")>
        MemberLCS = 2
    End Enum

    Public Enum DegreeOfFreedom
        Free = 0
        Rigid = 1
        Flexible = 2
        <Description("Nonlinear")>
        Nonlinear = 7
    End Enum

    Public Enum Direction
        X = 0
        Y = 1
        Z = 2
    End Enum

    Public Enum DistributionOfLineLoad
        Uniform = 0
        Trapez = 1
    End Enum

    Public Enum DistributionOfSurfaceLoad
        Uniform = 0
        DirectionX = 1
        DirectionY = 2
    End Enum


    Enum EsaObjectCategory
        Project
        Library
        Structure0D
        Structure1D
        Structure2D
        BoundaryCondition
        LoadCase
        PointLoad
        LineLoad
        SurfaceLoad
        ThermalLoad
        NonLinear
    End Enum

    ''' <summary>
    ''' Different types of ESA objects that can be created using Koala
    ''' </summary>
    Public Enum EsaObjectType
        ProjectData
        MeshSetup
        Layer
        CrossSection

        Node
        <Description("1D Member")>
        Member1D
        <Description("2D Member")>
        Member2D
        LoadPanel
        Opening
        ArbitraryProfile
        InternalNode1D
        InternalEdge2D

        NodeSupport
        SurfaceEdgeSupport
        BeamLineSupport
        BeamPointSupport
        SurfaceSupport
        Subsoil

        Hinge
        LineHinge
        CrossLink
        RigidArm

        LoadCase
        LoadGroup
        LinearCombination
        NonLinearCombination
        StabilityCombination

        PointLoadNode
        PointMomentNode
        PointLoadBeam
        PointMomentBeam
        FreePointLoad
        FreePointMoment

        LineLoadBeam
        LineMomentBeam
        LineLoadEdge
        LineMomentEdge
        FreeLineLoad

        SurfaceLoad
        FreeSurfaceLoad

        ThermalLoad1D
        ThermalLoad2D

        NonLinearFunction

        PreTensionElement
        GapElement
        LimitForceElement
        Cable

        IntegrationStrip
        AveragingStrip

        SectionOnBeam
    End Enum

    Public Enum LoadPanelType
        <Description("To panel nodes")>
        ToPanelNode = 0
        <Description("To panel edges")>
        ToPanelEdges = 1
        <Description("To panel edges and beams")>
        ToPanelEdgesAndBeams = 2
    End Enum

    Public Enum LoadPanelTransferDirection
        <Description("X (LCS panel)")>
        X = 0
        <Description("Y (LCS panel)")>
        Y = 1
        <Description("all (LCS panel)")>
        Both = 2
    End Enum

    Public Enum LoadPanelTransferMethod
        <Description("Accurate(FEM),fixed link with beams")>
        AccurateFixed = 0
        <Description("Standard")>
        Standard = 1
        <Description("Accurate(FEM),hinged link with beams")>
        AccurateHinged = 2
        <Description("Tributary area")>
        TributaryArea = 3
    End Enum

    Public Enum Material
        Concrete
        Steel
        Timber
        Aluminium
        Masonry
        SteelFibreConcrete
        Other
    End Enum

    Public Enum MemberSystemLine
        <Description("Centre")>
        Centre = 1
        <Description("Top")>
        Top = 2
        <Description("Bottom")>
        Bottom = 4
        <Description("Left")>
        Left = 8
        <Description("Top left")>
        TopLeft = 10
        <Description("Bottom left")>
        BottomLeft = 12
        <Description("Right")>
        Right = 16
        <Description("Top right")>
        TopRight = 18
        <Description("Bottom right")>
        BottomRight = 20
    End Enum

    Public Enum MemberSystemPlane
        Centre = 1
        Top = 2
        Bottom = 4
    End Enum

    Public Enum Selection
        Auto = 0
        [Select] = 1
    End Enum

    Public Enum SlabFEMType
        <Description("none")>
        None = 0
        <Description("Press only")>
        PressOnly = 1
        <Description("Membrane")>
        Membrane = 2
    End Enum

    Public Enum UILanguage
        <Description("English")>
        EN = 0
        <Description("Nederlands")>
        NL = 1
        <Description("Français")>
        FR = 2
        <Description("Deutsch")>
        DE = 3
        <Description("Čeština")>
        CZ = 4
        <Description("Slovenčina")>
        SI = 5
    End Enum

    Public Enum Validity
        <Description("All")>
        All = 0
        <Description("-Z")>
        ZNeg = 1
        <Description("+Z")>
        ZPos = 2
        <Description("From-to")>
        FromTo = 3
        <Description("Z=0")>
        ZZero = 4
        <Description("-Z (incl. 0)")>
        ZNegOrZero = 5
        <Description("+Z (incl. 0)")>
        ZPosOrZero = 6
    End Enum

    Public Enum EffectiveWidthGeometry
        <Description("Constant symmeric")>
        symm = 0
        <Description("Constant nonsymmetric")>
        nonsymm = 1
    End Enum


    Public Enum EffectiveWidthDefinition
        <Description("Width")>
        width = 0
        <Description("Number of thickness")>
        numberOfThickness = 1
    End Enum

    Public Enum Origin
        <Description("From start")>
        fromStart = 0
        <Description("From end")>
        fromEnd = 1
    End Enum

    Public Enum AveragingStripType
        <Description("Strip")>
        strip = 0
        <Description("Point")>
        point = 1
    End Enum

    Public Enum AveragingStripDirection
        <Description("longitudinal")>
        longitudinal = 0
        <Description("perpendicular")>
        perpendicular = 1
        <Description("both")>
        both = 2
        <Description("none")>
        node = 3
    End Enum

End Namespace

