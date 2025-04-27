namespace LinearEffects.DefaultEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    [System.Serializable]
    ///<Summary>Plays a block name from a selected flowchart</Summary>
    public class SetText_TMP_Executor : EffectExecutor<SetText_TMP_Executor.MyEffect>
    {
        [System.Serializable]
        public class MyEffect : Effect
        {
            [Header("----- Play Block -----")]
            public TMP_Text TextUI = default;

            [TextArea]
            public string TextToSet = default;
        }


        protected override bool ExecuteEffect(MyEffect effectData)
        {
            effectData.TextUI.text = effectData.TextToSet;
            return true;
        }


    }

}