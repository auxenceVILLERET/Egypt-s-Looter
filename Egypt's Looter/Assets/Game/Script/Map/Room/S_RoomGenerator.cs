using UnityEngine;

public class S_RoomGenerator
{
    readonly S_DungeonData _data;
    readonly SO_DataMap _dataMap;
    readonly SO_RoomStyle _startRoom;
    readonly SO_RoomStyle _chestRoom;
    readonly SO_RoomStyle[] _roomStyles;
    readonly int _roomSpacing;

    public S_RoomGenerator(S_DungeonData data, SO_DataMap dataMap, SO_RoomStyle startRoom, SO_RoomStyle chestRoom, SO_RoomStyle[] roomStyles, int roomSpacing)
    {
        _data = data;
        _dataMap = dataMap;
        _startRoom = startRoom;
        _chestRoom = chestRoom;
        _roomStyles = roomStyles;
        _roomSpacing = roomSpacing;
    }

    public void GenerateStartRoom()
    {
        if (_startRoom == null)
            return;

        if (_data._startRoomGenerated)
            return;

        SO_RoomStyle style = _startRoom;

        int width = Random.Range(style.MinWidth, style.MaxWidth);
        int height = Random.Range(style.MinHeight, style.MaxHeight);

        int maxX = _dataMap.SizeX - width - _dataMap.Padding;
        int maxZ = _dataMap.SizeY - height - _dataMap.Padding;

        int posX = Random.Range(_dataMap.Padding, maxX);
        int posZ = Random.Range(_dataMap.Padding, maxZ);

        RectInt bounds = new RectInt(posX, posZ, width, height);

        AddRoom(style, bounds);
        _data._startRoomGenerated = true;
    }

    public void GenerateRooms()
    {
        if (_roomStyles == null || _roomStyles.Length == 0)
            return;

        for (int i = 0; i < _dataMap.MaxAttemps; i++)
        {
            if (_data._rooms.Count >= _dataMap.RoomCount)
                break;

            SO_RoomStyle style = _roomStyles[Random.Range(0, _roomStyles.Length)];

            int width = Random.Range(style.MinWidth, style.MaxWidth);
            int height = Random.Range(style.MinHeight, style.MaxHeight);

            int maxX = _dataMap.SizeX - width - _dataMap.Padding;
            int maxZ = _dataMap.SizeY - height - _dataMap.Padding;

            if (maxX < _dataMap.Padding || maxZ < _dataMap.Padding)
                continue;

            int posX = Random.Range(_dataMap.Padding, maxX);
            int posZ = Random.Range(_dataMap.Padding, maxZ);

            RectInt bounds = new RectInt(posX, posZ, width, height);

            if (OverlapsExistingRoom(bounds))
                continue;

            AddRoom(style, bounds);
        }
    }

    void AddRoom(SO_RoomStyle style, RectInt bounds)
    {
        S_RoomData room = new S_RoomData();
        room._style = style;
        room._bounds = bounds;
        _data._rooms.Add(room);

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                _data._roomTiles.Add(new Vector2Int(x, y));
            }
        }
    }

    bool OverlapsExistingRoom(RectInt candidate)
    {
        RectInt expanded = new RectInt(
            candidate.x - _roomSpacing,
            candidate.y - _roomSpacing,
            candidate.width + _roomSpacing * 2,
            candidate.height + _roomSpacing * 2);

        for (int i = 0; i < _data._rooms.Count; i++)
        {
            if (expanded.Overlaps(_data._rooms[i]._bounds))
                return true;
        }

        return false;
    }
}