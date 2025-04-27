namespace AudioManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Audio;

    public class AudioChannelManager : MonoBehaviour
    {
        [field: SerializeField, Header("----- " + nameof(MasterVolume) + " -----")]
        public SetVolume_MixerGroup_Event MasterVolume { get; private set; } = default;

        [field: SerializeField, Header("----- " + nameof(SFXVolume) + " -----")]
        public SetVolume_MixerGroup_Event SFXVolume { get; private set; } = default;

        [field: SerializeField, Header("----- " + nameof(BGMVolume) + " -----")]
        public SetVolume_MixerGroup_Event BGMVolume { get; private set; } = default;


        public static AudioChannelManager Instance { get; private set; } = default;

        private void Awake()
        {
            //Lvl singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                if (Instance == this)
                {
#if UNITY_EDITOR
                    Debug.LogError("Leak detected! Duplicate instance calling Awake twice!", this);
#endif
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError($"Leak detected! Another instance trying to set the {nameof(AudioChannelManager)} instance!", this);
#endif 
                }
            }
        }

        public static void SaveVolumeData(GameData data)
        {
            data.BGMVol = Instance.BGMVolume.GetCurrentValue();
            data.MasterVol = Instance.MasterVolume.GetCurrentValue();
            data.SFXVol = Instance.SFXVolume.GetCurrentValue();
        }

    }

}