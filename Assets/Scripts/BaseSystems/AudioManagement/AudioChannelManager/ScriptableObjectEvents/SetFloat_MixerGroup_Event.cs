namespace AudioManagement
{
    using UnityEngine;
    using UnityEngine.Audio;
    using ScriptableObjectEvents;

    [CreateAssetMenu(fileName = nameof(SetFloat_MixerGroup_Event), menuName = AudioManager.CREATEASSETMENU_AUDIOMANAGER + "/" + nameof(SetFloat_MixerGroup_Event), order = 0)]
    ///<Summary>Caches an audio mixergroup which would then set its parameter to an inputted float value</Summary>
    public class SetFloat_MixerGroup_Event : SOEvent_RVoid_Float
    {
        [Header("===== Set Audio Values =====")]
        [SerializeField]
        protected string ParameterName = "ParameterName";

        public AudioMixerGroup MixerGroup = default;

        public virtual float GetCurrentValue()
        {
            if (MixerGroup.audioMixer.GetFloat(ParameterName, out float getValue))
            {
                return getValue;
            }
#if UNITY_EDITOR
            Debug.LogError($"The parameter {ParameterName} does not exist or is exposed on {MixerGroup.name}!", MixerGroup);
#endif
            return -1;
        }

        public override void RaiseEvent(float f)
        {
            MixerGroup.audioMixer.SetFloat(ParameterName, f);
            base.RaiseEvent(f);
        }

    }
}