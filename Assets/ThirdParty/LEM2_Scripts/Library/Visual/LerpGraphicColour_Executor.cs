namespace LinearEffects.DefaultEffects
{
    using UnityEngine;
    using UnityEngine.UI;
    [System.Serializable]
    ///<Summary>Lerp a graphic's color to another color value</Summary>
    public class LerpGraphicColour_Executor : PooledUpdateEffectExecutor<LerpGraphicColour_Executor.MyEffect, LerpGraphicColour_Executor.MyRuntimeData>
    {
        public class MyRuntimeData
        {
            //Runtime
            public float Timer = default;
            public Color StartColor = default;

        }
        [System.Serializable]
        public class MyEffect : UpdateEffectWithRuntimeData<MyRuntimeData>
        {
            [Header("----- Colour References -----")]
            public Color TargetColor = Color.white;

            [SerializeField]
            public Graphic TargetGraphic = default;

            [SerializeField]
            [Range(0, 1000)]
            public float Duration = 1f;



            public void StartExecute()
            {
                RuntimeData.Timer = Duration;
                RuntimeData.StartColor = TargetGraphic.color;
            }

            public bool Execute()
            {
                if (RuntimeData.Timer <= 0)
                {
                    TargetGraphic.color = TargetColor;
                    return true;
                }

                //Count down the timer
                RuntimeData.Timer -= Time.deltaTime;

                //By inverting your start and target vector, you can skip the 1 - (_timer / _duration)
                float percentage = (RuntimeData.Timer / Duration);
                TargetGraphic.color = Vector4.Lerp(TargetColor, RuntimeData.StartColor, percentage);

                return false;
            }


        }


        protected override void BeginExecuteEffect(MyEffect effectData)
        {
            base.BeginExecuteEffect(effectData);
            effectData.StartExecute();
        }

        protected override bool ExecuteEffect(MyEffect effectData)
        {
            return effectData.Execute();
        }
    }

}