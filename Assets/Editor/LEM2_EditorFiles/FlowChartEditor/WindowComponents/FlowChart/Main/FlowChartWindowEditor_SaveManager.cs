namespace LinearEffectsEditor
{
    using UnityEngine;
    using UnityEditor;
    using LinearEffects;
    using UnityEditor.SceneManagement;
    using UnityEngine.SceneManagement;

    //Handles the saving and loading of main FlowChartWindow level data ie. FlowChart _flowChart variable
    public partial class FlowChartWindowEditor : EditorWindow
    {
        #region Constants
        const string EDITORPREFS_PREV_FLOWCHART_SCENEPATH = "FlowChartPath";
        #endregion

        ///<Summary>Saves the current FlowChart path</Summary>
        void SaveManager_SaveFlowChartPath()
        {
            if (_flowChart == null)
            {
                return;
            }

            string path = _flowChart.transform.GetFullPath();
            EditorPrefs.SetString(EDITORPREFS_PREV_FLOWCHART_SCENEPATH, path);
        }

        ///<Summary>Tries to load the FlowChart.cs instance from the scene</Summary>
        BaseFlowChart SaveManager_TryLoadFlowChartPath()
        {
            string path = EditorPrefs.GetString(EDITORPREFS_PREV_FLOWCHART_SCENEPATH);
            // Debug.Log(path);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            //Path is not empty
            for (int i = 0; i < EditorSceneManager.loadedSceneCount; i++)
            {
                Scene loadedScene = EditorSceneManager.GetSceneAt(i);
                // Debug.Log($"The loaded scene index: {i} with the name: {loadedScene.name} is being checked");
                if (!loadedScene.GetTransform(path, out Transform flowChartTransform))
                {
                    continue;
                }
                // Debug.Log($"The found transform name is {flowChartTransform.name}", flowChartTransform);
                if (!flowChartTransform.TryGetComponent<BaseFlowChart>(out BaseFlowChart flowChart))
                {
                    continue;
                }

                return flowChart;
            }


            //Else if no flowchart is found because the FlowChart.cs instance is destroyed or deleted 
            return null;
        }

        ///<Summary>Tries to load the FlowChart.cs instance from the scene but during Editor's runtime</Summary>
        BaseFlowChart SaveManager_TryLoadFlowChartPath_Runtime()
        {
            string path = EditorPrefs.GetString(EDITORPREFS_PREV_FLOWCHART_SCENEPATH);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (!loadedScene.GetTransform(path, out Transform flowChartTransform))
                {
                    continue;
                }

                if (!flowChartTransform.TryGetComponent<BaseFlowChart>(out BaseFlowChart flowChart))
                {
                    continue;
                }

                return flowChart;
            }

            //Else if no flowchart is found
            return null;

        }
    }

}