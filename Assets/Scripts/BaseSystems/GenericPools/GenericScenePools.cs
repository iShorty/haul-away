using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GenericScenePools<PooledObject, T> : GenericPools<PooledObject, T>
where PooledObject : Component
where T : GenericPools<PooledObject, T>
{
    Scene m_SceneContainer = default;


    public override void SetUpPools()
    {
        

        string sceneName = GetType().Name + " Scene Pool";

#if UNITY_EDITOR
        m_SceneContainer = SceneManager.GetSceneByName(sceneName);

        //In editor, an extra scene is created before hand
        if (m_SceneContainer.isLoaded)
            return;
#endif

        m_SceneContainer = SceneManager.CreateScene(sceneName);
        base.SetUpPools();
    }

    protected override PooledObject CreateInstance(PooledObject prefab)
    {
        PooledObject o = Instantiate(prefab);
        o.gameObject.SetActive(false);
        SceneManager.MoveGameObjectToScene(o.gameObject, m_SceneContainer);
        return o;
    }

    public override void ReturnInstance(GameObject prefab, PooledObject o)
    {
#if UNITY_EDITOR
        Debug.Assert(o, "Object to return to " + this.GetType().Name + " is null!", this);
#endif

        o.gameObject.SetActive(false);

        o.transform.SetParent(null);
        if (o.gameObject.scene != m_SceneContainer)
            SceneManager.MoveGameObjectToScene(o.gameObject, m_SceneContainer);

        m_PoolDictionary[prefab].Add(o);
    }

    public void ReturnInstanceToContainerScene(PooledObject o)
    {
        o.transform.SetParent(null);
        SceneManager.MoveGameObjectToScene(o.gameObject, m_SceneContainer);
    }

    #region Get
    public override PooledObject GetInstance(GameObject originalPrefab, Transform parent)
    {
        PooledObject o = GetInstance(originalPrefab);

        if (parent.gameObject.scene != m_SceneContainer)
            SceneManager.MoveGameObjectToScene(o.gameObject, parent.gameObject.scene);

        o.transform.SetParent(parent);
        return o;
    }

    public override PooledObject GetInstance(GameObject originalPrefab, Transform parent, Vector3 localPosition)
    {
        PooledObject o = GetInstance(originalPrefab);

        if (parent.gameObject.scene != m_SceneContainer)
            SceneManager.MoveGameObjectToScene(o.gameObject, parent.gameObject.scene);

        o.transform.SetParent(parent);
        o.transform.localPosition = localPosition;
        return o;
    }

    #endregion

}
