namespace LinearEffects.DefaultEffects
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    ///<Summary>Lerps a transform's scale to a vector3 value</Summary>
    public class LerpScale_ToVector3_Executor : PooledUpdateEffectExecutor<LerpScale_ToVector3_Executor.MyEffect, LerpScale_ToVector3_Executor.MyRuntimeData>
    {
        public class MyRuntimeData
        {
            //Runtime
            public Vector3 _initialScale = default;
            public float _timer = default;
        }

        [System.Serializable]
        public class MyEffect : UpdateEffectWithRuntimeData<MyRuntimeData>
        {
            public Transform TargetTransform = default;

            public Vector3 TargetScale = default;

            [Range(0, 1000)]
            public float Duration = 1;



            public void BeginExecute()
            {
                RuntimeData._initialScale = TargetTransform.localScale;
                RuntimeData._timer = Duration;
            }

            public bool Execute()
            {
                if (RuntimeData._timer <= 0)
                {
                    TargetTransform.localScale = TargetScale;
                    return true;
                }

                RuntimeData._timer -= Time.deltaTime;

                float percentage = RuntimeData._timer / Duration;
                TargetTransform.localScale = Vector3.Lerp(TargetScale, RuntimeData._initialScale, percentage);
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