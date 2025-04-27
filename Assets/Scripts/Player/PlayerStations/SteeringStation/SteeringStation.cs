using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringStation : OverridePlayerMovementStation
{
    [Header("===== STEERING STATION =====")]
    [SerializeField]
    BoatController _boatController = default;

    // [SerializeField]
    // MeshRenderer _litInstance = default;
    //Runtime
    float _horizontalInput = 0;

    protected override float stationCollider_HalfExtents_Z => GetComponent<BoxCollider>().size.z * 0.5f;

    protected override void Awake()
    {
        base.Awake();
        // _litInstance.material.EnableKeyword("_EMISSION");
    }


    public override bool UpdateInteract()
    {
        _horizontalInput = playerUsingStation.MovementInput.x;
        // _horizontalInput = playerUsingStation.ControlInfo.GetMovementXInput();
        // if (playerUsingStation.DesireUse)
        // {
        //     BoatManager.Controller._DirectionToggle = !BoatManager.Controller._DirectionToggle;
        //     ToggleLight(BoatManager.Controller._DirectionToggle);
        // }
        return true;
    }

    // private void ToggleLight(bool isMovingForward)
    // {
    //     if (isMovingForward)
    //     {
    //         _litInstance.material.SetColor("_EmissionColor", Color.black);
    //     }
    //     else
    //     {
    //         _litInstance.material.SetColor("_EmissionColor", Constants.For_PlayerStations.STEERING_REVERSEMODE_LIGHTBULB_COLOR);
    //     }
    // }

    public override void FixedUpdateInteract()
    {
        _boatController.AddTorqueToBoat(_horizontalInput);
    }
}
