namespace AudioManagement
{
    using System;
    using UnityEngine;

    public static partial class AudioEvents
    {
        #region ============= 2D Sounds ======================

        #region ------------- 2DSFX ----------------------

        public static event Action<AudioClipType, bool> OnPlay2DSFX = null;
        ///<Summary>Plays a 2D audio clip on the SFX_TWO_DIMENISIONAL pooled prefab. If isOneShot is true, the sfx played wont be paused if there is a pause event subscribed</Summary>
        public static void RaiseOnPlay2DSFX(AudioClipType clipType, bool isOneShot)
        {
            if (ReferenceEquals(OnPlay2DSFX, null))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"There is no {nameof(AudioManager)} subscribed to the event {nameof(OnPlay2DSFX)}! Is AudioManager in the scene?!?!");
#endif
                return;
            }
            OnPlay2DSFX.Invoke(clipType, isOneShot);
        }

        public static event Action<AudioClipType, float, bool> OnPlay2DSFX_Volume = null;
        ///<Summary>Plays a 2D audio clip on the SFX_TWO_DIMENISIONAL pooled prefab. If isOneShot is true, the sfx played wont be paused if there is a pause event subscribed. You can adjust the volume scale here</Summary>
        public static void RaiseOnPlay2DSFX(AudioClipType clipType, float volume, bool isOneShot)
        {
            if (ReferenceEquals(OnPlay2DSFX_Volume, null))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"There is no {nameof(AudioManager)} subscribed to the event {nameof(OnPlay2DSFX_Volume)}! Is AudioManager in the scene?!?!");
#endif
                return;
            }
            OnPlay2DSFX_Volume?.Invoke(clipType, volume, isOneShot);
        }


        #endregion

        #region ------------- BGM ----------------------

        public static event Action<AudioClipType, BGMAudioPlayer.BGM_PlayType> OnPlayBGM = null;
        ///<Summary>Plays a 2D audio clip on the BGM_TWO_DIMENISIONAL pooled prefab. Peek at the enum's defintiion to know what kinds of effect you can apply onto the audio you are about to play</Summary>
        public static void RaiseOnPlayBGM(AudioClipType clipType, BGMAudioPlayer.BGM_PlayType type)
        {
            if (ReferenceEquals(OnPlayBGM, null))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"There is no {nameof(AudioManager)} subscribed to the event {nameof(OnPlayBGM)}! Is AudioManager in the scene?!?!");
#endif
                return;
            }
            OnPlayBGM.Invoke(clipType, type);
        }

        public static event Action<AudioClipType, float, BGMAudioPlayer.BGM_PlayType> OnPlayBGM_Volume = null;
        ///<Summary>Plays a 2D audio clip on the BGM_TWO_DIMENISIONAL pooled prefab. Peek at the enum's defintiion to know what kinds of effect you can apply onto the audio you are about to play</Summary>
        public static void RaiseOnPlayBGM(AudioClipType clipType, float volume, BGMAudioPlayer.BGM_PlayType type)
        {
            if (ReferenceEquals(OnPlayBGM_Volume, null))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"There is no {nameof(AudioManager)} subscribed to the event {nameof(OnPlayBGM_Volume)}! Is AudioManager in the scene?!?!");
#endif
                return;
            }
            OnPlayBGM_Volume?.Invoke(clipType, volume, type);
        }

        #endregion

        #endregion

        #region ============= 3D Sounds ======================

        #region ------------- SPATIAL_AT_LOCATION ----------------------

        public static event Func<AudioClipType, Vector3, bool, bool, BasicAudioPlayer> OnPlay3DAtLocation = null;
        ///<Summary>Plays a 3D audio clip on the SPATIAL_AT_LOCATION pooled prefab at a loction. If isOneShot is true, the sfx played wont be paused if there is a pause event subscribed</Summary>
        public static BasicAudioPlayer RaiseOnPlay3DAtLocation(AudioClipType clipType, Vector3 worldPos, bool isOneShot, bool autoReturn)
        {
            if (ReferenceEquals(OnPlay3DAtLocation, null))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"There is no {nameof(AudioManager)} subscribed to the event {nameof(OnPlay3DAtLocation)}! Is AudioManager in the scene?!?!");
#endif
                return null;
            }
            return OnPlay3DAtLocation.Invoke(clipType, worldPos, isOneShot,autoReturn);
        }

        public static event Func<AudioClipType, Vector3, float, bool, bool, BasicAudioPlayer> OnPlay3DAtLocation_Volume = null;
        ///<Summary>Plays a 3D audio clip on the SPATIAL_AT_LOCATION pooled prefab at a loction. If isOneShot is true, the sfx played wont be paused if there is a pause event subscribed</Summary>
        public static BasicAudioPlayer RaiseOnPlay3DAtLocation(AudioClipType clipType, Vector3 worldPos, float volume, bool isOneShot, bool autoReturn)
        {
            if (ReferenceEquals(OnPlay3DAtLocation_Volume, null))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"There is no {nameof(AudioManager)} subscribed to the event {nameof(OnPlay3DAtLocation_Volume)}! Is AudioManager in the scene?!?!");
#endif
                return null;
            }
            return OnPlay3DAtLocation_Volume.Invoke(clipType, worldPos, volume, isOneShot,autoReturn);
        }


        #endregion


        #region ------------- SPATIAL_FOLLOW ----------------------

        public static event Func<AudioClipType, Transform, bool, bool, BasicAudioPlayer> OnPlay3DFollow = null;
        ///<Summary>Plays a 3D audio clip on the SPATIAL_FOLLOW pooled prefab whilst following a target. If isOneShot is true, the sfx played wont be paused if there is a pause event subscribed</Summary>
        public static BasicAudioPlayer RaiseOnPlay3DFollow(AudioClipType clipType, Transform followTarget, bool isOneShot, bool autoReturn)
        {
            if (ReferenceEquals(OnPlay3DFollow, null))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"There is no {nameof(AudioManager)} subscribed to the event {nameof(OnPlay3DFollow)}! Is AudioManager in the scene?!?!");
#endif
                return null;
            }
            return OnPlay3DFollow.Invoke(clipType, followTarget, isOneShot,autoReturn);
        }

        public static event Func<AudioClipType, Transform, float, bool, bool, BasicAudioPlayer> OnPlay3DFollow_Volume = null;
        ///<Summary>Plays a 3D audio clip on the SPATIAL_FOLLOW pooled prefab whilst following a target. If isOneShot is true, the sfx played wont be paused if there is a pause event subscribed</Summary>
        public static BasicAudioPlayer RaiseOnPlay3DFollow(AudioClipType clipType, Transform followTarget, float volume, bool isOneShot, bool autoReturn)
        {
            if (ReferenceEquals(OnPlay3DFollow_Volume, null))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"There is no {nameof(AudioManager)} subscribed to the event {nameof(OnPlay3DFollow_Volume)}! Is AudioManager in the scene?!?!");
#endif
                return null;
            }
            return OnPlay3DFollow_Volume.Invoke(clipType, followTarget, volume, isOneShot,autoReturn);
        }


        #endregion

        #endregion

    }
}
