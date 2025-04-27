using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using LinearEffects;
using System;

public partial class MenuCanvas : GenericManager<MenuCanvas>
{
    BaseFlowChart _flowchart = default;

    [Header("----- Runtime -----")]
#if UNITY_EDITOR
    [SerializeField, ReadOnly]
#endif
    MenuState _state = MenuState.INITIAL;

    MasterControls _masterControls = default;

    public static GameData GameDataFile { get; private set; } = default;

    public static bool IsReturningFromGame = false;

    #region Manager Methods
    protected override void OnGameAwake()
    {
        GameDataFile = SaveSystem.LoadGame();

        _state = MenuState.INITIAL;
        GlobalEvents.OnGameUpdate_DURINGGAME += DuringGameUpdate;
        _flowchart = GetComponent<BaseFlowChart>();
        _flowchart.GameAwake();
        Menu_GameAwake();
        LevelSelect_GameAwake();
        Credits_GameAwake();
        Settings_GameAwake();


        MenuState_SetState(MenuState.MENU);
        if (IsReturningFromGame)
        {
            IsReturningFromGame = false;
            MenuState_SetState(MenuState.LEVEL_SELECT);
        }
    }

    private void Start()
    {
        Settings_Start();
    }


    public override void OnDestroy()
    {
        GlobalEvents.OnGameUpdate_DURINGGAME -= DuringGameUpdate;
        Menu_GameDestroy();
        LevelSelect_GameDestroy();
    }



    private void DuringGameUpdate()
    {
        _flowchart.GameUpdate();
        switch (_state)
        {
            case MenuState.LEVEL_SELECT:
                LevelSelect_GameUpdate();
                break;
        }
    }
    #endregion

    #region Useful Methods

    ///<Summary>Toggles an array of buttons' navigation status to either none or automatic. True = automatic and false = none</Summary>
    void ToggleButton_Navigation(bool state, params Selectable[] buttons)
    {
        Navigation navMode = _playButton.navigation;
        navMode.mode = state ? Navigation.Mode.Automatic : Navigation.Mode.None;

        foreach (var item in buttons)
        {
            item.navigation = navMode;
            item.interactable = state;
        }
    }

    ///<Summary>Used by unity event in LEM to toggle whether or not toggle joining is available when character select panel is shown/hidden</Summary>
    public void Toggle_AllowPlayerJoin(bool state)
    {
        GlobalPlayerInputManager.TogglePlayerInputManagerJoining(state);
    }
    #endregion

}
