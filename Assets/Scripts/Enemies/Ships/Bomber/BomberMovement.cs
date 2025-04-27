using UnityEngine;
// using UnityEngine.AI;


[RequireComponent(typeof(BomberController))]
public class BomberMovement : BaseShipMovement {

    [Header("===== BOMBERMOVEMENT INFO =====")]

    [SerializeField, Range(0, 1f), Tooltip("Movement speed multiplier while in PATROL state.")]
    float patrolStateMultiplier = 0.7f;
    [SerializeField, Range(0, 1f), Tooltip("Movement speed multiplier while in FOLLOW state.")]
    float followStateMultiplier = 1f;
    [SerializeField, Range(0, 1f), Tooltip("Movement speed multiplier while in ATTACK state.")]
    float attackStateMultiplier = 0.5f;

    protected BomberController _bomberController;
    
    
    public override void GameAwake() {
        // Debug.Log("bomber movement GameAwake");
        base.GameAwake();
        _bomberController = base._shipController as BomberController;
        
        if(_PatrolPath) {
            _PatrolPath._ActiveUser = this.gameObject;

            Transform[] nodeTransforms = _PatrolPath.pathNodes.ToArray();
            Vector3[] nodePositions = new Vector3[nodeTransforms.Length];

            for (int i = 0; i < nodeTransforms.Length; i++) {
                nodePositions[i] = nodeTransforms[i].position;
            }
            SetNodes(nodePositions);
            
            SetDestinationToClosestNode(ref _nodes, ref _NodeIndex);
            CalculatePathToTarget(_CurrentDestination);

            SetNavNodes(_CurrentPath.corners);
            SetDestinationToClosestNode(ref _navNodes, ref _navNodeIndex);
        }
        
        // Debug.Log("bom move " + _CurrentDestination, this);
    }

    public override void GameFixedUpdate()  {
        base.GameFixedUpdate();
    }

	///<summary>Move to target destination.</summary>
    public override void UpdateDesiredVelocity() {
        if(_CurrentPath == null)  return;

        // Used for rotation
		_dir = (_CurrentDestination - transform.position).normalized;
		_dir.y = 0;

		_desiredVelocity = transform.forward * _MaxSpeed;
    }

    protected override float StateMovementMultiplier() {
        float multiplier = 1f;
        switch (_bomberController.aiState) {
            case BomberState.NONE:
            case BomberState.IDLE:
                multiplier = 0.3f;
                break;
            case BomberState.PATROL:
                multiplier = patrolStateMultiplier;
                break;
            case BomberState.FOLLOW:
                multiplier = followStateMultiplier;
                break;
            case BomberState.ATTACK:
                // multiplier = attackStateSpeedCurve.Evaluate(distRatio) * attackStateMultiplier;
                multiplier = attackStateMultiplier;
                break;
            default:
                Debug.LogWarning("No valid state found in StateMultiplier " + _bomberController.aiState + 
                " " + gameObject.name, gameObject);
                break;
        }
        return multiplier;
    }

    public override void LookAtDir(Vector3 target) {
        if (target == Vector3.zero) {
            return;
        }
        // target.y = 0;
        _lookRot = Quaternion.LookRotation(target);
        // Debug.Log("_orientationSpeed " + _orientationSpeed + " " + _MaxRotSpeed);
        _bomberController.transform.localRotation = 
        Quaternion.Slerp(_bomberController.transform.rotation, _lookRot, Time.deltaTime * _rotationSpeed);
    }

    public override void TrackTarget() {
        if(_bomberController.KnownDetectedTarget == null) return;
        if((_bomberController.LastKnownDetectedTargetPos - 
        _bomberController.KnownDetectedTarget.transform.position).sqrMagnitude >= _bomberController.SqredMinDetectionUpdateDist) {
            
            bool rDotNow = GameUtils.DotR(transform.position, _bomberController.KnownDetectedTarget.transform);
            // Debug.Log(rDotNow + " " + offsetRDot);
            if(offsetRDot != rDotNow) {
                offsetRDot = rDotNow;
                _offsetFromTarget = GameUtils.GetCombatRangeOffset(_bomberController.AttackRange, _bomberController._MinDist, offsetRDot);
            }
            if(SetPosNearTarget(_bomberController.KnownDetectedTarget) == true) {
                
                CalculatePathToTarget(_CurrentDestination);
                SetNavNodes(_CurrentPath.corners);
                SetDestinationToClosestNode(ref _navNodes, ref _navNodeIndex);
                
                NextPathNode(_CurrentDestination);
            }
        }
    }

    // Override the nextPathNode from NavNodeMovement to accomodate specific behaviour
    public override void NextPathNode(Vector3 node) {
        if(node.x == Mathf.Infinity) {
            return;
        }
        // if(_CurrentPath == null) return;
        switch (_bomberController.aiState) {
            case BomberState.NONE:
            case BomberState.IDLE:
                break;
            case BomberState.PATROL:
                // Debug.DrawRay(transform.position, 
                // (_CurrentDestination - transform.position).normalized * Vector3.Distance(_CurrentDestination, transform.position),
                // Color.red, 0.1f);
                if ((transform.position - _CurrentDestination).sqrMagnitude > _sqredPointReachingRadius) {
                    break;
                }

                // Original NextPathNode:
                if(_navNodes.Length == 0 || _navNodeIndex == _navNodes.Length - 1) {
                    SetNextNavAndNodeDestination();
                }
                else {
                    UpdateNodeIndex(_navNodes, ref _navNodeIndex, ref _NavNodeStep, true);
                    SetCurrentDestination(_navNodes[_navNodeIndex]);
                }
                break;
            case BomberState.FOLLOW:
            case BomberState.ATTACK:
                // Debug.DrawRay(transform.position, 
                // (_CurrentDestination - transform.position).normalized * Vector3.Distance(_CurrentDestination, transform.position),
                // Color.red, 0.1f);
                
                // If close to destination
                if ((transform.position - _CurrentDestination).sqrMagnitude > _sqredPointReachingRadius) {
                    break;
                }

                // If im at the end of the vector path, and im at the end of the nav path
                if(_NodeIndex < _nodes.Length - _NodeStep && _navNodeIndex == _navNodes.Length - 1) {
                    // Debug.Log("at end of follow/attack path!");
                }
                // Otherwise,
                else {
                    // If i'm not at the end of the nav path, update the nav node index and set my next destination.
                    if(_navNodeIndex < _navNodes.Length - _NavNodeStep) {
                        UpdateNodeIndex(_navNodes, ref _navNodeIndex, ref _NavNodeStep, true);
                        SetCurrentDestination(_navNodes[_navNodeIndex]);
                    }
                }
                break;
            default:
                Debug.LogWarning("NextPathNode found no valid state on " + _bomberController.name, _bomberController.gameObject);
                break;
        }
    }

    protected override void OnAttack() {
		// VFX, SFX, Anim, etc?

    }

	// Transition on detect
    protected override void OnDetectedTarget() {
        offsetRDot = GameUtils.DotR(_bomberController.KnownDetectedTarget.transform.position, transform);
        _offsetFromTarget = GameUtils.GetCombatRangeOffset(_bomberController.AttackRange, _bomberController._MinDist, offsetRDot);

        if(SetPosNearTarget(_bomberController.KnownDetectedTarget) == true) {
            SetNavNodes(_CurrentPath.corners);
            SetDestinationToClosestNode(ref _navNodes, ref _navNodeIndex);
        }
		// VFX, SFX, Anim, etc?
        
    }

	// Transition on losing target
    protected override void OnLostTarget() {
        SetDestinationToClosestNode(ref _nodes, ref _NodeIndex);
        CalculatePathToTarget(_CurrentDestination);

        SetNavNodes(_CurrentPath.corners);
        SetDestinationToClosestNode(ref _navNodes, ref _navNodeIndex);

		// VFX, SFX, Anim, etc?
    }

    protected override void OnDamaged(GameObject source, int damage) {
        if(source == null) {
            return;
        }
		// VFX, SFX, Anim, etc
    }


}



