using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.InputSystem;


//This script takes care of none ingame ui like pause screen and game over screen
public partial class GameUI
{
    // Keyboard _keyboard = default;

    void NotInGame_Awake()
    {
        // _keyboard = InputSystem.GetDevice<Keyboard>();

        NotInGame_GameOverScreen_GameAwake();
        NotInGame_PauseScreen_GameAwake();
        NotInGame_HandleOnGameReset();
    }

    void NotInGame_OnEnable()
    {
        GlobalEvents.OnGameEnd += NotInGame_HandleOnGameEnd;
        GlobalEvents.OnGameUpdate_PAUSEDGAME += NotInGame_HandleDuringandPausedGameUpdate;
        GlobalEvents.OnGameUpdate_DURINGGAME += NotInGame_HandleDuringandPausedGameUpdate;

        // NotInGame_PauseScreen_OnEnable();

    }



    void NotInGame_OnDisable()
    {
        GlobalEvents.OnGameEnd -= NotInGame_HandleOnGameEnd;
        GlobalEvents.OnGameUpdate_PAUSEDGAME -= NotInGame_HandleDuringandPausedGameUpdate;
        GlobalEvents.OnGameUpdate_DURINGGAME -= NotInGame_HandleDuringandPausedGameUpdate;

        // NotInGame_PauseScreen_OnDisable();


    }



    #region Handles


    private void NotInGame_HandleOnGameEnd()
    {
        //Hide the pause screens if they are somehow up
        NotInGame_HandleOnGameReset();
        NotInGame_GameOverScreen_GameEnd();
    }

    private void NotInGame_HandleOnGameReset()
    {
        NotInGame_GameOverScreen_GameReset();
        NotInGame_PauseScreen_GameReset();
    }

    private void NotInGame_HandleDuringandPausedGameUpdate()
    {
        // #if UNITY_EDITOR
        // if (_keyboard.escapeKey.wasPressedThisFrame)
        // {
        //     Onclick_TogglePause(default);
        // }
        // #endif
    }


    #endregion




}
