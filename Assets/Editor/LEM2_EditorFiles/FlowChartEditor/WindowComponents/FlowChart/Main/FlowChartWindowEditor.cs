namespace LinearEffectsEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using LinearEffects;

    public partial class FlowChartWindowEditor : EditorWindow
    {
        #region Current Cached Variable
        // static FlowChart _target = default;
        static SerializedObject _targetObject = default;
        static BaseFlowChart _flowChart = default;
        static EditorState _state;

        #endregion

        #region Defintition
        enum EditorState
        {
            ///<Summary>State where the window editor has just been opened</Summary>
            UNINITIALIZE = -1
            ,
            ///<Summary>State where the window editor has just been opened and needs to load its values</Summary>
            UNLOADED = 0
            ,
            ///<Summary>State where the window editor has a FlowChart.cs instance being inspected</Summary>
            LOADED = 1
            ,
            RUNTIME_DEBUG = 2
        }
        #endregion

        #region  Properties
        Vector2 CenterScreen => new Vector2(Screen.width, Screen.height) * 0.35f;

        protected static FlowChartWindowEditor instance = null;
        public static bool IsOpen => instance != null;
        #endregion


        #region Unity LifeTime
        // [MenuItem(itemName: "Window/FlowChart Editor")]
        // ///<Summary>Opens the window via buttons by menu item context</Summary>
        // public static void OpenWindow()
        // {
        //     var window = GetWindow<FlowChartWindowEditor>();
        //     window.titleContent = new GUIContent("FlowChartEditor");
        //     // instance = window;
        //     ReloadWindow();
        // }

        ///<Summary>Opens the window via buttons by passing in flowchart reference</Summary>
        public static void OpenWindow(BaseFlowChart flowChart)
        {
            var window = GetWindow<FlowChartWindowEditor>();
            window.titleContent = new GUIContent("FlowChartEditor");

            if (_flowChart == flowChart)
            {
                return;
            }

            _flowChart = flowChart;

            //Reload the window
            ReloadWindow();
        }

        private void OnEnable()
        {
            _state = EditorState.UNINITIALIZE;
            instance = this;
        }

        void OnDisable()
        {
            switch (_state)
            {
                case EditorState.UNLOADED:
                    UNLOADED_OnDisable();
                    break;
                case EditorState.LOADED:
                    LOADED_OnDisable();
                    break;
                case EditorState.RUNTIME_DEBUG:
                    RUNTIME_DEBUG_OnDisable();
                    break;
            }
            SaveManager_SaveFlowChartPath();
        }

        #endregion

        #region Initializations
        ///<Summary>To be called when changing states to ensure that the current editor state's components have their OnDisables called</Summary>
        void TryCallDisable()
        {
            if (_state != EditorState.UNINITIALIZE)
            {
                //Disable the previous state's components
                OnDisable();
            }
        }
        ///<Summary>Called to save the window and call all disables</Summary>
        public static void DisableWindow()
        {
            instance.TryCallDisable();
        }

        ///<Summary>Called to give the window a new initial state</Summary>
        public static void EnableWindow()
        {
            instance.AssignNewInitialState();
        }

        public static void ReloadWindow()
        {
            DisableWindow();
            EnableWindow();
        }

        //I dont put the code in OnEnable because if i want to intialize styles, i have to do in during OnGUI calls
        void UNINITIALIZE_OnGUI()
        {
            //Initalize whatever styles here 
            AssignNewInitialState();
        }

        void AssignNewInitialState()
        {
            switch (EditorApplication.isPlaying)
            {
                //============================ RUNTIME =======================
                case true:
                    Debug.Log("Debug mode for LEM 2 has not been implemented");
                    //Get the flow chart during runtime then
                    _flowChart = SaveManager_TryLoadFlowChartPath_Runtime();

                    //This is for future development if i want to add a runtime debug mode
                    _state = EditorState.RUNTIME_DEBUG;
                    RUNTIME_DEBUG_OnEnable();

                    break;
                //============================ EDITOR TIME ==========================
                case false:
                    // Debug.Log($"Editor is not playing. Is flowchart reference present?: { _flowChart != null}");
                    //Try get previous flowchart
                    if (_flowChart == null)
                    {
                        _flowChart = SaveManager_TryLoadFlowChartPath();
                        // Debug.Log($"Tried to find previous flowchart... Flowchart found: {_flowChart}");
                    }


                    //This means user has opened flowchart editor from menu context
                    //Which for now..... is empty!
                    if (_flowChart == null)
                    {
                        _state = EditorState.UNLOADED;
                        UNLOADED_OnEnable();
                        return;
                    }

                    //This means user has opened flowchart editor via button press 
                    _state = EditorState.LOADED;
                    LOADED_OnEnable();

                    break;
            }

        }
        #endregion


        void OnGUI()
        {
            switch (_state)
            {
                case EditorState.UNINITIALIZE:
                    UNINITIALIZE_OnGUI();
                    break;
                case EditorState.UNLOADED:
                    UNLOADED_OnGUI();
                    break;
                case EditorState.LOADED:
                    LOADED_OnGUI();
                    break;
                case EditorState.RUNTIME_DEBUG:
                    RUNTIME_DEBUG_OnGUI();
                    break;
            }


        }


        #region Enable Disable Proxy Calls
        void UNLOADED_OnEnable() { }

        void UNLOADED_OnDisable() { }

        ///<Summary>Is called when there is a FlowChart.cs instance found. Calls all the various components to call their OnEnables</Summary>
        void LOADED_OnEnable()
        {
            _targetObject = new SerializedObject(_flowChart);
            LoadedBackground_OnEnable();
            NodeManager_OnEnable();
            ToolBar_OnEnable();
            ProcessEvent_OnEnable();
            BlockEditor_OnEnable();
        }

        ///<Summary>Is called on Disable if the window editor is LOADED</Summary>
        void LOADED_OnDisable()
        {
            LoadedBackground_OnDisable();
            NodeManager_OnDisable();
            ToolBar_OnDisable();
            ProcessEvent_OnDisable();
            BlockEditor_OnDisable();
        }

        void RUNTIME_DEBUG_OnEnable()
        {
            Debug.Log("Entering playmode");
        }

        void RUNTIME_DEBUG_OnDisable()
        {

        }
        #endregion

        #region GUI Proxy Calls
        void LOADED_OnGUI()
        {
            //Incase player accidentally deletes the flowchart gameobject
            if (_flowChart == null)
            {
                AssignNewInitialState();
                return;
            }
            //=========== DRAW ORDER===============
            LoadedBackground_OnGUI();
            NodeManager_OnGUI();
            ToolBar_OnGUI();
            ProcessEvent_OnGUI();
        }

        void UNLOADED_OnGUI()
        {
            UnloadedBackground_OnGUI();
        }

        void RUNTIME_DEBUG_OnGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                TryCallDisable();
                AssignNewInitialState();
                return;
            }
        }


        #endregion




    }
}