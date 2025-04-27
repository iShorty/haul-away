using UnityEngine;
using AudioManagement;


public enum BomberState { NONE, IDLE, PATROL, FOLLOW, ATTACK }

[SelectionBase, RequireComponent(typeof(BomberMovement))]
public partial class BomberController : EnemyShipController
{

    [field: Header("BoatController Fields"), SerializeField]
    public BomberState aiState { get; private set; } = BomberState.PATROL;

    [Header("Bomber Attack Fields")]
    public Transform[] _weapons;
    [Tooltip("Minimum duration between two attacks")]
    [SerializeField] float delayBetweenAttacks = 0.5f;
    [SerializeField, Tooltip("Vary attacked position in this radius.")]
    float AttackRadius = 1.5f;

    // [SerializeField] Projectile ProjectilePrefab = default;
    [SerializeField] ProjectileInfo projectileInfo = default;
    [SerializeField] VFXInfo cannonFireInfo = default;
    [SerializeField] VFXInfo deathExplosionInfo = default;

    LaunchData[] projectileData;
    int iterationAhead;
    float _lastAttacked = Mathf.NegativeInfinity;
    BomberMovement _boatMovement;
    // bool _firedCannonDelay = default;


    // Controller init, then movement
    public override void GameAwake()
    {
        base.GameAwake();

        projectileData = new LaunchData[_weapons.Length];
        for (int i = 0; i < projectileData.Length; i++)
        {
            projectileData[i] = new LaunchData(Vector3.positiveInfinity, Vector3.positiveInfinity, Mathf.Infinity);
        }

        _boatMovement = GetComponent<BomberMovement>();
        _boatMovement.GameAwake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        // _firedCannonDelay = false;
    }

    public override bool GameUpdate()
    {
        // Calls from Enemy. Checks death condition if it falls out of world/opposite.
        base.GameUpdate();

        _detectionModule.HandleTargetDetection();

        UpdateAIStateTransitions();

        UpdateCurrentAIState();

        // Late update for movement to clean up things
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
            aiState = BomberState.IDLE;
            return;
        }

        // Handle transitions 
        switch (aiState)
        {
            case BomberState.NONE:
            case BomberState.IDLE:
                if (_boatMovement._PatrolPath != null)
                {
                    aiState = BomberState.PATROL;
                }
                break;
            case BomberState.PATROL:
                // If I have LOS and its in range- attack
                if (IsSeeingTarget && IsTargetInAttackRange)
                {
                    aiState = BomberState.ATTACK;
                }
                break;
            case BomberState.FOLLOW:
                // If I have LOS and its in range- attack
                if (IsSeeingTarget && IsTargetInAttackRange)
                {
                    aiState = BomberState.ATTACK;
                }
                break;
            case BomberState.ATTACK:
                // Transition to FOLLOW when no longer a target in attack range
                if (!IsTargetInAttackRange)
                {
                    aiState = BomberState.FOLLOW;
                }
                break;
        }
    }

    void UpdateCurrentAIState()
    {
        // Handle logic
        switch (aiState)
        {
            case BomberState.IDLE:
                // // Find a path. Done in movement scripts.
                // if(_boatMovement._PatrolPath == null) {
                //     _boatMovement._PatrolPath = PathManager.Instance.GetCachedPath();
                //     return;
                // }
                break;
            case BomberState.PATROL:
                _boatMovement.NextPathNode(_boatMovement._CurrentDestination);
                _boatMovement.UpdateDesiredVelocity();
                break;
            case BomberState.FOLLOW:
                // No path nodes, and destination can change. Update if it moves.
                _boatMovement.TrackTarget();
                _boatMovement.NextPathNode(_boatMovement._CurrentDestination);
                // Update math for physics movement.
                _boatMovement.UpdateDesiredVelocity();
                break;
            case BomberState.ATTACK:
                _boatMovement.TrackTarget();
                _boatMovement.NextPathNode(_boatMovement._CurrentDestination);
                _boatMovement.UpdateDesiredVelocity();

                TryAttack(KnownDetectedTarget);
                break;
        }
    }

    public bool TryAttack(GameObject targetGO)
    {
        // Don't attack if game over

        // CalculateLaunchData ahead of time. Should be using targetGO, but dont pass it in for now
        CalculateAttack();
        // Use data to orient ship weaponry appropriately.
        // OrientWeaponsTowards();

        if ((_lastAttacked + delayBetweenAttacks) >= Time.time)
            return false;

        // If a weapon is ready (aiming at target). If only one fire at a time and not individual cooldowns, shufflebag the array and iterate.
        for (int i = 0; i < _weapons.Length; i++)
        {
            // float dot = Vector3.Dot(_weapons[i].forward, projectileData[i].initialVelocity.normalized);
            // if (dot > 0.95f)
            // {
                // Pass firepoint into Fire
                Fire(_weapons[i], projectileData[i].initialVelocity);
                projectileData[i] = new LaunchData(Vector3.positiveInfinity, Vector3.positiveInfinity, Mathf.Infinity);
                // If we need to propogate the successful attack to other stuff e.g. SFX/anims/etc
                OnAttack();
                _lastAttacked = Time.time;
            // }
        }
        // switch (_firedCannonDelay)
        // {
        //     #region ------------- Fire Cannon Delay ----------------
        //     case true:
        //         if ((_lastAttacked + Constants.For_PlayerStations.MULTIPURPOSE_ANIMATION_DELAY) >= Time.time)
        //             return false;

        //         //Actually fires the cannon
        //         // If a weapon is ready (aiming at target). If only one fire at a time and not individual cooldowns, shufflebag the array and iterate.
        //         for (int i = 0; i < _weapons.Length; i++)
        //         {
        //             // float dot = Vector3.Dot(_weapons[i].forward, projectileData[i].initialVelocity.normalized);
        //             // if (dot > 0.95f)
        //             // {

        //             // }

        //              // Pass firepoint into Fire
        //                 Fire(_weapons[i], projectileData[i].initialVelocity);
        //                 projectileData[i] = new LaunchData(Vector3.positiveInfinity, Vector3.positiveInfinity, Mathf.Infinity);

        //                 // If we need to propogate the successful attack to other stuff e.g. SFX/anims/etc
        //                 OnAttack();

        //         }

        //         //Reset values
        //         _firedCannonDelay = false;
        //         _lastAttacked = Time.time;
        //         break;
        //     #endregion

        //     case false:
        //         if ((_lastAttacked + delayBetweenAttacks) >= Time.time)
        //             return false;

        //         //Ready to fire
        //         Animation_FireCannon();
        //         _firedCannonDelay = true;
        //         _lastAttacked = Time.time;
        //         break;
        // }

        if ((_lastAttacked + delayBetweenAttacks) >= Time.time)
            return false;

        //Actually fires the cannon
        // If a weapon is ready (aiming at target). If only one fire at a time and not individual cooldowns, shufflebag the array and iterate.
        for (int i = 0; i < _weapons.Length; i++)
        {
            float dot = Vector3.Dot(_weapons[i].forward, projectileData[i].initialVelocity.normalized);
            if (dot > 0.95f)
            {
                // Pass firepoint into Fire
                Fire(_weapons[i], projectileData[i].initialVelocity);
                projectileData[i] = new LaunchData(Vector3.positiveInfinity, Vector3.positiveInfinity, Mathf.Infinity);

                // If we need to propogate the successful attack to other stuff e.g. SFX/anims/etc
                OnAttack();

            }
        }

        return true;
    }

    // For now since it only attacks player just leave the _playerBoat instead of KnownDetectedTarget stuff be, am sleepy
    void CalculateAttack()
    {
        Vector3 targetSpeed = BoatManager.Controller._rigidBody.velocity;

        for (int i = 0; i < _weapons.Length; i++)
        {
            iterationAhead = (int)((BoatManager.Controller.transform.position - _weapons[i].position).magnitude / _boatMovement._MaxSpeed);
            Vector3 targetPos = BoatManager.Controller.transform.position + targetSpeed * iterationAhead;

            Vector3 targetOffset = GameUtils.GetTargetOffset(AttackRadius);
            projectileData[i] = GameUtils.CalculateLaunchData(_weapons[i].position, targetPos + targetOffset);
        }
    }

    // public void OrientWeaponsTowards()
    // {
    //     if (_weapons.Length <= 0)
    //     {
    //         return;
    //     }
    //     for (int i = 0; i < _weapons.Length; i++)
    //     {
    //         Vector3 dir = projectileData[i].initialVelocity.normalized;
    //         if (dir == Vector3.zero)
    //         {
    //             return;
    //         }
    //         Quaternion lookRot = Quaternion.LookRotation(dir);
    //         // Debug.DrawRay(_weapons[i].position, dir * 10f, Color.red, 0.1f);
    //         // Debug.Log("rottate " + _weapons[i].rotation + " " + lookRot);
    //         _weapons[i].rotation = Quaternion.Slerp(_weapons[i].rotation, lookRot, Time.deltaTime /** orientationSpeed */);
    //         // EnemyUtils.DrawPath(_weapons[i].position, projectileData[i]);
    //     }
    // }

    void Fire(Transform firePoint, Vector3 velocity)
    {
        AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_CannonFire, firePoint.position, true, true);

        VFXObj e = VFXPool.GetInstanceOf(cannonFireInfo.Prefab, firePoint, Vector3.zero, Quaternion.Euler(0, -90f, 0));
        e.Initialise();

        var projectile = ProjectilePool.GetInstanceOf(projectileInfo, firePoint.position, velocity);
    }



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

        aiState = BomberState.FOLLOW;
    }
    public override void OnLostTarget()
    {
        OnLostTargetEvent();

        aiState = BomberState.PATROL;
    }

    #endregion

    public override void OnDie()
    {
        if (_boatMovement._PatrolPath != null)
        {
            _boatMovement._PatrolPath.UserDeath();
        }

        base.OnDie();

        // Drop items
        for (int i = 0; i < EnemyStats.dropItemNum; i++)
        {
            CargoInfo randomCargo = EnemyStats.DroppableCargo[Random.Range(0, EnemyStats.DroppableCargo.Count)];
            Vector3 pos = transform.position + GameUtils.GetTargetOffset(5) + Vector3.up * Random.Range(3, 5);
            BaseCargoPool.GetInstanceOf(randomCargo, PropState.FLOATING, pos);
        }

        // this will return it to the pool in enemyManager

        VFXObj e = VFXPool.GetInstanceOf(deathExplosionInfo.Prefab, transform.position);
        e.Initialise();

        EnemyManager.ReturnInstanceOf(EnemyStats.Prefab, this);
    }

}