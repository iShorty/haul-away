using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearEffects;

[System.Serializable]
public class SetIndicatorTarget_Executor : EffectExecutor<SetIndicatorTarget_Executor.MyEffect>
{
    [System.Serializable]
    public class MyEffect : Effect
    {
        public BaseUIIndicator Indicator = default;
        public Transform Target = default;
    }

    protected override bool ExecuteEffect(MyEffect effectData)
    {
        effectData.Indicator.Initialize(effectData.Target);
        return true;
    }
}
