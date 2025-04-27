using UnityEngine;



public class Health : MonoBehaviour, IDamageable
{
    [Tooltip("Maximum amount of health")]
    [SerializeField] int _MaxHealth = 1;
    
    public int _Health
    {
        get
        {
            return _health;
        } 
        set
        {
            _health = Mathf.Clamp(value, 0, _MaxHealth);
        } 
    }
    [SerializeField, ReadOnly] private int _health;

    public System.Action<GameObject, int> onDamaged;
    public System.Action _OnDie;

    public bool _IsDead => _Health <= 0 ? true : false;

    private void Start() {
        _Health = _MaxHealth;
    }

    public void ApplyDamageWithSource(GameObject source, int damage) {
        ApplyDamage(damage);
        onDamaged?.Invoke(source, damage);
    }

    public void ApplyDamage(int damage) {
        _Health -= damage;

        if (_IsDead)
        {
            HandleDeath();
        }
    }

    // Only for autokill
    public void Kill() {
        _Health = 0;
        onDamaged?.Invoke(null, _MaxHealth);

        if (_IsDead)
        {
            HandleDeath();
        }
    }

    private void HandleDeath() {
        _OnDie?.Invoke();
    }
}
