using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// [DefaultExecutionOrder(-102)]
public class BoatNavBaker : MonoBehaviour, IGlobalEventManager {
    [SerializeField]
    NavMeshSurface m_BoatSurface = default;
    [SerializeField]
    Transform m_TrackedTransform;
    [SerializeField]
    public int ExecutionOrder { get; }
    // public int ExecutionPriority => _executionOrder;
    public GameObject GO => gameObject;

    [SerializeField]
    float _RebakeDist = 0.2f;
    float _SqredRebakeDist = default;
    Vector3 _lastBakedSurfacePos = Vector3.positiveInfinity;
    float _SqredDistBetweenLastPos => (_lastBakedSurfacePos - m_TrackedTransform.position).sqrMagnitude;
    bool _IsFarEnoughAway => _SqredDistBetweenLastPos > _SqredRebakeDist;

    bool _runUpdate = false, _runUpdateLastState = false;
    
    

    // The size of the build bounds = boat size
    // public Vector3 m_Size = new Vector3(80.0f, 20.0f, 80.0f);

    // NavMeshData m_NavMeshData;
    // AsyncOperation m_Operation;
    // NavMeshDataInstance m_Instance;
    // List<NavMeshBuildSource> m_Sources = new List<NavMeshBuildSource>();


    #region IUpdateGlobalEventManager Methods
    
    public void GameAwake()
    {
        m_TrackedTransform = FindObjectOfType<BoatDeck>().gameObject.transform;
        _SqredRebakeDist = _RebakeDist * _RebakeDist;
    }
    
    private void OnEnable() 
    {
        EnemyManager.onAddFirstEnemyBoarder += BakeBoatNavMesh;
        EnemyManager.onRemoveLastEnemyBoarder += ClearBakedBoatNavMesh;
    }

    private void OnDisable() 
    {
        EnemyManager.onAddFirstEnemyBoarder -= BakeBoatNavMesh;
        EnemyManager.onRemoveLastEnemyBoarder -= ClearBakedBoatNavMesh;
    }


    // public void GameSceneEnter()
    // {
        
    // }

    public void GameStart()
    {
        _runUpdate = true;
    }
    public void GameUpdate()
    {
        if(_runUpdate == false) return;

        if(EnemyManager.Instance.numberOfBoardersRemaining > 0 && _IsFarEnoughAway)
        {
            BakeBoatNavMesh();
        }
    }

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


    public void BakeBoatNavMesh() 
    {
        // Debug.Log("BakeBoatNavMesh called");
        _runUpdate = true;
        // Debug.Log("BakeBoatNavMesh layer: " + m_BoatSurface.layerMask.value + " ");
        m_BoatSurface.BuildNavMesh();
        _lastBakedSurfacePos = m_TrackedTransform.position;
    }

    public void ClearBakedBoatNavMesh() 
    {
        Debug.Log("ClearBakedBoatNavMesh");
        _runUpdate = false;
        m_BoatSurface.RemoveData();
        _lastBakedSurfacePos = Vector3.positiveInfinity;
    }

    // void UpdateNavMesh(bool asyncUpdate = false)
    // {
    //     NavMeshSourceTag.Collect(ref m_Sources);
    //     var defaultBuildSettings = NavMesh.GetSettingsByID(0);
    //     var bounds = GetTrackedBounds();

    //     // if (asyncUpdate)
    //     //     m_Operation = NavMeshBuilder.UpdateNavMeshDataAsync(m_NavMesh, defaultBuildSettings, m_Sources, bounds);
    //     // else
    //     NavMeshBuilder.UpdateNavMeshData(m_BoatSurface.navMeshData, m_BoatSurface.GetBuildSettings(), m_Sources, bounds);
    // }
    // Bounds GetTrackedBounds()
    // {
    //     var center = m_TrackedTransform ? m_TrackedTransform.position : transform.position;
    //     return new Bounds(center, m_Size);
    // }

}
