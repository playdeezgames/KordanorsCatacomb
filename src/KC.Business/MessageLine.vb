Friend Class MessageLine
    Implements IMessageLine

    Private _data As WorldData
    Private _messageId As Integer
    Private _lineId As Integer
    Private ReadOnly Property MessageLineData As MessageLineData
        Get
            Return _data.Messages(_messageId).Lines(_lineId)
        End Get
    End Property

    Public ReadOnly Property Text As String Implements IMessageLine.Text
        Get
            Return MessageLineData.Text
        End Get
    End Property

    Public Sub New(data As WorldData, messageId As Integer, lineId As Integer)
        Me._data = data
        Me._messageId = messageId
        Me._lineId = lineId
    End Sub
End Class
