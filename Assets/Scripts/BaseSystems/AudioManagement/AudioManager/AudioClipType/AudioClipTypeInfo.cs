namespace AudioManagement
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = nameof(AudioClipTypeInfo), menuName = AudioManager.CREATEASSETMENU_AUDIOMANAGER + "/" + nameof(AudioClipTypeInfo))]
    ///<Summary>Holds all of the audio clips corresponding to the AudioClipType enum</Summary>
    // public class AudioClipTypeInfo : EnumBasedArray_SO<AudioClipType, AudioClip> { }
    public class AudioClipTypeInfo : BetterScriptableObject
    {
#if UNITY_EDITOR
        [Header("----- Resource Path -----")]
        [SerializeField]
        string Path = default;

        protected override void OnValidate()
        {
            base.OnValidate();
            if (!_triggerOnValidate) return;

            Clips = Resources.LoadAll<AudioClip>(Path);
            Clips = EnumBasedArrayExtension.SortToEnumOrder<AudioClip, AudioClipType>(Clips, FindAudio);
        }

        private Predicate<AudioClip> FindAudio(string arg1, int arg2)
        {
            return (AudioClip clip) =>
            {
                return clip.name == arg1;
            };
        }
#endif
        public AudioClip[] Clips = default;

    }
}