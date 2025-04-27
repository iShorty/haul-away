namespace LinearEffects.DefaultEffects
{
    using LinearEffects;
    using UnityEngine;

    [DisallowMultipleComponent]
    ///<Summary>Waits for a block to stop playing before continuing the code flow on the selected flowchart. Must ensure that halt code flow boolean is set to true</Summary>
    public class WaitForThisBlock_Executor : UpdateEffectExecutor<WaitForThisBlock_Executor.MyEffect>
    {
        [System.Serializable]
        public class MyEffect : UpdateEffect
        {
            public string BlockName = default;
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
            BaseFlowChart flowChart = GetComponent<BaseFlowChart>();
            return !flowChart.CheckBlockIsPlaying(effectData.BlockName);
        }

    }
}