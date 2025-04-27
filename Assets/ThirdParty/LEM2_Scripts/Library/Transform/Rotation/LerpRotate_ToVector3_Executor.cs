namespace LinearEffects.DefaultEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    ///<Summary>Lerps a transform's rotation to a vector3 value</Summary>
    public class LerpRotate_ToVector3_Executor : PooledUpdateEffectExecutor<LerpRotate_ToVector3_Executor.MyEffect, LerpRotate_ToVector3_Executor.MyRuntimeData>
    {
        public class MyRuntimeData
        {
            //Runtime
            public Quaternion InitialRotation = default
               , TargetRotation = default
                ;

            public float Timer = default;
        }
        [System.Serializable]
        public class MyEffect : UpdateEffectWithRuntimeData<MyRuntimeData>
        {
            public Transform TargetTransform = default;

            public Vector3 TargetRotation = default
            ;

            [Range(0, 1000)]
            public float Duration = 1;



            public void BeginExecute()
            {
                RuntimeData.InitialRotation = TargetTransform.localRotation;
                RuntimeData.TargetRotation = Quaternion.Euler(TargetRotation);
                RuntimeData.Timer = Duration;
            }

            public bool Execute()
            {
                if (RuntimeData.Timer <= 0)
                {
                    TargetTransform.localRotation = RuntimeData.TargetRotation;
                    return true;
                }

                RuntimeData.Timer -= Time.deltaTime;

                float percentage = 1 - (RuntimeData.Timer / Duration);
                TargetTransform.localRotation = Quaternion.Lerp(RuntimeData.InitialRotation, RuntimeData.TargetRotation, percentage);
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