using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.PlayerInput;
using TMPro;
using UnityEngine.InputSystem;
using AudioManagement;
using System;

//Handles the level select movable low poly boat
public partial class MenuCanvas
{
    #region Const
    const string BLOCKNAME_LEAVE_LVLSELECT_MENU = "LevelSelect_Leave_To_Menu";
    const string BLOCKNAME_LVLSELECT_HIDE_UIELEMENTS = "LevelSelect_Hide_UI";
    const string BLOCKNAME_LVLSELECT_SHOW_UIELEMENTS = "LevelSelect_Show_UI";
    const string BLOCKNAME_LVLSELECT_PULLUP_PANEL = "LevelSelect_PullUpPanel";
    const string BLOCKNAME_LVLSELECT_PULLDOWN_PANEL = "LevelSelect_PullDownPanel";
    #endregion

    [Header("===== Level Select =====")]
    [SerializeField]
    MenuBoatController _boatController = default;

    [SerializeField]
    Camera _menuCamera = default;

    [Header("----- Level Panel -----")]
    [SerializeField]
    GameObject _levelLockedUI = default;

    [SerializeField]
    TextMeshProUGUI _levelLockedText = default;

    [SerializeField]
    TextMeshProUGUI _levelTitle = default;

    [SerializeField]
    Image _lvlScreenShot = default;

    [SerializeField]
    TextMeshProUGUI[] _levelObjectives = default;

    [SerializeField]
    Image[] _starImages = default;

    [Header("----- Sprite References -----")]
    [SerializeField]
    Sprite _filledStar = default;
    [SerializeField]
    Sprite _unfilledStar = default;


    //Runtime
    LevelSelectTrigger _currTrigger = default;

    LevelInfo _prevLoadedLevel = default;

    bool _currentlyLocked = false;

    public static Camera MenuCamera => instance._menuCamera;

    public static event Action LevelSelect_OnPanelUp = null;
    public static event Action LevelSelect_OnPanelDown = null;

    static event Action<InputAction.CallbackContext> LevelSelect_OnConfirm = null;
    static event Action<InputAction.CallbackContext> LevelSelect_OnCancel = null;

    void LevelSelect_GameAwake()
    {
        _masterControls = GlobalPlayerInputManager.MasterControls;
        MenuBoatController.OnEnterLevelTrigger += LevelSelect_HandleOnEnterLevelTrigger;
        MenuBoatController.OnExitLevelTrigger += LevelSelect_HandleOnExitLevelTrigger;

        for (int i = 0; i < _levelObjectives.Length; i++)
        {
            _levelObjectives[i].richText = true;
        }
    }



    void LevelSelect_GameDestroy()
    {
        MenuBoatController.OnEnterLevelTrigger -= LevelSelect_HandleOnEnterLevelTrigger;
        MenuBoatController.OnExitLevelTrigger -= LevelSelect_HandleOnExitLevelTrigger;

        LevelSelect_OnCancel = null;
        LevelSelect_OnConfirm = null;
        _masterControls.MainMenu.Confirm.performed -= LevelSelect_RaiseOnConfirm;
        _masterControls.MainMenu.Cancel.performed -= LevelSelect_RaiseOnCancel;

    }

    #region ----------------------- Proxy Event --------------------
    private void LevelSelect_RaiseOnConfirm(InputAction.CallbackContext obj)
    {
        LevelSelect_OnConfirm?.Invoke(obj);
    }

    private void LevelSelect_RaiseOnCancel(InputAction.CallbackContext obj)
    {
        LevelSelect_OnCancel?.Invoke(obj);
    }
    #endregion


    #region ------------------- Enter Level Trigger ------------------
    ///<Summary>This will handle entering loading in of text values into the ui text whenever a new levelselect trigger has been "ontriggerentered". This also subscribes to the Menu action map's confirm to pull up the level panel if player press confirm</Summary>
    private void LevelSelect_HandleOnEnterLevelTrigger(LevelSelectTrigger trigger)
    {
        //Trigger passed thru cannot be null cause theres is a check above
        //Dont flow down if ive already done the text updating for this level previously
        if (_currTrigger == trigger) return;

        //Subscribe on confirm button to Plays block where the level select panel pulls up
        LevelSelect_OnConfirm = null;
        LevelSelect_OnConfirm += LevelSelect_HandlePullUpLevelPanel;

        _currTrigger = trigger;

        //Populate the level select panel with all of the text first before player even decides to pull it up
        LevelSelect_PopulateLevelPanelUI();
    }

    void LevelSelect_PopulateLevelPanelUI()
    {
        //Dont load ui again if previous loaded levelinfo is the currenttrigger's lvl info. This will prevent loading the same level's ui multiple times
        if (_prevLoadedLevel == _currTrigger.LevelIndicator.LevelInfo) return;

        #region  -------------- Level title -------------------
        _prevLoadedLevel = _currTrigger.LevelIndicator.LevelInfo;
        _levelTitle.text = _prevLoadedLevel.LevelName;
        _lvlScreenShot.sprite = _prevLoadedLevel.LevelScreenShot;

        //For now ill hook this to a true val
        // LevelData level = GameDataFile.levelDats.Find(x => x.levelID == _prevLoadedLevel.SceneName);
        _currentlyLocked = GameDataFile.TotalStarCount < _prevLoadedLevel.StarUnlock;
        _levelLockedText.text = $"Only staff rank of {_prevLoadedLevel.StarUnlock} <sprite index=[2]> or above can access this assignment";
        _levelLockedUI.SetActive(_currentlyLocked);
        #endregion

        #region -------------- Objective texts and Star status -------------------
        BaseLevelObjectiveInfo[] textInfos = _prevLoadedLevel.ObjectiveConditionInfos;
#if UNITY_EDITOR
        // Debug.Assert(textInfos.Length == _levelObjectives.Length, $"Objective Condition infos on {_prevLoadedLevel.name} must be the same length as the number of assigned {nameof(_levelObjectives)} texts! {_prevLoadedLevel.name} length: {textInfos.Length} \n {this.name} length: {_levelObjectives.Length}", _prevLoadedLevel);
        // Debug.Assert(textInfos.Length == _starImages.Length, "There must be the same amount of textinfos and starimages assigned!", this);
#endif

        #endregion

        #region ---------------- Objective  -------------------

        //Get data from save manager
        LevelData lvlData = GameDataFile.levelDats.Find(x => x.levelID == _prevLoadedLevel.SceneName);

        //If lvl data is null
        if (ReferenceEquals(lvlData, null))
        {
            // Debug.Log("nul");
            // lvlData = new LevelData();
            GameDataFile.levelDats.Add(lvlData);
        }


        bool[] starStatusTemp = lvlData.stars;

        for (int i = 0; i < _starImages.Length; i++)
        {
            _starImages[i].gameObject.SetActive(false);
            _levelObjectives[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < starStatusTemp.Length; i++)
        {
            var status = starStatusTemp[i];
            _starImages[i].sprite = status ? _filledStar : _unfilledStar;

            string textValue = string.Format(textInfos[i].GetLevelSelectText());
            _levelObjectives[i].fontStyle = status ? FontStyles.Strikethrough : FontStyles.Normal;
            _levelObjectives[i].text = textValue;
            _levelObjectives[i].gameObject.SetActive(true);
            _starImages[i].gameObject.SetActive(true);
        }

        #endregion

    }


    private void LevelSelect_HandleOnExitLevelTrigger()
    {
        //Unsub on confirm button
        _currTrigger = null;
        LevelSelect_OnConfirm = null;
        // Debug.Log("Im out");
    }


    #region ---------------- Pull Up Panel Methods --------------------
    ///<Summary>This is the delegate subscribed to MainMenu action map's confirm performed event. This will play a block which will pull the panel up and handle the change in subscription change after the panel is up </Summary>
    private void LevelSelect_HandlePullUpLevelPanel(InputAction.CallbackContext obj)
    {
        if (ReferenceEquals(_currTrigger, null))
        {
            //TO combat a bug where events can be unsubbed when player leaves on trigger
            // Debug.Log("Bug found");
            LevelSelect_OnConfirm = null;
            return;
        }

        LevelSelect_OnConfirm = null;
        LevelSelect_OnCancel = null;
        // _masterControls.MainMenu.Confirm.performed -= LevelSelect_HandlePullUpLevelPanel;
        // _masterControls.MainMenu.Cancel.performed -= LevelSelect_HandleReturnToMenu;
        //Play block here
        _flowchart.PlayBlock(BLOCKNAME_LVLSELECT_PULLUP_PANEL);
        LevelSelect_OnPanelUp?.Invoke();
        // Debug.Log("Play pull up panel block here");
    }

    ///<Summary>This handles the changing of menu action maps' confirm and cancel event subscriptions. This method will be called via LEM's unity event call function after the panel has been pulled up or down </Summary>
    public void LevelSelect_HandleSubscriptionsChange(bool pullingUp)
    {
        if (pullingUp)
        {
            //Remove and repalce this handler with a handler that loads the selected lvl scene
            if (!_currentlyLocked)
            {
                LevelSelect_OnConfirm += LevelSelect_HandleLoadScene;
                // _masterControls.MainMenu.Confirm.performed += LevelSelect_HandleLoadScene;
            }
            LevelSelect_OnCancel += LevelSelect_HandlePullDownLevelPanel;
            // _masterControls.MainMenu.Cancel.performed += LevelSelect_HandlePullDownLevelPanel;
        }
        else
        {
            LevelSelect_OnCancel += LevelSelect_HandleReturnToMenu;
            LevelSelect_OnConfirm += LevelSelect_HandlePullUpLevelPanel;
            // _masterControls.MainMenu.Cancel.performed += LevelSelect_HandleReturnToMenu;
            // _masterControls.MainMenu.Confirm.performed += LevelSelect_HandlePullUpLevelPanel;
        }
    }

    ///<Summary>This gets called when MainMenu's action map's cancelled button is pressed. This will pull down the LevelPanel via LEM's block.This needs calling of the subscription change method with its bool value set to false after the LEM has finished playing the pulling down of level panel ui </Summary>
    private void LevelSelect_HandlePullDownLevelPanel(InputAction.CallbackContext obj)
    {
        // _masterControls.MainMenu.Confirm.performed -= LevelSelect_HandleLoadScene;
        // _masterControls.MainMenu.Cancel.performed -= LevelSelect_HandlePullDownLevelPanel;

        LevelSelect_OnCancel = null;
        LevelSelect_OnConfirm = null;

        //Play block here
        // Debug.Log("Play pull down panel block here");
        LevelSelect_OnPanelDown?.Invoke();
        _flowchart.PlayBlock(BLOCKNAME_LVLSELECT_PULLDOWN_PANEL);

    }

    private void LevelSelect_HandleLoadScene(InputAction.CallbackContext obj)
    {
        AudioEvents.RaiseOnPlay2DSFX(AudioClipType.SFX_LevelSelected, true);

        LevelSelect_OnCancel = null;
        LevelSelect_OnConfirm = null;

        // _masterControls.MainMenu.Confirm.performed -= LevelSelect_HandleLoadScene;
        // _masterControls.MainMenu.Cancel.performed -= LevelSelect_HandlePullDownLevelPanel;
        LevelInfo lvlInfo = _currTrigger.LevelIndicator.LevelInfo;
        TransitionManager.LoadScene(lvlInfo);
    }

    #endregion





    #endregion

    #region ---------------- State -------------------------
    private void LevelSelect_EnterState(MenuState prevState)
    {
        //If i was from menu,
        if (prevState == MenuState.MENU)
        {
            //Subscribe setting of set camera rotation and position
            _flowchart.PlayBlock(BLOCKNAME_MENU_HIDE_UIELEMENTS);
            TransitionManager.SubscribeToOnFade_1_To_0_end(LevelSelect_HandleOnFadeEnd);

            //transition black sccreen
            TransitionManager.PlayGlobalFlowChart(TransitionManager.TransitionTrigger.FADE_0_TO_1_TO_0,true);
        }
        else
        {
            _flowchart.PlayBlock(BLOCKNAME_LVLSELECT_SHOW_UIELEMENTS);
        }

        _masterControls.MainMenu.Confirm.performed += LevelSelect_RaiseOnConfirm;
        _masterControls.MainMenu.Cancel.performed += LevelSelect_RaiseOnCancel;
        LevelSelect_OnCancel += LevelSelect_HandleReturnToMenu;

    }


    ///<Summary>This handles what happens when the transitionamanger's flow chart has faded from 0 to 1 when user enters from menu to lvl select </Summary>
    private void LevelSelect_HandleOnFadeEnd()
    {
        //Hide menu ui
        _flowchart.PlayBlock(BLOCKNAME_LEAVEMENU_TO_LEVELSELECT);
        TransitionManager.UnSubscribeToOnFade_1_To_0_end(LevelSelect_HandleOnFadeEnd);
    }


    ///<Summary>This handles the confirm button's perform event to return the player to the main menu </Summary>
    private void LevelSelect_HandleReturnToMenu(InputAction.CallbackContext obj)
    {
        if (TransitionManager.IsTransitionPlaying(TransitionManager.TransitionTrigger.FADE_0_TO_1_TO_0)) return;

        MenuState_SetState(MenuState.MENU);
    }

    private void LevelSelect_LeaveState()
    {
        _masterControls.MainMenu.Confirm.performed -= LevelSelect_RaiseOnConfirm;
        _masterControls.MainMenu.Cancel.performed -= LevelSelect_RaiseOnCancel;

        _currTrigger = null;
        _flowchart.PlayBlock(BLOCKNAME_LVLSELECT_HIDE_UIELEMENTS);

    }
    #endregion


    void LevelSelect_GameUpdate()
    {
        _boatController.GameUpdate();
    }
}
