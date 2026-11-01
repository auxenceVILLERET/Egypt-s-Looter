using System.Collections.Generic;
using UnityEngine;

public class S_CorridorGenerator
{
    struct RoomPair
    {
        public S_RoomData _a;
        public S_RoomData _b;
        public float _sqrDist;
    }

    readonly S_DungeonData _data;
    readonly SO_DataMap _dataMap;
    readonly SO_CorridorStyle[] _corridorsStyles;
    readonly int _cornerClearance;
    readonly bool _allowShortLegs;
    readonly bool _requireWallGap;

    public S_CorridorGenerator(S_DungeonData data, SO_DataMap dataMap, SO_CorridorStyle[] corridorsStyles, int cornerClearance, bool allowShortLegs, bool requireWallGap)
    {
        _data = data;
        _dataMap = dataMap;
        _corridorsStyles = corridorsStyles;
        _cornerClearance = cornerClearance;
        _allowShortLegs = allowShortLegs;
        _requireWallGap = requireWallGap;
    }

    #region Generate

    public void GenerateCorridors()
    {
        if (_corridorsStyles == null || _corridorsStyles.Length == 0)
            return;

        if (_data._rooms.Count < 2)
            return;

        List<S_RoomData> connected = new List<S_RoomData>();
        List<S_RoomData> remaining = new List<S_RoomData>(_data._rooms);

        connected.Add(remaining[0]);
        remaining.RemoveAt(0);

        while (remaining.Count > 0)
        {
            List<RoomPair> pairs = new List<RoomPair>();

            for (int i = 0; i < connected.Count; i++)
            {
                for (int j = 0; j < remaining.Count; j++)
                {
                    RoomPair pair = new RoomPair();
                    pair._a = connected[i];
                    pair._b = remaining[j];
                    pair._sqrDist = ((Vector2)(pair._a.Center - pair._b.Center)).sqrMagnitude;
                    pairs.Add(pair);
                }
            }

            bool linked = false;

            while (pairs.Count > 0 && !linked)
            {
                int bestIndex = 0;

                for (int i = 1; i < pairs.Count; i++)
                {
                    if (pairs[i]._sqrDist < pairs[bestIndex]._sqrDist)
                        bestIndex = i;
                }

                RoomPair best = pairs[bestIndex];
                pairs.RemoveAt(bestIndex);

                if (TryConnect(best._a, best._b))
                {
                    connected.Add(best._b);
                    remaining.Remove(best._b);
                    linked = true;
                }
            }

            if (!linked)
            {
                break;
            }
        }
    }

    #endregion

    #region Try Methods

    bool TryConnect(S_RoomData roomA, S_RoomData roomB)
    {
        SO_CorridorStyle style = _corridorsStyles[Random.Range(0, _corridorsStyles.Length)];

        int maxWidth = Random.Range(style.MinWidth, style.MaxWidth);
        int minWidth = Mathf.Max(1, style.MinWidth);

        if (maxWidth < minWidth)
            maxWidth = minWidth;

        bool horizontalFirst = Random.value > 0.5f;

        for (int width = maxWidth; width >= minWidth; width--)
        {
            if (TryBuildL(roomA, roomB, style, width, horizontalFirst, true))
                return true;

            if (TryBuildL(roomA, roomB, style, width, !horizontalFirst, true))
                return true;

            if (TryBuildOffsetStraight(roomA, roomB, style, width))
                return true;
        }

        if (!_allowShortLegs)
            return false;

        for (int width = maxWidth; width >= minWidth; width--)
        {
            if (TryBuildL(roomA, roomB, style, width, horizontalFirst, false))
                return true;

            if (TryBuildL(roomA, roomB, style, width, !horizontalFirst, false))
                return true;
        }

        return false;
    }

    bool TryBuildL(S_RoomData roomA, S_RoomData roomB, SO_CorridorStyle style, int width, bool horizontalFirst, bool requireVisibleCorner)
    {
        Vector2Int from = roomA.Center;
        Vector2Int to = roomB.Center;
        Vector2Int elbow;

        if (horizontalFirst)
            elbow = new Vector2Int(to.x, from.y);
        else
            elbow = new Vector2Int(from.x, to.y);

        bool isStraight = elbow == from || elbow == to;

        if (requireVisibleCorner && !isStraight)
        {
            int required = width / 2 + 1 + _cornerClearance;

            if (CountCellsOutside(from, elbow, roomA) < required)
                return false;

            if (CountCellsOutside(elbow, to, roomB) < required)
                return false;
        }

        List<Vector2Int> path = BuildPath(from, to, horizontalFirst);
        return TryCommitCorridor(roomA, roomB, style, width, path);
    }

    bool TryBuildOffsetStraight(S_RoomData roomA, S_RoomData roomB, SO_CorridorStyle style, int width)
    {
        RectInt a = roomA._bounds;
        RectInt b = roomB._bounds;

        int low = -((width - 1) / 2);
        int high = width / 2;

        List<Vector2Int> path = new List<Vector2Int>();

        int rowMin = Mathf.Max(a.yMin, b.yMin);
        int rowMax = Mathf.Min(a.yMax, b.yMax);

        if (rowMax - rowMin >= width)
        {
            int row = Mathf.Clamp((rowMin + rowMax - 1) / 2, rowMin - low, rowMax - 1 - high);

            AddLine(path, new Vector2Int(roomA.Center.x, row), new Vector2Int(roomB.Center.x, row));
            return TryCommitCorridor(roomA, roomB, style, width, path);
        }

        int columnMin = Mathf.Max(a.xMin, b.xMin);
        int columnMax = Mathf.Min(a.xMax, b.xMax);

        if (columnMax - columnMin >= width)
        {
            int column = Mathf.Clamp((columnMin + columnMax - 1) / 2, columnMin - low, columnMax - 1 - high);

            AddLine(path, new Vector2Int(column, roomA.Center.y), new Vector2Int(column, roomB.Center.y));
            return TryCommitCorridor(roomA, roomB, style, width, path);
        }

        return false;
    }

    bool TryCommitCorridor(S_RoomData roomA, S_RoomData roomB, SO_CorridorStyle style, int width, List<Vector2Int> path)
    {
        HashSet<Vector2Int> cells = ExpandPath(path, width);
        List<Vector2Int> tiles = new List<Vector2Int>();

        foreach (Vector2Int cell in cells)
        {
            if (roomA._bounds.Contains(cell) || roomB._bounds.Contains(cell))
                continue;

            if (!IsInsideMap(cell))
                return false;

            if (_data._roomTiles.Contains(cell))
                return false;

            if (_data._corridorTiles.Contains(cell))
                return false;

            tiles.Add(cell);
        }

        if (_requireWallGap && TouchesOtherStructure(tiles, roomA, roomB))
            return false;

        if (tiles.Count == 0)
            return true;

        S_CorridorData corridor = new S_CorridorData();
        corridor._style = style;
        corridor._roomA = roomA;
        corridor._roomB = roomB;
        corridor._tiles = tiles;
        _data._corridors.Add(corridor);

        for (int i = 0; i < tiles.Count; i++)
        {
            _data._corridorTiles.Add(tiles[i]);
        }

        return true;
    }

    #endregion

    #region Utils

    bool TouchesOtherStructure(List<Vector2Int> tiles, S_RoomData roomA, S_RoomData roomB)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    Vector2Int neighbor = new Vector2Int(tiles[i].x + dx, tiles[i].y + dy);

                    if (roomA._bounds.Contains(neighbor) || roomB._bounds.Contains(neighbor))
                        continue;

                    if (_data._roomTiles.Contains(neighbor))
                        return true;

                    if (_data._corridorTiles.Contains(neighbor))
                        return true;
                }
            }
        }

        return false;
    }

    int CountCellsOutside(Vector2Int from, Vector2Int to, S_RoomData room)
    {
        List<Vector2Int> leg = new List<Vector2Int>();
        AddLine(leg, from, to);

        int count = 0;

        for (int i = 0; i < leg.Count; i++)
        {
            if (!room._bounds.Contains(leg[i]))
                count++;
        }

        return count;
    }

    bool IsInsideMap(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= _dataMap.SizeX)
            return false;

        if (cell.y < 0 || cell.y >= _dataMap.SizeY)
            return false;

        return true;
    }

    List<Vector2Int> BuildPath(Vector2Int from, Vector2Int to, bool horizontalFirst)
    {
        Vector2Int elbow;

        if (horizontalFirst)
            elbow = new Vector2Int(to.x, from.y);
        else
            elbow = new Vector2Int(from.x, to.y);

        List<Vector2Int> path = new List<Vector2Int>();
        AddLine(path, from, elbow);
        AddLine(path, elbow, to);
        return path;
    }

    HashSet<Vector2Int> ExpandPath(List<Vector2Int> path, int width)
    {
        HashSet<Vector2Int> cells = new HashSet<Vector2Int>();

        int low = -((width - 1) / 2);
        int high = width / 2;

        for (int i = 0; i < path.Count; i++)
        {
            for (int dx = low; dx <= high; dx++)
            {
                for (int dy = low; dy <= high; dy++)
                {
                    cells.Add(new Vector2Int(path[i].x + dx, path[i].y + dy));
                }
            }
        }

        return cells;
    }

    void AddLine(List<Vector2Int> path, Vector2Int from, Vector2Int to)
    {
        int stepX = Mathf.Clamp(to.x - from.x, -1, 1);
        int stepY = Mathf.Clamp(to.y - from.y, -1, 1);
        Vector2Int step = new Vector2Int(stepX, stepY);

        Vector2Int current = from;
        path.Add(current);

        while (current != to)
        {
            current += step;
            path.Add(current);
        }
    }

    #endregion
}