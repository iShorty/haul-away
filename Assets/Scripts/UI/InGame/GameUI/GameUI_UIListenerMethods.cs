using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using AudioManagement;
using UnityEngine.InputSystem;

public partial class GameUI
{
    public static void RaiseOnPause()
    {
        instance.Onclick_TogglePause(default);
    }

    void Onclick_TogglePause(InputAction.CallbackContext obj)
    {
        //Invert pause val
        _isPaused = !_isPaused;

        switch (_isPaused)
        {
            case true:
                //Only when game isnt paused,
                MasterGameManager.SendGamePause();
                _pauseScreen.SetActive(true);
                NotInGame_PauseScreen_GamePause();
                break;
            case false:
                MasterGameManager.SendGameResume();
                NotInGame_PauseScreen_GameResume();
                _pauseScreen.SetActive(false);
                break;
        }



        // Debug.Log($"Selected {_resumeButton.name}", _resumeButton);
    }

    private void Onclick_RestartButton()
    {
        AudioEvents.RaiseOnPlay2DSFX(AudioClipType.SFX_ButtonClick, true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // MasterGameManager.SendGameReset();
    }

    // void Onclick_TogglePause()
    // {
    //     //Invert pause val
    //     _isPaused = !_isPaused;

    //     if (!_isPaused)
    //     {
    //         MasterGameManager.SendGameResume();
    //     }
    //     else
    //     {
    //         //Only when game isnt paused,
    //         MasterGameManager.SendGamePause();
    //     }

    //     _pauseScreen.SetActive(_isPaused);
    // }

    private void Onclick_QuitButton()
    {
        AudioEvents.RaiseOnPlay2DSFX(AudioClipType.SFX_ButtonClick, true);
        TransitionManager.LoadScene(Constants.For_Scene.SCENENAME_MAINMENU);
    }

     private void Onclick_ContinueButton()
    {
        MenuCanvas.IsReturningFromGame = true;
        AudioEvents.RaiseOnPlay2DSFX(AudioClipType.SFX_ButtonClick, true);
        TransitionManager.LoadScene(Constants.For_Scene.SCENENAME_MAINMENU);
    }

}
