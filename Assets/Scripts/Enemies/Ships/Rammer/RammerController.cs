// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.AI;
using AudioManagement;



public enum RammerState { NONE, IDLE, PATROL, FOLLOW, CHARGEUP, ATTACK, STUNNED }

public class RammerController : EnemyShipController
{

    [field: Header("BoatController Fields"), SerializeField, RenameField("AI State")]
    public RammerState aiState { get; private set; } = RammerState.PATROL;

    [Header("Rammer Attack Fields")]
    // public Transform[] _weapons;
    [SerializeField, Tooltip("Minimum duration between two attacks")]
    float delayBetweenAttacks = 10f;
    [SerializeField, Tooltip("Vary attacked position in this radius.")]
    float AttackRadius = 2.5f;
    [SerializeField, Tooltip("Charge Up Duration")]
    float chargeUpTimeDuration = 2f;
    float chargeUpTime = Mathf.Infinity;
    [SerializeField, Tooltip("Stunned Duration")]
    float stunDuration = 2f;
    float stunTime = Mathf.Infinity;

    const float attackAnimDuration = 0.54f;
    float attackAnimTime = Mathf.Infinity;
    Collision cachedAttackCollision;


    [Header("----- Animation -----")]
    [SerializeField]
    SphereCollider _animationTrigger = default;

    [SerializeField]
    Animator _animController = default;

    // [SerializeField, Tooltip("Speed multiplier when the agent is in attack state")]
    // float attackStateMultiplier = 0.5f;
    // [SerializeField] Bomb bombPrefab = default;
    // LaunchData[] projectileData;

    Vector3 _TargetPos, targetOffset = Vector3.positiveInfinity, contactNormal;
    int iterationAhead;
    float _lastAttacked = Mathf.NegativeInfinity;
    RammerMovement _boatMovement;


    public override void GameAwake()
    {
        // Debug.Log("bomber gameawake called " + gameObject.name, gameObject);
        base.GameAwake();

        _boatMovement = GetComponent<RammerMovement>();
        _boatMovement.GameAwake();

#if UNITY_EDITOR
        Debug.Assert(_animationTrigger, $"Rammer {name} does not have its animation trigger sphere assigned!", this);
        Debug.Assert(_animationTrigger.isTrigger, $"Rammer {name} does not have its animation trigger sphere set its isTriggered to true!", this);
        Debug.Assert(_animController, $"Rammer {name} does not have its _animController assigned!!", this);
#endif

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _animationTrigger.enabled = false;
    }

    public override bool GameUpdate()
    {
        if (!IsDead)
        {
            if (transform.position.y < Constants.KILLHEIGHTMIN || transform.position.y > Constants.KILLHEIGHTMAX)
            {
                _Health.Kill();
            }
        }

        _detectionModule.HandleTargetDetection();

        UpdateAIStateTransitions();

        UpdateCurrentAIState();

        // Late update bc im trash
        _detectionModule.PostMovement();

        return false;
    }

    public override void FixedGameUpdate()
    {
        _boatMovement.GameFixedUpdate();
        base.FixedGameUpdate();
    }

    ///<summary>Holds rules and transitions to switch between states.</summary>
    void UpdateAIStateTransitions()
    {
        // If no path, go to Idle.
        if (_boatMovement._PatrolPath == null)
        {
            aiState = RammerState.IDLE;
            return;
        }

        // Handle transitions 
        switch (aiState)
        {
            case RammerState.NONE:
            case RammerState.IDLE:
                // Add condition so it can just hang out in idle if we want to

                if (_boatMovement._PatrolPath != null)
                {
                    aiState = RammerState.PATROL;
                }
                break;
            case RammerState.FOLLOW:
                // Transition to CHARGEUP when in LOS and in range
                // Debug.Log("patrolling and? " + IsSeeingTarget + " " + IsTargetInAttackRange);
                if (IsSeeingTarget && IsTargetInAttackRange)
                {
                    Debug.Log("transition from FOLLOW to CHARGEUP state");
                    chargeUpTime = Time.time;
                    aiState = RammerState.CHARGEUP;
                    _animationTrigger.enabled = true;
                    // Don't calculate path here, in chargeup transition after chargeup is done. See below.
                }
                break;
            case RammerState.PATROL:
                // Transition to CHARGEUP when in LOS and in range
                // Debug.Log("patrolling and? " + IsSeeingTarget + " " + IsTargetInAttackRange);
                if (IsSeeingTarget && IsTargetInAttackRange)
                {
                    Debug.Log("transition from PATROL to CHARGEUP state");
                    chargeUpTime = Time.time;
                    aiState = RammerState.CHARGEUP;
                    _animationTrigger.enabled = true;
                }
                break;
            case RammerState.CHARGEUP:
                // Transition to attack when CHARGEUP complete
                if (IsSeeingTarget && IsTargetInAttackRange && ChargeUp())
                {
                    Debug.Log("transition from CHARGEUP to ATTACK state");
                    TryAttack(KnownDetectedTarget);

                    // Predict where the target will be based in its current velocity, then adds an offset using AttackRadius
                    PrepAttack();

                    _boatMovement.CalculatePathToTarget(_TargetPos);
                    _boatMovement.SetNavNodes(_boatMovement._CurrentPath.corners);
                    _boatMovement.SetDestinationToClosestNode(ref _boatMovement._navNodes, ref _boatMovement._navNodeIndex);

                    // _boatMovement.CalculatePathAndSetCurrentDestination(_TargetPos);

                    aiState = RammerState.ATTACK;
                }

                // Transition to FOLLOW when no longer a target in attack range
                if (!IsTargetInAttackRange && !IsSeeingTarget)
                {
                    Debug.Log("transition from CHARGEUP to FOLLOW state");
                    aiState = RammerState.FOLLOW;
                }

                // Also transition if at any point along the path to the target, the turning angle is too steep


                break;
            case RammerState.ATTACK:
                if (!IsTargetInAttackRange && !IsSeeingTarget) {
                    Debug.Log("transition from ATTACK to PATROL state");
                    aiState = RammerState.PATROL;
                }
                else if (!IsTargetInAttackRange && IsSeeingTarget) {
                    Debug.Log("transition from ATTACK to FOllOW state");
                    aiState = RammerState.FOLLOW;
                }
                // Transition to FOLLOW when no longer a target in attack range
                if (AttackAnimTimer() || cachedAttackCollision == null)
                {
                    break;
                }
                else
                {
                    Debug.Log("tail attack");
                    TailAttack(cachedAttackCollision);
                    cachedAttackCollision = null;
                }

                break;
            case RammerState.STUNNED:
                // Stun is over
                if (Stun() == false)
                {
                    // Debug.Log("Stunned state. Stun() == false, its over.");
                    // SHOULD GO INTO CHARGEUP STATE INSTEAD

                    // If lost target during this time, transition out now.
                    // If still tracking target, follow.
                    // Transition to FOLLOW when no longer a target in attack range

                    // Don't see the target? Go back to patrolling
                    if (!IsSeeingTarget)
                    {
                        Debug.Log("stun complete, target out of range and not visible. Back to patrolling.");
                        aiState = RammerState.PATROL;
                    }
                    else
                    {
                        // See the target and its in range, chargeup again.
                        if (IsTargetInAttackRange)
                        {
                            Debug.Log("stun complete, target in range and visible. chargeup.");
                            chargeUpTime = Time.time;
                            aiState = RammerState.CHARGEUP;
                            _animationTrigger.enabled = true;
                        }
                        // See the target and its not in range, follow it till you're in range again.
                        else
                        {
                            Debug.Log("stun complete, target out of range and visible. Follow target.");
                            aiState = RammerState.FOLLOW;
                        }
                    }

                    // if (IsTargetInAttackRange)
                    // {
                    //     // Debug.Log("transition from STUNNED to ATTACK state");
                    //     PrepAttack();

                    //     Debug.Log("missing next line o code");
                    //     // _boatMovement.CalculatePathAndSetCurrentDestination(_TargetPos);
                    //     aiState = RammerState.ATTACK;
                    // }
                    // else {
                    //     if(IsSeeingTarget) {
                    //         Debug.Log("stun complete, target out of range but visible. Follow target");
                    //         aiState = RammerState.FOLLOW;
                    //     }
                    //     else {
                    //         Debug.Log("stun complete, target out of range and not visible. Back to patrolling.");
                    //         aiState = RammerState.PATROL;
                    //     }
                    // }
                }
                break;
            default:
                Debug.LogWarning("No valid state found! " + gameObject.name, gameObject);
                break;
        }
    }

    void UpdateCurrentAIState()
    {
        // Handle logic
        switch (aiState)
        {
            // Play idle anims, maybe have pirates do target practice at nearby things?
            case RammerState.IDLE:
                // // Find a path. Done in movement scripts.
                // if(_boatMovement._PatrolPath == null) {
                //     _boatMovement.GetCachedPath();
                //     return;
                // }
                aiState = RammerState.PATROL;
                break;
            case RammerState.PATROL:
                // Update A Star path index, and the path node if needed. 
                // Internally, calls methods to update the current destination if index is updated, 
                // and to calculate a new path to the next patrol node if needed.
                _boatMovement.NextPathNode(_boatMovement._CurrentDestination);
                _boatMovement.UpdateDesiredVelocity();
                break;
            case RammerState.FOLLOW:
                // No path nodes, and destination can change. Update if it moves.
                _boatMovement.TrackTarget();
                _boatMovement.NextPathNode(_boatMovement._CurrentDestination);
                _boatMovement.UpdateDesiredVelocity();
                break;
            case RammerState.CHARGEUP:
                // _boatMovement.TrackTarget();
                _boatMovement.NextPathNode(_boatMovement._CurrentDestination);
                _boatMovement.UpdateDesiredVelocity();
                break;
            case RammerState.ATTACK:
                // _boatMovement.TrackTarget();
                // _boatMovement.UpdatePathToTarget();
                _boatMovement.NextPathNode(_boatMovement._CurrentDestination);
                _boatMovement.UpdateDesiredVelocity();
                // TryAttack(KnownDetectedTarget);
                break;
            case RammerState.STUNNED:
                _boatMovement.NextPathNode(_boatMovement._CurrentDestination);
                _boatMovement.UpdateDesiredVelocity();
                break;
        }
    }

    // Get a position... far away to begin attacking. Outside of player attack range.
    // Charge up. Anims, SFX. Move much slower than normal so you don't fall too far behind.
    // Ram. Build up speed, until you're at/around RammingSpeed when you hit the player.
    // On collision, bump the player. Shove the player in the direction Rammer was moving. Rock player boat. Items go whee.
    // Reduce my velocity greatly on collision, or even send self backwards a little.
    // Wait for a while. Stunned or smth bc Attack success.
    // 

    public bool TryAttack(GameObject targetGO)
    {
        // Don't attack if game over

        // If we need to propogate the successful attack to other stuff e.g. SFX/anims/etc
        OnAttack();
        _lastAttacked = Time.time;

        return true;
    }

    void PrepAttack()
    {
        Vector3 targetSpeed = KnownDetectedTarget.GetComponent<Rigidbody>().velocity;
        targetSpeed.y = 0;
        // If < 1, don't predict.
        if (targetSpeed.magnitude < 1)
        {
            iterationAhead = 0;
        }
        else
        {
            iterationAhead = (int)((KnownDetectedTarget.transform.position - transform.position).magnitude / targetSpeed.magnitude);
        }
        _TargetPos = KnownDetectedTarget.transform.position + targetSpeed * iterationAhead;

        // target position = randomised position around the predicted position based on targets current speed.
        if (targetOffset.x == Mathf.Infinity)
        {
            targetOffset = GameUtils.GetTargetOffset(AttackRadius);
            _TargetPos += targetOffset;
        }

        // if (_RamStartPos.x == Mathf.Infinity)
        // {
        //     _RamStartPos = transform.position;
        // }
    }

    bool Stun()
    {
        if (stunTime + stunDuration < Time.time)
        {
            stunTime = Mathf.Infinity;
            // }
            // if (Time.time - stunTime > stunDuration)
            // {
            // Debug.Log("Stun complete " + Time.time + " " + stunTime);
            // Play SFX, VFX, anim, etc

            return false;
        }
        // Play SFX, VFX, anim, etc

        return true;
    }

    bool ChargeUp()
    {
        if (chargeUpTime + chargeUpTimeDuration < Time.time)
        {
            chargeUpTime = Mathf.Infinity;

            // Debug.Log("Charged up, attack");
            // Play SFX, VFX, anim, etc

            return true;
        }
        // Play SFX, VFX, anim, etc


        return false;
    }
    bool AttackAnimTimer()
    {
        // Timer finish
        if (attackAnimTime + attackAnimDuration < Time.time)
        {
            attackAnimTime = Mathf.Infinity;

            return false;
        }
        // Play SFX, VFX, anim, etc

        return true;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        EvaluatePlayerBoatCollision(collision);
    }

    void EvaluatePlayerBoatCollision(Collision collision)
    {
        if (BoatManager.IsPartOfBoat(collider: collision.collider))
        {
            // Collision with player boat
            switch (aiState)
            {
                case RammerState.NONE:
                case RammerState.IDLE:
                case RammerState.PATROL:
                case RammerState.FOLLOW:
                case RammerState.CHARGEUP:
                case RammerState.STUNNED:
                    break;
                case RammerState.ATTACK:
                    if (!BoatManager.IsPartOfBoat(collider: collision.collider)) return;

                    Debug.Log("rammer on collision with player boat");

                    cachedAttackCollision = collision;
                    attackAnimTime = Time.time;

                    _animController.SetTrigger(Constants.For_Enemy.ORCA_ANIMATION_PARAM_ATTACK);
                    _animationTrigger.enabled = false;
                    break;
                default:
#if UNITY_EDITOR
                    Debug.LogWarning("No appropriate state found!");
#endif
                    break;
            }
        }
    }

    public void TailAttack(Collision collision)
    {
        // Bump whatever is hit
        AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_WhaleImpact, transform.position, true, true);
#if UNITY_EDITOR
        Debug.Log("tailattack " + collision);
#endif
        int collisionLayer = collision.gameObject.layer;

        GameUtils.ApplyImpulse(collision, transform, bumpForce);
        _animationTrigger.enabled = false;

        // // Bump self
        // if (collisionLayer == Constants.For_Layer_and_Tags.LAYERINDEX_BOATONBOAT_ENEMY)
        // {
        //     GameUtils.Bump(collision, transform, PropRigidBody, 10);
        // }

        PropRigidBody.velocity = -PropRigidBody.velocity;
        stunTime = Time.time;
        aiState = RammerState.STUNNED;
        
#if UNITY_EDITOR
        Debug.Log("stunned now");
#endif
        // Move backwards
        var offset = transform.rotation * (-Vector3.forward * 30f);
        offset.y = 0;
        _TargetPos = transform.position + offset;
        // transform.position += Vector3.up;
        // transform.position -= Vector3.forward;

        // var snappedTargetPos = _boatMovement.TrySetNavPos(_TargetPos, 1f);
        // if(snappedTargetPos.x != Mathf.Infinity) {
        // _boatMovement.CalculatePathToTarget(snappedTargetPos);
        // _boatMovement.SetNavNodes(_boatMovement._CurrentPath.corners);
        // _boatMovement.SetDestinationToClosestNode(ref _boatMovement._navNodes, ref _boatMovement._navNodeIndex);
        // }

        _boatMovement.CalculatePathToTarget(_TargetPos);
        _boatMovement.SetNavNodes(_boatMovement._CurrentPath.corners);
        _boatMovement.SetDestinationToClosestNode(ref _boatMovement._navNodes, ref _boatMovement._navNodeIndex);

#if UNITY_EDITOR
        Debug.Log("new destination? " + _boatMovement._CurrentDestination);
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = _TargetPos;
        sphere.GetComponent<Collider>().enabled = false;
        Debug.Break();
#endif
        // _boatMovement.CalculatePathToTarget(_TargetPos);
        // _boatMovement.SetNavNodes(_boatMovement._CurrentPath.corners);
        // _boatMovement.SetDestinationToClosestNode(ref _boatMovement._navNodes, ref _boatMovement._navNodeIndex);
    }

    // ///<Summary>Handles the triggering of the attack animation for the rammer</Summary>
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (aiState != RammerState.ATTACK) return;

    //     if (!BoatManager.IsPartOfBoat(collider: other)) return;

    //     _animController.SetTrigger(Constants.For_Enemy.ORCA_ANIMATION_PARAM_ATTACK);
    //     _animationTrigger.enabled = false;
    // }


    #region Public Wrappers For Event Wrappers

    public override void OnAttack()
    {
        OnAttackEvent();
    }
    public override void OnDamaged(GameObject source, int damage)
    {
        OnDamagedEvent(source, damage);
    }
    public override void OnDetectedTarget()
    {
        OnDetectedTargetEvent();

        aiState = RammerState.FOLLOW;
    }
    public override void OnLostTarget()
    {
        OnLostTargetEvent();
        aiState = RammerState.PATROL;
    }

    #endregion

}


