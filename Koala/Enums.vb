Imports System.ComponentModel
Imports System.Reflection
Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Parameters

Namespace Koala
    Module EnumExtensions

        Public Class Param_Enum
            Inherits Param_Integer

            Public Sub New(_name As String, _description As String, _access As GH_ParamAccess, _default As [Enum])
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
                SetPersistentData(Convert.ToInt32(_default))

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

        Public Function GetEnum(Of T)(ByVal name As String) As [Enum]
            For Each item As [Enum] In [Enum].GetValues(GetType(T))
                If name = item.ToString Then Return item
                Dim descr As String = GetEnumDescription(item)
                If name = descr Then Return item
            Next
            Throw New ArgumentException(String.Format("Invalid {0}: '{1}'", GetType(T).Name, name))
            'Return [Enum].GetValues(GetType(T)).Cast(Of [Enum]).First
        End Function
    End Module


    Public Enum ArbitraryProfileCoordDefinition
        Abso = 0
        Rela = 1
    End Enum

    Public Enum ArbitraryProfileAlignment
        Undefined = 0  'default
        CentreLine = 1
        TopSurface = 2
        BottomSurface = 3
        LeftSurface = 4
        RightSurface = 5
        TopLeft = 6
        TopRight = 7
        BottomLeft = 8
        BottomRight = 9
    End Enum

    Public Enum ArbitraryProfileSpanType
        Prismatic = 0
        ParametricHaunch = 1
        TwoCss = 2
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

