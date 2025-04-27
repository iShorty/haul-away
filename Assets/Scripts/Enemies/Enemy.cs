using UnityEngine;
using UnityEngine.AI;



[RequireComponent(typeof(Health), typeof(NavMeshAgent), typeof(DetectionModule))]
public abstract class Enemy : FloatableProp, IBombable
{
    // Make enemies accomodate
    // OnGameStart
    // OnGameEnd
    // OnGamePause
    // OnGameResume

    [Header("===== ENEMY INFO =====")]
    public Health _Health;
    public EnemyInfo EnemyStats;

    #region Events
    public event System.Action onAttack = null;
    public event System.Action onDetectedTarget = null;
    public event System.Action onLostTarget = null;
    public event System.Action<GameObject, int> onDamaged = null;

    #endregion


    public GameObject KnownDetectedTarget => _detectionModule.knownDetectedTarget;
    public Vector3 LastKnownDetectedTargetPos => _detectionModule.lastKnownDetectedTargetPos;
    public float SqredMinDetectionUpdateDist => _detectionModule.SqredMinUpdateDist;
    public bool IsTargetInAttackRange => _detectionModule.isTargetInAttackRange;
    public float AttackRange => _detectionModule.attackRange;
    public bool IsSeeingTarget => _detectionModule.isSeeingTarget;
    protected bool IsDead => _Health._IsDead;


    [field: Tooltip("Delay after death where the GameObject is destroyed (to allow for animation)")]
    protected float deathDuration { get; private set; } = 0f;
    [SerializeField] protected DetectionModule _detectionModule;
    public DetectionModule _DetMod => _detectionModule;
    [SerializeField] protected Collider[] _selfColliders;
    protected EnemyManager _enemyManager;



    private void Start()
    {
        GameAwake();
    }

    // Should be overridden. Listing out what should be in start for derived classes.
    public override void GameAwake()
    {
        // Don't call base/FloatableProp GameAwake. Do not turn off the floater group.

        PropRigidBody = GetComponent<Rigidbody>();
        _floaterGroup = GetComponent<BaseFloaterGroup>();

        _selfColliders = GetComponentsInChildren<Collider>();

        _enemyManager = EnemyManager.Instance;
        _Health = GetComponent<Health>();
        _detectionModule = GetComponent<DetectionModule>();
    }

    protected override void OnEnable()
    {
        EnemyManager.RegisterEnemy(this);
    }

    protected override void OnDisable()
    {
        EnemyManager.UnregisterEnemy(this);
    }

    // Do not use base.GameUpdate, it does sinking behaviour after sinkDuration in water. No thanks for enemy boats.
    public override bool GameUpdate()
    {
        if (!IsDead)
        {
            if (transform.position.y < Constants.KILLHEIGHTMIN || transform.position.y > Constants.KILLHEIGHTMAX)
            {
                _Health.Kill();
            }
        }
        return false;
    }

    #region Wrappers to raise base class events

    protected void OnAttackEvent()
    {
        onAttack?.Invoke();
    }

    protected void OnDamagedEvent(GameObject source, int damage)
    {
        _detectionModule.OnDamaged(source);
        onDamaged?.Invoke(source, damage);
    }

    protected void OnDetectedTargetEvent()
    {
#if UNITY_EDITOR
        Debug.Log("OnDetectedTargetEvent ", this);
#endif
        onDetectedTarget?.Invoke();
    }

    protected void OnLostTargetEvent()
    {
        onLostTarget?.Invoke();
    }

    #endregion

    public virtual void OnDie()
    {
        // Drop items
        for (int i = 0; i < EnemyStats.dropItemNum; i++)
        {
            CargoInfo randomCargo = EnemyStats.DroppableCargo[Random.Range(0, EnemyStats.DroppableCargo.Count)];
            Vector3 pos = transform.position + GameUtils.GetTargetOffset(5) + Vector3.up * Random.Range(5, 10);
            BaseCargoPool.GetInstanceOf(randomCargo, PropState.FLOATING, pos);
        }

        // Return to pooler
        EnemyManager.ReturnInstanceOf(EnemyStats.Prefab, this);
    }


    #region IBombable

    // public abstract void BombBlast(float force, Vector3 bombPosition, float blastRadius, float upwardsModifier);
    public virtual void BombBlast(float force, Vector3 bombPosition, float blastRadius, float upwardsModifier)
    {
        _Health.ApplyDamage(1);
    }

    #endregion

    #region FloatableProp Methods

    // Not needed, but needs to be implemented from base class
    protected override void OnSinkTimerUp() { }
    protected override void RegisterToUpdateLoop() { }

    #endregion


}
