using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySeaMinesObjectiveEventHook : BaseScoreBasedObjectiveEventHook
{

    int _count = 0;
    protected override void Awake()
    {
        base.Awake();
        SeaMine.OnSeaMineExplode += HandleOnSeaMineExplode;
        _count = 0;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }


    private void HandleOnSeaMineExplode(Collision obj)
    {
        if (FulFilled) return;

        PlayerCannonProjectile playerCannonProjectile = obj.collider.attachedRigidbody.GetComponent<PlayerCannonProjectile>();
        //If the collision that hit the seamine is indeed a player cannonball,
        if (!playerCannonProjectile)
        {
            return;
        }

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
