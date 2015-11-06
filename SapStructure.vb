Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

''' <summary>
''' Represents the structure
''' </summary>
''' <remarks></remarks>
''' 
<Serializable>
Public Class SapStructure
    'reference to the sap application
    Private SapObject As SAP2000v16.SapObject
    'reference to the sap model
    Private SapModel As SAP2000v16.cSapModel
    'list of joints along the cast unit
    Public lstJoints As List(Of Joint)
    'list of elements along the cast unit
    Public lstElements As List(Of Element)

    ''' <summary>
    ''' Class constructor
    ''' </summary>
    ''' <param name="__strPath">Path to the sap model</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal __strPath As String)
        Dim ret As Long



        'create Sap2000 object
        SapObject = New SAP2000v16.SapObject

        'start Sap2000 application
        SapObject.ApplicationStart()

        'create SapModel object
        SapModel = SapObject.SapModel

        'initialize model
        ret = SapModel.InitializeNewModel

        ret = SapModel.SetPresentUnits(8)


        SetModelDetails(__strPath)
    End Sub

    ''' <summary>
    ''' Saves the SapStructure object. The object can later be loaded without invoking the sap model
    ''' </summary>
    ''' <param name="__strPath"></param>
    ''' <param name="__SapStructure"></param>
    ''' <remarks></remarks>
    Public Shared Sub SaveObject(ByVal __strPath As String, ByRef __SapStructure As SapStructure)
        Dim fs As FileStream = New FileStream(__strPath, FileMode.OpenOrCreate)
        Dim bf As New BinaryFormatter()
        bf.Serialize(fs, __SapStructure)
        fs.Close()

    End Sub

    ''' <summary>
    ''' Loads the SapStructure object through the saved file. The sap model is not invoked in this method
    ''' </summary>
    ''' <param name="__strPath"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function OpenObject(ByVal __strPath As String) As SapStructure
        Dim fsRead As New FileStream(__strPath, FileMode.Open)
        Dim bf As New BinaryFormatter()
        Dim objTest As Object = bf.Deserialize(fsRead)
        Dim _output As SapStructure
        _output = TryCast(objTest, SapStructure)
        fsRead.Close()

        Return _output
    End Function

    ''' <summary>
    ''' Gets the results of the anlysis based on the input model.
    ''' </summary>
    ''' <param name="__strPath">The path to the model to retrieve the analysis results from</param>
    ''' <remarks></remarks>
    Public Sub GetResults(ByVal __strPath As String)
        Dim _strName As String

        _strName = GetName(__strPath)
        OpenModel(__strPath)
        Element.SetResults(SapModel, _strName, lstElements)

    End Sub


    ''' <summary>
    ''' Gets the result of the analysis based on the input model only for the input element
    ''' </summary>
    ''' <param name="__strPath">The path of the model to retrieve the results from</param>
    ''' <param name="__ID">The ID of the element</param>
    ''' <remarks></remarks>
    Public Sub GetResults(ByVal __strPath As String, ByVal __ID As Long)
        Dim _strName As String

        _strName = GetName(__strPath)
        OpenModel(__strPath)
        Element.SetResults(SapModel, _strName, lstElements, __ID)

    End Sub



    ''' <summary>
    ''' Returns the name of the sap file from its path
    ''' </summary>
    ''' <param name="__strPath">Sap file path</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetName(ByVal __strPath As String) As String
        Dim _strOuput As String
        Dim _lngIndex As Long
        _lngIndex = InStrRev(__strPath, "\")
        _strOuput = Strings.Right(__strPath, Strings.Len(__strPath) - _lngIndex)
        Return _strOuput
    End Function

    ''' <summary>
    ''' Sets the details of the cast unit. This includes all details except forces.
    ''' </summary>
    ''' <param name="__strPath"></param>
    ''' <remarks></remarks>
    Private Sub SetModelDetails(ByVal __strPath As String)
        OpenModel(__strPath)
        lstJoints = Joint.GetSpecJoints(SapModel)
        lstElements = Element.GetEles(SapModel)
        Joint.SetConnectivity(lstJoints, lstElements, SapModel)
        Element.SetJoints(lstElements, lstJoints)
        Element.SetDirs(lstElements)

    End Sub

    ''' <summary>
    ''' Opens the sap model
    ''' </summary>
    ''' <param name="__strPath">Path to sap model</param>
    ''' <remarks></remarks>
    Private Sub OpenModel(ByVal __strPath As String)

        Dim ret As Long
        'create new blank model
        ret = SapModel.File.OpenFile(__strPath)
    End Sub

    ''' <summary>
    ''' closes the sap application
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CloseApplication()
        SapObject.ApplicationExit(False)
        SapObject = Nothing
        SapModel = Nothing
    End Sub

    ''' <summary>
    ''' returns the list of forces along the cast unit
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Forces(ByVal __ID As Long) As List(Of Force)
        For i As Integer = 0 To lstElements.Count - 1
            If lstElements.Item(i).ID = __ID Then
                Return lstElements.Item(i).Forces
            End If
        Next
        Return Nothing

    End Function

    ''' <summary>
    ''' returns the list of t2 dimensions along the cast unit. Each t2 is at a station
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function t2(ByVal __ID As Long) As Double
        For i As Integer = 0 To lstElements.Count - 1
            If lstElements.Item(i).ID = __ID Then
                Return lstElements.Item(i).t2
            End If
        Next
        Return Nothing
    End Function

    ''' <summary>
    ''' returns the list of t3 dimensions along the cast unit. Each t3 is at a station.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function t3(ByVal __ID As Long) As Double
        For i As Integer = 0 To lstElements.Count - 1
            If lstElements.Item(i).ID = __ID Then
                Return lstElements.Item(i).t3
            End If
        Next
        Return Nothing
    End Function



    ''' <summary>
    ''' Returns the element object based on the input ID
    ''' </summary>
    ''' <param name="__ID">The ID of the element to return</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetElement(ByVal __ID As Long) As Element
        For i As Integer = 0 To lstElements.Count - 1
            If lstElements.Item(i).ID = __ID Then
                Return lstElements.Item(i)
            End If
        Next
        Return Nothing
    End Function
End Class
