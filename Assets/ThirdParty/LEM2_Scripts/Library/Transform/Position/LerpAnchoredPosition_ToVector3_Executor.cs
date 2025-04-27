namespace LinearEffects.DefaultEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    ///<Summary>Lerps a recttransform's anchored value from its current value to a target value</Summary>
    public class LerpAnchoredPosition_ToVector3_Executor : PooledUpdateEffectExecutor<LerpAnchoredPosition_ToVector3_Executor.MyEffect, LerpAnchoredPosition_ToVector3_Executor.MyRuntimeData>
    {
        public class MyRuntimeData
        {
            //Runtime
            public Vector3 InitialPosition = default;
            public float Timer = default;
        }

        [System.Serializable]
        public class MyEffect : UpdateEffectWithRuntimeData<MyRuntimeData>
        {
            public RectTransform TargetTransform = default;

            public Vector3 TargetPosition = default;

            [Range(0, 1000)]
            public float Duration = 1;



            public void BeginExecute()
            {
                RuntimeData.InitialPosition = TargetTransform.anchoredPosition;
                RuntimeData.Timer = Duration;
            }

            public bool Execute()
            {
                if (RuntimeData.Timer <= 0)
                {
                TargetTransform.anchoredPosition = TargetPosition;
                    return true;
                }

                RuntimeData.Timer -= Time.deltaTime;

                float percentage = RuntimeData.Timer / Duration;
                TargetTransform.anchoredPosition = Vector3.Lerp(TargetPosition, RuntimeData.InitialPosition, percentage);
                return false;
            }


        }



        protected override void BeginExecuteEffect(MyEffect effectData)
        {
            base.BeginExecuteEffect(effectData);
            effectData.BeginExecute();
        }

        protected override bool ExecuteEffect(MyEffect effectData)
        {
            return effectData.Execute();
        }


    }

}