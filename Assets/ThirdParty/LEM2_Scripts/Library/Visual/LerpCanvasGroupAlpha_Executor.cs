namespace LinearEffects.DefaultEffects
{
    using UnityEngine;
    ///<Summary>Lerps a canvas group's alpha to a value over time</Summary>
    [System.Serializable]
    public class LerpCanvasGroupAlpha_Executor : PooledUpdateEffectExecutor<LerpCanvasGroupAlpha_Executor.MyEffect, LerpCanvasGroupAlpha_Executor.MyRuntimeData>
    {
        public class MyRuntimeData
        {
            //Runtime
            public float Timer = default;
            public float StartAlpha = default;
        }
        [System.Serializable]
        public class MyEffect : UpdateEffectWithRuntimeData<MyRuntimeData>
        {
            [Header("----- Target -----")]
            [Range(0, 1)]
            public float TargetAlpha = 0;

            [SerializeField]
            public CanvasGroup TargetGroup = default;

            [SerializeField]
            [Range(0, 1000)]
            public float Duration = 1f;




            public void BeginExecute()
            {
                RuntimeData.Timer = Duration;
                RuntimeData.StartAlpha = TargetGroup.alpha;
            }

            public bool Execute()
            {
                if (RuntimeData.Timer <= 0)
                {
                    TargetGroup.alpha = TargetAlpha;
                    return true;
                }

                //Count down the timer
                RuntimeData.Timer -= Time.deltaTime;

                //By inverting your start and target vector, you can skip the 1 - (_timer / _duration)
                float percentage = (RuntimeData.Timer / Duration);
                TargetGroup.alpha = Mathf.Lerp(TargetAlpha, RuntimeData.StartAlpha, percentage);

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