namespace LinearEffects.DefaultEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    [System.Serializable]
    public class SetAnimatorTrigger_Executor : EffectExecutor<SetAnimatorTrigger_Executor.MyEffect>
    {
        [System.Serializable]
        public class MyEffect : Effect
        {
            public Animator Animator = default;
            public string Trigger = default;
        }

        protected override bool ExecuteEffect(MyEffect effectData)
        {
            effectData.Animator.SetTrigger(effectData.Trigger);
            return true;
        }


    }

}