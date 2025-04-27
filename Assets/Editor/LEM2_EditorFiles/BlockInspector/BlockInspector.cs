namespace LinearEffectsEditor
{
    using UnityEngine;
    using UnityEditor;
    using LinearEffects;
    using System;

    [CustomEditor(typeof(BlockScriptableInstance))]
    public partial class BlockInspector : ImprovedEditor
    {
        #region Constants
        const string EDITORPREFS_HEIGHTRATIO = "TopHalf_To_Height";
        const float DEFAULT_HEIGHTRATIO = 0.5f;
        #endregion

        BlockScriptableInstance _target = null;
        float _ratioOfTopHalfToInspectorHeight = DEFAULT_HEIGHTRATIO;

        #region Inheritance
        public override bool AllowBaseInspectorGUI => false;
        #endregion

        Vector2 _topHalfSize = default;




        #region LifeTime Methods
        private void OnEnable()
        {
            _target = (BlockScriptableInstance)target;
            OnInspectorWindowResize += HandleWindowResize;

            TopHalf_OnEnable();
            CenterDiv_OnEnable();
            BottomHalf_OnEnable();
            Load();
        }



        private void OnDisable()
        {
            OnInspectorWindowResize -= HandleWindowResize;

            TopHalf_OnDisable();
            CenterDiv_OnDisable();
            BottomHalf_OnDisable();

            Save();
        }


        public override void HandleInspectorGUI()
        {
            // //Initialise each halve's sizes
            // Vector2 topHalfSize;
            // topHalfSize.x = Screen.width * 0.725f;
            _topHalfSize.y = _ratioOfTopHalfToInspectorHeight * Screen.height;


            serializedObject.Update();
            EditorGUILayout.BeginVertical();

            //Draw top half
            TopHalf_OnInspectorGUI();
            CenterDiv_OnInspectorGUI();
            BottomHalf_OnInspectorGUI();


            EditorGUILayout.EndVertical();
            if (serializedObject.ApplyModifiedProperties())
            {
                _target.SaveModifiedProperties();
            }
        }

        #endregion

        #region  HandleEvents
        void HandleWindowResize()
        {
            //Initialise each halve's sizes
            _topHalfSize.x = Screen.width * 0.725f;
        }

        private void HandleDivisonDrag(float mouseDeltaY)
        {
            if (mouseDeltaY == 0) return;

            float scaleMultiplier = 0.0015f;
            mouseDeltaY *= scaleMultiplier;
            _ratioOfTopHalfToInspectorHeight += mouseDeltaY;
            _ratioOfTopHalfToInspectorHeight = Mathf.Clamp(_ratioOfTopHalfToInspectorHeight, 0.1f, 0.9f);
        }


        #endregion


        #region Saving Editor's Preferences
        void Load()
        {
            if (!EditorPrefs.HasKey(EDITORPREFS_HEIGHTRATIO))
            {
                EditorPrefs.SetFloat(EDITORPREFS_HEIGHTRATIO, DEFAULT_HEIGHTRATIO);
                _ratioOfTopHalfToInspectorHeight = DEFAULT_HEIGHTRATIO;
                return;
            }

            _ratioOfTopHalfToInspectorHeight = EditorPrefs.GetFloat(EDITORPREFS_HEIGHTRATIO);
        }

        void Save()
        {
            EditorPrefs.SetFloat(EDITORPREFS_HEIGHTRATIO, _ratioOfTopHalfToInspectorHeight);
        }


        #endregion

    }
}