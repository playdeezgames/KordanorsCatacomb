Friend Class Border
    Implements IBorder

    Private ReadOnly _data As WorldData
    Private ReadOnly _borderId As Integer
    Private ReadOnly Property BorderData As BorderData
        Get
            Return _data.Borders(_borderId)
        End Get
    End Property

    Public Sub New(data As WorldData, borderId As Integer)
        _data = data
        _borderId = borderId
    End Sub

    Public ReadOnly Property Id As Integer Implements IBorder.Id
        Get
            Return _borderId
        End Get
    End Property
End Class
