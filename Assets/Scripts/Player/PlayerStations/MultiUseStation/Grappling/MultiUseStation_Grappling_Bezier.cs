using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MultiUseStation
{
    public enum BezierState
    {
        ///<Summary>Bezier curve doesnt need any updating as there is no grappleable grappled</Summary>
        OFF = 0
        ,
        ///<Summary>The grappling hand has reached the cargo in the ocean and has triggered update for the bezier curve's delay</Summary>
        BEFOREDELAY = 1
        ,
        ///<Summary>The bezier curve's delay timer has finished and now it is really updating the cargo's movement along the bezier curve</Summary>
        AFTERDELAY = 2
    }

#if UNITY_EDITOR
    [Header("----- Runtime -----")]
    [Header("===== BEZIER =====")]
    [SerializeField, ReadOnly]
#endif
    float _bezierTimer = 0;
    ///<Summary>The bezier update will have two states: False = BeforeBezierDelayIsUp and True =AfterBezierDelayIsUp. BeforeBezierDelayIsUp is to allow for the grapplinghand to do a slight flick of the wrist before starting to update the cargo to follow the bezier path  </Summary>
    BezierState _bezierState = BezierState.OFF;

    Transform _cargoPrevParent = default;
    IGrappleable _bezierGrappleable = default;
    #region Properites
    // ///<Summary>Due to the Bezier update loop having 2 states, do not use this property during the BeforeBezierDelayIsUp state</Summary>
    bool bezierTimerDone => _bezierTimer <= 0;
    bool bezierNeedUpdate => _bezierState != BezierState.OFF;
    #endregion

#if UNITY_EDITOR
    const float AWAKECHECKS_TOSSTIME_LEEWAY = 0f;
    void Grappling_Bezier_AwakeChecks()
    {
        float totalBezierDuration = Info.BezierDuration + Info.BezierStartDelay + AWAKECHECKS_TOSSTIME_LEEWAY;
        float totalGrapplingDur = Info.CannonFireDelay + Info.GrapplingOutDuration + Info.ReelingInDuration;
        string debug = $"The Multiusestation info {Info.name} should not have its totalBezierDuration: \n (Info.BezierDuration: {Info.BezierDuration} + Info.BezierStartDelay: {Info.BezierStartDelay} + AWAKECHECKS_TOSSTIME_LEEWAY:{AWAKECHECKS_TOSSTIME_LEEWAY} = {totalBezierDuration}) more than or equal to the totalGrapplingDur: \n {totalGrapplingDur} (Info.CannonFireDelay: {Info.CannonFireDelay} + Info.GrapplingOutDuration: {Info.GrapplingOutDuration} + Info.ReelingInDuration: {Info.ReelingInDuration})!";
        Debug.Assert((totalBezierDuration) <= totalGrapplingDur, debug, Info);
    }
#endif

    void Grappling_Bezier_OnEnable()
    {
        _bezierState = BezierState.OFF;
        _bezierTimer = 0;
        _bezierGrappleable = null;
    }

    ///<Summary>Start the bezier update by firstly doing the delay timer</Summary>
    void Grappler_Bezier_TriggerUpdate(Vector3 pos)
    {
        _bezierGrappleable = _grappleable;
        _bezierState = BezierState.BEFOREDELAY;
        _bezierTimer = Info.BezierStartDelay;
        _bezierPath.SetPoint(2, pos);
    }



    ///<Summary>Updates the bezier curve. Should only be updated when there is Grappleable hit. Returns true when bezier is not updating</Summary>
    bool Grappling_Bezier_Update()
    {
        switch (_bezierState)
        {
            case BezierState.OFF:
                return true;

            case BezierState.BEFOREDELAY:
                Grappling_Bezier_BEFORE_Update();
                break;

            case BezierState.AFTERDELAY:
                Grappling_Bezier_AFTER_Update();
                break;

        }

        return false;

    }

    ///<Summary>Updates a timer to countdown the bezier delay. Will set bezierstate to AFTERDELAY state when time is up</Summary>
    void Grappling_Bezier_BEFORE_Update()
    {
        if (Grappling_Bezier_TickTimer())
        {
            _bezierState = BezierState.AFTERDELAY;
            _bezierTimer = Info.BezierDuration;
            //Return grappler to their original parent
            _bezierGrappleable.Transform.SetParent(_cargoPrevParent);
        }

    }


    void Grappling_Bezier_AFTER_Update()
    {
        //If timer is up
        //Bezer update should end earlier than grappling in update
        if (Grappling_Bezier_TickTimer())
        {
            // #if UNITY_EDITOR
            //             Debug.Assert(_firedTrigger, $"The multiuse station {name} should not have been done with its grappling update before the bezier update reaches its end!", this);
            // #endif

            _bezierGrappleable.LeaveGrapplingInteraction();
            _bezierGrappleable.Transform.position = _grapplingReturnPosition.position;
            _bezierGrappleable = null;
            _bezierState = BezierState.OFF;
            _bezierTimer = 0;

            //If force update is active and the grappling/cannon is not in active states
            if (_forceUpdate && !timerDone)
            {
                _forceUpdate = false;
            }

            return;
        }

        float percentage = _bezierTimer / Info.BezierDuration;
        Grappling_Bezier_UpdatePoints(percentage);
    }

    #region Supporting Methods


    ///<Summary>Ticks the bezier timer and returns true if timer is up</Summary>
    bool Grappling_Bezier_TickTimer()
    {
        if (bezierTimerDone)
            return true;

        _bezierTimer -= Time.deltaTime;
        return false;
    }

    void Grappling_Bezier_UpdatePoints(float time)
    {
#if UNITY_EDITOR
        Debug.Assert(_bezierGrappleable != null, $"Code shuld only run here when there is a grappleable!", this);
#endif

        //End Point
        _bezierPath.SetPoint(0, _grapplingReturnPosition.position);

        //Curve point
        Vector3 point1 = (_grapplingReturnPosition.position - _bezierPath.LastPoint) * 0.5f;
        point1 += _bezierPath.LastPoint;
        point1.y = Constants.For_PlayerStations.MULTIPURPOSE_BEZIER_HEIGHT;
        _bezierPath.SetPoint(1, point1);

        _bezierGrappleable.Transform.position = _bezierPath.GetPointInTime(time);
    }
    #endregion

}
