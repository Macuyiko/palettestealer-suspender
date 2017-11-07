Public Class GameEntry
    Private gameExecutableName As String
    Private gameExecutablePath As String
    Private gameWorkingPath As String
    Private monitorGame As Boolean

    Public Sub New(ByVal gameExecutableName, ByVal gameExecutablePath, ByVal gameWorkingPath)
        Me.gameExecutableName = gameExecutableName
        Me.gameExecutablePath = gameExecutablePath
        Me.gameWorkingPath = gameWorkingPath
        Me.monitorGame = False
    End Sub

    Public Overrides Function ToString() As String
        Return Me.gameExecutableName + " in working dir " + Me.gameExecutablePath
    End Function

    Public Function getGameExecutableName() As String
        Return Me.gameExecutableName
    End Function
    Public Function getGameExecutablePath() As String
        Return Me.gameExecutablePath
    End Function
    Public Function getGameWorkingPath() As String
        Return Me.gameWorkingPath
    End Function
    Public Function getMonitorGame() As Boolean
        Return Me.monitorGame
    End Function

    Public Sub setMonitorGame(ByVal monitor As Boolean)
        Me.monitorGame = monitor
    End Sub
End Class
