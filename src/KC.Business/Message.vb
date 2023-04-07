Friend Class Message
    Implements IMessage

    Private ReadOnly _data As WorldData
    Private ReadOnly _messageId As Integer
    Private ReadOnly Property MessageData As MessageData
        Get
            Return _data.Messages(_messageId)
        End Get
    End Property

    Public ReadOnly Property Lines As IEnumerable(Of IMessageLine) Implements IMessage.Lines
        Get
            Dim result As New List(Of IMessageLine)
            For lineId = 0 To MessageData.Lines.Count - 1
                result.Add(New MessageLine(_data, _messageId, lineId))
            Next
            Return result
        End Get
    End Property

    Public Property Cue As SoundCue Implements IMessage.Cue
        Get
            Return MessageData.Cue
        End Get
        Set(value As SoundCue)
            MessageData.Cue = value
        End Set
    End Property

    Public Sub New(data As WorldData, messageId As Integer)
        Me._data = data
        Me._messageId = messageId
    End Sub

    Public Sub AddLine(mood As Mood, text As String) Implements IMessage.AddLine
        MessageData.Lines.Add(New MessageLineData With {.Mood = mood, .Text = text})
    End Sub

    Friend Shared Function Create(data As WorldData) As IMessage
        Dim messageId = data.Messages.Count
        data.Messages.Add(New MessageData)
        Return New Message(data, messageId)
    End Function
End Class
