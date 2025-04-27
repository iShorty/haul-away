namespace LinearEffects.DefaultEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    ///<Summary>Lerps a transform's localposition from its current value to a target value</Summary>
    public class LerpLocalPosition_ToPosition_Executor : PooledUpdateEffectExecutor<LerpLocalPosition_ToPosition_Executor.MyEffect, LerpLocalPosition_ToPosition_Executor.MyRuntimeData>
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
            public Transform TargetTransform = default;

            [Tooltip("The local position of the target point you wish to lerp towards")]
            public Vector3 TargetLocalPos = default;

            [Range(0, 1000)]
            public float Duration = 1;



            public void BeginExecute()
            {
                RuntimeData.InitialPosition = TargetTransform.localPosition;
                RuntimeData.Timer = Duration;
            }

            public bool Execute()
            {
                if (RuntimeData.Timer <= 0)
                {
                    TargetTransform.localPosition = TargetLocalPos;
                    return true;
                }

                RuntimeData.Timer -= Time.deltaTime;

                float percentage = RuntimeData.Timer / Duration;
                TargetTransform.localPosition = Vector3.Lerp(TargetLocalPos, RuntimeData.InitialPosition, percentage);
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