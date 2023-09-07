Public Class Form1
    Public r As Microsoft.Win32.RegistryKey

    Sub RefreshR()
        ListBox1.Items.Clear()
        For Each subkey In r.GetSubKeyNames()
            If subkey.Contains("{") Then
                Continue For
            End If
            Dim sk = r.OpenSubKey(subkey)
            For Each nsubkey In sk.GetSubKeyNames()
                If Not nsubkey.ToLower() = "perfoptions" Then
                    Continue For
                End If
                Dim skk = sk.OpenSubKey(nsubkey)
                Dim dw = skk.GetValueNames()
                For Each d In dw
                    If d.ToLower() = "cpupriorityclass" Or d.ToLower() = "iopriority" Then
                        ListBox1.Items.Add(subkey)
                        Exit For
                    End If
                Next
                skk.Dispose()
            Next
            sk.Dispose()
        Next
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        r = My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options", True)
        RefreshR()
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        If ListBox1.SelectedIndex <> -1 Then
            Button2.Enabled = True
            Exit Sub
        End If
        Button2.Enabled = False
    End Sub

    Sub OpenEditCurrentEntry()
        If ListBox1.SelectedIndex = -1 Then
            ' wtf happened
            Button2.Enabled = False
            Exit Sub
        End If
        Dim p As New Form2()
        p.ProcessName = ListBox1.SelectedItem
        p.Mode = 1
        p.ShowDialog()
        If p.SomethingChanged Then
            RefreshR()
        End If
        p.Dispose()
    End Sub

    Private Sub ListBox1_DoubleClick(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        If ListBox1.SelectedIndex = -1 Then
            Exit Sub
        End If
        OpenEditCurrentEntry()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ListBox1.SelectedIndex = -1 Then
            Button2.Enabled = False
            Exit Sub
        End If
        OpenEditCurrentEntry()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim p As New Form2()
        p.Mode = 0
        p.ShowDialog()
        If p.SomethingChanged Then
            RefreshR()
        End If
        p.Dispose()
    End Sub
End Class
