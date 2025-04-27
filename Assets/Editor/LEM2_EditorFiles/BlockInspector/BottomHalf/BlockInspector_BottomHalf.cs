namespace LinearEffectsEditor
{
    using UnityEngine;
    using UnityEditor;
    using System;
    using LinearEffects;
    using CategorizedSearchBox;
    using System.Collections.Generic;

    //The bottom half class will render the current observed command as well as the command toolbar (add,minus coppy etc)
    public partial class BlockInspector : ImprovedEditor
    {
        GameObject BlockGameObject => _target.BlockGameObject;

        // #region Constants
        // const string DEBUG_EFFECTEXECUTOR = "TestUpdateExecutor";
   
        // #endregion

        #region LifeTime Method
        void BottomHalf_OnEnable()
        {
            BottomHalf_ToolBar_OnEnable();
            BottomHalf_SearchBox_OnEnable();
        }

        void BottomHalf_OnDisable()
        {
            BottomHalf_SearchBox_OnDisable();
        }

        void BottomHalf_OnInspectorGUI()
        {
            BottomHalf_DrawToolBar();
            BottomHalf_SearchBox_OnGUI();
            BottomHalf_OnGUI_ObservedEffect(Screen.width);

        }


        #endregion

    }

}