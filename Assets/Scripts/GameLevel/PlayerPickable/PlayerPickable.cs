using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerPickable : FloatableProp, IPlayerInteractable
{
    protected Collider _collider = default;
    // [Header("===== PLAYERPICKABLE =====")]
    // [SerializeField]

    #region Hidden Fields 
    protected BaseMeshOutline _outlinedMesh = default;
    //Holder can either be a player or a multiusestation
    protected int _holderIndex = -1;
    #endregion


    ///<Summary>If true, Player could be holding onto this pickable object. This also implies that the current PropState is KINEMATIC </Summary>
    protected virtual bool isSomeOneHoldingMe => _holderIndex >= 0;

    public abstract PlayerPickableInfo PickableInfo { get; }

    #region IPlayerInteractable
    public virtual PlayerInteractableType PlayerInteractableType => PlayerInteractableType.ITEM;
    public Transform Transform => transform;


    ///<Summary>If true, pickable object is not sinking and doesnt have anyone holding it (that means the current propstate is not in KINEMATIC ) </Summary>
    public virtual bool IsPlayerInteractable => (_currentPropState != PropState.SINKING) && !isSomeOneHoldingMe;

    public Vector3 Size => PickableInfo.BoundingBox;

    public virtual void EnterDetection()
    {
        //Turn on outline
        _outlinedMesh.ToggleOutline(BaseMeshOutline.OutlineMode.PARTIALLYHIDDEN);
        // _outlinedMesh.gameObject.layer = Constants.For_Layer_and_Tags.LAYERINDEX_DETECTEDINTERACTABLE;
    }

    public virtual void LeaveDetection()
    {
        //Turn off outline
        _outlinedMesh.ToggleOutline(BaseMeshOutline.OutlineMode.OFF);
        // _outlinedMesh.gameObject.layer = Constants.For_Layer_and_Tags.LAYERINDEX_INTERACTABLE;

    }

    public virtual void LeavePlayerInteraction(bool forcefully)
    {
        _holderIndex = -1;
        SetPropState(PropState.ONLAND);
    }

    public virtual void TossInteraction(Vector3 force)
    {
        LeavePlayerInteraction(false);
        PropRigidBody.AddForce(force, ForceMode.Impulse);
    }

    public virtual void UsePlayerInteraction(int playerIndex)
    {
        SetPropState(PropState.KINEMATIC);
        _holderIndex = playerIndex;
        _outlinedMesh.ToggleOutline(BaseMeshOutline.OutlineMode.OFF);
    }

    public virtual bool UpdateInteract() { return true; }
    public virtual void FixedUpdateInteract() { }


    #endregion


    #region Floatable Prop Methods
    #region Lifetime Methods
    protected virtual void Awake()
    {
        _collider = GetComponentInChildren<Collider>();
        _outlinedMesh = GetComponentInChildren<BaseMeshOutline>();
#if UNITY_EDITOR
        if (PlayerPickableManager.IsCargo(_collider))
        {
            Debug.Assert(_collider.gameObject.layer == Constants.For_Layer_and_Tags.LAYERINDEX_PLAYERINTERACTABLE, $"The collider {_collider} does not have its layer set to Interactable!", _collider);
        }
        Debug.Assert(_outlinedMesh != null, $"The PlayerPickable {name} does not have an meshoutline  assigned!", this);
        Debug.Assert(PickableInfo != null, $"The playerpickable {name} does not have playerpickableinfo assigned!", this);
#endif
        _outlinedMesh.GameAwake();
        base.GameAwake();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //Sub to global events so that when game is paused, the pickable, regardless of being updated by a manager or not will be able to pause
        GlobalEvents.OnGamePause += HandlePause;
        GlobalEvents.OnGameResume += HandleResume;
        GlobalEvents.OnGameEnd += HandleGameEnd;
        _outlinedMesh.ToggleOutline(BaseMeshOutline.OutlineMode.OFF);
    }



    protected override void OnDisable()
    {
        base.OnDisable();
        GlobalEvents.OnGamePause -= HandlePause;
        GlobalEvents.OnGameResume -= HandleResume;
        GlobalEvents.OnGameEnd -= HandleGameEnd;
    }

    #region Global Events
    private void HandleResume()
    {
        if (!isSomeOneHoldingMe)
            PropRigidBody.isKinematic = false;

    }
    private void HandlePause()
    {
        PropRigidBody.isKinematic = true;
    }

    private void HandleGameEnd()
    {
        PropRigidBody.isKinematic = true;
        _floaterGroup.enabled = false;
    }
    #endregion

    #endregion

    #region Set Prop Methods
    protected override void SetPropStateTo_KINEMATIC()
    {
        _collider.isTrigger = true;
        base.SetPropStateTo_KINEMATIC();
        PropRigidBody.transform.rotation = Quaternion.identity;
    }

    protected override void SetPropStateTo_ONLAND()
    {
        _collider.isTrigger = false;
        base.SetPropStateTo_ONLAND();
    }

    protected override void SetPropStateTo_FLOATING()
    {
        _collider.isTrigger = false;
        base.SetPropStateTo_FLOATING();
    }

    public override void Prop_OnEvaluateWaterTrigger()
    {
        if (BoatManager.IsPropOnBoat(PropRigidBody)) return;

        if (isSomeOneHoldingMe) return;

        switch (_currentPropState)
        {
            //======= STATES THAT ALLOW ENTERING FLOATING STATE ===========
            case PropState.ONLAND:
                //Item is not held by anyone or grappling station
                break;

            //======= STATES THAT DENY ENTERING FLOATING STATE ===========
            default:
                //KINEMATIC
                //item can be grappled or held by an object
                // #if UNITY_EDITOR
                // Debug.Log("hi");
                // #endif
                //SINKING , FLOATING, INWATER
                //States which are lower or at the same level in the logical path of a sinking objects shuld be omitted
                return;
        }

        SetPropState(PropState.FLOATING);
    }

    #endregion

    #endregion

}
