// using UnityEngine.Audio;
// using System.Collections;
// using UnityEngine;
// using System;

// public class AudioManager : MonoBehaviour
// {
//     public AudioMixer mainMixer;
//     public static AudioManager theAM;
//     public Sound[] sounds;
//     public Sound[] oceanSFX;
//     int currentOceanSFX;
//     public Sound[] bgMusic;
//     private Sound currentSFX;
//     public bool musicMute, sfxMute;

//     private void Awake() {
//         InitializeAudioManager();
//         PlayMusic("The Sailors Waltz");
//     }


//     public void InitializeAudioManager()
//     {
//         theAM = this;
//         LoadSounds(list: sounds);
//         LoadSounds(list: oceanSFX);
//         LoadSounds(list: bgMusic);
//         currentOceanSFX = oceanSFX.Length - 1;
//     }
//     private void LoadSounds(Sound sound = null, Sound[] list = null)
//     {
//         if (sound != null)
//         {
//             sound.source = gameObject.AddComponent<AudioSource>();
//             sound.source.clip = sound.clip;
//             sound.source.outputAudioMixerGroup = sound.output;
//             sound.source.volume = sound.volume;
//             sound.source.pitch = sound.pitch;
//             sound.source.loop = sound.loop;
//         }
//         else if (list != null)
//         {
//             foreach (Sound s in list)
//             {
//                 s.source = gameObject.AddComponent<AudioSource>();
//                 s.source.clip = s.clip;
//                 s.source.outputAudioMixerGroup = s.output;
//                 s.source.volume = s.volume;
//                 s.source.pitch = s.pitch;
//                 s.source.loop = s.loop;
//             }
//         }
//     }

//     void ShuffleOceanSFX()
//     {
//         for (int i = 0; i < oceanSFX.Length; i++) {
//             if(i + 1 < oceanSFX.Length)
//             {
//                 int randIndex = UnityEngine.Random.Range(i + 1, oceanSFX.Length - 1);
//                 Sound temp = oceanSFX[randIndex];
//                 oceanSFX[randIndex] = oceanSFX[i];
//                 oceanSFX[i] = temp;
//             }
//         }
//     }

//     public void PlayOceanSFX()
//     {
//         currentSFX = oceanSFX[currentOceanSFX];
//         currentOceanSFX--;
//         if(currentOceanSFX < 0)
//         {
//             ShuffleOceanSFX();
//             currentOceanSFX = oceanSFX.Length - 1;
//         }
//         // #if UNITY_EDITOR
//         // Debug.Log("Playoceansfx" + Time.time);
//         // #endif
//         currentSFX.source.Play();
//     }

//     public void PlaySFX(string name)
//     {
//         currentSFX = Array.Find(sounds, sound => sound.name == name);
//         currentSFX.source.Play();
//     }

//     public void PlayMusic(string name)
//     {
//         Sound s = Array.Find(bgMusic, sound => sound.name == name);
//         if (!s.source.isPlaying)
//         {
//             s.source.Play();
//         }
//     }

//     public void StopMusic(string name)
//     {
//         Sound s = Array.Find(bgMusic, sound => sound.name == name);
//         if (s.source.isPlaying)
//         {
//             s.source.Stop();
//         }
//     }
    
//     #region Mixer Controls
//     public void SetSFXVol(float volume)
//     {
//         mainMixer.SetFloat("sfxVol", volume);
//         if(volume == -80f)
//         {
//             sfxMute = true;
//         }
//         else
//         {
//             sfxMute = false;
//         }
//     }
//     public void SetMusicVol(float volume)
//     {
//         mainMixer.SetFloat("musicVol", volume);
//         if (volume == -80f)
//         {
//             musicMute = true;
//         }
//         else
//         {
//             musicMute = false;
//         }
//     }
//     #endregion
// }
