using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public partial class PlayerController : FloatableProp, IGrappleable, IBombable, IGrowableCollider
{
    #region Exposed Fields
    [field: Header("----- Infos -----"), Header("===== PLAYER MAIN =====")]
    // [field: SerializeField, RenameField(nameof(ControlInfo))]
    // public PlayerInputInfo ControlInfo { get; protected set; } = null;

    // [field: SerializeField, RenameField(nameof(NewControlInfo))]
    // public MasterControls NewControlInfo { get; protected set; } = null;

    [field: SerializeField, RenameField(nameof(StatsInfo))]
    public PlayerStatsInfo StatsInfo { get; protected set; } = null;

    [field: SerializeField, RenameField(nameof(OffBoatIndicatorInfo))]
    public IndicatorInfo OffBoatIndicatorInfo { get; protected set; } = null;

    [Header("References")]
    [Header("----- General -----")]
    public Transform _AttachPoint = default;
    public Transform _aimPoint = default;
    public Transform _modelContainer = default;
    [SerializeField]
    [Tooltip("The transform in which you want the character to move in reference to its forward transform. Example for this is a third person camera")]
    Transform _referenceTransform = default;


    [Header("----- Collider -----")]
    [SerializeField]
    CapsuleCollider _playerCollider = default;

    #endregion



    #region Hidden but shared Fields
    public PlayerInput PlayerInput { get; private set; } = default;
    Animator _anim = default;


    #region Runtime
#if UNITY_EDITOR
    [Header("===== PLAYER RUNTIME =====")]
    [ReadOnly, SerializeField]
#endif
    PlayerState _playerState = PlayerState.NONE;

#if UNITY_EDITOR
    [ReadOnly, SerializeField]
#endif
    float _playerTimer = default;
    #endregion




    #endregion

    #region Properties
    public int PlayerIndex { get; protected set; } = -1;
    #endregion


    #region Small Interfaces
    public Vector3 Size => StatsInfo.PlayerColliderSize;
    // private Vector3 _playerColliderSize;

    #endregion


    // #if UNITY_EDITOR
    //     private void OnDrawGizmosSelected()
    //     {
    //         OnDrawGizmos_Interaction();
    //     }
    // #endif


    #region GlobalEvent Methods
    public void Initialize(int playerIndex, PlayerInput playerInput, Transform referenceTransform, Animator anim)
    {
        PlayerIndex = playerIndex;
        PlayerInput = playerInput;
        _anim = anim;
        _anim.transform.SetParent(_modelContainer);
        _anim.transform.localRotation = Quaternion.identity;
        _anim.transform.localPosition = Vector3.zero;

        #region ------- Set player ring color -----------
        UnityEngine.Transform ringUI = _playerCollider.transform.Find(Constants.For_Player.PLAYER_RINGUI_NAME);
        MeshRenderer ringUImr = ringUI.GetComponent<MeshRenderer>();

#if UNITY_EDITOR
        Debug.Assert(ringUImr.material.FindPass(Constants.MATERIAL_URPDECAL_PASSNAME) != -1, $"The ring ui meshrenderer {ringUImr.name} on player {PlayerIndex} does not have a material with the decal shader!", ringUImr);
#endif
        ringUImr.material.SetColor(Constants.MATERIAL_URPDECAL_PROPERTYNAME_Color, Constants.For_Player.GetColor(PlayerIndex));
        #endregion

        // NewControlInfo = new MasterControls();
        Movement_Awake(referenceTransform);
        Input_GameAwake();
    }

    protected void OnDestroy()
    {
        Input_GameDestroy();
    }

    public void GamePause()
    {
        //Dont allow for player to use gravity
        PropRigidBody.isKinematic = true;

    }

    public void GameResume()
    {
        // allow for player to use gravity
        switch (_playerState)
        {
            case PlayerState.NONE:
                PropRigidBody.isKinematic = false;
                break;

            case PlayerState.INACTIVE:
                break;

            case PlayerState.IN_STATION:
                break;

            case PlayerState.PICKEDUP_ITEM:
                PropRigidBody.isKinematic = false;
                break;

            case PlayerState.STUNNED:
                PropRigidBody.isKinematic = false;
                break;

        }

    }

    #endregion

    #region Enable Disable
    private void Awake()
    {
        base.GameAwake();
#if UNITY_EDITOR
        Debug.Assert(CompareTag(Constants.For_Layer_and_Tags.TAG_PLAYER), "Player does not have its tag set as Player!", this);
#endif
        PlayerStates_SetPlayerState(PlayerState.NONE);

        PropRigidBody.interpolation = RigidbodyInterpolation.None;
    }

    protected override void OnEnable()
    {
        FloaterProp_OnEnable();
        base.OnEnable();
        Interaction_OnEnable();
        Movement_OnEnable();

        // PlayerInput.ActivateInput();
        // NewControlInfo.Enable();
    }

    protected override void OnDisable()
    {
        FloaterProp_OnDisable();
        base.OnDisable();
        // NewControlInfo.Disable();
        // PlayerInput.DeactivateInput();

    }


    #endregion

    #region Update
    public new void GameUpdate()
    {
        // Interaction_ToggleCamera();

        switch (_currentPropState)
        {
            case PropState.FLOATING:
                Player_GameUpdate_FLOATING();
                Prop_GameUpdate_FLOATING();
                break;

            case PropState.SINKING:
                Prop_GameUpdate_SINKING();
                break;


            case PropState.ONLAND:
                Player_GameUpdate_ONLAND();
                break;

            //Can mean that player is in station or being grappled by grappling hook
            case PropState.KINEMATIC:
                Player_GameUpdate_KINEMATIC();
                break;

            case PropState.INWATER: break;

            default:
#if UNITY_EDITOR
                Debug.LogError("Code should not flow here!");
#endif
                break;


        }
    }


    ///<Summary>Called every frame when player is in ONLAND state. Player can pick up items and walk. Player can be ragdolled from this state</Summary>
    void Player_GameUpdate_ONLAND()
    {
        switch (_playerState)
        {
            //========== PLAYER IS INTERACTING WITH NOTHING ==========
            case PlayerState.NONE:
                Movement_GameUpdate_Default();
                Interaction_Update_NONE();
                break;

            //========== PLAYER IS PICKING UP ITEM ==========
            case PlayerState.PICKING_UP_ITEM:
                Interaction_Update_PICKINGUPITEM();
                break;

            //========== PLAYER IS HOLDING ITEM ==========
            case PlayerState.PICKEDUP_ITEM:
                Movement_GameUpdate_Default();
                Interaction_Update_PICKEDUPITEM();
                break;

            //========== PLAYER IS STUNNED ==========
            case PlayerState.STUNNED:
                Movement_GameUpdate_RAGDOLLED();
                break;
            //========== PLAYER IS STUNNED ==========
            case PlayerState.ENDRESPAWN:
                //no input gathering but still updating projected axis
                Movement_UpdateProjectAxis();
                break;

            //========== STATES WHICH SHOULDNT BE POSSIBLE ============
            default:
                //INSTATION, GRAPPLED
#if UNITY_EDITOR
                Debug.LogError($"Player {PlayerIndex} should not have the code flowed into here when player is in {_playerState}", this);
#endif
                break;
        }
    }


    ///<Summary>Called every frame when player is in FLOATING state. Prop timer is being ticked here. Player can still pick up items and walk as if on land. Player can still be ragdolled from this state</Summary>
    void Player_GameUpdate_FLOATING()
    {
        switch (_playerState)
        {
            //========== PLAYER IS INTERACTING WITH NOTHING ==========
            case PlayerState.NONE:
                Movement_GameUpdate_Default();
                Interaction_Update_NONE();
                break;

            //========== PLAYER IS PICKING UP ITEM ==========
            case PlayerState.PICKING_UP_ITEM:
                Interaction_Update_PICKINGUPITEM();
                break;


            //========== PLAYER IS HOLDING ITEM ==========
            case PlayerState.PICKEDUP_ITEM:
                Movement_GameUpdate_Default();
                Interaction_Update_PICKEDUPITEM();

                break;

            //========== PLAYER IS STUNNED ==========
            case PlayerState.STUNNED:
                Movement_GameUpdate_RAGDOLLED();
                break;

            //========== STATES WHICH SHOULDNT BE POSSIBLE ============
            default:
                //INSTATION, GRAPPLED
#if UNITY_EDITOR
                Debug.LogError($"Player {PlayerIndex} should not have the code flowed into here when player is in {_playerState}", this);
#endif
                break;
        }
    }

    ///<Summary>Called every frame when player is in kinematic state. Can be when player enters a station or is being rescued by grappling hook</Summary>
    void Player_GameUpdate_KINEMATIC()
    {
        switch (_playerState)
        {
            //========== PLAYER IS INTERACTING WITH STATION ==========
            case PlayerState.IN_STATION:
                Movement_GameUpdate_Default();
                Interaction_Update_INSTATION();
                break;

            case PlayerState.INACTIVE: break;

            //========== STATES WHICH SHOULDNT BE POSSIBLE ============
            default:
                //NONE, PICKEDUP_ITEM, RAGDOLLED, GRAPPLED
#if UNITY_EDITOR
                Debug.LogError($"Player {PlayerIndex} should not have the code flowed into here when player is in {_playerState}", this);
#endif
                break;
        }
    }

    #endregion

    #region Fixed Update

    public override void FixedGameUpdate()
    {
        switch (_currentPropState)
        {
            case PropState.ONLAND:
                Player_FixedGameUpdate_ONLAND();
                break;

            case PropState.INWATER:
                _floaterGroup.GameFixedUpdate();
                break;

            case PropState.FLOATING:
                _floaterGroup.GameFixedUpdate();
                Player_FixedGameUpdate_FLOATING();
                break;

            case PropState.SINKING:
                _floaterGroup.GameFixedUpdate();
                break;

            case PropState.KINEMATIC:
                Player_FixedGameUpdate_KINEMATIC();
                break;

            default:
                break;
        }

    }

    ///<Summary>Called every physics frame when player is in kinematic state. Can be when player enters a station or is being rescued by grappling hook</Summary>
    private void Player_FixedGameUpdate_KINEMATIC()
    {
        switch (_playerState)
        {
            case PlayerState.IN_STATION:
                Interaction_FixedUpdate_INSTATION();
                break;

            case PlayerState.INACTIVE: break;

            //========== STATES WHICH SHOULDNT BE POSSIBLE ============
            default:
                //NONE, PICKEDUP_ITEM, RAGDOLLED
#if UNITY_EDITOR
                Debug.LogError($"Player {PlayerIndex} should not have the code flowed into here when player is in {_playerState}", this);
#endif
                break;
        }
    }

    ///<Summary>Called every physics frame when prop is in ONLAND state. This is the usual prop state when player is on the boat. Player can pick up items while in this state</Summary>
    void Player_FixedGameUpdate_ONLAND()
    {
        switch (_playerState)
        {
            case PlayerState.NONE:
                FixedUpdate_Movement();
                Interaction_FixedUpdate_NONE();
                break;

#if UNITY_EDITOR
            case PlayerState.PICKING_UP_ITEM:
                break;
#endif

            case PlayerState.PICKEDUP_ITEM:
                FixedUpdate_Movement();
                Interaction_FixedUpdate_PICKEDUPITEM();
                break;

            case PlayerState.STUNNED:
                //Ensures that the player will rotate and still be "parented" to the player boat while on land and is ragdolled
                FixedUpdate_Movement();
                break;
            //========== PLAYER IS STUNNED ==========
            case PlayerState.ENDRESPAWN:
                //no input gathering but still updating projected axis
                FixedUpdate_Movement();
                break;



            //========== STATES WHICH SHOULDNT BE POSSIBLE ============
            default:
                //INSTATION,GRAPPLED
#if UNITY_EDITOR
                Debug.LogError($"Player {PlayerIndex} should not have the code flowed into here when player is in {_playerState}", this);
#endif
                break;
        }

    }

    ///<Summary>Called every physics frame when prop is in FLOATING state. This is called when player is in the water floating. During this state, the player can still move as if on land and pick up items if desired.</Summary>
    void Player_FixedGameUpdate_FLOATING()
    {
        switch (_playerState)
        {
            case PlayerState.NONE:
                FixedUpdate_Movement();
                Interaction_FixedUpdate_NONE();
                break;

            case PlayerState.PICKEDUP_ITEM:
                FixedUpdate_Movement();
                Interaction_FixedUpdate_PICKEDUPITEM();
                break;

            case PlayerState.STUNNED: break;

            //========== STATES WHICH SHOULDNT BE POSSIBLE ============
            default:
                //INSTATION,GRAPPLED
#if UNITY_EDITOR
                Debug.LogError($"Player {PlayerIndex} should not have the code flowed into here when player is in {_playerState}", this);
#endif
                break;
        }

    }
    #endregion

    #region Collision Methods

    void OnCollisionEnter(Collision other)
    {
        Movement_OnCollisionEnter(other);
    }

    void OnCollisionStay(Collision other)
    {
        Movement_OnCollisionStay(other);
    }

    #endregion

    #region Trigger Methods
    private void OnTriggerEnter(Collider other)
    {
        Interaction_OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        Interaction_OnTriggerExit(other);
    }
    #endregion

}
