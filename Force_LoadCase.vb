''' <summary>
''' The values of a certain force type at a specific station for the different models
''' </summary>
''' <remarks></remarks>
''' 
<Serializable>
Public Class Force_LoadCase
    'the load case associated with the force
    Public LoadCase As String
    'the different force values
    Public Force_Infos As List(Of Force_Load_Info)


    Friend Sub New(ByVal __LoadCase As String)
        LoadCase = __LoadCase
        Force_Infos = New List(Of Force_Load_Info)
    End Sub

    ''' <summary>
    ''' Adds the new value to the list of values
    ''' </summary>
    ''' <param name="__Value">The value of the force</param>
    ''' <param name="__Model">The model associated with the force</param>
    ''' <remarks></remarks>
    Friend Sub Add_Force(ByVal __Value As Double, ByVal __Model As String)
        Dim _Force_Info As Force_Load_Info
        _Force_Info = New Force_Load_Info(__Value, __Model)
        Force_Infos.Add(_Force_Info)
    End Sub

    ''' <summary>
    ''' Retruns the largest value among all models
    ''' </summary>
    ''' <param name="__Value"></param>
    ''' <param name="__Model"></param>
    ''' <remarks></remarks>
    Public Sub GetMax(ByRef __Value As Double, ByRef __Model As String)
        __Value = Double.NegativeInfinity
        For i As Integer = 0 To Force_Infos.Count - 1
            If Force_Infos.Item(i).Value > __Value Then
                __Value = Force_Infos.Item(i).Value
                __Model = Force_Infos.Item(i).Model
            End If
        Next
    End Sub


    ''' <summary>
    ''' Returns the larges force value regardless of sign
    ''' </summary>
    ''' <param name="__Value"></param>
    ''' <param name="__Model"></param>
    ''' <remarks></remarks>
    Public Sub GetMaxAbs(ByRef __Value As Double, ByRef __Model As String)
        __Value = Double.NegativeInfinity
        For i As Integer = 0 To Force_Infos.Count - 1
            If Math.Abs(Force_Infos.Item(i).Value) > __Value Then
                __Value = Math.Abs(Force_Infos.Item(i).Value)
                __Model = Force_Infos.Item(i).Model
            End If
        Next
    End Sub

    ''' <summary>
    ''' returns the smallest value among all models
    ''' </summary>
    ''' <param name="__Value"></param>
    ''' <param name="__Model"></param>
    ''' <remarks></remarks>
    Public Sub GetMin(ByRef __Value As Double, ByRef __Model As String)
        __Value = Double.PositiveInfinity
        For i As Integer = 0 To Force_Infos.Count - 1
            If Force_Infos.Item(i).Value < __Value Then
                __Value = Force_Infos.Item(i).Value
                __Model = Force_Infos.Item(i).Model
            End If
        Next
    End Sub
End Class
