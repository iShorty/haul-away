using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>The base station class to inherit from if you want to create a station which overrides the player movement when the player interacts with it. The transform forward of a station which inherits from this class must be facing in the same direction as the direction in which the player will face when attempting to use the station</Summary>
public abstract class OverridePlayerMovementStation : BaseStation
{
    public override PlayerInteractableType PlayerInteractableType => PlayerInteractableType.OVERRIDESTATION;
    public override bool IsPlayerInteractable => playerUsingStation==null;
    [Header("===== OverridePlayerMovementStation ======")]
    [SerializeField]
    protected BoxCollider _stationGrowCollider = default;

    ///<Summary>The z value of the half extents of the station collider</Summary>
    protected abstract float stationCollider_HalfExtents_Z { get; }

    protected override void OnEnable()
    {
        base.OnEnable();
        _stationGrowCollider.enabled = false;
    }



    public override void FixedUpdateInteract() { }

    public override bool UpdateInteract()
    {
        return true;
    }

    public override void UsePlayerInteraction(int playerIndex)
    {
        base.UsePlayerInteraction(playerIndex);
        GrowStationCollider();
    }

    public override void LeavePlayerInteraction(bool forcefully)
    {
        base.LeavePlayerInteraction(forcefully);
        //Grow collider
        _stationGrowCollider.enabled = false;
    }

    ///<Summary> Grows station collider using the currently using station player</Summary>
    protected virtual void GrowStationCollider()
    {
        //============== SET PLAYER POSITION & ROTATION ===============
        //Set the player's position to be behind the station's front (and also account for the growing of the player's collider size)
        //must also keep in mind that player's pivot is at the feet
        Vector3 playerPos = transform.position - transform.forward * (stationCollider_HalfExtents_Z + playerUsingStation.Size.z * 0.5f);
        playerPos.y = playerUsingStation.transform.position.y;
        playerUsingStation.transform.position = playerPos;

        //Make the player face the direction the station is facing
        playerUsingStation.transform.rotation = transform.rotation;

        //============== GROWING COLLIDER SIZE & CENTER ===============
        //must also keep in mind that player's pivot is at the feet but collider's center is always at the center
        //Increment half the size of player cause player is pivoted on the foot
        playerPos.y += playerUsingStation.Size.y * 0.5f;
        playerPos = transform.InverseTransformPoint(playerPos);


        _stationGrowCollider.size = playerUsingStation.Size;
        _stationGrowCollider.center = playerPos;
        _stationGrowCollider.enabled = true;
    }


}
