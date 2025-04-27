namespace AudioManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = nameof(SetVolume_MixerGroup_Event), menuName = AudioManager.CREATEASSETMENU_AUDIOMANAGER + "/" + nameof(SetVolume_MixerGroup_Event))]
    ///<Summary>This event will cache an audiomixer group which will set the volume of the audiomixer parameter using a Math formula to ensure that the audiomixer's volume is set correctly</Summary>
    public class SetVolume_MixerGroup_Event : SetFloat_MixerGroup_Event
    {

        ///<Summary>The passed in value of f must be > 0 and <= 1. This method will set the MixerGroup's volume parameter.</Summary>
        public override void RaiseEvent(float f)
        {
#if UNITY_EDITOR
            Debug.Assert(f > 0, $"Float value must not lesser or equal to 0 as Log10 function will be unable to return a value!", this);
            Debug.Assert(f <= 1, $"Float value must not be more than 1 as Log10 function will be unable to return a value!", this);
#endif
            f = Mathf.Log10(f) * 20;
            base.RaiseEvent(f);
        }

        public override float GetCurrentValue()
        {
            //if y = log10(x), then x = 10^y.
            float actualSliderValue = base.GetCurrentValue();
            actualSliderValue /= 20;
            actualSliderValue = Mathf.Pow(10, actualSliderValue);
            return actualSliderValue;
        }


    }

}