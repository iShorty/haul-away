namespace AudioManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    //This file holds all of the overloads which will belong to and be used by future derived classes only
    public abstract partial class BasicAudioPlayer
    {
        #region  ============= Play Audios Methods =================

        ///<Summary>Plays the audio clip by deciding whether the audio is to be played using oneshot or normal method</Summary>
        public virtual void PlayAudio(AudioClip clip, bool isOneShot)
        {
            switch (isOneShot)
            {
                case true:
                    PlayOneShotAudio(clip);
                    break;
                case false:
                    PlayNormalAudio(clip);
                    break;
            }
        }

        ///<Summary>Plays the audio clip using the normal audiosource play method with volume scale</Summary>
        public virtual void PlayAudio(AudioClip clip, float volumeScale, bool isOneShot)
        {
            switch (isOneShot)
            {
                case true:
                    PlayOneShotAudio(clip, volumeScale);
                    break;
                case false:
                    PlayNormalAudio(clip, volumeScale);
                    break;
            }
        }

        #region -------------- Base Methods ------------------
        //These methods are used by both 2D & 3D audio players 

        ///<Summary>Plays the audio clip using oneshot on the audio source. This should be used for SFXs only as sfx wont be paused</Summary>
        public virtual void PlayOneShotAudio(AudioClip clip)
        {
            _audioSource.PlayOneShot(clip, 1);
            StartAudioOneShotTimerCo(clip.length);
        }

        ///<Summary>Plays the audio clip using oneshot on the audio source with volume scale. This should be used for SFXs only as sfx wont be paused</Summary>
        public virtual void PlayOneShotAudio(AudioClip clip, float volumeScale)
        {
            _audioSource.PlayOneShot(clip, volumeScale);
            StartAudioOneShotTimerCo(clip.length);
        }

        ///<Summary>Plays the audio clip using the normal audiosource play method on audio player</Summary>
        public virtual void PlayNormalAudio(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.volume = 1;
            _audioSource.Play();
            StartAudioTimerCo(clip.length);
        }

        ///<Summary>Plays the audio clip using the normal audiosource play method with volume scale</Summary>
        public virtual void PlayNormalAudio(AudioClip clip, float volumeScale)
        {
            _audioSource.volume = volumeScale;
            _audioSource.clip = clip;
            _audioSource.Play();
            StartAudioTimerCo(clip.length);
        }
        #endregion

        #region ------------- BGM Methods ------------------
        ///<Summary>Plays the audio clip using the normal audiosource play method with volume scale</Summary>
        public virtual void PlayBGMAudio(AudioClip clip, BGMAudioPlayer.BGM_PlayType type) { }

        ///<Summary>Plays the audio clip using the normal audiosource play method with volume scale</Summary>
        public virtual void PlayBGMAudio(AudioClip clip, float volumeScale, BGMAudioPlayer.BGM_PlayType type) { }
        #endregion

        #region  ------------- 3D Methods -----------------
        ///<Summary>Plays the audio clip at a world position. If autoReturn is true, the player will count down a timer which is the duration of the clip and automatically return itself to the pooler once the timer is up. Else, the AudioPlayer will be left unavailable and not returned to the pool </Summary>
        //Since this is no longer the base methods (cause i can reuse methods by doing if check here), i wan isOneShot as para here
        public virtual void PlayAudioAtLocation(AudioClip clip, Vector3 worldPosition, bool isOneShot, bool autoReturn)
        {
#if UNITY_EDITOR
            Debug.Assert(_audioSource.spatialBlend > 0, $"The audiosource {_audioSource.name} does not have its spatialBlend set to 3D", _audioSource);
#endif
            transform.position = worldPosition;
            PlayAudio(clip, isOneShot);
        }

        ///<Summary>Plays the audio clip at a world position. If autoReturn is true, the player will count down a timer which is the duration of the clip and automatically return itself to the pooler once the timer is up. Else, the AudioPlayer will be left unavailable and not returned to the pool </Summary>
        public virtual void PlayAudioAtLocation(AudioClip clip, Vector3 worldPosition, float volumeScale, bool isOneShot, bool autoReturn)
        {
#if UNITY_EDITOR
            Debug.Assert(_audioSource.spatialBlend > 0, $"The audiosource {_audioSource.name} does not have its spatialBlend set to 3D", _audioSource);
#endif
            transform.position = worldPosition;
            PlayAudio(clip, volumeScale, isOneShot);
        }

        ///<Summary>Plays the audio clip using the normal audiosource play method as a child of a parent target. If autoReturn is true, the player will count down a timer which is the duration of the clip and automatically return itself to the pooler once the timer is up. Else, the AudioPlayer will be left unavailable and not returned to the pool </Summary>

        ///<Summary></Summary>
        public virtual void PlayAudioFollow(AudioClip clip, Transform target, bool isOneShot, bool autoReturn)
        {
#if UNITY_EDITOR
            Debug.Assert(_audioSource.spatialBlend > 0, $"The audiosource {_audioSource.name} does not have its spatialBlend set to 3D", _audioSource);
#endif
            transform.SetParent(target);
            transform.localPosition = Vector3.zero;
            PlayAudio(clip, isOneShot);
        }

        ///<Summary>Plays the audio clip using the normal audiosource play method at a location.</Summary>
        public virtual void PlayAudioFollow(AudioClip clip, Transform target, float volumeScale, bool isOneShot, bool autoReturn)
        {
#if UNITY_EDITOR
            Debug.Assert(_audioSource.spatialBlend > 0, $"The audiosource {_audioSource.name} does not have its spatialBlend set to 3D", _audioSource);
#endif
            transform.SetParent(target);
            transform.localPosition = Vector3.zero;
            PlayAudio(clip, volumeScale, isOneShot);
        }

        #endregion

        #endregion


        #region  ------------------- Coroutine Methods -----------------
        ///<Summary>Caches and plays the coroutine which wll yield return the audio clip duration. Call this when the derived AudioPlayer script plays its audio</Summary>
        protected virtual void StartAudioTimerCo(float audioClipDuration)
        {
            _mainTimerCo = Co_ClipTimer(audioClipDuration);
            StartCoroutine(_mainTimerCo);
        }

        ///<Summary>Called when this audio player plays audio using the AudioSource.PlayOneShot() </Summary>
        protected void StartAudioOneShotTimerCo(float audioClipDuration)
        {
            IEnumerator coroutine = Co_ClipTimer(audioClipDuration);
            StartCoroutine(coroutine);
        }

        ///<Summary>Stops the audio timer coroutine if there is any playing</Summary>
        protected virtual void TryStopAudioTimerCo()
        {
            if (_mainTimerCo != null)
            {
                StopCoroutine(_mainTimerCo);
                _mainTimerCo = null;
            }
        }


        ///<Summary>Starts counting down a corountine timer, once the timer is done, it is assumed that the BaseAudioPlayer has finished playing its audio. Override and yield return base to execute code after timer is up</Summary>
        protected virtual IEnumerator Co_ClipTimer(float audioClipDuration)
        {
            yield return new WaitForSeconds(audioClipDuration);
            JobDone();
            TryStopAudioTimerCo();
        }

        #endregion

    }

}