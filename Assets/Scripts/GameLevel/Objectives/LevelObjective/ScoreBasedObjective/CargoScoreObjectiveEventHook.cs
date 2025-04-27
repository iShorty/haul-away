using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoScoreObjectiveEventHook : BaseScoreBasedObjectiveEventHook
{
    protected override void Awake()
    {
        base.Awake();
        MasterGameManager.OnIncrementScore += HandleScoreIncrement;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        MasterGameManager.OnIncrementScore -= HandleScoreIncrement;
    }

    protected override void UpdateObjectiveText()
    {
        string s = string.Format(_scoreInfo.InGameTextFormat, _scoreInfo.ScoreToAchieve, MasterGameManager.CurrentCargoScore);
        GameUI.InGameUI_UpdateObjectiveText(_objectiveIndex, s);
    }

    private void HandleScoreIncrement(int newScore)
    {
        if (FulFilled) return;

        UpdateObjectiveText();

        //If new score has reached expectations
        if (newScore >= _scoreInfo.ScoreToAchieve)
        {
              #if UNITY_EDITOR
            Debug.Log($"Cargo score objective event hook is fulfilled! Target score: {_scoreInfo.ScoreToAchieve}, newScore: {newScore}",this);
            #endif
            RaiseStarConditionFulFilled(this, _objectiveIndex);
        }
    }
}
