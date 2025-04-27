namespace AudioManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    ///<Summary>A BGM player which plays audio on the BGM audiomixergroup. I made it to be able to queue songs back to back as well as to be able to override the currently playing song (note that if the current playing song is the same as the clip to be played and overrideCurrent is true, nothing will happen)</Summary>
    public partial class BGMAudioPlayer : BasicAudioPlayer
    {

        #region --------- Exposed Field -------------
        [Header("----- Snapshot Events -----")]
        [Header("===== BGM Audio Player =====")]
        [SerializeField]
        Transition_SnapShot_Event _muteBGM_SnapShot = default;

        [SerializeField]
        Transition_SnapShot_Event _normalSnapShot = default;

        [SerializeField]
        [Range(0, 10)]
        float _fadeDuration = 0.5f;
        #endregion


        Queue<ValueTuple<AudioClip, float>> _songQueue = new Queue<ValueTuple<AudioClip, float>>();

        ///<Summary>Plays the audio only in normal mode. If bool value is true, the current clip will be overriden else, it will be queued. If the currently playing clip is the same clip as the one passed in and overrideCurrent is true, nothing will happen</Summary>
        AudioClip _prevPlayingClip = default;

        ///<Summary>Previous playback time on the previously playing clip</Summary>
        float _prevTime = default;

        ///<Summary>Previous playtype called on the BGM Player</Summary>
        BGM_PlayType _prevPlayType = default;

        WaitForSeconds _fadeWaitForSeconds = default;
        IEnumerator _fadeCoroutine = default;

        protected override void Awake()
        {
            base.Awake();
            _fadeWaitForSeconds = new WaitForSeconds(_fadeDuration);
        }

        #region ------------- Pool Availability ----------------
        public override void JobRequested()
        {
            IsAvailable = true;
        }

        public override void JobDone()
        {
            IsAvailable = true;
            _audioSource.clip = null;
        }
        #endregion

        #region =============== Timer & Cause Of Action Methods ==============
        protected override IEnumerator Co_ClipTimer(float audioClipDuration)
        {
            yield return new WaitForSeconds(audioClipDuration);

            //Determine the next cause of action when clip timer has ended
            switch (_prevPlayType)
            {
                case BGM_PlayType.FADEIN_LOOP:
                    FADEIN_LOOP_CauseOfAction();
                    break;

                case BGM_PlayType.LOOP:
                    LOOP_CauseOfAction();
                    break;

                //By default, QUEUE, OVERRIDE, OVERRIDE_REGARDLESS will fall into this
                default:
                    DEFAULT_CauseOfAction();
                    break;
            }

        }





        #region ------------------ Causes of Action ------------------
        protected virtual void DEFAULT_CauseOfAction()
        {
            //Check if queue has any songs
            if (_songQueue.Count > 0)
            {
                //Play a new song
                ValueTuple<AudioClip, float> songData = _songQueue.Dequeue();
                PlayNormalAudio(songData.Item1, songData.Item2);
            }
            //Else if there is no more songs queued to play, stop the timer
            else
            {
                TryStopAudioTimerCo();
                JobDone();
            }
        }

        protected virtual void LOOP_CauseOfAction()
        {
            //Loop the currently played song
            OverrideClipInLoop(_audioSource.clip, _audioSource.volume);
        }

        protected virtual void FADEIN_LOOP_CauseOfAction()
        {
            _audioSource.time = 0;
            //Loop the currently played song
            OverrideClipInLoop(_audioSource.clip, _audioSource.volume);
        }

        #endregion

        #endregion


    }

}