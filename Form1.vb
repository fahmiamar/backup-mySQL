Imports mySql.Data.MySqlClient
Imports System.IO
Public Class Form1
    Dim Conn As MySqlConnection
    Dim dt As New DataTable
    Dim da As MySqlDataAdapter
    Dim cmd As String
    Dim i As Integer

    Public Sub KoneksiDB()
        Try
            Conn = New MySqlConnection("Server=" & TxtServer.Text & ";user id=" & TxtUser.Text & ";password=" & TxtPassword.Text & ";")
            If Conn.State = ConnectionState.Closed Then
                Conn.Open()
                cboDatabase.Enabled = True
            End If
        Catch ex As Exception
            MsgBox("Koneksi gagal ..... !", MsgBoxStyle.Critical, "Gagal Koneksi !")
        End Try
    End Sub

    Private Sub BtnKoneksi_Click(sender As Object, e As EventArgs) Handles BtnKoneksi.Click
        Try
            Call KoneksiDB()
            cmd = "SELECT DISTINCT TABLE_SCHEMA FROM information_schema.TABLES"
            da = New MySqlDataAdapter(cmd, Conn)
            da.Fill(dt)
            i = 0
            cboDatabase.Enabled = True
            cboDatabase.Items.Clear()
            cboDatabase.Items.Add("+++ Select Database +++")
            While i < dt.Rows.Count
                cboDatabase.Items.Add(dt.Rows(i)(0).ToString())
                i = i + 1

            End While
            cboDatabase.SelectedIndex = 0
            BtnKoneksi.Enabled = False
            btnBackup.Enabled = True
            btnRestore.Enabled = True
            Conn.Clone()
            dt.Dispose()
            da.Dispose()
            MsgBox("Koneksi database berhasil dilakukan", MsgBoxStyle.Information, "Informasi")
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        cboDatabase.Enabled = False
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub btnBackup_Click(sender As Object, e As EventArgs) Handles btnBackup.Click
        Dim dbFile As String
        Try
            SaveFileDialog1.Filter = "SQL Dump File(*.sql|*.sql|All files(*.*)|*.*"
            SaveFileDialog1.FileName = "Backup " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".sql"
            If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                Call KoneksiDB()
                dbFile = SaveFileDialog1.FileName
                Dim BackuProses As New Process
                BackuProses.StartInfo.FileName = "cmd.exe"
                BackuProses.StartInfo.UseShellExecute = False
                BackuProses.StartInfo.WorkingDirectory = "C:\xampp\mysql\bin\"
                BackuProses.StartInfo.RedirectStandardInput = True
                BackuProses.StartInfo.RedirectStandardOutput = True
                BackuProses.Start()
                Dim BackupStream As StreamWriter = BackuProses.StandardInput
                Dim MyStreamReader As StreamReader = BackuProses.StandardOutput
                BackupStream.WriteLine("mysqldump --routines --user=" & TxtUser.Text & " --password=" & TxtPassword.Text & " -h " & TxtServer.Text & " " & cboDatabase.Text & ">""" + dbFile + """")
                BackupStream.Close()
                BackuProses.WaitForExit()
                BackuProses.Close()
                Conn.Close()
                MsgBox("Backup data mySQL telah berhasil dilakukan !", MsgBoxStyle.Information, "Backup databasse  MySQL")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub btnRestore_Click(sender As Object, e As EventArgs) Handles btnRestore.Click
        Dim dbFile As String
        Try
            OpenFileDialog1.Filter = "SQL Dump File(*.sql|*.sql|All files(*.*)|*.*"

            If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                Call KoneksiDB()
                dbFile = OpenFileDialog1.FileName
                Dim BackuProses As New Process
                BackuProses.StartInfo.FileName = "cmd.exe"
                BackuProses.StartInfo.UseShellExecute = False
                BackuProses.StartInfo.WorkingDirectory = "C:\xampp\mysql\bin\"
                BackuProses.StartInfo.RedirectStandardInput = True
                BackuProses.StartInfo.RedirectStandardOutput = True
                BackuProses.StartInfo.CreateNoWindow = True
                BackuProses.Start()
                Dim BackupStream As StreamWriter = BackuProses.StandardInput
                Dim MyStreamReader As StreamReader = BackuProses.StandardOutput
                BackupStream.WriteLine("mysql  --user=" & TxtUser.Text & " --password=" & TxtPassword.Text & " -h " & TxtServer.Text & " " & cboDatabase.Text & "<""" + dbFile + """")
                BackupStream.Close()
                BackuProses.WaitForExit()
                BackuProses.Close()
                Conn.Close()
                MsgBox("Restore your Mysql Database Created Succesfully !", MsgBoxStyle.Information, "Restore MySQL Database")

            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Class
