

''' <summary>
''' represents the forces at a station. Due to the fact that there may be different models and each
''' model will have different load cases, there will be multiple values for each force type
''' at a single station.
''' </summary>
''' <remarks></remarks>
''' 
<Serializable>
Public Class Force
    'V2 shear
    Public V2 As ForceType
    'V3 shear
    Public V3 As ForceType
    'axial
    Public P As ForceType
    'axial
    Public M2 As ForceType
    'axial
    Public M3 As ForceType
    'axial
    Public T As ForceType
    'station associated with force object
    Public Station As Double
    'the ID of the elemenet
    Public ElementID As Long

    Friend Sub New(ByVal __Station As String, ByVal __ElementID As Long)
        V2 = New ForceType()
        V3 = New ForceType()
        P = New ForceType()
        T = New ForceType
        M2 = New ForceType
        M3 = New ForceType

        Station = __Station
        ElementID = __ElementID
    End Sub

End Class
