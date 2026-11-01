using System.Collections.Generic;
using UnityEngine;

public struct S_WallEdge
{
    public Vector2Int _cell;
    public Vector2Int _direction;
    public GameObject _prefab;
}

public class S_DungeonData
{
    public readonly List<S_RoomData> _rooms = new List<S_RoomData>();
    public readonly List<S_CorridorData> _corridors = new List<S_CorridorData>();

    public readonly HashSet<Vector2Int> _roomTiles = new HashSet<Vector2Int>();
    public readonly HashSet<Vector2Int> _corridorTiles = new HashSet<Vector2Int>();

    public readonly List<S_WallEdge> _wallEdges = new List<S_WallEdge>();

    public S_RoomData _endRoom;
    public bool _startRoomGenerated;

    public S_RoomData StartRoom
    {
        get
        {
            if (_rooms.Count == 0)
                return null;

            return _rooms[0];
        }
    }

    public HashSet<Vector2Int> GetFloor()
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>(_roomTiles);
        floor.UnionWith(_corridorTiles);
        return floor;
    }

    public void Clear()
    {
        _rooms.Clear();
        _corridors.Clear();
        _roomTiles.Clear();
        _corridorTiles.Clear();
        _wallEdges.Clear();
        _endRoom = null;
        _startRoomGenerated = false;
    }
}