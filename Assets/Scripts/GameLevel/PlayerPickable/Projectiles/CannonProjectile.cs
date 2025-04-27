using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManagement;

public class CannonProjectile : Projectile
{
    [field: Header("===== Cannon Info =====")]

    [field: SerializeField, RenameField(nameof(CannonInfo))]
    public CannonProjectileInfo CannonInfo { get; private set; } = default;
    public override PlayerPickableInfo PickableInfo => CannonInfo;


#if UNITY_EDITOR
    [Header("----- Runtime -----")]
    [SerializeField, ReadOnly]
#endif
    bool impacted;


    // public override void Initialize(Vector3 launchPoint, Vector3 velocity)
    // {
    //     base.Initialize(launchPoint, velocity);

    //     // Initialise VFX, trails, etc?

    // }


    protected override void Awake()
    {
        base.Awake();
        // Initialise VFX, trails, etc?
    }

    public override void Initialize(Vector3 velocity)
    {
        impacted = false;
        base.Initialize(velocity);
    }

    public override bool GameUpdate()
    {
        if (transform.position.y < Constants.KILLHEIGHTMIN || transform.position.y > Constants.KILLHEIGHTMAX)
        {
            // Destroy(gameObject);
            ProjectilePool.ReturnInstanceOf(this);
            return true;
        }
        return false;
    }


    // Impact boat, player
    private void OnCollisionEnter(Collision collision)
    {
        // First time?
        if (impacted == false)
        {
            EvaluateCannonHit(collision);
            // AudioManager.theAM.PlaySFX("Boat Impact");
            AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_BoatImpact, transform.position, true, true);
            impacted = true;
        }
    }

    // On first impact
    // if boat, ApplyImpulse
    // if player, KO && Impulse
    void EvaluateCannonHit(Collision collision)
    {
        // Apply to player boat or apply to enemy boat
        if (BoatManager.IsPartOfBoat(collision.collider) || EnemyManager.IsEnemy(collision.collider))
        {
            GameUtils.ApplyImpulse(collision, transform, CannonInfo.Force * CannonInfo.BoatForceScalar);
            return;
        }

        //Only Player, cargo will be affected by bombs
        IBombable bombable = collision.collider.attachedRigidbody?.GetComponent<IBombable>();

        // Affect player and cargo
        // We use bomb blast because its cool. And because it already ropes in the player stun methods.
        // And because getting hit directly by a cannon is probably worse than getting hit with a brick of C4.
        if (bombable != null)
        {
#if UNITY_EDITOR
            Debug.Log("hit player or cargo " + collision.gameObject.name, collision.collider);
#endif
            bombable.BombBlast(CannonInfo.Force * CannonInfo.PlayerForceScaler, transform.position, CannonInfo.Radius, Constants.BOMB_UPWARDSMODIFIER);
            return;
        }
    }




}
