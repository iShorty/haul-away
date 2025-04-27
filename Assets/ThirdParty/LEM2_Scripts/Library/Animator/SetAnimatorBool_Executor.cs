namespace LinearEffects.DefaultEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    [System.Serializable]
    public class SetAnimatorBool_Executor : EffectExecutor<SetAnimatorBool_Executor.MyEffect>
    {
        [System.Serializable]
        public class MyEffect : Effect
        {
            public Animator Animator = default;
            public string Name = "ParameterName";
            public bool Bool = default;
        }

        protected override bool ExecuteEffect(MyEffect effectData)
        {
            effectData.Animator.SetBool(effectData.Name, effectData.Bool);
            return true;
        }


    }

}