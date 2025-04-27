using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MultiUseStation
{
    #region Exposed Field
    // [SerializeField]
    // float _attackCooldown = default;
    [Header("===== CANNON =====")]
    [SerializeField]
    PlayerCannonProjectileInfo _cannonInfo = default;
    [SerializeField]
    VFXInfo fireVFX;

    #endregion

    #region Hidden Field

    LaunchData _launchData = default;


    #endregion
    private void CANNON_Awake()
    {
        _launchData = new LaunchData(Vector3.positiveInfinity, Vector3.negativeInfinity, Mathf.Infinity);
    }

    ///<Summary>Called when player uses the station when the station is in any of the CANNON state</Summary>
    private void ANY_CANNON_UsePlayerInteraction()
    {
        //Toggle the trajectory on
        Trajectory_ToggleActiveAll(true);
        //set current station state to inactive
        // _currentState = MultiUseState.INACTIVE_CANNON;
    }

    ///<Summary>Called when player leaves the station when the station is in any of the CANNON state</Summary>
    private void ANY_CANNON_LeavePlayerInteraction(bool forcefully)
    {
        //Toggle the trajectory off
        Trajectory_ToggleActiveAll(false);

        // //Check if cannon is cooling down
        // if (!_forceUpdate && forcefully && !timerDone)
        // {
        //     //If timer is still not yet over
        //     _forceUpdate = true;
        // }

    }


    #region Updates
    void INACTIVE_CANNON_Update()
    {
        _launchData = GameUtils.CalculateLaunchData(_firePoint.position, _targetPoint);

        if (playerUsingStation.DesireUse)
        {
            Animation_FireCannon();
            _currentState = MultiUseState.ACTIVE_CANNON_FIRE_DELAY;

            CANNON_Trajectory_ChangeLineColor(true);
            return;
        }

        // bool prevDone = _prevTimerDone;
        // //========== DESIRE USE ===========
        // switch (TickTimer())
        // {
        //     //=== Timer Done ===
        //     case true:

        //         //If previous frame was false and since timer is done, this frame is the frame where the cooldown is done
        //         if (!prevDone)
        //         {
        //             if (_forceUpdate) _forceUpdate = false;
        //             CANNON_Trajectory_ChangeLineColor(false);
        //             return;
        //         }

        //         if (playerUsingStation.DesireUse)
        //         {
        //             Animation_FireCannon();
        //             _currentState = MultiUseState.ACTIVE_CANNON_FIRE_DELAY;

        //             CANNON_Trajectory_ChangeLineColor(true);
        //             return;
        //         }
        //         break;

        //     case false:
        //         break;
        // }

        ANY_CANNON_MoveReticle();
    }


    ///<Summary>Update is called here because we want to include a slight delay after the cannon has played its firecannon animation before actually spawning a cannon ball</Summary>
    void ACTIVE_CANNON_FIREDELAY_Update()
    {
        if (TickTimer())
        {
            //--------- Fire the Cannon --------
            SetTimer(Info.AttackCooldown);
            
            VFXObj e = VFXPool.GetInstanceOf(fireVFX.Prefab, _firePoint, Vector3.zero, Quaternion.Euler(0, -90f, 0));
            e.Initialise();

            ProjectilePool.GetInstanceOf(_cannonInfo, _firePoint.position, _launchData.initialVelocity);
            _currentState = MultiUseState.ACTIVE_CANNON_COOLDOWN_ONLY;
            return;
        }
    }

    void ACTIVE_CANNON_COOLDOWN_ONLY_Update()
    {
        if (TickTimer())
        {
            if (_forceUpdate)
            {
                _forceUpdate = false;
            }

            _currentState = MultiUseState.INACTIVE_CANNON;
            CANNON_Trajectory_ChangeLineColor(false);
            return;
        }

        if (_forceUpdate) return;
        _launchData = GameUtils.CalculateLaunchData(_firePoint.position, _targetPoint);
        ANY_CANNON_MoveReticle();
    }

    void ANY_CANNON_MoveReticle()
    {

        // Move reticle and update the trajectory spheres only when there is input
        if (playerUsingStation.MovementInput.sqrMagnitude >= Constants.For_Player.MOVEMENTINPUTSQR_LEEWAY)
        {
            _reticle.UpdateReticle(playerUsingStation.MovementInput);
        }

        Trajectory_CANNON_Update();
        // _targetPoint = transform.TransformPoint(_reticle.FakeReticle.localPosition);

        // Rotation of model
        var dir = _launchData.initialVelocity.normalized;
        if (dir != Vector3.zero)
        {
            Animation_LookTowards(dir);
        }
    }
    #endregion


    private void INACTIVE_CANNON_FixedUpdate()
    {
        ANY_CANNON_FIXEDUPATE();
        // Vector3 startPoint = _reticle.FakeReticle.transform.position;
        // startPoint.y = Constants.For_PlayerStations.MULTIPURPOSE_WATERLEVEL; ;

        //======= NO COLLIDERS FOUND ==========
        //Get as many interactables in the ocean water 
        if (Physics.OverlapSphereNonAlloc(_targetPoint, Info.DetectionRaidus, _result, Constants.For_Layer_and_Tags.LAYERMASK_ENEMYBOATMODEL) <= 0)
        {
            //======= IF THERE WAS IGRAPPEABLE FOUND LAST FRAME ===========
            if (hasGrappleable)
            {
                //Change reticle colour/sprite/ui
                // _reticle.ChangeReticleMaterial(false);
                _grappleable.LeaveDetection();
                _grappleable = null;
            }
            _result[0] = null;
            return;
        }

        // =========== COLLIDERS FOUND ===========
        // _reticle.ChangeReticleMaterial(true);
    }

    void ANY_CANNON_FIXEDUPATE()
    {
        _targetPoint = _reticle.Reticle.transform.position;
        _targetPoint.y = Constants.For_PlayerStations.MULTIPURPOSE_WATERLEVEL;
    }

}
