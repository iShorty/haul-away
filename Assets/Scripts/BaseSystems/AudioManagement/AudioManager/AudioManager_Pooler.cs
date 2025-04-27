namespace AudioManagement
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    //Handles pooling of different baseaudioplayers
    public partial class AudioManager
    {
        void Pooler_Awake()
        {
            SetUpPools();
        }

        #region ----------- SetUp Methods -----------------
        ///<Summary>Audio manager's pooler is a bit special. The returned instance will not be setactive true. Instead all pooled objects are active from the get go. This is to allow audiosources to be able to play all the time (as some audiosources can play more than one clip at multiple different times, a typical pooler code wont suffice)</Summary>
        protected override BasicAudioPlayer CreateInstance(BasicAudioPlayer prefab)
        {
            BasicAudioPlayer o = Instantiate(prefab);
            o.transform.SetParent(transform);
            return o;
        }
        #endregion

        #region --------------- Get Methods --------------
        protected BasicAudioPlayer GetInstanceOf(AudioPlayerType type)
        {
            GameObject prefab = GetAudioPlayerTypePrefab(type);
            return GetInstance(prefab);
        }

        ///<Summary>There will be cases where BaseAudioPlayers can be used more than once by multiple objects hence the m_PoolDictionary's lists will contain only instances of BaseAudioPlayers whose IsAvailable bool is true. Whenever a new instance is requested, the instance will update its availability before being checked if it is still available. If it is, then it stays in the List else it gets removed and only gets returned when its availability is true  </Summary>
        public override BasicAudioPlayer GetInstance(GameObject originalPrefab)
        {

#if UNITY_EDITOR
            Debug.Assert(originalPrefab, "Prefab to Get from " + this.GetType().Name + " is null!", this);
#endif

            //Check if prefab's pool was created
            if (!m_PoolDictionary.ContainsKey(originalPrefab))
            {
#if UNITY_EDITOR
                Debug.LogWarning("The prefab " + originalPrefab + " does not have a pool setup!", this);
#endif
                //create pool
                CreatePool(originalPrefab.GetComponent<BasicAudioPlayer>(), k_DefaultCount);
            }

            return GetAvailablePlayer(originalPrefab);
        }

        ///<Summary>Whenever a new instance is requested, the instance will update its availability before being checked if it is still available. If it is, then it stays in the List else it gets removed and only gets returned when its availability is true  </Summary>
        BasicAudioPlayer GetAvailablePlayer(GameObject originalPrefab)
        {
            BasicAudioPlayer player;
            int lastIndex = m_PoolDictionary[originalPrefab].Count - 1;

            if (lastIndex >= 0)
            {
                player = m_PoolDictionary[originalPrefab][lastIndex];

#if UNITY_EDITOR
                Debug.Assert(player, "Pool Array's element at index " + lastIndex + " is null! PooledObject must have been destroyed externally!", this);
#endif
                //Update the baseaudioplayer's availability because someone is requesting for it
                player.JobRequested();

                //If the player is no longer available,
                if (!player.IsAvailable)
                {
                    m_PoolDictionary[originalPrefab].RemoveAt(lastIndex);
                }
                //Else keep it there so that it can be reused on the next request

            }
            //Else create instance if last index is -ve
            else
            {
#if UNITY_EDITOR
                Debug.Log(name + " ran out of " + originalPrefab.name + " instances and is creating more of it. Please adjust PoolSettings for said prefab.", this);
#endif

                player = CreateInstance(originalPrefab.GetComponent<BasicAudioPlayer>());

                //Update the baseaudioplayer's availability because someone is requesting for it
                player.JobRequested();

                //If the player is still available,
                if (player.IsAvailable)
                {
                    m_PoolDictionary[originalPrefab].Add(player);
                }
            }


            // player.gameObject.SetActive(true);
            return player;
        }

        #endregion

        #region --------------- Return Methods -----------------
        ///<Summary>ReturnInstanceOf should only be called when a BaseAudioPlayer's job is done</Summary>
        public static void ReturnInstanceOf(AudioPlayerType type, BasicAudioPlayer o)
        {
            //Convert type to prefab gameobject 
            GameObject originalPrefab = instance.GetAudioPlayerTypePrefab(type);
            instance.ReturnInstance(originalPrefab, o);
        }

        ///<Summary>Audio manager's pooler is a bit special. The returned instance will not be setactive true. Instead all pooled objects are active from the get go.</Summary>
        public override void ReturnInstance(GameObject prefab, BasicAudioPlayer o)
        {
#if UNITY_EDITOR
            Debug.Assert(o, "Object to return to " + this.GetType().Name + " is null!", this);
#endif

            o.transform.SetParent(transform);
            m_PoolDictionary[prefab].Add(o);
        }

        #endregion

    }

}