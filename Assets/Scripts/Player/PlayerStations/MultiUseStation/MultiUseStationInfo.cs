using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MultiUseStationInfo), menuName = Constants.ASSETMENU_CATEGORY_PLAYER_BOAT + "/" + nameof(MultiUseStationInfo))]
public class MultiUseStationInfo : ScriptableObject
{
    #region Main
    //Main
    [Header("----- Detection -----")]
    [Header("===== MAIN =====")]
    [Min(0)]
    [Tooltip("Detection raidus of the reticle on the surface of the water to detect any grappleable objects in the water")]
    ///<Summary>Detection raidus of the reticle on the surface of the water to detect any grappleable objects in the water</Summary>
    public float DetectionRaidus = 1;

#if UNITY_EDITOR
    [Header("----- Editor -----")]
    public Color DetectionColour = Color.red;

    public bool DrawDetectionSphere = true;
#endif
    #endregion

    #region Reticle
    //Movereticle
    [Header("===== RETICLE =====")]
    [Range(0f, 100f)]
    [Tooltip("Speed of the reticle movement")]
    ///<Summary>Speed of the reticle movement</Summary>
    public float ReticleSpeed = 10f;

    [Min(0)]
    ///<Summary>The size of the rect in which the players could fire in from the multiuse station</Summary>
    [Tooltip("The size of the rect in which the players could fire in from the multiuse station")]
    public Vector2 AttackRectSize = default;

    [Min(0)]
    [Tooltip("The X distance to offset the rect in which the players could fire in from the multiuse station")]
    ///<Summary>The X distance to offset the rect in which the players could fire in from the multiuse station</Summary>
    public float AttackRectOffset = 0;

public Color CannonReticleColor = Color.white;
public Color GrapplingReticleColor = Color.red;

    // public Texture TargetFoundReticle = default;
    // public Texture TargetNotFoundReticle = default;

#if UNITY_EDITOR
    [Header("----- Editor -----")]
    public bool DrawRectCorners = true;
#endif

    #endregion

    //Animation
    // [Range(0, 180)]
    // public float _maxBaseAngleRotation = 45;

    #region Animation
    [Header("===== ANIMATION =====")]
    [Range(0, 100)]
    [Tooltip("The speed of rotation for the cannon rotating towards its target")]
    ///<Summary>The speed of rotation for the cannon rotating towards its target</Summary>
    public float RotationSpeed = 10f;

    #endregion


    #region Cannon

    //Cannon
    [Header("===== CANNON =====")]
    [Range(0, 100)]
    ///<Summary>The duration for the cannon to cooldown</Summary>
    [Tooltip("The duration for the cannon to cooldown")]
    public float AttackCooldown = default;

    [Range(0, 100)]
    ///<Summary>The time needed for the cannon to play out the firing animation before spawning the cannonball or send out the grappling hook</Summary>
    [Tooltip("The time needed for the cannon to play out the firing animation before spawning the cannonball or send out the grappling hook")]
    public float CannonFireDelay = 0.25f;

    [Header("----- Trajectory -----")]
    ///<Summary>The Trajectory sphere's prefab</Summary>
    [Tooltip("The Trajectory sphere's prefab")]
    public GameObject TrajectoryPrefab = default;

     [Header("----- Trajectory -----")]
    ///<Summary>The Trajectory reticle</Summary>
    [Tooltip("The Trajectory reticle prefab. Will be spawned at the end of the iteration")]
    public GameObject TrajectoryReticlePrefab = default;

    [Range(0, 1000)]
    ///<Summary>The number of trajectory spheres to draw the trajectory line<Summary>
    [Tooltip("The number of trajectory spheres to draw the trajectory line")]
    public int Iteration = 12;

    [Range(0, 20)]
    ///<Summary>The interval of big spheres in the trajectory line which is composed of big and small spheres</Summary>
    [Tooltip("The interval of big spheres in the trajectory line which is composed of big and small spheres")]
    public int BigSphereInterval = 3;

    [Range(0, 1)]
    ///<Summary>Uniform scale of the small sphere</Summary>
    [Tooltip("Uniform scale of the small sphere")]
    public float SmallSphereSize = 0.25f;

    [Range(0, 1)]
    ///<Summary>Uniform scale of the big sphere</Summary>
    [Tooltip("Uniform scale of the big sphere")]
    public float BigSphereSize = 0.5f;

    [Range(0, 5)]
    ///<Summary>The duration of which the trajectory line's sphere animate</Summary>
    [Tooltip("The duration of which the trajectory line's sphere animate")]
    public float TrajectoryAnimationInterval = 1f;

    ///<Summary>The color for the trajectory when the cannon cooldown is finished</Summary>
    [Tooltip("The color for the trajectory when the cannon cooldown is finished")]
    public Color TrajectoryActive = Color.white;
    ///<Summary>The color for the trajectory when the cannon cooldown is not finished</Summary>
    [Tooltip("The color for the trajectory when the cannon cooldown is not finished")]
    public Color TrajectoryInActive = Color.white;

    #endregion

    #region Grappling
    //Grappling
    [Header("----- Durations -----")]
    [Header("===== GRAPPLING =====")]
    [Range(0, 1)]
    ///<Summary>The duration taken for the grappling hook to reach the point in the water which the grappling hook has been fired towards</Summary>
    public float GrapplingOutDuration = 0.25f;

    ///<Summary>The duration taken for the grappling hook to reel back in after reaching the target point in the water</Summary>
    [Range(0, 10)]
    public float ReelingInDuration = 1f;

    [Range(0, 10)]
    ///<Summary>When the MultiuseStation is grappling out, the update will check if _timer is below this GrabAnimationDelay. If so, the grapple animation will play.</Summary>
    public float GrabTimeStamp = 0.1f;

    [Range(0, 10)]
    ///<Summary>When the MultiuseStation is reeling in, the update will check if _timer is below this TossAnimationDelay. If so, the Toss animation will play.</Summary>
    public float TossTimeStamp = 0.75f;

    #endregion

    #region Bezier

    [Header("----- Durations -----")]
    [Header("===== BEZIER =====")]
    ///<Summary>The delay before actually starting to update the cargo's position via the bezier curve </Summary>
    [Range(0.1f, 10f)]
    public float BezierStartDelay = 1f;

    ///<Summary>The time needed for the cargo to move throught the entire bezier curve onto the boat</Summary>
    [Range(0.1f, 10f)]
    public float BezierDuration = 1f;

    #endregion

}