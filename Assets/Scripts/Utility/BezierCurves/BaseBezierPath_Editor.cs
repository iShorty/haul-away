#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public abstract partial class BaseBezierPath : MonoBehaviour
{
    [Header("===== Edit Buttons =====")]
    [SerializeField]
    [HideInInspector]
    [Tooltip("Control these points during editor time")]
    /// <summary>Control these points during editor time</summary>
    protected Transform[] m_Controls = new Transform[0];



    [Header("----- Size -----")]
    [Header("===== Visualisation =====")]
    [Range(0.0001f, 1f)]
    [SerializeField]
    float m_SpaceBetweenSpheres = 0.05f;

    [SerializeField, Range(0.1f, 1f)]
    float m_SizeOfSpheres = 0.1f;

    [SerializeField, Range(0.1f, 1f)]
    float m_SizeOfControlPoints = 0.1f;

    [Header("----- Color -----")]
    [SerializeField]
    Color m_ControlPointColor = Color.red;
    [SerializeField]
    Color m_bezierSpheresColor = Color.white;

    [SerializeField]
    bool m_AlwaysDraw = true;

    // protected bool IsGizmosControlPointsEmpty => m_GizmosControlPoints.Length <= 0;
    /// <summary> Please place me inside #if UNITY_EDITOR</summary>
    protected abstract int PointCount { get; }


    //Runtime
    Color _prevGizmoColor = default;
    Vector3[] _tempControlPoints = default;

    #region ------------- Properties ------------
    /// <summary>When true, Edit Mode is in action and Control Transforms will be created to allow for visualizing where each control point will be and how the curve will look like. When set to false, the control points will save its current world positins into the m_ControlPoints vector3 array.</summary>
    public bool IsInEditMode { get; protected set; } = false;
    #endregion

    #region ------------------ Editor Awake --------------------
    private void Editor_Awake()
    {
        if (EditorApplication.isPlaying) return;

        //If gameobject has not been duplicated,
        if (m_ControlPoints.Length <= 0)
        {
            m_ControlPoints = new Vector3[PointCount];
        }

        //If gameobject has been duplicated,
        if (transform.childCount > 0)
        {
            IsInEditMode = true;
        }

        _tempControlPoints = new Vector3[PointCount];
    }


    #endregion

    #region -------------------------  Editor Visual ------------------------------

    private void OnDrawGizmosSelected()
    {
        if (m_AlwaysDraw) return;
        Editor_DrawGizmos();
    }

    private void OnDrawGizmos()
    {
        if (!m_AlwaysDraw) return;
        Editor_DrawGizmos();
    }

    /// <summary>Just holds the switch case for determining which draw method to use</summary>
    void Editor_DrawGizmos()
    {
        switch (IsInEditMode)
        {
            case true:
                Editor_DrawUsingTransforms();
                break;
            case false:
                Editor_DrawUsingPoints();
                break;
        }
    }

    void Editor_DrawUsingPoints()
    {
        if (IsControlPointsEmpty) return;

        if (m_ControlPoints.Length != PointCount)
        {
            Debug.LogError($"The bezier path {name} doesnt have the correct number of PointCount which is {PointCount}", this);
            return;
        }

        if (m_SpaceBetweenSpheres == 0)
            return;

        _prevGizmoColor = Gizmos.color;
        Gizmos.color = m_bezierSpheresColor;

        //Draw the bezier spheres
        for (float t = 0; t <= 1.0f; t += m_SpaceBetweenSpheres)
        {
            Vector3 gizmoPos = BezierFormula(t, m_ControlPoints);
            Gizmos.DrawSphere(gizmoPos, m_SizeOfSpheres);
        }

        Gizmos.color = m_ControlPointColor;

        //Draw the control points
        Vector3 size = m_SizeOfControlPoints * Vector3.one;
        for (int i = 0; i < PointCount; i++)
        {
            Gizmos.DrawCube(m_ControlPoints[i], size);
        }

        Gizmos.color = _prevGizmoColor;
    }


    void Editor_DrawUsingTransforms()
    {
        if (IsControlPointsEmpty) return;

        if (m_Controls.Length != PointCount)
        {
            Debug.LogError($"The bezier path {name} doesnt have the correct number of PointCount which is {PointCount}", this);
            return;
        }

        if (m_SpaceBetweenSpheres == 0)
            return;

        _prevGizmoColor = Gizmos.color;
        Gizmos.color = m_bezierSpheresColor;

        for (int i = 0; i < m_Controls.Length; i++)
        {
            _tempControlPoints[i] = m_Controls[i].position;
        }

        //Draw the bezier spheres
        for (float t = 0; t <= 1.0f; t += m_SpaceBetweenSpheres)
        {
            Vector3 gizmoPos = BezierFormula(t, _tempControlPoints);
            Gizmos.DrawSphere(gizmoPos, m_SizeOfSpheres);
        }

        Gizmos.color = m_ControlPointColor;

        //Draw the control points
        Vector3 size = m_SizeOfControlPoints * Vector3.one;
        for (int i = 0; i < PointCount; i++)
        {
            Gizmos.DrawCube(m_Controls[i].position, size);
        }

        Gizmos.color = _prevGizmoColor;

    }


    #endregion

    #region ----------- Editor Validate Methods -----------
    // private void OnValidate()
    // {
    //     switch (m_EditMode)
    //     {
    //         case true:
    //             Editor_BeginEdit();
    //             break;
    //         case false:
    //             Editor_Bake();
    //             Editor_EndEdit();
    //             break;
    //     }
    // }



    /// <summary>Spawns in all the control transforms for editor editting</summary>
    public void Editor_BeginEdit()
    {
        #region ------- Checks ----------
        if (IsInEditMode)
        {
            Debug.LogError($"The bezier curve {name} cant go into edit mode because it is already in edit mode!", this);
            return;
        }

        //Prolly wont happen
        if (m_Controls.Length > 0)
        {
            Debug.LogError($"The bezier curve {name} cant go into edit mode because it has more than 0 control transforms!", this);
            return;
        }
        #endregion

        IsInEditMode = true;
        m_Controls = new Transform[PointCount];
        for (int i = 0; i < m_Controls.Length; i++)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(this.transform);
            go.name = $"Control Point {i}";

            //Reassign transform to old positions
            go.transform.position = m_IsLocal ? transform.TransformPoint(m_ControlPoints[i]) : m_ControlPoints[i];
            m_Controls[i] = go.transform;
        }

    }
    /// <summary>Destroys all the control transforms for editor editting</summary>
    public void Editor_EndEdit()
    {
        #region ------- Checks ----------
        //Prolly wont happen
        if (!IsInEditMode)
        {
            Debug.LogError($"The bezier curve {name} cant exit out of edit mode because it is already out of edit mode!", this);
            return;
        }

        if (m_Controls.Length <= 0)
        {
            Debug.LogError($"The bezier curve {name} cant exit out of edit mode because it has 0 or lesser control transfoms!", this);
            return;
        }
        #endregion
        Editor_Bake();
        for (int i = 0; i < m_Controls.Length; i++)
        {
            DestroyImmediate(m_Controls[i].gameObject);
        }
        m_Controls = new Transform[0];
        IsInEditMode = false;
    }


    /// <summary> Saves all of m_Controls's local space position into the m_ControlPoints</summary>
    public void Editor_Bake()
    {
        Debug.Assert(m_ControlPoints.Length == m_Controls.Length, $"The bezeir curve {name} does not have the same number of control points to controls! Expected number: {PointCount}", this);
        if (m_IsLocal)
        {
            for (int i = 0; i < m_ControlPoints.Length; i++)
            {
                m_ControlPoints[i] = m_Controls[i].localPosition;
            }
        }
        else
        {
            for (int i = 0; i < m_ControlPoints.Length; i++)
            {
                m_ControlPoints[i] = m_Controls[i].position;
            }
        }
        
     
    }
    #endregion

}

#endif