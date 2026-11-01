using UnityEngine;

public static class S_DungeonGizmos
{
    public static void DrawRoom(S_RoomData room, float cellSize, float height)
    {
        if (room == null)
            return;

        Vector3 center = room.GetWorldCenter(cellSize, 0f);
        Vector3 size = new Vector3(room._bounds.width * cellSize, height, room._bounds.height * cellSize);
        Vector3 boxCenter = center + Vector3.up * (height * 0.5f);

        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawCube(boxCenter, size);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, size);
        Gizmos.DrawSphere(center + Vector3.up * (height + 1f), 1f);
    }
}