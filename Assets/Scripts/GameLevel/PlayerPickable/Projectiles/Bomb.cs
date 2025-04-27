using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManagement;

public class Bomb : Projectile
{
    #region ---------- Constants ------------
    const float TARGET_ENDALPHA = 0.25f
    , TARGET_STARTALPHA = 0
    ;
    #endregion

    [field: Header("===== Bomb Info =====")]
    [field: SerializeField, RenameField(nameof(BombInfo))]
    public BombProjectileInfo BombInfo { get; private set; } = default;

    public override PlayerPickableInfo PickableInfo => BombInfo;
    [SerializeField]
    MeshRenderer _bombFlashRenderer = default;

#if UNITY_EDITOR
    [ReadOnly, Header("---- Runtime -----"), SerializeField]
#endif
    float _bombTimer = default;

    Color _targetColor = default;

    float _nextFlashTimer = default;
    float _flashTimer = default;

    ///<Summary>When set to true, the bomb is lerping its emission material's alpha towards 1 else 0</Summary>
    bool _flashingRed = false;

    protected override void Awake()
    {
        base.Awake();
        Color flashingColor = Color.red;
        flashingColor.a = TARGET_STARTALPHA;
        _bombFlashRenderer.material.SetColor("_EmissionColor", Color.red);
        _bombFlashRenderer.material.SetColor("_BaseColor", flashingColor);
        _bombFlashRenderer.material.renderQueue = 3000;

        // PlayerPickableManager.RegisterBomb(this);
        // mat = GetComponent<Renderer>().material;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _targetColor = Color.red;
        _targetColor.a = 1;
        _flashingRed = true;
        _flashTimer = _nextFlashTimer = BombInfo.FlashInterval;
    }

    public override void Initialize(Vector3 velocity)
    {
        base.Initialize(velocity);
        // startTime = Time.time;
        _bombTimer = BombInfo.Delay;
        transform.rotation = Random.rotation;
        Spin();
    }

    void Spin()
    {
        PropRigidBody.AddTorque(transform.up * Random.Range(BombInfo.SpinMin, BombInfo.SpinMax), ForceMode.VelocityChange);
        PropRigidBody.AddTorque(transform.right * Random.Range(BombInfo.SpinMin, BombInfo.SpinMax), ForceMode.VelocityChange);
    }

    public override bool GameUpdate()
    {
        if (_bombTimer <= 0)
        {
            Explode();
            ProjectilePool.ReturnInstanceOf(this);
            // Play SFX, VFX, anim, etc
            AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_BombExplosion, transform.position, true, true);
            return true;
        }

        FlashBomb();
        _bombTimer -= Time.deltaTime;
        return false;
    }

    public void Explode()
    {
        // Give a Manager for the bombs a collider[] for overlapspherenonalloc
        Collider[] colliders = Physics.OverlapSphere(transform.position, BombInfo.Radius, Constants.For_Layer_and_Tags.LAYERMASK_PROJECTILE, QueryTriggerInteraction.Collide);

        foreach (Collider obj in colliders)
        {
            //Only Player, cargo will be affected by bombs
            IBombable bombable = obj.attachedRigidbody?.GetComponent<IBombable>();

            if (bombable == null)
            {
                continue;
            }

            if (obj.attachedRigidbody.TryGetComponent(out Bomb bomb))
            {
                bomb.Explode();
                continue;
            }

            if (obj.attachedRigidbody.TryGetComponent(out SeaMine mine))
            {
                mine.Explode();
                continue;
            }

            bombable.BombBlast(BombInfo.Force * Constants.For_Projectiles.BOMB_SCALAR, transform.position, BombInfo.Radius, Constants.BOMB_UPWARDSMODIFIER);
        }

        VFXObj e = VFXPool.GetInstanceOf(BombInfo.ExplosionPrefab, transform.position, Quaternion.identity);
        e.Initialise();
    }


    ///<Summary>Lerps the bomb's emission material to show that the bomb is going to explode</Summary>
    void FlashBomb()
    {
        _flashTimer -= Time.deltaTime;

        if (_flashTimer > 0)
        {
            return;
        }

        //----------- Reached 0 --------------
        _nextFlashTimer *= BombInfo.IntervalMultiplier;
        _flashTimer = _nextFlashTimer;
        _flashingRed = !_flashingRed;

        _targetColor.a = _flashingRed ? TARGET_STARTALPHA : TARGET_ENDALPHA;
        _bombFlashRenderer.material.SetColor("_BaseColor", _targetColor);

    }



}

