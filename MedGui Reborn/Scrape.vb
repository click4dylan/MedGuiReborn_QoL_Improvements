﻿Imports System.IO

Module Scrape
    Public SBoxF, SboxR As String, ScrapeForce As Integer
    Dim ConsoleID As String, ScrapeCount As Integer

    Private Sub GetConsoleID()
        ConsoleID = ""
        Try

            Select Case MedGuiR.DataGridView1.CurrentRow.Cells(5).Value()
                Case "Atari - Lynx"
                    ConsoleID = "Atari Lynx"
                Case "Bandai - WonderSwan Color"
                    ConsoleID = "WonderSwan Color"
                Case "Bandai - WonderSwan"
                    ConsoleID = "WonderSwan"
                Case "PC Engine - TurboGrafx 16"
                    ConsoleID = "TurboGrafx 16"
                Case "TurboGrafx 16 (CD)"
                    ConsoleID = "TurboGrafx CD"
                Case "Nintendo - Famicom Disk System"
                    ConsoleID = "Nintendo Entertainment System (NES)"
                Case "Nintendo - Game Boy Advance"
                    ConsoleID = "Nintendo Game Boy Advance"
                Case "Nintendo - Game Boy Color"
                    ConsoleID = "Nintendo Game Boy Color"
                Case "Nintendo - Game Boy"
                    ConsoleID = "Nintendo Game Boy"
                Case "Super Nintendo Entertainment System"
                    ConsoleID = "Super Nintendo (SNES)"
                Case ("Nintendo - Virtual Boy")
                    ConsoleID = "Nintendo Virtual Boy"
                Case "Nintendo Entertainment System"
                    ConsoleID = "Nintendo Entertainment System (NES)"
                Case "Sega - Game Gear"
                    ConsoleID = "Sega Game Gear"
                Case "Sega - Master System - Mark III"
                    ConsoleID = "Sega Master System"
                Case "Sega - Mega Drive - Genesis"
                    If UCase(MedGuiR.DataGridView1.CurrentRow.Cells(2).Value()).ToString.Contains("(US") Or
    UCase(MedGuiR.DataGridView1.CurrentRow.Cells(2).Value()).ToString.Contains("(JA") Then
                        ConsoleID = "Sega Genesis"
                    Else
                        ConsoleID = "Sega Mega Drive"
                    End If
                Case "SNK - Neo Geo Pocket Color"
                    ConsoleID = "Neo Geo Pocket Color"
                Case "SNK - Neo Geo Pocket"
                    ConsoleID = "Neo Geo Pocket"
                Case "TurboGrafx 16 (CD)"
                    ConsoleID = "TurboGrafx 16"
                Case "PC-FX"
                    ConsoleID = ""
                Case "SegaCD/MegaCD"
                    ConsoleID = "Sega CD"
                Case "Sony PlayStation"
                    ConsoleID = "Sony Playstation"
                Case "Sega Saturn"
                    ConsoleID = "Sega Saturn"
            End Select
        Catch
        End Try

    End Sub

    Public Sub GetParseXML()
        Try
            If MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() = "" Then Exit Sub
        Catch
            Exit Sub
        End Try

        GetConsoleID()

        Try
            If Directory.Exists(MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value())) Then
            Else
                Directory.CreateDirectory(MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()))
            End If

            If Directory.Exists(MedExtra & "Scraped\Temp\") Then
            Else
                Directory.CreateDirectory(MedExtra & "Scraped\Temp\")
            End If

            Dim cleanstring As String = Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value())

            'If ConsoleID = "Sony Playstation" Then cleanstring = Trim(cleanpsx(cleanstring))
            Select Case ConsoleID
                Case "Sega Saturn", "Sony Playstation"
                    cleanstring = Trim(cleanpsx(cleanstring))
            End Select

            If cleanstring.Contains(", The") Then cleanstring = Replace(cleanstring, ", The", "") : cleanstring = "The " & cleanstring
            If cleanstring.Contains("&") Then cleanstring = Replace(cleanstring, "&", "%26")
            If cleanstring.Contains("+") Then cleanstring = Replace(cleanstring, "+", "%2B")
            If cleanstring.Contains(" - ") Then cleanstring = Replace(cleanstring, " - ", ": ")

            If ScrapeCount = 6 Then
                If cleanstring.Contains("'") Then cleanstring = Replace(cleanstring, "'", "")
                If cleanstring.Contains(".") Then cleanstring = Replace(cleanstring, ".", "")
                If cleanstring.Contains(": ") Then cleanstring = Replace(cleanstring, ": ", " ")
                If cleanstring.Contains(" II ") Then cleanstring = Replace(cleanstring, "II", "2")
                If cleanstring.Contains(" III") Then cleanstring = Replace(cleanstring, "III", "3")
                ScrapeCount = 1
            End If

            SoxStatus.Text = "Waiting for Scraping..."
            SoxStatus.Label1.Text = "Downloading..."
            SoxStatus.Show()
            SoxStatus.TopMost = True

            Dim search As String
            search = "exactname="

            If File.Exists(MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & ".xml") = False Then
                ScrapeForce = 1
            End If

            If File.Exists(MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & ".xml") And ScrapeForce = 3 Then
                Dim W As New Net.WebClient
                'Dim site_scrape As String = "http://legacy.thegamesdb.net/api/GetGame.php?exactname=" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & "&platform=" & ConsoleID
                W.DownloadFile("http://legacy.thegamesdb.net/api/GetGame.php?" & search & cleanstring.ToString & "&platform=" & ConsoleID, MedExtra & "Scraped\Temp\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & ".xml")

                Dim infoReader As System.IO.FileInfo
                Dim OldXML, NewXML As Integer
                infoReader = My.Computer.FileSystem.GetFileInfo(MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & ".xml")
                OldXML = Val(infoReader.Length)
                infoReader = My.Computer.FileSystem.GetFileInfo(MedExtra & "Scraped\Temp\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & ".xml")
                NewXML = Val(infoReader.Length)

                If OldXML < NewXML Then
                    System.IO.File.Delete(MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & ".xml")
                    My.Computer.FileSystem.MoveFile(MedExtra & "Scraped\Temp\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & ".xml",
                         MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & ".xml")
                Else
                    System.IO.File.Delete(MedExtra & "Scraped\Temp\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & ".xml")
                End If
            ElseIf ScrapeForce = 0 Then
            ElseIf ScrapeForce = 1 Then
                Dim W As New Net.WebClient
                'Dim site_scrape As String = "http://legacy.thegamesdb.net/api/GetGame.php?exactname=" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & "&platform=" & ConsoleID
                W.DownloadFile("http://legacy.thegamesdb.net/api/GetGame.php?" & search & cleanstring.ToString & "&platform=" & ConsoleID, MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & ".xml")
            End If

            ReadXml()
        Catch ex As System.Net.WebException
            MessageBox.Show(ex.Message)
            If (ex.Response IsNot Nothing) Then
                Dim hr As System.Net.HttpWebResponse = DirectCast(ex.Response, System.Net.HttpWebResponse)
            End If
            SoxStatus.Close()
        End Try
    End Sub

    Private Sub ReadXml()
        Dim TGDBXml, BaseUrl, tBack, tFront, fBack, fFront As String
        TGDBXml = MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & ".xml"

        Dim reader As New System.Xml.XmlTextReader(TGDBXml)
        Dim W As New Net.WebClient

        TheGamesDB.Show()
        TheGamesDB.Label4.Text = "Genre: "
        TheGamesDB.PictureBox1.Image = Nothing
        TheGamesDB.PictureBox2.Image = Nothing

        While reader.Read()
            Dim contents As String
            reader.MoveToContent()

            If reader.NodeType = Xml.XmlNodeType.Element Then
                contents = reader.Name
            End If

            If reader.NodeType = Xml.XmlNodeType.Text Then
                Select Case contents
                    Case "baseImgUrl"
                        BaseUrl = reader.Value
                    Case "GameTitle"
                        TheGamesDB.Label1.Text = "Game Title: " & Replace(reader.Value, "&", "&&")
                    Case "Platform"
                        TheGamesDB.Label2.Text = "Platform: " & (reader.Value)
                    Case "ReleaseDate"
                        Dim fdate As String
                        If Len(reader.Value) = 10 Then fdate = reader.Value Else fdate = "0" & reader.Value
                        If Len(reader.Value) = 4 Then
                            TheGamesDB.Label3.Text = "Release Date: " & (reader.Value)
                        Else
                            Dim expenddt As Date = Date.ParseExact(fdate, "MM/dd/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo).ToString
                            TheGamesDB.Label3.Text = "Release Date: " & (expenddt)
                        End If
                    Case "Overview"
                        TheGamesDB.RichTextBox1.Text = (reader.Value)
                    Case "genre"
                        If Len(TheGamesDB.Label4.Text) <= 7 Then
                            TheGamesDB.Label4.Text = "Genre: " & (reader.Value)
                        Else
                            TheGamesDB.Label4.Text = TheGamesDB.Label4.Text & " - " & (reader.Value)
                        End If
                    Case "Players"
                        TheGamesDB.Label11.Text = "Players: " & (reader.Value)
                    Case "Publisher"
                        TheGamesDB.Label5.Text = "Publisher: " & (reader.Value)
                    Case "Developer"
                        TheGamesDB.Label6.Text = "Developer: " & (reader.Value)
                    Case "Co-op"
                        TheGamesDB.Label7.Text = "Co-op: " & (reader.Value)
                    Case "boxart"

                        If reader.Value.Contains("boxart/original/back/") Then
                            fBack = reader.Value
                            Dim SIF As String = MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & "\back_" & Path.GetFileName(fBack)

                            If File.Exists(SIF) Then
                            ElseIf ScrapeForce > 0 Then
                                W.DownloadFile(BaseUrl & fBack, SIF)
                            End If

                        ElseIf reader.Value.Contains("boxart/original/front/") Then
                            fFront = reader.Value
                            Dim SIF As String = MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & "\front_" & Path.GetFileName(fFront)

                            If File.Exists(SIF) Then
                            ElseIf ScrapeForce > 0 Then
                                W.DownloadFile(BaseUrl & fFront, SIF)
                            End If

                        End If

                End Select

                contents = ""
            End If

            If reader.HasAttributes Then 'If attributes exist
                While reader.MoveToNextAttribute()
                    Dim AtName As String = reader.LocalName
                    'Display attribute name and value.
                    If AtName = "thumb" Then
                        Select Case True
                            Case reader.Value.Contains("boxart/thumb/original/back/")
                                tBack = reader.Value
                                Dim SIF As String = MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & "\tback_" & Path.GetFileName(tBack)

                                If File.Exists(SIF) Then
                                ElseIf ScrapeForce > 0 Then
                                    W.DownloadFile(BaseUrl & tBack, SIF)
                                End If

                                SboxR = (MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & "\tback_" & Path.GetFileName(tBack))

                                Try
                                    TheGamesDB.PictureBox2.Load(SboxR)
                                Catch
                                End Try

                            Case reader.Value.Contains("boxart/thumb/original/front/")
                                tFront = reader.Value
                                Dim SIF As String = MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & "\tfront_" & Path.GetFileName(tFront)

                                If File.Exists(SIF) Then
                                ElseIf ScrapeForce > 0 Then
                                    W.DownloadFile(BaseUrl & tFront, SIF)
                                End If

                                SBoxF = (MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & "\tfront_" & Path.GetFileName(tFront))

                                Try
                                    TheGamesDB.PictureBox1.Load(SBoxF)
                                Catch
                                End Try

                                If File.Exists(MedExtra & "BoxArt\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & rn & ".png") = False Then MedGuiR.PictureBox1.Load(SBoxF) : pathimage = SBoxF

                        End Select
                    End If
                    AtName = ""
                End While
            End If
            ScrapeCount = ScrapeCount + 1
        End While
        reader.Close()

        TheGamesDB.Focus()

        If ScrapeCount = 6 Then
            GetParseXML()
        ElseIf ScrapeCount = 7 Then
            TheGamesDB.Visible = False
            MsgBox("No TheGamesDB compatible rom name or info not Available", vbOKOnly + vbInformation)
            TheGamesDB.Close()
            Try
                File.Delete(MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()) & ".xml")
                Directory.Delete(MedExtra & "Scraped\" & MedGuiR.DataGridView1.CurrentRow.Cells(5).Value() & "\" & Trim(MedGuiR.DataGridView1.CurrentRow.Cells(0).Value()))
            Catch
            End Try
        End If
        SoxStatus.Close()
        ScrapeCount = 0
    End Sub

    Public Function cleanpsx(ByVal cleanstring As String) As String
        Dim i1, i2 As Integer

        i1 = cleanstring.IndexOf("[")
        i2 = cleanstring.IndexOf("]")
        While i1 >= 0 And i2 >= 0
            cleanstring = cleanstring.Remove(i1, i2 - i1 + 1)
            i1 = cleanstring.IndexOf("[")
            i2 = cleanstring.IndexOf("]")
        End While
        cleanpsx = cleanstring
    End Function

    Public Function RemoveAmpersand(ByVal CleanAmp As String) As String
        RemoveAmpersand = Replace(CleanAmp, " &amp; ", " & ")
    End Function

    Public Function AddAmpersand(ByVal AddAmp As String) As String
        AddAmpersand = Replace(AddAmp, " & ", " %26 ")
    End Function

End Module