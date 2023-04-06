Friend Class Item
    Implements IItem

    Private ReadOnly _data As WorldData
    Private ReadOnly _itemId As Integer

    Public Sub New(data As WorldData, itemId As Integer)
        Me._data = data
        Me._itemId = itemId
    End Sub

    Public ReadOnly Property Id As Integer Implements IItem.Id
        Get
            Return _itemId
        End Get
    End Property
    Private ReadOnly Property ItemData As ItemData
        Get
            Return _data.Items(_itemId)
        End Get
    End Property

    Public Property Location As ILocation Implements IItem.Location
        Get
            If Not ItemData.Location.HasValue Then
                Return Nothing
            End If
            Return New Location(_data, ItemData.Location.Value)
        End Get
        Set(value As ILocation)
            If ItemData.Location.HasValue Then
                _data.Locations(ItemData.Location.Value).Items.Remove(Id)
            End If
            ItemData.Location = value?.Id
            If ItemData.Location.HasValue Then
                If ItemData.Location.HasValue Then
                    _data.Locations(ItemData.Location.Value).Items.Add(Id)
                End If
            End If
        End Set
    End Property

    Public ReadOnly Property ItemType As ItemType Implements IItem.ItemType
        Get
            Return ItemData.ItemType
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IItem.Name
        Get
            Return ItemType.ToDescriptor.Name
        End Get
    End Property

    Public ReadOnly Property IsUsable As Boolean Implements IItem.IsUsable
        Get
            Return ItemType.ToDescriptor.IsUsable
        End Get
    End Property

    Public ReadOnly Property CanTake As Boolean Implements IItem.CanTake
        Get
            Return ItemType.ToDescriptor.CanTake
        End Get
    End Property

    Friend Shared Function Create(data As WorldData, itemType As ItemType, location As ILocation) As IItem
        Dim itemId = data.Items.Count
        Dim itemData = New ItemData With
                             {
                                .ItemType = itemType
                             }
        data.Items.Add(itemData)
        Return New Item(data, itemId) With {
            .Location = location
        }
    End Function

    Public Function OnUse(character As ICharacter) As Boolean Implements IItem.OnUse
        Return ItemType.ToDescriptor.OnUse(_data, character)
    End Function
End Class
