Imports KC.Data

Public Module GameContext
    Public Const FrameWidth = 160
    Public Const FrameHeight = 80
    Public Const ViewWidth = FrameWidth
    Public Const ViewHeight = FrameHeight + 10
    Private _worldData As WorldData
    Public ReadOnly Property World As IWorld
        Get
            Return New World(_worldData)
        End Get
    End Property
    Friend Sub Embark()
        _worldData = New WorldData
        World.Generate()
    End Sub
    Friend Sub Initialize()
        InitializeFonts()
    End Sub
    Friend ReadOnly Fonts As New Dictionary(Of GameFont, Font)
    Private Sub InitializeFonts()
        Fonts.Clear()
        Fonts.Add(GameFont.Font3x5, New Font(JsonSerializer.Deserialize(Of FontData)(File.ReadAllText("Content/CyFont3x5.json"))))
        Fonts.Add(GameFont.Font4x6, New Font(JsonSerializer.Deserialize(Of FontData)(File.ReadAllText("Content/CyFont4x6.json"))))
        Fonts.Add(GameFont.Font5x7, New Font(JsonSerializer.Deserialize(Of FontData)(File.ReadAllText("Content/CyFont5x7.json"))))
        Fonts.Add(GameFont.Font8x8, New Font(JsonSerializer.Deserialize(Of FontData)(File.ReadAllText("Content/CyFont8x8.json"))))
    End Sub
End Module
