using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_RoomStyle", menuName = "Egypt's Looter/SO_RoomStyle")]
public class SO_RoomStyle : ScriptableObject
{
    [Header("Size")]
    [SerializeField] protected int _minWidth = 6;
    [SerializeField] protected int _maxWidth = 12;
    [SerializeField] protected int _minHeight = 6;
    [SerializeField] protected int _maxHeight = 12;

    [BoxGroup("Prefabs")]
    [SerializeField] protected GameObject _roomFloorPrefab;
    [BoxGroup("Prefabs")]
    [SerializeField] protected GameObject _roomWallPrefab;

    public GameObject RoomFloorPrefab { get { return _roomFloorPrefab; } }
    public GameObject RoomWallPrefab { get { return _roomWallPrefab; } }
    public int MinWidth { get { return _minWidth; } }
    public int MaxWidth { get { return _maxWidth; } }
    public int MinHeight { get { return _minHeight; } }
    public int MaxHeight { get { return _maxHeight; } }
}
