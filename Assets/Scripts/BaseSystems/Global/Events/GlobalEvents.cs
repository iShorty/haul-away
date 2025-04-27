using System.Collections.Generic;

///<Summary>Global events stores all the events which can be subscribed to by basemanager class or any other scripts (however, the latter will not have their methods called in the order they may prefer) For now, (or well for the rest of the project,) the menu scene uses this file to subscribe to Update generic loop also</Summary>
public static class GlobalEvents
{

    #region Delegate Definitions
    //Here are all the kinds of delegates we would be using 
    public delegate void GenericEvent();
    public delegate void TransitionSceneCallback(string s1, string s2);


    #endregion


    #region One-Frame Events
    #region Scene Events
    public static event GenericEvent OnSceneEnter = null;

    /// <summary>Is called by the Transition Manager after the loading of a new scene and unloading of the current scene. Really usefuly when be subscribed to by GlobalSingleton classes.</summary>
    public static void SendEnterScene()
    {
        OnSceneEnter?.Invoke();
    }

    /// <summary>Subscribed to by usually singletons which persists across scenes. eg. AudioManager when loading music</summary>
    public static event TransitionSceneCallback OnSceneTransition = null;

    /// <summary>Is called by the Transition Manager after the loading of a new scene and unloading of the current scene</summary>
    public static void SendTransitionScene(string prevSceneName, string newSceneName)
    {
        OnSceneTransition?.Invoke(prevSceneName, newSceneName);
    }

    /// <summary>Is called by the Transition Manager before the loading a new scene and unloading of the current scene</summary>
    public static event GenericEvent OnExitScene = null;

    public static void SendExitScene()
    {
        OnExitScene?.Invoke();
    }
    #endregion

    #region Game Events
    /// <summary>Is called by the MasterGameManager after Awake and BeforeGameStart update are called and finished (for example maybe there is a delay before the start the game, so the update loop will minus a timer before actual game starts)</summary>
    public static event GenericEvent OnGameStart = null;

    public static void SendStartGame()
    {
        OnGameStart?.Invoke();
    }

    /// <summary>Is called by the MasterGameManager when the game ends either via a timer or something else</summary>
    public static event GenericEvent OnGameEnd = null;

    public static void SendEndGame()
    {
        OnGameEnd?.Invoke();
    }

    /// <summary>Is called when a pause button or key binding is pressed. You will have to set your own source of this event caller</summary>
    public static event GenericEvent OnGamePause = null;

    public static void SendPauseGame()
    {
        OnGamePause?.Invoke();
    }

    /// <summary>Is called when a resume button or key binding is pressed. You will have to set your own source of this event caller</summary>
    public static event GenericEvent OnGameResume = null;

    public static void SendResumeGame()
    {
        OnGameResume?.Invoke();
    }



    /// <summary>Is called when a reset button or key binding is pressed. You will have to set your own source of this event caller</summary>
    public static event GenericEvent OnGameReset = null;

    public static void SendResetGame()
    {
        OnGameReset?.Invoke();
    }

    /// <summary>Is called when a quit button or key binding is pressed. You will have to set your own source of this event caller</summary>
    public static event GenericEvent OnGameQuit = null;

    public static void SendQuitGame()
    {
        OnGameQuit?.Invoke();
    }
    #endregion
    #endregion

    #region Update Events
    #region Game Events
    //================== BEFORE GAME START =================
    public static event GenericEvent OnGameUpdate_BEFOREGAMESTART = null;

    public static void SendGameUpdate_BEFOREGAMESTART()
    {
        OnGameUpdate_BEFOREGAMESTART?.Invoke();
    }

    public static event GenericEvent OnGameFixedUpdate_BEFOREGAMESTART = null;

    public static void SendGameFixedUpdate_BEFOREGAMESTART()
    {
        OnGameFixedUpdate_BEFOREGAMESTART?.Invoke();
    }

    public static event GenericEvent OnGameLateUpdate_BEFOREGAMESTART = null;

    public static void SendGameLateUpdate_BEFOREGAMESTART()
    {
        OnGameLateUpdate_BEFOREGAMESTART?.Invoke();
    }


    //================== DURING GAME =================
    public static event GenericEvent OnGameUpdate_DURINGGAME = null;

    public static void SendGameUpdate_DURINGGAME()
    {
        OnGameUpdate_DURINGGAME?.Invoke();
    }

    public static event GenericEvent OnGameFixedUpdate_DURINGGAME = null;

    public static void SendGameFixedUpdate_DURINGGAME()
    {
        OnGameFixedUpdate_DURINGGAME?.Invoke();
    }

    public static event GenericEvent OnGameLateUpdate_DURINGGAME = null;

    public static void SendGameLateUpdate_DURINGGAME()
    {
        OnGameLateUpdate_DURINGGAME?.Invoke();
    }


    //================ PAUSED GAME ================
    public static event GenericEvent OnGameUpdate_PAUSEDGAME = null;

    public static void SendGameUpdate_PAUSEDGAME()
    {
        OnGameUpdate_PAUSEDGAME?.Invoke();
    }

    public static event GenericEvent OnGameFixedUpdate_PAUSEDGAME = null;

    public static void SendGameFixedUpdate_PAUSEDGAME()
    {
        OnGameFixedUpdate_PAUSEDGAME?.Invoke();
    }

    public static event GenericEvent OnGameLateUpdate_PAUSEDGAME = null;

    public static void SendGameLateUpdate_PAUSEDGAME()
    {
        OnGameLateUpdate_PAUSEDGAME?.Invoke();
    }


    //================ AFTER GAME END ================
    public static event GenericEvent OnGameUpdate_GAMEEND = null;

    public static void SendGameUpdate_GAMEEND()
    {
        OnGameUpdate_GAMEEND?.Invoke();
    }

    public static event GenericEvent OnGameFixedUpdate_GAMEEND = null;

    public static void SendGameFixedUpdate_GAMEEND()
    {
        OnGameFixedUpdate_GAMEEND?.Invoke();
    }

    public static event GenericEvent OnGameLateUpdate_GAMEEND = null;

    public static void SendGameLateUpdate_GAMEEND()
    {
        OnGameLateUpdate_GAMEEND?.Invoke();
    }
    #endregion

    #endregion

}



