using UnityEngine;


// Handle basic logic for node-based movement.
public class NodeMovement : MonoBehaviour {
    [Header("===== NODE MOVEMENT INFO ======")]
	[ReadOnly] public Vector3 _CurrentDestination;
    [ReadOnly] public int _NodeStep = 1;
    protected int _NodeIndex = 0;
    protected Vector3[] _nodes;
    // [SerializeField] protected Transform[] _nodeTransforms;

    public virtual void GameAwake() {
        // Debug.Log("nodemovement gameawake");
        // Vector3[] nodePositions = new Vector3[_nodeTransforms.Length];
        
        // for (int i = 0; i < _nodeTransforms.Length; i++) {
        //     nodePositions[i] = _nodeTransforms[i].position;
        // }

        // SetNodes(nodePositions);
        // SetDestinationToClosestNode(ref _nodes, ref _NodeIndex);
    }
    
    #region Node-based Movement Logic

    ///<Summary>checks if nodeArray.Length > 0</Summary>
    protected bool IsNodeArrayValid(Vector3[] nodeArray) {
        return nodeArray.Length > 0;
    }

    public void SetNodes(Vector3[] inputArray) {
        _nodes = inputArray;
    }

	///<summary>Move to nearest node. Useful at start.</summary>
    public void SetDestinationToClosestNode(ref Vector3[] nodeArray, ref int index) {
        if (IsNodeArrayValid(nodeArray) == false) {
            nodeArray = new Vector3[1];
            nodeArray[0] = transform.position;
            index = 0;
        }
        else {
            int closestPathNodeIndex = 0;
            float closestPathDist = Mathf.Infinity;

            // Optimise this later
            for (int i = 0; i < nodeArray.Length; i++) {
                float sqredDistanceToNode = (gameObject.transform.position - nodeArray[i]).sqrMagnitude;
                // float distanceToPathNode = _patrolPath.GetDistanceToNode(transform.position, i);
                if (sqredDistanceToNode < closestPathDist) {
                    closestPathNodeIndex = i;
                    closestPathDist = sqredDistanceToNode;
                }
            }
            index = closestPathNodeIndex;
        }
        SetCurrentDestination(nodeArray[index]);
    }

	///<summary>Set _CurrentDestination, checks if it isn't the input V3.</summary>
    public void SetCurrentDestination(Vector3 dest) {
        _CurrentDestination = _CurrentDestination == dest ? _CurrentDestination : dest;
    }

	///<summary>UpdateNodeIndex(), then SetCurrentDestination() using args.</summary>
    public void SetNextDestination(Vector3[] nodeArray, ref int index, ref int step) {
        UpdateNodeIndex(nodeArray, ref index, ref step);
        SetCurrentDestination(nodeArray[index]);
    }

    // Update nodeIndex, validate index
    // Adapt to behaviours by overriding.
    // Clamp index to ends of array
	///<summary>index += step, then validate index. Adapt to behaviours by overriding. Default behaviour clamps index to ends of array.</summary>
    public virtual void UpdateNodeIndex(Vector3[] nodeArray, ref int index, ref int step, bool useDefault = false) {
        // Debug.Log("UpdateNodeIndex " + nodeArray.Length + " nodeslength: " + _nodes.Length);
        if(IsNodeArrayValid(nodeArray) == false) {
            Debug.Log(IsNodeArrayValid(nodeArray));
            index = 0;
            return;
        }
        index += step;
        
        if (index < 0) {
            Debug.LogWarning("invalid, less than 0");
            index = 0;
        }
        else if (index >= nodeArray.Length) {
            Debug.LogWarning("invalid, bigger than array size " + nodeArray.Length);
            index = nodeArray.Length - 1;
        }
    }
    
    #endregion



#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
		if(_nodes == null || _nodes.Length <= 0) {
			return;
		}
        for (int i = 0; i < _nodes.Length; i++) {
            int nextIndex = i + 1;
            if(nextIndex < _nodes.Length)
                Gizmos.DrawLine(_nodes[i], _nodes[nextIndex]);
            Gizmos.DrawSphere(_nodes[i], 0.1f);
        }
        if(_NodeIndex < _nodes.Length) {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_nodes[_NodeIndex], 1f);
        }
    }
#endif



}
