using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManagement;

[RequireComponent(typeof(Health))]
public class SeaMine : FloatableProp, IBombable
{
    [Header("===== SEA MINE INFO =====")]
    [SerializeField]
    float blastForceForBoat = 250000f;
    [SerializeField]
    float blastForceForCargo = 5000f;
    [SerializeField]
    float blastRadius = 10f;

    [SerializeField] VFXInfo mineExplosionInfo = default;

    public static event Action<Collision> OnSeaMineExplode = null;

    private void Awake()
    {
#if UNITY_EDITOR
        Debug.Assert(CompareTag(Constants.For_Layer_and_Tags.TAG_SEAMINE), $"The seamine does not have a seamine tag!", this);
#endif
        GameAwake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        GlobalEvents.OnGamePause += HandleGamePause;
        GlobalEvents.OnGameResume += HandleGameResume;
        GlobalEvents.OnGameFixedUpdate_DURINGGAME += HandleGameFixedUpdate;
    }

    private void HandleGameFixedUpdate()
    {
        _floaterGroup.GameFixedUpdate();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GlobalEvents.OnGamePause -= HandleGamePause;
        GlobalEvents.OnGameResume -= HandleGameResume;
        GlobalEvents.OnGameFixedUpdate_DURINGGAME -= HandleGameFixedUpdate;
    }

    private void OnCollisionEnter(Collision other)
    {
        OnSeaMineExplode?.Invoke(other);
        AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_SeaMineExplosion, transform.position, true, true);
        Explode();
    }

    public void Explode()
    {
        // Give a Manager for the bombs a collider[] for overlapspherenonalloc
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius, Constants.For_Layer_and_Tags.LAYERMASK_PROJECTILE);

        foreach (Collider obj in colliders)
        {
            if (obj.gameObject == gameObject || BoatManager.IsStation(obj)) continue;

#if UNITY_EDITOR
            if (obj.attachedRigidbody)
            {
                Debug.Log($"Obj hit {obj.name} and Obj Rb: {obj.attachedRigidbody.name}", obj.attachedRigidbody);
            }
            else
            {
                Debug.Log($"Obj hit {obj.name}", obj);
            }
#endif
            IBombable bombable = obj.attachedRigidbody?.GetComponent<IBombable>();

            // if (bombable == null || Constants.For_Layer_and_Tags.LAYERINDEX_TERRAIN == (Constants.For_Layer_and_Tags.LAYERINDEX_TERRAIN | (1 << obj.gameObject.layer)))
            if (bombable == null)
            {
                continue;
            }

            // Scale the force back for players and cargo
            if (Constants.For_Layer_and_Tags.LAYERMASK_INTERACTABLE_FINALMASK.Contains(obj.gameObject.layer))
            {
                bombable.BombBlast(blastForceForCargo, transform.position, blastRadius, Constants.BOMB_UPWARDSMODIFIER);

                //Because Enemyinteractables is considered inside LAYERMASK_INTERACTABLE_FINALMASK
                if (obj.attachedRigidbody.TryGetComponent(out Bomb bomb))
                {
                    bomb.Explode();
                }

                continue;
            }

            //Else its assumed that its playerboat
            bombable.BombBlast(blastForceForBoat, transform.position, blastRadius, Constants.BOMB_UPWARDSMODIFIER);
        }

        VFXObj e = VFXPool.GetInstanceOf(mineExplosionInfo.Prefab, transform.position, Quaternion.identity);
        e.Initialise();

        Destroy(gameObject);
    }


    #region IBombable

    // If exploded, explode.
    public virtual void BombBlast(float force, Vector3 bombPosition, float blastRadius, float upwardsModifier)
    {
        Explode();
    }

    #endregion


    #region FloatableProp Methods

    protected override void OnSinkTimerUp()
    {
        // Return to enemy pool, destroy, etc
    }

    protected override void RegisterToUpdateLoop()
    {

    }

    #endregion

    private void HandleGamePause()
    {
        _floaterGroup.Rigidbody.isKinematic = true;
    }

    private void HandleGameResume()
    {
        _floaterGroup.Rigidbody.isKinematic = false;
    }

}

