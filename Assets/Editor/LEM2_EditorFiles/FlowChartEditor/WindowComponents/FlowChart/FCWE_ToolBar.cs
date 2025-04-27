namespace LinearEffectsEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public partial class FlowChartWindowEditor : EditorWindow
    {

        #region Definition
        public enum ToolBarState
        {
            ///<Summary>Normal mode is where user can do stuff like create blocks, duplicate and delete blocks nodes</Summary>
            NORMAL = 0
            ,
            ///<Summary>Arrow mode is where user can draw arrows between nodes but only if there is only 1 selected nodes. The user will no longer be able to create blocks, duplicate and delete blocks. User must exit the Arrow mode in order to return to Normal mode</Summary>
            ARROW = 1
        }
        #endregion

        const string TOOLBAR_BUTTONSYMBOL_ADD = "＋"
        , TOOLBAR_BUTTONSYMBOL_DUPLICATE = "❏"
        , TOOLBAR_BUTTONSYMBOL_DELETE = "X"
        , TOOLBAR_BUTTONSYMBOL_ARROWCONNECTION_LINE = "↘"
        ;

        const float TOOLBAR_HEIGHT = 30f
        , TOOLBAR_BUTTON_SPACING = 5f
        ;
        static readonly Vector2 BUTTONSIZE = new Vector2(20f, 20f);

        Rect _toolBarRect;

        ToolBarState _toolBarState = default;

        public void ToolBar_OnEnable()
        {
            _toolBarRect = new Rect();
            _toolBarRect.height = TOOLBAR_HEIGHT;
            _toolBarState = ToolBarState.NORMAL;
            // _toolBarHidden = false;
        }

        public void ToolBar_OnDisable()
        {

        }


        public void ToolBar_OnGUI()
        {
            Color prevColor = GUIExtensions.Start_GUI_ColourChange(Color.white);
            //======================= DRAW TOOLBAR BG ========================
            _toolBarRect.width = position.width;
            GUI.Box(_toolBarRect, string.Empty);

            //================ DRAW TOOL BAR STATE =============
            switch (_toolBarState)
            {
                case ToolBarState.NORMAL:
                    ToolBar_DrawNormalState();
                    break;
                case ToolBarState.ARROW:
                    ToolBar_DrawArrowState();

                    break;

                default:
                    Debug.Log($"{_toolBarState}'s Toolbar gui has not been implemented!");
                    break;
            }



            GUIExtensions.End_GUI_ColourChange(prevColor);
        }

        private void ToolBar_DrawArrowState()
        {
            Rect rect = _toolBarRect;
            rect.size = BUTTONSIZE;

            //Start the rect at the end of the window
            rect.x = _toolBarRect.xMax;
            rect.x -= TOOLBAR_BUTTON_SPACING;

            //---------------- Draw Exit ArrowMode Button -----------------
            rect.x -= (BUTTONSIZE.x + TOOLBAR_BUTTON_SPACING);
            if (GUI.Button(rect, TOOLBAR_BUTTONSYMBOL_DELETE))
            {
                ToolBar_ExitArrowState();
            }
        }

        private void ToolBar_DrawNormalState()
        {
            Rect rect = _toolBarRect;
            rect.size = BUTTONSIZE;

            #region Draw Left Handside
            //---------------- Draw Add Button -----------------
            rect.x += TOOLBAR_BUTTON_SPACING;
            if (GUI.Button(rect, TOOLBAR_BUTTONSYMBOL_ADD))
            {
                NodeManager_NodeCycler_TriggerCreateNewNode(AddNewBlockFrom.ToolBar);
            }

            //---------------- Draw Duplicate Button -----------------
            // rect.size = BUTTONSIZE;
            rect.x += BUTTONSIZE.x + TOOLBAR_BUTTON_SPACING;
            if (GUI.Button(rect, TOOLBAR_BUTTONSYMBOL_DUPLICATE))
            {
                NodeManager_NodeCycler_DuplicateSelectedNodes();
            }



            //---------------- Draw Delete Button -----------------
            // rect.size = BUTTONSIZE;
            rect.x += BUTTONSIZE.x + TOOLBAR_BUTTON_SPACING;
            if (GUI.Button(rect, TOOLBAR_BUTTONSYMBOL_DELETE))
            {
                NodeManager_NodeCycler_DeleteButton();
            }
            #endregion

            #region Draw Right Handside
            //Start the rect at the end of the window
            rect.x = _toolBarRect.xMax;
            rect.x -= TOOLBAR_BUTTON_SPACING;

            //---------------- Draw ArrowMode Button -----------------
            rect.x -= (BUTTONSIZE.x + TOOLBAR_BUTTON_SPACING);
            if (GUI.Button(rect, TOOLBAR_BUTTONSYMBOL_ARROWCONNECTION_LINE))
            {
                ToolBar_TryEnterArrowState();
            }


            #endregion
        }


        void ToolBar_TryEnterArrowState()
        {
            //Enter arrow mode only if there is only 1 node selected
            if (_selectedBlocks.Count == 1)
            {
                _toolBarState = ToolBarState.ARROW;
            }
            else
            {
                Debug.Log("To enter the connecting arrow mode, you can only select one node block!");
            }
        }

        void ToolBar_ExitArrowState()
        {
            //Exit arrow mode
            _toolBarState = ToolBarState.NORMAL;
        }
    }

}
