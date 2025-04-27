namespace LinearEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class Vector3Extension
    {
        public static Vector3 Divide(this Vector3 vector3, Vector3 divideBy)
        {
            vector3.x /= divideBy.x;
            vector3.y /= divideBy.y;
            vector3.z /= divideBy.z;
            return vector3;
        }

        public static Vector2 Divide(this Vector2 vector, Vector2 divideBy)
        {
            vector.x /= divideBy.x;
            vector.y /= divideBy.y;
            return vector;
        }
    }

}