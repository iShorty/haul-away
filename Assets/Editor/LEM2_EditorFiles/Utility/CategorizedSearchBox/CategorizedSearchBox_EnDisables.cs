namespace CategorizedSearchBox
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor.IMGUI.Controls;


    public partial class CategorizedSearchBox
    {
        #region Definitions
        public delegate void SearchBarTextChangeCallback(string newSearchBarText);
        public delegate void OnPressConfirmCallback(string fullResultPathPressed);
        public delegate void UpOrDownArrowPressedCallback(bool upArrowWasPressed);
        #endregion

        public void Initialize(string[] resultsToPopulate)
        {
            _searchBar = new SearchField();
            STYLE_RESULTS_EVEN = new GUIStyle("CN EntryBackOdd");
            STYLE_RESULTS_ODD = new GUIStyle("CN EntryBackEven");
            STYLE_RESULTS_SELECTED = new GUIStyle("PR Ping");

            _library = new List<string>();
            _results = new List<string>();
            _categoriesToBeDrawn = new HashSet<string>();
            _library.AddRange(resultsToPopulate);

            RaiseSearchBarTextChange(string.Empty);
        }

        #region Enables & Disables

        //============ ENABLES ================
        public virtual void EnableSearchBox()
        {
            _currentlySelectedResult = -1;
        }


        public virtual void EnableSearchBox(UpOrDownArrowPressedCallback handleDownOrUpArrowKeyPressed, SearchBarTextChangeCallback handleSearchBarTextChanged, OnPressConfirmCallback handleOnConfirmPressed)
        {
            EnableSearchBox();
            OnSearchBarTextChange += handleSearchBarTextChanged;
            OnUpOrDownArrowPressed += handleDownOrUpArrowKeyPressed;
            OnPressConfirm += handleOnConfirmPressed;
        }

        //========= DISABLES ===============
        public virtual void DisableSearchBox()
        {
        }

        public virtual void DisableSearchBox(UpOrDownArrowPressedCallback handleDownOrUpArrowKeyPressed, SearchBarTextChangeCallback handleSearchBarTextChanged, OnPressConfirmCallback handleOnConfirmPressed)
        {
            DisableSearchBox();
            OnSearchBarTextChange -= handleSearchBarTextChanged;
            OnUpOrDownArrowPressed -= handleDownOrUpArrowKeyPressed;
            OnPressConfirm -= handleOnConfirmPressed;
        }

        #endregion



    }

}