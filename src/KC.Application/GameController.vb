Public Class GameController
    Inherits BaseGameController(Of Hue, Command, Sfx, GameState)
    Private ReadOnly _configSink As Action(Of (Integer, Integer), Single)

    Public Sub New(windowSizeSource As Func(Of (Integer, Integer)), volumeSource As Func(Of Single), configSink As Action(Of (Integer, Integer), Single))
        MyBase.New(windowSizeSource(), volumeSource())
        _configSink = configSink
        _configSink(Size, Volume)
        GameContext.Initialize()
        SetState(GameState.Splash, New SplashState(Me, AddressOf SetCurrentState))
        SetState(GameState.MainMenu, New MainMenuState(Me, AddressOf SetCurrentState))
        SetState(GameState.SFXVolume, New SFXVolumeState(Me, AddressOf SetCurrentState, Sub() _configSink(Size, Volume)))
        SetCurrentState(GameState.Splash)
    End Sub
End Class
