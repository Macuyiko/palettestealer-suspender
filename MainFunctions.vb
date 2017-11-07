#Region "Options And .NET Imports"

Option Strict On
Imports System.Runtime.InteropServices

#End Region

Module MainFunctions

#Region "Constants, Globals And .dll Imports"

    Public Const TH_TERMINATE As Integer = &H1
    Public Const TH_SUSPEND_RESUME As Integer = &H2
    Public Const TH_GET_CONTEXT As Integer = &H8
    Public Const TH_SET_CONTEXT As Integer = &H10
    Public Const TH_SET_INFORMATION As Integer = &H20
    Public Const TH_QUERY_INFORMATION As Integer = &H40
    Public Const TH_SET_THREAD_TOKEN As Integer = &H80
    Public Const TH_IMPERSONATE As Integer = &H100
    Public Const TH_DIRECT_IMPERSONATION As Integer = &H200

    Public Const PROCESS_TERMINATE As Integer = &H1
    Public Const PROCESS_CREATE_THREAD As Integer = &H2
    Public Const PROCESS_SET_SESSIONID As Integer = &H4
    Public Const PROCESS_VM_OPERATION As Integer = &H8
    Public Const PROCESS_VM_READ As Integer = &H10
    Public Const PROCESS_VM_WRITE As Integer = &H20
    Public Const PROCESS_DUP_HANDLE As Integer = &H40
    Public Const PROCESS_CREATE_PROCESS As Integer = &H80
    Public Const PROCESS_SET_QUOTA As Integer = &H100
    Public Const PROCESS_SET_INFORMATION As Integer = &H200
    Public Const PROCESS_QUERY_INFORMATION As Integer = &H400
    Public Const PROCESS_SUSPEND_RESUME As Integer = &H800
    Public Const PROCESS_ALL_ACCESS As Integer = &HF0000 Or SYNCHRONIZE Or &HFFF

    Private Const INFINITE As Integer = -1&
    Private Const SYNCHRONIZE As Integer = &H100000

    Const ENUM_CURRENT_SETTINGS As Integer = -1
    Const CDS_UPDATEREGISTRY As Integer = &H1
    Const CDS_TEST As Long = &H2

    Const CCDEVICENAME As Integer = 32
    Const CCFORMNAME As Integer = 32

    Const DISP_CHANGE_SUCCESSFUL As Integer = 0
    Const DISP_CHANGE_RESTART As Integer = 1
    Const DISP_CHANGE_FAILED As Integer = -1

    Enum DISP_CHANGE As Integer
        Successful = 0
        Restart = 1
        Failed = -1
        BadMode = -2
        NotUpdated = -3
        BadFlags = -4
        BadParam = -5
        BadDualView = -1
    End Enum

    <Flags()> _
    Enum DM As Integer
        Orientation = &H1
        PaperSize = &H2
        PaperLength = &H4
        PaperWidth = &H8
        Scale = &H10
        Position = &H20
        NUP = &H40
        DisplayOrientation = &H80
        Copies = &H100
        DefaultSource = &H200
        PrintQuality = &H400
        Color = &H800
        Duplex = &H1000
        YResolution = &H2000
        TTOption = &H4000
        Collate = &H8000
        FormName = &H10000
        LogPixels = &H20000
        BitsPerPixel = &H40000
        PelsWidth = &H80000
        PelsHeight = &H100000
        DisplayFlags = &H200000
        DisplayFrequency = &H400000
        ICMMethod = &H800000
        ICMIntent = &H1000000
        MediaType = &H2000000
        DitherType = &H4000000
        PanningWidth = &H8000000
        PanningHeight = &H10000000
        DisplayFixedOutput = &H20000000
    End Enum

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure DEVMODE
        Public Const CCHDEVICENAME As Integer = 32
        Public Const CCHFORMNAME As Integer = 32

        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=CCHDEVICENAME)> _
        Public dmDeviceName As String
        Public dmSpecVersion As Short
        Public dmDriverVersion As Short
        Public dmSize As Short
        Public dmDriverExtra As Short
        Public dmFields As DM

        Public dmOrientation As Short
        Public dmPaperSize As Short
        Public dmPaperLength As Short
        Public dmPaperWidth As Short

        Public dmScale As Short
        Public dmCopies As Short
        Public dmDefaultSource As Short
        Public dmPrintQuality As Short
        Public dmColor As Short
        Public dmDuplex As Short
        Public dmYResolution As Short
        Public dmTTOption As Short
        Public dmCollate As Short
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=CCHFORMNAME)> _
        Public dmFormName As String
        Public dmLogPixels As Short
        Public dmBitsPerPel As Integer ' Declared wrong in the full framework
        Public dmPelsWidth As Integer
        Public dmPelsHeight As Integer
        Public dmDisplayFlags As Integer
        Public dmDisplayFrequency As Integer

        Public dmICMMethod As Integer
        Public dmICMIntent As Integer
        Public dmMediaType As Integer
        Public dmDitherType As Integer
        Public dmReserved1 As Integer
        Public dmReserved2 As Integer
        Public dmPanningWidth As Integer
        Public dmPanningHeight As Integer

        Public dmPositionX As Integer ' Using a PointL Struct does not work
        Public dmPositionY As Integer
    End Structure

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function CloseHandle(ByVal hObject As IntPtr) As Integer
    End Function
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function ResumeThread(ByVal hThread As IntPtr) As Integer
    End Function
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function SuspendThread(ByVal hThread As IntPtr) As Integer
    End Function
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function OpenThread(ByVal dwDesiredAccess As Integer, ByVal bInheritHandle As Boolean, ByVal dwThreadId As Integer) As IntPtr
    End Function
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function WaitForSingleObject(ByVal hHandle As IntPtr, ByVal timeOut As Integer) As Integer
    End Function
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function OpenProcess(ByVal dwDesiredAccess As Integer, ByVal bInheritHandle As Boolean, ByVal dwProcessId As Integer) As IntPtr
    End Function
    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Function GetWindowThreadProcessId(ByVal handle As IntPtr, <Out()> ByRef processId As Integer) As Integer
    End Function

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function DebugActiveProcess(ByVal dwProcessId As Integer) As Boolean
    End Function
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function DebugSetProcessKillOnExit(ByVal KillOnExit As Boolean) As Boolean
    End Function
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function IsDebuggerPresent() As Boolean
    End Function
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function CheckRemoteDebuggerPresent(ByVal hProcess As IntPtr, <Out()> ByRef pbDebuggerPresent As Boolean) As Boolean
    End Function
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function DebugActiveProcessStop(ByVal dwProcessId As Integer) As Boolean
    End Function

    <DllImport("ntdll.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function NtSuspendProcess(ByVal ProcessHandle As IntPtr) As Integer
    End Function
    <DllImport("ntdll.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function NtResumeProcess(ByVal ProcessHandle As IntPtr) As Integer
    End Function

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function TerminateProcess(ByVal hProcess As IntPtr, ByVal uExitCode As Integer) As Boolean
    End Function

    <DllImport("user32", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function EnumDisplaySettings(ByVal deviceName As String, ByVal modeNum As Integer, ByRef devMode As DEVMODE) As Boolean
    End Function
    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function ChangeDisplaySettings(ByRef devMode As DEVMODE, ByVal flags As Integer) As DISP_CHANGE
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function GetSystemMenu(ByVal hwnd As IntPtr, ByVal bRevert As Long) As Long
    End Function
    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True, ExactSpelling:=True)> _
    Public Function EnableMenuItem(ByVal hMenu As Long, ByVal wIDEnableItem As Long, ByVal wEnable As Long) As Long
    End Function
#End Region

#Region "Main Suspend And Resume Handlers"

    Public Sub SuspendProcess(ByVal PID As Integer, ByVal Method As Integer)
        Dim proc As Process = Nothing
        Try 'Catch missing process
            proc = Process.GetProcessById(PID)
        Catch ex As ArgumentException
        End Try
        If IsNothing(proc) Then
            Startup.AddToDebugLog("[SuspendProcess] Process no longer exists")
            Exit Sub
        ElseIf proc.ProcessName = "" Then
            Startup.AddToDebugLog("[SuspendProcess] Invalid process name")
            Exit Sub
        End If

        If Method = 1 Then
            Startup.AddToDebugLog("[SuspendProcess] Using kernel32 method")
            Kernel32Suspend(PID)
        ElseIf Method = 2 Then
            Startup.AddToDebugLog("[SuspendProcess] Using ntdll method")
            NtdllSuspend(PID)
        ElseIf Method = 3 Then
            Startup.AddToDebugLog("[SuspendProcess] Using dbgproc method")
            DbgprocSuspend(PID)
        ElseIf Method = 4 Then
            Startup.AddToDebugLog("[SuspendProcess] Using killproc method")
            KillprocSuspend(PID)
        End If
    End Sub

    Public Sub ResumeProcess(ByVal PID As Integer, ByVal Method As Integer)
        Dim proc As Process = Nothing
        Try 'Catch missing process
            proc = Process.GetProcessById(PID)
        Catch ex As ArgumentException
        End Try
        If IsNothing(proc) Then
            Startup.AddToDebugLog("[ResumeProcess] Process no longer exists")
            Exit Sub
        ElseIf proc.ProcessName = "" Then
            Startup.AddToDebugLog("[ResumeProcess] Invalid process name")
            Exit Sub
        End If

        If Method = 1 Then
            Startup.AddToDebugLog("[ResumeProcess] Using kernel32 method")
            Kernel32Resume(PID)
        ElseIf Method = 2 Then
            Startup.AddToDebugLog("[ResumeProcess] Using ntdll method")
            NtdllResume(PID)
        ElseIf Method = 3 Then
            Startup.AddToDebugLog("[ResumeProcess] Using dbgproc method")
            DbgprocResume(PID)
        End If
    End Sub

    Public Sub SuspendAll(ByVal GameExecutablePath As String, ByVal Items As Collection, ByVal Method As Integer)
        Dim gameExecutableName = System.IO.Path.GetFileNameWithoutExtension(GameExecutablePath)

        Startup.AddToDebugLog("[SuspendAll] Called")
        For Each item In Items
            If IsAllowedTamperProcess(item.ToString, gameExecutableName) Then
                For Each proc In Process.GetProcessesByName(item.ToString)
                    Startup.AddToDebugLog("[SuspendAll] Process found: " & proc.ProcessName & " (" & proc.Id & ")")
                    SuspendProcess(proc.Id, Method)
                    Startup.AddToDebugLog("[SuspendAll] Suspended " & proc.ProcessName & " (" & proc.Id & ")")
                Next
            Else
                Startup.AddToDebugLog("[SuspendAll] " & item.ToString & " can or may not be suspended by IsAllowedTamperProcess")
            End If
        Next
    End Sub

    Public Sub ResumeAll(ByVal GameExecutablePath As String, ByVal Items As Collection, ByVal Method As Integer)
        Dim gameExecutableName = System.IO.Path.GetFileNameWithoutExtension(GameExecutablePath)

        Startup.AddToDebugLog("[ResumeAll] Called")
        For Each item In Items
            If IsAllowedTamperProcess(item.ToString, gameExecutableName) Then
                For Each proc In Process.GetProcessesByName(item.ToString)
                    Startup.AddToDebugLog("[ResumeAll] Process found: " & proc.ProcessName & " (" & proc.Id & ")")
                    ResumeProcess(proc.Id, Method)
                    Startup.AddToDebugLog("[ResumeAll] Resumed " & proc.ProcessName & " (" & proc.Id & ")")
                Next
            Else
                Startup.AddToDebugLog("[ResumeAll] " & item.ToString & " can or may not be resumed by IsAllowedTamperProcess")
                If Method = 4 Then
                    If (item.ToString.ToLower() = "explorer") Then
                        Startup.AddToDebugLog("[ResumeAll] Using killproc method to start")
                        KillprocResume(item.ToString & ".exe")
                    End If
                End If
            End If
        Next

        Dim gX As Integer = Get_Screen_Width()
        Dim gY As Integer = Get_Screen_Height()
        Startup.AddToDebugLog("[Resolution] Got: " & gX.ToString & "X" & gY.ToString)
    End Sub

#End Region

#Region "Specific Suspend And Resume Handlers"

    Private Sub NtdllSuspend(ByVal PID As Integer)
        Dim proc As Process = Process.GetProcessById(PID)
        Try 'Catch missing process
            proc = Process.GetProcessById(PID)
        Catch ex As ArgumentException
        End Try
        If IsNothing(proc) Then
            Startup.AddToDebugLog("[NtdllSuspend] Process no longer exists")
            Exit Sub
        ElseIf proc.ProcessName = "" Then
            Startup.AddToDebugLog("[NtdllSuspend] Invalid process name")
            Exit Sub
        End If

        Dim pName As String = proc.ProcessName
        Startup.AddToDebugLog("[NtdllSuspend] Trying to alternatively suspend " & pName)
        Dim pHandle As IntPtr = OpenProcess(PROCESS_SUSPEND_RESUME, False, PID)
        Dim sRet As Integer = NtSuspendProcess(pHandle)
        CloseHandle(pHandle)
        Startup.AddToDebugLog("[NtdllSuspend] sRet was: " & sRet.ToString)
    End Sub

    Private Sub NtdllResume(ByVal PID As Integer)
        Dim proc As Process = Process.GetProcessById(PID)
        Try 'Catch missing process
            proc = Process.GetProcessById(PID)
        Catch ex As ArgumentException
        End Try
        If IsNothing(proc) Then
            Startup.AddToDebugLog("[NtdllResume] Process no longer exists")
            Exit Sub
        ElseIf proc.ProcessName = "" Then
            Startup.AddToDebugLog("[NtdllResume] Invalid process name")
            Exit Sub
        End If

        Dim pName As String = proc.ProcessName
        Startup.AddToDebugLog("[NtdllResume] Trying to alternatively resume " & pName)
        Dim pHandle As IntPtr = OpenProcess(PROCESS_SUSPEND_RESUME, False, PID)
        Dim sRet As Integer = NtResumeProcess(pHandle)
        CloseHandle(pHandle)
        Startup.AddToDebugLog("[NtdllResume] sRet was: " & sRet.ToString)
    End Sub

    Private Sub KillprocSuspend(ByVal PID As Integer)
        Dim proc As Process = Process.GetProcessById(PID)
        Try 'Catch missing process
            proc = Process.GetProcessById(PID)
        Catch ex As ArgumentException
        End Try
        If IsNothing(proc) Then
            Startup.AddToDebugLog("[KillprocSuspend] Process no longer exists")
            Exit Sub
        ElseIf proc.ProcessName = "" Then
            Startup.AddToDebugLog("[KillprocSuspend] Invalid process name")
            Exit Sub
        End If

        Dim pName As String = proc.ProcessName
        Startup.AddToDebugLog("[KillprocSuspend] Trying to kill " & pName)
        Dim pHandle As IntPtr = OpenProcess(PROCESS_ALL_ACCESS, False, PID)
        TerminateProcess(pHandle, 1)
    End Sub

    Public Sub KillprocResume(ByVal procname As String)
        Startup.AddToDebugLog("[KillprocResume] Starting: " & procname)
        Try 'Catch missing process
            Process.Start(procname)
        Catch ex As Exception
            Startup.AddToDebugLog("[KillprocResume] Could not start: " & procname)
        End Try
    End Sub

    Private Sub DbgprocSuspend(ByVal PID As Integer)
        Dim proc As Process = Process.GetProcessById(PID)
        Try 'Catch missing process
            proc = Process.GetProcessById(PID)
        Catch ex As ArgumentException
        End Try
        If IsNothing(proc) Then
            Startup.AddToDebugLog("[DbgprocSuspend] Process no longer exists")
            Exit Sub
        ElseIf proc.ProcessName = "" Then
            Startup.AddToDebugLog("[DbgprocSuspend] Invalid process name")
            Exit Sub
        End If

        Dim sRet As Boolean
        Dim pbDebuggerPresent As Boolean
        Dim pName As String = proc.ProcessName

        sRet = CheckRemoteDebuggerPresent(proc.Handle, pbDebuggerPresent)
        Startup.AddToDebugLog("[DbgprocSuspend] CheckRemoteDebuggerPresent sRet was: " & sRet.ToString & " PBOOL: " & pbDebuggerPresent.ToString)
        If sRet = False Then Startup.AddToDebugLog("[GetLastWin32Error] is " & Marshal.GetLastWin32Error().ToString)

        Startup.AddToDebugLog("[DbgprocSuspend] Trying to alternatively suspend " & pName)
        sRet = DebugActiveProcess(PID)
        Startup.AddToDebugLog("[DbgprocSuspend] sRet was: " & sRet.ToString)
        If sRet = False Then
            Dim ProcError As String = Err.LastDllError.ToString
            Startup.AddToDebugLog("[DbgprocSuspend] Could not suspend, error number is: " & ProcError)
        Else
            sRet = DebugSetProcessKillOnExit(False)
            Startup.AddToDebugLog("[DbgprocSuspend] DebugSetProcessKillOnExit sRet was: " & sRet.ToString)

            sRet = CheckRemoteDebuggerPresent(proc.Handle, pbDebuggerPresent)
            Startup.AddToDebugLog("[DbgprocSuspend] CheckRemoteDebuggerPresent sRet was: " & sRet.ToString & " PBOOL: " & pbDebuggerPresent.ToString)
            If sRet = False Then Startup.AddToDebugLog("[GetLastWin32Error] is " & Marshal.GetLastWin32Error().ToString)
        End If
    End Sub

    Private Sub DbgprocResume(ByVal PID As Integer)
        Dim proc As Process = Process.GetProcessById(PID)
        Try 'Catch missing process
            proc = Process.GetProcessById(PID)
        Catch ex As ArgumentException
        End Try
        If IsNothing(proc) Then
            Startup.AddToDebugLog("[DbgprocResume] Process no longer exists")
            Exit Sub
        ElseIf proc.ProcessName = "" Then
            Startup.AddToDebugLog("[DbgprocResume] Invalid process name")
            Exit Sub
        End If

        Dim sRet As Boolean = DebugActiveProcessStop(PID)
        Dim pName As String = proc.ProcessName
        Dim pbDebuggerPresent As Boolean

        sRet = CheckRemoteDebuggerPresent(proc.Handle, pbDebuggerPresent)
        Startup.AddToDebugLog("[DbgprocSuspend] CheckRemoteDebuggerPresent sRet was: " & sRet.ToString & " PBOOL: " & pbDebuggerPresent.ToString)
        If sRet = False Then Startup.AddToDebugLog("[GetLastWin32Error] is " & Marshal.GetLastWin32Error().ToString)

        Startup.AddToDebugLog("[DbgprocResume] Trying to alternatively resume " & pName)
        Startup.AddToDebugLog("[DbgprocResume] sRet was: " & sRet.ToString)
        If sRet = False Then
            Dim ProcError As String = Err.LastDllError.ToString
            Startup.AddToDebugLog("[DbgprocResume] Could not resume, error number is: " & ProcError)
        Else
            sRet = CheckRemoteDebuggerPresent(proc.Handle, pbDebuggerPresent)
            Startup.AddToDebugLog("[DbgprocSuspend] CheckRemoteDebuggerPresent sRet was: " & sRet.ToString & " PBOOL: " & pbDebuggerPresent.ToString)
            If sRet = False Then Startup.AddToDebugLog("[GetLastWin32Error] is " & Marshal.GetLastWin32Error().ToString)
        End If
    End Sub

    Private Sub Kernel32Suspend(ByVal PID As Integer)
        Dim errorgiven As Boolean = False
        Dim proc As Process = Process.GetProcessById(PID)
        Try 'Catch missing process
            proc = Process.GetProcessById(PID)
        Catch ex As ArgumentException
        End Try
        If IsNothing(proc) Then
            Startup.AddToDebugLog("[Kernel32Suspend] Process no longer exists")
            Exit Sub
        ElseIf proc.ProcessName = "" Then
            Startup.AddToDebugLog("[Kernel32Suspend] Invalid process name")
            Exit Sub
        End If

        Startup.AddToDebugLog("[Kernel32Suspend] Trying to suspend: " & proc.ProcessName)

        For Each pT As ProcessThread In proc.Threads
            Dim pOpenThread As IntPtr = OpenThread(TH_SUSPEND_RESUME, False, pT.Id)
            If pOpenThread = IntPtr.Zero Then
                Dim ThreadError As String = Err.LastDllError.ToString
                Startup.AddToDebugLog("[Kernel32Suspend] Could not open thread, error number is: " & ThreadError)
                Continue For
            End If
            Dim sCount As Integer = SuspendThread(pOpenThread)
            If sCount = -1 Then
                If Not errorgiven Then
                    Dim ThreadError As String = Err.LastDllError.ToString
                    Startup.AddToDebugLog("[Kernel32Suspend] Could not suspend, error number is: " & ThreadError)
                    Continue For
                    errorgiven = True
                End If
            Else
                errorgiven = False
            End If
            CloseHandle(pOpenThread)
        Next
    End Sub

    Private Sub Kernel32Resume(ByVal PID As Integer)
        Dim proc As Process = Process.GetProcessById(PID)
        Try 'Catch missing process
            proc = Process.GetProcessById(PID)
        Catch ex As ArgumentException
        End Try
        If IsNothing(proc) Then
            Startup.AddToDebugLog("[Kernel32Resume] Process no longer exists")
            Exit Sub
        ElseIf proc.ProcessName = "" Then
            Startup.AddToDebugLog("[Kernel32Resume] Invalid process name")
            Exit Sub
        End If

        Startup.AddToDebugLog("[Kernel32Resume] Trying to resume: " & proc.ProcessName)

        For Each pT As ProcessThread In proc.Threads
            Dim pOpenThread As IntPtr = OpenThread(TH_SUSPEND_RESUME, False, pT.Id)
            If pOpenThread = IntPtr.Zero Then
                Dim ThreadError As String = Err.LastDllError.ToString
                Startup.AddToDebugLog("[Kernel32Resume] Could not open thread, error number is: " & ThreadError)
                Continue For
            End If
            Dim sCount As Integer = ResumeThread(pOpenThread)
            If sCount = -1 Then
                Dim ThreadError As String = Err.LastDllError.ToString
                Startup.AddToDebugLog("[Kernel32Resume] Could not resume, error number is: " & ThreadError)
                Continue For
            End If
            CloseHandle(pOpenThread)
        Next
    End Sub

#End Region

#Region "Working Dir Handler"

    Public Function HandleWorkingDirectory(ByVal Path As String) As String
        Dim WorkDir As String = System.IO.Path.GetDirectoryName(Path)

        ' Game specific directories below
        '-- Age of Empires 2 The Conquerors
        WorkDir = Replace(System.IO.Path.GetDirectoryName(Path), "\age2_x1", "", 1, , CompareMethod.Text)

        Return WorkDir
    End Function

#End Region

#Region "Screen Resolution Functions"

    Public Function Get_Screen_Height() As Integer
        Dim intY As Integer = Screen.PrimaryScreen.Bounds.Height
        Return intY
    End Function

    Public Function Get_Screen_Width() As Integer
        Dim intX As Integer = Screen.PrimaryScreen.Bounds.Width
        Return intX
    End Function

    Public Sub Change_Resolution(ByVal theWidth As Integer, ByVal theHeight As Integer)

        Dim DevM As DEVMODE

        DevM.dmDeviceName = New [String](New Char(32) {})
        DevM.dmFormName = New [String](New Char(32) {})
        DevM.dmSize = CShort(Marshal.SizeOf(GetType(DEVMODE)))

        If False <> EnumDisplaySettings(Nothing, ENUM_CURRENT_SETTINGS, DevM) Then
            Dim lResult As Integer

            DevM.dmPelsWidth = theWidth
            DevM.dmPelsHeight = theHeight
            Dim tf As Double = Math.Round(DevM.dmDisplayFrequency / 10) * 10
            DevM.dmDisplayFrequency = CInt(tf)
            DevM.dmBitsPerPel = 32
            Startup.AddToDebugLog("[Change_Resolution] dmDisplayFrequency:" & DevM.dmDisplayFrequency.ToString)

            lResult = ChangeDisplaySettings(DevM, CDS_TEST)

            If lResult = DISP_CHANGE_FAILED Then
                Startup.AddToDebugLog("[Change_Resolution] Screen resolution change failed")
            Else
                lResult = ChangeDisplaySettings(DevM, CDS_UPDATEREGISTRY)

                Select Case lResult
                    Case DISP_CHANGE_RESTART
                        Startup.AddToDebugLog("[Change_Resolution] Screen resolution change requires restart")
                    Case DISP_CHANGE_SUCCESSFUL
                        Startup.AddToDebugLog("[Change_Resolution] Screen resolution change successful")
                    Case Else
                        Startup.AddToDebugLog("[Change_Resolution] Screen resolution change failed: unknown lResult (" & lResult.ToString & ")")
                End Select
            End If


        End If
    End Sub
#End Region

#Region "Process Name Rules Check Handlers"

    Public Function IsAllowedTamperProcess(ByVal procname As String, ByVal gameexe As String) As Boolean
        Dim singleexe As New Collection
        singleexe.Add(gameexe, gameexe)
        If (Not IsAllowedTamperName(procname, singleexe)) Then
            Return False
        End If

        If Process.GetProcessesByName(procname).Length > 0 Then
            Return True
        End If

        Return False
    End Function

    Public Function IsAllowedTamperName(ByVal procname As String, ByVal gameexe As Collection) As Boolean
        'First the obvious ones
        If gameexe.Contains(procname.ToLower()) _
                Or (procname.ToLower() = "csrss") _
                Or (procname.ToLower() = "idle") Then
            Return False
        End If

        ' Add game specific names here
        '-- Age of Empires 2 The Conquerors --
        If (procname.ToLower() = "age2_x1.icd") Then
            Return False
        End If

        Return True
    End Function

#End Region

#Region "XML Functions"

#End Region

#Region "Process Kill Handlers"
    Private Sub KillProcess(ByVal PID As Integer)
        Dim proc As Process = Nothing
        Try 'Catch missing process
            proc = Process.GetProcessById(PID)
        Catch ex As ArgumentException
        End Try
        If IsNothing(proc) Then
            Startup.AddToDebugLog("[KillProcess] Process no longer exists")
            Exit Sub
        ElseIf proc.ProcessName = "" Then
            Startup.AddToDebugLog("[KillProcess] Invalid process name")
            Exit Sub
        End If
        CloseHandle(proc.Handle)
    End Sub

    Private Sub TerminateProcess(ByVal PID As Integer)
        Dim proc As Process = Nothing
        Try 'Catch missing process
            proc = Process.GetProcessById(PID)
        Catch ex As ArgumentException
        End Try
        If IsNothing(proc) Then
            Startup.AddToDebugLog("[TerminateProcess] Process no longer exists")
            Exit Sub
        ElseIf proc.ProcessName = "" Then
            Startup.AddToDebugLog("[TerminateProcess] Invalid process name")
            Exit Sub
        End If
        TerminateProcess(proc.Handle, 1)
    End Sub
#End Region

End Module
