﻿Imports System.IO

Public NotInheritable Class DeviceAttribType
    Public Const Location As String = "LOCATION"
    Public Const ChangeType As String = "CHANGETYPE"
    Public Const EquipType As String = "EQ_TYPE"
    Public Const OSType As String = "OS_TYPE"
    Public Const StatusType As String = "STATUS_TYPE"

End Class
Public NotInheritable Class SibiAttribType

    Public Const SibiStatusType As String = "STATUS"
    Public Const SibiItemStatusType As String = "ITEM_STATUS"
    Public Const SibiRequestType As String = "REQ_TYPE"
    Public Const SibiAttachFolder As String = "ATTACH_FOLDER"
End Class

Public NotInheritable Class CheckType
    Public Const Checkin As String = "IN"
    Public Const Checkout As String = "OUT"
End Class

Public Enum EntryType
    Sibi
    Device
End Enum

Public Enum PdfFormType
    InputForm
    TransferForm
    DisposeForm
End Enum

Public Enum LiveBoxType
    DynamicSearch
    InstaLoad
    SelectValue
    UserSelect
End Enum

Public Enum FindDevType
    AssetTag
    Serial
End Enum

Public Enum CommandArgs
    TESTDB
End Enum

Public Class DBParameter
    Public Property FieldName As String
    Public Property Value As Object

    Sub New(ByVal fieldName As String, ByVal fieldValue As Object)
        Me.FieldName = fieldName
        Me.Value = fieldValue
    End Sub

End Class


Public Class DBQueryParameter
    Inherits DBParameter
    Public Property IsExact As Boolean
    Public Property OperatorString As String

    Sub New(ByVal fieldName As String, ByVal fieldValue As Object, operatorString As String)
        MyBase.New(fieldName, fieldValue)
        Me.IsExact = IsExact
        Me.OperatorString = operatorString
    End Sub

    Public Sub New(ByVal fieldName As String, ByVal fieldValue As Object, isExact As Boolean)
        MyBase.New(fieldName, fieldValue)
        Me.IsExact = isExact
        Me.OperatorString = "AND"
    End Sub

    Public Sub New(ByVal fieldName As String, ByVal fieldValue As Object, isExact As Boolean, operatorString As String)
        MyBase.New(fieldName, fieldValue)
        Me.IsExact = isExact
        Me.OperatorString = operatorString
    End Sub


End Class

Public Class SibiAttachment
    Inherits Attachment
    Public Property SelectedFolder As String

    Sub New(newFile As String, attachTable As AttachmentsBaseCols)
        MyBase.New(newFile, attachTable)
    End Sub

    Sub New(newFile As String, folderGUID As String, attachTable As AttachmentsBaseCols)
        MyBase.New(newFile, folderGUID, attachTable)
    End Sub

    Sub New(newFile As String, folderGUID As String, selectedFolder As String, attachTable As AttachmentsBaseCols)
        MyBase.New(newFile, folderGUID, attachTable)
        Me.SelectedFolder = selectedFolder
    End Sub

    Sub New(attachInfoTable As DataTable, selectedFolder As String, attachTable As AttachmentsBaseCols)
        MyBase.New(attachInfoTable, attachTable)
        Me.SelectedFolder = selectedFolder
    End Sub

End Class

Public Class DeviceAttachment
    Inherits Attachment

    Sub New(newFile As String, attachTable As AttachmentsBaseCols)
        MyBase.New(newFile, attachTable)
    End Sub

    Sub New(newFile As String, folderGUID As String, attachTable As AttachmentsBaseCols)
        MyBase.New(newFile, folderGUID, attachTable)
    End Sub

End Class

Public Class Attachment : Implements IDisposable
    Private _fileInfo As FileInfo
    Private _fileName As String
    Private _fileSize As Integer
    Private _extention As String
    Private _folderGUID As String
    Private _MD5 As String
    Private _fileUID As String
    Private _attachTable As AttachmentsBaseCols
    Private _dataStream As Stream

    Sub New()
        _fileInfo = Nothing
        _fileName = Nothing
        _fileSize = Nothing
        _extention = Nothing
        _folderGUID = Nothing
        _MD5 = Nothing
        _fileUID = Nothing
        _attachTable = Nothing
        _dataStream = Nothing
    End Sub

    ''' <summary>
    ''' Create new Attachment from a file path.
    ''' </summary>
    ''' <param name="NewFile">Full path to file.</param>
    Sub New(newFile As String, attachTable As AttachmentsBaseCols)
        _fileInfo = New FileInfo(newFile)
        _fileName = Path.GetFileNameWithoutExtension(_fileInfo.Name)
        _fileUID = Guid.NewGuid.ToString
        _MD5 = Nothing
        _fileSize = CInt(_fileInfo.Length)
        _extention = _fileInfo.Extension
        _folderGUID = String.Empty
        _attachTable = attachTable
        _dataStream = _fileInfo.OpenRead
    End Sub

    Sub New(newFile As String, folderGUID As String, attachTable As AttachmentsBaseCols)
        _fileInfo = New FileInfo(newFile)
        _fileName = Path.GetFileNameWithoutExtension(_fileInfo.Name)
        _fileUID = Guid.NewGuid.ToString
        _MD5 = Nothing
        _fileSize = CInt(_fileInfo.Length)
        _extention = _fileInfo.Extension
        _folderGUID = folderGUID
        _attachTable = attachTable
        _dataStream = _fileInfo.OpenRead
    End Sub

    Sub New(attachInfoTable As DataTable, attachTable As AttachmentsBaseCols)
        Dim TableRow As DataRow = attachInfoTable.Rows(0)
        _fileInfo = Nothing
        _dataStream = Nothing
        _attachTable = attachTable
        With TableRow
            _fileName = .Item(attachTable.FileName).ToString
            _fileUID = .Item(attachTable.FileUID).ToString
            _MD5 = .Item(attachTable.FileHash).ToString
            _fileSize = CInt(.Item(attachTable.FileSize))
            _extention = .Item(attachTable.FileType).ToString
            _folderGUID = .Item(attachTable.FKey).ToString
        End With
    End Sub

    Public ReadOnly Property FileInfo As FileInfo
        Get
            Return _fileInfo
        End Get
    End Property

    Public ReadOnly Property FileName As String
        Get
            If _fileInfo IsNot Nothing Then
                Return Path.GetFileNameWithoutExtension(_fileInfo.Name)
            Else
                Return _fileName
            End If
        End Get
    End Property

    Public ReadOnly Property FullFileName As String
        Get
            If _fileInfo IsNot Nothing Then
                Return _fileInfo.Name
            Else
                Return _fileName & Extension
            End If
        End Get
    End Property


    Public ReadOnly Property Extension As String
        Get
            If _fileInfo IsNot Nothing Then
                Return _fileInfo.Extension
            Else
                Return _extention
            End If

        End Get
    End Property

    Public ReadOnly Property Filesize As Long
        Get
            If _fileInfo IsNot Nothing Then
                Return _fileInfo.Length
            Else
                Return _fileSize
            End If
        End Get
    End Property

    Public ReadOnly Property FileUID As String
        Get
            Return _fileUID
        End Get
    End Property

    Public ReadOnly Property MD5 As String
        Get
            If _MD5 IsNot Nothing Then
                Return _MD5
            Else
                _MD5 = GetHash(_fileInfo)
                Return _MD5
            End If
        End Get
    End Property

    Public ReadOnly Property FolderGUID As String
        Get
            Return _folderGUID
        End Get
    End Property

    Public Property DataStream As Stream
        Get
            Return _dataStream
        End Get
        Set(value As Stream)
            _dataStream = value
        End Set
    End Property

    Public Property AttachTable As AttachmentsBaseCols
        Get
            Return _attachTable
        End Get
        Set(value As AttachmentsBaseCols)
            _attachTable = value
        End Set
    End Property

    Private Function GetHash(Fileinfo As FileInfo) As String
        Using HashStream As FileStream = Fileinfo.OpenRead
            Return GetHashOfFileStream(HashStream)
        End Using
    End Function

#Region "IDisposable Support"

    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If _dataStream IsNot Nothing Then _dataStream.Dispose()
                _fileInfo = Nothing
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class

Public Class GridTheme

    Sub New(highlightCol As Color, cellSelCol As Color, backCol As Color)
        RowHighlightColor = highlightCol
        CellSelectColor = cellSelCol
        BackColor = backCol
    End Sub

    Sub New()

    End Sub

    Public Property RowHighlightColor As Color
    Public Property CellSelectColor As Color
    Public Property BackColor As Color
End Class