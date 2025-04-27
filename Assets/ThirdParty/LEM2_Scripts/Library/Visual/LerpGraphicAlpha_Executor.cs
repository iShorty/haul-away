namespace LinearEffects.DefaultEffects
{
    using UnityEngine;
    using UnityEngine.UI;

    ///<Summary>Lerp a graphic's alpha to a value</Summary>
    public class LerpGraphicAlpha_Executor : PooledUpdateEffectExecutor<LerpGraphicAlpha_Executor.MyEffect, LerpGraphicAlpha_Executor.MyRuntimeData>
    {
        public class MyRuntimeData
        {
            //Runtime
            public float Timer = default;
            public float StartAlpha = default;
            public Color CurrentColor = default;

        }
        [System.Serializable]
        public class MyEffect : UpdateEffectWithRuntimeData<MyRuntimeData>
        {
            [Header("----- Target -----")]
            [Range(0, 1)]
            public float TargetAlpha = 0;

            public Graphic TargetGraphic = default;

            [Range(0, 1000)]
            public float Duration = 1f;



            public void BeginExecute()
            {
                RuntimeData.Timer = Duration;
                RuntimeData.StartAlpha = TargetGraphic.color.a;
            }

            public bool Execute()
            {
                if (RuntimeData.Timer <= 0)
                {
                    SetCurrentColorAlpha(TargetAlpha);
                    return true;
                }

                //Count down the timer
                RuntimeData.Timer -= Time.deltaTime;

                //By inverting your start and target vector, you can skip the 1 - (_timer / _duration)
                float percentage = (RuntimeData.Timer / Duration);
                SetCurrentColorAlpha(Mathf.Lerp(TargetAlpha, RuntimeData.StartAlpha, percentage));
                return false;
            }

            void SetCurrentColorAlpha(float alpha)
            {
                RuntimeData.CurrentColor = TargetGraphic.color;
                RuntimeData.CurrentColor.a = alpha;
                TargetGraphic.color = RuntimeData.CurrentColor;
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