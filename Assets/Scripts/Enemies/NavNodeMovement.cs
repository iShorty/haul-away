using UnityEngine;
using UnityEngine.AI;

    
    // Double layered nodemovement. This layer handles nav mesh pathfinding data, nodeMovement layer handles movement from point to point.

    // 1. Get path to the target.
    // 2. Get NavNodes from path between current node and next node in PatrolNodes.
    // 3. ???
    // 4. Profit. (Add behaviours in derived scripts, adapt as needed.)


[RequireComponent(typeof(NavMeshAgent))]
public class NavNodeMovement : NodeMovement {

    [Header("===== NAV NODE MOVEMENT INFO =====")]
    [ReadOnly] public int _NavNodeStep = 1;
    public NavMeshPath _CurrentPath { get; protected set; } = default;
    public int _navNodeIndex = 0; // should be protected
    [SerializeField] protected NavMeshAgent _NavMeshAgent;
    protected NavMeshHit _navHit = default;
    public Vector3[] _navNodes; // should be protected
    protected Vector3 targetPos = Vector3.positiveInfinity;
    public System.Action EndOfNavPath;


    
    public override void GameAwake() {
        _CurrentPath = new NavMeshPath();
        _NavMeshAgent.enabled = false;

        base.GameAwake();
        // SetNodes done in base.gameawake if using that
    	// SetDestinationToClosestNode(_nodes);done in base.gameawake if using that
        // if(_CurrentDestination == transform.position) {
        //     // Debug.Log("_CurrentDestination is tranform.pos, what now chump " + gameObject.name, gameObject);
        //     SetNavNodes(new Vector3[0]);
        //     return;
        // }
        // CalculatePathToTarget(_CurrentDestination);

        // SetNavNodes(_CurrentPath.corners);
        // SetDestinationToClosestNode(ref _navNodes, ref _navNodeIndex);
    }
    
    ///<Summary>_navNodes = input Array.</Summary>
    public void SetNavNodes(Vector3[] inputArray) {
        _navNodes = inputArray;
        // _navNodes = new Vector3[inputArray.Length];
        // inputArray.CopyTo(_navNodes, 0);
    }

    // at end of path, we need to either iterate through patrol nodes (standard movement) or 
    // not (end of path and we are tracking the player/attacking)

    ///<Summary>SetNextDestination on the nav path.</Summary>
    public void NextNavNode() {
        // Debug.Log("NextNavNode, _navNodes " + _navNodes.Length + " _navNodeIndex: " + _navNodeIndex + " _NavStep: " + _NavNodeStep);
        SetNextDestination(_navNodes, ref _navNodeIndex, ref _NavNodeStep);
    }
    
    ///<Summary>SetNextDestination on the vector path.</Summary>
    public void NextNode() { // Path length != navmesh generated path. 
        // Debug.Log("NextPatrolNode, _nodes " + _nodes.Length + " _NodeIndex: " + _NodeIndex + " _NodeStep: " + _NodeStep);
        SetNextDestination(_nodes, ref _NodeIndex, ref _NodeStep);
    }

    // public void EndOfNodePath() {
    //     Debug.Log("endofnodepath ");
    //     UpdateNodeStep();
    // }

    ///<Summary>Set the next nav path point, calculate a new vector path to it, then set the curr Destination to that</Summary>
    protected void SetNextNavAndNodeDestination() {
        NextNode(); // Jump _CurrDest to next patrol
        CalculatePathToTarget(_CurrentDestination);

        SetNavNodes(_CurrentPath.corners);
        SetDestinationToClosestNode(ref _navNodes, ref _navNodeIndex);
    }

    ///<Summary>Iterate through navNodes, then nodes. Override to adapt to behaviours.</Summary>
    public virtual void NextPathNode(Vector3 node) {
        // If using a target, dont update PatrolNode, dont calc and set.
        // Calc path to target instead and set Navs and new dest.
        if(node.x != Mathf.Infinity) {
            CalculatePathToTarget(node);

            SetNavNodes(_CurrentPath.corners);
            SetDestinationToClosestNode(ref _navNodes, ref _navNodeIndex);
        }
        // If destination is ever infinity, use this
        else {
            if(_navNodes.Length == 0 || _navNodeIndex == _navNodes.Length - 1) {
                SetNextNavAndNodeDestination();
            }
            else {
                NextNavNode();
            }
        }
    }

    ///<Summary>Get path to target.</Summary>
    public void CalculatePathToTarget(Vector3 target) {
        if(_NavMeshAgent == null) return;
        
        else if(_NavMeshAgent.destination == target) return;

        _NavMeshAgent.enabled = true;

        if(_NavMeshAgent.CalculatePath(target, _CurrentPath)) {
            // Debug.Log("Path calculation success. Current path set.");
            _NavMeshAgent.path = _CurrentPath;
        }
        else {
            Debug.LogWarning("Path calculation failed. Default invoked- move to current pos");
            _NavMeshAgent.CalculatePath(transform.position, _CurrentPath);
            _NavMeshAgent.path = _CurrentPath; // Unnecessary, probably
        }
        _NavMeshAgent.enabled = false;
    }

	///<summary>Try to snap the position to the NavMesh, then return it.</summary>
    public virtual Vector3 TrySetNavPos(Vector3 position, float maxDist) {
        Vector3 navPos = GameUtils.GetNearestNavPos(position, _navHit, maxDist);
        if(navPos.x == Mathf.Infinity) {
            // Debug.LogWarning("NavPos was infinity.");
            return Vector3.positiveInfinity;
        }
        return navPos;
    }


#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(_CurrentDestination, 0.3f);

        Gizmos.color = Color.yellow;
		if(_nodes == null || _nodes.Length <= 0) {
			return;
		}
        for (int i = 0; i < _nodes.Length; i++) {
            int nextIndex = i + 1;
            if(nextIndex < _nodes.Length)
                Gizmos.DrawLine(_nodes[i], _nodes[nextIndex]);
            Gizmos.DrawSphere(_nodes[i], 0.2f);
        }

        Gizmos.color = Color.green;
		if(_navNodes == null || _navNodes.Length <= 0) {
			return;
		}
        for (int i = 0; i < _navNodes.Length; i++) {
            int nextIndex = i + 1;
            if(nextIndex < _navNodes.Length)
                Gizmos.DrawLine(_navNodes[i], _navNodes[nextIndex]);
            Gizmos.DrawSphere(_navNodes[i], 0.2f);
        }
        // if(_navNodeIndex < _navNodes.Length) {
        //     Gizmos.color = Color.black;
        //     Gizmos.DrawSphere(_navNodes[_navNodeIndex], .2f);
        // }
    }
#endif

}
