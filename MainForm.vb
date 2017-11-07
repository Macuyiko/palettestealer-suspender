#Region "Options And .NET Imports"

Option Strict On
Imports System.Runtime.InteropServices
Imports System.Xml
Imports System.IO
#End Region

Public Class MainForm

    Private OriginalScreenX As Integer = 0
    Private OriginalScreenY As Integer = 0

    Private MilliSecondsGameRunning As Integer = 0
    Private MonitorGameStarted As Boolean = False

#Region "WndProc Event Handler"

    Protected Overrides Sub WndProc(ByRef recWinMessage As System.Windows.Forms.Message)
        Select Case recWinMessage.Msg
            Case &H311 ' WM_PALETTECHANGED 
                Dim pid As Integer
                GetWindowThreadProcessId(recWinMessage.WParam, pid)

                Dim proc As Process = Nothing
                Try 'Catch missing process
                    proc = Process.GetProcessById(pid)
                Catch ex As ArgumentException
                End Try
                If IsNothing(proc) Then
                    Startup.AddToDebugLog("[WndProc] Process no longer exists " & " (" & pid.ToString & ")")
                    Exit Sub
                ElseIf proc.ProcessName = "" Then
                    Startup.AddToDebugLog("[WndProc] Invalid process name" & " (" & pid.ToString & ")")
                    Exit Sub
                Else
                    'Startup.AddToDebugLog("[WndProc] Intercepted: " & proc.ProcessName)
                End If

                Dim processisthere = False
                If CheckedListBox1.FindStringExact(proc.ProcessName.ToLower) > 0 Then
                    processisthere = True
                End If

                If processisthere = False Then
                    CheckedListBox1.Items.Add(proc.ProcessName.ToLower)
                    If MainFunctions.IsAllowedTamperName(proc.ProcessName, GameItemsToNameCollection(CheckedListBox2.Items)) Then
                        CheckedListBox1.SetItemChecked(CheckedListBox1.Items.IndexOf(proc.ProcessName.ToLower), True)
                    Else
                        CheckedListBox1.SetItemChecked(CheckedListBox1.Items.IndexOf(proc.ProcessName.ToLower), False)
                    End If

                    Startup.AddToDebugLog("[WndProc] Process " & proc.ProcessName.ToLower & " (" & pid.ToString & ") Added to list")
                    NotifyIcon1.ShowBalloonTip(5000, "PalettestealerSuspender found stealer program", "PalettestealerSuspender has found a program which is trying to steal the color palette: `" + proc.ProcessName.ToLower + "`, it has been added to the monitor list.", ToolTipIcon.Info)
                End If

            Case &H310 ' WM_PALETTEISCHANGING 
            Case &H30F ' WM_QUERYNEWPALETTE
        End Select

        MyBase.WndProc(recWinMessage)
    End Sub

#End Region

#Region "Process List Rules Check Handlers"

    Private Sub CheckedListBoxFullCheck()
        If CheckedListBox1.Items.Count = 0 Then
            Startup.AddToDebugLog("[CheckedListBoxFullCheck] No count found")
            Exit Sub
        End If

        Dim I As Integer
        For I = 0 To CheckedListBox1.Items.Count - 1
            Startup.AddToDebugLog("[CheckedListBoxFullCheck] Checking: " & CheckedListBox1.Items(I).ToString)
            If Not MainFunctions.IsAllowedTamperName(CheckedListBox1.Items(I).ToString, GameItemsToNameCollection(CheckedListBox2.Items)) Then
                Startup.AddToDebugLog("[CheckedListBoxFullCheck] Not allowed: " & CheckedListBox1.Items(I).ToString)
                If CheckedListBox1.GetItemChecked(I) = True Then
                    Startup.AddToDebugLog("[CheckedListBoxFullCheck] And currently checked")
                    CheckedListBox1.SetItemChecked(I, False)
                    Beep()
                End If
            Else
                If CheckedListBox1.Items(I).ToString.ToLower = "explorer" Then
                    If CheckedListBox1.GetItemChecked(I) = False Then
                        CheckedListBox1.SetItemChecked(I, True)
                        Beep()
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub ManualStart(ByRef myGame As GameEntry)
        'Do an extra check
        CheckedListBoxFullCheck()

        Try
            Startup.AddToDebugLog("[ManualStart] Game information:")
            Startup.AddToDebugLog("Name: " & myGame.getGameExecutableName)
            Startup.AddToDebugLog("Path: " & myGame.getGameExecutablePath)
        Catch
        End Try

        NotifyIcon1.ShowBalloonTip(1000, "PalettestealerSuspender is starting the game", "Starting the game: `" + myGame.getGameExecutableName + "`...", ToolTipIcon.Info)
        System.Threading.Thread.Sleep(3500)

        MainFunctions.SuspendAll(myGame.getGameExecutablePath, CheckedListToCollection(CheckedListBox1.CheckedItems), RadioToMethod())
        System.Threading.Thread.Sleep(2000)

        Dim gameProcess As Process = New Process()
        gameProcess.StartInfo.UseShellExecute = False
        gameProcess.StartInfo.FileName = myGame.getGameExecutablePath
        gameProcess.StartInfo.WorkingDirectory = myGame.getGameWorkingPath
        Startup.AddToDebugLog("[ManualStart] Working dir set to " & gameProcess.StartInfo.WorkingDirectory)
        gameProcess.Start()

        Startup.AddToDebugLog("[ManualStart] Waiting for exit")

        System.Threading.Thread.Sleep(2000)

        MonitorGameStarted = True
        MilliSecondsGameRunning = 0
        Timer1.Tag = myGame

        Startup.AddToDebugLog("[ManualStart] Control given to timer")
    End Sub
#End Region

#Region "Form Helpers"

    Private Function CheckedListToCollection(ByVal input As CheckedListBox.CheckedItemCollection) As Collection
        Dim output = New Collection
        For Each item In input
            output.Add(item)
        Next
        Return output
    End Function

    Private Function GameItemsToNameCollection(ByVal input As CheckedListBox.ObjectCollection) As Collection
        Dim output = New Collection
        For Each item In input
            Dim exename As String = CType(item, GameEntry).getGameExecutableName().ToLower()
            output.Add(exename, exename)
        Next
        Return output
    End Function

    Private Function RadioToMethod() As Integer
        If RadioButton1.Checked = True Then
            Return 1
        ElseIf RadioButton2.Checked = True Then
            Return 2
        ElseIf RadioButton3.Checked = True Then
            Return 3
        ElseIf RadioButton4.Checked = True Then
            Return 4
        End If
    End Function

    Private Sub RebuildMenu()
        Startup.AddToDebugLog("[RebuildMenu] Rebuilding menu")
        ' First transfer list check state to internal state
        Startup.AddToDebugLog("[RebuildMenu] Transferring state")
        For index = 0 To CheckedListBox2.Items.Count - 1
            Dim chkstate As CheckState = CheckedListBox2.GetItemCheckState(index)
            CType(CheckedListBox2.Items.Item(index), GameEntry).setMonitorGame(chkstate = CheckState.Checked)
        Next

        ' Now rebuild menu
        Startup.AddToDebugLog("[RebuildMenu] Building menu")
        ToolStripMenuItem1.DropDownItems.Clear()
        For Each element In CheckedListBox2.Items
            Dim myGame As GameEntry = DirectCast(element, GameEntry)
            Startup.AddToDebugLog("[RebuildMenu] Adding: " & myGame.tostring)
            Dim myMenu As New ToolStripMenuItem(myGame.ToString, Nothing, New EventHandler(AddressOf Menu_Click))
            myMenu.CheckOnClick = True
            myMenu.Checked = myGame.getMonitorGame()
            myMenu.Tag = myGame

            Dim mySubMenu As New ToolStripMenuItem("Manual Start", Nothing, New EventHandler(AddressOf Submenu_Click))
            mySubMenu.Tag = myGame

            myMenu.DropDownItems.Add(mySubMenu)
            ToolStripMenuItem1.DropDownItems.Add(myMenu)
        Next
        Startup.AddToDebugLog("[RebuildMenu] Done")
    End Sub

    Private Sub Menu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Startup.AddToDebugLog("[Menu_Click] Checking game item")
        Dim myMenu As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim myGame As GameEntry = DirectCast(myMenu.Tag, GameEntry)

        For i = 0 To CheckedListBox2.Items.Count - 1
            Dim element As GameEntry = DirectCast(CheckedListBox2.Items.Item(i), GameEntry)
            Dim test As GameEntry = DirectCast(element, GameEntry)
            If test.getGameExecutableName = myGame.getGameExecutableName And test.getGameExecutablePath = myGame.getGameExecutablePath And test.getGameWorkingPath = myGame.getGameWorkingPath Then
                If myMenu.Checked Then
                    CheckedListBox2.SetItemCheckState(i, CheckState.Checked)
                Else
                    CheckedListBox2.SetItemCheckState(i, CheckState.Unchecked)
                End If
                CType(CheckedListBox2.Items.Item(i), GameEntry).setMonitorGame(myMenu.Checked)
            End If
        Next
    End Sub

    Private Sub Submenu_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Startup.AddToDebugLog("[Submenu_Click] Clicking game item for manual start")
        Dim myMenu As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
        Dim myGame As GameEntry = DirectCast(myMenu.Tag, GameEntry)

        ManualStart(myGame)
    End Sub

    Private Sub XMLLoadList()
        Startup.AddToDebugLog("[XMLLoadList] Loading XML file")
        Try
            Dim xmldoc As New XmlDataDocument()
            Dim xmlnode As XmlNodeList
            Dim i As Integer
            Dim fs As New FileStream("save.xml", FileMode.Open, FileAccess.Read)
            xmldoc.Load(fs)
            xmlnode = xmldoc.GetElementsByTagName("game")
            For i = 0 To xmlnode.Count - 1
                Dim game As New GameEntry(xmlnode(i).ChildNodes.Item(0).InnerText.Trim(), xmlnode(i).ChildNodes.Item(1).InnerText.Trim(), xmlnode(i).ChildNodes.Item(2).InnerText.Trim())
                game.setMonitorGame(xmlnode(i).ChildNodes.Item(3).InnerText.Trim() = "True")
                Dim processisthere = False
                For Each element In CheckedListBox2.Items
                    Dim test As GameEntry = DirectCast(element, GameEntry)
                    If test.getGameExecutableName = game.getGameExecutableName And test.getGameExecutablePath = game.getGameExecutablePath And test.getGameWorkingPath = game.getGameWorkingPath Then
                        processisthere = True
                    End If
                Next
                If Not processisthere Then
                    CheckedListBox2.Items.Add(game)
                    If game.getMonitorGame = True Then
                        CheckedListBox2.SetItemCheckState(CheckedListBox2.Items.Count - 1, CheckState.Checked)
                    End If
                End If
            Next
            TextBox1.Text = ""
            TextBox2.Text = ""
            fs.Close()
            Startup.AddToDebugLog("[XMLLoadList] Done, asking menu rebuild")
            RebuildMenu()
            Startup.AddToDebugLog("[XMLLoadList] All done")
        Catch ex As Exception
            Startup.AddToDebugLog("[XMLLoadList] Error while loading XMl")
        End Try
    End Sub

    Private Sub XMLSaveList()
        Startup.AddToDebugLog("[XMLSaveList] Saving XML file")
        Try
            Dim writer As New XmlTextWriter("save.xml", System.Text.Encoding.UTF8)
            writer.WriteStartDocument(True)
            writer.Formatting = Formatting.Indented
            writer.Indentation = 2
            writer.WriteStartElement("savedata")

            writer.WriteStartElement("games")
            For Each element In CheckedListBox2.Items
                Dim myGame As GameEntry = DirectCast(element, GameEntry)
                XMLAddGame(myGame, writer)
            Next
            writer.WriteEndElement() ' games

            writer.WriteStartElement("stealers")
            For i = 0 To CheckedListBox1.Items.Count - 1
                writer.WriteStartElement("process")

                writer.WriteStartElement("exename")
                writer.WriteString(CheckedListBox1.Items.Item(i).ToString)
                writer.WriteEndElement()
                writer.WriteStartElement("monitor")
                writer.WriteString(CheckedListBox1.GetItemChecked(i).ToString)
                writer.WriteEndElement()

                writer.WriteEndElement() ' process
            Next
            writer.WriteEndElement() ' stealers

            writer.WriteEndElement() ' savedata

            writer.WriteEndDocument()
            writer.Close()
            Startup.AddToDebugLog("[XMLSaveList] Saved XML file")
        Catch ex As Exception
            Startup.AddToDebugLog("[XMLSaveList] Error while saving XML file")
        End Try
    End Sub

    Private Sub XMLAddGame(ByRef game As GameEntry, ByVal writer As XmlTextWriter)
        Startup.AddToDebugLog("[XMLAddGame] Adding XML entry")
        writer.WriteStartElement("game")

        writer.WriteStartElement("exename")
        writer.WriteString(game.getGameExecutableName)
        writer.WriteEndElement()
        writer.WriteStartElement("exepath")
        writer.WriteString(game.getGameExecutablePath)
        writer.WriteEndElement()
        writer.WriteStartElement("workdir")
        writer.WriteString(game.getGameWorkingPath)
        writer.WriteEndElement()
        writer.WriteStartElement("monitor")
        writer.WriteString(game.getMonitorGame.ToString)
        writer.WriteEndElement()

        writer.WriteEndElement()
    End Sub

    Private Sub FormHide()
        Me.Opacity = 0
        Me.WindowState = FormWindowState.Minimized
        Me.ShowInTaskbar = False
        Me.Visible = False

    End Sub

    Private Sub FormShow()
        Me.Opacity = 1
        Me.WindowState = FormWindowState.Normal
        Me.ShowInTaskbar = True
        Me.Visible = True
        Dim n As Long = EnableMenuItem(GetSystemMenu(Me.Handle, 0), &HF060&, &H1&)
    End Sub
#End Region

#Region "Form Subs"

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Check if program is already loaded
        Dim myName As String = System.IO.Path.GetFileNameWithoutExtension(Application.ExecutablePath)

        Dim procsFound = Process.GetProcessesByName(myName).Length
        If procsFound > 1 Then
            Me.Close()
        End If

        Dim n As Long = EnableMenuItem(GetSystemMenu(Me.Handle, 0), &HF060&, &H1&)

        ' Remove for debug
        Button2.Visible = False
        Button5.Visible = False

        CheckedListBox1.SetItemChecked(CheckedListBox1.Items.IndexOf("explorer"), True)
        OriginalScreenX = Get_Screen_Width()
        OriginalScreenY = Get_Screen_Height()
        Startup.AddToDebugLog("[Resolution] Got: " & OriginalScreenX.ToString & "X" & OriginalScreenY.ToString)

        Startup.AddToDebugLog("[Load] Going to load XML")
        XMLLoadList()

        NotifyIcon1.ShowBalloonTip(5000, "PalettestealerSuspender", "The PalettestealerSuspender program has been placed in your task tray." + _
                                   " Right click this icon to show the menu, manage your games, or change settings.", ToolTipIcon.Info)
        Startup.AddToDebugLog("[Load] Mainform loaded")

        FormHide()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            Dim stFilePathAndName As String = OpenFileDialog1.FileName
            TextBox2.Text = stFilePathAndName
            TextBox1.Text = MainFunctions.HandleWorkingDirectory(TextBox2.Text)
        End If
    End Sub

    Private Sub Form1_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        Startup.AddToDebugLog("[Closed] Mainform closing")
        Try
            If MonitorGameStarted = True Then MainFunctions.ResumeAll(TextBox2.Text, CheckedListToCollection(CheckedListBox1.CheckedItems), RadioToMethod())
        Catch ex As Exception
        End Try
        Startup.AddToDebugLog("[Closed] Mainform closed")
    End Sub

    Private Sub CheckedListBox1_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckedListBox1.SelectedValueChanged
        CheckedListBoxFullCheck()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        ' TODO: Due to the handling of timers/tags/resuming and suspending, only one game process is monitorred at a time.
        ' This does not matter that much for now, as it's unlikely that the user will start multiple games using the PSS.

        Dim gameFound = False
        Dim myGame As GameEntry = Nothing
        Dim myFoundGame As GameEntry = Nothing
        ' Search if a monitored game was started. In the case multiple games are started at once, one will "win".
        ' TODO: Fix this later, see note above.
        For Each element In CheckedListBox2.CheckedItems
            myGame = DirectCast(element, GameEntry)
            Dim procsFound = Process.GetProcessesByName(myGame.getGameExecutableName).Length
            Startup.AddToDebugLog("[Timer] " & myGame.getGameExecutableName & ": " & procsFound.ToString)
            If procsFound > 0 Then
                gameFound = True
                myFoundGame = myGame
                Startup.AddToDebugLog("[Timer] Found a running game")
            End If
        Next

        If MonitorGameStarted = False And gameFound = True Then
            ' A game we are monitoring was started, suspend programs now
            Startup.AddToDebugLog("[Timer] A game we are monitoring was started, suspend programs now")
            MonitorGameStarted = True
            Timer1.Tag = myFoundGame
            MainFunctions.SuspendAll(myFoundGame.getGameExecutablePath, CheckedListToCollection(CheckedListBox1.CheckedItems), RadioToMethod())
            Startup.AddToDebugLog("[Timer] All done")
        ElseIf MonitorGameStarted = True Then
            myGame = DirectCast(Timer1.Tag, GameEntry)
            Dim procsFound = Process.GetProcessesByName(myGame.getGameExecutableName).Length
            If procsFound = 0 Then ' A monitored game was stopped, resume operations
                Startup.AddToDebugLog("[Timer] A monitored game was stopped, resume operations")
                MonitorGameStarted = False
                MainFunctions.ResumeAll(myGame.getGameExecutablePath, CheckedListToCollection(CheckedListBox1.CheckedItems), RadioToMethod())
                NotifyIcon1.ShowBalloonTip(5000, "PalettestealerSuspender is resuming programs", "The game `" + myGame.getGameExecutableName + "` was closed, resuming programs...", ToolTipIcon.Info)
                Startup.AddToDebugLog("[Timer] All done")
            End If
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Startup.AddToDebugLog("[ResumeAll] Debug button pressed")
        Dim myExe As String = ""
        If MonitorGameStarted = True Then
            Try
                Dim myGame As GameEntry = DirectCast(Timer1.Tag, GameEntry)
                myExe = myGame.getGameExecutableName
            Catch ex As Exception
            End Try
        End If
        MainFunctions.ResumeAll(myExe, CheckedListToCollection(CheckedListBox1.CheckedItems), RadioToMethod())
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Try
            For i = 0 To System.Windows.Forms.Screen.AllScreens.Length() - 1
                Startup.AddToDebugLog("[ScreenInfo] " & System.Windows.Forms.Screen.AllScreens(i).ToString())
            Next
        Catch ex As Exception
        End Try

        Change_Resolution(1024, 768)
        System.Threading.Thread.Sleep(2000) 'Sleep before moving on
        Change_Resolution(OriginalScreenX, OriginalScreenY)
    End Sub

    Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem2.Click
        FormShow()
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        Startup.AddToDebugLog("[Configuration] Saving and hiding")
        RebuildMenu()
        XMLSaveList()
        FormHide()
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        If System.IO.File.Exists(TextBox2.Text) Then
            Dim gameExecutableName = System.IO.Path.GetFileNameWithoutExtension(TextBox2.Text)
            Dim game As New GameEntry(gameExecutableName, TextBox2.Text, TextBox1.Text)
            Dim processisthere = False
            For Each element In CheckedListBox2.Items
                Dim test As GameEntry = DirectCast(element, GameEntry)
                If test.getGameExecutableName = game.getGameExecutableName And test.getGameExecutablePath = game.getGameExecutablePath And test.getGameWorkingPath = game.getGameWorkingPath Then
                    processisthere = True
                    Beep()
                End If
            Next
            If Not processisthere Then
                CheckedListBox2.Items.Add(game)
                Startup.AddToDebugLog("[Adding] Added a game")
                TextBox1.Text = ""
                TextBox2.Text = ""
                RebuildMenu()
            End If
        Else
            Beep()
        End If
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Me.Close()
    End Sub

    Private Sub ToolStripMenuItem3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripMenuItem3.Click
        Me.Close()
    End Sub

    Private Sub CheckedListBox2_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckedListBox2.DoubleClick
        Dim index As Integer = CheckedListBox2.SelectedIndex
        If CheckedListBox2.SelectedIndex > -1 Then
            CheckedListBox2.Items.RemoveAt(index)
        End If
        RebuildMenu()
    End Sub

    Private Sub NotifyIcon1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotifyIcon1.DoubleClick
        FormShow()
    End Sub

    Private Sub NotifyIcon1_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseClick
        RebuildMenu()
    End Sub

#End Region

End Class

