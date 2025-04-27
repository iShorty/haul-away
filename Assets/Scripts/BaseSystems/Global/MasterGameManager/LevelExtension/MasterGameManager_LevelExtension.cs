using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a file which acts like an extension for the mastergamemanager to subscribe to the global events themselves
//ill be using this as the parent handler for the level score and timer
// LevelExtension 
//  V       V
// timer   score
public partial class MasterGameManager
{
    #region Exposed Field
    [field: Header("----- Level Info -----")]
    [field: Header("===== Level Extension =====")]
    [field: SerializeField, RenameField(nameof(Info))]
    public LevelInfo Info { get; private set; } = null;

    #endregion

    #region Properties
    public static LevelInfo CurrentLevelInfo => instance.Info;

    #endregion



    #region Event Subscription
    ///<Summary>Subscribes methods in the MasterGameManager as the very first method to call in the GlobalEvents delegates </Summary>
    private void LevelExtension_GameAwake()
    {
#if UNITY_EDITOR
        Debug.Assert(Info, $"Mastergamemanger does not have a level info scriptable object!", this);
#endif
        GlobalEvents.OnGameReset += LevelExtension_GameReset;
        GlobalEvents.OnGameStart += LevelExtension_GameStart;
        GlobalEvents.OnGamePause += LevelExtension_GamePause;
        GlobalEvents.OnGameEnd += LevelExtension_GameEnd;
        GlobalEvents.OnGameResume += LevelExtension_GameResume;

        LevelExtension_Score_OnGameAwake();
        LevelExtension_Timer_OnGameAwake();
    }



    private void LevelExtension_OnDestroy()
    {
        GlobalEvents.OnGameReset -= LevelExtension_GameReset;
        GlobalEvents.OnGameStart -= LevelExtension_GameStart;
        GlobalEvents.OnGamePause -= LevelExtension_GamePause;
        GlobalEvents.OnGameEnd -= LevelExtension_GameEnd;
        GlobalEvents.OnGameResume -= LevelExtension_GameResume;

        LevelExtension_Timer_OnDestroy();

    }



    #region One-Frame Event
    ///<Summary>The wrapper to store all sub-level extension classes' gamestart methods</Summary>
    private void LevelExtension_GameStart()
    {
        LevelExtension_Timer_GameStart();
    }

    ///<Summary>The wrapper to store all sub-level extension classes' gamend methods</Summary>
    private void LevelExtension_GameEnd()
    {
        LevelExtension_Timer_GameEnd();
    }

    ///<Summary>The wrapper to store all sub-level extension classes' gamepause methods</Summary>
    private void LevelExtension_GamePause()
    {
        LevelExtension_Timer_GamePause();
    }

    ///<Summary>The wrapper to store all sub-level extension classes' gameresume methods</Summary>
    private void LevelExtension_GameResume()
    {
        LevelExtension_Timer_GameResume();
    }

    ///<Summary>The wrapper to store all sub-level extension classes' gamereset methods</Summary>
    private void LevelExtension_GameReset()
    {
        LevelExtension_Timer_GameReset();
        LevelExtension_Score_GameReset();
    }



    #endregion

    #endregion







}
