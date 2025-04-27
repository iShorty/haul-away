// namespace AudioManagement
// {
//     using System;
//     using UnityEngine;
//     using UnityEngine.Audio;
//     using ScriptableObjectEvents;

//     // [CreateAssetMenu(fileName = nameof(AudioChannelInfo), menuName = AudioManager.CREATEASSETMENU_AUDIOMANAGER + "/" + nameof(AudioChannelInfo), order = 0)]
//     ///<Summary>The base scriptableobject where any derived classes will be able to set parameters of the audio related assets like (AudioMixerGroup and AudioSnapShot). The reason why i inherited this from BaseScriptable event is because I want to make this compatible with LEM 2's future executors which will use SOEvent_RVoid_Float</Summary>
//     public abstract class Base_SetFloat_AudioMixerEvent : SOEvent_RVoid_Float
//     {
      

//         ///<Summary>Sets the float value of the parameter</Summary>
//         public override void RaiseEvent(float f)
//         {
//             base.RaiseEvent(f);
//         }

//         ///<Summary>Returns the current float value of the parameter</Summary>
//         public abstract float GetCurrentValue();

//     }


// }