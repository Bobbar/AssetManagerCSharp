﻿Imports System.Data.SqlClient

Public Class Munis_Comms

#Region "Fields"

    Private Const MSSQLConnectString As String = "server=svr-munis5.core.co.fairfield.oh.us; database=mu_live; trusted_connection=True;"

#End Region

#Region "Methods"

    Public Function Return_MSSQLCommand(strSQLQry As String) As SqlCommand
        Dim conn As SqlConnection = New SqlConnection(MSSQLConnectString)
        Dim cmd As New SqlCommand
        cmd.Connection = conn
        cmd.CommandText = strSQLQry
        Return cmd
    End Function

    Public Function Return_MSSQLTable(strSQLQry As String) As DataTable
        Using conn As SqlConnection = New SqlConnection(MSSQLConnectString),
                    NewTable As New DataTable,
                    da As New SqlDataAdapter
            da.SelectCommand = New SqlCommand(strSQLQry)
            da.SelectCommand.Connection = conn
            da.Fill(NewTable)
            Return NewTable
        End Using
    End Function

    Public Async Function Return_MSSQLTableAsync(strSQLQry As String) As Task(Of DataTable)
        Using conn As SqlConnection = New SqlConnection(MSSQLConnectString),
                    NewTable As New DataTable,
                cmd As New SqlCommand(strSQLQry, conn)
            Await conn.OpenAsync
            Dim dr As SqlDataReader = Await cmd.ExecuteReaderAsync
            NewTable.Load(dr)
            Return NewTable
        End Using
    End Function

    Public Function Return_MSSQLTableFromCmd(cmd As SqlCommand) As DataTable
        Using NewTable As New DataTable,
                    da As New SqlDataAdapter(cmd)
            da.Fill(NewTable)
            cmd.Dispose()
            Return NewTable
        End Using
    End Function

    Public Async Function Return_MSSQLTableFromCmdAsync(cmd As SqlCommand) As Task(Of DataTable)
        Using conn = cmd.Connection,
                NewTable As New DataTable
            Await conn.OpenAsync
            Dim dr As SqlDataReader = Await cmd.ExecuteReaderAsync
            NewTable.Load(dr)
            cmd.Dispose()
            Return NewTable
        End Using
    End Function


#End Region

End Class