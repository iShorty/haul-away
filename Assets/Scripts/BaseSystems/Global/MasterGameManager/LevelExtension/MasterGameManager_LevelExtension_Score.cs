using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class MasterGameManager
{
    #region Definition
    public delegate void IncrementScoreCallback(int newScore);
    #endregion

    #region Exposed Field

    #region Runtime
#if UNITY_EDITOR
    [SerializeField]
    [ReadOnly]
    [Header("----- Runtime -----")]
    [Header("===== Level Score =====")]
#endif
    int _currentCargoScore = 0;

    #endregion
    #endregion

    #region Hidden Field
    List<BaseObjectiveEventHook> _objectiveHookInstances = default;
    #endregion

    #region Properties
    public static int CurrentCargoScore => instance._currentCargoScore;
    #endregion

    #region Event
    public static event IncrementScoreCallback OnIncrementScore = null;
    #endregion




    #region Static Method
    public static void LevelExtension_Score_SendIncrementScore(int increment)
    {
        instance._currentCargoScore += increment;
        OnIncrementScore?.Invoke(instance._currentCargoScore);
    }


    #endregion


    private void LevelExtension_Score_OnGameAwake()
    {
        _objectiveHookInstances = new List<BaseObjectiveEventHook>();
        _currentCargoScore = 0;

        //Instantiate all of the objective hooks
        BaseLevelObjectiveInfo[] objInfos = Info.ObjectiveConditionInfos;

        for (int i = 0; i < objInfos.Length; i++)
        {
            BaseObjectiveEventHook hook = Instantiate(objInfos[i].SubscriberPrefab).GetComponent<BaseObjectiveEventHook>();
            hook.transform.SetParent(this.transform);
            hook.SetObjectiveInfo(objInfos[i]);
            hook.SetObjectiveIndex(i);
            _objectiveHookInstances.Add(hook);
        }

        // BaseObjectiveEventHook.OnStarConditionFulFilled += LevelExtension_Score_HandleStarConditionFulfilled;
    }

    // private void LevelExtension_Score_HandleStarConditionFulfilled(int obj)
    // {
    //     //If objective index goes past 2 (zerobased index counting)
    //     if (obj == 2)
    //     {
    //         //Endgame
    //         SendGameEnd();
    //     }
    // }

    private void LevelExtension_Score_OnDestroy()
    {
        // BaseObjectiveEventHook.OnStarConditionFulFilled -= LevelExtension_Score_HandleStarConditionFulfilled;

    }

    private void LevelExtension_Score_GameReset()
    {
        _currentCargoScore = 0;
    }
}
