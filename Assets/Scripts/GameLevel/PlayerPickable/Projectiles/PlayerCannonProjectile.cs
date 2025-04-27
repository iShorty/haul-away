using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManagement;

///<Summary>The projectile in which the player cannon station will fire at the enemy</Summary>
public class PlayerCannonProjectile : Projectile
{
    [field: Header("===== PLAYER CANNON PROJECTILE ===== ")]
    [field: SerializeField, RenameField(nameof(Info))]
    public PlayerCannonProjectileInfo Info { get; protected set; } = default;
    public override PlayerPickableInfo PickableInfo => Info;

    #region Runtime
#if UNITY_EDITOR
    [SerializeField, ReadOnly]
#endif
    float _timer = default;

#if UNITY_EDITOR
    [SerializeField, ReadOnly]
#endif
    bool _impacted = default;

    #endregion

    public override void Initialize(Vector3 velocity)
    {
        base.Initialize(velocity);
        _timer = Info.LifeTime;
    }


    public override bool GameUpdate()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            return false;
        }
        ProjectilePool.ReturnInstanceOf(this);
        return true;
    }

    // Impact boat, player
    private void OnCollisionEnter(Collision collision)
    {
        // First time?
        if (!_impacted)
        {
            if (BoatManager.IsPartOfBoat(collision.collider)) return;

            _impacted = true;
            EvaluateCannonHit(collision);
            // AudioManager.theAM.PlaySFX("Boat Impact");
            AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_BoatImpact, transform.position, true, true);
        }
    }

    private void EvaluateCannonHit(Collision collision)
    {
        Collider c = collision.collider;
        if (EnemyManager.IsEnemy(c))
        {
            Enemy enemy = c.attachedRigidbody.GetComponent<Enemy>();

#if UNITY_EDITOR
            Debug.Assert(enemy != null, $"The collider's attached rigidbody {c.attachedRigidbody} does not have an Enemy class attached to it!", c.attachedRigidbody);
#endif

            VFXObj e = VFXPool.GetInstanceOf(Info.enemyHitVFXInfo.Prefab, transform.position, Quaternion.Euler(collision.contacts[0].normal));
            e.Initialise();

            GameUtils.ApplyImpulse(collision, transform, Info.Force);
            enemy._Health.ApplyDamageWithSource(gameObject, Info.Damage);
        }


    }
}


