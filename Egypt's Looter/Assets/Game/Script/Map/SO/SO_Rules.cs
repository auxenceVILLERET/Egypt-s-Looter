using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public enum E_PlacementMode { Floor, Wall }

[CreateAssetMenu(fileName = "SO_Rules", menuName = "Egypt's Looter/SO_Rules")]
public class SO_Rules : ScriptableObject
{
    [BoxGroup("Prefab")]
    [SerializeField] private GameObject _prefab;
    [BoxGroup("Prefab")]
    [Tooltip("Rotation Y ajoutée pour corriger l'orientation du modèle")]
    [SerializeField] private float _yawOffset = 0f;
    [BoxGroup("Prefab")]
    [SerializeField] private float _yOffset = 0f;

    [BoxGroup("Count")]
    [SerializeField] private int _minCount = 1;
    [BoxGroup("Count")]
    [SerializeField] private int _maxCount = 3;

    [BoxGroup("Placement")]
    [SerializeField] private E_PlacementMode _mode = E_PlacementMode.Floor;
    [BoxGroup("Placement")]
    [Tooltip("Cases vides imposées autour (1 = ne se touchent pas, même en diagonale)")]
    [SerializeField] private int _minSpacing = 1;
    [BoxGroup("Placement")]
    [Tooltip("Cases libres autour des portes (-1 = ignoré)")]
    [SerializeField] private int _doorClearance = 1;
    [BoxGroup("Placement")]
    [Tooltip("Vide = toutes les salles")]
    [SerializeField] private SO_RoomStyle[] _allowedRoomStyles;

    [BoxGroup("Floor")]
    [ShowIf("IsFloor")]
    [SerializeField] private Vector2Int _footprint = Vector2Int.one;
    [BoxGroup("Floor")]
    [ShowIf("IsFloor")]
    [Tooltip("Cases libres entre le prop et les murs (0 = peut être collé)")]
    [SerializeField] private int _wallMargin = 0;
    [BoxGroup("Floor")]
    [ShowIf("IsFloor")]
    [SerializeField] private bool _randomYaw = true;

    [BoxGroup("Wall")]
    [ShowIf("IsWall")]
    [Tooltip("Rapproche l'objet vers l'intérieur de la salle")]
    [SerializeField] private float _wallInset = 0f;
    [BoxGroup("Wall")]
    [ShowIf("IsWall")]
    [Tooltip("Réserve aussi la case de sol devant (utile pour un piège qui tire)")]
    [SerializeField] private bool _blockFloorCell = true;
    [BoxGroup("Wall")]
    [ShowIf("IsWall")]
    [SerializeField] private int _wallWidth = 1;
    [BoxGroup("Wall")]
    [ShowIf("IsWall")]
    [SerializeField] private int _wallDepth = 1;
    [BoxGroup("Wall")]
    [ShowIf("IsWall")]
    [SerializeField] private int _cornerMargin = 0;

    private bool IsFloor => _mode == E_PlacementMode.Floor;
    private bool IsWall => _mode == E_PlacementMode.Wall;

    private struct WallSlot
    {
        public Vector2Int _cell;
        public Vector2Int _direction;
    }

    public void Apply(S_PlacementContext ctx)
    {
        if (_prefab == null || !AppliesTo(ctx.Room)) return;

        int target = Random.Range(_minCount, _maxCount + 1);

        if (_mode == E_PlacementMode.Floor)
            PlaceOnFloor(ctx, target);
        else
            PlaceOnWalls(ctx, target);
    }

    bool AppliesTo(S_RoomData room)
    {
        if (_allowedRoomStyles == null || _allowedRoomStyles.Length == 0) return true;

        for (int i = 0; i < _allowedRoomStyles.Length; i++)
        {
            if (_allowedRoomStyles[i] == room._style) return true;
        }
        return false;
    }

    #region Floor

    void PlaceOnFloor(S_PlacementContext ctx, int target)
    {
        RectInt room = ctx.Room._bounds;
        Vector2Int size = new Vector2Int(Mathf.Max(1, _footprint.x), Mathf.Max(1, _footprint.y));

        List<Vector2Int> anchors = new List<Vector2Int>();
        for (int x = room.xMin; x <= room.xMax - size.x; x++)
        {
            for (int y = room.yMin; y <= room.yMax - size.y; y++)
            {
                anchors.Add(new Vector2Int(x, y));
            }
        }
        Shuffle(anchors);

        int placed = 0;
        for (int i = 0; i < anchors.Count && placed < target; i++)
        {
            RectInt footprint = new RectInt(anchors[i].x, anchors[i].y, size.x, size.y);
            if (!CanPlaceOnFloor(ctx, footprint)) continue;

            SpawnOnFloor(ctx, footprint);
            ReserveFloor(ctx, footprint);
            placed++;
        }
    }

    bool CanPlaceOnFloor(S_PlacementContext ctx, RectInt footprint)
    {
        RectInt room = ctx.Room._bounds;

        if (footprint.xMin < room.xMin + _wallMargin || footprint.xMax > room.xMax - _wallMargin) return false;
        if (footprint.yMin < room.yMin + _wallMargin || footprint.yMax > room.yMax - _wallMargin) return false;

        foreach (Vector2Int cell in footprint.allPositionsWithin)
        {
            if (ctx.Halo.Contains(cell)) return false;
            if (ctx.IsNearDoor(cell, _doorClearance)) return false;
        }

        foreach (Vector2Int cell in Grow(footprint, _minSpacing).allPositionsWithin)
        {
            if (ctx.Occupied.Contains(cell)) return false;
        }

        return true;
    }

    void ReserveFloor(S_PlacementContext ctx, RectInt footprint)
    {
        foreach (Vector2Int cell in footprint.allPositionsWithin)
            ctx.Occupied.Add(cell);

        foreach (Vector2Int cell in Grow(footprint, _minSpacing).allPositionsWithin)
            ctx.Halo.Add(cell);
    }

    void SpawnOnFloor(S_PlacementContext ctx, RectInt footprint)
    {
        Vector3 position =  new Vector3((footprint.x + footprint.width * 0.5f) * ctx.CellSize + ctx.CellOffset.x, _yOffset, (footprint.y + footprint.height * 0.5f) * ctx.CellSize + ctx.CellOffset.z);

        float yaw = _yawOffset;
        if (_randomYaw)
        {
            yaw += footprint.width == footprint.height ? Random.Range(0, 4) * 90f : Random.Range(0, 2) * 180f;
        }

        Instantiate(_prefab, position, Quaternion.Euler(0f, yaw, 0f), ctx.Parent);
    }

    #endregion

    #region Wall

    void PlaceOnWalls(S_PlacementContext ctx, int target)
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        List<WallSlot> slots = new List<WallSlot>();
        foreach (Vector2Int cell in ctx.Room._bounds.allPositionsWithin)
        {
            for (int d = 0; d < dirs.Length; d++)
            {
                if (ctx.Floor.Contains(cell + dirs[d])) continue;

                WallSlot slot = new WallSlot();
                slot._cell = cell;
                slot._direction = dirs[d];
                slots.Add(slot);
            }
        }
        Shuffle(slots);

        int placed = 0;
        for (int i = 0; i < slots.Count && placed < target; i++)
        {
            if (!CanPlaceOnWall(ctx, slots[i])) continue;

            SpawnOnWall(ctx, slots[i]);
            ReserveWall(ctx, slots[i]);
            placed++;
        }
    }

    static Vector2Int Tangent(Vector2Int direction)
    {
        return new Vector2Int(Mathf.Abs(direction.y), Mathf.Abs(direction.x));
    }

    static RectInt RectBetween(Vector2Int a, Vector2Int b)
    {
        Vector2Int min = Vector2Int.Min(a, b);
        Vector2Int max = Vector2Int.Max(a, b);
        return new RectInt(min.x, min.y, max.x - min.x + 1, max.y - min.y + 1);
    }

    RectInt WallStrip(WallSlot slot)
    {
        Vector2Int t = Tangent(slot._direction);
        return RectBetween(slot._cell, slot._cell + t * (Mathf.Max(1, _wallWidth) - 1));
    }

    RectInt WallFootprint(WallSlot slot)
    {
        Vector2Int t = Tangent(slot._direction);
        Vector2Int far = slot._cell
                    + t * (Mathf.Max(1, _wallWidth) - 1)
                    - slot._direction * (Mathf.Max(1, _wallDepth) - 1);
        return RectBetween(slot._cell, far);
    }

    bool CanPlaceOnWall(S_PlacementContext ctx, WallSlot slot)
    {
        RectInt room = ctx.Room._bounds;
        Vector2Int t = Tangent(slot._direction);
        int width = Mathf.Max(1, _wallWidth);

        for (int k = -_cornerMargin; k < width + _cornerMargin; k++)
        {
            Vector2Int c = slot._cell + t * k;
            if (!room.Contains(c)) return false;
            if (ctx.Floor.Contains(c + slot._direction)) return false;
        }

        RectInt footprint = WallFootprint(slot);
        if (footprint.xMin < room.xMin || footprint.xMax > room.xMax) return false;
        if (footprint.yMin < room.yMin || footprint.yMax > room.yMax) return false;

        foreach (Vector2Int cell in footprint.allPositionsWithin)
        {
            if (ctx.IsNearDoor(cell, _doorClearance)) return false;
            if (_blockFloorCell && ctx.Halo.Contains(cell)) return false;
        }

        RectInt strip = WallStrip(slot);
        foreach (Vector2Int cell in strip.allPositionsWithin)
        {
            if (ctx.WallHalo.Contains(cell)) return false;
        }
        foreach (Vector2Int cell in Grow(strip, _minSpacing).allPositionsWithin)
        {
            if (ctx.WallOccupied.Contains(cell)) return false;
        }

        if (_blockFloorCell)
        {
            foreach (Vector2Int cell in Grow(footprint, _minSpacing).allPositionsWithin)
            {
                if (ctx.Occupied.Contains(cell)) return false;
            }
        }

        return true;
    }

    void ReserveWall(S_PlacementContext ctx, WallSlot slot)
    {
        RectInt strip = WallStrip(slot);
        foreach (Vector2Int cell in strip.allPositionsWithin)
            ctx.WallOccupied.Add(cell);
        foreach (Vector2Int cell in Grow(strip, _minSpacing).allPositionsWithin)
            ctx.WallHalo.Add(cell);

        if (_blockFloorCell)
        {
            RectInt footprint = WallFootprint(slot);
            foreach (Vector2Int cell in footprint.allPositionsWithin)
                ctx.Occupied.Add(cell);
            foreach (Vector2Int cell in Grow(footprint, _minSpacing).allPositionsWithin)
                ctx.Halo.Add(cell);
        }
    }

    void SpawnOnWall(S_PlacementContext ctx, WallSlot slot)
    {
        Vector3 outward = new Vector3(slot._direction.x, 0f, slot._direction.y);
        Vector2Int t = Tangent(slot._direction);

        Vector3 along = new Vector3(t.x, 0f, t.y) * ((Mathf.Max(1, _wallWidth) - 1) * 0.5f * ctx.CellSize);

        Vector3 position = ctx.CellToWorld(slot._cell, _yOffset) + along
                        + outward * (ctx.CellSize * 0.5f - _wallInset);

        Quaternion rotation = Quaternion.LookRotation(-outward) * Quaternion.Euler(0f, _yawOffset, 0f);

        Instantiate(_prefab, position, rotation, ctx.Parent);
    }

    #endregion

    #region Utils

    static RectInt Grow(RectInt rect, int margin)
    {
        return new RectInt(rect.x - margin, rect.y - margin, rect.width + margin * 2, rect.height + margin * 2);
    }

    static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    #endregion
}