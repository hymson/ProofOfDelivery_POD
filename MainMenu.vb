Imports System.Net
Imports System.Text.RegularExpressions
Imports System.IO
Imports System.Net.Mime.MediaTypeNames
Imports Microsoft.Office.Interop
Imports Excel = Microsoft.Office.Interop.Excel
Imports Microsoft.Office.Interop.Excel
'Imports Setting.IniFile
Imports System.Reflection.Assembly
Imports System.Reflection
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports DotnetFiles.WebsiteSnap
Imports System.Drawing.Imaging




Public Class MainMenu
    Dim cancel As Boolean = False
    Private Sub MainMenu_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim mainScreen As Screen = Screen.FromPoint(Me.Location)
        Dim X As Integer = (mainScreen.WorkingArea.Width - Me.Width) / 2 + mainScreen.WorkingArea.Left
        Dim Y As Integer = (mainScreen.WorkingArea.Height - Me.Height) / 2 + mainScreen.WorkingArea.Top

        Me.StartPosition = FormStartPosition.Manual
        Me.Location = New System.Drawing.Point(X, Y)
        lblstatus.Visible = False
        MsgBox("Please make sure that Invoice Spreadsheet is CLOSED before you continue")
    End Sub

    Private Sub mainMenu_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing

        If e.CloseReason = CloseReason.UserClosing Then
            cancel = True
        End If

    End Sub

    'To Complete
    Dim clocation = Assembly.GetExecutingAssembly().Location
    Dim appPath = Path.GetDirectoryName(clocation & "\")


    Dim ini As New Setting.IniFile("C:\Users\Hym\Desktop\ePod\pod\bin\Debug\config.ini")
    Dim sPOD = ini.ReadValue("PODColumn", "1")
    Dim sHyper = ini.ReadValue("HyperlinkColumn", "1")
    Dim sInvoice = ini.ReadValue("InvoiceLocation", "1")
    Dim sScan = ini.ReadValue("ScanedPODs", "1")
    Dim sSheet = ini.ReadValue("SheetName", "1")
    Dim sDir = ini.ReadValue("InvoiceDirectory", "1")

    Private Sub btnImport_Click_1(ByVal sender As Object, ByVal e As EventArgs) Handles btnImport.Click
        '////////////////GET PODs NUMBERS and link hyperlinks /////////////////////////////////

        btnImport.Enabled = False
        btnSettings.Enabled = False

        Dim xlApp As New Excel.Application
        Dim xlWb As Excel.Workbook
        Dim xlsheet As Excel.Worksheet
        Dim lRow As Long = 0

        With xlApp
            .Visible = False
            '~~> Open workbook
            xlWb = .Workbooks.Open(sInvoice)
            '~~> Set it to the relevant sheet
            xlsheet = xlWb.Sheets(sSheet)
            With xlsheet
                lRow = .Range(sPOD & .Rows.Count).End(Excel.XlDirection.xlUp).Row

            End With
            
            Dim a As Excel.Range = xlsheet.Range(sPOD & "4:" & sPOD & lRow)
            For Each rngg As Excel.Range In a
                'add the data from Excel to listbox 
                ListBox1.Items.Add(rngg.Value.ToString)

            Next


        End With

        Dim i As Integer
        Dim j As Integer
        Dim link As String
        Dim iCount As Integer
        Dim progresscount As Integer = ListBox1.Items.Count
        ProgressBar1.Maximum = progresscount

        While ListBox1.Items.Count <> 0
            Dim status As Integer = lRow - 3
            Dim howManyDone As Integer
            lblstatus.Visible = True

            For iCount = 0 To iCount = ListBox1.Items.Count - 1

                Dim row As String = ListBox1.Items(iCount)
                ProgressBar1.Increment(1)
                howManyDone = howManyDone + 1
                lblstatus.Text = howManyDone & "/" & status
                ListBox1.Items.RemoveAt(iCount)
                Dim objSS As New Screenshot()
                Dim url As [String] = "https://www.royalmail.com/track-your-item/print-proof-of-delivery/" & row
                Dim sResult As Screenshot.SnapResult
                sResult = objSS.SnapWebpage(url)
                sResult = Screenshot.SnapResult.Snapped
                'cancel is checking if sesion was terminated - [button exit was clicked] - we dont want any undone excel processes in task manager 
                If cancel = True Then
                    xlApp.DisplayAlerts = False
                    xlsheet.SaveAs(sInvoice)
                    xlWb.Close()
                    xlApp.Quit()
                    DataGridView1.Rows.Clear()
                    xlApp = Nothing
                    GC.Collect()
                    GC.WaitForFullGCComplete()
                    '~~> Clean Up
                    releaseObject(xlsheet)
                    releaseObject(xlWb)
                    releaseObject(xlApp)
                    Exit Sub
                End If
                
                PictureBox2.Image = objSS.GetImage()



                If PictureBox2.Image IsNot Nothing Then
                    Dim fileName As String = (ini.ReadValue("ScanedPODs", "1") + "\" + row + ".png")
                    objSS.ImageFormat = Screenshot.ImageFormats.PNG
                    objSS.SaveImage(fileName)
                    DataGridView1.Rows.Add(ini.ReadValue("ScanedPODs", "1") + "\" + row + ".png")
                    For i = 0 To DataGridView1.RowCount - 2

                        For j = 0 To DataGridView1.ColumnCount - 1
                            link = DataGridView1(j, i).Value & "".ToString()
                            xlsheet.Hyperlinks.Add(xlsheet.Range(sHyper & i + 4), link, "", "Link", "Link")

                        Next

                    Next
                    PictureBox2.Image.Dispose()
                    objSS.Dispose()
                End If

            Next

        End While
        MsgBox("ePOD's imported to excel spreadsheet")
        btnImport.Enabled = True
        btnSettings.Enabled = True

        xlApp.DisplayAlerts = False

        xlsheet.SaveAs(sInvoice)


        xlWb.Close()
        xlApp.Quit()

        DataGridView1.Rows.Clear()
        xlApp = Nothing
        GC.Collect()
        GC.WaitForFullGCComplete()
        '~~> Clean Up
        releaseObject(xlsheet)
        releaseObject(xlWb)
        releaseObject(xlApp)
    End Sub

    Private Sub btnExit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnExit.Click
        Cancel = True
        Dim result As Integer = MessageBox.Show("Terminate this session?", "ePOD Manager", MessageBoxButtons.YesNo)
        If result = DialogResult.Yes Then

            Me.Dispose()

        ElseIf result = DialogResult.No Then

            Exit Sub

        End If

    End Sub


    'every 3 second function is checking if computer have internet connection
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        CheckConnection1()
    End Sub

    Private Sub btnSettings_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSettings.Click
        Settings.Show()
    End Sub



End Class
