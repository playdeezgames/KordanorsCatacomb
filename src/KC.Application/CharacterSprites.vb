Friend Module CharacterSprites
    Private ReadOnly _table As IReadOnlyDictionary(Of CharacterType, Sprite(Of Hue)) =
        New Dictionary(Of CharacterType, Sprite(Of Hue)) From
        {
            {
                CharacterType.Blob,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "            ",
                    "            ",
                    "            ",
                    "   ......   ",
                    "  .XXXXXX.  ",
                    " .XXXXXXXX. ",
                    ".XX.X.XXXXX.",
                    ".XX.X.XXXXX.",
                    ".XX.X.XXXXX.",
                    ".XXXXXXXXXX.",
                    " .XXXXXXXX. ",
                    "  ........  "
                }, AddressOf ColorizeBlob)}
        }

    Private Function ColorizeBlob(character As Char) As Hue
        Select Case character
            Case "."c
                Return Hue.Black
            Case "X"c
                Return Hue.Cyan
            Case Else
                Return Hue.LightMagenta
        End Select
    End Function
    Friend Function GetSprite(characterType As CharacterType) As Sprite(Of Hue)
        Return _table(characterType)
    End Function
End Module
