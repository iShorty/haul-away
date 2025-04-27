using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearEffects;

[System.Serializable]
public class SetIndicator_ScreenOffset_Executor : EffectExecutor<SetIndicator_ScreenOffset_Executor.MyEffect>
{
    [System.Serializable]
    public class MyEffect : Effect
    {
        public BaseUIIndicator Indicator = default;
        public Vector2 ScreenOffset = default;
    }

    protected override bool ExecuteEffect(MyEffect effectData)
    {
        effectData.Indicator.SetScreen_XOffset(effectData.ScreenOffset.x);
        effectData.Indicator.SetScreen_YOffset(effectData.ScreenOffset.y);
        return true;
    }
}
