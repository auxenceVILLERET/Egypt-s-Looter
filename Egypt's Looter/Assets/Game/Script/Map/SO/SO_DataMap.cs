using UnityEngine;

[CreateAssetMenu(fileName = "SO_DataMap", menuName = "Egypt's Looter/SO_DataMap")]
public class SO_DataMap : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private int _roomCount;
    [SerializeField] private int _maxAttemps;

    [Header("Size")]
    [SerializeField] private int _sizeX = 100;
    [SerializeField] private int _sizeY = 100;
    [SerializeField] private int _padding = 10;

    public int SizeX { get { return _sizeX; } }
    public int SizeY { get { return _sizeY; } }
    public int Padding { get { return _padding; } }
    public int RoomCount { get { return _roomCount; } }
    public int MaxAttemps { get { return _maxAttemps; } }
}
