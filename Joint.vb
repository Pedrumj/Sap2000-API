''' <summary>
''' Represents a joint in the model
''' </summary>
''' <remarks></remarks>
'''
<Serializable>
Public Class Joint
    'the x coorindate
    Public X As Double
    'the y coordinate
    Public Y As Double
    'the z coordinate
    Public Z As Double
    'the element ID as in SAP
    Public ID As Integer

    'a list of elements connected to the joint in the direction and line we are targeting (along the cast unit)
    Public Connectivity As List(Of Element)

    Private Const TOLERANCE = 0.001
    Public Sub New(ByVal __X As Double, ByVal __Y As Double, ByVal __Z As Double, _
                   ByVal __ID As Double, ByRef __SapModel As SAP2000v16.cSapModel)
        X = __X
        Y = __Y
        Z = __Z
        ID = __ID


    End Sub







    ''' <summary>
    ''' Sets the ConAll member of the joint
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <remarks></remarks>
    Private Function GetJConnected(ByRef __SapModel As SAP2000v16.cSapModel) As List(Of Long)
        Dim ret As Long
        Dim NumberItems As Long
        Dim ObjectType() As Integer
        Dim ObjectName() As String
        Dim PointNumber() As Integer
        Dim i As Integer
        Dim _lstOutput As List(Of Long)
        Dim _strID As String


        ReDim ObjectType(0 To 0)
        ReDim ObjectName(0 To 0)
        ReDim PointNumber(0 To 0)


        _lstOutput = New List(Of Long)
        ret = __SapModel.PointElm.GetConnectivity(Strings.Trim(Str(ID)), NumberItems, _
                                                  ObjectType, ObjectName, PointNumber)

        For i = LBound(ObjectType) To UBound(ObjectType)
            If ObjectType(i) = 2 Then
                _strID = Element.GetElId(ObjectName(i))
                If _strID <> "" Then
                    _lstOutput.Add(_strID)
                End If
            End If




        Next
        Return _lstOutput
    End Function


    ''' <summary>
    ''' Sets the connectivity list for the joint. 
    ''' </summary>
    ''' <param name="__Elements">List of all elemnts in the model</param>
    ''' <param name="__lstConnected">List of IDs of the connected elements to the joint</param>
    ''' <remarks></remarks>
    Private Sub SetConnectivity(ByRef __Elements As List(Of Element), ByRef __lstConnected As List(Of Long))
        Dim _Element As Element
        Connectivity = New List(Of Element)
        'loop through the IDs of all connected elemnets to the joint
        For i As Integer = 0 To __lstConnected.Count - 1
            'find the element from the list of elements
            _Element = Element.GetElement(__lstConnected.Item(i), __Elements)
            'if a match was found add it the connnectivity list
            If IsNothing(_Element) = False Then
                Connectivity.Add(_Element)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Sets the connectivity member of all input joints. 
    ''' </summary>
    ''' <param name="__Joints">Joints to set their connectivity member</param>
    ''' <param name="__Elements">List of all elements in the model</param>
    ''' <param name="__SapModel">Sap model</param>
    ''' <remarks></remarks>
    Public Shared Sub SetConnectivity(ByRef __Joints As List(Of Joint), ByRef __Elements As List(Of Element), _
                                  ByRef __SapModel As SAP2000v16.cSapModel)
        Dim _lstConnected As List(Of Long)

        For i As Integer = 0 To __Joints.Count - 1
            _lstConnected = __Joints.Item(i).GetJConnected(__SapModel)
            __Joints.Item(i).SetConnectivity(__Elements, _lstConnected)
        Next
    End Sub
    ''' <summary>
    ''' returns the distance of 2 joints
    ''' </summary>
    ''' <param name="__Joint1"></param>
    ''' <param name="__Joint2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Distance(ByRef __Joint1 As Joint, ByRef __Joint2 As Joint) As Double
        Dim _distance As Double
        _distance = ((__Joint1.X - __Joint2.X) ^ 2 + (__Joint1.Y - __Joint2.Y) ^ 2 + (__Joint1.Z - __Joint2.Z) ^ 2) ^ 0.5
        Return _distance
    End Function


    ''' <summary>
    ''' Compares the input joints and determines if the are the same
    ''' </summary>
    ''' <param name="__Joint1_C"></param>
    ''' <param name="__Joint2_C"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function IsEqu(ByRef __Joint1_C As Double, ByRef __Joint2_C As Double) As Boolean

        If Math.Abs(__Joint1_C - __Joint2_C) < 0.0001 Then
            Return True
        Else
            Return False
        End If

    End Function

    ''' <summary>
    ''' Returns all the joints in the sap model
    ''' </summary>
    ''' <param name="__sapModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetAllJoints(ByRef __sapModel As SAP2000v16.cSapModel) As List(Of Joint)
        Dim _intCount As Integer
        Dim x As Double
        Dim y As Double
        Dim z As Double
        Dim ret As Long
        Dim _Joint
        Dim _lstOutput As List(Of Joint)

        _lstOutput = New List(Of Joint)
        _intCount = __sapModel.PointElm.Count
        For i As Integer = 1 To _intCount
            ret = __sapModel.PointObj.GetCoordCartesian(Strings.Trim(Str(i)), x, y, z)
            _Joint = New Joint(x, y, z, i, __sapModel)
            _lstOutput.Add(_Joint)
        Next
        Return _lstOutput
    End Function


    ''' <summary>
    ''' Returns the set of joints in the sap model that are along the cast unit
    ''' </summary>
    ''' <param name="__sapModel">A refernce to the sap model</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetSpecJoints(ByRef __sapModel As SAP2000v16.cSapModel) As List(Of Joint)
        Dim x As Double
        Dim y As Double
        Dim z As Double

        Dim _lstAllJoints As List(Of Joint)

        Dim _lstOutput As List(Of Joint)
        Dim _Joint As Joint

        _lstAllJoints = GetAllJoints(__sapModel)
        _lstOutput = New List(Of Joint)

        'loop through all joints in the model
        For i As Integer = 0 To _lstAllJoints.Count - 1
            x = _lstAllJoints.Item(i).X
            y = _lstAllJoints.Item(i).Y
            z = _lstAllJoints.Item(i).Z

            _Joint = New Joint(x, y, z, _lstAllJoints.Item(i).ID, __sapModel)
            _lstOutput.Add(_Joint)

        Next

        Return _lstOutput
    End Function

    'determines if the input points are the same. 
    Public Shared Function PointEq(ByVal X1 As Double, ByVal X2 As Double, ByVal Y1 As Double, ByVal Y2 As Double) As Boolean
        If ((X1 - X2) ^ 2 - (Y1 - Y2) ^ 2) ^ 0.5 < 0.00001 Then
            Return True
        Else
            Return False
        End If
    End Function

End Class
