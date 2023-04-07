Public Interface IMessage
    ReadOnly Property Lines As IEnumerable(Of IMessageLine)
    Sub AddLine(mood As Mood, text As String)
    Property Cue As SoundCue
End Interface
