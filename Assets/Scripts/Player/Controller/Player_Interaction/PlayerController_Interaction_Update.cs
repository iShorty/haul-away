using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManagement;
//This file holds methods for interaction for the update loop
public partial class PlayerController
{
    void Interaction_Update_NONE()
    {
        // Interaction_GatherInput();
        if (DesireInteract)
        {
            Interaction_TryUseInteraction();
        }
    }

    void Interaction_Update_INSTATION()
    {
        // Interaction_GatherInput();
        switch (_isSteeringWheel)
        {
            case true:
                _anim.SetInteger(Constants.For_Player.ANIMATOR_PARAM_STEERVALUE, Mathf.RoundToInt(MovementInput.x));
                break;

            case false:
                break;
        }

        //Update the station
        if (!_currInteract.UpdateInteract()) return;

        //===== LEAVE BUTTON PRESSED ==========
        if (DesireLeave)
        {
            Interaction_LeaveStation(false);
            return;
        }
    }

    private void Interaction_Update_PICKINGUPITEM()
    {
        _playerTimer -= Time.deltaTime;

        if (_playerTimer <= 0)
        {
            Interaction_PickedUpItem();
        }
    }


    void Interaction_Update_PICKEDUPITEM()
    {
        // Interaction_GatherInput();

        if (DesireLeave)
        {
            Interaction_TryDropItem(false);
            return;
        }

        if (DesireUse)
        {
            Interaction_TossItem();
            return;
        }

    }

    #region Input Methods

    // void Interaction_GatherInput()
    // {
    //     DesireUse = ControlInfo.GetUseInput();
    //     DesireToss = ControlInfo.GetTossInput();
    //     DesireLeave = ControlInfo.GetLeaveInput();
    //     DesireToggleLeft = ControlInfo.GetToggleLeftInput();
    //     DesireToggleRight = ControlInfo.GetToggleRightInput();
    // }

    #region Entering Interaction
    void Interaction_TryUseInteraction()
    {
        if (_currInteract == null) return;

        //==== USING INTERACTION ======
        _currInteract.UsePlayerInteraction(PlayerIndex);

        switch (_currInteract.PlayerInteractableType)
        {
            // ====== HANDLE INTERACTING WITH ITEM =============
            case PlayerInteractableType.ITEM:
                Interaction_PickUpItem();
                break;

            // ====== HANDLE INTERACTING WITH OVERRIDESTATION =============
            case PlayerInteractableType.OVERRIDESTATION:

                SetPropState(PropState.KINEMATIC);
                transform.SetParent(BoatManager.Controller.transform);
                PlayerStates_SetPlayerState(PlayerState.IN_STATION);
                _anim.SetFloat(Constants.For_Player.ANIMATOR_PARAM_VELOCITY_SQRMAG, 0);
                _isSteeringWheel = BoatManager.SteeringStation.transform == _currInteract.Transform;
                break;

            // ====== HANDLE INTERACTING WITH NONOVERRIDESTATION =============
            case PlayerInteractableType.NONEOVERRIDESTATION:
                _currInteract.LeavePlayerInteraction(false);
                break;

            default:
#if UNITY_EDITOR
                Debug.LogError("Code should not flow here!");
#endif
                break;
        }
    }

    ///<Summary>Allows player to pick up item from a public method</Summary>
    public void Interaction_PickUpItem(IPlayerInteractable item)
    {
        _currInteract = item;
        _currInteract.UsePlayerInteraction(PlayerIndex);
        Interaction_PickUpItem();
    }

    ///<Summary>Enters the player into picking up item state (where there is a delay before the object actually gets teleported to the head)</Summary>
    private void Interaction_PickUpItem()
    {
        _anim.SetTrigger(Constants.For_Player.ANIMATOR_PARAM_PICKUP);
        AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_ItemPickUp, transform.position, true, true);
        _playerTimer = Constants.For_Player.PICKUP_ANIMATION_DELAY;
        // AudioManager.theAM.PlaySFX("Item Pickup");
        PlayerStates_SetPlayerState(PlayerState.PICKING_UP_ITEM);
    }

    void Interaction_PickedUpItem()
    {
        //Transfer parentship
        _itemPrevParent = _currInteract.Transform.parent;
        _currInteract.Transform.SetParent(transform);

        // Grow the collider size 
        _growCollider.size = _currInteract.Size;

        //So theres two vectors to assign to two properties
        //1 ) grow collider's center  2) currentInteract's local position
        //The two vectors to be assigned correctly are not the same

        Vector3 startingOffset = (_playerCollider.height + Constants.For_Player.PICKUP_OBJECT_OFFSET_Y) * Vector3.up;
        //^ Because player's pivot starts on the ground we need to offset the player collider's entire height value

        //Set position & rot of the object
        _currInteract.Transform.localPosition = startingOffset;
        _currInteract.Transform.localRotation = Quaternion.identity;


        //Setting the value for the grow collider's center 
        startingOffset.y += _currInteract.Size.y * 0.5f;
        //^ Because all playerpickable's pivot starts on the ground and hence the collider starts with its center at the feet of the object we need to offset the collider by half of the object's height size

        _growCollider.center = startingOffset;


        _growCollider.enabled = true;
        PlayerStates_SetPlayerState(PlayerState.PICKEDUP_ITEM);
    }
    #endregion

    #region Leaving Interactions
    ///<Summary>Tries to leave whatever interaction player is currently in now</Summary>
    void Interaction_TryLeaveInteraction(bool forcefully)
    {
        switch (_playerState)
        {
            case PlayerState.PICKING_UP_ITEM:
                _playerTimer = 0;
                PlayerStates_SetPlayerState(PlayerState.NONE);
                break;

            case PlayerState.PICKEDUP_ITEM:
                Interaction_TryDropItem(forcefully);
                break;

            case PlayerState.IN_STATION:
                Interaction_LeaveStation(forcefully);
                break;

            default:
                //Ignore states: NONE, STUNNED,ENDRESPAWN, INACTIVE
                return;

        }
    }
    #region Item

    ///<Summary>Tries to drop currently held item. If forcefully is true, then the item will not have its position altered. Else, the item will be placed infront of the player</Summary>
    void Interaction_TryDropItem(bool forcefully)
    {
#if UNITY_EDITOR
        if (_currInteract == null)
        {
            Debug.Assert(_currInteract != null, $"There is no interact to drop!", this);
        }
#endif

        switch (forcefully)
        {
            case true:
                _growCollider.enabled = false;
                //Drop the item
                _currInteract.Transform.SetParent(_itemPrevParent);
                AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_ItemDropped, transform.position, true, true);

                //Set position of current interact at the feet position
                _currInteract.LeavePlayerInteraction(forcefully);
                PlayerStates_SetPlayerState(PlayerState.NONE);
                break;


            case false:
                if (!Interaction_CheckPlayerFront())
                {
                    AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_CannotDropItem, transform.position, true, true);
                    return;
                }

                _growCollider.enabled = false;
                //Drop the item
                _currInteract.Transform.SetParent(_itemPrevParent);
                AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_ItemDropped, transform.position, true, true);
                //Set position of current interact at the feet position
                _currInteract.LeavePlayerInteraction(forcefully);
                PlayerStates_SetPlayerState(PlayerState.NONE);
                break;
        }


    }

    ///<Summary>Checks if the space infront of the player is valid to place the currently held item there. Returns true if valid</Summary>
    private bool Interaction_CheckPlayerFront()
    {
        Vector3 pickableHalfExtents = _currInteract.Size * 0.5f;
        //There are two vectors needed here, 
        //pickable placement position (since all pickables have their pivots at the base of the mesh model)
        //and the checkbox center where the physics check box can use to check if the pos they are going to put the box on is valid
        //ill use pos for storing both vectors
        Vector3 pos = transform.position + transform.forward * (pickableHalfExtents.z + _playerCollider.radius + Constants.For_Player.PLACE_OBJECT_OFFSET_Z) + transform.up * (pickableHalfExtents.y + Constants.For_Player.PLACE_OBJECT_OFFSET_Y);
#if UNITY_EDITOR
        RaycastDebugTools.DrawBox(pos, pickableHalfExtents, transform.rotation, Color.red);
#endif
        // //Overlap a box collider to check if the space infront is empty or not
        // foreach (var item in Physics.OverlapBox(pos,pickableHalfExtents,transform.rotation,StatsInfo.DropItemLayerMask))
        // {
        //     Debug.Log(item.name,item);
        // }
        if (Physics.CheckBox(pos, pickableHalfExtents, transform.rotation, StatsInfo.DropItemLayerMask, QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        //If so set the interact's transform to the placement pos
        pos.y -= pickableHalfExtents.y;
        _currInteract.Transform.position = pos;
        return true;
    }

    void Interaction_TossItem()
    {
        _growCollider.enabled = false;
        //Toss the item
        _currInteract.Transform.SetParent(_itemPrevParent);
        AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_Toss, transform.position, true, true);

        _itemPrevParent = null;
        _currInteract.TossInteraction(StatsInfo.TossForce * transform.forward);
        PlayerStates_SetPlayerState(PlayerState.NONE);
    }
    #endregion

    void Interaction_LeaveStation(bool forcefully)
    {
#if UNITY_EDITOR
        if (_currInteract == null)
        {
            Debug.Assert(_currInteract != null, $"There is no interact to drop!", this);
        }
#endif
        transform.SetParent(PlayerManager.SceneObject.transform);

        //Turn off the station's grow collider before turning the player's istrigger collider on
        _currInteract.LeavePlayerInteraction(forcefully);

        SetPropState(PropState.ONLAND);
        PlayerStates_SetPlayerState(PlayerState.NONE);
    }

    #endregion

    #endregion
}
