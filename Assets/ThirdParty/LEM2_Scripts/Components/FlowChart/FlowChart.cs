namespace LinearEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class FlowChart : BaseFlowChart
    {
        protected override void Awake()
        {
#if UNITY_EDITOR
            base.Awake();
#endif
            GameAwake();
        }

        private void Update()
        {
            GameUpdate();
        }


    }

}