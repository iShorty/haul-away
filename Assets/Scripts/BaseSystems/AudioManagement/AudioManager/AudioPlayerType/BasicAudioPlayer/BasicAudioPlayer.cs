namespace AudioManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(AudioSource))]
    ///<Summary>A general audioplayer, there is code for playing OneShot and none oneshot audio. It is also easy to subscribe to a onpause event. This script has 3d spawning methods inside as well</Summary>
    public abstract partial class BasicAudioPlayer : MonoBehaviour
    {
        #region =============== All Fields =====================


        #region -------- Exposed Fields -----------------
        [SerializeField]
        protected AudioPlayerType _type = AudioPlayerType.SFX_TWO_DIMENISIONAL;
        #endregion

        #region --------------- Hidden Fields ------------------

        protected AudioSource _audioSource = default;

        ///<Summary>The default timer corountine reference. This is cached so that you could stop and play the coroutine during events like pause and play</Summary>
        protected IEnumerator _mainTimerCo = default;

        protected bool _wasAudioSourcePlaying = default;
        #endregion

        #region -------- Properties -----------------
        public AudioPlayerType Type => _type;

        ///<Summary>Returns true if AudioPlayer is available to be called to play more audioclips</Summary>
        public bool IsAvailable { get; protected set; }
        #endregion
        #endregion


        protected virtual void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _wasAudioSourcePlaying = false;
        }

        #region ---------- Pooler Availability Methods -------------
        ///<Summary>Updates the IsAvailable property to allow the AudioManager pooler to determine whether or not an instance of BaseAudioPlayer can still be used after it has been requested</Summary>
        public virtual void JobRequested()
        {
            IsAvailable = false;
        }


        ///<Summary>Updates IsAvailable bool when a job request is done (playing of a clip is done) to determine whether or not it gets sent back to the AudioManager pool or not (usually it does)</Summary>
        public virtual void JobDone()
        {
            IsAvailable = true;
            AudioManager.ReturnInstanceOf(_type, this);
        }
        #endregion

        #region ----------- Events Handlers -------------
        ///<Summary>Subscribe to an onpause event if you want the audio to stop (however you might just wanna muffle the sound using channel filters)</Summary>
        protected virtual void OnPauseHandler()
        {
            //Record whether audiosource is playing (the non-one shot way cause oneshots dont affect the isPlaying bool)
            _wasAudioSourcePlaying = _audioSource.isPlaying;
            if (!_wasAudioSourcePlaying) return;

            TryStopAudioTimerCo();
            _audioSource.Pause();
        }
        ///<Summary>Subscribe to an onresume event if you want the audio to resume (however you might just wanna unmuffle the sound using channel filters)</Summary>
        protected virtual void OnResumeHandler()
        {
            if (!_wasAudioSourcePlaying) return;

            float duration = _audioSource.clip.length;
            float currentPlayBackPos = _audioSource.timeSamples / _audioSource.clip.frequency;
            StartAudioTimerCo(duration - currentPlayBackPos);
            _audioSource.UnPause();
        }


        #endregion

    }

}