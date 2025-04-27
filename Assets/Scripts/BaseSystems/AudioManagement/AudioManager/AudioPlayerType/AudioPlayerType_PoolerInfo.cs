namespace AudioManagement
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;
    [CreateAssetMenu(fileName = nameof(AudioPlayerType_PoolerInfo), menuName = AudioManager.CREATEASSETMENU_AUDIOMANAGER + "/" + nameof(AudioPlayerType_PoolerInfo))]
    ///<Summary>Holds all of the different kinds of audio players which could be used and pooled by the AudioManager. Refer to AudioPlayerType file to know what AudioPlayers are</Summary>
    public class AudioPlayerType_PoolerInfo : PoolerInfo
    {

#if UNITY_EDITOR

        [Header("----- Resource Path -----")]
        [SerializeField]
        string Path = default;

        protected override void OnValidate()
        {
            base.OnValidate();

            if (!_triggerOnValidate) return;

            //Load all objectpooler infos in a resources location
            PooledObjectInfos = Resources.LoadAll<PooledObjectInfo>(Path);

            //Then, sort them according to a enum's value
            PooledObjectInfos = EnumBasedArrayExtension.SortToEnumOrder<PooledObjectInfo, AudioPlayerType>(PooledObjectInfos, FindMatchingPooledObjectInfo);


            // #region  --------------- Description ------------------
            // //Overview of what is happening here:
            // //I copy all of the current cached pooledobjectinfos into a temp list as AudioPlayerType_PooledObjectInfo type 

            // //then, i sort the list in an ascending order according to the AudioPlayerType_PooledObjectInfo's PlayerType int field 
            // //After sorting, i resetted the PooledObjectInfos array into a new array with the correct number of elements (the enum names.length will give the correct length that is supposed to be there)

            // //Then i check the sorted list against the enum type AudioPlayerType 
            // //For every enum value, i check if the sorted list's elements had a PlayerType int field which matched the enum value
            // //If there is, then set the PooledObjectInfos[enum value] (rmb since the enum defined will be zero based and not -ve) to the found reference
            // //Else if there is not set the PooledObjectInfos[enum value] to null and log a warning



            // //Reason why i needed to do this:
            // //I wanted to ensure that when this pooler info always has its PooledObjectInfos to be sorted according to a enum's value.This is because in the AudioManager, i have a method to which i want to get the correct gameobject prefab by simply inputting the AudioPlayerType value.
            // #endregion


            // List<PooledObjectInfo> sortedCurrentInfos = new List<PooledObjectInfo>();
            // #region ------------- Populating Temp Lists --------------------
            // sortedCurrentInfos.AddRange(PooledObjectInfos);

            // //Sort the list
            // sortedCurrentInfos.Sort(ComparePlayerType);
            // #endregion

            // #region  ------------------ Detectiong new Enum types or Unassigned Enum Types ------------------
            // var enumValues = Enum.GetValues(typeof(AudioPlayerType));

            // //Then, set pooledobjectinfos to the correct size
            // PooledObjectInfos = new PooledObjectInfo[enumValues.Length];

            // foreach (int enumValueAsInt in enumValues)
            // {
            //     int indexOfPooledObject = sortedCurrentInfos.FindIndex(x => (int)x.Prefab.GetComponent<BasicAudioPlayer>().Type == enumValueAsInt);
            //     string enumValueName = Enum.GetName(typeof(AudioPlayerType), enumValueAsInt);
            //     PooledObjectInfo reference;

            //     //If such pooled object doenst exist, 
            //     if (indexOfPooledObject < 0)
            //     {
            //         Debug.LogWarning($"The pooledObject for the enum {nameof(AudioPlayerType)}'s {enumValueName} is not assigned in the {nameof(AudioPlayerType_PoolerInfo)} {name}!", this);
            //         reference = null;
            //     }
            //     else
            //     {
            //         //Else if it does,
            //         reference = sortedCurrentInfos[indexOfPooledObject];
            //         sortedCurrentInfos.RemoveAt(indexOfPooledObject);
            //     }

            //     PooledObjectInfos[enumValueAsInt] = reference;
            // }

            // #endregion
        }

        private Predicate<PooledObjectInfo> FindMatchingPooledObjectInfo(string arg1, int arg2)
        {
            return (PooledObjectInfo info) =>
            {
                BasicAudioPlayer playerPrefab = info.Prefab.GetComponent<BasicAudioPlayer>();
                return (int)playerPrefab.Type == arg2;
            };
        }

        // int ComparePlayerType(PooledObjectInfo a, PooledObjectInfo b)
        // {
        //     int playerAType = (int)a.Prefab.GetComponent<BasicAudioPlayer>().Type;
        //     int playerBType = (int)b.Prefab.GetComponent<BasicAudioPlayer>().Type;

        //     if (playerAType == playerBType)
        //     {
        //         Debug.LogError($"{a.name} and {b.name} share the same enum values! There should not be two same enum values", a);
        //         Debug.LogError($"{a.name} and {b.name} share the same enum values! There should not be two same enum values", b);
        //         return 0;
        //     }
        //     else if (playerAType > playerBType)
        //         return 1;
        //     else
        //         return -1;
        // }


#endif

    }
}