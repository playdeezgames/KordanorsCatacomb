Public Class GameController
    Inherits BaseGameController(Of Hue, Command, Sfx, GameState)
    Private ReadOnly _configSink As Action(Of (Integer, Integer), Single)

    Public Sub New(windowSizeSource As Func(Of (Integer, Integer)), volumeSource As Func(Of Single), configSink As Action(Of (Integer, Integer), Single))
        MyBase.New(windowSizeSource(), volumeSource())
        _configSink = configSink
        _configSink(Size, Volume)
        GameContext.Initialize()
        SetState(GameState.Prolog, New PrologState(Me, AddressOf SetCurrentState))
        SetState(GameState.Splash, New SplashState(Me, AddressOf SetCurrentState))
        SetState(GameState.About, New AboutState(Me, AddressOf SetCurrentState))
        SetState(GameState.GameOver, New GameOverState(Me, AddressOf SetCurrentState))
        SetState(GameState.Win, New WinState(Me, AddressOf SetCurrentState))
        SetState(GameState.Message, New MessageState(Me, AddressOf SetCurrentState))
        SetState(GameState.Navigation, New NavigationState(Me, AddressOf SetCurrentState))
        SetState(GameState.ModeSelect, New ModeSelectState(Me, AddressOf SetCurrentState))
        SetState(GameState.Ground, New GroundState(Me, AddressOf SetCurrentState))
        SetState(GameState.Inventory, New InventoryState(Me, AddressOf SetCurrentState))
        SetState(GameState.InventoryDetail, New InventoryDetailState(Me, AddressOf SetCurrentState))
        SetState(GameState.CombatInventory, New CombatInventoryState(Me, AddressOf SetCurrentState))
        SetState(GameState.Combat, New CombatState(Me, AddressOf SetCurrentState))
        SetState(GameState.Neutral, New NeutralState(Me, AddressOf SetCurrentState))
        SetState(GameState.MainMenu, New MainMenuState(Me, AddressOf SetCurrentState))
        SetState(GameState.SFXVolume, New SFXVolumeState(Me, AddressOf SetCurrentState, Sub() _configSink(Size, Volume)))
        SetState(GameState.ScreenSize, New ScreenSizeState(Me, AddressOf SetCurrentState, Sub() _configSink(Size, Volume)))
        SetCurrentState(GameState.Splash)
    End Sub
End Class
