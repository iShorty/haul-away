namespace LinearEffects.DefaultEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    [System.Serializable]
    ///<Summary>Tries to stop a block with the given name from a selected flowchart </Summary>
    public class TryStopBlock_Executor : EffectExecutor<TryStopBlock_Executor.MyEffect>
    {
        [System.Serializable]
        public class MyEffect : Effect
        {
            public BaseFlowChart FlowChart = default;
            public string BlockToStop = default;
        }

        protected override bool ExecuteEffect(MyEffect effectData)
        {
            effectData.FlowChart.TryStopBlock(effectData.BlockToStop);
            return true;
        }
    }

}