﻿Imports System.IO

Module scan
    Public consoles, icon_console, gif, real_name, searchrom, Counter As String, stopscan, skipother As Boolean, dettaglio As FileInfo

    Public Sub scansiona()
        skipother = True
        If Dir(T_MedExtra & TempFolder & "\*.*") = "" Then Exit Sub
        searchrom = MedGuiR.TextBox3.Text & "*.*"
        Dim fileEntries As String() = Directory.GetFiles(T_MedExtra & TempFolder, searchrom)

        'If MedGuiR.CheckBox14.Checked = False Then
        If fileEntries.GetLength(0) > 500 Then
            Dim msgfetr As String
            msgfetr = MsgBox("The " & TempFolder & " contain " & fileEntries.GetLength(0).ToString & "  files, the scan might take a long time" & vbCrLf &
"Do you want to continue the operation?", MsgBoxStyle.YesNo + MsgBoxStyle.Exclamation)
            If msgfetr = vbNo Then Exit Sub
        End If
        'End If

        If IO.File.Exists(MedExtra & "RomExt.ini") = False Then DownloadRomext() : Threading.Thread.Sleep(500)

        Dim defreeze As Integer = 0
        For Each fileName As String In fileEntries
            If Path.HasExtension(fileName) = False Then Continue For
            Dim oRead As StreamReader
            Try
                oRead = File.OpenText(MedExtra & "RomExt.ini")
                While oRead.Peek <> -1

                    If LCase(Path.GetExtension(fileName)) = (oRead.ReadLine()) Then
                        ext = ""
                        Counter = Counter + 1
                        percorso = fileName
                        get_ext()

                        If dettaglio.Length < 40000000 Then 'And LCase(dettaglio.Extension) <> ".bin"
                            Select Case LCase(Path.GetExtension(fileName))
                                Case ".zip", ".rar", ".7z"
                                Case Else
                                    If consoles = "ss" And LCase(dettaglio.Extension) = ".bin" Then
                                    ElseIf consoles = "psx" And LCase(dettaglio.Extension) = ".bin" Then
                                    ElseIf consoles = "pcfx" And LCase(dettaglio.Extension) = ".bin" Then
                                    Else
                                        decript()
                                    End If
                            End Select
                        End If

                        Try
                            MedGuiR.MainGrid.Rows(Counter - 1).Cells(3).ToolTipText = "CRC " & base_file
                        Catch
                        End Try
                    End If

                End While

                oRead.Dispose()
                oRead.Close()
            Catch ex As Exception
                MGRWriteLog("Scan - Scansiona: " & ex.Message)
            Finally
                oRead.Dispose()
                oRead.Close()
            End Try

            MedGuiR.TextBox2.Visible = False
            MedGuiR.ProgressBar1.Visible = True
            MedGuiR.ProgressBar1.Maximum = fileEntries.GetLength(0)
            MedGuiR.ProgressBar1.PerformStep()
            MedGuiR.Label95.Text = "Scan " & MedGuiR.ProgressBar1.Value & "/" & fileEntries.GetLength(0)
            MedGuiR.Label95.Refresh()
            defreeze += 1
            If (defreeze Mod 50) = 0 Then Application.DoEvents()

        Next
        MedGuiR.ProgressBar1.Value = 0
        MedGuiR.ProgressBar1.Visible = False
        MedGuiR.Label95.Text = "Custom Setting"
        MedGuiR.TextBox2.Visible = True
        SoxStatus.Close()
        RenameLikeDat = 0
    End Sub

    Public Sub decript()
        filepath = percorso
        GetCRC32()
        LMain()
    End Sub

    Public Sub get_ext()
        Try
            dettaglio = New FileInfo(percorso)
            ext = LCase(dettaglio.Extension)
            romname = Path.GetFileNameWithoutExtension(dettaglio.Name)
            full_path = dettaglio.FullName

            Select Case LCase(ext)
                Case ".bin", ".iso"
                    dettaglio = My.Computer.FileSystem.GetFileInfo(percorso)
                    If dettaglio.Length > 16000000 And Dir(Replace(percorso, dettaglio.Extension, ".cue")) <> "" Then
                        ext = ".cue"
                        percorso = Replace(percorso, dettaglio.Extension, ".cue")
                    End If
            End Select

            estensione()
        Catch ex As Exception
        End Try
    End Sub

    Public Sub estensione()
        Select Case LCase(ext)
            Case ".21", ".30", ".31", ".sd0"
                If Val(vmedClear) > 12800 Then
                    consoles = "sasplay"
                    gif = "arcade"
                    real_name = "Sega Arcade SCSP Player"
                    fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\CUE.dat"
                End If
            Case ".1", ".2", ".3", ".4", ".5", ".6", ".7", ".ic8", ".u1", ".ic13", ".nv"
                If Val(vmedClear) > 12900 Then
                    consoles = "ss"
                    gif = "arcade"
                    real_name = "Sega Titan Video"
                    fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\CUE.dat"
                    skipother = True
                End If
            Case ".po", ".dsk", ".do", ".woz", ".d13", ".mai", ".hdv"
                consoles = "apple2"
                gif = "apple2"
                real_name = "Apple II/II+"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\CUE.dat"
            Case ".gb"
                consoles = "gb"
                gif = "gb"
                real_name = "Nintendo - Game Boy"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Nintendo - Game Boy.dat"
            Case ".gbc"
                consoles = "gb"
                gif = "gbc"
                real_name = "Nintendo - Game Boy Color"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Nintendo - Game Boy Color.dat"
            Case ".gg"
                consoles = "gg"
                gif = "gg"
                real_name = "Sega - Game Gear"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Sega - Game Gear.dat"
            Case ".gba", ".gsf"
                consoles = "gba"
                gif = "gba"
                real_name = "Nintendo - Game Boy Advance"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Nintendo - Game Boy Advance.dat"
            Case ".minigsf", ".gsflib"
                consoles = "gba"
                gif = "gba"
                real_name = "Nintendo - Game Boy Advance - Music Module"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Nintendo - Game Boy Advance.dat"
            Case ".lnx"
                consoles = "lynx"
                gif = "lynx"
                real_name = "Atari - Lynx"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Atari - Lynx.dat"
            Case ".bin"
                Select Case LCase(dettaglio.Extension)
                    Case ".zip", ".rar", ".7z"
                        consoles = "md"
                        gif = "md"
                        real_name = "Sega - Mega Drive - Genesis"
                        fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Sega - Mega Drive - Genesis.dat"
                    Case Else
                        decrunch_size = dettaglio.Length

                        If decrunch_size <= 16000000 Then
                            Dim parsebin As String = ""
                            Using fs As New FileStream(percorso, FileMode.Open, FileAccess.Read)
                                For offset = 0 To 352
                                    parsebin = parsebin & Convert.ToChar(fs.ReadByte())
                                Next offset
                            End Using

                            If parsebin.Contains("VGM PLAYER") Then
                                consoles = "md"
                                gif = "md"
                                real_name = "Sega - 8/16 bit console - Music Module"
                                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\none.dat"
                                If MedGuiR.MainGrid.Rows.Count > 0 Then MedGuiR.MainGrid.CurrentRow.Cells(0).Value() = romname
                            ElseIf parsebin.Contains("SEGA MEGA DRIVE") Or parsebin.Contains("SEGA GENESIS") Then 'Or parsebin.Contains("SEGA ") Then
                                consoles = "md"
                                gif = "md"
                                real_name = "Sega - Mega Drive - Genesis"
                                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Sega - Mega Drive - Genesis.dat"
                            Else
                                consoles = ""
                                gif = ""
                                real_name = ""
                                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\CUE.dat"
                            End If
                        End If
                End Select

                If skipother = False Then
                    If decrunch_size > 16000000 And Dir(Replace(percorso, dettaglio.Extension, ".cue")) = "" Then
                        If stopiso = False Then Make_Temp_CUE()
                    End If
                End If

            Case ".smd", ".gen", ".32x", ".md"
                consoles = "md"
                gif = "md"
                real_name = "Sega - Mega Drive - Genesis"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Sega - Mega Drive - Genesis.dat"
            Case ".vgm", ".vgz"
                consoles = "md"
                gif = "md"
                real_name = "Sega - 8/16 bit console - Music Module"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\none.dat"

            Case ".gbs"
                consoles = "gb"
                gif = "gb"
                real_name = "Nintendo - Game Boy - Music Module"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Gameboy Sound System.dat"
            Case ".ngp"
                consoles = "ngp"
                gif = "ngp"
                real_name = "SNK - Neo Geo Pocket"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\SNK - Neo Geo Pocket.dat"
            Case ".ngc", ".npc"
                consoles = "ngp"
                gif = "ngpc"
                real_name = "SNK - Neo Geo Pocket Color"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\SNK - Neo Geo Pocket Color.dat"
            Case ".nes", ".unf", ".nez"
                consoles = "nes"
                gif = "nes"
                real_name = "Nintendo Entertainment System"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Nintendo Entertainment System.dat"
            Case ".nsf"
                consoles = "nes"
                gif = "nes"
                real_name = "Nintendo Entertainment System - Music Module"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\NES Sound Format.dat"
            Case ".fds"
                consoles = "nes"
                gif = "fds"
                real_name = "Nintendo - Famicom Disk System"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Nintendo - Famicom Disk System.dat"
            Case ".pce"
                consoles = "pce"
                gif = "pce"
                real_name = "PC Engine - TurboGrafx 16"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\NEC - PC Engine - TurboGrafx 16.dat"
            Case ".hes"
                consoles = "pce"
                gif = "pce"
                real_name = "PC Engine - TurboGrafx 16 - Music Module"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Hudson Entertainment Sound.dat"
            Case ".psf", ".psf1", ".minipsf"
                consoles = "psx"
                gif = "psx"
                real_name = "PlayStation One - Music Module"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\none.dat"
            Case ".exe"
                decrunch_size = dettaglio.Length
                If decrunch_size <= 3145728 Then
                    Dim parsebin As String = ""
                    Using fs As New FileStream(percorso, FileMode.Open, FileAccess.Read)
                        For offset = 0 To 100
                            parsebin = parsebin & Convert.ToChar(fs.ReadByte())
                        Next offset
                    End Using

                    If parsebin.Contains("PS-X EXE") Then
                        consoles = "psx"
                        gif = "psx"
                        real_name = "PSX-EXE executable file"
                        fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\none.dat"
                    Else
                        consoles = ""
                        gif = ""
                        real_name = ""
                        fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\none.dat"
                    End If
                End If
            Case ".ssf", ".minissf"
                consoles = "ssfplay"
                gif = "ss"
                real_name = "Sega Saturn - Music Module"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\none.dat"
            Case ".sms"
                consoles = "sms"
                gif = "sms"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Sega - Master System - Mark III.dat"
                real_name = "Sega - Master System - Mark III"
            Case ".smc", ".fig", ".sfc", ".swc", ".ufo", ".gd3", ".gd7", ".dx2", ".mgd", ".mgh", ".bs", ".st"
                consoles = "snes"
                gif = "snes"
                real_name = "Super Nintendo Entertainment System"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Nintendo - Super Nintendo Entertainment System.dat"
            Case ".rsn"
                consoles = "snes_faust"
                gif = "snes"
                real_name = "Super Nintendo - Music Module"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\RAR Super Nintendo.dat"
            Case ".spc"
                consoles = "snes_faust"
                gif = "snes"
                real_name = "Super Nintendo - Music Module"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\none.dat"
            Case ".vb", ".vboy"
                consoles = "vb"
                gif = "vb"
                real_name = "Virtual Boy"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Nintendo - Virtual Boy.dat"
            Case ".ws"
                consoles = "wswan"
                gif = "wswan"
                real_name = "Bandai - WonderSwan"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Bandai - WonderSwan.dat"
            Case ".wsr"
                consoles = "wswan"
                gif = "wswan"
                real_name = "Bandai - WonderSwan - Music Module"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\none.dat"
            Case ".wsc"
                consoles = "wswan"
                gif = "wswanc"
                real_name = "Bandai - WonderSwan Color"
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\Bandai - WonderSwan Color.dat"
            Case ".zip", ".rar", ".7z"
                scan_compressed()

                'CONTROLLA SE DISABILITANDO CREA CASINI IN FUTURO
                'ext = ""
                '''''''''''''''''''''''''''''''''''''''''''''''''

                        'Case ".rar", ".7z"
                        'ClearFile()
                        'extract_7z()
            Case ".cue", ".ccd"
                If skipother = True Then
                    IsoSelector.detectcdtype()
                Else
                    If stopiso = False Then
                        If stopscan = False Then
                            If MedGuiR.CheckBox21.Checked = True Then
                                IsoSelector.detectcdtype()
                            Else
                                cd_consoles()
                            End If
                        End If
                    End If
                End If
            Case ".m3u"
                If skipother = True Then
                    IsoSelector.DetectM3U()
                Else
                    If stopiso = False Then
                        If stopscan = False Then
                            If MedGuiR.CheckBox21.Checked = True Then
                                IsoSelector.DetectM3U()
                            Else
                                cd_consoles()
                            End If
                        End If
                    End If
                End If
            Case ".toc"
                If skipother = False Then
                    If stopiso = False Then
                        If stopscan = False Then cd_consoles()
                    End If
                End If
            Case ".iso"
                If skipother = False Then
                    If stopiso = False Then Make_Temp_CUE()
                End If
            Case ".cfs", ".ciso"
                Pismo()
                If checkpismo = False Then
                    MsgBox("I can't mount .cfs, you need to install Pismo File Mount Audit Package", MsgBoxStyle.Exclamation + vbOKOnly, "Missing Pismo File Mount...")
                    consoles = ""
                    ext = ""
                    fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\none.dat"
                Else
                    If skipother = False Then
                        If stopiso = False Then
                            stopscan = True
                            MountPismo()
                            Counter = 1
                            RecuScan()
                        End If
                    End If
                End If
            Case ".ecm"
                If File.Exists(MedExtra & "Plugins\unecm.exe") Then

                    If skipother = False Then
                        If stopiso = False Then
                            stopscan = True
                            unECM()
                        End If
                    End If
                Else
                    MsgBox("I can't extract .ECM, missing unecm.exe into Plugins folder", MsgBoxStyle.Exclamation + vbOKOnly, "Missing unecm...")
                    consoles = ""
                    ext = ""
                    fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\none.dat"
                End If

            Case ".pbp"
                If File.Exists(MedExtra & "Plugins\copstation\popstation.exe") Then

                    If skipother = False Then
                        If stopiso = False Then
                            stopscan = True
                            unPBP()
                        End If
                    End If
                Else
                    MsgBox("I can't extract .PBP, missing Copstation.exe into Plugins folder", MsgBoxStyle.Exclamation + vbOKOnly, "Missing copstation...")
                    consoles = ""
                    ext = ""
                    fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\none.dat"
                End If
            Case ".chd"
                If File.Exists(MedExtra & "Plugins\chdman.exe") Then

                    If skipother = False Then
                        If stopiso = False Then
                            stopscan = True
                            unCHD()
                        End If
                    End If
                Else
                    MsgBox("I can't extract .CHD, missing chdman.exe into Plugins folder", MsgBoxStyle.Exclamation + vbOKOnly, "Missing chdman...")
                    consoles = ""
                    ext = ""
                    fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\none.dat"
                End If
            Case ".zst"
                If Val(vmedClear) > 12710 Then
                    consoles = "generic"
                    gif = "game"
                    real_name = "Generic Zstd compressed file"
                    fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\none.dat"
                End If
            Case Else
                consoles = ""
                ext = ""
                fileTXT = MedExtra & "DATs\" & MedGuiR.ComboBox1.Text & "\none.dat"
        End Select

        Dim IsIcon As String
        If File.Exists(MedExtra & "Resource\System\" & UCase(gif) & ".ico") Then
            IsIcon = ".ico"
        Else
            IsIcon = ".gif"
        End If
        icon_console = MedExtra & "Resource\System\" & UCase(gif) & IsIcon

    End Sub

    Public Sub GBS2GB()
        If File.Exists(MedExtra & "Plugins\gbs2gb\GBS2GB.exe") Then
            wDir = (MedExtra & "Plugins\gbs2gb")
            tProcess = "GBS2GB"
            Arg = percorso
            StartProcess()

            execute.WaitForExit()
            If File.Exists(percorso.Substring(0, percorso.Length - 1)) Then
                My.Computer.FileSystem.MoveFile(percorso.Substring(0, percorso.Length - 1), MedExtra & "RomTemp\" & Path.GetFileName(percorso.Substring(0, percorso.Length - 1)), True)
                percorso = MedExtra & "RomTemp\" & Path.GetFileName(percorso.Substring(0, percorso.Length - 1))
            Else
                Exit Sub
            End If

            get_ext()
        Else
            MsgBox("I can't convert gbs file, GBS2GB missing!", MsgBoxStyle.Exclamation + vbOKOnly, "Missing plugin..")
        End If

    End Sub

    Public Sub VGtoBIN()

        If File.Exists(MedExtra & "Plugins\vgmPlay\vgmPlay.exe") Then
            wDir = (MedExtra & "Plugins\vgmPlay")
            tProcess = "vgmPlay"
            Dim PVG, dir1, dir2, Vg_append As String
            PVG = ""
            Vg_append = ""

            PVG = Chr(34) & percorso & Chr(34)

            Arg = PVG
            StartProcess()

            dir1 = Path.GetDirectoryName(percorso)
            dir2 = Path.GetDirectoryName(dir1)

            If dir2 <> "" Then
                Vg_append = Replace(percorso, dir2 & "\", "")
            Else
                Vg_append = percorso
            End If

            Vg_append = Replace(Vg_append, "\", " - ")

            execute.WaitForExit()
            If File.Exists(MedExtra & "Plugins\vgmPlay\vgmPlay.bin") Then
                My.Computer.FileSystem.MoveFile(MedExtra & "Plugins\vgmPlay\vgmPlay.bin", MedExtra & "RomTemp\" & Path.GetFileNameWithoutExtension(Vg_append) & ".bin", True)
                percorso = MedExtra & "RomTemp\" & Path.GetFileNameWithoutExtension(Vg_append) & ".bin"
            Else
                Exit Sub
            End If

            get_ext()
        Else
            MsgBox("I can't convert vgz/vgm file, vgmPlay missing!", MsgBoxStyle.Exclamation + vbOKOnly, "Missing plugin..")
        End If

    End Sub

    Private Sub unECM()
        wDir = (MedExtra & "Plugins")
        tProcess = "unecm"
        Dim PECM As String
        If LCase(percorso).Contains(".bin.") Then
            PECM = ""
        Else
            PECM = Chr(34) & Replace(percorso, dettaglio.Extension, ".bin") & Chr(34)
        End If
        Arg = Chr(34) & percorso & Chr(34) & " " & PECM

        SoxStatus.Text = "Waiting For UNECM Conversion..."
        SoxStatus.Label1.Text = "Decrunching..."
        SoxStatus.Show()
        Application.DoEvents()

        StartProcess()

        execute.WaitForExit()
        If PECM = "" Then
            percorso = Replace(percorso, ".bin.ecm", ".bin")
        Else
            percorso = Replace(percorso, dettaglio.Extension, ".bin")
        End If

        SoxStatus.Close()
        get_ext()
        stopscan = False
    End Sub

    Private Sub unPBP()
        Dim cleanPBP As String = Path.GetFileNameWithoutExtension(percorso)
        wDir = Path.GetDirectoryName(percorso)
        File.Copy(MedExtra & "Plugins\copstation\popstation.exe", Path.Combine(wDir, "popstation.exe"), True)
        File.Copy(MedExtra & "Plugins\copstation\cygwin1.dll", Path.Combine(wDir, "cygwin1.dll"), True)
        File.Copy(MedExtra & "Plugins\copstation\cygz.dll", Path.Combine(wDir, "cygz.dll"), True)

        If cleanPBP <> "EBOOT" Then My.Computer.FileSystem.RenameFile(percorso, "EBOOT.PBP")

        tProcess = "popstation"
        Arg = "-iso " & Chr(34) & cleanPBP & ".iso" & Chr(34)

        SoxStatus.Text = "Waiting For Copstation Conversion..."
        SoxStatus.Label1.Text = "Converting..."
        SoxStatus.Show()
        Application.DoEvents()

        StartProcess()
        execute.WaitForExit()

        If File.Exists(Path.Combine(wDir, cleanPBP & ".iso")) Then
            If cleanPBP <> "EBOOT" Then My.Computer.FileSystem.RenameFile(Path.Combine(wDir, "EBOOT.PBP"), Path.GetFileName(percorso))
            'File.Delete(Path.Combine(wDir, "EBOOT.PBP"))
            File.Delete(Path.Combine(wDir, "popstation.exe"))
            File.Delete(Path.Combine(wDir, "cygwin1.dll"))
            File.Delete(Path.Combine(wDir, "cygz.dll"))
            percorso = Path.Combine(wDir, cleanPBP & ".iso")
        Else
            Exit Sub
        End If

        SoxStatus.Close()
        get_ext()
        stopscan = False
    End Sub

    Private Sub unCHD()
        Dim cleanCHD As String = Path.Combine(Path.GetDirectoryName(percorso), Path.GetFileNameWithoutExtension(percorso))

        wDir = (MedExtra & "Plugins")
        tProcess = "chdman"

        Arg = "extractcd -i " & Chr(34) & percorso & Chr(34) & " -o " & Chr(34) & cleanCHD & ".cue" & Chr(34) & " -ob " & Chr(34) & cleanCHD & ".bin" & Chr(34)

        SoxStatus.Text = "Waiting For CHD Conversion..."
        SoxStatus.Label1.Text = "Decrunching..."
        SoxStatus.Show()
        Application.DoEvents()

        StartProcess()

        execute.WaitForExit()

        If File.Exists(cleanCHD & ".bin") Then
            percorso = cleanCHD & ".cue"
        Else
            Exit Sub
        End If

        SoxStatus.Close()
        get_ext()
        stopscan = False
    End Sub

    Public Sub Make_Temp_CUE()
        If stopscan = False Then cd_consoles() Else Exit Sub

        Dim C_TRACK As String
        Select Case consoles
            Case "psx"
                C_TRACK = "  TRACK 01 MODE2/2352"
            Case "pcfx"
                MsgBox("I'm sorry, you can not create a generic CUE file for PC-FX games", vbOKOnly + vbExclamation)
                Exit Sub
            Case "pce"
                C_TRACK = " TRACK 02 MODE1/2048"
            Case "ss"
                C_TRACK = "  TRACK 01 MODE1/2352"
        End Select

        Dim RIso As String
        RIso = MsgBox("Start an ISO file is not a good idea, Mednafen don't support it." & vbCrLf &
               "Anyway i will make a standard .CUE file to force load ISO/BIN game." & vbCrLf &
                "Expect interruptions during startup and almost certainly the lack of sound during emulation", vbOKCancel + vbExclamation)

        If RIso = vbCancel Then stopscan = True : Exit Sub

        MedGuiR.Label34.Text = percorso
        biso = Path.GetFileName(MedGuiR.Label34.Text)

        strimg = "FILE " & Chr(34) & biso & Chr(34) & " Binary" & vbCrLf & C_TRACK & vbCrLf & "    INDEX 01 00:00:00"
        miso = "cue"
        ext = ".cue"
        If MedGuiR.tempiso = "" Then MedGuiR.tempiso = Replace(MedGuiR.Label34.Text, Path.GetExtension(MedGuiR.Label34.Text), "." & miso)
        MedGuiR.Make_CUE()
    End Sub

    Public Sub cd_consoles()
        IsoSelector.CheckBox1.Checked = False
        IsoSelector.CheckBox1.Enabled = False
        IsoSelector.UNI.Items.Clear()
        IsoSelector.UNI.Text = ""
        IsoSelector.UNI.Enabled = False
        IsoSelector.ShowDialog()
    End Sub

    Public Sub detect_icon()
        Select Case real_name
            Case "Apple II/II+"
                gif = "apple2"
            Case "Nintendo - Game Boy", "Nintendo - Game Boy - Music Module"
                gif = "gb"
            Case "Nintendo - Game Boy Color"
                gif = "gbc"
            Case "Sega - Game Gear"
                gif = "gg"
            Case "Nintendo - Game Boy Advance", "Nintendo - Game Boy Advance - Music Module"
                gif = "gba"
            Case "Atari - Lynx"
                gif = "lynx"
            Case "Sega - Mega Drive - Genesis", "Sega - 8/16 bit console - Music Module"
                gif = "md"
            Case "SNK - Neo Geo Pocket"
                gif = "ngp"
            Case "SNK - Neo Geo Pocket Color"
                gif = "ngpc"
            Case "Nintendo Entertainment System", "Nintendo Entertainment System - Music Module"
                gif = "nes"
            Case "Nintendo - Famicom Disk System"
                gif = "fds"
            Case "PC Engine - TurboGrafx 16", "PC Engine - TurboGrafx 16 - Music Module"
                gif = "pce"
            Case "Sega - Master System - Mark III"
                gif = "sms"
            Case "Super Nintendo Entertainment System", "Super Nintendo - Music Module"
                gif = "snes"
            Case "Sega Saturn", "Sega Saturn - Music Module"
                gif = "ss"
            Case "Sega Arcade SCSP Player", "Sega Titan Video"
                gif = "arcade"
            Case "Virtual Boy"
                gif = "vb"
            Case "Bandai - WonderSwan", "Bandai - WonderSwan - Music Module"
                gif = "wswan"
            Case "Bandai - WonderSwan Color"
                gif = "wswanc"
            Case "TurboGrafx 16 (CD)"
                gif = "pcecd"
            Case "PC-FX"
                gif = "pcfx"
            Case Is = "SegaCD/MegaCD"
                gif = "mdcd"
            Case Is = "Sony PlayStation", "PlayStation One - Music Module"
                gif = "psx"
            Case Is = "Audio CD"
                gif = "cdplay"
            Case Is = "Generic Zstd compressed file", "Sega Arcade SCSP Player"
                gif = "game"
            Case Else
                gif = "unknow"
        End Select
    End Sub

    Public Sub RecuScan()
        Dim root = TempFolder
        If Directory.Exists(root) = False Then
            MsgBox("No directory founded in " & root, MsgBoxStyle.Exclamation + vbOKOnly, "Missing folder...")
            Exit Sub
        End If
        Dim ResRecu As MsgBoxResult
        Dim subdirectoryEntries As String() = Directory.GetDirectories(root)
        If subdirectoryEntries.Length > 0 Then

            Dim countallfiles As Integer = 0
            For i = 0 To subdirectoryEntries.Length - 1
                Dim fcount As Integer = Directory.GetFiles(subdirectoryEntries(i).ToString & "\", "*.*", SearchOption.AllDirectories).Length
                countallfiles = countallfiles + fcount

                If countallfiles = 0 Then
                    MsgBox("No files founded in " & root & " Folder...", vbInformation + vbOKOnly)
                    MedGuiR.MainGrid.Rows.Clear()
                    TempFolder = ""
                    Exit Sub
                Else
                    Exit For
                End If
            Next

            If MedGuiR.FirstStart = True Then Exit Sub

            If checkpismo = False Then
                ResRecu = MsgBox("Do you want to make a recursive folder scan?" &
                          vbCrLf & "Recursive scan could be inaccurate, could generate error and take many time!", vbYesNo + MsgBoxStyle.Exclamation)
            Else
                ResRecu = vbYes
            End If

            If ResRecu = vbYes Then
                MedGuiR.MainGrid.Rows.Clear()
                stopiso = True
                scansiona()
                For Each subdirectory As String In subdirectoryEntries
                    LoadSubDirs(subdirectory)
                Next
                SoxStatus.Close()
            Else
                TempFolder = ""
                Exit Sub
            End If
        Else
            MedGuiR.ScanFolder()
        End If
        checkpismo = False
    End Sub

    Private Sub LoadSubDirs(dir As String)
        TempFolder = dir
        PopulateDataGridRecursive()
        Dim subdirectoryEntries As String() = Directory.GetDirectories(dir)
        For Each subdirectory As String In subdirectoryEntries
            LoadSubDirs(subdirectory)
        Next

    End Sub

    Private Sub PopulateDataGridRecursive()
        stopiso = True
        scansiona()
    End Sub

End Module