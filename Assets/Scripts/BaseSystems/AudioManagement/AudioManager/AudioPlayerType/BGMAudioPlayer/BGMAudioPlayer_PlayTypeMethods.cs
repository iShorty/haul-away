namespace AudioManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    //This file will hold all the play type methods

    public partial class BGMAudioPlayer : BasicAudioPlayer
    {
        #region ----------------- Constants ----------------
        ///<Summary>When you play a BGM, you can choose how to override it or not to override it at all. This enum holds all the different kinds of methods of playing a BGM </Summary>
        public enum BGM_PlayType
        {
              ///<Summary>Queues the audio clip to be the next clip to be played. If there is no clip playing, then the passed in clip will be played instead. Be aware that the queue will not be afffected by any of the other play functions and will still retain its values even if you override the current clip.</Summary>
            QUEUE
            ,
            ///<Summary>Immediately plays the new clip over the current playing clip. However, if the new clip is the same as the currently playing clip, the current playing clip wont be overriden. If no clip is playing, the new clip is played instead</Summary>
            OVERRIDE
            ,
            ///<Summary>Immediately plays the new clip over the current playing clip regardless if the new clip is the same as the currently playing clip or not.</Summary>
            OVERRIDE_REGARDLESS
            ,
            ///<Summary>Fades in an audio clip and fades out the currently playing one if any. Any active queue will still be retained. The faded in audio clip will be played on loop until another kind of PlayBGMAudio() is called. If the clip to play is the same as the currently playing clip, nothing will happen</Summary>
            FADEIN_LOOP
            ,
          ///<Summary>Immediately plays the new clip over the current playing clip and will repeatly play the same clip even when the clip is finished. However, if the new clip is the same as the currently playing clip, the current playing clip wont be overriden.</Summary>
            LOOP
        }

        #endregion

        #region  ----------------- Main Play Methods --------------------
        ///<Summary>Plays a BGM audio clip depending on the type selected. Peek at the enum's defintiion to know what kinds of effect you can apply onto the audio you are about to play</Summary>
        public override void PlayBGMAudio(AudioClip clip, BGM_PlayType type)
        {
            switch (type)
            {
                case BGM_PlayType.QUEUE:
                    QueueClip(clip, 1);
                    break;

                case BGM_PlayType.OVERRIDE:
                    OverrideClip(clip, 1);
                    break;

                case BGM_PlayType.OVERRIDE_REGARDLESS:
                    OverrideRegardlessClip(clip, 1);
                    break;

                case BGM_PlayType.FADEIN_LOOP:
                    FadeInClip(clip, 1);
                    break;

                case BGM_PlayType.LOOP:
                    OverrideClipInLoop(clip, 1);
                    break;
            }

            _prevPlayType = type;
        }



        ///<Summary>Plays a BGM audio clip depending on the type selected. Peek at the enum's defintiion to know what kinds of effect you can apply onto the audio you are about to play</Summary>
        public override void PlayBGMAudio(AudioClip clip, float volumeScale, BGM_PlayType type)
        {
            switch (type)
            {
                case BGM_PlayType.QUEUE:
                    QueueClip(clip, volumeScale);
                    break;

                case BGM_PlayType.OVERRIDE:
                    OverrideClip(clip, volumeScale);
                    break;

                case BGM_PlayType.OVERRIDE_REGARDLESS:
                    OverrideRegardlessClip(clip, volumeScale);
                    break;

                case BGM_PlayType.FADEIN_LOOP:
                    FadeInClip(clip, volumeScale);
                    break;

                case BGM_PlayType.LOOP:
                    OverrideClipInLoop(clip, volumeScale);
                    break;
            }

            _prevPlayType = type;
        }



        #endregion

      #region ------------------- Base Play Methods -------------------------
        ///<Summary>Plays the audio clip using the normal audiosource play method with volume scale as well as giving you to ability to choose when the audio should start playing at.</Summary>
        public virtual void PlayNormalAudio(AudioClip clip, float volumeScale, float startTime)
        {
            _audioSource.time = startTime;
            _audioSource.volume = volumeScale;
            _audioSource.clip = clip;
            _audioSource.Play();

            //The timer should be calculated for the time needed to wait
            startTime = clip.length - startTime;
            StartAudioTimerCo(startTime);
        }
        #endregion


        #region ------------- PlayType Methods ---------------
        ///<Summary>Queues the audio clip to be the next clip to be played. If there is no clip playing, then the passed in clip will be played instead.</Summary>
        protected virtual void QueueClip(AudioClip clip, float volume)
        {
            if (_audioSource.isPlaying)
            {
                //Queue the song
                _songQueue.Enqueue(new ValueTuple<AudioClip, float>(clip, volume));
                return;
            }

            //Else play song
            PlayNormalAudio(clip, volume);
        }

        ///<Summary>Immediately plays the new clip over the current playing clip. However, if the new clip is the same as the currently playing clip, the current playing clip wont be overriden. If no clip is playing, the new clip is played instead</Summary>
        protected virtual void OverrideClip(AudioClip clip, float volume)
        {
            //If clip to play is the same, then return
            if (clip == _audioSource.clip)
            {
                return;
            }

            //Stop timer first before playing
            TryStopAudioTimerCo();
            PlayNormalAudio(clip, volume);
        }


        ///<Summary>Immediately plays the new clip over the current playing clip regardless if the new clip is the same as the currently playing clip or not</Summary>
        protected virtual void OverrideRegardlessClip(AudioClip clip, float volume)
        {
            //Stop timer first before playing
            TryStopAudioTimerCo();
            PlayNormalAudio(clip, volume);
        }

        ///<Summary>Immediately plays the new clip over the current playing clip and will repeatly play the same clip even when the clip is finished. However, if the new clip is the same as the currently playing clip, the current playing clip wont be overriden. Be aware that playing an audio clip using the LOOP type will clear all queues that are currently active</Summary>
        private void OverrideClipInLoop(AudioClip clip, float volumeScale)
        {
            //Stop timer first before playing
            TryStopAudioTimerCo();
            PlayNormalAudio(clip, volumeScale);
        }

        #region --------- Fading Audio Methods --------------
        ///<Summary>Fades in an audio clip and fades out the currently playing one if any. Any active queue will still be retained. The faded in audio clip will be played on loop until another kind of PlayBGMAudio() is called. If the clip to play is the same as the currently playing clip, nothing will happen</Summary>
        protected virtual void FadeInClip(AudioClip clip, float volume)
        {
            //If the clip to play is the same as the currently playing clip, 
            if (clip == _audioSource.clip)
            {
                return;
            }

            float timeToStartFrom = 0;

            //If new clip which you are playing is something that was previously faded out,
            if (clip == _prevPlayingClip)
            {
                timeToStartFrom = _prevTime;
            }

            //Record the current playing clip so that we can at least return to it if another fadeinclip() is called again
            _prevPlayingClip = _audioSource.clip;
            _prevTime = _audioSource.time;

            TryStopFadeCo();
            _fadeCoroutine = FadeSnapShotCo(clip, volume, timeToStartFrom);
            StartCoroutine(_fadeCoroutine);
        }

        protected virtual void TryStopFadeCo()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
        }

         protected virtual IEnumerator FadeSnapShotCo(AudioClip clip, float volume, float oldTimeToStartFrom)
        {
            //use the audiosnapshot event to fade out bgm
            _muteBGM_SnapShot.RaiseEvent(_fadeDuration);

            yield return _fadeWaitForSeconds;

            //Swap bgm clip after x duration
            TryStopAudioTimerCo();
            _audioSource.Stop();

            PlayNormalAudio(clip, volume, oldTimeToStartFrom);

            //Use audiosnapshot event to fade back to normal
            _normalSnapShot.RaiseEvent(_fadeDuration);
        }
        #endregion

        #endregion

    }

}