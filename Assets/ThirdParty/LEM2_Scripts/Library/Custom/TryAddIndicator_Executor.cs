using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearEffects;

[System.Serializable]
public class TryAddIndicator_Executor : EffectExecutor<TryAddIndicator_Executor.MyEffect>
{
    [System.Serializable]
    public class MyEffect : Effect
    {
        public IndicatorInfo TypeOfIndicator = default;
        public Rigidbody Target = default;
    }

    protected override bool ExecuteEffect(MyEffect effectData)
    {
        UIIndicatorPool.GetIndicator(effectData.TypeOfIndicator, PlayerManager.PlayerCanvas.transform, effectData.Target);
        return true;
    }
}
