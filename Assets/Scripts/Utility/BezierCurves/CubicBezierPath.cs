using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicBezierPath : BaseBezierPath
{

    // protected override Vector3 BezierFormula(float t)
    // {
    //     return Mathf.Pow((1 - t), 3) * m_ControlPoints[0] +
    //        3 * Mathf.Pow((1 - t), 2) * t * m_ControlPoints[1] +
    //        3 * (1 - t) * t * t * m_ControlPoints[2] +
    //        t * t * t * m_ControlPoints[3];
    // }

    protected override Vector3 BezierFormula(float t, params Vector3[] controlPositions)
    {
#if UNITY_EDITOR
        Debug.Assert(PointCount == controlPositions.Length, $"The bezier curve {name} does not have the correct amount of control positions passed in! It expected {PointCount} number of points \n but was given {controlPositions.Length} points", this);
#endif

        return Mathf.Pow((1 - t), 3) * controlPositions[0] +
          3 * Mathf.Pow((1 - t), 2) * t * controlPositions[1] +
          3 * (1 - t) * t * t * controlPositions[2] +
          t * t * t * controlPositions[3];
    }

#if UNITY_EDITOR
    protected override int PointCount => 4;

#endif
}
