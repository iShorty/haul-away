using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


public abstract class ImprovedEditor : Editor
{
    #region Window resize Fields

    protected event Action OnInspectorWindowResize = null;
    protected Vector2 _previousWindowSize = default;

    #endregion


    public abstract void HandleInspectorGUI();
    public abstract bool AllowBaseInspectorGUI { get; }

    public override void OnInspectorGUI()
    {
        CheckChanges();

        if (AllowBaseInspectorGUI)
        {
            base.OnInspectorGUI();
        }
        HandleInspectorGUI();

    }

    void CheckChanges()
    {
        CheckWindowResizeFields();
    }

    #region Window Resize Methods
    void CheckWindowResizeFields()
    {
        Vector2 currentWindowSize;
        currentWindowSize.x = Screen.width;
        currentWindowSize.y = Screen.height;

        if (currentWindowSize == _previousWindowSize) return;

        OnInspectorWindowResize?.Invoke();
        _previousWindowSize = currentWindowSize;
    }

    #endregion

}
