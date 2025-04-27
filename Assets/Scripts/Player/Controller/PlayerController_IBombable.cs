using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    public VFXInfo stunInfo = default;

    #region IBombable
    public void BombBlast(float force, Vector3 bombPosition, float blastRaidus, float upwardsModifier)
    {
        bool shouldBeBlasted = false;

        switch (_currentPropState)
        {
            case PropState.ONLAND:
                shouldBeBlasted = Player_BombBlast_ONLAND_or_FLOATING(force, bombPosition, blastRaidus, upwardsModifier);
                break;

            case PropState.FLOATING:
                force *= Constants.BOMB_INWATER_FORCE_DAMPENING;
                shouldBeBlasted = Player_BombBlast_ONLAND_or_FLOATING(force, bombPosition, blastRaidus, upwardsModifier);
                break;

            case PropState.KINEMATIC:
                shouldBeBlasted = Player_BombBlast_KINEMATIC();
                break;

            default:
                //======== STATES THAT SHOULDNT BE BLASTED: ====================
                //SINKING, INWATER
#if UNITY_EDITOR
                Debug.Log($"Player {name}, of index {PlayerIndex} got hit by a bomb while in {_currentPropState} Prop State. Is this normal?", this);
#endif
                return;
        }

        if (!shouldBeBlasted) return;


        //Start timer for ragdoll effect to be over. Need to clear movement input & desired velocity so that player doesnt start sliding
        Movement_ClearMoveInput();

        
        VFXObj e = VFXPool.GetInstanceOf(stunInfo.Prefab, transform, Vector3.up * 1.5f);
        e.Initialise();

        PropRigidBody.AddExplosionForce(force, bombPosition, blastRaidus, upwardsModifier);
        _playerTimer = StatsInfo.StunDuration;
        _anim.SetTrigger(Constants.For_Player.ANIMATOR_PARAM_ENTERDAZED);
        PlayerStates_SetPlayerState(PlayerState.STUNNED);
    }

    ///<Summary>Does a switch case and execute appropriate methods to when player is blasted by a bomb when prop state is KINEMATIC. Returns true if player is in the state to get blasted by a bomb</Summary>
    private bool Player_BombBlast_KINEMATIC()
    {
        switch (_playerState)
        {

            case PlayerState.IN_STATION:
                //Leave station first then get blasted
                Interaction_LeaveStation(true);
                return true;


            //=========== STATES THAT DONT ALLOW PLAYER TO BE BLASTED: ==============
            case PlayerState.STUNNED:
                return false;

            case PlayerState.INACTIVE:
                return false;

            //========== STATES WHICH SHOULDNT BE POSSIBLE ============
            default:
                //NONE, PICKEDUP_ITEM
#if UNITY_EDITOR
                Debug.LogError($"Player {PlayerIndex} should not have the code flowed into here when player is in {_playerState}", this);
#endif
                return true;

        }

    }

    ///<Summary>Does a switch case and execute appropriate methods to when player is blasted by a bomb when prop state is ONLAND or FLOATING</Summary>
    private bool Player_BombBlast_ONLAND_or_FLOATING(float force, Vector3 bombPosition, float blastRadius, float upwardsModifier)
    {
        switch (_playerState)
        {
            case PlayerState.NONE:
                //do nth
                return true;

            case PlayerState.PICKEDUP_ITEM:
                //drop item first then get blasted
                Interaction_TryLeaveInteraction(true);

                //Item the player is holding could be cargo or it could be another projectile
                //blast the item also
                IBombable pickable = _currInteract.Transform.GetComponent<IBombable>();

                //If the item player is holding is bombable, apply bomb
                if (pickable != null)
                {
                    pickable.BombBlast(force, bombPosition, blastRadius, upwardsModifier);
                }

                return true;

            case PlayerState.PICKING_UP_ITEM:
                //drop item first then get blasted
                Interaction_TryLeaveInteraction(true);
                return true;


            //=========== STATES THAT DONT ALLOW PLAYER TO BE BLASTED: ==============
            case PlayerState.STUNNED:
                return false;

            //========== STATES WHICH SHOULDNT BE POSSIBLE ============
            default:
                //IN_STATION, GRAPPLED
#if UNITY_EDITOR
                Debug.LogError($"Player {PlayerIndex} should not have the code flowed into here when player is in {_playerState}", this);
#endif
                return true;
        }
    }
    #endregion

}
