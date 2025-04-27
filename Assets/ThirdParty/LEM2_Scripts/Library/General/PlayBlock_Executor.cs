namespace LinearEffects.DefaultEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    [System.Serializable]
    ///<Summary>Plays a block name from a selected flowchart</Summary>
    public class PlayBlock_Executor : EffectExecutor<PlayBlock_Executor.MyEffect>
    {
        [System.Serializable]
        public class MyEffect : Effect
        {
            [Header("----- Play Block -----")]
            public BaseFlowChart FlowChart = default;

            public string BlockToPlay = default;
        }


        protected override bool ExecuteEffect(MyEffect effectData)
        {
            effectData.FlowChart.PlayBlock(effectData.BlockToPlay);
            return true;
        }


    }

}