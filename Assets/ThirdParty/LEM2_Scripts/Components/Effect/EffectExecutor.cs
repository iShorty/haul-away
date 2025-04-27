namespace LinearEffects
{
    using System.Collections.Generic;
    using UnityEngine;

    //The EffectExecutor is by default assumed to have an ExecuteEffect function which can be completed
    //in a single frame call
    ///<Summary>A base effectexecutor which will finish its effect in a single frame</Summary>
    public abstract partial class EffectExecutor<T> : BaseEffectExecutor
    where T : Effect, new()
    {
        //=============================FOR RUN TIME==============================
        [SerializeField]
        protected T[] _effectDatas = new T[0];

        ///<Summary>Returns a effect data instance at the given index</Summary>
        public T GetEffectData(int index) { return _effectDatas[index]; }

        ///<Summary>The method which will called every frame after StartExecuteEffect is called. Returns true if Effect has been completely finished executing.</Summary>
        protected abstract bool ExecuteEffect(T effectData);

        ///<Summary>Called by Block's EffectOrder during a block's execute effects call. Returns true when the effect being executed is complete</Summary>
        public override bool ExecuteEffectAtIndex(int index, out bool haltCodeFlow)
        {
#if UNITY_EDITOR
            Debug.Assert(index >= 0, $"Name of EffectExecutor is {this.GetType().ToString()} Index passed in is {index}");
#endif
            haltCodeFlow = false;
            return ExecuteEffect(_effectDatas[index]);
        }

        ///<Summary>Effects which do not take multiple frames to complete will not need to call EndEffectExecute because it is not defined for them</Summary>
        public override void StopEffectUpdate(int index)
        {
     //Effects which do not take multiple frames to complete will not need to call EndEffectExecute because it is not defined for them
        }

    }

}