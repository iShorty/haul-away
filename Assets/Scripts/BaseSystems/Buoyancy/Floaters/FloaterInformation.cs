using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Floater Data", menuName = Constants.ASSETMENU_CATEGORY_WATERPHYSICS + "/FloaterInformation")]
public class FloaterInformation : ScriptableObject
{
    [Header("----- Floater Values -----")]
    [Tooltip("Basically the height of the object before it is considered as submerged. The higher this value, the lower the buoyancy force exerted in the upwards direction is applied to the floater")]
    [Range(0, 100)]
    public float DepthBeforeSubmerged = 1f;

    [Tooltip("Basically the volume (and density combined) of the object and henceforth the volume of fluid displaced. The higher this value, the higher the buoyancy force exerted on floater")]
    [Range(0, 100)]
    public float DisplacementAmount = 3f;


    [Tooltip("The drag coefficient of the floater when it is moving in the water")]
    [Range(0, 100)]
    public float VelocityDrag = 0.1f;

    [Tooltip("The angular drag coefficient of the floater when it is moving in the water")]
    [Range(0, 100)]
    public float AngularVelocityDrag = 0.1f;

    [Header("----- Dampening -----")]
    [Tooltip("Overall Dampening on Buoyancy")]
    [Range(0, 1)]
    public float BuoyancyDampening = 1;

    [Tooltip("Overall Dampening on Drag")]
    [Range(0, 1)]
    public float DragDampening = 1;

    // [field: Header("---- Sinking Values -----")]
    // public Vector2 SinkSpeedRange  = default;

    // public float FloatDuration  = default;

    // public float SinkDuration  = default;

}
