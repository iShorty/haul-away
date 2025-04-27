namespace LinearEffects.DefaultEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    [System.Serializable]
    public class SetAnimatorInt_Executor : EffectExecutor<SetAnimatorInt_Executor.MyEffect>
    {
        [System.Serializable]
        public class MyEffect : Effect
        {
            public Animator Animator = default;
            public string Name = "ParameterName";
            public int Int = default;
        }

        protected override bool ExecuteEffect(MyEffect effectData)
        {
            effectData.Animator.SetFloat(effectData.Name, effectData.Int);
            return true;
        }


    }

}