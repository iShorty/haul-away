using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;

[CustomEditor(typeof(BaseBezierPath), true)]
[CanEditMultipleObjects]
public class BezierCurve_Inspector : Editor
{
    #region ------- Const --------
    const string BUTTON_NAME_NOT_EDITMODE = "Not In EditMode. Click to Edit";
    const string BUTTON_NAME_IN_EDITMODE = "In EditMode. Click to Save";
    #endregion

    HashSet<BaseBezierPath> _targets = new HashSet<BaseBezierPath>();
    bool _currentAssumedState = false;
    // BaseBezierPath _target = default;
    private void OnEnable()
    {
        // _target = (BaseBezierPath)target;

        for (int i = 0; i < targets.Length; i++)
        {
            BaseBezierPath path = (BaseBezierPath)targets[i];
            _targets.Add(path);
        }
    }

    private void OnDisable()
    {
        _targets.Clear();
        // _target = null;
    }

    public override void OnInspectorGUI()
    {
        if (TryGetCurrentState(out _currentAssumedState))
        {
            switch (_currentAssumedState)
            {
                case true:
                    if (GUILayout.Button(BUTTON_NAME_IN_EDITMODE))
                    {
                        ExitEditMode();
                    }
                    break;

                case false:
                    if (GUILayout.Button(BUTTON_NAME_NOT_EDITMODE))
                    {
                        BeginEditMode();
                    }
                    break;
            }
        }

        // UpdateInspectedObjects();

        base.OnInspectorGUI();
    }

    #region ------------------ Editing & Saving ------------------
    /// <summary>Returns true if all the selected targets have their edit modes in the same state (eg all true or all false). Out represents the current state of all the targets if all the targets have the same states</summary>
    bool TryGetCurrentState(out bool currentState)
    {
        bool allSame = currentState = true;

        //Get current state first so that we can compare in the allSame
        foreach (var item in _targets)
        {
            currentState = item.IsInEditMode;
            break;
        }

        foreach (var item in _targets)
        {
            //If item and the current assumed state are not the same
            if (item.IsInEditMode != currentState)
            {
                allSame = false;
                break;
            }
        }
        return allSame;
    }

    void BeginEditMode()
    {
        foreach (var item in _targets)
        {
            item.Editor_BeginEdit();
        }
    }

    void ExitEditMode()
    {
        foreach (var item in _targets)
        {
            item.Editor_EndEdit();
        }

        for (int i = 0; i < serializedObject.targetObjects.Length; i++)
        {
            UnityEngine.Object inspectingObject = serializedObject.targetObjects[i];

            if (PrefabUtility.IsPartOfRegularPrefab(inspectingObject))
            {
                EditorUtility.SetDirty(inspectingObject);
                Undo.RecordObject(inspectingObject, "Bake BezierPath");
                PrefabUtility.RecordPrefabInstancePropertyModifications(inspectingObject);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
    #endregion


    // /// <summary>Updates the transform changes if any of the inspected objects</summary>
    // private void UpdateInspectedObjects()
    // {
    //     foreach (var item in _targets)
    //     {
    //         if (!item.transform.hasChanged) continue;

    //         //If there was transform changes done to the inspected object,
    //         Debug.Log($"Object {item.name} has moved to {item.transform.position}",item);


    //     }
    // }

}
