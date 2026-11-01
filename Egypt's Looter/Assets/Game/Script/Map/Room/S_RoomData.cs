using UnityEngine;

public class S_RoomData
{
    public SO_RoomStyle _style;

    public RectInt _bounds;

    public Vector2Int Center
    {
        get { return new Vector2Int(_bounds.x + _bounds.width / 2, _bounds.y + _bounds.height / 2); }
    }

    public Vector3 GetWorldCenter(float cellSize, float y)
    {
        float x = (_bounds.xMin + _bounds.width / 2f) * cellSize;
        float z = (_bounds.yMin + _bounds.height / 2f) * cellSize;
        return new Vector3(x, y, z);
    }
}