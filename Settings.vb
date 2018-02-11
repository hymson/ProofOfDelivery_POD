Imports Setting.IniFile
Imports Microsoft.Office.Interop


Public Class Settings
    Dim App_Path = New System.IO.FileInfo(Application.ExecutablePath).DirectoryName
    Dim ini As New Setting.IniFile(App_Path & "\config.ini")
    Dim sDir = ini.ReadValue("InvoiceDirectory", "1")


    Private Sub Form2_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim mainScreen As Screen = Screen.FromPoint(Me.Location)
        Dim X As Integer = (mainScreen.WorkingArea.Width - Me.Width) / 2 + mainScreen.WorkingArea.Left
        Dim Y As Integer = (mainScreen.WorkingArea.Height - Me.Height) / 2 + mainScreen.WorkingArea.Top

        Me.StartPosition = FormStartPosition.Manual
        Me.Location = New System.Drawing.Point(X, Y)
        TextBox1.Text = ini.ReadValue("PODColumn", "1")
        TextBox2.Text = ini.ReadValue("HyperlinkColumn", "1")
        TextBox3.Text = ini.ReadValue("InvoiceLocation", "1")
        TextBox4.Text = ini.ReadValue("ScanedPODs", "1")
        TextBox5.Text = ini.ReadValue("SheetName", "1")

    End Sub


    Private Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        Try
            ini.WriteValue("PODColumn", "1", TextBox1.Text)
            ini.WriteValue("HyperlinkColumn", "1", TextBox2.Text)
            ini.WriteValue("SheetName", "1", TextBox5.Text)
            ini.WriteValue("InvoiceLocation", "1", TextBox3.Text)
            ini.WriteValue("ScanedPODs", "1", TextBox4.Text)
        Catch ex As Exception
            MsgBox("Ini file does not exist, create one in " & App_Path & "\config.ini")
        End Try

        If MsgBox("Restart necessary", MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            Dim xlApp As New Excel.Application

            Dim sInvoice = ini.ReadValue("InvoiceLocation", "1")

            xlApp.DisplayAlerts = False

            xlApp.Quit()

            xlApp = Nothing
            GC.Collect()
            GC.WaitForFullGCComplete()
            releaseObject(xlApp)


            Application.Restart()



        Else
            MsgBox("Cancel")

        End If

        Me.Close()

    End Sub


    Private Sub btnBrowse_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        Dim oOpenDiag As OpenFileDialog = New OpenFileDialog()
        Dim strFileName As String

        oOpenDiag.Title = "Open File Dialog"
        oOpenDiag.InitialDirectory = sDir
        oOpenDiag.Filter = "All files (*.*)|*.*|All files (*.*)|*.*"
        oOpenDiag.FilterIndex = 2
        oOpenDiag.RestoreDirectory = True

        If oOpenDiag.ShowDialog() = DialogResult.OK Then
            strFileName = oOpenDiag.FileName
            TextBox3.Text = strFileName
        End If
    End Sub

    Private Sub btnBrowse2_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse2.Click
       Dim dialog As New FolderBrowserDialog()
        dialog.RootFolder = Environment.SpecialFolder.Desktop
        dialog.SelectedPath = "C:\"
        dialog.Description = "Select Application Configeration Files Path"
        If dialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
            TextBox4.Text = dialog.SelectedPath
        End If
    End Sub
End Class
