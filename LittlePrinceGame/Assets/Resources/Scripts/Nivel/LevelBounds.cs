using UnityEngine;

public static class LevelBounds
{
    public static Vector2 Center;
    public static Vector2 Size;
    public static Vector2 HalfSize => Size * 0.5f;
    public static bool TryWrap(Vector2 pos, out Vector2 wrappedPos)
    {
        if (Size == Vector2.zero)
        {
            wrappedPos = pos;
            return false;
        }

        Vector2 relative = pos - Center;
        Vector2 half = HalfSize;
        bool wrapped = false;

        // Eje X
        if (relative.x > half.x)
        {
            relative.x = -half.x;
            wrapped = true;
        }
        else if (relative.x < -half.x)
        {
            relative.x = half.x;
            wrapped = true;
        }

        // Eje Y
        if (relative.y > half.y)
        {
            relative.y = -half.y;
            wrapped = true;
        }
        else if (relative.y < -half.y)
        {
            relative.y = half.y;
            wrapped = true;
        }

        wrappedPos = Center + relative;
        return wrapped;
    }

}
