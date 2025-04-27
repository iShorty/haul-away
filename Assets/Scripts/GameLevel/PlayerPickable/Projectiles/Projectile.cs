using UnityEngine;

public abstract class Projectile : PlayerPickable
{

    // [field: Header("===== Projectile Info =====")]
    // [field: SerializeField, RenameField(nameof(ProjectileInfo))]
    // public ProjectileInfo ProjectileInfo { get; private set; } = default;
    // public override PlayerPickableInfo PickableInfo => ProjectileInfo;



    public virtual void Initialize(Vector3 velocity)
    {
        PlayerPickableManager.RegisterPlayerPickable(this);
        PropRigidBody.useGravity = true;
        PropRigidBody.velocity = velocity;
    }

    protected override void Awake()
    {
        base.Awake();
        #if UNITY_EDITOR
        if (GetType() != typeof(PlayerCannonProjectile))
        {
            Debug.Assert(_collider.gameObject.layer == Constants.For_Layer_and_Tags.LAYERINDEX_ENEMYINTERACTABLE, $"The collider {_collider} does not have its layer set to EnemyInteractable!", _collider);
        }
#endif
        // PlayerPickableManager.RegisterProjectile(this);
        PropRigidBody.interpolation = RigidbodyInterpolation.None;

    }
    public override bool GameUpdate()
    {
        // Don't use base.GameUpdate(), it handles water physics- NA to this. 
        // Override to block, do anything needed here, then call base for inherited.
        return false;
    }

    protected override void OnSinkTimerUp() { }
    protected override void RegisterToUpdateLoop() { }

}