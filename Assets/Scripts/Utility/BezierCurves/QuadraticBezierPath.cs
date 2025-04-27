using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadraticBezierPath : BaseBezierPath
{

    // protected override Vector3 BezierFormula(float t)
    // {
    //     return Mathf.Pow((1 - t), 2) * m_ControlPoints[0] +
    //              2 * (1 - t) * t * m_ControlPoints[1] +
    //              Mathf.Pow(t, 2) * m_ControlPoints[2];
    // }

    protected override Vector3 BezierFormula(float t, params Vector3[] controlPositions)
    {
#if UNITY_EDITOR
        Debug.Assert(PointCount == controlPositions.Length, $"The bezier curve {name} does not have the correct amount of control positions passed in! It expected {PointCount} number of points \n but was given {controlPositions.Length} points", this);
#endif
        return Mathf.Pow((1 - t), 2) * controlPositions[0] +
                   2 * (1 - t) * t * controlPositions[1] +
                   Mathf.Pow(t, 2) * controlPositions[2];
    }

#if UNITY_EDITOR
    protected override int PointCount => 3;
#endif

}
