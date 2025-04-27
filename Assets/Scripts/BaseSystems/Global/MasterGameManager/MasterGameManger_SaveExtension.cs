using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is a file which acts like an extension for the mastergamemanager to save the level when it's over.
public partial class MasterGameManager
{
    // #region Exposed Field
    // [field: Header("----- Level Save Data -----")]
    // [field: Header("===== Save Extension =====")]
    // [field: SerializeField, RenameField(nameof(SaveInfo))]
    // public LevelData SaveInfo { get; private set; } = null;

    // #endregion

    // #region Properties
    // public static LevelData CurrentSaveInfo => instance.SaveInfo;

    // #endregion


    ///<Summary>Subscribes methods in the MasterGameManager as the very first method to call in the GlobalEvents delegates </Summary>
    private void SaveExtension_GameAwake()
    {
        // #if UNITY_EDITOR
        //         Debug.Assert(SaveInfo, $"Mastergamemanger does not have a save info scriptable object!", this);
        // #endif
        //
        GlobalEvents.OnGameEnd += SaveExtension_GameEnd;
    }


    private void SaveExtension_OnDestroy()
    {
        GlobalEvents.OnGameEnd -= SaveExtension_GameEnd;
    }

    ///<Summary>The wrapper to store all save extension classes' gamend methods</Summary>
    private void SaveExtension_GameEnd()
    {
        bool[] levelStars = new bool[CurrentLevelInfo.ObjectiveConditionInfos.Length];

        //For every lvl objectives there are,
        for (int i = 0; i < levelStars.Length; i++)
        {
            levelStars[i] = _objectiveHookInstances[i].FulFilled;
#if UNITY_EDITOR
            Debug.Log($"The objective {_objectiveHookInstances[i].name} is fulfilled: {levelStars[i]}", _objectiveHookInstances[i]);
#endif
        }
        LevelData levelData = new LevelData(levelStars);
        SaveSystem.SaveLevel(levelData);
    }




}
