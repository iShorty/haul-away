using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController
{
    #region Definition
    enum PlayerState
    {
        ///<Summary>The default state where player can walk about and interact with interactables </Summary>
        NONE
    ,
        ///<Summary>The state where player is within a station (which most likely overrides the movement input) </Summary>
        IN_STATION
    ,
        ///<Summary>The state where player is currently picking up an item so the item is not on their heads yet</Summary>
        PICKING_UP_ITEM
    ,
        ///<Summary>The state where player has picked up an item and can either throw, place or walk around with the item </Summary>
        PICKEDUP_ITEM
    ,
        ///<Summary>The state where the player cannot move due to being stunned by either a cannon or octopus</Summary>
        STUNNED
    ,
        ///<Summary>The state where the player is unable to move or update its physics state via gameupdate or fixedgameupdate. Possible cases for this is when player is grappled or respawning</Summary>
        INACTIVE
        ,

        ///<Summary>The state where the player is falling from the respawn point but shouldnt be moving yet</Summary>
        ENDRESPAWN
    }
    #endregion

    ///<Summary>Set player state so on enter state methods should be added</Summary>
    void PlayerStates_SetPlayerState(PlayerState playerState)
    {
        if (_playerState == playerState) return;
        _playerState = playerState;

        // switch (playerState)
        // {
        //     case PlayerState.NONE:
        //     // Interaction_OnEnter_NONE_State();
        //         break;

        //     case PlayerState.PICKEDUP_ITEM:
        //         break;

        //     case PlayerState.IN_STATION:
        //         break;

        //     case PlayerState.STUNNED:
        //         break;

        //     case PlayerState.INACTIVE:
        //         break;
        // }
    }
}
