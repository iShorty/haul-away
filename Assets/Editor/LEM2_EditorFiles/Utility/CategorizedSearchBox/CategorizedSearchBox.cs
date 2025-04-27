namespace CategorizedSearchBox
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;

    public partial class CategorizedSearchBox
    {
        #region SearchBar Fields
        SearchField _searchBar = default;
        Rect _searchBarRect = default;


        string _searchedBarText = String.Empty;


        //======= EVENTS ========

        event SearchBarTextChangeCallback OnSearchBarTextChange = null;
        //Pls add Repaint() inside of this event
        event OnPressConfirmCallback OnPressConfirm = null;
        event UpOrDownArrowPressedCallback OnUpOrDownArrowPressed = null;

        #endregion

        #region ResultBox Fields
        Rect _resultBoxRect = default;
        Vector2 _scrollPosition = default;

        List<string> _library, _results;
        HashSet<string> _categoriesToBeDrawn = default;
        int _currentlySelectedResult = -1;

        #region Constants
        public const string CATEGORY_IDENTIFIER = "/", CATEGORY_ARROWSYMBOL = "＞ ";
        const float BAR_CANCELICON_WIDTH = 17.5f
        ;

        static GUIStyle STYLE_RESULTS_EVEN = default
        , STYLE_RESULTS_ODD = default
        , STYLE_RESULTS_SELECTED = default
        ;
        #endregion

        #endregion

        ///<Summary>
        ///Returns the height that the entire searchbox will need to occupy
        ///</Summary>
        public virtual float Handle_OnGUI(Rect searchBarRect, float resultBoxHeight)
        {
            //========= SEARCHBAR ==============
            SearchBar_OnGUI(searchBarRect);

            //=========== RESULTBOX ===============
            //Substract the cross icon's width
            ResultBox_OnGUI(resultBoxHeight);


            return _searchBarRect.height + _resultBoxRect.height;
        }


        #region Search Bar Methods
        protected void SearchBar_OnGUI(Rect rect)
        {
            //====== UPDATE BAR RECT =========
            _searchBarRect = rect;

            //====== UPDATE THE SEARCH BAR TEXT ============
            string newSearchBarText = _searchBar.OnGUI(_searchBarRect, _searchedBarText);

            if (newSearchBarText == _searchedBarText)
            {
                return;
            }

            _searchedBarText = newSearchBarText;
            RaiseSearchBarTextChange(_searchedBarText);
        }


        #endregion

        #region Result Box Methods
        void ResultBox_OnGUI(float searchBoxHeight)
        {
            ResultBox_UpdateValues(searchBoxHeight);
            ResultBox_DrawResults();
        }

        void ResultBox_UpdateValues(float searchBoxHeight)
        {
            //must occur after _barRect updates itself
            //====== UPDATE BOX RECT =========
            _resultBoxRect.x = _searchBarRect.x;
            _resultBoxRect.y = _searchBarRect.y + _searchBarRect.height;
            _resultBoxRect.width = _searchBarRect.width - BAR_CANCELICON_WIDTH;
            _resultBoxRect.height = searchBoxHeight;

        }

        void ResultBox_DrawResults()
        {
            // ============ DRAW A SPACE ===============
            EditorGUILayout.Space(_searchBarRect.y + _searchBarRect.height);

            // ============ SCROLL VIEW ================
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true, GUILayout.MinWidth(_resultBoxRect.width), GUILayout.MinHeight(_resultBoxRect.height));

            for (int i = 0; i < _results.Count; i++)
            {
                string result = _results[i];
                GUIStyle resultStyle = GetResultStyle(i);

                //If no category was found, just draw the result as it is
                if (!TryGetCategory(_searchedBarText, result, out string category))
                {
                    //Remove any path categories before the result found if the forward path (in this case is _searchedBarText) is not empty or null

                    //========= FRONT PATH NOT EMPTY ==========
                    if (!String.IsNullOrEmpty(_searchedBarText))
                    {
                        //Get prev category slash's index if possible. If it is possible then remove everything from start to that slash
                        //Example below to visualize

                        //_searchedBarText = Hello/Debu
                        //Result = Hello/Debug/1
                        result = TryGetIndexOfPrevCategory(_searchedBarText, out int prevCategoryStartingIndex) ? result.Remove(0, prevCategoryStartingIndex + 1) : result;
                    }


                    EditorGUILayout.LabelField(result, resultStyle);
                    ProcessResult(result);
                    continue;
                }

                //=== DRAWING CATERGORY =====
                EditorGUILayout.LabelField(CATEGORY_ARROWSYMBOL + category, resultStyle);
                ProcessResult(category);
            }

            GUILayout.EndScrollView();

        }

        GUIStyle GetResultStyle(int resultIndex)
        {
            if (resultIndex == _currentlySelectedResult) return STYLE_RESULTS_SELECTED;

            return resultIndex % 2 == 0 ? STYLE_RESULTS_EVEN : STYLE_RESULTS_ODD;
        }

        //frontPathToExclude must be start from index 0 of fullPath
        bool TryGetCategory(string frontPathToExclude, string fullPath, out string category)
        {
            category = string.Empty;

            //====== FRONT PATH IS EMPTY ==========
            if (String.IsNullOrEmpty(frontPathToExclude))
            {
                return TryGetCategory(fullPath, out category);
            }

            //========= FRONT PATH NOT EMPTY ==========
            //Get prev category slash's index if possible. If it is possible then remove everything from start to that slash
            //Example below to visualize

            //frontPathToExclude = Hello/Debu
            //fullPath = Hello/Debug/1
            if (!TryGetIndexOfPrevCategory(frontPathToExclude, out int prevCategoryStartingIndex))
            {
                return TryGetCategory(fullPath, out category);
            }

            string stringToSearchFrom = fullPath.Remove(0, prevCategoryStartingIndex + 1);
            return TryGetCategory(stringToSearchFrom, out category);
        }

        bool TryGetCategory(string s, out string category)
        {
            category = string.Empty;

            int slashChar = s.IndexOf(CATEGORY_IDENTIFIER);

            if (slashChar == -1) return false;

            category = s.Substring(0, slashChar);
            return true;
        }

        bool TryGetIndexOfPrevCategory(string s, out int categoryStartingIndex)
        {
            categoryStartingIndex = s.LastIndexOf(CATEGORY_IDENTIFIER);

            if (categoryStartingIndex == -1) return false;

            return true;
        }




        //Must only be called after the EditorGUILayout field is drawn
        void ProcessResult(string resultName)
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            Event e = Event.current;

            switch (e.type)
            {
                //==============MOUSE UP EVENT ================
                case EventType.MouseUp:
                    if (rect.Contains(e.mousePosition, true))
                    {
                        RaiseOnConfirm(resultName);
                    }
                    break;


                //============== KEY DOWN EVENT ================
                case EventType.KeyDown:
                    switch (e.keyCode)
                    {
                        case KeyCode.UpArrow:
                            RaiseDownOrUpArrowKeyPressed(true);
                            break;

                        case KeyCode.DownArrow:
                            RaiseDownOrUpArrowKeyPressed(false);
                            break;

                        case KeyCode.KeypadEnter:
                            if (_currentlySelectedResult < 0) return;

                            if (_results[_currentlySelectedResult] != resultName) return;

                            RaiseOnConfirm(resultName);
                            break;

                        //Else do nth
                        default: break;
                    }
                    break;


                //Else do nth
                default: break;

            }


        }

        #endregion



        #region Handle  Events
        #region SearchTextChange Methods
        //======= RAISE SEARCH BAR TEXT CHANGE ==========
        void RaiseSearchBarTextChange(string newSearchBarText)
        {
            _currentlySelectedResult = -1;

            _categoriesToBeDrawn.Clear();
            _results.Clear();

            if (newSearchBarText == string.Empty)
                _results.AddRange(_library);
            else
                _results.AddRange(_library.FindAll(SearchBarSearchPredicate));

            FilterResultCategories(newSearchBarText);

            OnSearchBarTextChange?.Invoke(newSearchBarText);
            Event.current?.Use();
        }

        bool SearchBarSearchPredicate(string s)
        {
            if (s.Contains(_searchedBarText, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        void FilterResultCategories(string frontPath)
        {
            for (int i = 0; i < _results.Count; i++)
            {

                string s = _results[i];

                //if this string doesnt hv a category inside of it, leave it in results
                if (!TryGetCategory(frontPath, s, out string category))
                {
                    continue;
                }

                //Check if the category is found inside of the category hashset
                if (_categoriesToBeDrawn.Contains(category))
                {
                    //if so remove this result element
                    _results.RemoveAt(i);
                    i--;
                    continue;
                }

                //Else
                _categoriesToBeDrawn.Add(category);

            }
        }


        #endregion

        #region  On Confirm Methods
        // ============= RAISE CONFIRM ==============
        //Handles when search bar has "enter" pressed down (when a result has been highlighted using the up & down arrow keys) or when a result has been pressed down
        //resultname can be: category name, result name
        void RaiseOnConfirm(string resultName)
        {
            //Check if the confirmed result is a category. if so, append that category name to the searchbar text
            if (_categoriesToBeDrawn.Contains(resultName))
            {
                OnConfirm_RemoveTillLastCategorySlash();

                _searchedBarText += $"{resultName}/";

                RaiseSearchBarTextChange(_searchedBarText);
                return;
            }

            OnConfirm_RemoveTillLastCategorySlash();

            _searchedBarText += resultName;

            //Else, invoke the onconfirm event cause this is prolly the final result name
            //Close the search box or something?
            OnPressConfirm?.Invoke(_searchedBarText);

            // Event.current.Use();
        }

        //Note, this does not remove the last category's slash
        void OnConfirm_RemoveTillLastCategorySlash()
        {
            int lastZeroIndex = _searchedBarText.Length - 1;
            if (TryGetIndexOfPrevCategory(_searchedBarText, out int categoryStartingIndex))
            {
                //Remove everything that has been incorrect in terms of spelling (assuming the worse case), from prevSlash onwards till the end and then append the correct spelling

                //we need to avoid removing the last category's slash so we carry out this only if the categoryStartingIndex is not the last index
                //If the category is not in the last index on the searchbar string,
                if (categoryStartingIndex == lastZeroIndex)
                {
                    return;
                }

                //Push the starting index by one so as to avoid removing the /
                categoryStartingIndex++;
                _searchedBarText = _searchedBarText.Remove(categoryStartingIndex, _searchedBarText.Length - categoryStartingIndex);
                return;
            }

            //If we cant find a prev category, then we just need to remove the entire searchbar text and replace it later with the correct text
            _searchedBarText = _searchedBarText.Remove(0, _searchedBarText.Length);
        }
        #endregion

        //============= RAISE DOWN OR UP ARROW PRESSED ==============
        private void RaiseDownOrUpArrowKeyPressed(bool upArrowKeyWasPressed)
        {
            int addition = upArrowKeyWasPressed ? -1 : 1;
            _currentlySelectedResult = Mathf.Clamp(_currentlySelectedResult + addition, -1, _results.Count - 1);
            OnUpOrDownArrowPressed?.Invoke(upArrowKeyWasPressed);
            // Event.current.Use();
        }

        #endregion



    }

}