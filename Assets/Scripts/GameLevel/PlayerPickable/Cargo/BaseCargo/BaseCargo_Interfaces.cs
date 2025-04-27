using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseCargo
{
    public bool IsGrappleableInteractable => _currentPropState != PropState.SINKING;


    ///<Summary>If true, Player or Grapplinghook could be holding onto this pickable object. This also implies that the current PropState is KINEMATIC </Summary>
    protected override bool isSomeOneHoldingMe => base.isSomeOneHoldingMe;

    ///<Summary>If true, pickable object is not sinking and doesnt have any player or grapplinghook holding it (that means the current propstate is not in KINEMATIC ) </Summary>
    public override bool IsPlayerInteractable => base.IsPlayerInteractable;

    #region IGrappleable
    public void RescueGrappleableInteraction()
    {
        SetPropState(PropState.KINEMATIC);
        ReturnSinkIndicator();
    }

    public void LeaveGrapplingInteraction()
    {
        LeavePlayerInteraction(false);
    }

    public IGrappleable GetRootGrappleable()
    {
        return isSomeOneHoldingMe ? PlayerManager.GetPlayer(_holderIndex).GetRootGrappleable() : this;
    }

    public void TargetGrappleableInteraction(int stationIndex)
    {
        _holderIndex = stationIndex;

        LeaveDetection();
    }
    #endregion

    #region IBombable
    public void BombBlast(float force, Vector3 bombPosition, float blastRadius, float upwardsModifier)
    {
        //Return when in kinematic state
        //if player is holding onto this cargo, player will handle the blasting logic
        if (isSomeOneHoldingMe) return;


        switch (_currentPropState)
        {
            //======= STATES THAT ALLOW CARGO TO BE BLASED ============
            case PropState.ONLAND:
                break;

            case PropState.FLOATING:
                force *= Constants.BOMB_INWATER_FORCE_DAMPENING;
                break;

            case PropState.INWATER:
                force *= Constants.BOMB_INWATER_FORCE_DAMPENING;
                break;

            //======= STATES THAT DENY CARGO TO BE BLASED ============
            default:
                //SINKING, KINEMATIC
                return;
        }


        PropRigidBody.AddExplosionForce(force, bombPosition, blastRadius, upwardsModifier);
    }
    #endregion


    void ReturnSinkIndicator()
    {
        //Return sinktimer indiator
        UIIndicatorPool.TryRemoveIndicator(PropRigidBody);
        _sinkTimerIndicator = null;
    }

}
