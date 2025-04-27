namespace LinearEffects
{
    using UnityEngine;
    using System;
    [Serializable]
    public abstract class Effect { }

    [Serializable]
    public abstract class UpdateEffect : Effect
    {
        ///<Summary>If this is set to true, the effect will essentially act like a stopper. When the Block executes all the effects sequentially and reaches this UpdateEffect, it will going down and keep updating all of the current effects until this UpdateEffect has finished executing all of its code</Summary>
        [Tooltip("If this is set to true, the effect will essentially act like a stopper. When the Block executes all the effects sequentially and reaches this UpdateEffect, it will going down and keep updating all of the current effects until this UpdateEffect has finished executing all of its code")]
        [Header("====== UPDATE EFFECT =====")]
        public bool HaltUntilFinished = false;


        ///<Summary>This is a boolean which is used to determine whether or not an update effect has triggered a StartEffectExecutor method call on its Executor. Please dont touch</Summary>
        public bool FirstFrameCall { get; set; } = false;
    }

    [Serializable]
    public abstract class UpdateEffectWithRuntimeData<T> : UpdateEffect
     where T : class, new()
    {
        ///<Summary>This is the runtime data class which will be assigned during the first call of execution of the effect and removed on the last call of execution when the effect is completed</Summary>
       public T RuntimeData {get; set;} = default;
    }
}