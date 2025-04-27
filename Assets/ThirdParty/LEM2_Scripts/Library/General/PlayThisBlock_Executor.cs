namespace LinearEffects.DefaultEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    [System.Serializable]
    ///<Summary>Plays a block name from the flowchart which this executor is on</Summary>
    public class PlayThisBlock_Executor : EffectExecutor<PlayThisBlock_Executor.MyEffect>
    {
        [System.Serializable]
        public class MyEffect : Effect
        {
            public string BlockToPlay = default;
        }

        protected override bool ExecuteEffect(MyEffect effectData)
        {
            BaseFlowChart flowChart = GetComponent<BaseFlowChart>();
            flowChart.PlayBlock(effectData.BlockToPlay);
            return true;
        }
    }

}