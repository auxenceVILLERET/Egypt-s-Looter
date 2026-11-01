using UnityEngine;

[CreateAssetMenu(fileName = "SO_ChestRoom", menuName = "Egypt's Looter/SO_ChestRoom")]
public class SO_ChestRoom : SO_RoomStyle
{
    new int MinWidth { get { return _minWidth; } }
    new int MaxWidth { get { return _maxWidth; } }
    new int MinHeight { get { return _minHeight; } }
    new int MaxHeight { get { return _maxHeight; } }
}