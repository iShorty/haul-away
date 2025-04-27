namespace LinearEffects.DefaultEffects
{
    using LinearEffects;
    using UnityEngine;

    [DisallowMultipleComponent]
    ///<Summary>Waits for a block to stop playing before continuing the code flow in the flowchart which is on the same gameobject as this one. Must ensure that halt code flow boolean is set to true</Summary>
    public class WaitForBlock_Executor : UpdateEffectExecutor<WaitForBlock_Executor.MyEffect>
    {
        [System.Serializable]
        public class MyEffect : UpdateEffect
        {
            public string BlockName = default;
            public BaseFlowChart FlowChart = default;
        }

        private void Awake()
        {
            //Ensures that effectdatas' halt until finish are all set to true
            for (int i = 0; i < _effectDatas.Length; i++)
            {
                _effectDatas[i].HaltUntilFinished = true;
            }
        }

        protected override bool ExecuteEffect(MyEffect effectData)
        {
            return !effectData.FlowChart.CheckBlockIsPlaying(effectData.BlockName);
        }

    }
}