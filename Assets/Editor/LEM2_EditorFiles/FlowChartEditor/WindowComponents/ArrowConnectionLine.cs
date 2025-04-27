namespace LinearEffectsEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using System;

    [System.Serializable]
    public class ArrowConnectionLine
    {
        public delegate void RemoveArrowConnectionLineCallback(string from, string to);

        #region Constants
        const float LINE_WIDTH = 5f
        , LINE_TRIANGLE_HALF_WIDTH = 5
        , LINE_TRAINGLE_HALF_HEIGHT = 5
        , LINE_REMOVEBUTTON_SIZE = 5f
        , LINE_REMOVEBUTTON_PICKSIZE = 7f
        , LINE_TRAINGLE_THICKNESS = 7.5f
        , LINE_FIRST_TRIANGLE_POSITION_PERCENTAGE = 0.333f
        , LINE_SECOND_TRIANGLE_POSITION_PERCENTAGE = 0.666f
        ;

        static readonly Color LIGHT_THEME_CONNECTIONLINE_COLOUR = Color.black;
        static readonly Color DARK_THEME_CONNECTIONLINE_COLOUR = Color.white;
        static readonly Vector2 REMOVEBUTTON_SIZE = new Vector2(50f, 50f);
        #endregion


        #region Cached Variables

        public BlockNode StartNode { get; private set; }
        public BlockNode EndNode { get; private set; }

        Color _lineColour = default;

        ///<Summary>The delegate assigned wil be called when the Remove button is pressed to remove a arrow connection line which is connecting from this block towards the block with the ConnectedTowardsBlockName</Summary>
        RemoveArrowConnectionLineCallback onRemove;

        #endregion

        public ArrowConnectionLine(BlockNode start, BlockNode end, RemoveArrowConnectionLineCallback onRemove)
        {
            this.onRemove = onRemove;
            StartNode = start;
            EndNode = end;
            UpdateConnectionLineColour(EditorGUIUtility.isProSkin);
            FlowChartWindowEditor.OnEditorSkinChange += UpdateConnectionLineColour;
        }

        ~ArrowConnectionLine()
        {
            FlowChartWindowEditor.OnEditorSkinChange -= UpdateConnectionLineColour;
        }

        public void Draw()
        {
            Color prevColour;
            prevColour = Handles.color;
            Handles.color = _lineColour;

            Handles.BeginGUI();


            //Useful vars
            Vector2 startPoint = StartNode.OutConnectionPoint, endPoint = EndNode.InConnectionPoint;
            Vector2 dir = endPoint - startPoint;
            Vector2 dirNormalized = dir.normalized;

            //=========== DRAW LINE =============
            Handles.DrawAAPolyLine(LINE_WIDTH, startPoint, endPoint);

            //============ DRAW TRIANGLES ===========
            //Draw one triangle 1/3 along the line 
            Vector2 currCenterPoint = dir * LINE_FIRST_TRIANGLE_POSITION_PERCENTAGE + startPoint;
            DrawTriangleArrow(currCenterPoint, dirNormalized);
            //Draw one triangle 2/3 along the line 
            currCenterPoint = dir * LINE_SECOND_TRIANGLE_POSITION_PERCENTAGE + startPoint;
            DrawTriangleArrow(currCenterPoint, dirNormalized);

            //============ DRAW REMOVE BUTTON ===========
            DrawRemoveButton();

            Handles.EndGUI();
            Handles.color = prevColour;
        }

        private void DrawRemoveButton()
        {
            //The halfway mark for the line
            Vector2 centerPoint = (StartNode.Center + EndNode.Center) * 0.5f;
            if (Handles.Button(centerPoint, Quaternion.identity, LINE_REMOVEBUTTON_SIZE, LINE_REMOVEBUTTON_PICKSIZE, Handles.RectangleHandleCap))
            {
                onRemove?.Invoke(StartNode.Label, EndNode.Label);
            }
        }

        void DrawTriangleArrow(Vector2 centerPoint, Vector2 dir)
        {
            //Instruction to visualise: Draw a unit circle and draw a line to any direction inside that circle.
            //The centerPoint will be the center of that line 
            //Imagine a triangle being drawn at that center pointing towards the circle's edges from the origin of the unit circle
            //=========== DRAW TRIANGLE =============
            //------- Top Point ---------
            // Vector2 centerPoint = (StartNode.Center + EndNode.Center) * 0.5f;
            // Vector2 dir = (EndNode.Center - StartNode.Center).normalized;
            Vector2 triangleTopPoint = centerPoint + dir * LINE_TRAINGLE_HALF_HEIGHT;


            Vector2 orthonganalDir = Vector2.Perpendicular(dir);

            Vector2 triangleBaseCenter = centerPoint + (-dir * LINE_TRAINGLE_HALF_HEIGHT);
            Vector2 triangleBottomLeftPoint = triangleBaseCenter + (orthonganalDir * LINE_TRIANGLE_HALF_WIDTH);
            Vector2 triangleBottomrightPoint = triangleBaseCenter + (-orthonganalDir * LINE_TRIANGLE_HALF_WIDTH);
            Handles.DrawAAPolyLine(LINE_TRAINGLE_THICKNESS, triangleTopPoint, triangleBottomLeftPoint, triangleBottomrightPoint, triangleTopPoint);


        }


        void UpdateConnectionLineColour(bool isDark)
        {
            _lineColour = !isDark ? LIGHT_THEME_CONNECTIONLINE_COLOUR : DARK_THEME_CONNECTIONLINE_COLOUR;
            // _lineColour = !EditorGUIUtility.isProSkin ? LIGHT_THEME_CONNECTIONLINE_COLOUR : DARK_THEME_CONNECTIONLINE_COLOUR;
        }

    }

}