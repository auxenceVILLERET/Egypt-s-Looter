using System.Collections.Generic;
using UnityEngine;

public class S_PlacementContext
{
    public readonly S_RoomData Room;
    public readonly HashSet<Vector2Int> Floor;
    public readonly float CellSize;
    public readonly Transform Parent;

    public readonly HashSet<Vector2Int> DoorCells = new HashSet<Vector2Int>();
    public readonly HashSet<Vector2Int> Occupied = new HashSet<Vector2Int>(); 
    public readonly HashSet<Vector2Int> Halo = new HashSet<Vector2Int>(); 
    public readonly HashSet<Vector2Int> WallOccupied = new HashSet<Vector2Int>();
    public readonly HashSet<Vector2Int> WallHalo = new HashSet<Vector2Int>();
    public readonly Vector3 CellOffset;

    public S_PlacementContext(S_RoomData room, HashSet<Vector2Int> floor, float cellSize, Transform parent, Vector3 cellOffset)
    {
        Room = room;
        Floor = floor;
        CellSize = cellSize;
        Parent = parent;
        CellOffset = cellOffset;

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (Vector2Int cell in room._bounds.allPositionsWithin)
        {
            for (int i = 0; i < dirs.Length; i++)
            {
                Vector2Int n = cell + dirs[i];
                if (!room._bounds.Contains(n) && floor.Contains(n))
                {
                    DoorCells.Add(cell);
                    break;
                }
            }
        }
    }

    public Vector3 CellToWorld(Vector2Int cell, float y)
    {
        return new Vector3((cell.x + 0.5f) * CellSize + CellOffset.x, y, (cell.y + 0.5f) * CellSize + CellOffset.z);
    }

    public bool IsNearDoor(Vector2Int cell, int clearance)
    {
        if (clearance < 0) return false;

        foreach (Vector2Int door in DoorCells)
        {
            if (Mathf.Abs(door.x - cell.x) <= clearance && Mathf.Abs(door.y - cell.y) <= clearance)
                return true;
        }
        return false;
    }
}