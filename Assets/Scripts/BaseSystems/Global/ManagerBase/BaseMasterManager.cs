using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>The base class where you can host all your proxy update calls using various global events</Summary>
public abstract class BaseMasterManager<T> : LevelSingleton<T>
where T : LevelSingleton<T>
{
    [Header("----- Manager References -----")]
    [Header("===== MASTERGAMEMANAGER =====")]
    [Tooltip("Dump all the managers under one gameobject and rename as container")]
    [SerializeField]
    //Comment out if not used
    protected GameObject _managerContainer = default;

    [Header("----- Manager References -----")]
    [SerializeField]
    [Tooltip("For managers which are outside of the manager container")]
    //Comment out if not used
    protected BaseManager[] _baseManagers = new BaseManager[0];


    ///<Summary>Finds all the Managers from the given sources and calls their GameAwake function to subscribe to events</Summary>
    protected override void GameAwake()
    {
        //Find all managers in the given sources. Doign this with a temp list cause we wont need use them after this
        List<IGlobalEventManager> allManagers = new List<IGlobalEventManager>();

        if (_managerContainer)
        {
            allManagers.AddRange(_managerContainer.GetComponentsInChildren<IGlobalEventManager>());
        }

        if (_baseManagers.Length > 0)
        {
            allManagers.AddRange(_baseManagers);
        }

        allManagers.Sort(ManagerUtility.CompareManagerExecutionPriority);

        //Call all of them to subscribe 
        for (int i = 0; i < allManagers.Count; i++)
        {
            allManagers[i].GameAwake();
        }
    }

    protected abstract void OnDestroy();
    protected abstract void Update();
    protected abstract void FixedUpdate();
    protected abstract void LateUpdate();

}
