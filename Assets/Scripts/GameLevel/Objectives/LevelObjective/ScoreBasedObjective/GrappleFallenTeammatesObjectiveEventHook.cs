using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleFallenTeammatesObjectiveEventHook : BaseScoreBasedObjectiveEventHook
{
    int _count = 0;

    protected override void Awake()
    {
        base.Awake();
        // PlayerManager.OnPlayerRespawn += RecordNumberOfDeaths;
        MultiUseStation.OnGrappleFireSuccess += CheckIfPlayerIsGrappled;
    }



    protected override void OnDestroy()
    {
        base.OnDestroy();
        MultiUseStation.OnGrappleFireSuccess -= CheckIfPlayerIsGrappled;
        // PlayerManager.OnPlayerRespawn -= RecordNumberOfDeaths;
    }

    private void CheckIfPlayerIsGrappled(IGrappleable grappled)
    {
        if (FulFilled) return;

        if(!grappled.Transform.GetComponent<PlayerController>())return;

        _count++;
        //Update ui
        UpdateObjectiveText();
        //if count meets the correct score to achieve, aise star condition fulfilled
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
