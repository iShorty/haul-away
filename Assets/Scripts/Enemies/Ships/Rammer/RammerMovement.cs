using UnityEngine;
using UnityEngine.AI;



public class RammerMovement : BaseShipMovement {
    [Header("===== RAMMERMOVEMENT INFO =====")]

    [SerializeField, Range(0, 1f), Tooltip("Movement speed multiplier while in STUNNED state.")]
    float _StunnedStateMultiplier = 0.1f;
    [SerializeField, Range(0, 1f), Tooltip("Movement speed multiplier while in IDLE state.")]
    float _IdleStateMultiplier = 0.3f;
    [SerializeField, Range(0, 1f), Tooltip("Movement speed multiplier while in PATROL state.")]
    float _PatrolStateMultiplier = 0.7f;
    [SerializeField, Range(0, 1f), Tooltip("Movement speed multiplier while in FOLLOW state.")]
    float _FollowStateMultiplier = 1f;
    [SerializeField, Range(0, 1f), Tooltip("Movement speed multiplier while in CHARGEUP state.")]
    float _ChargeUpStateMultiplier = 0.5f;
    [SerializeField, Range(0, 5f), Tooltip("Movement speed multiplier while in ATTACK state.")]
    float _AttackStateMultiplier = 1.5f;
    [Tooltip("Movement speed curve when in ATTACK state")]
    public AnimationCurve attackStateSpeedCurve;

    protected RammerController _rammerController;

    [SerializeField] LayerMask stunnedAvoidanceMask;
    
    
    
    public override void GameAwake() {
        // Debug.Log("bomber movement GameAwake");
        base.GameAwake();
        _rammerController = base._shipController as RammerController;
        
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
    }

    public override void GameFixedUpdate()  {
            // Collision with player boat
            switch (_rammerController.aiState)
            {
                case RammerState.NONE:
                case RammerState.IDLE:
                case RammerState.PATROL:
                case RammerState.FOLLOW:
                case RammerState.CHARGEUP:
                case RammerState.ATTACK:
                case RammerState.STUNNED:
                    base.GameFixedUpdate();
                    break;
                // case RammerState.STUNNED:
                //     UpdateState();
                //     AdjustVelocity();
                //     _rammerController.PropRigidBody.velocity = _velocity;
                //     ClearState();
                //     // Handle rotation
                //     if(_rammerController.KnownDetectedTarget != null)
                //         LookAtDir(_rammerController.KnownDetectedTarget.transform.position);
                //     break;
            }
    }
    
    #region Behaviour

    #region Inherited

    public override void TrackTarget() {
        if(_rammerController.KnownDetectedTarget == null) return;

        if((_rammerController.LastKnownDetectedTargetPos - 
        _rammerController.KnownDetectedTarget.transform.position).sqrMagnitude >= _rammerController.SqredMinDetectionUpdateDist) {
            
            bool rDotNow = GameUtils.DotR(transform.position, _rammerController.KnownDetectedTarget.transform);
            if(offsetRDot != rDotNow) {
                offsetRDot = rDotNow;
                _offsetFromTarget = GameUtils.GetCombatRangeOffset(_rammerController.AttackRange, _rammerController._MinDist, offsetRDot);
            }
            if(SetPosNearTarget(_rammerController.KnownDetectedTarget) == true) {
                
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
            // Debug.Log("bomber Nextpath node is null");
            return;
        }
        // if(_CurrentPath == null) return;
        switch (_rammerController.aiState) {
            case RammerState.NONE:
            case RammerState.IDLE:
                break;
            case RammerState.PATROL:
                if ((transform.position - _CurrentDestination).sqrMagnitude > _sqredPointReachingRadius) {
                    break;
                }

                if(_navNodes.Length == 0 || _navNodeIndex == _navNodes.Length - 1) {
                    SetNextNavAndNodeDestination();
                }
                else {
                    UpdateNodeIndex(_navNodes, ref _navNodeIndex, ref _NavNodeStep, true);
                    SetCurrentDestination(_navNodes[_navNodeIndex]);
                }
                break;
                // if stunned, shouldn't move (animatedly at least, but we need the whale to drift to the side so we still "move" it)
            case RammerState.STUNNED:
            case RammerState.FOLLOW:
            case RammerState.CHARGEUP:
                // If close to destination
                if ((transform.position - _CurrentDestination).sqrMagnitude > _sqredPointReachingRadius) {
                    break;
                }
                // If im at the end of the vector path, and im at the end of the nav path
                if(_NodeIndex < _nodes.Length - _NodeStep && (_navNodeIndex == _navNodes.Length - 1 || _navNodes.Length == 0)) {
                    // Debug.Log("at end of its path! Am following/stunned/chargingup.");
                }
                // Otherwise,
                else {
                    // If i'm not at the end of the nav path, update the nav node index and set my next destination.
                    if(_navNodeIndex < _navNodes.Length - _NavNodeStep) {
                    // Debug.Log("tracking till I am at end of follow/attack path");
                    UpdateNodeIndex(_navNodes, ref _navNodeIndex, ref _NavNodeStep, true);
                    SetCurrentDestination(_navNodes[_navNodeIndex]);
                    }
                }
                break;
            case RammerState.ATTACK:
                // If close to destination
                if ((transform.position - _CurrentDestination).sqrMagnitude > _sqredPointReachingRadius) {
                    break;
                }
                // If im at the end of the vector path, and im at the end of the nav path
                if(_NodeIndex < _nodes.Length - _NodeStep && (_navNodeIndex == _navNodes.Length - 1 || _navNodes.Length == 0)) {
                    // Debug.Log("at end of follow/attack path! Am attacking! I must have missed. For attacking, this means I transition out");
                    
                }
                // Otherwise,
                else {
                    // If i'm not at the end of the nav path, update the nav node index and set my next destination.
                    if(_navNodeIndex < _navNodes.Length - _NavNodeStep) {
                    // Debug.Log("tracking till I am at end of follow/attack path");
                    UpdateNodeIndex(_navNodes, ref _navNodeIndex, ref _NavNodeStep, true);
                    SetCurrentDestination(_navNodes[_navNodeIndex]);
                    }
                }
                break;
            default:
                Debug.LogWarning("NextPathNode found no valid state on " + _rammerController.name, _rammerController.gameObject);
                break;
        }
    }

    protected override float StateMovementMultiplier() {
        float multiplier = 1f;
        switch (_rammerController.aiState) {
            case RammerState.NONE:
                multiplier = 0;
                break;
            case RammerState.STUNNED:
                multiplier = _StunnedStateMultiplier;
                break;
            case RammerState.IDLE:
                multiplier = _IdleStateMultiplier;
                break;
            case RammerState.PATROL:
                multiplier = _PatrolStateMultiplier;
                break;
            case RammerState.FOLLOW:
                multiplier = _FollowStateMultiplier;
                break;
            case RammerState.CHARGEUP:
                multiplier = _ChargeUpStateMultiplier;
                break;
            case RammerState.ATTACK:
                // Scale up speed based on distance to target.
                _NavMeshAgent.enabled = true;
                float distRatioScaler = 1 - _NavMeshAgent.remainingDistance / GameUtils.PathLength(_CurrentPath.corners);
                _NavMeshAgent.enabled = false;

                multiplier = attackStateSpeedCurve.Evaluate(distRatioScaler) * _AttackStateMultiplier;
                break;
            default:
                Debug.LogWarning("No valid state found in StateMultiplier " + _rammerController.aiState + 
                " " + gameObject.name, gameObject);
                break;
        }
        return multiplier;
    }
    
    
    protected override Vector3 AvoidanceModifier(Vector3 currentDirection) {
        // Debug.DrawRay(transform.position, currentDirection * rayRange, Color.white, 0.01f);
        // Cast a bunch of rays
        for (int i = 0; i < numOfRays; i++)
        {
            Quaternion currentRotation = transform.rotation;
            // Rotate them so they fit between an angl
            Quaternion rotMod = Quaternion.AngleAxis((i / ((float)numOfRays - 1)) * avoidAngleRange * 2 - avoidAngleRange, transform.up);
            Vector3 dir = currentRotation * rotMod * Vector3.forward;

            var ray = new Ray(transform.position, dir);

            LayerMask useMe = _rammerController.aiState == RammerState.STUNNED ? stunnedAvoidanceMask : avoidanceMask;

            if(Physics.Raycast(ray, out RaycastHit hitInfo, rayRange, useMe)) {
                // Offset direction by 1 / num of rays, also scaled by distance to the hit obj so hopefully the force falls off nicely
                currentDirection -= (1.0f / numOfRays) * (1 - Mathf.Clamp01(hitInfo.distance / rayRange)) * dir;
                Debug.DrawRay(transform.position, dir * rayRange, Color.red, 0.01f);
            }
            else {
                currentDirection += (1.0f / numOfRays) * (1 - Mathf.Clamp01(hitInfo.distance / rayRange)) * dir;
                Debug.DrawRay(transform.position, dir * rayRange, Color.green, 0.01f);
            }
        }
        
        Debug.DrawRay(transform.position, currentDirection * rayRange, Color.yellow, 0.01f);

        return currentDirection;
    }

    #endregion

    #endregion

    
    #region Event Methods

    protected override void OnAttack() {
		// VFX, SFX, Anim, etc?

    }

    // Rewrite, check this later
	// Transition on detect
    protected override void OnDetectedTarget() {
        offsetRDot = GameUtils.DotR(_rammerController.KnownDetectedTarget.transform.position, transform);
        _offsetFromTarget = GameUtils.GetCombatRangeOffset(_rammerController.AttackRange, _rammerController._MinDist, offsetRDot);

        if(SetPosNearTarget(_rammerController.KnownDetectedTarget) == true) {
            CalculatePathToTarget(_CurrentDestination);
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

    #endregion


}

