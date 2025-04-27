namespace LinearEffects.DefaultEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    ///<Summary>Lerps a transform's world positon value from its current value to a target value</Summary>
    public class LerpPosition_ToVector3_Executor : PooledUpdateEffectExecutor<LerpPosition_ToVector3_Executor.MyEffect, LerpPosition_ToVector3_Executor.MyRuntimeData>
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

            public Vector3 TargetPosition = default;

            [Range(0, 1000)]
            public float Duration = 1;



            public void BeginExecute()
            {
                RuntimeData.InitialPosition = TargetTransform.position;
                RuntimeData.Timer = Duration;
            }

            public bool Execute()
            {
                if (RuntimeData.Timer <= 0)
                {
                TargetTransform.position = TargetPosition;
                    return true;
                }

                RuntimeData.Timer -= Time.deltaTime;

                float percentage = RuntimeData.Timer / Duration;
                TargetTransform.position = Vector3.Lerp(TargetPosition, RuntimeData.InitialPosition, percentage);
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