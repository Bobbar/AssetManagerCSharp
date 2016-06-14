﻿Imports MySql.Data.MySqlClient
Module SecurityMod
    Public Sub GetAccessLevels()
        On Error Resume Next
        Dim reader As MySqlDataReader
        Dim strQRY = "SELECT * FROM security ORDER BY sec_access_level" ' WHERE usr_username='" & strLocalUser & "'"
        Dim cmd As New MySqlCommand(strQRY, GlobalConn)
        Dim rows As Integer
        reader = cmd.ExecuteReader
        ReDim AccessLevels(0)
        rows = -1
        With reader
            Do While .Read()
                rows += 1
                ReDim Preserve AccessLevels(rows)
                AccessLevels(rows).intLevel = !sec_access_level
                AccessLevels(rows).strModule = !sec_module
                AccessLevels(rows).strDesc = !sec_desc
            Loop
        End With
        reader.Close()
    End Sub
    Public Sub GetUserAccess()
        On Error Resume Next
        Dim reader As MySqlDataReader
        Dim strQRY = "SELECT * FROM users WHERE usr_username='" & strLocalUser & "'"
        Dim cmd As New MySqlCommand(strQRY, GlobalConn)
        reader = cmd.ExecuteReader
        With reader
            If .HasRows Then
                Do While .Read()
                    UserAccess.strUsername = !usr_username
                    UserAccess.strFullname = !usr_fullname
                    UserAccess.bolIsAdmin = Convert.ToBoolean(reader("usr_isadmin"))
                    UserAccess.intAccessLevel = !usr_access_level
                    UserAccess.strUID = !usr_UID
                Loop
            Else
                UserAccess.intAccessLevel = 0
                UserAccess.bolIsAdmin = False
            End If
        End With
        reader.Close()
    End Sub
    Public Function IsAdmin() As Boolean
        Return UserAccess.bolIsAdmin
    End Function
    Public Function CheckForAdmin() As Boolean
        If Not UserAccess.bolIsAdmin Then
            Dim blah = MsgBox("Administrator rights required for this function.", vbOKOnly + vbExclamation, "Access Denied")
            Return False
        Else
            Return True
        End If
    End Function
    Public Function CanAccess(recModule As String) As Boolean 'bitwise access levels
        Dim mask As UInteger = 1
        Dim calc_level As UInteger
        Dim UsrLevel As UInteger = UserAccess.intAccessLevel
        Dim levels As Integer
        For levels = 0 To UBound(AccessLevels)
            calc_level = UsrLevel And mask
            If calc_level <> 0 Then
                If AccessLevels(levels + 1).strModule = recModule Then
                    Return True
                End If
            End If
            mask = mask << 1
        Next
        Return False
    End Function
    Public Function CheckForAccess(recModule As String) As Boolean
        If Not CanAccess(recModule) Then
            Dim blah = MsgBox("You do not have the required rights for this function. Must have access to '" & recModule & "'.", vbOKOnly + vbExclamation, "Access Denied")
            Return False
        Else
            Return True
        End If
    End Function
End Module
