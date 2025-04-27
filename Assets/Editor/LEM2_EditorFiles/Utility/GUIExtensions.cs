    using UnityEngine;
    using UnityEditor;

    public static class GUIExtensions
    {

        #region Rect Extensions
        public static Vector2 TopLeft(this Rect rect)
        {
            Vector2 v = Vector2.zero;
            v.x = rect.xMin;
            v.y = rect.yMax;
            return v;
        }

        public static Vector2 TopRight(this Rect rect)
        {
            Vector2 v = Vector2.zero;
            v.x = rect.xMax;
            v.y = rect.yMax;
            return v;
        }

        public static Vector2 BottomLeft(this Rect rect)
        {
            Vector2 v = Vector2.zero;
            v.x = rect.xMin;
            v.y = rect.yMin;
            return v;
        }

        public static Vector2 BottomRight(this Rect rect)
        {
            Vector2 v = Vector2.zero;
            v.x = rect.xMax;
            v.y = rect.yMin;
            return v;
        }




        public static Rect ScaleAboutPivot(this Rect rect, Vector2 pivot, Vector2 scale)
        {
            rect.position -= pivot;
            rect.size = Vector2.Scale(rect.size, scale);
            rect.position += pivot;
            return rect;
        }

        public static Rect ScaleAboutPivot(this Rect rect, Vector2 pivot, float scale)
        {
            rect.position -= pivot;
            rect.size *= scale;
            rect.position += pivot;
            return rect;
        }


        #endregion




        #region Colour Changes
        public static Color Start_Handles_ColourChange(Color newColour)
        {
            Color prevColour = Handles.color;
            Handles.color = newColour;
            return prevColour;
        }
        public static void End_Handles_ColourChange(Color prevColour)
        {
            Handles.color = prevColour;
        }


        public static Color Start_GUI_ColourChange(Color guiColour)
        {
            Color prevColour = GUI.color;
            GUI.color = guiColour;
            return prevColour;
        }
        public static void End_GUI_ColourChange(Color prevColour)
        {
            GUI.color = prevColour;
        }

        public static Color Start_StyleText_ColourChange(Color textColour, GUIStyle style)
        {
            Color prevColour = style.normal.textColor;
            style.normal.textColor = textColour;
            return prevColour;
        }
        public static void End_StyleText_ColourChange(Color prevColour, GUIStyle style)
        {
            style.normal.textColor = prevColour;
        }

        public static Color Start_GUIBg_ColourChange(Color bgColour)
        {
            Color prevColour = GUI.backgroundColor;
            GUI.backgroundColor = bgColour;
            return prevColour;
        }
        public static void End_GUIBg_ColourChange(Color prevColour)
        {
            GUI.backgroundColor = prevColour;
        }

        #endregion

    }