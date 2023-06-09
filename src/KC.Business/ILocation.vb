﻿Public Interface ILocation
    ReadOnly Property Id As Integer
    Sub SetBorder(direction As Direction, border As IBorder)
    Function GetBorder(direction As Direction) As IBorder
    Sub SetNeighbor(direction As Direction, location As ILocation)
    Function GetNeighbor(direction As Direction) As ILocation
    Function IsVisitedBy(character As ICharacter) As Boolean
    ReadOnly Property Allies As IEnumerable(Of ICharacter)
    ReadOnly Property ExitCount As Integer
    ReadOnly Property Enemies As IEnumerable(Of ICharacter)
    ReadOnly Property HasEnemies As Boolean
    ReadOnly Property Items As IEnumerable(Of IItem)
    ReadOnly Property ItemTypes As IEnumerable(Of ItemType)
    ReadOnly Property HasItems As Boolean
    ReadOnly Property HasUsableItems As Boolean
    ReadOnly Property UsableItems As IEnumerable(Of IItem)
End Interface
