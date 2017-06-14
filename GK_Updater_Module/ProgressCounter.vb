﻿Public Class ProgressCounter
    Private _progBytesMoved As Integer
    Private _progTotalBytes As Integer
    Private _speedBytesMoved As Integer
    Private _currentTick As Integer
    Private _startTick As Integer
    Private _speedThroughput As Double
    Sub New()
        _progBytesMoved = 0
        _progTotalBytes = 0
        _speedBytesMoved = 0
        _currentTick = 0
        _startTick = 0
        _speedThroughput = 0
    End Sub
    Public Property BytesToTransfer As Integer
        Get
            Return _progTotalBytes
        End Get
        Set(value As Integer)
            _progTotalBytes = value
        End Set
    End Property
    Public ReadOnly Property Percent As Integer
        Get
            Return CInt((_progBytesMoved / _progTotalBytes) * 100)
        End Get
    End Property
    Public Property BytesMoved As Integer
        Get
            Return _progBytesMoved
        End Get
        Set(value As Integer)
            _speedBytesMoved += value
            _progBytesMoved += value
        End Set
    End Property
    Public ReadOnly Property Throughput As Double
        Get
            Return _speedThroughput
        End Get
    End Property
    Public Sub ResetProgress()
        _progBytesMoved = 0
    End Sub
    Public Sub Tick()
        _currentTick = Environment.TickCount
        If _startTick > 0 Then
            If _speedBytesMoved > 0 Then
                Dim elapTime = _currentTick - _startTick
                _speedThroughput = Math.Round((_speedBytesMoved / elapTime) / 1000, 2)
            End If
        Else
            _startTick = _currentTick
        End If
    End Sub
End Class