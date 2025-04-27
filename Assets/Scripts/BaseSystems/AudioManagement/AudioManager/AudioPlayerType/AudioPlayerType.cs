namespace AudioManagement
{
    ///<Summary>Audio players are BaseAudioPlayerType instances (script which inherits from monobehaviour and requires a AudioSource on them) which are used to play out special audio behaviours. (Eg. We want a audiosource to follow someone. Thats a SPATIAL_FOLLOW typed audioplayer) To add new types, please add them into the same folder as what the AudioPlayerTypeInfo's path is in, in the Resources folder</Summary>
    public enum AudioPlayerType
    {
        ///<Summary>Plays the audio clip using a two dimensional audio source on the 2DSFX output group. The clip is flat and has no sense of direction nor distance to it.</Summary>
        SFX_TWO_DIMENISIONAL
        ,
        ///<Summary>Plays the audio clip using a two dimensional audio source on the BGM output group. The clip is flat and has no sense of direction nor distance to it.</Summary>
        BGM_TWO_DIMENISIONAL
        ,
        ///<Summary>Plays the audio clip using a three dimensional audio source. The clip will be played at a position in world space and will remain there until the audio clip has finished playing</Summary>
        SPATIAL_AT_LOCATION
        ,
        ///<Summary>Plays the audio clip using a three dimensional audio source. The clip will be set as a child of a transform and play until the audio clip has finished playing</Summary>
        SPATIAL_FOLLOW
    }

}