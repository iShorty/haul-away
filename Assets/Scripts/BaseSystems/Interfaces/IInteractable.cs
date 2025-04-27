using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInteractable
{
    Transform Transform { get; }
    void EnterDetection();
    void LeaveDetection();
}


public enum PlayerInteractableType
{
    ///<Summary>This is the basic picking up item type</Summary>
    ITEM
    ,
    ///<Summary>This is the station which overrrides the player's movement</Summary>
    OVERRIDESTATION
    ,
    ///<Summary>This is the station which doesnt override the player's movement. The player will just enter and leave the station immediately. (eg press Use and the station just poops fuel)</Summary>
    NONEOVERRIDESTATION
    ,
    ///<Summary>This is an enemy that can be interacted with.</Summary>
    INTERACTABLEENEMY
}

///<Summary>Inherited by classes which you want players to be able to be interacted with. Current uses: Stations, PlayerPickables</Summary>
public interface IPlayerInteractable : IInteractable,IGrowableCollider
{
    ///<Summary>This is called when the player leaves the interaction. If forcefully = true, that means the player must have been hit by a bomb or something and leave the interactable forcefully else, it is by the player's own choice that he leaves the station</Summary>
    void LeavePlayerInteraction(bool forcefully);

    ///<Summary>Determines whether this object is detected by the player's physics detection system and hence triggering EnterDetection</Summary>
    bool IsPlayerInteractable { get; }
    PlayerInteractableType PlayerInteractableType { get; }
    void UsePlayerInteraction(int playerIndex);
    void TossInteraction(Vector3 force);

    ///<Summary>Called every frame when after the player has pressed "Use" button on the interactable. Will return true when station allows player to leave</Summary>
    bool UpdateInteract();

    ///<Summary>Called every physics frame when after the player has pressed "Use" button on the interactable.</Summary>
    void FixedUpdateInteract();

}

///<Summary>Inherited by classes which you want Grappling Hook to be able to be interacted with. Current uses: Player, BaseCargo</Summary>
public interface IGrappleable : IInteractable
{
    ///<Summary>Object leaves the grappling interaction when the entire grappling process has ended (eg when player has reached the end of the bezier path)</Summary>
    void LeaveGrapplingInteraction();

    ///<Summary>Checked during the grappling hook's physics detection code to determine whether or not target is valid </Summary>
    bool IsGrappleableInteractable { get; }
    ///<Summary>Called during the fixed update of Grappling Hook's physics detection to check if the collider (of a supposed cargo) is being held by a player </Summary>
    IGrappleable GetRootGrappleable();
    ///<Summary>Called after the grappling hook exits from the ACTIVE_OUT state and enters the ACTIVE_IN state. Basically when the grappling hook reaches the shot targetpoint, this method will be called</Summary>
    void RescueGrappleableInteraction();
    ///<Summary>Called when player presses DesireUse at a valid target and grappling hook is in the INACTIVE state</Summary>
    void TargetGrappleableInteraction(int stationIndex);
}

