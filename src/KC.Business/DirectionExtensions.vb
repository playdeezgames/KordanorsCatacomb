Imports System.Runtime.CompilerServices

Friend Module DirectionExtensions
    Private ReadOnly _table As IReadOnlyDictionary(Of Direction, DirectionDescriptor) =
        New Dictionary(Of Direction, DirectionDescriptor) From
        {
            {Direction.North, New DirectionDescriptor(Direction.West, Direction.East)},
            {Direction.East, New DirectionDescriptor(Direction.North, Direction.South)},
            {Direction.South, New DirectionDescriptor(Direction.East, Direction.West)},
            {Direction.West, New DirectionDescriptor(Direction.South, Direction.North)}
        }
    <Extension>
    Friend Function ToDescriptor(direction As Direction) As DirectionDescriptor
        Return _table(direction)
    End Function
End Module
