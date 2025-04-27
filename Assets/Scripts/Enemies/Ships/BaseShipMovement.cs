using UnityEngine;



public abstract class BaseShipMovement : NavNodeMovement {
    
    [field: Header("===== SHIP NAV NODE MOVEMENT INFO =====")]
    [field: SerializeField] 
    public MovementPath _PatrolPath {get; protected set; } = default; // Set in inspector

    [Tooltip("Max speed")]
    public float _MaxSpeed = 10f;
    [Range(0, 1f), Tooltip("Higher value, higher minimum velocity- the further it'll travel while turning when not facing destination")]
    public float _MinVelocityDot = 0.25f;
    [Tooltip("Max acceleration")]
    public float _MaxAcceleration = 25f/*, maxAirAcceleration = 1f*/; 
    [Range(0, 1f), Tooltip("Higher value, higher min accel- the faster it'll change direction when not facing destination")]
    public float _MinAccelDot = 0.3f; 
    [Tooltip("Max rotation speed")]
    public float _MaxRotSpeed = 0.1f;

    // 0 to 1. Accelerate faster when facing direction, clamp to min value so acceleration when not facing direction isnt too slow.
    float _accelDotClamped => Mathf.Clamp((1 + GameUtils.Dot(_CurrentDestination, transform)) * 0.5f, _MinAccelDot, 1);

    // 0 to 1. Move faster when facing direction, clamp to min value so movement when not facing direction isnt too slow.
    float _velocityDotClamped => Mathf.Clamp((1 + GameUtils.Dot(_CurrentDestination, transform)) * 0.5f, _MinVelocityDot, 1);

    // 1 to 0. Rotate faster when not facing direction, clamp to min value so rotation when facing direction isnt too slow.
    // float _rotDotClamped => Mathf.Clamp((1 - Dot(_CurrentDestination)) * 0.5f, _MinRotDot, 1);
    protected virtual float _CurrentMaxVelocity => _MaxSpeed * StateMovementMultiplier();
    protected virtual float _rotationSpeed => _MaxRotSpeed /* * _rotDotClamped*/; // 1 to 0

    protected EnemyShipController _shipController;
    protected Quaternion _lookRot;
    [SerializeField, Tooltip("The distance at which the agent considers that it has reached its current path destination point")]
    protected float _pointReachingRadius = 7.5f;
    protected float _sqredPointReachingRadius = default;
	protected Vector3 _dir, 
    _velocity, _desiredVelocity, _contactNormal;
    protected Vector3 _offsetFromTarget;
    protected bool offsetRDot;

    [SerializeField] protected LayerMask avoidanceMask;
    public int numOfRays = 5;
    public float avoidAngleRange = 90f;
    public float rayRange = 25f;



    // Update handling of index to suit behaviours by overriding.
    public override void UpdateNodeIndex(Vector3[] nodeArray, ref int index, ref int step, bool useDefault = false) {
        if(IsNodeArrayValid(nodeArray) == false) {
            Debug.Log("invalid nodearray, default to index 0");
            index = 0;
            return;
        }
        if(!_PatrolPath || useDefault) {
            base.UpdateNodeIndex(nodeArray, ref index, ref step);
            return;
        }
        
        switch (_PatrolPath._PathType) {
            case PathType.Stationary:
                // Debug.Log("PathType.Stationary, not going anywhere", this);
                break;
            case PathType.BackAndForth:
                index = GameUtils.BdLoop(index, ref step, nodeArray.Length);
                break;
            // Loop back around
            case PathType.Cyclic:
                index = GameUtils.CyclicLoop(index, step, nodeArray.Length);
                break;
            // Random af, just not to last index value
            case PathType.Roaming:
                break;
            // Not for use with indexes, so set a rand pos, do the path calc, set indexes to appropriate values
            case PathType.WithinCircle:
                break;
            default:
                Debug.LogWarning("UpdateNodeIndex found no valid pathtype in " + _shipController.name, _shipController.gameObject);
                break;
        }
    }
    


    public override void GameAwake() {
        _shipController = GetComponent<EnemyShipController>();

    	_shipController.onAttack += OnAttack;
    	_shipController.onDetectedTarget += OnDetectedTarget;
    	_shipController.onLostTarget += OnLostTarget;
    	_shipController.onDamaged += OnDamaged;

        _sqredPointReachingRadius = _pointReachingRadius * _pointReachingRadius;

        base.GameAwake();
    }

    public virtual void GameFixedUpdate()  {
		UpdateState();
		AdjustVelocity();
		_shipController.PropRigidBody.velocity = _velocity;
        ClearState();
        // Handle rotation
        LookAtDir(_dir);
    }
    
    #region Update Velocity
    
	///<summary>Updates fields used to compute velocity/movement/etc.</summary>
    protected virtual void UpdateState() {
        _velocity = _shipController.PropRigidBody.velocity;
        // Calculate surface normal.
		_contactNormal = Vector3.up;
    }
	
    protected void AdjustVelocity () {
        // Align desired velocity to ground/ocean surface.
        // Project vectors on axes on the contact plane.
		// Vector3 xAxis = GameUtils.ProjectOnContactPlane(Vector3.right, _contactNormal).normalized;
		// Vector3 zAxis = GameUtils.ProjectOnContactPlane(Vector3.forward, _contactNormal).normalized;

        // Current relative speeds
		float currentX = Vector3.Dot(_velocity, Vector3.right);
		float currentZ = Vector3.Dot(_velocity, Vector3.forward);

        // Usual speed calculation, relative to the ocean surface.
		float maxSpeedChange = _MaxAcceleration * Time.deltaTime;

        // Accelerate faster if moving in same direction i'm facing, like a ship
        maxSpeedChange *= _accelDotClamped;

		float newX = Mathf.MoveTowards(currentX, _desiredVelocity.x, maxSpeedChange);
		float newZ = Mathf.MoveTowards(currentZ, _desiredVelocity.z, maxSpeedChange);

        // Adjust velocity
		_velocity += Vector3.right * (newX - currentX) + Vector3.forward * (newZ - currentZ);
        
        // Scale velocity down depending on how off the direction is
        // Makes the unit slow down when turning like a ship
        _velocity *= _velocityDotClamped; // Value between _MinVelocityDot to 1

        // Get direction vector
        Vector3 avoidanceDirection = _velocity.normalized;
        avoidanceDirection = AvoidanceModifier(avoidanceDirection);
        _velocity = avoidanceDirection * _velocity.magnitude;

        // Clamp to max value just in case
        _velocity = Vector3.ClampMagnitude(_velocity, _CurrentMaxVelocity);

	}

    protected virtual Vector3 AvoidanceModifier(Vector3 currentDirection) {
        // Debug.DrawRay(transform.position, currentDirection * rayRange, Color.white, 0.01f);
        // Cast a bunch of rays
        for (int i = 0; i < numOfRays; i++)
        {
            Quaternion currentRotation = transform.rotation;
            // Rotate them so they fit between an angl
            Quaternion rotMod = Quaternion.AngleAxis((i / ((float)numOfRays - 1)) * avoidAngleRange * 2 - avoidAngleRange, transform.up);
            Vector3 dir = currentRotation * rotMod * Vector3.forward;

            var ray = new Ray(transform.position, dir);

            if(Physics.Raycast(ray, out RaycastHit hitInfo, rayRange, avoidanceMask)) {
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
    
    protected void ClearState () {
		_contactNormal = Vector3.zero;
	}
    
    #endregion

	///<summary>Move to target destination.</summary>
    public virtual void UpdateDesiredVelocity() {
        if(_CurrentPath == null) {
            return;
        }

        // Used for looking rotation
        _dir = (_CurrentDestination - _shipController.transform.position).normalized;
		_dir.y = 0;
		_desiredVelocity = transform.forward * _CurrentMaxVelocity;
    }

    public virtual void LookAtDir(Vector3 target) {
        if (target == Vector3.zero) {
            return;
        }
        
        _lookRot = Quaternion.LookRotation(target);
        _shipController.transform.localRotation = 
        Quaternion.Slerp(_shipController.transform.rotation, _lookRot, Time.deltaTime * _rotationSpeed);
    }

    // Tweak this to use a dummy collider to approximate the ship in its new pos instead of testing a singular point to test if its pos is ok
	///<summary>Set a point near the target to calculate a path to.</summary>
	public virtual bool SetPosNearTarget(GameObject target) {
		if(target == null) return false;
		Vector3 newPos = Vector3.zero;
        newPos = target.transform.position + target.transform.rotation * _offsetFromTarget;

        newPos.y = 1f;
        newPos = TrySetNavPos(newPos, 1f);
        if(newPos.x != Mathf.Infinity) {
            SetCurrentDestination(newPos);
            return true;
        }
        // Try other side
        else {

            Vector3 otherOffset = _offsetFromTarget;
            otherOffset.x *= -1;
            newPos = target.transform.position + target.transform.rotation * otherOffset;
            
            newPos.y = 1f;
            newPos = TrySetNavPos(newPos, 1f);
            if(newPos.x != Mathf.Infinity) {
                SetCurrentDestination(newPos);
                return true;
            }
        }

        // both sides failed, nearest position near player boat/target?
        newPos = target.transform.position;
        
        newPos.y = 1f;
        newPos = TrySetNavPos(newPos, 1f);
        if(newPos.x != Mathf.Infinity) {
            SetCurrentDestination(newPos);
            return true;
        }

        // stay put.
        newPos = transform.position;
        
        newPos.y = 1f;
        newPos = TrySetNavPos(newPos, 1f);
        if(newPos.x != Mathf.Infinity) {
            SetCurrentDestination(newPos);
            return true;
        }
        else {
            return false;
        }
	}

	///<summary>Try to snap the position to the NavMesh, then return it.</summary>
    public override Vector3 TrySetNavPos(Vector3 position, float maxDist) {
        Vector3 navPos = GameUtils.GetNearestNavPos(position, _navHit, maxDist);
        if(navPos.x == Mathf.Infinity) {
            return Vector3.positiveInfinity;
        }
        return navPos;
    }
    
    protected virtual void OnCollisionEnter(Collision collision) {
        GameUtils.Bump(collision, transform, _shipController.PropRigidBody, 1);
    }

    public abstract void TrackTarget();
    protected abstract float StateMovementMultiplier();


    protected abstract void OnAttack();
	// Transition on detect
    protected abstract void OnDetectedTarget();
	// Transition on losing target
    protected abstract void OnLostTarget();
    protected abstract void OnDamaged(GameObject source, int damage);



}
