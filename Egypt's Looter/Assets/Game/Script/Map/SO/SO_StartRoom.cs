using UnityEngine;

[CreateAssetMenu(fileName = "SO_StartRoom", menuName = "Egypt's Looter/SO_StartRoom")]
public class SO_StartRoom : SO_RoomStyle
{
    new int MinWidth { get { return _minWidth; } }
    new int MaxWidth { get { return _maxWidth; } }
    new int MinHeight { get { return _minHeight; } }
    new int MaxHeight { get { return _maxHeight; } }
}
