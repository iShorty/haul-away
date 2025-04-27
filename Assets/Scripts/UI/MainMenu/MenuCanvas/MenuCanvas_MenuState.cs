using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Just a file to dump all of the enum states + their descriptions
public partial class MenuCanvas
{
    #region State

    public enum MenuState
    {
        INITIAL = -1
        ,
        ///<Summary>The default menu state where players could either have either just opened the game or returned to menu</Summary>
        MENU = 0
    ,
    //     ///<Summary>If the player is playing the game for the first time, the character select screen will pop up after pressing Play for a new game. Players register their controllers and (for gold) choose their characters here. This screen can later be accessed in LevelSelect state</Summary>
    //     CHARACTER_SELECT = 1
    // ,
        ///<Summary>The state where players control a low poly boat to visit levels. If players are playing this game for the first time, this level select state will be skipped and tutorial level will be loaded immediately when players press Play</Summary>
        LEVEL_SELECT = 2
    ,
        ///<Summary>This can be accessed from the Menu state and will just pop up a panel to show all of the different options. Hide the menu's unseen buttons/ui elements when this is open</Summary>
        SETTINGS = 3
    ,
        ///<Summary>This can be accessed from the Menu state and prolly just load another scene but for now we pop up a ui panel to show the names and stuff. Hide menu's unseen buttons/ui elements when this is opened</Summary>
        CREDITS = 4

    }

    #endregion

    void MenuState_SetState(MenuState newState)
    {
        if (_state == newState) return;

        //previous state
        switch (_state)
        {
            case MenuState.MENU:
                Menu_LeaveState();
                break;
            // case MenuState.CHARACTER_SELECT:
                // CharacterSelect_LeaveState();
                // break;
            case MenuState.LEVEL_SELECT:
                LevelSelect_LeaveState();
                break;
            case MenuState.SETTINGS:
                Settings_LeaveState();
                break;
            case MenuState.CREDITS:
                Credits_LeaveState();
                break;

        }

        //previous state
        switch (newState)
        {
            case MenuState.MENU:
                Menu_EnterState(_state);
                break;
            // case MenuState.CHARACTER_SELECT:
                // CharacterSelect_EnterState(_state);
                // break;
            case MenuState.LEVEL_SELECT:
                LevelSelect_EnterState(_state);
                break;
            case MenuState.SETTINGS:
                Settings_EnterState(_state);
                break;
            case MenuState.CREDITS:
                Credits_EnterState(_state);
                break;

        }

        _state = newState;
    }

 
}
