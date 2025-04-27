using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CannonTurret : Enemy
{
    [Header("Cannon Attack Fields")]
    public Transform _Weapon;
    [Tooltip("Minimum duration between two attacks")]
    [SerializeField] float delayBetweenAttacks = 0.5f;
    [SerializeField, Tooltip("Vary attacked position in this radius.")]
    float AttackRadius = 1.5f;

    // [SerializeField] Projectile ProjectilePrefab = default;
    [SerializeField] ProjectileInfo Info = default;

    LaunchData[] projectileData;
    int iterationAhead;
    float _lastAttacked = Mathf.NegativeInfinity;
    BomberMovement _boatMovement;
    
    public override void GameAwake() {
        _detectionModule.Initialise();
        base.GameAwake();
    }





    
    #region Enable / Disable

    protected override void OnEnable() {
        base.OnEnable();
        
        _Health._OnDie += OnDie;
        _Health.onDamaged += OnDamaged;
        _detectionModule.onDetectedTarget += OnDetectedTarget;
        _detectionModule.onLostTarget += OnLostTarget;
    }

    protected override void OnDisable() {
        base.OnDisable();
        
        _Health._OnDie -= OnDie;
        _Health.onDamaged -= OnDamaged;
        _detectionModule.onDetectedTarget -= OnDetectedTarget;
        _detectionModule.onLostTarget -= OnLostTarget;
    }

    #endregion

    #region Public Wrappers For Event Wrappers

    public virtual void OnAttack()
    {
        OnAttackEvent();
    }
    public virtual void OnDamaged(GameObject source, int damage) {
        OnDamagedEvent(source, damage);
    }
    public virtual void OnDetectedTarget()
    {
        OnDetectedTargetEvent();
    }
    public virtual void OnLostTarget()
    {
        OnLostTargetEvent();
    }

    #endregion

}
