Imports System
Imports System.Math
Imports System.Data
Imports System.Linq
Imports System.Xml.Linq
Imports System.Globalization

Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Path


Imports System.Threading
Imports System.Threading.Tasks


Class MainWindow
    Public cronJobPath As String

    Private Sub Window_Loaded()

        MW1.Title = "CronJob Dispatcher"

        Dim MCCJ1 As New Thread(AddressOf MonitorCurrentCronJobs)
        MCCJ1.IsBackground = True
        MCCJ1.Start()
        'Thread.Sleep(TimeSpan.FromSeconds(10))

        Exit Sub

    End Sub

    Public Sub MonitorCurrentCronJobs()
        Dim forever = False

        Dim cnt = 1
        While forever = False
            Try
                Dim _dbApp1 As New ApplicationDbContext
                Using (_dbApp1)

                    Dim queryCurrentCronJobs = (From qCCJ1 In _dbApp1.CurrentCronJobs).ToList
                    Dim queryAppFolders = (From qAF1 In _dbApp1.AppFolders).Single

                    If queryCurrentCronJobs.Count > 0 Then

                        For Each cronJob1 In queryCurrentCronJobs

                            If cronJob1.CronJobType = "Test" Then
                                cronJobPath = queryAppFolders.testCronJobFolder
                            ElseIf cronJob1.CronJobType = "Schedule" Then
                                cronJobPath = queryAppFolders.scheduleCronJobFolder
                            ElseIf cronJob1.CronJobType = "Score" Then
                                cronJobPath = queryAppFolders.scoreCronJobFolder
                            End If

                            Dim queryCronJobs = (From qCJ1 In _dbApp1.CronJobs
                                                 Where qCJ1.CronJobName = cronJob1.CronJobName).Single

                            queryCronJobs.CronJobType = cronJob1.CronJobType
                            queryCronJobs.CronJobNumber = cronJob1.CronJobNumber
                            queryCronJobs.CronJobStartDateTime = cronJob1.CronJobStartDateTime

                            _dbApp1.SaveChanges()

                            Dim cronJobPath1 = CType(cronJobPath, String)
                            Process.Start(cronJobPath1 + "\setup.exe")

                            If cnt > 100 Then
                                forever = True
                            End If

                            cnt = cnt + 1
                            Thread.Sleep(TimeSpan.FromSeconds(10))
                            _dbApp1.CurrentCronJobs.Remove(cronJob1)
                            _dbApp1.SaveChanges()

                        Next

                    End If

                End Using
            Catch ex As Exception

            End Try

            Thread.Sleep(100)

        End While

    End Sub


End Class
