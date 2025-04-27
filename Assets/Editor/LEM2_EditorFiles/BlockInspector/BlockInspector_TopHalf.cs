namespace LinearEffectsEditor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;
    using LinearEffects;
    using System.Collections.Generic;
    using System;

    //The top half class will render the settings & command list
    public partial class BlockInspector : ImprovedEditor
    {

        #region Cached Variable
        ReorderableList _list = default;
        SerializedProperty _settingsProperty = default;
        Vector2 _scrollPosition = default;
        HashSet<int> _selectedElements = default;
        int _firstClickedIndex = -1, _prevClickedIndex = -1;


        #endregion

        #region Properties
        int CurrentClickedListIndex
        {
            get
            {
                return _list.index;
            }
            set
            {
                _list.index = Mathf.Clamp(value, -1, _list.count - 1);
                _firstClickedIndex = _list.index;
                _selectedElements.Clear();
                _selectedElements.Add(_firstClickedIndex);
            }
        }
        #endregion

        #region Constants
        //For now dark theme editor skin doesnt work as good as i thought it would...
        static readonly Color ORDERELEMENT_COLOUR_SELECTED = new Color(73 / 255f, 113 / 255f, 170 / 255f, 1f),
        ORDERELEMENT_COLOUR_UNSELECTED = new Color(0.8f, 0.8f, 0.8f, 1f)
        ;

        const string COPY_REMINDER_TEXT = "Copy Dont Delete";
        const float COPY_REMINDER_RECTWIDTH = 500f;

        #endregion

        #region Statics
        protected static GUIStyle ExecutorNameLabelStyle { get; private set; } = null;
        protected static GUIStyle CopyLabelStyle { get; private set; } = null;
        #endregion

        #region LifeTime Methods

        void TopHalf_OnEnable()
        {
            SerializedProperty orderArray = serializedObject.FindProperty(BlockScriptableInstance.PROPERTYPATH_ORDERARRAY);
            _settingsProperty = serializedObject.FindProperty(BlockScriptableInstance.PROPERTYPATH_SETTINGS);
            _list = new ReorderableList(serializedObject, orderArray, displayAddButton: false, displayHeader: true, displayRemoveButton: false, draggable: true);
            _selectedElements = new HashSet<int>();
            TopHalf_ResetFirstClickedIndex();

            _list.drawHeaderCallback = TopHalf_HandleDrawHeaderCallBack;
            _list.drawElementCallback = TopHalf_HandleDrawElementCallBack;
            _list.elementHeightCallback += TopHalf_HandleElementHeightCallBack;
            _list.onChangedCallback += TopHalf_HandleOnChange;
            _list.onSelectCallback += TopHalf_HandleOnSelect;


            TopHalf_InitializeStyles();
        }

        private void TopHalf_InitializeStyles()
        {
            //Style initialize
            if (ExecutorNameLabelStyle == null)
            {
                ExecutorNameLabelStyle = new GUIStyle(EditorStyles.label);
                ExecutorNameLabelStyle.normal.textColor = Color.black;
            }

            if (CopyLabelStyle == null)
            {
                CopyLabelStyle = new GUIStyle(EditorStyles.label);
                CopyLabelStyle.normal.textColor = Color.red;
                CopyLabelStyle.fontStyle = FontStyle.Italic;
            }

        }

        void TopHalf_OnDisable()
        {
            _list.elementHeightCallback -= TopHalf_HandleElementHeightCallBack;
            _list.onChangedCallback -= TopHalf_HandleOnChange;
            _list.onSelectCallback -= TopHalf_HandleOnSelect;

            _list = null;

        }


        //Calll the reorderable list to update itself
        void TopHalf_OnInspectorGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(_topHalfSize.x), GUILayout.Height(_topHalfSize.y));
            EditorGUILayout.Space();
            TopHalf_DrawSettings();
            EditorGUILayout.Space(20f);
            TopHalf_DrawReOrderableList();
            EditorGUILayout.EndScrollView();
        }

        #endregion


        #region Event Handlers


        private void TopHalf_HandleOnSelect(ReorderableList list)
        {

            int clickedIndex = list.index;
            _prevClickedIndex = _firstClickedIndex;

            //====================== SHIFT CLICK =========================
            if (Event.current.shift && _firstClickedIndex != -1)
            {
                _selectedElements.Clear();

                TopHalf_GetSelectedForLoopValues(out int diff, out int direction, out _firstClickedIndex);

                for (int i = 0; i <= diff; i++)
                {
                    _selectedElements.Add(_firstClickedIndex + i * direction);
                }

                return;
            }

            //================= NO SHIFT CLICK ======================
            _selectedElements.Clear();
            _selectedElements.Add(clickedIndex);
            _firstClickedIndex = clickedIndex;
        }


        private void TopHalf_HandleDrawHeaderCallBack(Rect rect)
        {
            EditorGUI.LabelField(rect, "Effect List");
        }

        private float TopHalf_HandleElementHeightCallBack(int index)
        {
            return EditorGUIUtility.singleLineHeight * 2f;
        }

        private void TopHalf_HandleDrawElementCallBack(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (!TopHalf_GetOrderArrayElement(index, out SerializedProperty orderElement))
            {
                return;
            }

            //<================ DRAWING MAIN BG =========================>
            //Draw a bg for the entire list rect before we start modifying the rect
            Color colourOfBg = _selectedElements.Contains(index) ? ORDERELEMENT_COLOUR_SELECTED : ORDERELEMENT_COLOUR_UNSELECTED;

            Color prevBgColour = GUIExtensions.Start_GUIBg_ColourChange(colourOfBg);
            GUI.Box(rect, string.Empty, FlowChartWindowEditor.BlockNodeBoxStyle);
            GUIExtensions.End_GUIBg_ColourChange(prevBgColour);

            //<================ DRAWING COMMAND TYPE=========================>
            if (!EditorDebugExtension.TryGetProperty(orderElement, Block.EffectOrder.PROPERTYNAME_EXECUTORNAME, out SerializedProperty dummyProperty)) return;

            //By calculating the size of the content, i can ensure that the error log is always rendered 10 units past the commadntype
            GUIContent content = new GUIContent(dummyProperty.stringValue);
            // var style = EditorStyles.label;

            Vector2 sizeOfContent = ExecutorNameLabelStyle.CalcSize(content);

            //Modify rect
            //Shift rect 10units to avoid overlapping the stroke bullet points
            rect.width = sizeOfContent.x;
            rect.height = sizeOfContent.y;
            rect.x += 10;
            rect.y += 10;

            //Draw the Type of Command first
            EditorGUI.LabelField(rect, dummyProperty.stringValue, ExecutorNameLabelStyle);


            // //<================ DRAWING COPY/CUT TARGET =========================>
            if (!HadPreviouslyCopied) return;

            if (!_clipBoardUnOrderedIndices.Contains(index)) return;

            //Modify rect again
            //Shift rect 10 units to give space between errorlog and type of command
            rect.x += rect.width + 10;
            rect.y -= 5;

            rect.width = COPY_REMINDER_RECTWIDTH;


            Color pastLabelColour = GUIExtensions.Start_StyleText_ColourChange(Color.red, EditorStyles.label);
            EditorGUI.LabelField(rect, COPY_REMINDER_TEXT,CopyLabelStyle);
            GUIExtensions.End_StyleText_ColourChange(pastLabelColour, EditorStyles.label);

            //For now this is unneeded
            // //<================ DRAWING ERROR LOG =========================>
            // //Modify rect again
            // //Shift rect 10 units to give space between errorlog and type of command
            // rect.x += rect.width + 10;
            // rect.y -= 5;

            // if (!EditorDebugExtension.TryGetProperty(orderElement, Block.EffectOrder.PROPERTYNAME_ERRORLOG, out dummyProperty)) return;

            // Color pastLabelColour = GUIExtensions.Start_StyleText_ColourChange(Color.red, EditorStyles.label);
            // style.fontStyle = FontStyle.Italic;
            // //Draw Errorlog
            // EditorGUI.LabelField(rect, dummyProperty.stringValue);
            // style.fontStyle = FontStyle.Normal;
            // GUIExtensions.End_StyleText_ColourChange(pastLabelColour, EditorStyles.label);
        }

        private void TopHalf_HandleOnChange(ReorderableList list)
        {
            //Call the recalibration of effect order here in the block
            _target.SaveModifiedProperties();
        }

        #endregion

        #region  Draw Methods
        void TopHalf_DrawSettings()
        {
            if (_settingsProperty == null)
            {
                string debug = $"The property named: {BlockScriptableInstance.PROPERTYPATH_SETTINGS} inside the Block class has been renamed to something else or it doesnt exist anymore!";
                Debug.LogWarning(debug);
                return;
            }

            EditorGUILayout.PropertyField(_settingsProperty, includeChildren: true);
        }

        void TopHalf_DrawReOrderableList()
        {
            _list.DoLayoutList();
        }
        #endregion

        #region Supporting Methods
        bool TopHalf_GetOrderArrayElement(int i, out SerializedProperty property)
        {
            if (i < 0 || i >= _list.count)
            {
                property = null;
                return false;
            }
            property = _list.serializedProperty.GetArrayElementAtIndex(i);
            return true;
        }

        ///<Summary>
        ///Diff is the size difference between first clicked index and current clicked index, if direction is > 0, currentclicked index > firstclickedindex.
        /// Firstclickedindex is the index of the element which was clicked before the currently clicked index.
        ///</Summary>
        bool TopHalf_GetSelectedForLoopValues(out int diff, out int direction, out int firstClickedIndex)
        {
            if (_list.count <= 0 || _firstClickedIndex == -1)
            {
                diff = int.MinValue;
                direction = int.MinValue;
                firstClickedIndex = int.MinValue;
                return false;
            }

            diff = CurrentClickedListIndex - _firstClickedIndex;

            //If its positive it will move downards
            direction = diff > 0 ? 1 : -1;
            diff = Mathf.Abs(diff);
            firstClickedIndex = _firstClickedIndex;
            // Debug.Log($"Diff: {diff} Direction: {direction} FirstclickedIndex {_firstClickedIndex}");
            return true;
        }

        void TopHalf_ResetFirstClickedIndex()
        {
            _firstClickedIndex = -1;
        }
        #endregion

    }

}