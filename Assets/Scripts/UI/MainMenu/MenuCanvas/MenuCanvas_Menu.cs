using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using AudioManagement;

//Menu will handle the rigging of controllers and then spawning the characters in the respective join positions 
public partial class MenuCanvas
{
    #region Const
    const string BLOCKNAME_MENU_SHOW_UIELEMENTS = "Menu_Show_UI";
    const string BLOCKNAME_MENU_HIDE_UIELEMENTS = "Menu_Hide_UI";
    const string BLOCKNAME_LEAVEMENU_TO_LEVELSELECT = "Menu_Leave_To_LevelSelect";
    const string BLOCKNAME_SHOWJOINGAMEUI = "Show_JoinGameUI";
    const string BLOCKNAME_HIDEJOINGAMEUI = "Hide_JoinGameUI";

    // ///<Summary>Enters the menu world (that means doing fade in out transition and then setting the menu environment gameobject active)</Summary>
    // const string BLOCKNAME_ENTERMENU_WORLD = "EnterMenuWorld";
    // ///<Summary>Leaves the menu world (that means doing fade in out transition and then setting the menu environment gameobject inactive)</Summary>
    // const string BLOCKNAME_LEAVEMENU_WORLD = "LeaveMenuWorld";
    #endregion

    [Header("----- Listeners -----")]
    [Header("===== Menu =====")]
    [SerializeField]
    Button _playButton = default;
    [SerializeField]
    Button _settingsButton = default
    , _creditsButton = default
    , _quitButton = default
    ;

    [Header("----- Join Positions -----")]
    [SerializeField]
    PlayerJoinPosition[] _joinPositions = default;

    [SerializeField]
    PlayerModelInfo _modelInfo = default;

    [Header("----- Box Sprites -----")]
    [SerializeField]
    SpriteRenderer[] _deviceSR = default;

    [SerializeField]
    DeviceSpriteInfo _deviceSpriteInfo = default;

    [SerializeField]
    LevelSelectTrigger _tutorialLevelSelect = default;

    #region Awake and Destroy
    void Menu_GameAwake()
    {
        _playButton.onClick.RemoveAllListeners();
        _settingsButton.onClick.RemoveAllListeners();
        _creditsButton.onClick.RemoveAllListeners();
        _quitButton.onClick.RemoveAllListeners();

        _playButton.onClick.AddListener(Menu_OnClick_PlayButton);
        _settingsButton.onClick.AddListener(Menu_OnClick_SettingsButton);
        _creditsButton.onClick.AddListener(Menu_OnClick_CreditsButton);
        _quitButton.onClick.AddListener(Menu_OnClick_QuitButton);

        GlobalPlayerInputManager.OnPlayerJoined += Menu_HandlePlayerJoin;
        // GlobalPlayerInputManager.OnPlayerLeft += Menu_HandlePlayerLeft;

#if UNITY_EDITOR
        Debug.Assert(_joinPositions.Length > 0, "Forgot to assign join positions!", this);
        Debug.Assert(_deviceSR.Length > 0, "Forgot to assign box spries!", this);
        Debug.Assert(_tutorialLevelSelect != null, $"Forgot to assign {nameof(_tutorialLevelSelect)}!", this);
#endif



        //If players are returning to main menu from a game,
        for (int i = 0; i < GlobalPlayerInputManager.PlayerInputCount; i++)
        {
            PlayerInput p = GlobalPlayerInputManager.GetPlayerInput(i);

            _joinPositions[i].ChangeModel(i, _modelInfo);
            _deviceSR[i].sprite = _deviceSpriteInfo.GetDeviceSprite(p.devices[0].displayName);
        }
    }

    void Menu_GameDestroy()
    {
        GlobalPlayerInputManager.OnPlayerJoined -= Menu_HandlePlayerJoin;
        // GlobalPlayerInputManager.OnPlayerLeft -= Menu_HandlePlayerLeft;
    }
    #endregion


    private void Menu_LeaveState()
    {
        ToggleButton_Navigation(false, _playButton, _settingsButton, _creditsButton, _quitButton);
        Toggle_AllowPlayerJoin(false);
    }

    private void Menu_EnterState(MenuState prevState)
    {
        switch (prevState)
        {
            case MenuState.LEVEL_SELECT:
                TransitionManager.SubscribeToOnFade_1_To_0_end(Menu_HandleOnFadeEnd);

                //If it is assumed that menu is going back to menu scene
                TransitionManager.PlayGlobalFlowChart(TransitionManager.TransitionTrigger.FADE_0_TO_1_TO_0,true);
                // _flowchart.PlayBlock(BLOCKNAME_ENTERMENU_WORLD);
                // _flowchart.PlayBlock(BLOCKNAME_LEAVE_LVLSELECT_MENU);
                break;

            case MenuState.INITIAL:
                // TransitionManager.GlobalFlowChart.PlayBlock(BLOCKNAME_FADEBLACK_1_TO_0);
                TransitionManager.PlayGlobalFlowChart(TransitionManager.TransitionTrigger.FADE_1_TO_0,true);
                _flowchart.PlayBlock(BLOCKNAME_MENU_SHOW_UIELEMENTS);
                break;
        }

        // _flowchart.PlayBlock(BLOCKNAME_ENTERMENU_UIELEMENTS);
        ToggleButton_Navigation(true, _playButton, _settingsButton, _creditsButton, _quitButton);
        //Set player join to true
        Toggle_AllowPlayerJoin(true);
        _playButton.Select();
    }
    ///<Summary>This handles what happens when the transitionamanger's flow chart has faded from 0 to 1 when user enters from lvlselect to menu</Summary>
    private void Menu_HandleOnFadeEnd()
    {
        //Hide menu ui
        _flowchart.PlayBlock(BLOCKNAME_LEAVE_LVLSELECT_MENU);
        TransitionManager.UnSubscribeToOnFade_1_To_0_end(Menu_HandleOnFadeEnd);
    }

    #region Listener Events
    private void Menu_OnClick_QuitButton()
    {
        AudioEvents.RaiseOnPlay2DSFX(AudioClipType.SFX_ButtonClick, true);
#if UNITY_EDITOR
        Debug.Log("Quit!");
#else
        Application.Quit();
#endif
    }

    private void Menu_OnClick_CreditsButton()
    {
        AudioEvents.RaiseOnPlay2DSFX(AudioClipType.SFX_ButtonClick, true);
        MenuState_SetState(MenuState.CREDITS);
    }

    private void Menu_OnClick_SettingsButton()
    {
        AudioEvents.RaiseOnPlay2DSFX(AudioClipType.SFX_ButtonClick, true);
        MenuState_SetState(MenuState.SETTINGS);
    }

    private void Menu_OnClick_PlayButton()
    {

        //Else, enter the level select screen
        AudioEvents.RaiseOnPlay2DSFX(AudioClipType.SFX_ButtonClick, true);

        if (GlobalPlayerInputManager.PlayerInputCount > 0)
        {
            //Check if this is a new game using save system (assuming)
            //if is new game load tutorial level straight away
            if (GameDataFile.TotalStarCount == 0)
            {
#if UNITY_EDITOR
                Debug.Log("Loading tutorial!");
#endif

                _currTrigger = _tutorialLevelSelect;
                LevelSelect_HandleLoadScene(default);
            }


            MenuState_SetState(MenuState.LEVEL_SELECT);
        }
        else
        {
            //Else pull up the panel that asks you to press triangle to join
            ToggleButton_Navigation(false, _playButton, _settingsButton, _creditsButton, _quitButton);
            _flowchart.PlayBlock(BLOCKNAME_SHOWJOINGAMEUI);
            _masterControls.MainMenu.Join.performed += Menu_HideJoinGameUI;
        }
    }

    private void Menu_HideJoinGameUI(InputAction.CallbackContext obj)
    {
        _masterControls.MainMenu.Join.performed -= Menu_HideJoinGameUI;
        _flowchart.PlayBlock(BLOCKNAME_HIDEJOINGAMEUI);
        ToggleButton_Navigation(true, _playButton, _settingsButton, _creditsButton, _quitButton);
        _playButton.Select();
    }
    #endregion

    ///<Summary>Will handle when a player's controller joins by pressing the join button (space for pc and north button for controllers)</Summary>
    private void Menu_HandlePlayerJoin(PlayerInput obj, int newPlayerCount)
    {
        //Update the world's sprite ui by:
        //replacing the respective sprite based on player index with the correct device sprite

        //Spawning the character model at its spawn position
        int playerIndex = newPlayerCount - 1;
        PlayerInput p = GlobalPlayerInputManager.GetPlayerInput(playerIndex);
        _joinPositions[playerIndex].ChangeModel(playerIndex, _modelInfo);
        _deviceSR[playerIndex].sprite = _deviceSpriteInfo.GetDeviceSprite(p.devices[0].displayName);

    }

    // ///<Summary>Will handle when a player's controller leave by removing the playerinput's playerindex's sprite (or replace it with a disconnect sprite)</Summary>
    // private void Menu_HandlePlayerLeft(PlayerInput obj)
    // {

    // }

}
