
''' <summary>
''' The values of a certain force type at a certain station. Due to the fact that there may be different
''' models and each model may have different laod cases, there will be multiple values. 
''' </summary>
''' <remarks></remarks>
''' 
<Serializable>
Public Class ForceType


    Public Cases As List(Of Force_LoadCase)
    Public Combos As List(Of Force_LoadCase)
    Friend Sub New()
        Cases = New List(Of Force_LoadCase)
        Combos = New List(Of Force_LoadCase)
    End Sub

    Public Sub Max(ByRef __value As Double, ByRef __model As String, ByRef __loadCase As String)
        Dim _tmpValue As Double
        Dim _tmpModel As String

        __value = Double.NegativeInfinity
        __model = ""
        __loadCase = ""

        _tmpModel = ""
        For i As Integer = 0 To Combos.Count - 1
            Combos.Item(i).GetMax(_tmpValue, _tmpModel)
            If _tmpValue > __value Then
                __value = _tmpValue
                __model = _tmpModel
                __loadCase = Combos.Item(i).LoadCase
            End If
        Next
    End Sub

    Public Sub MaxAbs(ByRef __value As Double, ByRef __model As String, ByRef __loadCase As String)
        Dim _tmpValue As Double
        Dim _tmpModel As String

        __value = Double.NegativeInfinity
        __model = ""
        __loadCase = ""

        _tmpModel = ""
        For i As Integer = 0 To Combos.Count - 1
            Combos.Item(i).GetMaxAbs(_tmpValue, _tmpModel)
            If _tmpValue > __value Then
                __value = _tmpValue
                __model = _tmpModel
                __loadCase = Combos.Item(i).LoadCase
            End If
        Next
    End Sub


    ''' <summary>
    ''' Finds the index of the input load case. If a match is not found -1 is returned
    ''' </summary>
    ''' <param name="__LoadCase"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetCaseIndex(ByVal __LoadCase As String, ByVal __IsCombo As Boolean) As Long
        If __IsCombo = False Then


            For i As Integer = 0 To Cases.Count - 1
                If Cases.Item(i).LoadCase = __LoadCase Then
                    Return i
                End If
            Next
        Else
            For i As Integer = 0 To Combos.Count - 1
                If Combos.Item(i).LoadCase = __LoadCase Then
                    Return i
                End If
            Next
        End If
        Return -1
    End Function

    ''' <summary>
    ''' Adds the input force to the list of forces
    ''' </summary>
    ''' <param name="__Value">The force value</param>
    ''' <param name="__Case">The force load case</param>
    ''' <param name="__Model">The force model</param>
    ''' <remarks></remarks>
    Friend Sub AddForce(ByVal __Value As Double, ByVal __Case As String, ByVal __Model As String, _
                        ByVal __IsCombo As Boolean)
        Dim _tmpForce As Force_LoadCase
        Dim _CaseIndex As Long

        _CaseIndex = GetCaseIndex(__Case, __IsCombo)
        If __IsCombo = False Then

            If _CaseIndex <> -1 Then
                Cases.Item(_CaseIndex).Add_Force(__Value, __Model)
            Else
                _tmpForce = New Force_LoadCase(__Case)
                Cases.Add(_tmpForce)
                Cases.Item(Cases.Count - 1).Add_Force(__Value, __Model)
            End If
        Else
            If _CaseIndex <> -1 Then
                Combos.Item(_CaseIndex).Add_Force(__Value, __Model)
            Else
                _tmpForce = New Force_LoadCase(__Case)
                Combos.Add(_tmpForce)
                Combos.Item(Combos.Count - 1).Add_Force(__Value, __Model)
            End If
        End If
    End Sub


End Class
