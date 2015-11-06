''' <summary>
''' Auxilliary class used to manipulate combos and loadcases in the SAP model
''' </summary>
''' <remarks></remarks>
''' 

<Serializable>
Friend Class Cmb_LC


    ''' <summary>
    ''' Returns a list of all combos defined in the model
    ''' </summary>
    ''' <param name="__sapModel">Reference to a SAP object</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetCombos(ByRef __sapModel As SAP2000v16.cSapModel) As List(Of String)
        Dim ret As Long
        Dim NumberNames As Long
        Dim arrNames() As String
        Dim i As Long
        Dim _lstOutput As List(Of String)

        _lstOutput = New List(Of String)

        ReDim arrNames(0 To 0)
        ret = __sapModel.RespCombo.GetNameList(NumberNames, arrNames)
        For i = LBound(arrNames) To UBound(arrNames)
            _lstOutput.Add(arrNames(i))
        Next
        Return _lstOutput

    End Function

    ''' <summary>
    ''' Returns a list of all load cases defined in the model
    ''' </summary>
    ''' <param name="__sapModel">Reference to a SAP object</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetLoadCases(ByRef __sapModel As SAP2000v16.cSapModel) As List(Of String)
        Dim ret As Long
        Dim NumberNames As Long
        Dim arrNames() As String
        Dim i As Long
        Dim _lstOutput As List(Of String)

        _lstOutput = New List(Of String)

        ReDim arrNames(0 To 0)
        ret = __sapModel.LoadCases.GetNameList(NumberNames, arrNames)
        For i = LBound(arrNames) To UBound(arrNames)
            _lstOutput.Add(arrNames(i))
        Next
        Return _lstOutput

    End Function




End Class
