namespace LinearEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public partial class BaseFlowChart
    {
        protected virtual void Awake() 
        {
            #if UNITY_EDITOR
            Editor_Awake();
            #endif
         }
    }

}