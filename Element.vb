''' <summary>
''' Represents elements in the structure. Could be columns or beams
''' </summary>
''' <remarks></remarks>
''' 
<Serializable>
Public Class Element
    'First joint of the element
    Public Joint1 As Joint
    'second joint of the element
    Public Joint2 As Joint
    'element ID as in SAP
    Public ID As Long '
    'the direction of the element
    Public Dir As ELEMENT_DIRECTION
    'each force object defines the forces at a station
    Public Forces As List(Of Force)
    Public Type As ELEMENT_TYPE
    Public Section As String

    'the t2 dimension of the element
    Public t2 As Double
    'the t3 dimension of the element
    Public t3 As Double
    Public Enum ELEMENT_TYPE
        BEAM
        COLUMN
    End Enum
    Public Enum ELEMENT_DIRECTION
        X
        Y
        Z
        Dia
    End Enum
    ''' <summary>
    ''' class constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(ByVal __ID As Long)
        Forces = New List(Of Force)
        ID = __ID
    End Sub

    ''' <summary>
    ''' Determines the ID of the element associated with the input string. The input string
    ''' could be in the format "3-13". This function will parse the string and return "3"
    ''' </summary>
    ''' <param name="__strName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetElId(ByVal __strName As String) As String
        Dim _strOutput As String
        If InStr(__strName, "~") Then
            Return ""
        End If
        If InStr(__strName, "-") <> 0 Then
            _strOutput = Left(__strName, InStr(__strName, "-") - 1)
        Else
            _strOutput = __strName
        End If
        Return _strOutput
    End Function

    ''' <summary>
    ''' Determines if the input frame is found in the input list of frames. IDs are compared
    ''' </summary>
    ''' <param name="__Frames">List of frames</param>
    ''' <param name="__frame">Frame to find in list</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ChkFrame(ByRef __Frames As List(Of Element), ByRef __frame As Element)
        For i As Integer = 0 To __Frames.Count - 1
            If __Frames.Item(i).ID = __frame.ID Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' The function recieves as input a list of frames and a joint. It looks through all the connected
    ''' frames to the joint and tries to find a match in the list of frames. If a match is NOT found then that
    ''' frame is added to the list of frames. 
    ''' </summary>
    ''' <param name="__Joint"></param>
    ''' <param name="__frames"></param>
    ''' <remarks></remarks>
    Private Shared Sub AddJFrames(ByRef __Joint As Joint, ByRef __frames As List(Of Element))
        For i As Integer = 0 To __Joint.Connectivity.Count - 1
            If ChkFrame(__frames, __Joint.Connectivity.Item(i)) = False Then
                __frames.Add(__Joint.Connectivity.Item(i))
            End If
        Next
    End Sub

    ''' <summary>
    ''' Gets the list of elements from the sap model and returns of a list of empty elements. Only the ID will be set
    ''' </summary>
    ''' <param name="__SapModel">Sap model</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetEleList(ByRef __SapModel As SAP2000v16.cSapModel) As List(Of Element)
        Dim _lstOutput As List(Of Element)
        Dim ret As Long
        Dim NumberNames As Long
        Dim arrNames() As String = Nothing
        Dim _Element As Element

        _lstOutput = New List(Of Element)

        ret = __SapModel.FrameObj.GetNameList(NumberNames, arrNames)
        For i As Integer = LBound(arrNames) To UBound(arrNames)
            _Element = New Element(arrNames(i))
            _lstOutput.Add(_Element)
        Next
        Return _lstOutput
    End Function





    ''' <summary>
    ''' Gets the forces associated with the input frame and the input loadcase or combination
    ''' </summary>
    ''' <param name="__Frame">The frame for which the shear will be attained</param>
    ''' <param name="__Combo_LC">The combo/LoadCase for which the shear will be attained</param>
    ''' <param name="__SapModel">The sap model from which the shear will  be attained</param>
    Private Shared Sub GetForces_LC(ByRef __Frame As Element, ByVal __Combo_LC As String, _
                                         ByRef __SapModel As SAP2000v16.cSapModel, ByRef __P() As Double, _
                                         ByRef __V2() As Double, ByRef __V3() As Double, ByRef __M2() As Double, _
                                         ByRef __M3() As Double, ByRef __T() As Double)
        Dim ret As Long

        Dim NumberResults As Long
        Dim Obj() As String = Nothing
        Dim ObjSta() As Double = Nothing
        Dim Elm() As String = Nothing
        Dim ElmSta() As Double = Nothing
        Dim LoadCase() As String = Nothing
        Dim StepType() As String = Nothing
        Dim StepNum() As Double = Nothing



        ret = __SapModel.Results.Setup.DeselectAllCasesAndCombosForOutput

        'set case selected for output
        ret = __SapModel.Results.Setup.SetComboSelectedForOutput(__Combo_LC)
        'if this is a load case
        If ret = 1 Then
            ret = __SapModel.Results.Setup.SetCaseSelectedForOutput(__Combo_LC)
        End If
        ret = __SapModel.Results.FrameForce(Strings.Trim(Str(__Frame.ID)), 0, _
                                                  NumberResults, Obj, ObjSta, Elm, ElmSta, LoadCase, StepType, StepNum, _
                                                  __P, __V2, __V3, __T, __M2, __M3)
    End Sub


    ''' <summary>
    ''' Adds the input list of forces to the elements force list
    ''' </summary>
    ''' <param name="__V2"></param>
    ''' <param name="__V3"></param>
    ''' <param name="__P"></param>
    ''' <param name="__strModel"></param>
    ''' <param name="__strCase"></param>
    ''' <remarks></remarks>
    Private Sub AddForces(ByRef __V2 As List(Of Double), ByRef __V3 As List(Of Double), _
                          ByRef __P As List(Of Double), ByRef __M2 As List(Of Double), ByRef __M3 As List(Of Double), _
                           ByRef __T As List(Of Double), ByVal __strModel As String, _
                           ByVal __strCase As String, ByVal __IsCombo As Boolean)
        For i As Integer = 0 To __V2.Count - 1
            Forces.Item(i).V2.AddForce(__V2.Item(i), __strModel, __strCase, __IsCombo)
            Forces.Item(i).V3.AddForce(__V3.Item(i), __strModel, __strCase, __IsCombo)
            Forces.Item(i).P.AddForce(__P.Item(i), __strModel, __strCase, __IsCombo)
            Forces.Item(i).M2.AddForce(__M2.Item(i), __strModel, __strCase, __IsCombo)
            Forces.Item(i).M3.AddForce(__M3.Item(i), __strModel, __strCase, __IsCombo)
            Forces.Item(i).T.AddForce(__T.Item(i), __strModel, __strCase, __IsCombo)

        Next
    End Sub

    ''' <summary>
    ''' converts the input array to a list
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="__arrInput"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function Arr2List(Of T)(ByRef __arrInput() As T) As List(Of T)
        Dim _lstOutput As List(Of T)
        _lstOutput = New List(Of T)
        For i As Integer = LBound(__arrInput) To UBound(__arrInput)
            _lstOutput.Add(__arrInput(i))
        Next

        Return _lstOutput
    End Function

    ''' <summary>
    ''' Sets the force values of the elements force list for each load case
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <param name="__StrModel"></param>
    ''' <remarks></remarks>
    Private Sub SetForces_LoadCases(ByRef __SapModel As SAP2000v16.cSapModel, ByVal __StrModel As String)
        Dim i As Long
        Dim _lstV2 As List(Of Double)
        Dim _lstP As List(Of Double)
        Dim _lstV3 As List(Of Double)
        Dim _lstM2 As List(Of Double)
        Dim _lstM3 As List(Of Double)
        Dim _lstT As List(Of Double)

        Dim _flagNew As Boolean
        Dim _lstLoadCases As List(Of String)
        Dim _P() As Double = Nothing
        Dim _T() As Double = Nothing
        Dim _V2() As Double = Nothing
        Dim _V3() As Double = Nothing
        Dim _M2() As Double = Nothing
        Dim _M3() As Double = Nothing

        _flagNew = False

        _lstLoadCases = Cmb_LC.GetLoadCases(__SapModel)
        For i = 0 To _lstLoadCases.Count - 1
            GetForces_LC(Me, _lstLoadCases.Item(i), __SapModel, _P, _V2, _V3, _M2, _M3, _T)
            _lstV2 = Arr2List(_V2)
            _lstP = Arr2List(_P)
            _lstV3 = Arr2List(_V3)
            _lstM2 = Arr2List(_M2)
            _lstM3 = Arr2List(_M3)
            _lstT = Arr2List(_T)
            ' _lstShears = GetV2Abs(_lstShears)
            AddForces(_lstV2, _lstV3, _lstP, _lstM2, _lstM3, _lstT, __StrModel, _lstLoadCases.Item(i), False)
        Next

    End Sub

    ''' <summary>
    ''' Sets the force value for the combinations in the model
    ''' </summary>
    ''' <param name="__SapModel">The sap model</param>
    ''' <param name="__StrModel">The name of the model</param>
    ''' <remarks></remarks>
    Private Sub SetForces_AllC(ByRef __SapModel As SAP2000v16.cSapModel, ByVal __StrModel As String)
        Dim i As Long
        Dim _lstV2 As List(Of Double)
        Dim _lstP As List(Of Double)
        Dim _lstV3 As List(Of Double)
        Dim _lstM2 As List(Of Double)
        Dim _lstM3 As List(Of Double)
        Dim _lstT As List(Of Double)

        Dim _flagNew As Boolean
        Dim _lstCombos As List(Of String)
        Dim _P() As Double = Nothing
        Dim _T() As Double = Nothing
        Dim _V2() As Double = Nothing
        Dim _V3() As Double = Nothing
        Dim _M2() As Double = Nothing
        Dim _M3() As Double = Nothing
        Dim _lngComboType As Long


        _flagNew = False

        _lstCombos = Cmb_LC.GetCombos(__SapModel)
        For i = 0 To _lstCombos.Count - 1

            __SapModel.RespCombo.GetType(_lstCombos.Item(i), _lngComboType)
            If _lngComboType <> 1 Then
                GetForces_LC(Me, _lstCombos.Item(i), __SapModel, _P, _V2, _V3, _M2, _M3, _T)
                _lstV2 = Arr2List(_V2)
                _lstP = Arr2List(_P)
                _lstV3 = Arr2List(_V3)
                _lstM2 = Arr2List(_M2)
                _lstM3 = Arr2List(_M3)
                _lstT = Arr2List(_T)
                ' _lstShears = GetV2Abs(_lstShears)
                AddForces(_lstV2, _lstV3, _lstP, _lstM2, _lstM3, _lstT, __StrModel, _lstCombos.Item(i), True)
            End If

        Next

    End Sub


    ''' <summary>
    ''' Returns the list of stations assocaited with the element
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetStations(ByRef __SapModel As SAP2000v16.cSapModel) As List(Of Double)
        Dim ret As Long

        Dim NumberResults As Long
        Dim Obj() As String = Nothing
        Dim ObjSta() As Double = Nothing
        Dim Elm() As String = Nothing
        Dim ElmSta() As Double = Nothing
        Dim LoadCase() As String = Nothing
        Dim StepType() As String = Nothing
        Dim StepNum() As Double = Nothing
        Dim P() As Double = Nothing
        Dim V2() As Double = Nothing
        Dim V3() As Double = Nothing
        Dim T() As Double = Nothing
        Dim M2() As Double = Nothing
        Dim M3() As Double = Nothing
        Dim _lstOutput As List(Of Double)

        Dim _strLoadCase As String

        _strLoadCase = Cmb_LC.GetLoadCases(__SapModel).Item(0)

        ret = __SapModel.Results.Setup.DeselectAllCasesAndCombosForOutput

        'set case selected for output
        ret = __SapModel.Results.Setup.SetCaseSelectedForOutput(_strLoadCase)
        ret = __SapModel.Results.FrameForce(Strings.Trim(Str(ID)), 0, _
                                                  NumberResults, Obj, ObjSta, Elm, ElmSta, LoadCase, StepType, StepNum, P, V2, V3, T, M2, M3)

        _lstOutput = New List(Of Double)
        For i = LBound(ElmSta) To UBound(ElmSta)
            _lstOutput.Add(ElmSta(i))
        Next
        Return _lstOutput
    End Function


    ''' <summary>
    ''' Generates the Force list of the element. The function only sets the force objects station
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <remarks></remarks>
    Private Sub SetStations(ByRef __SapModel As SAP2000v16.cSapModel)
        Dim _lstStations As List(Of Double)
        Dim i As Integer
        Dim _tmpForce As Force

        _lstStations = GetStations(__SapModel)
        For i = 0 To _lstStations.Count - 1
            _tmpForce = New Force(_lstStations.Item(i), ID)
            Forces.Add(_tmpForce)
        Next

    End Sub




    ''' <summary>
    ''' Sets the t3 and t2 values of the element
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <remarks></remarks>
    Private Sub SetDims(ByRef __SapModel As SAP2000v16.cSapModel)


        Dim _ret As Long
        Dim _FileName As String = ""
        Dim _MatProp As String = ""
        Dim _t3 As Double
        Dim _t2 As Double
        Dim _Color As Long
        Dim _Notes As String = ""
        Dim _GUID As String = ""




        _ret = __SapModel.PropFrame.GetRectangle(Section, _FileName, _MatProp, _t3, _t2, _Color, _Notes, _GUID)
        t3 = _t3
        t2 = _t2
    End Sub

    ''' <summary>
    ''' Sets the type of the element. This is based on the design property set in SAP for the section. 
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <remarks></remarks>
    Private Sub SetType(ByRef __SapModel As SAP2000v16.cSapModel)
        Dim Type As Long


        __SapModel.PropFrame.GetTypeRebar(Section, Type)
        If Type = 1 Then
            Me.Type = ELEMENT_TYPE.COLUMN
        Else
            Me.Type = ELEMENT_TYPE.BEAM
        End If
    End Sub


    ''' <summary>
    ''' Sets the section property for the element
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <remarks></remarks>
    Private Sub SetSection(ByRef __SapModel As SAP2000v16.cSapModel)
        Dim ret As Long
        Dim PropName As String = ""
        Dim SAuto As String = ""


        ret = __SapModel.FrameObj.GetSection(Strings.Trim(Str(ID)), PropName, SAuto)
        Section = PropName

    End Sub
    ''' <summary>
    ''' Returns the list of elements associated with the input list of joints. It is assumed the input joints are
    ''' all in the same line
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetEles(ByRef __SapModel As SAP2000v16.cSapModel) As List(Of Element)

        Dim _lstOutput As List(Of Element)

        _lstOutput = GetEleList(__SapModel)

        For i As Integer = 0 To _lstOutput.Count - 1
            _lstOutput.Item(i).SetStations(__SapModel)

            _lstOutput.Item(i).SetSection(__SapModel)
            _lstOutput.Item(i).SetDims(__SapModel)
            _lstOutput.Item(i).SetType(__SapModel)
        Next

        Return _lstOutput
    End Function


    ''' <summary>
    ''' Adds the input joint to either Joint1 or Joint2 of the element.
    ''' </summary>
    ''' <param name="__Joint"></param>
    ''' <remarks></remarks>
    Private Sub AddJoint(ByRef __Joint As Joint)
        Dim _X1 As Double
        Dim _X2 As Double
        Dim _Y1 As Double
        Dim _Y2 As Double
        Dim _Z1 As Double
        Dim _Z2 As Double
        'if joint1 is not set, set the input joint as joint1
        If IsNothing(Joint1) = True Then
            Joint1 = __Joint
            'if the Joint1 is set
        Else
            _X1 = Joint1.X
            _X2 = __Joint.X
            _Y1 = Joint1.Y
            _Y2 = __Joint.Y
            _Z1 = Joint1.Z
            _Z2 = __Joint.Z
            'check if the currently set joint should be moved to joint2
            If Joint.PointEq(_X1, _X2, _Y1, _Y2) = True And _Z1 > _Z2 Then
                Joint2 = Joint1
                Joint1 = __Joint
            ElseIf Joint.PointEq(_X1, _X2, _Z1, _Z2) = True And _Y1 > _Y2 Then
                Joint2 = Joint1
                Joint1 = __Joint
            ElseIf Joint.PointEq(_Y1, _Y2, _Z1, _Z2) = True And _X1 > _X2 Then
                Joint2 = Joint1
                Joint1 = __Joint
            Else
                Joint2 = __Joint

            End If
        End If

    End Sub


    ''' <summary>
    ''' Checks if the input joint is connected to the element
    ''' </summary>
    ''' <param name="__Joint"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ChkJoint(ByRef __Joint As Joint) As Boolean

        For i As Integer = 0 To __Joint.Connectivity.Count - 1
            If __Joint.Connectivity.Item(i).ID = ID Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Sets Joint1 and Joint2 for all input elements
    ''' </summary>
    ''' <param name="__Elements">List of elements to set Joint1 and joint2</param>
    ''' <param name="__Joints">List of all joints in the model</param>
    ''' <remarks></remarks>
    Public Shared Sub SetJoints(ByRef __Elements As List(Of Element), ByRef __Joints As List(Of Joint))
        For i As Integer = 0 To __Elements.Count - 1
            For j As Integer = 0 To __Joints.Count - 1
                If __Elements.Item(i).ChkJoint(__Joints.Item(j)) = True Then
                    __Elements.Item(i).AddJoint(__Joints.Item(j))
                End If

            Next
        Next
    End Sub

    ''' <summary>
    ''' Sets the dir property for all input elements
    ''' </summary>
    ''' <param name="__lstElements"></param>
    ''' <remarks></remarks>
    Public Shared Sub SetDirs(ByRef __lstElements As List(Of Element))
        Dim _X1 As Double
        Dim _X2 As Double
        Dim _Y1 As Double
        Dim _Y2 As Double
        Dim _Z1 As Double
        Dim _Z2 As Double



        For i As Integer = 0 To __lstElements.Count - 1
            _X1 = __lstElements.Item(i).Joint1.X
            _X2 = __lstElements.Item(i).Joint2.X
            _Y1 = __lstElements.Item(i).Joint1.Y
            _Y2 = __lstElements.Item(i).Joint2.Y
            _Z1 = __lstElements.Item(i).Joint1.Z
            _Z2 = __lstElements.Item(i).Joint2.Z

            If Joint.PointEq(_X1, _X2, _Y1, _Y2) = True Then
                __lstElements.Item(i).Dir = ELEMENT_DIRECTION.Z
            ElseIf Joint.PointEq(_X1, _X2, _Z1, _Z2) = True Then
                __lstElements.Item(i).Dir = ELEMENT_DIRECTION.Y
            ElseIf Joint.PointEq(_Y1, _Y2, _Z1, _Z2) = True Then
                __lstElements.Item(i).Dir = ELEMENT_DIRECTION.X
            Else
                __lstElements.Item(i).Dir = ELEMENT_DIRECTION.Dia
            End If
        Next
    End Sub


    ''' <summary>
    ''' Sets the analysis results from the sap model
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <param name="__strModelName"></param>
    ''' <param name="__lstFrames"></param>
    ''' <remarks></remarks>
    Public Shared Sub SetResults(ByRef __SapModel As SAP2000v16.cSapModel, ByRef __strModelName As String, _
                     ByRef __lstFrames As List(Of Element))

        For i As Integer = 0 To __lstFrames.Count - 1
            __lstFrames.Item(i).SetForces_AllC(__SapModel, __strModelName)
            __lstFrames.Item(i).SetForces_LoadCases(__SapModel, __strModelName)
        Next
    End Sub

    ''' <summary>
    ''' Sets the analysis results from the sap model for only the input elemnet
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <param name="__strModelName"></param>
    ''' <param name="__lstFrames"></param>
    ''' <param name="__ID"></param>
    ''' <remarks></remarks>
    Public Shared Sub SetResults(ByRef __SapModel As SAP2000v16.cSapModel, ByRef __strModelName As String, _
                 ByRef __lstFrames As List(Of Element), ByVal __ID As Long)

        For i As Integer = 0 To __lstFrames.Count - 1
            If __lstFrames.Item(i).ID = __ID Then
                __lstFrames.Item(i).SetForces_AllC(__SapModel, __strModelName)
                __lstFrames.Item(i).SetForces_LoadCases(__SapModel, __strModelName)
            End If

        Next
    End Sub

    ''' <summary>
    ''' Searches the list of elements from an elemnet with the id __ID
    ''' </summary>
    ''' <param name="__ID">Id to search for</param>
    ''' <param name="__lstElements">list of elements to search in</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetElement(ByVal __ID As Long, ByRef __lstElements As List(Of Element)) As Element
        For i As Integer = 0 To __lstElements.Count - 1
            If __lstElements.Item(i).ID = __ID Then
                Return __lstElements.Item(i)
            End If
        Next
        Return Nothing
    End Function

End Class
