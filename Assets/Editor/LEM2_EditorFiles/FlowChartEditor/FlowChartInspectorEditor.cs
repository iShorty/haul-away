namespace LinearEffectsEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using LinearEffects;

    [CustomEditor(typeof(BaseFlowChart), true)]
    ///<Summary>The inspector editor for the FlowChart component</Summary>
    public class FlowChartInspectorEditor : Editor
    {
        const string SETTINGS_PROPERTY_NAME = "_settings";

        const string BUTTON_OPENWINDOW_LABEL = "Open FlowChart"
        , BUTTON_CLEARFLOWCHART_LABEL = "Clear FlowChart"
        ;


        #region  Runtime Vars
        BaseFlowChart _target = default;
        SerializedProperty _settingsProperty = default;
        #endregion

        private void OnEnable()
        {
            _target = (BaseFlowChart)target;
            _settingsProperty = serializedObject.FindProperty(SETTINGS_PROPERTY_NAME);
        }

        private void OnDisable()
        {
            _target = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUILayout.BeginHorizontal();
            DrawButtons();
            GUILayout.EndHorizontal();
            DrawProperties();
            serializedObject.ApplyModifiedProperties();
        }

        #region Draw 
        void DrawButtons()
        {
            if (GUILayout.Button(BUTTON_OPENWINDOW_LABEL))
            {
                FlowChartWindowEditor.OpenWindow(_target);
            }
            else if (GUILayout.Button(BUTTON_CLEARFLOWCHART_LABEL))
            {
                //Reload window if it is opened
                if (FlowChartWindowEditor.IsOpen)
                {
                    FlowChartWindowEditor.DisableWindow();
                    _target.Editor_ResetFlowChart();
                    FlowChartWindowEditor.EnableWindow();
                }
                else
                {
                    _target.Editor_ResetFlowChart();
                }
            }
        }


        //Draw the properties only, dont want to draw the block array 
        void DrawProperties()
        {
            if (_settingsProperty == null)
            {
                string debug = $"The property named: {SETTINGS_PROPERTY_NAME} inside the Block class has been renamed to something else or it doesnt exist anymore!";
                Debug.LogWarning(debug);
                return;
            }

            EditorGUILayout.PropertyField(_settingsProperty, includeChildren: true);
        }

        #endregion


    }

}