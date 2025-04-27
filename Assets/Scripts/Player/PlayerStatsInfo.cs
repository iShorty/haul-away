using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerStats Info", menuName = Constants.ASSETMENU_CATEGORY_PLAYER + "/PlayerStatsInfo")]
public class PlayerStatsInfo : ScriptableObject
{
    #region Movement
    [field: SerializeField, RenameField(nameof(MaxLandSpeed))]
    [field: Header("----- Land Speed -----"), Header("===== MOVEMENT =====")]
    public float MaxLandSpeed { get; private set; } = 5f;

    [field: SerializeField, RenameField(nameof(MaxLandAcceleration))]
    public float MaxLandAcceleration { get; private set; } = 2f;

    [field: Header("----- Water Speed -----")]
    [field: SerializeField, RenameField(nameof(MaxWaterSpeed))]
    public float MaxWaterSpeed { get; private set; } = 2f;

    [field: SerializeField, RenameField(nameof(MaxWaterAcceleration))]
    public float MaxWaterAcceleration { get; private set; } = 2f;

    [field: Header("----- Air Speed -----")]
    [field: SerializeField, RenameField(nameof(MaxAirSpeed))]
    public float MaxAirSpeed { get; private set; } = 2f;

    [field: SerializeField, RenameField(nameof(MaxAirAcceleration))]
    public float MaxAirAcceleration { get; private set; } = 2f;

#if UNITY_EDITOR
    [Header("Values")]
    [Header("----- Ground checks -----")]
    [Min(0f)]
    [SerializeField]
    float _minimumSlopeAngle = 0f;
#endif

    [field: HideInInspector]
    [field: SerializeField]
    public float MinimumSlopeDot { get; private set; } = default;

    [field: SerializeField, RenameField(nameof(GroundLayerMask))]
    public LayerMask GroundLayerMask { get; private set; } = default;
    [field: SerializeField, RenameField(nameof(SnapRayDistance))]
    public float SnapRayDistance { get; private set; } = 1f;

#if UNITY_EDITOR
    [SerializeField]
    [Tooltip("If the speed player currently have exceeds this value, player will not try to snap to the ground below")]
    float _snapSpeedThreshold = 100f;
#endif

    [field: SerializeField, HideInInspector]
    public float SnapSpeedSqrThreshold { get; private set; } = 10000f;

    [field: Header("----- Rotation -----")]
    [field: SerializeField, RenameField(nameof(RotationSpeed))]
    public float RotationSpeed { get; private set; } = 15f;


    [field: Header("----- RagDoll -----")]
    [field: SerializeField, RenameField(nameof(StunDuration))]
    public float StunDuration { get; private set; } = 2f;

    #endregion

    #region Interaction
    [field: Header("----- Detection -----")]
    [field: Header("===== INTERACTION =====")]
    // [field: RenameField(nameof(CheckRaidus)), SerializeField]
    // public float CheckRaidus { get; private set; } = 1f;

    // [field: RenameField(nameof(DetectionSphereOffset)), SerializeField]
    // public Vector3 DetectionSphereOffset { get; private set; } = Vector3.zero;

    [field: RenameField(nameof(DropItemLayerMask)), SerializeField]
    public LayerMask DropItemLayerMask { get; private set; } = default;

#if UNITY_EDITOR
    [Tooltip("The angle in both clockwise & anti-clockwise direction from the player's forward axis that an interactable object has to be in before being considered as detected ")]
    [SerializeField, Min(0)]
    float _minDetectAngle = 45f;
#endif

    //========== INTERACTION DETECTION ==========
    [field: HideInInspector, SerializeField]
    public float MinDetectDot { get; private set; } = default;

    [field: Header("----- Toss -----")]
    [field: SerializeField, RenameField(nameof(TossForce))]
    public float TossForce { get; private set; } = 5f;

    #endregion

    #region Interfaces
    //IGrowable Collider

    [field: Header("----- IGrowableCollider -----")]
    [field: Header("===== INTERFACES =====")]
    [field: SerializeField, RenameField(nameof(PlayerColliderSize))]
    public Vector3 PlayerColliderSize { get; protected set; } = Vector3.one;

    #endregion

#if UNITY_EDITOR
    private void OnValidate()
    {
        MinimumSlopeDot = Mathf.Cos(_minimumSlopeAngle * Mathf.Deg2Rad);
        SnapSpeedSqrThreshold = _snapSpeedThreshold * _snapSpeedThreshold;
        MinDetectDot = Mathf.Cos(_minDetectAngle * Mathf.Deg2Rad);
    }
#endif

}
