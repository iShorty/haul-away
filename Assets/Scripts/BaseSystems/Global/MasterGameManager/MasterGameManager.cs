using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManagement;


public partial class MasterGameManager : BaseMasterManager<MasterGameManager>
{
    #region Definitions
    enum GameState
    {
        ///<Summary>MasterGameManager has not finished calling its Awake method yet. Nothing initialized</Summary>
        UNINITIALIZED = 0
        ,
        ///<Summary>MasterGameManager has finished calling its Awake method. All managers found have been initialized. MasterGameManager's update loop is now running on the assumption that the game has not started yet. There will be a countdown timer before OnGameStart is called</Summary>
        INITIALIZED = 1
        ,
        ///<Summary>MasterGameManager has finished counting down its timer. OnGameStart has been called and MasterGameManager's update loop is running for all subscribed update methods </Summary>
        RUNNING = 2
        ,
        ///<Summary>MasterGameManager is paused. MasterGameManager's update loop will still run to those who subscribe to the pause update loop events.</Summary>
        PAUSED = 3
        ,
        ///<Summary>MasterGameManager has finished counting down its timer. OnGameStart has been called and MasterGameManager's update loop is running for all subscribed update methods </Summary>
        GAMEENDED = 4
    }
    #endregion



    #region Hidden Fields

    GameState _prevState = GameState.UNINITIALIZED
    , _curState = GameState.UNINITIALIZED
    ;

    #endregion


    #region Initialization and Destruction
    protected override void GameAwake()
    {
        _curState = GameState.UNINITIALIZED;
        LevelExtension_GameAwake();
        SaveExtension_GameAwake();
        base.GameAwake();
        _curState = GameState.INITIALIZED;
    }

    protected override void OnDestroy()
    {
        LevelExtension_OnDestroy();
        SaveExtension_OnDestroy();
    }



    #endregion

    #region Updates Method

    protected override void Update()
    {
        switch (_curState)
        {
            //====== BEFORE GAME START UPDATE LOOP ======
            case GameState.INITIALIZED:
                GlobalEvents.SendGameUpdate_BEFOREGAMESTART();
                break;
            //====== GAME STARTED UPDATE LOOP ======
            case GameState.RUNNING:
                GlobalEvents.SendGameUpdate_DURINGGAME();
                break;
            //====== GAME PAUSED UPDATE LOOP ======
            case GameState.PAUSED:
                GlobalEvents.SendGameUpdate_PAUSEDGAME();
                break;
            //====== GAME ENDED UPDATE LOOP ======
            case GameState.GAMEENDED:
                GlobalEvents.SendGameUpdate_GAMEEND();
                break;

            //========= STATES THAT SHOULDNT BE POSSIBLE HERE ============
            //UNINITIALIZED
            default:
#if UNITY_EDITOR
                Debug.LogError($"Current state: {_curState} should not be possible!", this);
#endif
                break;

        }
    }

    protected override void FixedUpdate()
    {
        switch (_curState)
        {
            //====== BEFORE GAME START UPDATE LOOP ======
            case GameState.INITIALIZED:
                GlobalEvents.SendGameFixedUpdate_BEFOREGAMESTART();
                break;
            //====== GAME STARTED UPDATE LOOP ======
            case GameState.RUNNING:
                GlobalEvents.SendGameFixedUpdate_DURINGGAME();
                break;
            //====== GAME PAUSED UPDATE LOOP ======
            case GameState.PAUSED:
                GlobalEvents.SendGameFixedUpdate_PAUSEDGAME();
                break;
            //====== GAME ENDED UPDATE LOOP ======
            case GameState.GAMEENDED:
                GlobalEvents.SendGameFixedUpdate_GAMEEND();
                break;

            //========= STATES THAT SHOULDNT BE POSSIBLE HERE ============
            //UNINITIALIZED
            default:
#if UNITY_EDITOR
                Debug.LogError($"Current state: {_curState} should not be possible!", this);
#endif
                break;

        }
    }

    protected override void LateUpdate()
    {
        switch (_curState)
        {
            //====== BEFORE GAME START UPDATE LOOP ======
            case GameState.INITIALIZED:
                GlobalEvents.SendGameLateUpdate_BEFOREGAMESTART();
                break;
            //====== GAME STARTED UPDATE LOOP ======
            case GameState.RUNNING:
                GlobalEvents.SendGameLateUpdate_DURINGGAME();
                break;
            //====== GAME PAUSED UPDATE LOOP ======
            case GameState.PAUSED:
                GlobalEvents.SendGameLateUpdate_PAUSEDGAME();
                break;
            //====== GAME ENDED UPDATE LOOP ======
            case GameState.GAMEENDED:
                GlobalEvents.SendGameLateUpdate_GAMEEND();
                break;

            //========= STATES THAT SHOULDNT BE POSSIBLE HERE ============
            //UNINITIALIZED
            default:
#if UNITY_EDITOR
                Debug.LogError($"Current state: {_curState} should not be possible!", this);
#endif
                break;

        }
    }



    #endregion

    #region Send Events methods

    ///<Summary>Assign this method to a button or key binding to pause the master manager</Summary>
    public static void SendGamePause()
    {
#if UNITY_EDITOR
        if (instance._curState == GameState.PAUSED)
        {
            Debug.LogError($"Current Game State is {GameState.PAUSED} and you are transitioning to {GameState.PAUSED }. This should not happen!");
        }
#endif

        instance._prevState = instance._curState;
        instance._curState = GameState.PAUSED;

        AudioEvents.RaiseOnPlay2DSFX(AudioClipType.SFX_Pause, true);
        AudioEvents.RaiseOnPlayBGM(AudioClipType.BGM_LoadingScreen, BGMAudioPlayer.BGM_PlayType.FADEIN_LOOP);
        GlobalEvents.SendPauseGame();
    }

    ///<Summary>Assign this method to a button or key binding to resume the master manager</Summary>
    public static void SendGameResume()
    {
#if UNITY_EDITOR
        if (instance._curState != GameState.PAUSED)
        {
            Debug.LogError($"Current Game State is { instance._curState} and not PAUSED. You are transitioning to { instance._prevState}. This should not happen!");
        }
#endif

        instance._curState = instance._prevState;
        instance._prevState = GameState.PAUSED;

        AudioEvents.RaiseOnPlayBGM(CurrentLevelInfo.BGM, BGMAudioPlayer.BGM_PlayType.FADEIN_LOOP);

        GlobalEvents.SendResumeGame();
    }

    ///<Summary>Assign this method to a button or key binding to reset the master manager</Summary>
    public static void SendGameReset()
    {
        instance._prevState = GameState.UNINITIALIZED;
        instance._curState = GameState.INITIALIZED;

        GlobalEvents.SendResetGame();
    }

    ///<Summary>Assign this method to a button or key binding to reset the master manager</Summary>
    public static void SendGameQuit()
    {
        GlobalEvents.SendQuitGame();

        //Quit the application
#if UNITY_EDITOR
        Debug.Log("Game has been quit");
#else
        Application.Quit();
#endif
    }

    ///<Summary>Call this event to end the game and start the RUNNING update loop on the MasterGameManager</Summary>
    public static void SendGameStart()
    {
#if UNITY_EDITOR
        if (instance._curState != GameState.INITIALIZED)
        {
            Debug.LogError($"Current Game State is { instance._curState} and you are transitioning to {GameState.RUNNING}. This should not happen!");
        }
#endif

        instance._prevState = GameState.INITIALIZED;
        instance._curState = GameState.RUNNING;

        GlobalEvents.SendStartGame();
    }

    ///<Summary>Call this event to end the game and start the GAMEENDED update loop on the MasterGameManager</Summary>
    public static void SendGameEnd()
    {
#if UNITY_EDITOR
        if (instance._curState != GameState.RUNNING)
        {
            Debug.LogError($"Current Game State is { instance._curState} and you are transitioning to {GameState.GAMEENDED}. This should not happen!");
        }
#endif

        instance._prevState = instance._curState;
        instance._curState = GameState.GAMEENDED;

        GlobalEvents.SendEndGame();
    }



    #endregion




}
