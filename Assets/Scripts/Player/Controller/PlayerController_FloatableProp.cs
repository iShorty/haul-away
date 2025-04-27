using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    //====== FLOATABLE PROPS ======
    // protected override float floatDuration => Constants.For_Player.FLOATDELAY;
    // protected override float sinkDuration => Constants.For_Player.SINKDELAY;

    PlayerOffBoatIndicator _offBoatIndicator = default;

    #region Handle FloatableProp Methods
    void FloaterProp_OnEnable()
    {
        _floaterGroup.enabled = false;
        PropRigidBody.transform.rotation = Quaternion.identity;
        SetPropState(PropState.ONLAND);
        PropRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FloaterProp_OnDisable()
    {
        if (_offBoatIndicator)
        {
            ReturnOffBoatIndicator();
        }
    }


    //na dont need to do anything to subscribe to an update loop. Playercontroller got dis
    protected override void RegisterToUpdateLoop() { }


    #region ---------------- Sinking and Respawning -------------------
    protected override void OnSinkTimerUp()
    {
        //Handle respawn onto boat behaviour here
        SetPropState(PropState.KINEMATIC);
        PlayerStates_SetPlayerState(PlayerState.INACTIVE);
        // this.enabled = false;
        PlayerManager.StartRespawnPlayer(PlayerIndex);
    }

    ///<Summary>To be called by the PlayerManager_RespawnChecker only to respawn the player if they are too far away from the boat</Summary>
    public void StartRespawn()
    {
        if (_offBoatIndicator)
        {
            ReturnOffBoatIndicator();
        }

        //Check if player is holding object. if he is, drop it
        //Drop item
        Interaction_TryLeaveInteraction(true);
        // Interaction_TryDropItem(false);
        OnSinkTimerUp();
    }

    public void EndRespawn()
    {
        SetPropState(PropState.ONLAND);
        _parentRb = BoatManager.Controller._rigidBody;
        _playerPrevWorldPosition = PropRigidBody.position;
        _playerPrevLocalPosition = _parentRb.transform.InverseTransformPoint(_playerPrevWorldPosition);
        PlayerStates_SetPlayerState(PlayerState.ENDRESPAWN);
        // this.enabled = true;
    }

    #endregion

    #region ------------------- Set PropState ----------------------
    protected override void SetPropStateTo_SINKING()
    {
        base.SetPropStateTo_SINKING();

        switch (_playerState)
        {

            case PlayerState.PICKEDUP_ITEM:
                //Drop item
                Interaction_TryDropItem(false);
                break;

            //=== NOTHING HAPPENS =====
            case PlayerState.NONE: break;

            //========= STATES WHICH CANT BE POSSIBLE =========
            default:
                //IN_STATION, GRAPPLED
                break;


        }

        //Return the offboatindicator
        ReturnOffBoatIndicator();
    }

    protected override void SetPropStateTo_FLOATING()
    {
        base.SetPropStateTo_FLOATING();
        _playerCollider.isTrigger = false;

        //Get a offboatindicator
        _offBoatIndicator = UIIndicatorPool.GetIndicator(OffBoatIndicatorInfo, PlayerManager.PlayerCanvas.transform, PropRigidBody) as PlayerOffBoatIndicator;
        _offBoatIndicator.SetPlayerIndexUI(PlayerIndex);
        // base.PropRigidBody.constraints = RigidbodyConstraints.None;
    }

    protected override void SetPropStateTo_ONLAND()
    {
        _playerCollider.isTrigger = false;
        // base.PropRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        // base.PropRigidBody.transform.rotation = Quaternion.identity;
        base.SetPropStateTo_ONLAND();
    }

    protected override void SetPropStateTo_KINEMATIC()
    {
        _playerCollider.isTrigger = true;
        base.SetPropStateTo_KINEMATIC();
    }
    #endregion

    protected override bool Prop_GameUpdate_FLOATING()
    {
        _offBoatIndicator.SetImageFill(1-(_propTimer / _floaterGroup.SinkInfo.FloatDuration));
        return base.Prop_GameUpdate_FLOATING();
    }

    public override void Prop_OnEvaluateWaterTrigger()
    {
        //Need to do smth abt this trigger because the waves' amplitude can cause the players to drop into the water trgiger
        //Player is on boat
        if (BoatManager.IsPropOnBoat(PropRigidBody)) return;


        // player should be considered as floating state when?
        // he is not being rescued by grappling hook
        // he is not sinking
        // he is not in station
        // he got blown off the ship and set into ragdolled mode
        switch (_currentPropState)
        {
            //======= STATES THAT ALLOW ENTERING FLOATING STATE ===========
            case PropState.ONLAND:
                //Player can be HOLDING ITEM or in NONE state
                break;

            //======= STATES THAT DENY ENTERING FLOATING STATE ===========
            default:
                //KINEMATIC, SINKING , FLOATING, INWATER
                return;


        }

        //If player is holding onto a cargo in the water, the cargo doesnt sink. Player sinks instead.
        //once player has completely sunken, player will let go of cargo and cargo will being its sinking 
        SetPropState(PropState.FLOATING);
    }

    #endregion

    #region Supporting Methods
    void ReturnOffBoatIndicator()
    {
        //Return sinktimer indiator
        UIIndicatorPool.TryRemoveIndicator(PropRigidBody);
        _offBoatIndicator = null;
    }

    #endregion

}