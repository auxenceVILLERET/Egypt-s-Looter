using System.Collections.Generic;
using UnityEngine;

public static class S_DungeonPathfinder
{
    static readonly Vector2Int[] Directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    public static S_RoomData FindFarthestRoom(S_DungeonData data)
    {
        if (data._rooms.Count < 2)
            return null;

        HashSet<Vector2Int> floor = data.GetFloor();

        Dictionary<Vector2Int, int> distances = BFSDistance(data.StartRoom.Center, floor);

        S_RoomData farthest = null;
        int bestDistance = -1;

        for (int i = 1; i < data._rooms.Count; i++)
        {
            int distance;
            if (!distances.TryGetValue(data._rooms[i].Center, out distance))
                continue;

            if (distance > bestDistance)
            {
                bestDistance = distance;
                farthest = data._rooms[i];
            }
        }

        return farthest;
    }

    public static Dictionary<Vector2Int, int> BFSDistance(Vector2Int start, HashSet<Vector2Int> floor)
    {
        Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        distances[start] = 0;
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            int currentDistance = distances[current];

            for (int i = 0; i < Directions.Length; i++)
            {
                Vector2Int next = current + Directions[i];

                if (!floor.Contains(next))
                    continue;

                if (distances.ContainsKey(next))
                    continue;

                distances[next] = currentDistance + 1;
                queue.Enqueue(next);
            }
        }

        return distances;
    }
}