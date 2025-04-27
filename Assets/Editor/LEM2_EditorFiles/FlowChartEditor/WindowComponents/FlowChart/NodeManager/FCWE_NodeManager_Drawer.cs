namespace LinearEffectsEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using LinearEffects;
    using System;

    public partial class FlowChartWindowEditor : EditorWindow
    {
        #region Statics
        static GUIStyle DebugStyle;
        static GUIContent DebugGUIContent, FlowChart_GUIContent;

        ///<Summary>Can only be get by BlockNode's draw function. Thanks</Summary>
        public static GUIStyle BlockNodeConnectButtonStyle { get; private set; }

        ///<Summary>Can only be get by BlockNode's draw function. Thanks</Summary>
        public static GUIStyle BlockNodeBoxStyle { get; private set; }


        #endregion


        #region Constants
        static readonly Color SELECTIONBOX_COLOUR = new Color(.75f, .93f, .93f, 0.5f);
        #endregion




        #region Initialization
        void NodeManager_Drawer_OnEnable()
        {

            //================ INIT MAIN DRAWERS ================
            BlockNodeConnectButtonStyle = new GUIStyle(GUI.skin.button);
            BlockNodeConnectButtonStyle.wordWrap = true;
            BlockNodeConnectButtonStyle.alignment = TextAnchor.MiddleCenter;

            BlockNodeBoxStyle = new GUIStyle();
            BlockNodeBoxStyle.wordWrap = true;
            BlockNodeBoxStyle.normal.background = Texture2D.whiteTexture;
            BlockNodeBoxStyle.alignment = TextAnchor.MiddleCenter;
            BlockNodeBoxStyle.normal.textColor = Color.gray;


            OnEditorSkinChange += NodeManager_Drawer_HandleOnSkinChange;

            //================ INIT DEBUG DRAWERS ================
            DebugStyle = new GUIStyle();
            DebugStyle.wordWrap = true;
            DebugStyle.normal.textColor = Color.yellow;
            DebugGUIContent = new GUIContent();

            FlowChart_GUIContent = new GUIContent();
        }

        void NodeManager_Drawer_OnDisable()
        {
            OnEditorSkinChange -= NodeManager_Drawer_HandleOnSkinChange;
        }

        #endregion


        void NodeManager_Drawer_OnGUI()
        {
            NodeManager_Drawer_DrawMain();
            NodeManager_Drawer_DrawCurrentlyEditingFlowChart();
            // NodeManager_Drawer_DrawDebugger();
        }


        #region Draw Main Methods
        void NodeManager_Drawer_DrawMain()
        {
            //========== DRAW NODE ARROW LINES ===========
            for (int i = 0; i < _arrowConnectionLines.Count; i++)
            {
                _arrowConnectionLines[i].Draw();
            }

            switch (_toolBarState)
            {
                case ToolBarState.NORMAL:
                    NodeManager_Drawer_DrawMain_ToolBarState_NORMAL();
                    break;

                case ToolBarState.ARROW:
                    NodeManager_Drawer_DrawMain_ToolBarState_ARROW();
                    break;
            }


        }

        ///<Summary>Draw whatever is supposed to be drawn during arrow mode. This includes: (block nodes with a box, their label and a button which says "Connect" which when pressed, will connect the current selected node towards that node) </Summary>
        private void NodeManager_Drawer_DrawMain_ToolBarState_ARROW()
        {
            //Draw from the bottom up
            for (int i = 0; i < _allBlockNodes.Count; i++)
            {
                _allBlockNodes[i].Draw_ToolBarState_ARROW();
            }
        }

        ///<Summary>Draw whatever is supposed to be drawn during normal mode. This includes: (block nodes with just a box with their label), (arrow lines) and (selection box) </Summary>
        private void NodeManager_Drawer_DrawMain_ToolBarState_NORMAL()
        {
            //Draw from the bottom up
            for (int i = 0; i < _allBlockNodes.Count; i++)
            {
                _allBlockNodes[i].Draw_ToolBarState_NORMAL();
            }

            //Draw Selection box
            if (_dragState == DragState.DrawSelection_HadDragged)
            {
                Event e = Event.current;
                _selectionBox.width = e.mousePosition.x - _selectionBox.x;
                _selectionBox.height = e.mousePosition.y - _selectionBox.y;

                Color prevColour = GUIExtensions.Start_GUI_ColourChange(SELECTIONBOX_COLOUR);
                GUI.Box(_selectionBox, string.Empty, BlockNodeBoxStyle);
                GUIExtensions.End_GUI_ColourChange(prevColour);
            }
        }

        #endregion

        #region Dubugger

        void NodeManager_Drawer_DrawDebugger()
        {
            Rect rect = Rect.zero;
            //Abit of border
            rect.x = 5f;
            rect.y = TOOLBAR_HEIGHT;

            string debugStatement = $"Number of selected blocks: {_selectedBlocks.Count} \n Drag State: {_dragState} ";


            DebugGUIContent.text = debugStatement;
            rect.size = DebugStyle.CalcSize(DebugGUIContent);

            GUI.Label
            (
                rect,
                DebugGUIContent,
                DebugStyle
            );
        }

        void NodeManager_Drawer_DrawCurrentlyEditingFlowChart()
        {
            Rect rect = Rect.zero;
            //Abit of border
            rect.x = 5f;
            rect.y = TOOLBAR_HEIGHT;


            string statement = $"Flowchart: {_flowChart.name}";


            FlowChart_GUIContent.text = statement;
            rect.size = DebugStyle.CalcSize(FlowChart_GUIContent);

            bool wasPressed = GUI.Button(rect, FlowChart_GUIContent, DebugStyle);
            if (wasPressed)
            {
                //Ping the flowchart object
                EditorGUIUtility.PingObject(_flowChart);
            }
        }
        #endregion

        #region  Handle Methods

        private void NodeManager_Drawer_HandleOnSkinChange(bool isDark)
        {
            BlockNodeConnectButtonStyle.normal.textColor = GUI.skin.button.normal.textColor;
            BlockNodeBoxStyle.normal.textColor = GUI.skin.box.normal.textColor;
        }
        #endregion

    }

}