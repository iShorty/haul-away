namespace LinearEffectsEditor
{
    using UnityEngine;
    using UnityEditor;
    using LinearEffects;
    using System;

    public partial class BlockInspector : ImprovedEditor
    {

        #region Definitions
        delegate void DragCenterDivisionCallback(float mouseDeltaY);
        #endregion

        event DragCenterDivisionCallback OnDrag = null;
        bool _CenterDiv_isDragging = false;

        void CenterDiv_OnEnable()
        {
            _CenterDiv_isDragging = false;
            OnDrag += HandleDivisonDrag;
        }


        void CenterDiv_OnDisable()
        {
            OnDrag -= HandleDivisonDrag;
        }

         void CenterDiv_OnInspectorGUI()
        {
            //==================DRAW AN EMPTY DUMMY LAYOUT BOX====================
            EditorGUILayout.LabelField(string.Empty);

            //============DRAW THE REAL SEPARATOR USING THE LAYOUT BOX'S RECT====================
            Rect lastRect = GUILayoutUtility.GetLastRect();
            lastRect.x = 0;
            lastRect.width = Screen.width;
            EditorGUI.LabelField(lastRect, string.Empty, GUI.skin.horizontalSlider);


            //===================== PROCESS EVENTS ===========================
            Event e = Event.current;

            switch (_CenterDiv_isDragging)
            {
                case false:
                    //Run logic if mouse is over the division
                    if (!lastRect.Contains(e.mousePosition))
                    {
                        return;
                    }

                    //  Change cursor
                    EditorGUIUtility.AddCursorRect(lastRect, MouseCursor.SplitResizeUpDown);

                    //If mouse is clicked on the left click,
                    if (e.type != EventType.MouseDown || e.button != 0)
                    {
                        return;
                    }

                    _CenterDiv_isDragging = true;
                    break;

                case true:

                    //If mouse button goes up 
                    if (e.type == EventType.MouseUp)
                    {
                        _CenterDiv_isDragging = false;
                        return;
                    }

                    //We only care about mouse events now
                    if (!e.isMouse) return;

                    //Else if mouse is continuing the drag
                    OnDrag?.Invoke(e.delta.y);
                    break;
            }


        }
    }

}