using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;

public partial class GameUI
{
    [Header("----- Buttons -----")]
    [Header("===== Pause Screen =====")]
    [SerializeField]
    GameObject _pauseScreen = default;
    [SerializeField]
    Button _pauseRestartButton = default;
    [SerializeField]
    Button _resumeButton = default
    // , _settingsButton = default
    , _quitButton = default
    ;




    bool _isPaused = false;

    private void NotInGame_PauseScreen_GameAwake()
    {
        _pauseRestartButton.onClick.RemoveAllListeners();
        _pauseRestartButton.onClick.AddListener(Onclick_RestartButton);

        _resumeButton.onClick.RemoveAllListeners();
        _resumeButton.onClick.AddListener(() => Onclick_TogglePause(default));

        _quitButton.onClick.RemoveAllListeners();
        _quitButton.onClick.AddListener(Onclick_QuitButton);


        // _resumeButton.onClick.RemoveAllListeners();
        // _resumeButton.onClick.AddListener(Onclick_TogglePause);

    }



    // private void NotInGame_PauseScreen_OnEnable()
    // {
    //     GlobalEvents.OnGamePause += NotInGame_PauseScreen_GamePause;
    //     GlobalEvents.OnGameResume += NotInGame_PauseScreen_GameResume;

    // }

    // private void NotInGame_PauseScreen_OnDisable()
    // {
    //     GlobalEvents.OnGamePause -= NotInGame_PauseScreen_GamePause;
    //     GlobalEvents.OnGameResume -= NotInGame_PauseScreen_GameResume;
    // }



    private void NotInGame_PauseScreen_GameReset()
    {
        _pauseScreen.SetActive(false);
        _isPaused = false;
    }

    private void NotInGame_PauseScreen_GamePause()
    {
        UIExtensions.ToggleButtonNavigation(true, _pauseRestartButton, _resumeButton, _quitButton);
        // _resumeButton.gameObject.SetActive(true);
        _resumeButton.Select();
    }

    private void NotInGame_PauseScreen_GameResume()
    {
        // _pauseRestartButton.OnDeselect(default);
        // _resumeButton.OnDeselect(default);
        // // _resumeButton.Select();
        // _quitButton.OnDeselect(default);


        UIExtensions.ToggleButtonNavigation(false, _pauseRestartButton, _resumeButton, _quitButton);
    }

 
}
