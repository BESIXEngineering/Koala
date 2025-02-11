﻿Imports System.Collections.Generic

Imports Grasshopper.Kernel
Imports Grasshopper.Kernel.Parameters
Imports Rhino.Geometry


Namespace Koala

    Public Class LimitForceLocalBeamNL
        Inherits GH_KoalaComponent
        ''' <summary>
        ''' Each implementation of GH_Component must provide a public 
        ''' constructor without any arguments.
        ''' Category represents the Tab in which the component will appear, 
        ''' Subcategory the panel. If you use non-existing tab or panel names, 
        ''' new tabs/panels will automatically be created.
        ''' </summary>
        Public Sub New()
            MyBase.New("LimitForceLocalBeamNL", "LimitForceLocalBeamNL",
                "LimitForceLocalBeamNL description",
                "Structure", New EsaObjectType() {EsaObjectType.LimitForceElement})
        End Sub

        Public Overrides ReadOnly Property Exposure As GH_Exposure
            Get
                Return GH_Exposure.quinary
            End Get
        End Property

        ''' <summary>
        ''' Registers all the input parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterInputParams(pManager As GH_Component.GH_InputParamManager)
            pManager.AddTextParameter("BeamList", "BeamList", "List of beams where to apply gap", GH_ParamAccess.list)
            pManager.AddIntegerParameter("Direction", "Direction", "Direction: Limit compression, Limit tension ", GH_ParamAccess.item, 0)
            AddOptionsToMenuBeamNLLimnitForceDirection(pManager.Param(1))
            pManager.AddIntegerParameter("Type", "Type", "Type: Buckling ( results zero ), Plastic yielding", GH_ParamAccess.item, 1)
            AddOptionsToMenuBeamNLLimitForceType(pManager.Param(2))
            pManager.AddNumberParameter("Marginal force [kN]", "Marginal force [kN]", "Marginal force [kN]", GH_ParamAccess.item, 100)


        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("LimitForceElement", "LimitForceElement", "LimitForceElement data", GH_ParamAccess.list)
        End Sub

        ''' <summary>
        ''' This is the method that actually does the work.
        ''' </summary>
        ''' <param name="DA">The DA object can be used to retrieve data from input parameters and 
        ''' to store data in output parameters.</param>
        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
            Dim i As Integer
            Dim Type As String = "Plastic yielding"
            Dim BeamList = New List(Of String)
            Dim MarginalForce As Double = 100
            Dim Direction As String = "Limit compression"

            If (Not DA.GetDataList(Of String)(0, BeamList)) Then Return
            DA.GetData(Of Integer)(1, i)
            Direction = GetStringFromBeamNLLimnitForceDirection(i)
            DA.GetData(Of Integer)(2, i)
            Type = GetStringFromBeamNLLimitForceType(i)
            DA.GetData(Of Double)(3, MarginalForce)

            Dim SE_LimitForce(BeamList.Count, 4) As String
            Dim FlatList As New List(Of System.Object)()
            'a section consists of: Profile name, section definition, material

            Dim item As String
            Dim itemcount As Long

            'initialize some variables
            itemcount = 0

            'identify section information in the strings
            For Each item In BeamList
                SE_LimitForce(itemcount, 0) = Strings.Trim(item)
                SE_LimitForce(itemcount, 1) = Direction
                SE_LimitForce(itemcount, 2) = Type
                SE_LimitForce(itemcount, 3) = MarginalForce * 1000
                itemcount += 1
            Next

            'Flatten data for export as simple list
            FlatList.Clear()

            For i = 0 To itemcount - 1
                FlatList.Add(SE_LimitForce(i, 0))
                FlatList.Add(SE_LimitForce(i, 1))
                FlatList.Add(SE_LimitForce(i, 2))
                FlatList.Add(SE_LimitForce(i, 3))
            Next i
            DA.SetDataList(0, FlatList)
        End Sub


        ''' <summary>
        ''' Provides an Icon for every component that will be visible in the User Interface.
        ''' Icons need to be 24x24 pixels.
        ''' </summary>
        Protected Overrides ReadOnly Property Icon() As System.Drawing.Bitmap
            Get
                'You can add image files to your project resources and access them like this:
                ' return Resources.IconForThisComponent;
                Return My.Resources.LimitForceBeamNL
            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("fd4d9fdc-4e5c-4b37-902e-bcf088ed4a5e")
            End Get
        End Property

        Private Sub AddOptionsToMenuBeamNLLimitForceType(menuitem As Param_Integer)
            menuitem.AddNamedValue("Buckling ( results zero )", 0)
            menuitem.AddNamedValue("Plastic yielding", 1)

        End Sub

        Private Function GetStringFromBeamNLLimitForceType(item As Integer) As String
            Select Case item
                Case 0
                    Return "Buckling ( results zero )"
                Case 1
                    Return "Plastic yielding"
                Case Else
                    Return "Buckling ( results zero )"
            End Select
        End Function

        Private Sub AddOptionsToMenuBeamNLLimnitForceDirection(menuitem As Param_Integer)
            menuitem.AddNamedValue("Limit compression", 0)
            menuitem.AddNamedValue("Limit tension", 1)

        End Sub

        Private Function GetStringFromBeamNLLimnitForceDirection(item As Integer) As String
            Select Case item
                Case 0
                    Return "Limit compression"
                Case 1
                    Return "Limit tension"
                Case Else
                    Return "Limit compression"
            End Select
        End Function
    End Class

End Namespace