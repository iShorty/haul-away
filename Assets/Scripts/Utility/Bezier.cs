using UnityEngine;

public static class Bezier
{
    // For movement of objects along a curve. Gets the next position along a Bezier curve.
    // Inputs: a = starting point, b = interpolated point, c = end point, t = time/etc
    public static Vector3 GetPoint(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        float r = 1f - t;
        return r * r * a + 2f * r * t * b + t * t * c;
    }

    // For rotation of objects along a curve. Gets the correct rotation so an object faces forwards when travelling along a Bezier curve.
    // Inputs: a = starting point, b = interpolated point, c = end point, t = time/etc
    public static Vector3 GetDerivative(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        return 2f * ((1f - t) * (b - a) + t * (c - b));
    }
}