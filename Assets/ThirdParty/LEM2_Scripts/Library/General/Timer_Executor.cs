namespace LinearEffects.DefaultEffects
{
    using LinearEffects;
    using UnityEngine;

    [DisallowMultipleComponent]
    ///<Summary>Counts down a timer variable for a given duration. Is useful for halting flow of code</Summary>
    public class Timer_Executor : UpdateEffectExecutor<Timer_Executor.MyEffect>
    {
        [System.Serializable]
        public class MyEffect : UpdateEffect
        {
            public float Duration = default;

            float _timer = -1;


            public void Reset()
            {
                _timer = Duration;
            }

            public bool TickDown()
            {
                if (_timer > 0)
                {
                    _timer -= Time.deltaTime;
                    return false;
                }

                return true;
            }

        }

        protected override bool ExecuteEffect(MyEffect effectData)
        {
            return effectData.TickDown();
        }

        protected override void BeginExecuteEffect(MyEffect effectData)
        {
            base.BeginExecuteEffect(effectData);
            effectData.Reset();
        }
    }



}