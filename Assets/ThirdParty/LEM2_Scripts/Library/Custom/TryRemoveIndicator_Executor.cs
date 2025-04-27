using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearEffects;

[System.Serializable]
public class TryRemoveIndicator_Executor : EffectExecutor<TryRemoveIndicator_Executor.MyEffect>
{
    [System.Serializable]
    public class MyEffect : Effect
    {
        public Rigidbody Target = default;
    }

    protected override bool ExecuteEffect(MyEffect effectData)
    {
        UIIndicatorPool.TryRemoveIndicator(effectData.Target);
        return true;
    }
}
