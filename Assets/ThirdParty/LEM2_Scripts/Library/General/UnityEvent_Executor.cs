namespace LinearEffects.DefaultEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    [System.Serializable]
    ///<Summary>Invokes a unity event</Summary>
    public class UnityEvent_Executor : EffectExecutor<UnityEvent_Executor.MyEffect>
    {
        [System.Serializable]
        public class MyEffect : Effect
        {
            public UnityEvent EffectEvent = default;
        }

        protected override bool ExecuteEffect(MyEffect effectData)
        {
            effectData.EffectEvent?.Invoke();
            return true;
        }
    }

}