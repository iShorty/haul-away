using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This file holds fields, properties, variables for any file that has to do with Interaction
public partial class PlayerController
{
    #region Exposed Fields
    [Header("===== INTERACTION =====")]

#if UNITY_EDITOR
    [SerializeField]
    Color _checkRadiusColour = new Color(1, 1, 1, 0.25f);
#endif


    [Header("----- PickingItems -----")]
    [SerializeField]
    BoxCollider _growCollider = default;

    #endregion

    #region Hidden Fields
    ///<Summary>The most accurate interactable object which is within the detection angle</Summary>
    IPlayerInteractable _currInteract = default
    ;
    ///<Summary>The most accurate interactable object's dot product</Summary>
    float _currInteractDot = default;

    HashSet<Collider> _detectedInteractsHashset = new HashSet<Collider>();
    List<Collider> _removeInteractsList = new List<Collider>();
    // Collider[] _detectedInteracts = new Collider[20];

    //=========== PICKINGUP ITEMS ============
    Transform _itemPrevParent = default;


    bool _isSteeringWheel = default;
    #endregion


    #region Properties

    #region ------------------- Desire Inputs ------------------------
    bool _desireUse = default;
    ///<Summary>Input bool for when Use button is pressed. When Use button is pressed, if the player is INSTATION he will use the station's cannon/grappling/steering function, if the player is PICKEDUP_ITEM he will toss the object</Summary>
    public bool DesireUse
    {
        get
        {
            if (_desireUse)
            {
                //This is to ensure that desire toggle left is set to true in a single frame only
                _desireUse = false;
                return true;
            }
            return false;
        }
    }

    // ///<Summary>Input bool for when Use button is pressed. When Use button is pressed, if the player is INSTATION he will use the station's cannon/grappling/steering function, if the player is PICKEDUP_ITEM he will toss the object</Summary>
    // public bool DesireUse { get; private set; }

    bool _desireInteract = default;
    ///<Summary>Input bool for when Interact button is pressed. When Interact button is pressed, if the player is NONE and detected a station player will enter station, if the player is NONE and detected an object player will pickup object</Summary>
    public bool DesireInteract
    {
        get
        {
            if (_desireInteract)
            {
                //This is to ensure that desire toggle left is set to true in a single frame only
                _desireInteract = false;
                return true;
            }
            return false;
        }
    }

    // ///<Summary>Input bool for when Interact button is pressed. When Interact button is pressed, if the player is NONE and detected a station player will enter station, if the player is NONE and detected an object player will pickup object</Summary>
    // public bool DesireInteract { get; private set; }

    bool _desireLeave = default;
    ///<Summary>Input bool for when Leave button is pressed. When Leave button is pressed, player will drop item if he is carrying any, will leave station if he is in any</Summary>
    public bool DesireLeave
    {
        get
        {
            if (_desireLeave)
            {
                //This is to ensure that desire toggle left is set to true in a single frame only
                _desireLeave = false;
                return true;
            }
            return false;
        }
    }

    // ///<Summary>Input bool for when Leave button is pressed. When Leave button is pressed, player will drop item if he is carrying any, will leave station if he is in any</Summary>
    // public bool DesireLeave { get; private set; }

    bool _desireToggleLeft = default;
    ///<Summary>Input bool for when ToggleLeft button is pressed, is used for toggling stations</Summary>
    public bool DesireToggleLeft
    {
        get
        {
            if (_desireToggleLeft)
            {
                //This is to ensure that desire toggle left is set to true in a single frame only
                _desireToggleLeft = false;
                return true;
            }
            return false;
        }
    }

    // ///<Summary>Input bool for when ToggleLeft button is pressed, is checked before interactionupdate</Summary>
    // public bool DesireToggleLeft { get; private set; }


    bool _desireToggleRight = default;
    ///<Summary>Input bool for when ToggleRight button is pressed, is used for toggling stations</Summary>
    public bool DesireToggleRight
    {
        get
        {
            if (_desireToggleRight)
            {
                //This is to ensure that desire toggle left is set to true in a single frame only
                _desireToggleRight = false;
                return true;
            }
            return false;
        }
    }

    // ///<Summary>Input bool for when ToggleRight button is pressed, is checked before interactionupdate</Summary>
    // public bool DesireToggleRight { get; private set; }

    // bool _desireToggleCamera = default;
    // ///<Summary>Input bool for when ToggleCamera button is pressed, is used for toggling camera perspectives</Summary>
    // public bool DesireToggleCamera
    // {
    //     get
    //     {
    //         if (_desireToggleCamera)
    //         {
    //             //This is to ensure that desire toggle left is set to true in a single frame only
    //             _desireToggleCamera = false;
    //             return true;
    //         }
    //         return false;
    //     }
    // }

    // ///<Summary>Input bool for when ToggleCamera button is pressed, is checked before interactionupdate</Summary>
    // public bool DesireToggleCamera { get; private set; } = false;
    #endregion


    // Vector3 DetectionSpherePosition => transform.position + StatsInfo.DetectionSphereOffset;
    #endregion

    // #if UNITY_EDITOR
    //     void OnDrawGizmos_Interaction()
    //     {
    //         if (_playerState != PlayerState.NONE) return;
    //         Color prevColour = Gizmos.color;
    //         Gizmos.color = _checkRadiusColour;
    //         Gizmos.DrawSphere(DetectionSpherePosition, StatsInfo.CheckRaidus);
    //         Gizmos.color = prevColour;
    //     }
    // #endif



    #region Enable Disable

    void Interaction_OnEnable()
    {
        //Reset parent (cause prev state may be IN_STATION)
        transform.SetParent(PlayerManager.SceneObject.transform);
        _desireToggleLeft  = _desireToggleRight = _desireUse = _desireInteract = _desireLeave = false;
        MovementInput = Vector2.zero;
        _currInteract = null;
        _itemPrevParent = null;
        _growCollider.enabled = false;
        _detectedInteractsHashset.Clear();
        _removeInteractsList.Clear();

        // NewControlInfo.Gameplay.Movement.performed += (conxt) => MovementInput = conxt.ReadValue<Vector2>();
        // NewControlInfo.Gameplay.Movement.canceled += (conxt) => MovementInput = Vector2.zero;

        // NewControlInfo.Gameplay.ToggleLeft.performed += (conxt) => DesireToggleLeft = true;
        // NewControlInfo.Gameplay.ToggleLeft.canceled += (conxt) => DesireToggleLeft = false;

        // NewControlInfo.Gameplay.ToggleRight.performed += (conxt) => DesireToggleRight = true;
        // NewControlInfo.Gameplay.ToggleRight.canceled += (conxt) => DesireToggleRight = false;

        // NewControlInfo.Gameplay.Interact.performed += (conxt) => DesireInteract = true;
        // NewControlInfo.Gameplay.Interact.canceled += (conxt) => DesireInteract = false;

        // NewControlInfo.Gameplay.Use.performed += (conxt) => DesireUse = true;
        // NewControlInfo.Gameplay.Use.canceled += (conxt) => DesireUse = false;

        // NewControlInfo.Gameplay.Leave.performed += (conxt) => DesireLeave = true;
        // NewControlInfo.Gameplay.Leave.canceled += (conxt) => DesireLeave = false;
    }

    #endregion

    // ///<Summary>Toggles the camera perspective whenever desire toggle camera button is pressed</Summary>
    // void Interaction_ToggleCamera()
    // {
    //     if (DesireToggleCamera)
    //     {
    //         BoatManager.BoatCamera.ToggleZoomInfo();
    //     }
    // }

    #region OnTrigger Events
    private void Interaction_OnTriggerEnter(Collider other)
    {
        if (!Constants.For_Layer_and_Tags.LAYERMASK_INTERACTABLE_FINALMASK.Contains(other.gameObject.layer))
        {
            return;
        }
        _detectedInteractsHashset.Add(other);
    }
    private void Interaction_OnTriggerExit(Collider other)
    {
        _detectedInteractsHashset.Remove(other);
    }
    #endregion


}
