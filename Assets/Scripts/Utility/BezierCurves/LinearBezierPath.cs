using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearBezierPath : BaseBezierPath
{


    // protected override Vector3 BezierFormula(float t)
    // {
    //     return (1 - t) * m_ControlPoints[0] + t * m_ControlPoints[1];
    // }

    protected override Vector3 BezierFormula(float t, params Vector3[] controlPositions)
    {
#if UNITY_EDITOR
        Debug.Assert(PointCount == controlPositions.Length, $"The bezier curve {name} does not have the correct amount of control positions passed in! It expected {PointCount} number of points \n but was given {controlPositions.Length} points", this);
#endif
        return (1 - t) * controlPositions[0] + t * controlPositions[1];
    }

#if UNITY_EDITOR
    protected override int PointCount => 2;
#endif
}
