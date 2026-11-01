using System.Collections.Generic;
using UnityEngine;

public class S_DungeonSpawner
{
    readonly S_DungeonData _data;
    readonly Transform _parent;

    readonly float _cellSize;
    readonly float _ceilingY;
    readonly float _wallY;
    readonly float _wallOffset;
    readonly float _wallYawOffset;
    readonly SO_Rules[] _rules;
    readonly Vector2 _propsOffset;

    readonly Dictionary<GameObject, Vector3> _tileOffsets = new Dictionary<GameObject, Vector3>();

    GameObject _container;

    public S_DungeonSpawner(S_DungeonData data, Transform parent, float cellSize, float ceilingY, float wallY, float wallOffset, float wallYawOffset, SO_Rules[] rules, Vector2 propsOffset)
    {
        _data = data;
        _parent = parent;
        _cellSize = cellSize;
        _ceilingY = ceilingY;
        _wallY = wallY;
        _wallOffset = wallOffset;
        _wallYawOffset = wallYawOffset;
        _rules = rules;
        _propsOffset = propsOffset;
    }

    #region Public

    public void SpawnDungeon()
    {
        SpawnRooms();
        SpawnProps();
        SpawnCorridors();
        SpawnWalls();
        SpawnCeilings();
    }

    public void Clear()
    {
        _tileOffsets.Clear();

        if (_container == null)
            return;

        if (Application.isPlaying)
            Object.Destroy(_container);
        else
            Object.DestroyImmediate(_container);

        _container = null;
    }

    #endregion

    #region Container

    void EnsureContainer()
    {
        if (_container != null)
            return;

        _container = new GameObject("GeneratedDungeon");
        _container.transform.SetParent(_parent, false);
    }

    #endregion

    #region Spawn

    void SpawnRooms()
    {
        EnsureContainer();

        for (int i = 0; i < _data._rooms.Count; i++)
        {
            S_RoomData room = _data._rooms[i];
            GameObject prefab = room._style.RoomFloorPrefab;

            if (prefab == null)
                continue;

            GameObject roomRoot = new GameObject("Room_" + i);
            roomRoot.transform.SetParent(_container.transform, false);

            for (int x = room._bounds.xMin; x < room._bounds.xMax; x++)
            {
                for (int y = room._bounds.yMin; y < room._bounds.yMax; y++)
                {
                    SpawnTile(prefab, new Vector2Int(x, y), roomRoot.transform);
                }
            }
        }
    }

    void SpawnProps()
    {
        if (_rules == null || _rules.Length == 0)
            return;

        EnsureContainer();

        GameObject propsRoot = new GameObject("Props");
        propsRoot.transform.SetParent(_container.transform, false);

        HashSet<Vector2Int> floor = _data.GetFloor();

        for (int i = 0; i < _data._rooms.Count; i++)
        {
            GameObject roomProps = new GameObject("RoomProps_" + i);
            roomProps.transform.SetParent(propsRoot.transform, false);

            Vector3 offset = Vector3.zero;
            GameObject floorPrefab = _data._rooms[i]._style.RoomFloorPrefab;
            if (floorPrefab != null)
                _tileOffsets.TryGetValue(floorPrefab, out offset);

            offset += new Vector3(_propsOffset.x, 0f, _propsOffset.y);

            S_PlacementContext context = new S_PlacementContext(_data._rooms[i], floor, _cellSize, roomProps.transform, offset);

            for (int r = 0; r < _rules.Length; r++)
            {
                if (_rules[r] != null)
                    _rules[r].Apply(context);
            }
        }
    }

    void SpawnCorridors()
    {
        EnsureContainer();

        for (int i = 0; i < _data._corridors.Count; i++)
        {
            S_CorridorData corridor = _data._corridors[i];

            GameObject corridorRoot = new GameObject("Corridor_" + i);
            corridorRoot.transform.SetParent(_container.transform, false);

            for (int t = 0; t < corridor._tiles.Count; t++)
            {
                GameObject prefab = corridor._style.CorridorFloorPrefabs[Random.Range(0, corridor._style.CorridorFloorPrefabs.Length)];
                SpawnTile(prefab, corridor._tiles[t], corridorRoot.transform);
            }
        }
    }

    void SpawnWalls()
    {
        EnsureContainer();

        GameObject wallsRoot = new GameObject("Walls");
        wallsRoot.transform.SetParent(_container.transform, false);

        for (int i = 0; i < _data._wallEdges.Count; i++)
        {
            S_WallEdge edge = _data._wallEdges[i];

            if (edge._prefab == null)
                continue;

            float yaw = _wallYawOffset;

            if (edge._direction.y != 0)
            {
                if (edge._direction.y > 0) yaw += 180f;
            }
            else
            {
                yaw += 90f;
                if (edge._direction.x > 0) yaw += 180f;
            }

            Vector3 dir = new Vector3(edge._direction.x, 0f, edge._direction.y);

            Vector3 target = new Vector3(
                (edge._cell.x + 0.5f) * _cellSize + 2f,
                _wallY,
                (edge._cell.y + 0.5f) * _cellSize - 2f) + dir * (_cellSize * 0.5f + _wallOffset);

            GameObject wall = Object.Instantiate(edge._prefab, target, Quaternion.Euler(0f, yaw, 0f), wallsRoot.transform);

            Vector3 wallCenter;
            BoxCollider box = wall.GetComponent<BoxCollider>();

            if (box != null)
            {
                wallCenter = wall.transform.TransformPoint(box.center);
            }
            else
            {
                Renderer[] renderers = wall.GetComponentsInChildren<Renderer>();
                if (renderers.Length == 0)
                {
                    wall.name = "Wall_" + edge._cell.x + "_" + edge._cell.y + "_" + edge._direction.x + "_" + edge._direction.y;
                    continue;
                }

                Bounds bounds = renderers[0].bounds;
                for (int r = 1; r < renderers.Length; r++)
                    bounds.Encapsulate(renderers[r].bounds);

                wallCenter = bounds.center;
            }

            Vector3 delta = target - wallCenter;
            delta.y = 0f;
            wall.transform.position += delta;

            wall.name = "Wall_" + edge._cell.x + "_" + edge._cell.y + "_" + edge._direction.x + "_" + edge._direction.y;
        }
    }

    void SpawnCeilings()
    {
        EnsureContainer();

        GameObject ceilingRoot = new GameObject("Ceiling");
        ceilingRoot.transform.SetParent(_container.transform, false);

        for (int i = 0; i < _data._rooms.Count; i++)
        {
            S_RoomData room = _data._rooms[i];
            GameObject prefab = room._style.RoomFloorPrefab;

            if (prefab == null)
                continue;

            for (int x = room._bounds.xMin; x < room._bounds.xMax; x++)
            {
                for (int y = room._bounds.yMin; y < room._bounds.yMax; y++)
                {
                    SpawnCeilingTile(prefab, new Vector2Int(x, y), ceilingRoot.transform);
                }
            }
        }

        for (int i = 0; i < _data._corridors.Count; i++)
        {
            S_CorridorData corridor = _data._corridors[i];
            GameObject prefab = corridor._style.CorridorFloorPrefabs[Random.Range(0, 0)];

            if (prefab == null)
                continue;

            for (int t = 0; t < corridor._tiles.Count; t++)
            {
                SpawnCeilingTile(prefab, corridor._tiles[t], ceilingRoot.transform);
            }
        }
    }

    #endregion

    #region Tiles

    void SpawnTile(GameObject prefab, Vector2Int cell, Transform parent)
    {
        Vector3 position = new Vector3((cell.x + 0.5f) * _cellSize, 0f, (cell.y + 0.5f) * _cellSize);
        GameObject tile = Object.Instantiate(prefab, position, Quaternion.identity, parent);
        tile.name = "Tile_" + cell.x + "_" + cell.y;

        RegisterTileOffset(prefab, tile);
    }

    void SpawnCeilingTile(GameObject prefab, Vector2Int cell, Transform parent)
    {
        Vector3 position = new Vector3((cell.x + 0.5f) * _cellSize, _ceilingY, (cell.y + 0.5f) * _cellSize);

        GameObject tile = Object.Instantiate(prefab, position, Quaternion.identity, parent);
        tile.name = "Ceiling_" + cell.x + "_" + cell.y;
    }

    void RegisterTileOffset(GameObject prefab, GameObject tile)
    {
        if (_tileOffsets.ContainsKey(prefab))
            return;

        Renderer[] renderers = tile.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            _tileOffsets[prefab] = Vector3.zero;
            return;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        Vector3 offset = bounds.center - tile.transform.position;
        offset.y = 0f;
        _tileOffsets[prefab] = offset;
    }

    #endregion
}