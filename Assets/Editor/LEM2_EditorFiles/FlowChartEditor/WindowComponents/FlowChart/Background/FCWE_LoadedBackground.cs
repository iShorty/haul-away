namespace LinearEffectsEditor
{
    using UnityEngine;
    using UnityEditor;

    //Handles drawing and processing events for the background when editor is loaded
    public partial class FlowChartWindowEditor : EditorWindow
    {
        const int GRID_SMALL_SPACE = 10;
        const int GRID_LARGE_SPACE = 50;

        Vector3 _background_Offset;
        Color _grid1Colour = default
        , _grid2Colour = default
        ;

        void LoadedBackground_OnEnable()
        {
            OnPan += LoadedBackground_HandlePan;
            _background_Offset = Vector3.zero;

            //======== EDITOR SKIN ========
            LoadedBackground_GetGrid1Colour(EditorGUIUtility.isProSkin);
            LoadedBackground_GetGrid2Colour(EditorGUIUtility.isProSkin);

            OnEditorSkinChange += LoadedBackground_GetGrid1Colour;
            OnEditorSkinChange += LoadedBackground_GetGrid2Colour;
        }

        void LoadedBackground_OnDisable()
        {
            OnPan -= LoadedBackground_HandlePan;

            OnEditorSkinChange -= LoadedBackground_GetGrid1Colour;
            OnEditorSkinChange -= LoadedBackground_GetGrid2Colour;
        }

        void LoadedBackground_HandlePan(Vector2 mouseDelta)
        {
            // mouseDelta *= 0.5f;
            _background_Offset.x += mouseDelta.x;
            _background_Offset.y += mouseDelta.y;

        }

        void LoadedBackground_OnGUI()
        {
            LoadedBackground_DrawGrid(GRID_SMALL_SPACE, _grid1Colour);
            LoadedBackground_DrawGrid(GRID_LARGE_SPACE, _grid2Colour);
        }

        #region Draw
        void LoadedBackground_DrawGrid(float gridspacing, Color lineColour)
        {
            Handles.BeginGUI();
            // Color grid1Colour = GetGrid1Colour(), grid2Colour = GetGrid2Colour();
            Color prevColour = GUIExtensions.Start_Handles_ColourChange(lineColour);

            //=======================DRAW HORIZONTAL LINES===========================
            //Divide the total amount of offset by gridspacing. if remainder is 0, that means we dont need to draw new lines cause the canvas moved the exactly the same distance as gridspacing multiplied by a factor. So it is as if we didnt move the canvas however, if remainder is not zero, we have an offset value of ranging from 0 < value < gridspacing with that offset, we can draw lines at a new position inbetween the usual grid lines positions
            Vector3 adjustedOffset = _background_Offset;
            adjustedOffset.x %= gridspacing;
            adjustedOffset.y %= gridspacing;

            //Ensure that startV & endV is at least one Gridspace behind the screen's actual starting point
            Vector3 startV = Vector3.left * gridspacing, endV = Vector3.right * (position.width + gridspacing);
            int numberOfLines = Mathf.CeilToInt((position.height / gridspacing));

            for (int i = 0; i < numberOfLines; i++)
            {
                startV.y = endV.y = gridspacing * i;
                Handles.DrawLine(startV + adjustedOffset, endV + adjustedOffset);
            }

            startV = Vector3.down * gridspacing;
            endV = Vector3.up * (position.height + gridspacing);
            numberOfLines = Mathf.CeilToInt((position.width / gridspacing));

            for (int i = 0; i < numberOfLines; i++)
            {
                startV.x = endV.x = gridspacing * i;
                Handles.DrawLine(startV + adjustedOffset, endV + adjustedOffset);
            }

            GUIExtensions.End_GUI_ColourChange(prevColour);
            Handles.EndGUI();
        }
        #endregion


        #region Colours
        //https://www.rapidtables.com/web/color/gray-color.html
        static readonly Color LIGHT_GRIDLINE1_COLOUR = new Color(0.66f, 0.66f, 0.66f, 0.5f);
        static readonly Color LIGHT_GRIDLINE2_COLOUR = new Color(0.41f, 0.41f, 0.41f, 0.5f);

        static readonly Color DARK_GRIDLINE1_COLOUR = new Color(0.41f, 0.41f, 0.41f, 0.5f);
        static readonly Color DARK_GRIDLINE2_COLOUR = new Color(0.75f, 0.75f, 0.75f, 0.5f);

        void LoadedBackground_GetGrid1Colour(bool isDarkSkin)
        {
            _grid1Colour = !isDarkSkin ? LIGHT_GRIDLINE1_COLOUR : DARK_GRIDLINE1_COLOUR;
            // return !isDarkSkin ? LIGHT_GRIDLINE1_COLOUR : DARK_GRIDLINE1_COLOUR;
        }

        void LoadedBackground_GetGrid2Colour(bool isDarkSkin)
        {
            _grid2Colour = !isDarkSkin ? LIGHT_GRIDLINE2_COLOUR : DARK_GRIDLINE2_COLOUR;
            // return !isDarkSkin ? LIGHT_GRIDLINE2_COLOUR : DARK_GRIDLINE2_COLOUR;
        }
        #endregion


    }

}