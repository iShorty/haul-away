namespace AudioManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    //This file will subscribe to all the events in the audio events .cs file
    public partial class AudioManager
    {
        #region --------------- Awake & Destroy ------------------
        void EventHandlers_Awake()
        {
            //Subscribe all of your play methods to events here
            AudioEvents.OnPlay2DSFX += EventHandlers_OnPlay2DSFX;
            AudioEvents.OnPlay2DSFX_Volume += EventHandlers_OnPlay2DSFX;
            AudioEvents.OnPlayBGM += EventHandlers_OnPlayBGM;
            AudioEvents.OnPlayBGM_Volume += EventHandlers_OnPlayBGM;
            AudioEvents.OnPlay3DAtLocation += EventHandlers_OnPlayAtLocation;
            AudioEvents.OnPlay3DAtLocation_Volume += EventHandlers_OnPlayAtLocation;
            AudioEvents.OnPlay3DFollow += EventHandlers_OnPlayFollow;
            AudioEvents.OnPlay3DFollow_Volume += EventHandlers_OnPlayFollow;
        }


        void EventHandlers_Destroy()
        {
            AudioEvents.OnPlay2DSFX -= EventHandlers_OnPlay2DSFX;
            AudioEvents.OnPlay2DSFX_Volume -= EventHandlers_OnPlay2DSFX;
            AudioEvents.OnPlayBGM -= EventHandlers_OnPlayBGM;
            AudioEvents.OnPlayBGM_Volume -= EventHandlers_OnPlayBGM;
            AudioEvents.OnPlay3DAtLocation -= EventHandlers_OnPlayAtLocation;
            AudioEvents.OnPlay3DAtLocation_Volume -= EventHandlers_OnPlayAtLocation;
            AudioEvents.OnPlay3DFollow -= EventHandlers_OnPlayFollow;
            AudioEvents.OnPlay3DFollow_Volume -= EventHandlers_OnPlayFollow;
        }
        #endregion


        #region ============================= Play Audio Methods ===================================

        #region ----------- 2D Audio Methods ------------------
        #region *********** Play 2D SFX Methods ***************
        ///<Summary>Plays a 2D audio clip on the SFX_TWO_DIMENISIONAL pooled prefab. If isOneShot is true, the sfx played wont be paused if there is a pause event subscribed</Summary>
        void EventHandlers_OnPlay2DSFX(AudioClipType clipType, bool isOneShot)
        {
            AudioClip clip = GetAudioClip(clipType);
            BasicAudioPlayer player = GetInstanceOf(AudioPlayerType.SFX_TWO_DIMENISIONAL);

            player.PlayAudio(clip, isOneShot);
        }

        ///<Summary>Plays a 2D audio clip on the SFX_TWO_DIMENISIONAL pooled prefab. If isOneShot is true, the sfx played wont be paused if there is a pause event subscribed. Adjust the volume here</Summary>
        void EventHandlers_OnPlay2DSFX(AudioClipType clipType, float volume, bool isOneShot)
        {
            AudioClip clip = GetAudioClip(clipType);
            BasicAudioPlayer player = GetInstanceOf(AudioPlayerType.SFX_TWO_DIMENISIONAL);

            player.PlayAudio(clip, volume, isOneShot);
        }
        #endregion

        #region ************** Play BGM Methods ******************

        ///<Summary>Plays a 2D audio clip on the BGM_TWO_DIMENISIONAL pooled prefab. If bool value is true, the current clip will be overriden else, it will be queued. If the currently playing clip is the same clip as the one passed in and overrideCurrent is true, nothing will happen</Summary>
        void EventHandlers_OnPlayBGM(AudioClipType clipType, BGMAudioPlayer.BGM_PlayType type)
        {
            AudioClip clip = GetAudioClip(clipType);
            BasicAudioPlayer player = GetInstanceOf(AudioPlayerType.BGM_TWO_DIMENISIONAL);

            player.PlayBGMAudio(clip, type);

        }

        ///<Summary>Plays a 2D audio clip on the BGM_TWO_DIMENISIONAL pooled prefab. If bool value is true, the current clip will be overriden else, it will be queued. If the currently playing clip is the same clip as the one passed in and overrideCurrent is true, nothing will happen</Summary>
        private void EventHandlers_OnPlayBGM(AudioClipType clipType, float volume, BGMAudioPlayer.BGM_PlayType type)
        {
            AudioClip clip = GetAudioClip(clipType);
            BasicAudioPlayer player = GetInstanceOf(AudioPlayerType.BGM_TWO_DIMENISIONAL);

            player.PlayBGMAudio(clip, volume, type);
        }


        #endregion
        #endregion

        #region -------------- 3D Audio Methods ------------------
        #region ************** OnPlayAtLocation Methods ******************

        ///<Summary>Plays an 3D audio clip on the SPATIAL_AT_LOCATION pooled prefab. If isOneShot is true, the sfx played wont be paused if there is a pause event subscribed</Summary>
        BasicAudioPlayer EventHandlers_OnPlayAtLocation(AudioClipType clipType, Vector3 worldPos, bool isOneShot, bool autoReturn)
        {
            AudioClip clip = GetAudioClip(clipType);
            BasicAudioPlayer player = GetInstanceOf(AudioPlayerType.SPATIAL_AT_LOCATION);
            player.PlayAudioAtLocation(clip, worldPos, isOneShot, autoReturn);
            return player;
        }

        ///<Summary>Plays an 3D audio clip on the SPATIAL_AT_LOCATION pooled prefab. If isOneShot is true, the sfx played wont be paused if there is a pause event subscribed</Summary>
        private BasicAudioPlayer EventHandlers_OnPlayAtLocation(AudioClipType clipType, Vector3 worldPos, float volume, bool isOneShot, bool autoReturn)
        {
            AudioClip clip = GetAudioClip(clipType);
            BasicAudioPlayer player = GetInstanceOf(AudioPlayerType.SPATIAL_AT_LOCATION);
            player.PlayAudioAtLocation(clip, worldPos, volume, isOneShot, autoReturn);
            return player;

        }


        #endregion


        #region ************** OnPlayFollow Methods ******************

        ///<Summary>Plays an 3D audio clip on the SPATIAL_FOLLOW pooled prefab. If isOneShot is true, the sfx played wont be paused if there is a pause event subscribed</Summary>
        BasicAudioPlayer EventHandlers_OnPlayFollow(AudioClipType clipType, Transform target, bool isOneShot, bool autoReturn)
        {
            AudioClip clip = GetAudioClip(clipType);
            BasicAudioPlayer player = GetInstanceOf(AudioPlayerType.SPATIAL_FOLLOW);
            player.PlayAudioFollow(clip, target, isOneShot, autoReturn);
            return player;
        }

        ///<Summary>Plays an 3D audio clip on the SPATIAL_FOLLOW pooled prefab. If isOneShot is true, the sfx played wont be paused if there is a pause event subscribed</Summary>
        private BasicAudioPlayer EventHandlers_OnPlayFollow(AudioClipType clipType, Transform target, float volume, bool isOneShot, bool autoReturn)
        {
            AudioClip clip = GetAudioClip(clipType);
            BasicAudioPlayer player = GetInstanceOf(AudioPlayerType.SPATIAL_FOLLOW);
            player.PlayAudioFollow(clip, target, volume, isOneShot, autoReturn);
            return player;
        }


        #endregion
        #endregion


        #endregion














    }
}