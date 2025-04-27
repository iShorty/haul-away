using UnityEngine;



[SelectionBase, RequireComponent(typeof(Rigidbody))]
public abstract class EnemyShipController : Enemy {
    [Header("Ship Controller Info")]
	public float _MinDist = 10f;
    public float bumpForce = 1000f;



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

    #region Public Wrappers For Event Wrappers, because I'm smaht.

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
