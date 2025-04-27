namespace AudioManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    ///<Summary>The audio manager has an internal pooling system which will spawn different types of audiosources with different kinds of scripts on it</Summary>
    public partial class AudioManager : GenericPools<BasicAudioPlayer, AudioManager>
    {
        public const string CREATEASSETMENU_AUDIOMANAGER = "AudioManagement";

        [Header("----- AudioTypes Infos -----")]
        ///<Summary>The scriptableobject that stores all of the references to the corresponding AudioClipType enum value</Summary>
        [SerializeField]
        AudioClipTypeInfo _audioClipsInfo = default;

        private void Awake()
        {
            Pooler_Awake();
            EventHandlers_Awake();
        }

        private void OnDestroy()
        {
            EventHandlers_Destroy();
        }

        #region  --------------- Get Data Methods -------------------
        protected AudioClip GetAudioClip(AudioClipType type)
        {
            int enumAsInt = (int)type;
            AudioClip clip = _audioClipsInfo.Clips[enumAsInt];
#if UNITY_EDITOR
            Debug.Assert(clip, $"The cliptype {type} is not assigned in {_audioClipsInfo}", _audioClipsInfo);
#endif
            return clip;
        }

        ///<Summary>Returns an AudioPlayerType prefab inside of the current m_Settings (AudioPlayerType_PoolerInfo) by inputting a AudioPlayerType</Summary>
        protected GameObject GetAudioPlayerTypePrefab(AudioPlayerType playerType)
        {
            int enumAsInt = (int)playerType;
            GameObject prefab = m_Settings.PooledObjectInfos[enumAsInt].Prefab;
#if UNITY_EDITOR
            Debug.Assert(prefab, $"The player type {playerType} is not assigned in {m_Settings.name}", m_Settings);
#endif
            return prefab;
        }
        #endregion



    }

}