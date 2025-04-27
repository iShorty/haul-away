namespace LinearEffects.DefaultEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    [System.Serializable]
    ///<Summary>Tries to stop a block with the given name from the flowchart which this executor is on</Summary>
    public class TryStopThisBlock_Executor : EffectExecutor<TryStopThisBlock_Executor.MyEffect>
    {
        [System.Serializable]
        public class MyEffect : Effect
        {
            public string BlockToStop = default;
        }

        protected override bool ExecuteEffect(MyEffect effectData)
        {
            BaseFlowChart flowChart = GetComponent<BaseFlowChart>();
            flowChart.TryStopBlock(effectData.BlockToStop);
            return true;
        }
    }

}