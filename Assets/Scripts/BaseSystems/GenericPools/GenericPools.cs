using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericPools<PooledObject, Pooler> : BaseGenericPool
    where PooledObject : Component
    where Pooler : GenericPools<PooledObject, Pooler>
{
    [Header("Pool Info"), SerializeField]
    ///<Summary>The settings hold all of the information about what this pooler is going to instantiate</Summary>
    protected PoolerInfo m_Settings = default;

    //The number of instances which will be instantiated if a prefab which does not belong to the settings is asked to be created
   protected const int k_DefaultCount = 5;

    protected Dictionary<GameObject, List<PooledObject>> m_PoolDictionary = default;

    protected static Pooler instance { get; set; } = null;

    #region ------------ Setup Methods -----------------
    protected void SetUpStaticInstance()
    {
#if UNITY_EDITOR
        if (instance != null)
        {
            if (instance == this)
                Debug.LogError("Instance for " + typeof(Pooler) + " has already been established. Leak found.");
            else
                Debug.LogError("Instance for " + typeof(Pooler) + " has already been established. Duplicate found.");
        }
#endif

        instance = (Pooler)this;
    }

    public override void SetUpPools()
    {
        SetUpStaticInstance();
        PooledObjectInfo[] pooledObjectsInfos = m_Settings.PooledObjectInfos;

        m_PoolDictionary = new Dictionary<GameObject, List<PooledObject>>(pooledObjectsInfos.Length);

        foreach (var objectInfo in pooledObjectsInfos)
        {
            //Create keys
            m_PoolDictionary.Add(objectInfo.Prefab.gameObject, new List<PooledObject>());
            CreatePool(objectInfo.Prefab.GetComponent<PooledObject>(), objectInfo.Count);
        }


    }

    protected virtual void CreatePool(PooledObject prefab, int count)
    {
        PooledObject o;
        for (int i = 0; i < count; i++)
        {
            o = CreateInstance(prefab);
            o.name += " " + i;
            m_PoolDictionary[prefab.gameObject].Add(o);
        }
    }

    protected virtual PooledObject CreateInstance(PooledObject prefab)
    {
        PooledObject o = Instantiate(prefab);
        o.gameObject.SetActive(false);
        o.transform.SetParent(transform);
        return o;
    }


    #endregion

    #region ------------------- Get Methods ------------------

    public virtual PooledObject GetInstance(GameObject originalPrefab)
    {

#if UNITY_EDITOR
        Debug.Assert(originalPrefab, "Prefab to Get from " + this.GetType().Name + " is null!", this);
#endif

        PooledObject o;

        //Check if prefab's pool was created
        if (!m_PoolDictionary.ContainsKey(originalPrefab))
        {
#if UNITY_EDITOR
            Debug.LogWarning("The prefab " + originalPrefab + " does not have a pool setup!", this);
#endif
            //create pool
            CreatePool(originalPrefab.GetComponent<PooledObject>(), k_DefaultCount);
        }


        int lastIndex = m_PoolDictionary[originalPrefab].Count - 1;

        if (lastIndex >= 0)
        {
            o = m_PoolDictionary[originalPrefab][lastIndex];

#if UNITY_EDITOR
            Debug.Assert(o, "Pool Array's element at index " + lastIndex + " is null! PooledObject must have been destroyed externally!", this);
#endif

            m_PoolDictionary[originalPrefab].RemoveAt(lastIndex);
        }
        //Else create instance if last index is -ve
        else
        {
#if UNITY_EDITOR
            Debug.Log(name + " ran out of " + originalPrefab.name + " instances and is creating more of it. Please adjust PoolSettings for said prefab.", this);
#endif

            o = CreateInstance(originalPrefab.GetComponent<PooledObject>());
        }


        o.gameObject.SetActive(true);
        return o;
    }

    public virtual PooledObject GetInstance(GameObject originalPrefab, Vector3 worldPosition)
    {
        PooledObject o = GetInstance(originalPrefab);
        o.transform.position = worldPosition;
        return o;
    }

    public virtual PooledObject GetInstance(GameObject originalPrefab, Vector3 worldPosition, Quaternion rotation)
    {
        PooledObject o = GetInstance(originalPrefab);
        o.transform.position = worldPosition;
        o.transform.rotation = rotation;
        return o;
    }

    public virtual PooledObject GetInstance(GameObject originalPrefab, Transform parent)
    {
        PooledObject o = GetInstance(originalPrefab);
        o.transform.SetParent(parent);
        return o;
    }

    public virtual PooledObject GetInstance(GameObject originalPrefab, Transform parent, Vector3 localPosition)
    {
        PooledObject o = GetInstance(originalPrefab);
        o.transform.SetParent(parent);
        o.transform.localPosition = localPosition;
        return o;
    }
    
    public virtual PooledObject GetInstance(GameObject originalPrefab, Transform parent, Vector3 localPosition, Quaternion localRotation)
    {
        PooledObject o = GetInstance(originalPrefab);
        o.transform.SetParent(parent);
        o.transform.localPosition = localPosition;
        o.transform.localRotation = localRotation;
        return o;
    }

    public static PooledObject GetInstanceOf(GameObject originalPrefab)
    {
        return instance.GetInstance(originalPrefab);
    }

    public static PooledObject GetInstanceOf(GameObject originalPrefab, Vector3 worldPosition)
    {
        return instance.GetInstance(originalPrefab, worldPosition);
    }

    public static PooledObject GetInstanceOf(GameObject originalPrefab, Vector3 worldPosition, Quaternion rotation)
    {
        return instance.GetInstance(originalPrefab, worldPosition, rotation);
    }

    public static PooledObject GetInstanceOf(GameObject originalPrefab, Transform parent)
    {
        return instance.GetInstance(originalPrefab, parent);
    }

    public static PooledObject GetInstanceOf(GameObject originalPrefab, Transform parent, Vector3 localPosition)
    {
        return instance.GetInstance(originalPrefab, parent, localPosition);
    }

    public static PooledObject GetInstanceOf(GameObject originalPrefab, Transform parent, Vector3 localPosition, Quaternion localRotation)
    {
        return instance.GetInstance(originalPrefab, parent, localPosition, localRotation);
    }

    #endregion

    /// <summary>
    /// Returns an instance of a prefab to its respective pool.
    /// </summary>
    /// <param name="prefab"> The original prefab the instance is instantiated from. </param>
    /// <param name="o"> The instance of the prefab. </param>
    public virtual void ReturnInstance(GameObject prefab, PooledObject o)
    {
#if UNITY_EDITOR
        Debug.Assert(o, "Object to return to " + this.GetType().Name + " is null!", this);
#endif

        o.gameObject.SetActive(false);
        o.transform.SetParent(transform);
        m_PoolDictionary[prefab].Add(o);
    }

    public static void ReturnInstanceOf(GameObject prefab, PooledObject o)
    {
        instance.ReturnInstance(prefab, o);
    }



}
