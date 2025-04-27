namespace AudioManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine.Audio;
    using UnityEngine;
    using ScriptableObjectEvents;

    [CreateAssetMenu(fileName = nameof(Transition_SnapShot_Event), menuName = AudioManager.CREATEASSETMENU_AUDIOMANAGER + "/" + nameof(Transition_SnapShot_Event))]
    ///<Summary>This event will cache an audiomixer group which will set the volume of the audiomixer parameter using a Math formula to ensure that the audiomixer's volume is set correctly</Summary>
    public class Transition_SnapShot_Event : SOEvent_RVoid_Float
    {
        public AudioMixerSnapshot SnapShot = default;
        ///<Summary>The passed in value of f must be > 0. This method will transition to the audiosnapshot at the f duration.</Summary>
        public override void RaiseEvent(float f)
        {
#if UNITY_EDITOR
            Debug.Assert(f > 0, $"Float value must not lesser or equal to 0 as Log10 function will be unable to return a value!", this);
#endif
            SnapShot.TransitionTo(f);
            base.RaiseEvent(f);
        }
    }

}