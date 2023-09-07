Public Class Form2

    Public Mode = 0
    Public ProcessName = ""

    Public SomethingChanged = False

    Dim InitialCpuPriority = 6
    Dim InitialIOPriority = 5

    Dim PerfOptionsKey As Microsoft.Win32.RegistryKey
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.SelectedIndex = 6
        ComboBox2.SelectedIndex = 5
        InitialCpuPriority = 6
        InitialIOPriority = 5
        TextBox1.Text = ProcessName
        If Mode = 0 Then
            ComboBox1.SelectedIndex = 6
            ComboBox2.SelectedIndex = 5
        Else
            TextBox1.Enabled = False
            PerfOptionsKey = Form1.r.OpenSubKey(ProcessName & "\PerfOptions", True)
            For Each k In PerfOptionsKey.GetValueNames()
                If k.ToLower() = "cpupriorityclass" Or k.ToLower() = "iopriority" Then
                    Dim v As Integer = PerfOptionsKey.GetValue(k)
                    If k.ToLower() = "cpupriorityclass" Then
                        If v > 6 Then
                            ComboBox1.SelectedIndex = 6
                        Else
                            ComboBox1.SelectedIndex = v - 1
                            InitialCpuPriority = ComboBox1.SelectedIndex
                        End If
                    Else
                        If v > 4 Then
                            ComboBox2.SelectedIndex = 5
                        Else
                            ComboBox2.SelectedIndex = v
                            InitialIOPriority = ComboBox2.SelectedIndex
                        End If
                    End If
                End If
            Next
            'pc.Dispose()
        End If
    End Sub
    Private Sub Form2_Closed(sender As Object, e As EventArgs) Handles MyBase.Closed
        Try
            PerfOptionsKey.Dispose()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ComboBox1.SelectedIndex = InitialCpuPriority And ComboBox2.SelectedIndex = InitialIOPriority Then
            Me.Close()
            Exit Sub
        End If
        SomethingChanged = True
        If TextBox1.Text = "" Or TextBox1.Text.Replace(" ", "") = "" Then
            'wtf
            Exit Sub
        End If
        If Mode = 0 Then
            Dim FoundProcess = False
            For Each k In Form1.r.GetSubKeyNames()
                If k.ToLower() = TextBox1.Text.ToLower() Then
                    FoundProcess = True
                    Exit For
                End If
            Next
            Dim psubkey As Microsoft.Win32.RegistryKey
            If Not FoundProcess Then
                Try
                    psubkey = Form1.r.CreateSubKey(TextBox1.Text, True)
                Catch ex As Exception
                    MsgBox("Failed to create preset. Maybe invalid name?", MsgBoxStyle.Critical, "Error")
                    Exit Sub
                End Try
            Else
                psubkey = Form1.r.OpenSubKey(TextBox1.Text, True)
            End If
            Dim FoundPerfOptions = False
            For Each k In psubkey.GetSubKeyNames()
                If k.ToLower() = "perfoptions" Then
                    FoundPerfOptions = True
                    Exit For
                End If
            Next
            If Not FoundPerfOptions Then
                PerfOptionsKey = psubkey.CreateSubKey("PerfOptions", True)
            Else
                PerfOptionsKey = psubkey.OpenSubKey("PerfOptions", True)
            End If
            psubkey.Dispose()
        End If
        If ComboBox1.SelectedIndex = 6 Then
            Try
                PerfOptionsKey.DeleteValue("CpuPriorityClass")
            Catch ex As Exception
            End Try
        End If
        If ComboBox2.SelectedIndex = 5 Then
            Try
                PerfOptionsKey.DeleteValue("IoPriority")
            Catch ex As Exception
            End Try
        End If
        If ComboBox1.SelectedIndex < 6 Then
            PerfOptionsKey.SetValue("CpuPriorityClass", ComboBox1.SelectedIndex + 1, Microsoft.Win32.RegistryValueKind.DWord)
        End If
        If ComboBox2.SelectedIndex < 5 Then
            PerfOptionsKey.SetValue("IoPriority", ComboBox2.SelectedIndex, Microsoft.Win32.RegistryValueKind.DWord)
        End If
        PerfOptionsKey.Dispose()
        Me.Close()
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If TextBox1.Text = "" Or TextBox1.Text.Replace(" ", "") = "" Then
            Button1.Enabled = False
        End If
    End Sub
End Class