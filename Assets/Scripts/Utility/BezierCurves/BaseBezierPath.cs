using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

#if UNITY_EDITOR
[ExecuteInEditMode(), CanEditMultipleObjects, DisallowMultipleComponent]
#endif
public abstract partial class BaseBezierPath : MonoBehaviour
{
    [Header("----- Important Settings -----")]
    [SerializeField]
    [Tooltip("When set to true, the control points vector3 array will be saved as local positions during edit mode and then converted into worldspace during game awake. When set to false, the control points vector3 array will be saved as worldspace during edit mode.")]
    ///<Summary>When set to true, the control points vector3 array will be saved as local positions during edit mode and then converted into worldspace during game awake. When set to false, the control points vector3 array will be saved as worldspace during edit mode.</Summary>
    protected bool m_IsLocal = false;


    [Header("----- RunTime -----")]
    [SerializeField]
    ///<Summary>Control these points to see the gizmos draw at these world space positions</Summary>
    protected Vector3[] m_ControlPoints = new Vector3[0];

    bool IsControlPointsEmpty => m_ControlPoints.Length <= 0;

    protected virtual void Awake()
    {
        if (m_IsLocal)
        {
            for (int i = 0; i < m_ControlPoints.Length; i++)
            {
                m_ControlPoints[i] = transform.TransformPoint(m_ControlPoints[i]);
            }
        }
#if UNITY_EDITOR
        Editor_Awake();
#endif
    }



    #region -------------------------- Pathing Logic --------------------------

    public void SetPoint(int point, Vector3 vector3)
    {
        m_ControlPoints[point] = vector3;
    }

    /// <summary>
    /// Returns the position on the curve at the point of time.
    /// </summary>
    /// <param name="time">Time where 0 is the min and 1 is the max vlaue </param>
    /// <param name="reachedEnd">Returns true if time is > 1 (which means the Point you wish to get is outside of the curve) </param>
    public Vector3 GetPointInTime(float time, out bool reachedEnd)
    {
        reachedEnd = time > 1 ? true : false;
        return BezierFormula(time, m_ControlPoints);
    }
    /// <summary>
    /// Returns the position on the curve at the point of time.
    /// </summary>
    /// <param name="time">Time where 0 is the min and 1 is the max vlaue </param>
    /// <returns></returns>
    public Vector3 GetPointInTime(float time) { return BezierFormula(time, m_ControlPoints); }


    public Vector3 LastPoint => m_ControlPoints[m_ControlPoints.Length - 1];
    public Vector3 FirstPoint => m_ControlPoints[0];

    protected abstract Vector3 BezierFormula(float t, params Vector3[] controlPositions);
    #endregion

}
