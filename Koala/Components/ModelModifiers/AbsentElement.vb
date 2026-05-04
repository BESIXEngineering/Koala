Imports Grasshopper.Kernel


Namespace Koala

    Public Class AbsentElement
        Inherits GH_KoalaComponent

        Const DefaultNamePrefix As String = "AE"

        Public Sub New()
            MyBase.New("Absent element", "Absence",
                "Model modifier: absent support, beam or surface",
                 "Structure", New EsaObjectType() {EsaObjectType.AbsentElement})
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
            pManager.AddTextParameter("NamePrefix", "NamePrefix", "Absence object name prefix", GH_ParamAccess.item, DefaultNamePrefix)
            pManager.AddTextParameter("ModificationGroup", "ModGroup", "Name of the modification group of the absence", GH_ParamAccess.item)
            Dim supportedTypes = New [Enum]() {
                EsaObjectType.Undefined,
                EsaObjectType.Member1D,
                EsaObjectType.Member2D,
                EsaObjectType.NodeSupport,
                EsaObjectType.BeamPointSupport,
                EsaObjectType.BeamLineSupport,
                EsaObjectType.SurfaceEdgeSupport,
                EsaObjectType.SurfaceSupport
            }
            pManager.AddParameter(New Param_Enum("ElementType", "Type of the absent element", GH_ParamAccess.item, supportedTypes, 0, True))
            pManager.AddTextParameter("ElementName", "ElementName", "Name of absent element", GH_ParamAccess.item)
        End Sub

        ''' <summary>
        ''' Registers all the output parameters for this component.
        ''' </summary>
        Protected Overrides Sub RegisterOutputParams(pManager As GH_Component.GH_OutputParamManager)
            pManager.AddTextParameter("AbsentElement", "AbsentElement", "AbsentElement data", GH_ParamAccess.list)
        End Sub

        Protected Overrides Sub SolveInstance(DA As IGH_DataAccess)
            Dim namePrefix As String = DefaultNamePrefix
            Dim mgroupName As String = ""
            Dim targetName As String = ""
            Dim targetType As EsaObjectType = EsaObjectType.Undefined

            Dim i As Integer
            DA.GetData(0, namePrefix)
            If (Not DA.GetData(1, mgroupName)) Then Return
            If DA.GetData(2, i) Then targetType = CType(i, EsaObjectType)
            If (Not DA.GetData(3, targetName)) Then Return

            Dim FlatList As New List(Of System.Object)()

            If (String.IsNullOrWhiteSpace(targetName)) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Absent element name is absent...")
            End If
            If (String.IsNullOrWhiteSpace(mgroupName)) Then
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Modification group name is absent...")
            End If

            NameIndex += 1
            FlatList.Add(namePrefix & NameIndex.ToString())
            FlatList.Add(mgroupName)
            FlatList.Add(GetEnumDescription(targetType))
            FlatList.Add(targetName.Trim())

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
                Return My.Resources.ModelModifier

            End Get
        End Property

        ''' <summary>
        ''' Each component must have a unique Guid to identify it. 
        ''' It is vital this Guid doesn't change otherwise old ghx files 
        ''' that use the old ID will partially fail during loading.
        ''' </summary>
        Public Overrides ReadOnly Property ComponentGuid() As Guid
            Get
                Return New Guid("529ab2ad-8fd4-4bec-ac7b-149887de524b")
            End Get
        End Property
    End Class

End Namespace