using System;
using UnityEngine;
using AudioManagement;

public partial class MultiUseStation : OverridePlayerMovementStation
{
    #region Definitions
    ///<Summary>States of the multiuse station. Grappling hook states are even numbered while Cannon states are odd numbered</Summary>
    struct MultiUseState
    {
        //========= STATES WHEN STATION IS NOT ACTIVATING ===========
        ///<Summary>State for retriving items from the ocean</Summary>
        public const int INACTIVE_GRAPPLINGHOOK = 0;
        ///<Summary>State for firing projectiles at enemies</Summary>
        public const int INACTIVE_CANNON = 1;


        ///<Summary>The state inbetween Inactive and Active states</Summary>
        public const int ACTIVE_INACTIVE_BORDER = 2;

        ///<Summary>The state where the multistation just fired and there ought to be a delay before the spawning of the cannonball happens</Summary>
        public const int ACTIVE_CANNON_FIRE_DELAY = 3;

        ///<Summary>The state where the station is just ticking down its timer</Summary>
        public const int ACTIVE_CANNON_COOLDOWN_ONLY = 5;

        ///<Summary>The state where the multistation just fired and there ought to be a delay before the grappling hook goes flying out </Summary>
        public const int ACTIVE_GRAPPLINGHOOK_FIRE_DELAY = 4;

        //========= STATES WHEN STATION IS ACTIVATING ===========
        ///<Summary>State where grappling hook is shooting out </Summary>
        public const int ACTIVE_GRAPPLING_OUT = 6;

        ///<Summary>State where grappling hook is reeling in </Summary>
        public const int ACTIVE_GRAPPLING_IN = 8;

    }
    #endregion


    #region Exposed Fields

    [field: Header("----- Info -----")]
    [field: Header("===== MAIN =====")]
    [field: SerializeField, RenameField(nameof(Info))]
    public MultiUseStationInfo Info { get; private set; } = default;

    [SerializeField]
    bool _startAsCannon = true;

    [Header("----- Reticle -----")]
    [SerializeField] MoveReticle _reticle = default;

    [Header("----- Model -----")]
    [SerializeField]
    Transform _firePoint = default;

    #endregion


    #region Hidden Fields
    int _currentState = default;

    //===== AIMING =====
    ///<Summary>The targetpoint is effectively the reticle's position but on the y level of the ocean surface. It is set in the fixedupdate loop</Summary>
    Vector3 _targetPoint;
    Collider[] _result = new Collider[1];

#if UNITY_EDITOR
    [Header("----- Runtime -----")]
    [SerializeField, ReadOnly]
#endif
    //====== FIRING ========
    float _timer = default;

#if UNITY_EDITOR
    [SerializeField, ReadOnly]
#endif
    bool _forceUpdate = false;
    bool _prevTimerDone = true;

    #endregion


    #region Properties
    //====== STATION ========
    ///<Summary>0 = right station, 1 = left</Summary>
    public int MultiUseStationIndex { get; private set; }
    protected Transform trueForwardAxis => _stationGrowCollider.transform;

    //======= PLAYER INTERACTABLE ========
    bool hasGrappleable => _grappleable != null;
    Collider hitCollider => _result[0];

    //====== FIRING ==========
    bool timerDone => _timer <= 0;

    ///<Summary>The general state of the station. If true, the station is in any of the GRAPPLING state else it is in any of the CANNON state</Summary>
    bool generalState => _currentState % 2 == 0;

    protected override float stationCollider_HalfExtents_Z => GetComponent<SphereCollider>().radius;
    #endregion

#if UNITY_EDITOR
    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        switch (_currentState)
        {
            //============ UPDATE GRAPPLING HOOK ================
            case MultiUseState.INACTIVE_GRAPPLINGHOOK:
                {
                    DrawDetectionSphere();
                    // Animation_OnDrawGizmosSelected();
                    break;
                }

            //============ UPDATE CANNON ================
            case MultiUseState.INACTIVE_CANNON:
                {
                    DrawDetectionSphere();
                    // Animation_OnDrawGizmosSelected();
                    break;
                }

            default:
                {
                    break;
                }
        }
    }

    void DrawDetectionSphere()
    {
        if (!Info.DrawDetectionSphere) return;

        Vector3 startPoint = _reticle.Reticle.transform.position;
        startPoint.y = Constants.For_PlayerStations.MULTIPURPOSE_WATERLEVEL; ;
        Color prev = Gizmos.color;
        Gizmos.color = Info.DetectionColour;
        Gizmos.DrawSphere(startPoint, Info.DetectionRaidus);
        Gizmos.color = prev;
    }

    void DeleteFunction(Component c)
    {
        Destroy(c);
    }

    #endregion
#endif


    #region Awake/Enable
    public void GameAwake(int stationIndex)
    {
#if UNITY_EDITOR
        _reticle.GameAwake(transform, Info, DeleteFunction);
        Grappling_AwakeChecks();
#else
        _reticle.GameAwake(Info);
#endif


        MultiUseStationIndex = stationIndex;
        CANNON_Awake();
        Trajectory_Awake();
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        ResetTimer();
        //default reset state
        // _currentState = MultiUseState.INACTIVE_CANNON;
        _currentState = _startAsCannon ? MultiUseState.INACTIVE_CANNON : MultiUseState.INACTIVE_GRAPPLINGHOOK;
        AssertStationAmmo();
        _reticle.GameEnable();
        Grappling_OnEnable();
        Trajectory_OnEnable();
    }
    #endregion


    #region Interactable Methods

    #region Enter Leave
    public override void UsePlayerInteraction(int playerIndex)
    {
        base.UsePlayerInteraction(playerIndex);
        //pop up ui or smth idk?
        _reticle.PlayerUseStation();
        OnPlayerEnter?.Invoke(MultiUseStationIndex);

        //Prevent updating twice
        if (_forceUpdate)
        {
            _forceUpdate = false;
        }

        switch (generalState)
        {
            case false:
                ANY_CANNON_UsePlayerInteraction();
                break;

            case true:
                ANY_GRAPPLINGHOOK_UsePlayerInteraction();
                break;
        }
    }

    public override void LeavePlayerInteraction(bool forcefully)
    {
        //When do i want to update forcefully?
        // when forcefully == true cause player got blown by a bomb or smth and station is currently in a active state
        //If current state is active
        if (forcefully && _currentState > MultiUseState.ACTIVE_INACTIVE_BORDER)
        {
            _forceUpdate = true;
        }

        base.LeavePlayerInteraction(forcefully);
        // Disable pop up UI
        _reticle.PlayerLeaveStation();

        switch (generalState)
        {
            //If even
            case true:
                ANY_GRAPPLINGHOOK_LeavePlayerInteraction(forcefully);
                break;

            //Else if odd
            case false:
                ANY_CANNON_LeavePlayerInteraction(forcefully);
                break;
        }

    }



    protected override void GrowStationCollider()
    {
        //============== SET PLAYER POSITION & ROTATION ===============
        //Set the player's position to be behind the station's front (and also account for the growing of the player's collider size)
        //must also keep in mind that player's pivot is at the feet
        Vector3 playerPos = trueForwardAxis.position - trueForwardAxis.forward * (stationCollider_HalfExtents_Z + playerUsingStation.Size.z * 0.5f);
        playerPos.y = playerUsingStation.transform.position.y;
        playerUsingStation.transform.position = playerPos;

        //Make the player face the direction the station is facing
        playerUsingStation.transform.rotation = trueForwardAxis.rotation;

        //============== GROWING COLLIDER SIZE & CENTER ===============
        //must also keep in mind that player's pivot is at the feet but collider's center is always at the center
        //Increment half the size of player cause player is pivoted on the foot
        playerPos.y += playerUsingStation.Size.y * 0.5f;
        playerPos = trueForwardAxis.InverseTransformPoint(playerPos);


        _stationGrowCollider.size = playerUsingStation.Size;
        _stationGrowCollider.center = playerPos;
        _stationGrowCollider.enabled = true;
    }

    #endregion

    #region Update
    public void GameUpdate()
    {
        if (!_forceUpdate) return;

        UpdateInteract();
    }

    public override bool UpdateInteract()
    {
        bool allDone = Grappling_Bezier_Update();

        switch (_currentState)
        {
            //============ UPDATE GRAPPLING HOOK ================
            case MultiUseState.INACTIVE_GRAPPLINGHOOK:
                {
                    if (TryToggleState())
                        return true;

                    INACTIVE_GRAPPLING_Update();
                    return allDone;
                }

            //============ UPDATE CANNON ================
            case MultiUseState.INACTIVE_CANNON:
                {
                    if (TryToggleState())
                        return true;

                    INACTIVE_CANNON_Update();
                    return true;
                }

            //============ UPDATE CANNON FIRE DELAY ================
            case MultiUseState.ACTIVE_CANNON_FIRE_DELAY:
                {
                    ACTIVE_CANNON_FIREDELAY_Update();
                    return false;
                }

            //============ UPDATE CANNON FIRE DELAY ================
            case MultiUseState.ACTIVE_CANNON_COOLDOWN_ONLY:
                {
                    ACTIVE_CANNON_COOLDOWN_ONLY_Update();
                    return true;
                }

            //============ UPDATE CANNON FIRE DELAY ================
            case MultiUseState.ACTIVE_GRAPPLINGHOOK_FIRE_DELAY:
                {
                    ACTIVE_GRAPPLING_FIREDELAY_Update();
                    return false;
                }

            //============ UPDATE GRAPPLING SHOOTING OUT ================
            case MultiUseState.ACTIVE_GRAPPLING_OUT:
                {
                    ACTIVE_GRAPPLING_OUT_Update();
                    return false;
                }

            //============ UPDATE GRAPPLING SHOOTING OUT ================
            case MultiUseState.ACTIVE_GRAPPLING_IN:
                {
                    ACTIVE_GRAPPLING_IN_Update();
                    return false;
                }



            default:
                {
                    return true;
                }
        }
    }

    bool TryToggleState()
    {
        // if (playerUsingStation==null) return false;

        if (playerUsingStation.DesireToggleLeft)
        {
            //======= TOGGLES TO A STATION STATE WHEN IT IS NOT IN USE ==========
            _currentState--;
            _currentState = MathfExtensions.Repeat(_currentState, MultiUseState.INACTIVE_CANNON) + MultiUseState.INACTIVE_GRAPPLINGHOOK;

            AssertStationAmmo();
            return true;
        }

        if (playerUsingStation.DesireToggleRight)
        {
            //======= TOGGLES TO A STATION STATE WHEN IT IS NOT IN USE ==========
            _currentState++;
            _currentState = MathfExtensions.Repeat(_currentState, MultiUseState.INACTIVE_CANNON) + MultiUseState.INACTIVE_GRAPPLINGHOOK;

            AssertStationAmmo();
            return true;
        }

        return false;
    }

    void AssertStationAmmo()
    {
        // _reticle.ChangeReticleMaterial(false);
        switch (_currentState)
        {
            case MultiUseState.INACTIVE_GRAPPLINGHOOK:
                // AudioManager.theAM.PlaySFX("Grapple Gun Mode");
                AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_GrappleGunMode, transform.position, true, true);

                grapplerTransform.gameObject.SetActive(true);

                if (playerUsingStation != null)
                {
                    Trajectory_ToggleActiveSpheres(false);
                    Trajectory_ToggleActiveReticle(true);
                    Trajectory_ToggleReticleModelColor(false);
                }
                break;

            case MultiUseState.INACTIVE_CANNON:
                // AudioManager.theAM.PlaySFX("Cannon Gun Mode");
                AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_CannonGunMode, transform.position, true, true);
                grapplerTransform.gameObject.SetActive(false);

                if (playerUsingStation != null)
                {
                    Trajectory_ToggleActiveAll(true);
                    Trajectory_ToggleReticleModelColor(true);
                }
                break;
        }
    }

    ///<Summary>Ticks the timer and returns true if timer is up</Summary>
    bool TickTimer()
    {
        if (_prevTimerDone)
        {
            return true;
        }
        //Update ui here or smth
        if (timerDone)
        {
            _timer = 0;
            _prevTimerDone = true;
            return true;
        }
        _timer -= Time.deltaTime;
        return false;
    }

    void SetTimer(float time)
    {
#if UNITY_EDITOR
        Debug.Assert(time > 0, $"The SetTimer function should not be equal or leser than 0. Time value {time}", this);
#endif
        _prevTimerDone = false;
        _timer = time;
    }

    void ResetTimer()
    {
        _prevTimerDone = true;
        _timer = 0;
    }
    #endregion

    #region Fixedupdate
    public override void FixedUpdateInteract()
    {
        switch (_currentState)
        {
            case MultiUseState.INACTIVE_CANNON:
                INACTIVE_CANNON_FixedUpdate();
                break;

            case MultiUseState.ACTIVE_CANNON_COOLDOWN_ONLY:
                ANY_CANNON_FIXEDUPATE();
                break;

            case MultiUseState.INACTIVE_GRAPPLINGHOOK:
                INACTIVE_GRAPPLINGHOOK_FixedUpdate();
                break;

            default: break;
        }
    }

    #endregion

    #endregion

}