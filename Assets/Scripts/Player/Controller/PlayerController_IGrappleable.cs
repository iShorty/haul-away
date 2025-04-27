using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    //========= IGRAPPLEABLE =========
    public Transform Transform => transform;

    public bool IsGrappleableInteractable => isSwimming;

    PlayerState _beforeGrapplingState = default;

    #region Handle IGrappleable

    public void EnterDetection()
    {
        //Turn on outline or smth
    }

    public void LeaveDetection()
    {
        //Turn off outline or smth
    }

    public void RescueGrappleableInteraction()
    {
        //Set player's rb to iskinematic 
        _beforeGrapplingState = _playerState;
        SetPropState(PropState.KINEMATIC);
        PlayerStates_SetPlayerState(PlayerState.INACTIVE);
        ReturnOffBoatIndicator();
    }

    public void TargetGrappleableInteraction(int stationIndex) { }

    public IGrappleable GetRootGrappleable()
    {
        return this;
    }

    public void LeaveGrapplingInteraction()
    {
        //Set player from is kinematic to onland
        SetPropState(PropState.ONLAND);
        PropRigidBody.transform.rotation = Quaternion.identity;
        PlayerStates_SetPlayerState(_beforeGrapplingState);
    }

    #endregion

}
