''' <summary>
''' Represents a series of elements on the same line that comprise a beam or column cast unit
''' </summary>
Friend Class CastUnit
    'the list of elements 
    Friend Elements As List(Of Element)
    'the joint that specifies the start of the cast unit
    Friend SJoint As Joint
    'the list of forces associated with the cast unit
    Friend Forces As List(Of Force)
    'the t3 dimension of the cast unit at each station
    Friend t3 As List(Of Double)
    'the t2 dimension of the cast unit at each station
    Friend t2 As List(Of Double)

    ''' <summary>
    ''' Initiates a new cast unit based on the input elements. The elements are assumed to be in order
    ''' The constructor should be called when the general information about the structure is being obtained
    ''' </summary>
    ''' <param name="__lstFrames">list of elements which comprise the cast unit</param>
    ''' <remarks></remarks>
    Friend Sub New(ByRef __lstFrames As List(Of Element))
        'initiate members
        Elements = New List(Of Element)
        SJoint = __lstFrames.Item(0).Joint1
        Forces = New List(Of Force)
        t3 = New List(Of Double)
        t2 = New List(Of Double)

        For i As Integer = 0 To __lstFrames.Count - 1
            Elements.Add(__lstFrames.Item(i))
            'will only set the stations. The values will be added later
            InitForces(__lstFrames.Item(i))
            'the dimensions of the cast unit at each station is set
            SetDims(__lstFrames.Item(i))
        Next
    End Sub

    ''' <summary>
    ''' the dimensions of the cast unit at each station is set
    ''' </summary>
    ''' <param name="__Element">list of elements which comprise the cast unit</param>
    ''' <remarks></remarks>
    Private Sub SetDims(ByRef __Element As Element)

        For i As Integer = 0 To __Element.Forces.Count - 1
            t3.Add(__Element.t3)
            t2.Add(__Element.t2)
        Next
    End Sub

    ''' <summary>
    ''' will only set the stations. The values will be added later
    ''' </summary>
    ''' <param name="__Frame">list of elements which comprise the cast unit</param>
    ''' <remarks></remarks>
    Private Sub InitForces(ByRef __Frame As Element)
        Dim _tmpForce As Force
        Dim _dblDistance As Double
        For i As Integer = 0 To __Frame.Forces.Count - 1
            'the distance of the station from SJoint
            _dblDistance = GetStation(__Frame.Forces.Item(i).Station, __Frame.Joint1)
            If IsNothing(FindForce(__Frame.Forces.Item(i).Station, __Frame.Joint1)) = True Then
                _tmpForce = New Force(_dblDistance, __Frame.Forces.Item(i).ElementID)
                Forces.Add(_tmpForce)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Returns the force object at the specified station. 
    ''' </summary>
    ''' <param name="__Station">The local distance of the station from its Joint1</param>
    ''' <param name="__Joint1">The start joint of the element </param>
    ''' <remarks></remarks>
    Private Function FindForce(ByVal __Station As Double, ByRef __Joint1 As Joint)
        Dim _dblDistance As Double
        For i As Integer = 0 To Forces.Count - 1
            _dblDistance = GetStation(__Station, __Joint1)
            If Math.Abs(Forces.Item(i).Station - _dblDistance) < 0.0001 Then
                Return Forces.Item(i)
            End If
        Next
        Return Nothing
    End Function

    ''' <summary>
    ''' Adds all the force values of the input model to the list of frames
    ''' </summary>
    ''' <param name="__Frames">list of elements which comprise the cast unit</param>
    ''' <param name="__Model">The name of the model file to lookup forces from</param>
    ''' <remarks></remarks>
    Friend Sub AddForces(ByRef __Frames As List(Of Element), ByVal __Model As String)
        Dim _tmpForce As Force
        For j As Integer = 0 To __Frames.Count - 1
            For i As Integer = 0 To __Frames.Item(j).Forces.Count - 1
                _tmpForce = FindForce(__Frames.Item(j).Forces.Item(i).Station, __Frames.Item(j).Joint1)
                SetForce(_tmpForce, __Frames.Item(j).Forces.Item(i), __Model)

            Next
        Next
    End Sub

    ''' <summary>
    ''' Gets the distance of the station from the start of the cast unit
    ''' </summary>
    ''' <param name="__lclStation">The local distance of the station from its Joint1</param>
    ''' <param name="__JointI">The start joint of the element</param>
    Private Function GetStation(ByVal __lclStation As Double, ByVal __JointI As Joint) As Double
        Dim _dblDistance As Double
        _dblDistance = ((__JointI.X - SJoint.X) ^ 2 + (__JointI.Y - SJoint.Y) ^ 2 + (__JointI.Z - SJoint.Z) ^ 2) ^ 0.5
        Return __lclStation + _dblDistance
    End Function


    ''' <summary>
    ''' Finds all forces in the input force object that have a model matching the input model string and adds
    ''' them to the target force object
    ''' </summary>
    ''' <param name="__Force2Set">The destination force object</param>
    ''' <param name="__ForceSource">The force source</param>
    ''' <param name="__Model">The name of the model file with the forces</param>
    Private Sub SetForce(ByRef __Force2Set As Force, ByRef __ForceSource As Force, ByVal __Model As String)
        Dim _strModel As String
        Dim _strCase As String
        Dim _dblValue As Double

        'loop through all load cases
        For i As Integer = 0 To __ForceSource.V2.Cases.Count - 1
            'loop through models
            For j As Integer = 0 To __ForceSource.V2.Cases.Item(i).Force_Infos.Count - 1
                'if models match
                If __ForceSource.V2.Cases.Item(i).Force_Infos.Item(j).Model = __Model Then
                    _dblValue = __ForceSource.V2.Cases.Item(i).Force_Infos.Item(j).Value
                    _strModel = __ForceSource.V2.Cases.Item(i).Force_Infos.Item(j).Model
                    _strCase = __ForceSource.V2.Cases.Item(i).LoadCase
                    __Force2Set.AddV2(_dblValue, _strModel, _strCase)
                End If
                If __ForceSource.P.Cases.Item(i).Force_Infos.Item(j).Model = __Model Then
                    _dblValue = __ForceSource.P.Cases.Item(i).Force_Infos.Item(j).Value
                    _strModel = __ForceSource.P.Cases.Item(i).Force_Infos.Item(j).Model
                    _strCase = __ForceSource.P.Cases.Item(i).LoadCase
                    __Force2Set.AddP(_dblValue, _strModel, _strCase)
                End If
                If __ForceSource.V3.Cases.Item(i).Force_Infos.Item(j).Model = __Model Then
                    _dblValue = __ForceSource.V3.Cases.Item(i).Force_Infos.Item(j).Value
                    _strModel = __ForceSource.V3.Cases.Item(i).Force_Infos.Item(j).Model
                    _strCase = __ForceSource.V3.Cases.Item(i).LoadCase
                    __Force2Set.AddP(_dblValue, _strModel, _strCase)
                End If


            Next
        Next
    End Sub
End Class
