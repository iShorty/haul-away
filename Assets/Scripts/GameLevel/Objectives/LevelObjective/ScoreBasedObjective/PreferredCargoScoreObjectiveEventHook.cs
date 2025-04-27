using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreferredCargoScoreObjectiveEventHook : BaseScoreBasedObjectiveEventHook
{
    int _count = 0;

    protected override void Awake()
    {
        base.Awake();
        Destination.OnCollectPreferredCargo += OnPreferredCargo_CompareNumberOfTimes;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Destination.OnCollectPreferredCargo -= OnPreferredCargo_CompareNumberOfTimes;
    }

    ///<Summary>Records and compares the number of times players have delivered a preferred cargo</Summary>
    private void OnPreferredCargo_CompareNumberOfTimes()
    {
        if (FulFilled) return;

        _count++;
        UpdateObjectiveText();
        if (_count >= _scoreInfo.ScoreToAchieve)
        {
            RaiseStarConditionFulFilled(this, _objectiveIndex);
        }
    }

    protected override void ResetEventHook()
    {
        _count = 0;
        base.ResetEventHook();
    }

    protected override void UpdateObjectiveText()
    {
        string s = string.Format(_scoreInfo.InGameTextFormat, _scoreInfo.ScoreToAchieve, _count);
        GameUI.InGameUI_UpdateObjectiveText(_objectiveIndex, s);
    }
}
