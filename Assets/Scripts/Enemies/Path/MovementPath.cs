using System.Collections.Generic;
using UnityEngine;

public enum PathType { Stationary, BackAndForth, Cyclic, Roaming, WithinCircle, SingleUse }

public class MovementPath : MonoBehaviour {
    // public PathInfo pathInfo;
    public GameObject _ActiveUser;
    [SerializeField]
    float _UserSearchDelay = 20f;
    float _searchTime = Mathf.Infinity;
    public bool _SearchingForUser = false;
    public PathType _PathType;
    [Tooltip("The nodes making up the path")]
    public List<Transform> pathNodes = new List<Transform>();
    [SerializeField]
    GameObject enemyPrefab;


    // So I dont have to add them via inspector or adjust their y pos
    private void OnValidate() {
        Vector3 pos = transform.position;
        pos.y = 0;
        transform.position = pos;

        pathNodes.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);
            pos =  t.position;
            pos.y = 0;
            t.position = pos;
            pathNodes.Add(t);
        }
    }

    public void UserDeath() {
        _ActiveUser = null;
        _searchTime = Time.time;
        PathManager.Instance.RegisterPath(this);
    }

    void GetUser() {
        Enemy e = EnemyManager.GetInstanceOf(enemyPrefab, FloatableProp.PropState.ONLAND, transform.position);
    }

    
    public void GameUpdate()
    {
        if(_searchTime + _UserSearchDelay < Time.time) {
            _searchTime = Mathf.Infinity;
            //  get a new unit.
            GetUser();
            PathManager.Instance.UnRegisterPath(this);
        }
    }

    private void Start()
    {
        if(pathNodes.Count == 1) {
            if(_PathType != PathType.Stationary)
                Debug.LogWarning("Path type is not stationary, only 1 path node set. Check nodes", gameObject);
            _PathType = PathType.Stationary;
        }
        else if(pathNodes.Count <= 0)
            Debug.Log("No nodes set for this path.", gameObject);
    }

    public MovementPath(PathType type, List<Transform> pathNodes)
    {
        this.pathNodes = pathNodes;
        this._PathType = type;
    }

    public float GetDistanceToNode(Vector3 origin, int destinationNodeIndex)
    {
        if(destinationNodeIndex < 0 || destinationNodeIndex >= pathNodes.Count || pathNodes[destinationNodeIndex] == null)
        {
            return -1f;
        }

        return (pathNodes[destinationNodeIndex].position - origin).magnitude;
    }

    public Vector3 GetPositionOfPathNode(int NodeIndex)
    {
        if (NodeIndex < 0 || NodeIndex >= pathNodes.Count || pathNodes[NodeIndex] == null)
        {
            return Vector3.zero;
        }

        return pathNodes[NodeIndex].position;
    }



#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < pathNodes.Count; i++)
        {
            if(i + 1 < pathNodes.Count) {
                int nextIndex = i + 1;

                Gizmos.DrawLine(pathNodes[i].position, pathNodes[nextIndex].position);
                Gizmos.DrawSphere(pathNodes[i].position, 0.1f);
            }
        }
    }
#endif


}
