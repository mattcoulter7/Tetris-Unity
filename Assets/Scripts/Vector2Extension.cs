using UnityEngine;
 
public static class Vector2Extension
{
    public static Vector2 Rotate(this Vector2 v, float degrees,Vector2? rotationOrigin = null)
    {
        if (rotationOrigin.HasValue) v -= rotationOrigin.Value;
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);

        if (rotationOrigin.HasValue) v += rotationOrigin.Value;

        return v;
    }
}