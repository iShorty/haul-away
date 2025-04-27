using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
///<Summary>The base station class which by default is a station which does not override the player movement</Summary>
public abstract class BaseStation : MonoBehaviour, IPlayerInteractable
{
    #region Constants
    // public const string UI_UNINTERACTED = "Press Use";
    // public const string UI_INTERACTED = "Press Leave";
    #endregion


    #region Exposed Field

    // [Tooltip("The UI which will pop up when the player is near the station to prompt them to interact")]
    // [SerializeField]
    // [Header("----- UI Notification -----")]
    // [Header("===== BASE STATION =====")]
    // protected Text _notificationUI = null;

    #endregion




    #region Properties
    protected BaseMeshOutline _meshOutline = default;
    protected PlayerController playerUsingStation { get; set; }

    ///<Summary>
    ///Determines whether there is a player using the station right now
    ///</Summary>
    public abstract bool IsPlayerInteractable{get;}

    // public abstract PlayerInteractableType PlayerInteractableType { get; }
    public virtual PlayerInteractableType PlayerInteractableType => PlayerInteractableType.NONEOVERRIDESTATION;

    public Transform Transform => transform;

    public Vector3 Size => Vector3.zero;
    #endregion



    #region Unity Methods
    protected virtual void Awake()
    {
        _meshOutline = GetComponentInChildren<BaseMeshOutline>();
#if UNITY_EDITOR
        Debug.Assert(CompareTag(Constants.For_Layer_and_Tags.TAG_STATION), $"{name} station does not have the Station tag!", this);
        Debug.Assert(gameObject.layer == Constants.For_Layer_and_Tags.LAYERINDEX_PLAYERINTERACTABLE, $"{name} station does not have the Interactable layer!", this);
        Debug.Assert(_meshOutline != null, $"The station {name} does not have the meshoutline assigned!", this);
        // Debug.Assert(_meshOutline.GetComponent<MeshOutline>(), $"The station {name} does not have the outlinemeshfilter's normal calculator on it!",_meshOutline);
#endif
        _meshOutline.GameAwake();
    }

    protected virtual void OnEnable()
    {
        playerUsingStation = null;
        // _notificationUI.gameObject.SetActive(false);
        _meshOutline.ToggleOutline(BaseMeshOutline.OutlineMode.OFF);
        // _stationGrowCollider.enabled = false;

    }

    protected virtual void OnDisable() { }


    #endregion

    #region Handle Methods
    public virtual void EnterDetection()
    {
        // _notificationUI.gameObject.SetActive(true);
        _meshOutline.ToggleOutline(BaseMeshOutline.OutlineMode.PARTIALLYHIDDEN);
        // _outlineMeshFilter.gameObject.layer = Constants.For_Layer_and_Tags.LAYERINDEX_DETECTEDINTERACTABLE;
    }

    public virtual void LeaveDetection()
    {
        // _notificationUI.gameObject.SetActive(false);
        _meshOutline.ToggleOutline(BaseMeshOutline.OutlineMode.OFF);
        // _outlineMeshFilter.gameObject.layer = Constants.For_Layer_and_Tags.LAYERINDEX_INTERACTABLE;
    }

    #region Use Player Interaction
    ///<Summary>
    ///Function that is called when player has successfully pressed the "use" button and station is not occupied.
    ///</Summary>
    public virtual void UsePlayerInteraction(int playerIndex)
    {
        playerUsingStation = PlayerManager.GetPlayer(playerIndex);
        // _notificationUI.text = UI_INTERACTED;
        _meshOutline.ToggleOutline(BaseMeshOutline.OutlineMode.OFF);
    }
    #endregion


    ///<Summary>
    ///Function that is called when player has successfully pressed the "leave" button and station
    ///</Summary>
    public virtual void LeavePlayerInteraction(bool forcefully)
    {
        playerUsingStation = null;
        // _notificationUI.gameObject.SetActive(false);
        // _notificationUI.text = UI_UNINTERACTED;
    }

    public virtual void TossInteraction(Vector3 force) { }

    #endregion

    public abstract bool UpdateInteract();

    public abstract void FixedUpdateInteract();


}
