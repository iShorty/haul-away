using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManagement;
public static partial class AudioEvents_Ocean
{
    static int _oceanSFX_ArrayCounter = 0;
    static AudioClipType[] oceanSFXAudioClips = new AudioClipType[5] { AudioClipType.SFX_Sea1, AudioClipType.SFX_Sea2, AudioClipType.SFX_Sea3, AudioClipType.SFX_Sea4, AudioClipType.SFX_Sea5 };
    static void ShuffleOceanSFX()
    {
        for (int i = 0; i < oceanSFXAudioClips.Length; i++)
        {
            if (i + 1 < oceanSFXAudioClips.Length)
            {
                int randIndex = UnityEngine.Random.Range(i + 1, oceanSFXAudioClips.Length - 1);
                AudioClipType temp = oceanSFXAudioClips[randIndex];
                oceanSFXAudioClips[randIndex] = oceanSFXAudioClips[i];
                oceanSFXAudioClips[i] = temp;
            }
        }
    }

    public static void PlayOceanSFX()
    {
        AudioClipType currentSFX = oceanSFXAudioClips[_oceanSFX_ArrayCounter];
        _oceanSFX_ArrayCounter--;
        if (_oceanSFX_ArrayCounter < 0)
        {
            ShuffleOceanSFX();
            _oceanSFX_ArrayCounter = oceanSFXAudioClips.Length - 1;
        }
        // #if UNITY_EDITOR
        // Debug.Log("Playoceansfx" + Time.time);
        // #endif
        AudioEvents.RaiseOnPlay2DSFX(currentSFX, 0.2f,true);
    }
}