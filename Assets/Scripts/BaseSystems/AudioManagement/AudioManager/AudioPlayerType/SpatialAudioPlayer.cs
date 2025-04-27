namespace AudioManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    ///<Summary>A basic 3D AudioPlayer. The 3D AudioPlayer doesnt have any clips counter because unlike 2D players, the 3D will have to be moved to a position in world space. Thus, it wont be spammed to do oneshot audio too much (i hope). However, the IsAvailable will be set to immediately true and false on JobDone and JobRequested. To combat this, I made the 3D events return this instance of Player and give the event Raise method the option to force this player to return to pool automatically or not</Summary>
    public class SpatialAudioPlayer : BasicAudioPlayer
    {

        protected bool _isAutoReturning = true;

        protected override void Awake()
        {
            base.Awake();
            _isAutoReturning = true;
#if UNITY_EDITOR
            Debug.Assert(_audioSource.spatialBlend > 0, $"The AudioPlayer {name} doesnt have its spatialblend set to more than 1 despite being a Spatial Audio Player!", this);
#endif
        }

        public override void PlayAudioAtLocation(AudioClip clip, Vector3 worldPosition, bool isOneShot, bool autoReturn)
        {
            _isAutoReturning = autoReturn;
            base.PlayAudioAtLocation(clip, worldPosition, isOneShot, autoReturn);
        }

        public override void PlayAudioAtLocation(AudioClip clip, Vector3 worldPosition, float volumeScale, bool isOneShot, bool autoReturn)
        {
            _isAutoReturning = autoReturn;
            base.PlayAudioAtLocation(clip, worldPosition, volumeScale, isOneShot, autoReturn);
        }

        public override void PlayAudioFollow(AudioClip clip, Transform target, bool isOneShot, bool autoReturn)
        {
            _isAutoReturning = autoReturn;
            base.PlayAudioFollow(clip, target, isOneShot, autoReturn);
        }

        public override void PlayAudioFollow(AudioClip clip, Transform target, float volumeScale, bool isOneShot, bool autoReturn)
        {
            _isAutoReturning = autoReturn;
            base.PlayAudioFollow(clip, target, volumeScale, isOneShot, autoReturn);
        }

        public override void JobDone()
        {
            if (_isAutoReturning)
            {
                IsAvailable = true;
                AudioManager.ReturnInstanceOf(_type, this);
            }
        }

    }

}