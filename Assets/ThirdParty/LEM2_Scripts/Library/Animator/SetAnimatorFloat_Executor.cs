namespace LinearEffects.DefaultEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    [System.Serializable]
    public class SetAnimatorFloat_Executor : EffectExecutor<SetAnimatorFloat_Executor.MyEffect>
    {
        [System.Serializable]
        public class MyEffect : Effect
        {
            public Animator Animator = default;
            public string Name = "ParameterName";
            public float Float = default;
        }

        protected override bool ExecuteEffect(MyEffect effectData)
        {
            effectData.Animator.SetFloat(effectData.Name, effectData.Float);
            return true;
        }


    }

}