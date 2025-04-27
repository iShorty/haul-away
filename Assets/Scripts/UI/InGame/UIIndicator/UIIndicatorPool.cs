using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIndicatorPool : GenericPools<BaseUIIndicator, UIIndicatorPool>, IGlobalEventManager
{

    [Header("----- Manager Settings -----")]
    [SerializeField]
    private int _executionPriority;

    [SerializeField]
    IndicatorInfo _dangerInfo = default;

    #region  Properties
    static Dictionary<Rigidbody, BaseUIIndicator> activeIndicators { get; set; } = default;

    public int ExecutionOrder => _executionPriority;
    #endregion

    #region HiddenField
    List<BaseUIIndicator> _removalList = default;
    int _numberOfWarningSigns = 0;

    #endregion


    #region Static Method

    #region Get
    // public static BaseUIIndicator GetIndicator(IndicatorInfo info, Rigidbody target)
    // {
    //     BaseUIIndicator indicator = instance.GetInstance(info.Prefab.gameObject);
    //     indicator.Initialize(BoatManager.MainCamera, target.transform);
    //     // indicator.Initialize(BoatManager.MainCamera, BoatManager.Controller.transform, target.transform);


    //     TryRemoveIndicator(target);
    //     activeIndicators.Add(target, indicator);
    //     return indicator;
    // }

    public static BaseUIIndicator GetIndicator(IndicatorInfo info, Transform parent, Rigidbody target)
    {
        BaseUIIndicator indicator = instance.GetInstance(info.Prefab.gameObject, parent, Vector3.one);
        indicator.Initialize(BoatManager.MainCamera, target.transform);
        // indicator.Initialize(BoatManager.MainCamera, BoatManager.Controller.transform, target.transform);
        instance.TryAddWarningSign(info);

        TryRemoveIndicator(target);
        activeIndicators.Add(target, indicator);
        return indicator;
    }



    // public static BaseUIIndicator GetIndicator(IndicatorInfo info, Transform parent, Vector3 localPosition, Rigidbody target)
    // {
    //     BaseUIIndicator indicator = instance.GetInstance(info.Prefab.gameObject, parent, localPosition);
    //     indicator.Initialize(BoatManager.MainCamera, BoatManager.Controller.transform, target.transform);

    //     TryRemoveIndicator(target);
    //     activeIndicators.Add(target, indicator);
    //     return indicator;
    // }
    #endregion

    ///<Summary>Tries to remove an active indicator whose target is the passed in rigidbody</Summary>
    public static void TryRemoveIndicator(Rigidbody target)
    {
        //If there has been a warning indicator occuring b4 danger, remove warning firstt from the loop
        if (activeIndicators.ContainsKey(target))
        {
            // Debug.Log(target.name,target);
            BaseUIIndicator prevActive = activeIndicators[target];
            instance.TryMinusWarningSign(prevActive.Info);
            ReturnInstanceOf(prevActive.Info.Prefab, prevActive);
            activeIndicators.Remove(target);
        }
    }

    #endregion
    void TryMinusWarningSign(IndicatorInfo info)
    {
        if (info == instance._dangerInfo)
        {
            Debug.Log("Remove");
            //That means an enemy has died
            _numberOfWarningSigns--;
            if (_numberOfWarningSigns == 0)
            {
                AudioManagement.AudioEvents.RaiseOnPlayBGM(MasterGameManager.CurrentLevelInfo.BGM, AudioManagement.BGMAudioPlayer.BGM_PlayType.FADEIN_LOOP);
            }
        }
    }

    private void TryAddWarningSign(IndicatorInfo info)
    {
        if (info != _dangerInfo) return;

        //If this is the first time
        if (_numberOfWarningSigns == 0)
        {
            AudioManagement.AudioEvents.RaiseOnPlayBGM(AudioClipType.BGM_CombatMusic, AudioManagement.BGMAudioPlayer.BGM_PlayType.FADEIN_LOOP);
        }
        _numberOfWarningSigns++;
    }

    public void GameAwake()
    {
        GlobalEvents.OnGameUpdate_DURINGGAME += DuringGameUpdate;
        GlobalEvents.OnGameReset += HandleGameReset;

        activeIndicators = new Dictionary<Rigidbody, BaseUIIndicator>();
        _removalList = new List<BaseUIIndicator>();
    }



    public void OnDestroy()
    {
        GlobalEvents.OnGameUpdate_DURINGGAME -= DuringGameUpdate;
        GlobalEvents.OnGameReset -= HandleGameReset;

    }

    private void HandleGameReset()
    {
        //Remove all active cases of ui indicators
        foreach (var rb in activeIndicators.Keys)
        {
            TryRemoveIndicator(rb);
        }
        _removalList.Clear();
    }

    void DuringGameUpdate()
    {
        //Update the rest
        foreach (var item in activeIndicators.Values)
        {
            if (item.GameUpdate())
            {
                //Remove after all indicators are updated
                _removalList.Add(item);
            }
        }

        //Removed queued ui from current frame
        for (int i = 0; i < _removalList.Count; i++)
        {
            BaseUIIndicator ui = _removalList[i];
            //Enemy rb is found on the followtarget itself
            Rigidbody rb = ui.followingTarget.GetComponent<Rigidbody>();

            //Remove from both collection
            _removalList.RemoveEfficiently(i);
            TryRemoveIndicator(rb);
            // activeIndicators.Remove(rb);
            i--;
        }

    }

}


