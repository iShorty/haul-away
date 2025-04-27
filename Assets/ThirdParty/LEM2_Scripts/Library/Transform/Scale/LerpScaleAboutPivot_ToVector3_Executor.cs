namespace LinearEffects.DefaultEffects
{
    using UnityEngine;

    ///<Summary>Lerps a transform's scale towards a value about a local pivot</Summary>
    public class LerpScaleAboutPivot_ToVector3_Executor : PooledUpdateEffectExecutor<LerpScaleAboutPivot_ToVector3_Executor.MyEffect, LerpScaleAboutPivot_ToVector3_Executor.MyRuntimeData>
    {
        public class MyRuntimeData
        {
            //Runtime
            public Vector3 InitialScale = default
                , Direction = default
                , InitialPosition = default
                ;
            public float Timer = default;
        }

        [System.Serializable]
        public class MyEffect : UpdateEffectWithRuntimeData<MyRuntimeData>
        {
            public Transform TargetTransform = default;

            public Vector3 TargetScale = default
                , LocalPivot = default
                ;

            [Range(0, 1000)]
            public float Duration = 1;



            public void BeginExecute()
            {
                RuntimeData.InitialPosition = TargetTransform.position;
                // _pivotWorldPos = _transform.TransformPoint(_localPivot);
                //Get the original direction vector (with their mags intact) from the center of the transform to the pivot point
                RuntimeData.Direction = TargetTransform.TransformPoint(LocalPivot) - RuntimeData.InitialPosition;
                RuntimeData.InitialScale = TargetTransform.localScale;
                RuntimeData.Timer = Duration;
            }

            public bool Execute()
            {
                if (RuntimeData.Timer <= 0)
                {
                    TargetTransform.localScale = TargetScale;
                    return true;
                }

                RuntimeData.Timer -= Time.deltaTime;

                float percentage = 1 - (RuntimeData.Timer / Duration);
                //Lerp the scale of the cube
                TargetTransform.localScale = Vector3.Lerp(RuntimeData.InitialScale, TargetScale, percentage);


                //============= SETTING THE NEW POSITIION OF THE TRANSFORM ====================
                //Invert the direction so that we can change the transform's position in the scaleddirection
                Vector3 dir = -RuntimeData.Direction;
                dir *= percentage;

                //Translate the dir point back to pivot
                dir += RuntimeData.InitialPosition;
                TargetTransform.position = dir;

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