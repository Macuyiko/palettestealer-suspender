Module Startup
    ' Main display form
    Private myStartupForm As MainForm
    Private myStartupConsole As MainConsole.MainConsole
    Private consoleMode As Boolean = False
    Public debuglog As String = ""
    Private oWrite As System.IO.StreamWriter
    ' Delegate used to marshal back to the main UI thread
    Private Delegate Sub SafeApplicationThreadException(ByVal sender As Object, ByVal e As Threading.ThreadExceptionEventArgs)

    Sub Main()
        Try
            Dim strStartupArguments() As String
            Dim intCount As Integer
            strStartupArguments = System.Environment.GetCommandLineArgs
            For intCount = 0 To UBound(strStartupArguments)
                Select Case strStartupArguments(intCount).ToLower
                    Case "/nogui"
                        consoleMode = True
                End Select
            Next intCount
        Catch ex As Exception
        End Try

        ' Configure AppDomain 
        Dim currentDomain As AppDomain = AppDomain.CurrentDomain
        AddHandler currentDomain.UnhandledException, AddressOf AppDomain_UnhandledException
        AddHandler Application.ThreadException, AddressOf Application_ThreadException

        ' Get file
        Try
            Dim exePath As String = Application.StartupPath()
            oWrite = IO.File.AppendText(exePath & "\log.txt")
        Catch ex As Exception
        End Try

        ' Log basic info
        AddToDebugLog(vbCrLf & vbCrLf & "=============== Program started ====================")
        AddToDebugLog("(Info) OS Version: " & Environment.OSVersion.Platform.ToString())
        AddToDebugLog("(Info) OS VersionString: " & Environment.OSVersion.VersionString.ToString())
        AddToDebugLog("(Info) Username: " & Environment.UserName.ToString())
        AddToDebugLog("(Info) .NET CLR Version: " & Environment.Version.ToString())
        If IntPtr.Size = 8 Then AddToDebugLog("(Info) OS Bit: 64")
        If IntPtr.Size = 4 Then AddToDebugLog("(Info) OS Bit: 32")

        ' Start the application
        If consoleMode = False Then
            myStartupForm = New MainForm
            Try
                Application.EnableVisualStyles()
                Application.Run(myStartupForm)
            Catch ex As Exception
                ' Custom handling logic
                DisplayCustomErrorLogic(ex)
            End Try
        Else
            myStartupConsole = New MainConsole.MainConsole
            Try
                myStartupConsole.Main()
            Catch ex As Exception
                ' Custom handling logic
                DisplayCustomErrorLogic(ex)
            End Try
        End If
    End Sub

    Public Sub WriteToLogFile(ByVal message As String)
        Try
            oWrite.Write(message)
            oWrite.Flush()
        Catch ex As Exception
        End Try
    End Sub

    Public Sub AddToDebugLog(ByVal message As String)
        Dim formattedMessage = "[" & DateTime.Now.ToString("HH:mm:ss") & "] " & message & vbCrLf
        Startup.debuglog = Startup.debuglog & formattedMessage

        If Startup.debuglog.Length > 80 * 50000 Then
            Startup.debuglog = ""
        End If

        ' Remove for debug
        ' WriteToLogFile(formattedMessage)
        ' Debug.Print(message)
    End Sub

    Private Sub DisplayCustomErrorLogic(ByVal sourceException As Exception)
        If consoleMode = False Then myStartupForm.Hide()
        Startup.AddToDebugLog(vbCrLf & "----- Exception -----" & vbCrLf & sourceException.Message.ToString _
                        & vbCrLf & sourceException.Data.ToString _
                        & vbCrLf & sourceException.Source.ToString _
                        & vbCrLf & sourceException.StackTrace.ToString _
                        & vbCrLf & vbCrLf)
        Clipboard.SetText("======== Debuglog ========" & vbCrLf & Startup.debuglog)

        Dim errorMessage As String = "An error occured, the following information is available:" & vbCrLf & _
                        vbCrLf & sourceException.Message.ToString _
                        & vbCrLf & vbCrLf & "More information was copied to your clipboard. Please send this report to macuyiko@gmail.com"
        If consoleMode = False Then MsgBox(errorMessage, MsgBoxStyle.OkOnly, "Unexpected Error")
        If consoleMode = True Then Console.WriteLine(errorMessage, MsgBoxStyle.OkOnly, "Unexpected Error")

        If consoleMode = False Then myStartupForm.Close()
        Application.Exit()
    End Sub
    Private Sub Application_ThreadException(ByVal sender As Object, ByVal e As Threading.ThreadExceptionEventArgs)
        ' Pass to the safe exception handler
        SafeApplication_ThreadException(sender, e)
    End Sub
    Private Sub SafeApplication_ThreadException(ByVal sender As Object, ByVal e As Threading.ThreadExceptionEventArgs)
        ' Are we running on the correct thread?
        If consoleMode = False Then
            If myStartupForm.InvokeRequired Then
                ' Invoke back to the main thread
                myStartupForm.Invoke(New SafeApplicationThreadException(AddressOf SafeApplication_ThreadException), New Object() {sender, e})
            Else
                ' Custom handling logic
                DisplayCustomErrorLogic(e.Exception)
            End If
        Else
            DisplayCustomErrorLogic(e.Exception)
        End If
    End Sub
    Private Sub AppDomain_UnhandledException(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        ' Custom handling logic
        DisplayCustomErrorLogic(DirectCast(e.ExceptionObject, Exception))
    End Sub

End Module
