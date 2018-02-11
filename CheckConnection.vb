Module CheckConnection
    Public Sub CheckConnection1()
        If My.Computer.Network.IsAvailable Then

        Else
            MainMenu.Timer1.Stop()
            If MsgBox("Computer is not connected - application will be disposed", MsgBoxStyle.OkOnly) = MsgBoxResult.Ok Then
                MainMenu.Dispose()
                Settings.Dispose()
            End If
        End If
    End Sub
End Module
