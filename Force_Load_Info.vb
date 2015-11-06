''' <summary>
''' The value of a certain force type at a specific station under a certain load case
''' </summary>
''' <remarks></remarks>
''' 
<Serializable>
Public Class Force_Load_Info
    Public Value As Double
    Public Model As String
    Friend Sub New(ByVal __Value As Double, ByVal __Model As String)
        Value = __Value
        Model = __Model
    End Sub
End Class
