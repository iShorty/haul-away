namespace LinearEffectsEditor
{
    using System.Collections.Generic;
    using System;
    using UnityEngine;
    using UnityEditor;

    public partial class FlowChartWindowEditor : EditorWindow
    {

        #region  Events
        delegate void DragCallback(Vector2 mouseDelta);
        public delegate void EditorSkinChangeCallback(bool isDark);

        static event DragCallback OnPan = null;

        //Is called when mouse is clicked in a area which is not covered by: Toolbar
        static event Action OnLeftMouseDownInGraph = null;
        static event Action OnLeftMouseUpInGraph = null;
        static event DragCallback OnMouseDrag = null;
        public static event EditorSkinChangeCallback OnEditorSkinChange = null;
        #endregion


        bool _isPanning;


        void ProcessEvent_OnEnable()
        {
            _isPanning = false;
            _wasPrevSkinDark = EditorGUIUtility.isProSkin;
            ProcessEvent_InitializeNodeMenu();

        }

        void ProcessEvent_OnDisable()
        {
            OnPan = null;
            OnMouseDrag = null;
            OnLeftMouseUpInGraph = null;
            OnEditorSkinChange = null;
        }


        void ProcessEvent_OnGUI()
        {
            ProcessEvent_EditorSkinChange();

            switch (_toolBarState)
            {
                case ToolBarState.NORMAL:
                    ProcessEvent_ProcessToolBarState_NORMAL();
                    break;
                case ToolBarState.ARROW:
                    ProcessEvent_ProcessToolBarState_ARROW();
                    break;

                default:
                    Debug.Log($"{_toolBarState}'s Toolbar Process Event has not been implemented!");
                    break;
            }
        }


        #region Editor Skin Change
        bool _wasPrevSkinDark;

        private void ProcessEvent_EditorSkinChange()
        {
            //Check if current skin colour is same as prev one
            if (_wasPrevSkinDark != EditorGUIUtility.isProSkin)
            {
                _wasPrevSkinDark = EditorGUIUtility.isProSkin;
                OnEditorSkinChange?.Invoke(_wasPrevSkinDark);
            }
        }

        #endregion

        #region Tool Bar States
        ///<Summary>Events included in ARROW mode is the same as NORMAL mode except that there is no selection box, selecting and dragging around of node blocks</Summary>
        private void ProcessEvent_ProcessToolBarState_ARROW()
        {
            Event e = Event.current;

            switch (e.type)
            {
                case EventType.Repaint:
                    return;
                case EventType.Layout:
                    return;

                //======================== MOUSE DOWN ============================
                case EventType.MouseDown:

                    if (_toolBarRect.Contains(e.mousePosition, true))
                    {
                        return;
                    }

                    switch (e.button)
                    {
                        //========== MOUSE DOWN - LEFTCLICK =================
                        case 0:
                            if (e.alt)
                            {
                                _isPanning = true;
                                return;
                            }
                            break;

                        //========== MOUSE DOWN - RIGHTCLICK =================
                        case 1:

                            _nodeMenu_ARROW.ShowAsContext();
                            break;

                        //No intention of calling other mouse clicks
                        default: return;
                    }
                    break;

                //======================== MOUSE UP ============================
                case EventType.MouseUp:
                    if (_toolBarRect.Contains(e.mousePosition, true))
                    {
                        return;
                    }

                    if (_isPanning)
                    {
                        _isPanning = false;
                        return;
                    }
                    break;

                //======================== MOUSE DRAG ============================
                case EventType.MouseDrag:
                    if (_isPanning)
                    {
                        OnPan?.Invoke(e.delta * 0.5f);
                        e.Use();
                        return;
                    }
                    break;


            }
        }

        ///<Summary>Events included in NORMAL are: Panning, selecting nodes (either through clicking or selection box), Dragging nodes</Summary>
        private void ProcessEvent_ProcessToolBarState_NORMAL()
        {
            Event e = Event.current;

            switch (e.type)
            {
                case EventType.Repaint:
                    return;
                case EventType.Layout:
                    return;

                //======================== MOUSE DOWN ============================
                case EventType.MouseDown:

                    if (_toolBarRect.Contains(e.mousePosition, true))
                    {
                        return;
                    }

                    switch (e.button)
                    {
                        //========== MOUSE DOWN - LEFTCLICK =================
                        case 0:
                            if (e.alt)
                            {
                                _isPanning = true;
                                return;
                            }

                            OnLeftMouseDownInGraph?.Invoke();
                            break;

                        //========== MOUSE DOWN - RIGHTCLICK =================
                        case 1:

                            _nodeMenu_NORMAL.ShowAsContext();
                            break;

                        //No intention of calling other mouse clicks
                        default: return;
                    }
                    break;

                //======================== MOUSE UP ============================
                case EventType.MouseUp:
                    if (_toolBarRect.Contains(e.mousePosition, true))
                    {
                        return;
                    }

                    if (_isPanning)
                    {
                        _isPanning = false;
                        return;
                    }

                    //Else invoke the event of mouseup inside of the graph
                    OnLeftMouseUpInGraph?.Invoke();
                    break;

                //======================== MOUSE DRAG ============================
                case EventType.MouseDrag:
                    if (_isPanning)
                    {
                        OnPan?.Invoke(e.delta * 0.5f);
                        e.Use();
                        return;
                    }

                    //Else it is likely that user is attempting to drag
                    if (OnMouseDrag != null)
                    {
                        OnMouseDrag.Invoke(e.delta);
                        e.Use();
                    }
                    break;


            }
        }
        #endregion


        #region  Node Menu
        GenericMenu _nodeMenu_NORMAL = null
        , _nodeMenu_ARROW = null
        ;
        const string NODEMENU_NEWBLOCK = "New Block"
        , NODEMENU_DUPLICATEBLOCK = "Duplicate"
        , NODEMENU_DELETEBLOCK = "Delete"
        , NODEMENU_DRAWARROW = "Draw Arrow"
        , NODEMENU_EXITARROW = "Exit Arrow Mode"
        ;

        void ProcessEvent_InitializeNodeMenu()
        {
            // ============== NORMAL NODE MENU =========
            _nodeMenu_NORMAL = new GenericMenu();
            _nodeMenu_NORMAL.AddItem(new GUIContent(NODEMENU_NEWBLOCK), false, () => NodeManager_NodeCycler_TriggerCreateNewNode(AddNewBlockFrom.ContextMenu));
            _nodeMenu_NORMAL.AddItem(new GUIContent(NODEMENU_DUPLICATEBLOCK), false, NodeManager_NodeCycler_DuplicateSelectedNodes);
            _nodeMenu_NORMAL.AddItem(new GUIContent(NODEMENU_DELETEBLOCK), false, NodeManager_NodeCycler_DeleteButton);
            _nodeMenu_NORMAL.AddItem(new GUIContent(NODEMENU_DRAWARROW), false, ToolBar_TryEnterArrowState);

            // ============== ARROW NODE MENU =========
            _nodeMenu_ARROW = new GenericMenu();
            _nodeMenu_ARROW.AddItem(new GUIContent(NODEMENU_EXITARROW), false, ToolBar_ExitArrowState);

        }
        #endregion
    }

}