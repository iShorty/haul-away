using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public partial class PathManager : GenericPools<MovementPath, PathManager>, IGlobalEventManager
{
    [SerializeField, Header("----- Manager Settings -----")]
    private int _executionPriority = 0;
    public int ExecutionOrder => _executionPriority;

#if UNITY_EDITOR
    [Header("----- RunTime -----")]
    [SerializeField]
#endif
    List<MovementPath> paths = default;
    public List<MovementPath> _NeedsUsers;
    public static event System.Action FindPathForUsers = null;
    public static event System.Action StopFindingUsers = null;



    
    // [SerializeField]
    // List<MovementPath> _cachedPrebuiltPaths;
    // Queue<MovementPath> _cachedPrebuiltPaths; // maybe

    public static PathManager Instance => instance;



    public void GameAwake()
    {
        paths = new List<MovementPath>();

        // GlobalEvents.OnGameUpdate_DURINGGAME += DuringGameUpdate;
        // GlobalEvents.OnGameFixedUpdate_DURINGGAME += DuringGameFixedUpdate;
        GlobalEvents.OnGameReset += ResetGame;
    }

    // public MovementPath GetCachedPath() {
    //     if (_cachedPrebuiltPaths == null || _cachedPrebuiltPaths.Count <= 0)
    //     {
    //         Debug.Log("no cached paths or list is null?", this);
    //         return null;
    //     }
    //     MovementPath currentFirst = _cachedPrebuiltPaths[0];
    //     _cachedPrebuiltPaths.RemoveAt(0);
    //     // MovementPath currentFirst = _cachedPrebuiltPaths.Dequeue();
    //     return currentFirst;
    // }

    // public void CachePath(MovementPath path) {
    //     if (_cachedPrebuiltPaths == null)
    //     {
    //         Debug.Log("list is null?", this);
    //         return;
    //     }
    //     _cachedPrebuiltPaths.Add(path);
    //     // _cachedPrebuiltPaths.Enqueue(path);
    // }
    
    #region Static Fields/Methods

    // // Only if paths need their own update, which should be a resounding no.
    // public static void RegisterPath(PatrolPath path)
    // {
    //     instance.paths.Add(path);
    // }

    #endregion

    public  void OnDestroy()
    {
        // base.OnDestroy();
        GlobalEvents.OnGameReset -= ResetGame;

    }

    #region Update Handles

    // If paths need their own updates, or i need to iterate through em.
    void Update()
    {
        if(_NeedsUsers.Count <= 0) {
            return;
        }
        
        for (int i = _NeedsUsers.Count - 1; i >= 0; i--)
        {
            _NeedsUsers[i].GameUpdate();
        }
    }

    // void DuringGameFixedUpdate()
    // {
    //     for (int i = _NeedsUsers.Count - 1; i >= 0; i--)
    //     {
    //         _NeedsUsers[i].GameUpdate();
    //     }
    // }

    #endregion

    
    public void RegisterPath(MovementPath path)
    {
        _NeedsUsers.Add(path);
    }

    public void UnRegisterPath(MovementPath path)
    {
        _NeedsUsers.Remove(path);
    }

    #region One-Frame Events Handle

    private void ResetGame()
    {
        //Add whatever reseting of values or clean up code u want when lvl is reseting
        
    }

    #endregion

}
