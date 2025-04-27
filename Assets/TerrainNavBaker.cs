using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// [DefaultExecutionOrder(-102)]
public class TerrainNavBaker : MonoBehaviour, IGlobalEventManager {
    [SerializeField]
    NavMeshSurface _terrain = default;
    [SerializeField]
    public int ExecutionOrder { get; }
    // public int ExecutionPriority => _executionOrder;

    bool _runUpdate = false, _runUpdateLastState = false;
    


    #region IUpdateGlobalEventManager Methods
    
    public void GameAwake()
    {
        
    }
    
    private void OnEnable() 
    {
        BakeTerrainNavMesh();
    }

    private void OnDisable() 
    {
        
    }

    // public void GameSceneEnter()
    // {
        
    // }

    public void GameStart()
    {
        _runUpdate = true;
    }

    // public void GameUpdate()
    // {
    //     if(_runUpdate == false) return;

    // }

    public void GamePause()
    {
        _runUpdateLastState = _runUpdate;
        _runUpdate = false;
    }
    
    public void GameResume()
    {
        _runUpdate = _runUpdateLastState;
    }

    public void GameReset()
    {
        _runUpdate = false;
    }

    public void GameExitScene()
    {
        _runUpdate = false;
    }
    
    // public void GameQuit()
    // {
        
    // }

    public void OnDestroy() { }
    #endregion


    public void BakeTerrainNavMesh() 
    {
        // Debug.Log("BakeTerrainNavMesh called");
        _runUpdate = true;
        // Debug.Log("BakeTerrainNavMesh layer: " + _terrain.layerMask.value + " ");
        _terrain.BuildNavMesh();
    }

    public void ClearBakedNavMesh() 
    {
        Debug.Log("ClearBakedNavMesh ");
        _runUpdate = false;
        _terrain.RemoveData();
    }


}

