Imports System.ComponentModel
Imports System.Reflection
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Parameters
Imports Grasshopper.Kernel.Types

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

            ' Get the private field "m_namedValues" using reflection
            Dim namedValuesField As FieldInfo = Me.GetType().BaseType.GetField("m_namedValues", BindingFlags.NonPublic Or BindingFlags.Instance)

            Public ReadOnly Property NamedValuesDictionary As Dictionary(Of Integer, String)
                Get
                    Dim dictionary As New Dictionary(Of Integer, String)()
                    ' Access the value of the private field
                    Dim namedValuesValue As Object = namedValuesField.GetValue(Me)

                    For Each namedValue In CType(namedValuesValue, IEnumerable)
                        dictionary.Add(namedValue.Value, namedValue.Name)
                    Next
                    Return dictionary
                End Get
            End Property

            Private Function CastFromString(ByVal s As String) As GH_Integer
                If HasNamedValues Then
                    Dim i As Integer
                    If Integer.TryParse(s, i) Then
                        Return New GH_Integer(i)
                    End If

                    Dim namedValues = NamedValuesDictionary
                    For Each nv In namedValues
                        If nv.Value.Equals(s, StringComparison.CurrentCultureIgnoreCase) Then
                            Return New GH_Integer(nv.Key)
                        End If
                    Next
                End If
                Return Nothing
            End Function

            Protected Overrides Function PreferredCast(ByVal data As Object) As GH_Integer
                ' Enable assigning the enum based on its string representation
                Dim s As String = Nothing

                If TypeOf data Is String Then
                    s = DirectCast(data, String)
                ElseIf TypeOf data Is GH_String Then
                    Dim s_goo As GH_String = DirectCast(data, GH_String)
                    s = s_goo.Value
                ElseIf TypeOf data Is Integer Then
                    Return New GH_Integer(CInt(data))
                End If

                If s IsNot Nothing Then
                    Dim result = CastFromString(s)
                    If result IsNot Nothing Then
                        Return result
                    End If
                End If

                Return MyBase.PreferredCast(data)
            End Function
        End Class

        'Public Sub AddEnumOptions(Of T)(param As Param_Integer)
        '    For Each item As [Enum] In [Enum].GetValues(GetType(T))
        '        Dim s As String = GetEnumDescription(item)
        '        param.AddNamedValue(s, Convert.ToInt32(item))
        '        param.Description += " | " & s & "=" & Convert.ToInt32(item).ToString()
        '    Next
        'End Sub

        Public Function GetEnumDescription(ByVal e As [Enum]) As String
            If Not [Enum].IsDefined(e.GetType(), e) Then
                Throw New ArgumentOutOfRangeException($"{e.GetType().Name} value {e} is invalid")
            End If

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

    Public Enum CementClass
        <Description("S (slow hardening - CEM 32,5 N)")>
        S = 0
        <Description("N (normal hardening - CEM 32,5 R, CEM 42,5 N)")>
        N = 1
        <Description("R (rapidl hardening - CEM 42,5 R, CEM 52,5 N, CEM 52,5 R)")>
        R = 2
    End Enum

    Public Enum CoordinateDefinition
        Abso = 0
        Rela = 1
    End Enum

    Public Enum CoordSystem
        GCS = 0
        LCS = 1
    End Enum

    Public Enum CoordSystemNodeSupport
        GCS = 0
        <Description("LCS of node")>
        LCS = 1
    End Enum

    Public Enum CoordSystemFreeLoad
        <Description("GCS - Length")>
        GCSLength = 0
        <Description("GCS - Projection")>
        GCSProjection = 1
        <Description("Member LCS")>
        MemberLCS = 2
    End Enum

    Public Enum CoordSystemFreePointLoad
        <Description("GCS")>
        GCS = 0
        <Description("Member LCS")>
        MemberLCS = 1
        <Description("Load LCS")>
        LoadLCS = 2
    End Enum

    Public Enum DegreeOfFreedomHinge
        Free = 0
        Rigid = 1
        Flexible = 2
        Nonlinear = 3
    End Enum

    Public Enum DegreeOfFreedomSupport
        Free = 0
        Rigid = 1
        Flexible = 2
        <Description("Nonlinear")>
        Nonlinear = 7
    End Enum

    Public Enum DegreeOfFreedomForTranslation
        Free = 0
        Rigid = 1
        Flexible = 2
        <Description("Rigid press only")>
        RigidPressOnly = 3
        <Description("Rigid tension only")>
        RigidTensionOnly = 4
        <Description("Flexible press only")>
        FlexiblePressOnly = 5
        <Description("Flexible tension only")>
        FlexibleTensionOnly = 6
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
        <Description("Dir X")>
        DirectionX = 1
        <Description("Dir Y")>
        DirectionY = 2
        <Description("3 points")>
        ThreePoints = 3
    End Enum

    Public Enum DrawDiagram
        <Description("Upright to element")>
        UprightToElement = 0
        <Description("Element plane")>
        ElementPlane = 1
        <Description("X direction")>
        XDirection = 2
        <Description("Y direction")>
        YDirection = 3
        <Description("Z direction")>
        ZDirection = 4
        <Description("Draw similar as for setting in service properties")>
        ServiceProperties = 5
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

    Enum EsaObjectCategory
        Project
        Tools
        Library
        Structure0D
        Structure1D
        Structure2D
        BoundaryCondition
        LoadCase
        MassGroup
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
        Undefined = -1

        ProjectData
        MeshSetup
        Layer
        Material
        CrossSection
        Selection

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
        ResultClass

        MassGroup
        MassCombination
        SeismicSpectrum

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

        SectionOn1D
        SectionOn2D
    End Enum

    Public Enum LoadCaseType
        SW = 0
        Permanent = 1
        Variable = 2
        Seismic = 3
    End Enum

    Public Enum LoadGroupLoadType
        <Description("Permanent")>
        Permanent = 0
        <Description("Variable")>
        Variable = 1
        <Description("Accidental")>
        Accidental = 2
        <Description("Seismic")>
        Seismic = 3
    End Enum

    Public Enum LoadGroupRelation
        Undefined = -1
        <Description("Standard")>
        Standard = 0
        <Description("Exclusive")>
        Exclusive = 1
        <Description("Together")>
        Together = 2
    End Enum

    Public Enum LoadGroupVariableLoadType
        Undefined = -1
        <Description("Cat A : Domestic")>
        CatA = 0
        <Description("Cat B : Offices")>
        CatB = 1
        <Description("Cat C : Congregation")>
        CatC = 2
        <Description("Cat D : Shopping")>
        CatD = 3
        <Description("Cat E : Storage")>
        CatE = 4
        <Description("Cat F : Vehicle <30kN")>
        CatF = 5
        <Description("Cat G : Vehicle >30kN")>
        CatG = 6
        <Description("Cat H : Roofs")>
        CatH = 7
        <Description("Snow")>
        Snow = 8
        <Description("Wind")>
        Wind = 11
        <Description("Temperature")>
        Temperature = 12
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

    Public Enum MaterialType
        Undefined
        Concrete_EC_EN1
        Steel_EC1
        <Description("Reinforcement steel")>
        Reinforcement_EC_EN1
        'Timber_EC1
        'Aluminium_EN1
        'Masonry_EN1
        '<Description("Steel fibre concrete")>
        'SteelFibreConcrete_EC1
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

    Public Enum MomentDirection
        Mx = 0
        My = 1
        Mz = 2
    End Enum

    Public Enum NLFunctionType
        <Description("Translation")>
        Translation = 0
        <Description("Rotation")>
        Rotation = 1
        <Description("Nonlinear subsoil")>
        NLSubsoil = 2
    End Enum

    Public Enum NLFunctionEndType
        Rigid = 0
        Free = 1
        Flexible = 2
    End Enum

    Public Enum Origin
        <Description("From start")>
        fromStart = 0
        <Description("From end")>
        fromEnd = 1
    End Enum

    Public Enum ProjectMaterialType
        Concrete
        Steel
        Timber
        Aluminium
        Masonry
        SteelFibreConcrete
        Other
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

    Public Enum TypeOfAggregate
        <Description("Quartzite")>
        Quartzite = 1
        <Description("Limestone")>
        Limestone = 2
        <Description("Sandstone")>
        Sandstone = 3
        <Description("Basalt")>
        Basalt = 4
    End Enum

    Public Enum TypeOfDiagram
        <Description("Bi-linear stress-strain diagram")>
        BiLinear = 1
        <Description("Parabola-rectangle stress-strain diagram")>
        ParabolaRectangle = 2
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
End Namespace

