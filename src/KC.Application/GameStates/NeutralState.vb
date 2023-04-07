Friend Class NeutralState
    Inherits BaseGameState(Of Hue, Command, Sfx, GameState)

    Public Sub New(parent As IGameController(Of Hue, Command, Sfx), setState As Action(Of GameState))
        MyBase.New(parent, setState)
    End Sub

    Public Overrides Sub HandleCommand(command As Command)
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink(Of Hue))
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
        If World.HasMessages Then
            Select Case World.NextMessage.Cue
                Case SoundCue.EnemyDeath
                    PlaySfx(Sfx.EnemyDeath)
                Case SoundCue.PlayerDeath
                    PlaySfx(Sfx.PlayerDeath)
                Case SoundCue.EnemyHit
                    PlaySfx(Sfx.EnemyHit)
                Case SoundCue.PlayerHit
                    PlaySfx(Sfx.PlayerHit)
                Case SoundCue.Miss
                    PlaySfx(Sfx.Miss)
                Case SoundCue.Win
                    PlaySfx(Sfx.LevelUp)
            End Select
            SetState(GameState.Message)
            Return
        ElseIf World.PlayerCharacter.IsDead Then
            SetState(GameState.GameOver)
            Return
        ElseIf World.PlayerCharacter.Inventory.ItemTypes.Contains(ItemType.DewALitTea) Then
            SetState(GameState.Win)
            Return
        ElseIf World.PlayerCharacter.Location.HasEnemies Then
            SetState(GameState.Combat)
            Return
        End If
        SetState(GameState.Navigation)
    End Sub
End Class
