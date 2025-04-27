using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>Just handles the updating of various player pickable instances like bombs, cannonprojectiles ,etc</Summary>
public class PlayerPickableManager : GenericManager<PlayerPickableManager>
{
#if UNITY_EDITOR
    [Header("----- Runtime ----- ")]
    [Header("===== PLAYERPICKABLE MANAGER =====")]
    [SerializeField]
    [ReadOnly]
#endif
    List<PlayerPickable> _updatingPickables = default;


    #region HiddenFields



    #endregion



    #region Static

    public static bool IsGrappleable(Collider c)
    {
        if (c.attachedRigidbody == null) return false;
        return !c.attachedRigidbody.CompareTag(Constants.For_Layer_and_Tags.TAG_UNGRAPPLEABLE);
    }

    public static bool IsCargo(Collider c)
    {
        if (c.attachedRigidbody == null) return false;
        return c.attachedRigidbody.CompareTag(Constants.For_Layer_and_Tags.TAG_CARGO);
    }

    public static bool IsFuel(Collider c)
    {
        if (c.attachedRigidbody == null) return false;
        return c.attachedRigidbody.CompareTag(Constants.For_Layer_and_Tags.TAG_FUEL);
    }

    ///<Summary>Call this method if you want your playerpickable instance to be updated every frame by PlayerPickable Manager</Summary>
    public static void RegisterPlayerPickable(PlayerPickable pickable)
    {
        instance._updatingPickables.Add(pickable);
    }

    // ///<Summary>Call this method if you want your cargo to be updated every frame by PlayerPickable Manager</Summary>
    // public static void RegisterCargo(BaseCargo item)
    // {
    //     instance._updatingPickables.Add(item);
    // }

    // ///<Summary>Call this method if you want your projectile to be updated every frame by PlayerPickable Manager</Summary>
    // public static void RegisterProjectile(Projectile projectile)
    // {
    //     instance._updatingPickables.Add(projectile);
    // }


    #endregion

    #region Manager Methods
    protected override void OnGameAwake()
    {
        _updatingPickables = new List<PlayerPickable>();
        GlobalEvents.OnGameUpdate_DURINGGAME += DuringGameUpdate;
        GlobalEvents.OnGameFixedUpdate_DURINGGAME += DuringGameFixedUpdate;

    }

    public override void OnDestroy()
    {
        GlobalEvents.OnGameUpdate_DURINGGAME -= DuringGameUpdate;
        GlobalEvents.OnGameFixedUpdate_DURINGGAME -= DuringGameFixedUpdate;
    }

    #region Updates
    void DuringGameUpdate()
    {
        for (int i = 0; i < _updatingPickables.Count; i++)
        {
            PlayerPickable p = _updatingPickables[i];

            if (p.GameUpdate())
            {
                _updatingPickables.RemoveEfficiently(i);
                i--;
            }
        }
    }

    private void DuringGameFixedUpdate()
    {
        for (int i = 0; i < _updatingPickables.Count; i++)
        {
            _updatingPickables[i].FixedGameUpdate();
        }
    }



    #endregion

    #endregion
}
