using System.Collections.Generic;
using UnityEngine;

public class S_WallGenerator
{
    static readonly Vector2Int[] Directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    readonly S_DungeonData _data;
    readonly GameObject _doorPrefab;

    public S_WallGenerator(S_DungeonData data, GameObject doorPrefab)
    {
        _data = data;
        _doorPrefab = doorPrefab;
    }

    public void GenerateWalls()
    {
        _data._wallEdges.Clear();

        HashSet<Vector2Int> floor = _data.GetFloor();

        for (int i = 0; i < _data._rooms.Count; i++)
        {
            S_RoomData room = _data._rooms[i];

            for (int x = room._bounds.xMin; x < room._bounds.xMax; x++)
            {
                for (int y = room._bounds.yMin; y < room._bounds.yMax; y++)
                {
                    AddWallEdges(new Vector2Int(x, y), room._style.RoomWallPrefab, floor);
                }
            }
        }

        for (int i = 0; i < _data._corridors.Count; i++)
        {
            S_CorridorData corridor = _data._corridors[i];

            for (int t = 0; t < corridor._tiles.Count; t++)
            {
                AddWallEdges(corridor._tiles[t], corridor._style.CorridorWallPrefabs[Random.Range(0, corridor._style.CorridorWallPrefabs.Length)], floor);
            }
        }
    }

    public void GenerateDoors()
    {
        if (_doorPrefab == null)
            return;

        for (int i = 0; i < _data._corridors.Count; i++)
        {
            S_CorridorData corridor = _data._corridors[i];

            for (int j = 0; j < corridor._tiles.Count; j++)
            {
                AddDoors(corridor._tiles[j], corridor);
            }
        }
    }

    void AddWallEdges(Vector2Int cell, GameObject prefab, HashSet<Vector2Int> floor)
    {
        for (int d = 0; d < Directions.Length; d++)
        {
            Vector2Int neighbor = cell + Directions[d];

            if (floor.Contains(neighbor))
                continue;

            S_WallEdge edge = new S_WallEdge();
            edge._cell = cell;
            edge._direction = Directions[d];
            edge._prefab = prefab;
            _data._wallEdges.Add(edge);
        }
    }

    void AddDoors(Vector2Int corridorCell, S_CorridorData corridor)
    {
        for (int i = 0; i < Directions.Length; i++)
        {
            Vector2Int neighbor = corridorCell + Directions[i];

            bool touchesRoom = corridor._roomA._bounds.Contains(neighbor)
                            || corridor._roomB._bounds.Contains(neighbor);

            if (!touchesRoom)
                continue;

            S_WallEdge edge = new S_WallEdge();
            edge._cell = corridorCell;
            edge._direction = Directions[i];
            edge._prefab = _doorPrefab;
            _data._wallEdges.Add(edge);
        }
    }
}