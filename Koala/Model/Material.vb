Imports System.Text.RegularExpressions
Imports Koala.Koala

Public Class Material
    Public Enum MaterialProperty
        Density
        E
        G
        Poisson
        ThermalExpansion
        ThermalConductivity
        SpecificHeat
        DiagramType
        ' Steel specific
        YieldStrength
        UltimateStrength
        ' Rebar specific
        ReinforcementClass
        ' Concrete specific
        CompressiveStrength
        CementClass
        AggregateType
        AggregateSize
    End Enum

    Private Enum UnitType
        None
        Length
        Mass
        Time
        Temperature
        Pressure
        Density
        CoefficientOfThermalExpansion
        ThermalConductivity
        SpecificEntropy
    End Enum

    Public Name As String
    Public Type As Koala.MaterialType
    Public Properties As Dictionary(Of MaterialProperty, String)

    Public Sub New(serialized As String)
        Name = Nothing
        Type = Koala.MaterialType.Undefined
        Properties = New Dictionary(Of MaterialProperty, String)

        Dim splittingChars As Char() = {","c, ";"c}
        Dim equalityChars As Char() = {"="c, ":"c}

        For Each prop In serialized.Split(splittingChars)
            Dim parts As String() = prop.Split(equalityChars, 2)
            Dim key As String = parts(0).Trim().ToLower()
            Dim value As String = parts(1).Trim()

            If key = "name" Then
                Name = value

            ElseIf key = "type" Or key = "materialtype" Then
                If value.ToLower = "steel" Then
                    Type = Koala.MaterialType.Steel_EC1
                ElseIf value.ToLower = "concrete" Then
                    Type = Koala.MaterialType.Concrete_EC_EN1
                Else
                    Type = CType([Enum].Parse(GetType(Koala.MaterialType), value), Koala.MaterialType)
                End If

            ElseIf key = "density" Or key = "rho" Then
                Properties(MaterialProperty.Density) = GetSIValue(value, UnitType.Density)

            ElseIf key = "elasticmodulus" Or key = "e" Or key = "emodulus" Then
                Properties(MaterialProperty.E) = GetSIValue(value, UnitType.Pressure)

            ElseIf key = "shearmodulus" Or key = "g" Or key = "gmodulus" Then
                Properties(MaterialProperty.G) = GetSIValue(value, UnitType.Pressure)

            ElseIf key = "poissonratio" Or key = "poisson" Or key = "ν" Or key = "nu" Then
                Properties(MaterialProperty.Poisson) = GetSIValue(value, UnitType.None)

            ElseIf key = "thermalexpansion" Or key = "alpha" Or key = "α" Then
                Properties(MaterialProperty.ThermalExpansion) = GetSIValue(value, UnitType.CoefficientOfThermalExpansion)

            ElseIf key = "thermalconductivity" Or key = "lambda" Or key = "λ" Then
                Properties(MaterialProperty.ThermalConductivity) = GetSIValue(value, UnitType.ThermalConductivity)

            ElseIf key = "specificheat" Or key = "cp" Then
                Properties(MaterialProperty.SpecificHeat) = GetSIValue(value, UnitType.SpecificEntropy)

            ElseIf key = "yieldstrength" Or key = "fy" Or key = "fyk" Or key = "f_yk" Then
                Properties(MaterialProperty.YieldStrength) = GetSIValue(value, UnitType.Pressure)
                If Type = Koala.MaterialType.Undefined Then Type = Koala.MaterialType.Steel_EC1

            ElseIf key = "ultimatestrength" Or key = "fu" Or key = "fuk" Or key = "f_uk" Then
                Properties(MaterialProperty.UltimateStrength) = GetSIValue(value, UnitType.Pressure)
                If Type = Koala.MaterialType.Undefined Then Type = Koala.MaterialType.Steel_EC1

            ElseIf key = "compressivestrength" Or key = "fc" Or key = "fck" Or key = "f_ck" Then
                Properties(MaterialProperty.CompressiveStrength) = GetSIValue(value, UnitType.Pressure)
                If Type = Koala.MaterialType.Undefined Then Type = Koala.MaterialType.Concrete_EC_EN1

            ElseIf key = "aggregatesize" Or key = "stonediameter" Then
                Properties(MaterialProperty.AggregateSize) = GetSIValue(value, UnitType.Length)
                If Type = Koala.MaterialType.Undefined Then Type = Koala.MaterialType.Concrete_EC_EN1

            ElseIf key = "aggregatetype" Then
                If value.StartsWith("quartzite", StringComparison.InvariantCultureIgnoreCase) Then
                    Properties(MaterialProperty.AggregateType) = TypeOfAggregate.Quartzite.ToString
                ElseIf value.StartsWith("limestone", StringComparison.InvariantCultureIgnoreCase) Then
                    Properties(MaterialProperty.AggregateType) = TypeOfAggregate.Limestone.ToString
                ElseIf value.StartsWith("sandstone", StringComparison.InvariantCultureIgnoreCase) Then
                    Properties(MaterialProperty.AggregateType) = TypeOfAggregate.Sandstone.ToString
                ElseIf value.StartsWith("basalt", StringComparison.InvariantCultureIgnoreCase) Then
                    Properties(MaterialProperty.AggregateType) = TypeOfAggregate.Basalt.ToString
                Else
                    Throw New ArgumentException("Unsupported aggregate type: " & value)
                End If
                If Type = Koala.MaterialType.Undefined Then Type = Koala.MaterialType.Concrete_EC_EN1

            ElseIf key = "cementclass" Then
                If value.ToLower = "s" Then
                    Properties(MaterialProperty.CementClass) = CementClass.S.ToString
                ElseIf value.ToLower = "n" Then
                    Properties(MaterialProperty.CementClass) = CementClass.N.ToString
                ElseIf value.ToLower = "r" Then
                    Properties(MaterialProperty.CementClass) = CementClass.R.ToString
                Else
                    Throw New ArgumentException("Unsupported cement class: " & value)
                End If
                If Type = Koala.MaterialType.Undefined Then Type = Koala.MaterialType.Concrete_EC_EN1

            ElseIf key = "diagramtype" Then
                If value.ToLower = "bilinear" Then
                    Properties(MaterialProperty.DiagramType) = TypeOfDiagram.BiLinear
                ElseIf value.ToLower = "parabolic" Then
                    Properties(MaterialProperty.DiagramType) = TypeOfDiagram.ParabolaRectangle
                Else
                    Throw New ArgumentException("Unsupported diagram type: " & value)
                End If
                If Type = Koala.MaterialType.Undefined Then Type = Koala.MaterialType.Concrete_EC_EN1

            Else
                Rhino.RhinoApp.WriteLine("Unknown material property: " & key & " = " & value)
            End If
        Next

        If String.IsNullOrWhiteSpace(Name) Then
            Throw New ArgumentException("Material name not defined")
        End If

        If Type = Koala.MaterialType.Undefined Then
            Throw New ArgumentException("Material type not defined: " & Name)
        End If

    End Sub

    Private Function GetSIValue(value As String, unitType As UnitType) As String
        Dim number As Double

        If Double.TryParse(value, number) Then
            Return value
        ElseIf unitType = UnitType.None Then
            Throw New ArgumentException("Invalide double: " & value)
        End If

        Dim patternFormat As String = "^\s*([+-]?\d(?>[\deE\.+-]*[\d\.])?)\s*({0})\s*$"

        If unitType = UnitType.Pressure Then
            Dim match As Match = Regex.Match(value, String.Format(patternFormat, "Pa|kPa|MPa|GPa"))
            If match.Success Then
                number = Double.Parse(match.Groups(1).Value)
                Dim unit As String = match.Groups(2).Value

                If unit = "Pa" Then
                    Return number.ToString()
                ElseIf unit = "kPa" Then
                    Return (number * 1000.0).ToString()
                ElseIf unit = "MPa" Then
                    Return (number * 1000000.0).ToString()
                ElseIf unit = "GPa" Then
                    Return (number * 1000000000.0).ToString()
                Else
                    Throw New ArgumentException("Unsupported pressure unit: " & unit)
                End If
            Else
                Throw New ArgumentException("Unsupported pressure: " & value)
            End If
        End If

        If unitType = UnitType.Length Then
            Dim match As Match = Regex.Match(value, String.Format(patternFormat, "m|cm|mm"))
            If match.Success Then
                number = Double.Parse(match.Groups(1).Value)
                Dim unit As String = match.Groups(2).Value

                If unit = "m" Then
                    Return number.ToString()
                ElseIf unit = "cm" Then
                    Return (number * 0.01).ToString()
                ElseIf unit = "mm" Then
                    Return (number * 0.001).ToString()
                Else
                    Throw New ArgumentException("Unsupported length unit: " & unit)
                End If
            Else
                Throw New ArgumentException("Unsupported length: " & value)
            End If
        End If

        If unitType = UnitType.Density Then
            Dim match As Match = Regex.Match(value, String.Format(patternFormat, "kg/m³"))
            If match.Success Then
                number = Double.Parse(match.Groups(1).Value)
                Dim unit As String = match.Groups(2).Value

                If unit = "kg/m³" Then
                    Return number.ToString()
                Else
                    Throw New ArgumentException("Unsupported density unit: " & unit)
                End If
            Else
                Throw New ArgumentException("Unsupported density: " & value)
            End If
        End If

        If unitType = UnitType.CoefficientOfThermalExpansion Then
            Dim match As Match = Regex.Match(value, String.Format(patternFormat, "1/K|1/°C|K⁻¹|°C⁻¹"))
            If match.Success Then
                number = Double.Parse(match.Groups(1).Value)
                Dim unit As String = match.Groups(2).Value

                If unit = "K⁻¹" Or unit = "°C⁻¹" Or unit = "1/K" Or unit = "1/°C" Then
                    Return number.ToString()
                Else
                    Throw New ArgumentException("Unsupported coefficient of thermal expansion unit: " & unit)
                End If
            Else
                Throw New ArgumentException("Unsupported coefficient of thermal expansion: " & value)
            End If
        End If

        If unitType = UnitType.ThermalConductivity Then
            Dim match As Match = Regex.Match(value, String.Format(patternFormat, "W/m·?K"))
            If match.Success Then
                number = Double.Parse(match.Groups(1).Value)
                Dim unit As String = match.Groups(2).Value

                If unit = "W/mK" Or unit = "W/m·K" Then
                    Return number.ToString()
                Else
                    Throw New ArgumentException("Unsupported thermal conductivity unit: " & unit)
                End If
            Else
                Throw New ArgumentException("Unsupported thermal conductivity: " & value)
            End If
        End If

        If unitType = UnitType.SpecificEntropy Then
            Dim match As Match = Regex.Match(value, String.Format(patternFormat, "k?J/k?g·?\.?K|k?J/k?g·?\.?°?C"))
            If match.Success Then
                number = Double.Parse(match.Groups(1).Value)
                Dim unit As String = match.Groups(2).Value

                If unit.StartsWith("J/kg") Then
                    Return number.ToString()

                ElseIf unit.StartsWith("kJ/kg") Then
                    Return (number * 1000.0).ToString()

                ElseIf unit.StartsWith("J/g") Then
                    Return (number * 1000.0).ToString()

                ElseIf unit.StartsWith("kJ/g") Then
                    Return (number * 1000000.0).ToString()
                Else
                    Throw New ArgumentException("Unsupported specific entropy unit: " & unit)
                End If
            Else
                Throw New ArgumentException("Unsupported specific entropy: " & value)
            End If
        End If

        Throw New ArgumentException("Unsupported unit type: " & unitType.ToString)

    End Function

End Class
