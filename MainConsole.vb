Option Strict On
Imports System.Runtime.InteropServices
Imports System.IO

Module MainConsole
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function AttachConsole(ByVal dwProcessId As IntPtr) As Boolean
    End Function

    Public Class MainConsole
        Dim paramWait As Boolean = False
        Dim paramGame As String
        Dim paramMethod As Integer = 1
        Dim paramRestore As Boolean = False
        Dim paramWorkingDir As String

        Dim paramSuspendCollection As Collection = New Collection

        Public Sub Main()
            AttachConsole(CType(-1, IntPtr))

            Console.WriteLine(vbCrLf & vbCrLf & "  ---- PaletteStealerSuspender (Console Mode) ----  ")

            Dim strStartupArguments() As String
            Dim intCount As Integer

            strStartupArguments = System.Environment.GetCommandLineArgs
            For intCount = 0 To UBound(strStartupArguments)
                Select Case strStartupArguments(intCount).ToLower
                    Case "/wait"
                        paramWait = True
                    Case "/game"
                        paramGame = strStartupArguments(intCount + 1).ToLower
                    Case "/method"
                        Try
                            paramMethod = CInt(strStartupArguments(intCount + 1).ToLower)
                        Catch ex As Exception
                            Console.WriteLine("Unrecognized value for `/method`, quitting.")
                            Exit Sub
                        End Try
                    Case "/restore"
                        paramRestore = True
                End Select
            Next intCount

            paramSuspendCollection.Add("explorer")

            If paramRestore = True Then
                Console.WriteLine("`/restore` parameter set, trying to resume...")
                Try
                    For Each proc In Process.GetProcessesByName("explorer")
                        MainFunctions.ResumeProcess(proc.Id, 1)
                        MainFunctions.ResumeProcess(proc.Id, 2)
                        MainFunctions.ResumeProcess(proc.Id, 3)
                    Next
                    If Process.GetProcessesByName("explorer").Length = 0 Then
                        MainFunctions.KillprocResume("explorer.exe")
                    End If
                Catch ex As Exception
                End Try
                Console.WriteLine("Done. Quitting.")
                Exit Sub
            End If

            If paramMethod < 1 Or paramMethod > 4 Then
                Console.WriteLine("`/method` value set to unrecognized value. Set to default (`1`).")
                paramMethod = 1
            End If

            If paramGame = Nothing Then
                Console.WriteLine("You must set a `/game` path. Quitting.")
                Exit Sub
            End If

            If Not File.Exists(paramGame) Then
                Console.WriteLine("`/game` file does not appear to exist. Quitting.")
                Exit Sub
            End If

            Console.WriteLine("- Using executable: `" & paramGame & "`.")
            paramWorkingDir = HandleWorkingDirectory(paramGame)
            Console.WriteLine("- Using working dir: `" & paramWorkingDir & "`.")
            Console.WriteLine("- Using method nr.: `" & paramMethod.ToString & "`.")
            Console.WriteLine("")

            If Not paramWait Then
                StartGame()
            Else
                WaitGame()
            End If

        End Sub

        Private Sub StartGame()
            Console.WriteLine("- Suspending processes...")

            MainFunctions.SuspendAll(paramGame, paramSuspendCollection, paramMethod)

            Console.WriteLine("- Starting game...")

            System.Threading.Thread.Sleep(2000)

            Dim gameProcess As Process = New Process()
            gameProcess.StartInfo.UseShellExecute = False
            gameProcess.StartInfo.FileName = paramGame
            gameProcess.StartInfo.WorkingDirectory = paramWorkingDir
            gameProcess.Start()

            System.Threading.Thread.Sleep(2000)

            WaitForGameStop()
        End Sub

        Private Sub WaitGame()
            Console.WriteLine("- `/wait` method set, waiting for user to start executable...")

            Dim gameExecutableName As String = System.IO.Path.GetFileNameWithoutExtension(paramGame)
            Dim procsFound As Integer = 0

            While procsFound = 0
                System.Threading.Thread.Sleep(1000)
                procsFound = Process.GetProcessesByName(gameExecutableName).Length
            End While

            Console.WriteLine("- Game started, suspending processes...")

            MainFunctions.SuspendAll(paramGame, paramSuspendCollection, paramMethod)

            System.Threading.Thread.Sleep(2000)
            WaitForGameStop()
        End Sub

        Private Sub WaitForGameStop()
            Dim gameExecutableName As String = System.IO.Path.GetFileNameWithoutExtension(paramGame)
            Dim procsFound As Integer = 1

            Console.WriteLine("- Waiting for game exit...")

            Console.WriteLine("- NOTE: Do NOT close this console window or ALT+TAB out of the game ")
            Console.WriteLine("        until all processes are resumed.")
            Console.WriteLine("        Not doing so may result in a frozen system.")
            Console.WriteLine("        See the readme for more information.")

            While procsFound > 0
                System.Threading.Thread.Sleep(1000)
                procsFound = Process.GetProcessesByName(gameExecutableName).Length
            End While

            Console.WriteLine("- Exited, resuming processes...")

            MainFunctions.ResumeAll(paramGame, paramSuspendCollection, paramMethod)

            Console.WriteLine("- Done, you may now safely close this console window ")
            Console.WriteLine("(or press ENTER to return to prompt).")
        End Sub
    End Class
End Module
