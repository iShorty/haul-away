namespace AudioManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    ///<Summary>A basic 2D AudioPlayer. Since this player would be used to spam OneShot multiple times, i have a clip counter to keep track of max number of clips playing to prevent soundcard playing no sound</Summary>
    public class FlatSFXAudioPlayer : BasicAudioPlayer
    {
        #region ---------- Constants --------------
        protected const int MAX_ONESHOT_CLIPS_PLAYING = 20;
        #endregion

        ///<Summary>The number of clips currently playing in the audiosource as oneshot</Summary>
        protected int _numberOfClipsPlaying;


        protected override void Awake()
        {
            base.Awake();
            _audioSource.spatialBlend = _numberOfClipsPlaying = 0;
        }

        public override void JobRequested()
        {
            _numberOfClipsPlaying++;
            IsAvailable = _numberOfClipsPlaying < MAX_ONESHOT_CLIPS_PLAYING;
        }

        public override void JobDone()
        {
            _numberOfClipsPlaying--;
            IsAvailable = _numberOfClipsPlaying < MAX_ONESHOT_CLIPS_PLAYING;
            AudioManager.ReturnInstanceOf(_type, this);
        }

    }

}