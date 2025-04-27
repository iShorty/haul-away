using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<Summary>Any score based objective hook should inherit from this</Summary>
public abstract class BaseScoreBasedObjectiveEventHook : BaseObjectiveEventHook
{
    #if UNITY_EDITOR
    [SerializeField]
    [ReadOnly]
    #endif
    protected ScoreBasedObjectiveInfo _scoreInfo = default;

    public override void SetObjectiveInfo(BaseLevelObjectiveInfo info)
    {
        _scoreInfo = info as ScoreBasedObjectiveInfo;
    }

}
