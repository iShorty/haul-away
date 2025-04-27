namespace LinearEffectsEditor
{
    using UnityEngine;
    using UnityEditor;
    using LinearEffects;

    //Repsonsible for displaying empty flowchart editor
    public partial class FlowChartWindowEditor : EditorWindow
    {
        void UnloadedBackground_OnGUI()
        {
            EditorGUILayout.BeginVertical(); EditorGUILayout.LabelField(string.Empty); EditorGUILayout.EndVertical();
            Rect rect = GUILayoutUtility.GetLastRect();
            _flowChart = (BaseFlowChart)EditorGUI.ObjectField(rect, "Target FlowChart", _flowChart, typeof(BaseFlowChart), true);
            // instance = GetWindow<FlowChartWindowEditor>();
            if (_flowChart != null)
            {
                ReloadWindow();
            }

        }
    }

}