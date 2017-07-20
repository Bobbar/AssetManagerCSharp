﻿Public Class FTP_Comms

#Region "Fields"

    Private Const EncFTPUserPass As String = "BzPOHPXLdGu9CxaHTAEUCXY4Oa5EVM2B/G7O9En28LQ="
    Private Const strFTPUser As String = "asset_manager"
    Private FTPcreds As Net.NetworkCredential = New Net.NetworkCredential(strFTPUser, DecodePassword(EncFTPUserPass))
    Private intSocketTimeout As Integer = 5000

#End Region

#Region "Methods"

    Public Function Return_FTPRequestStream(strUri As String, Method As String) As IO.Stream
        Try
            Dim request As Net.FtpWebRequest = DirectCast(Net.FtpWebRequest.Create(strUri), Net.FtpWebRequest)
            With request
                .Proxy = New Net.WebProxy() 'set proxy to nothing to bypass .NET auto-detect process. This speeds up the initial connection greatly.
                .Credentials = FTPcreds
                .Method = Method
                .ReadWriteTimeout = intSocketTimeout
                Return .GetRequestStream
            End With
        Catch ex As Exception
            ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod())
            Return Nothing
        End Try
    End Function

    Public Function Return_FTPResponse(strUri As String, Method As String) As Net.WebResponse
        Dim request As Net.FtpWebRequest = DirectCast(Net.FtpWebRequest.Create(strUri), Net.FtpWebRequest)
        Try
            With request
                .Proxy = New Net.WebProxy() 'set proxy to nothing to bypass .NET auto-detect process. This speeds up the initial connection greatly.
                .Credentials = FTPcreds
                .Method = Method
                .ReadWriteTimeout = intSocketTimeout
                Return .GetResponse
            End With
        Catch ex As Exception
            Return request.GetResponse
        Finally
            request = Nothing
        End Try
    End Function

#End Region

End Class