using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class MultiUseStation
{
    #region INACTIVE_GRAPPLING
    ///<Summary>Update method called when the grappling hook is loaded and inactive </Summary>
    void INACTIVE_GRAPPLING_Update()
    {
        Trajectory_GRAPPLING_Update();

        //Incase the players had previously used the cannon and toggled inmmediately to grappling hook
        //========== DESIRE USE ===========
        if (TickTimer() && playerUsingStation.DesireUse)
        {
            if (hasGrappleable)
            {
                _grappleable.TargetGrappleableInteraction(MultiUseStationIndex);
                _targetPoint = _grappleable.Transform.position;
                OnGrappleFireSuccess?.Invoke(_grappleable);
            }

            // _reticle.ChangeReticleMaterial(false);
            Trajectory_ToggleActiveReticle(false);
            Animation_FireCannon();
            Grappling_GrapplingHook_PrepareForFiring(true);
            _currentState = MultiUseState.ACTIVE_GRAPPLINGHOOK_FIRE_DELAY;
            return;
        }


        // Move reticle
        if (playerUsingStation.MovementInput.sqrMagnitude >= Constants.For_Player.MOVEMENTINPUTSQR_LEEWAY)
        {
            _reticle.UpdateReticle(playerUsingStation.MovementInput);
        }

        //===== UPDATE TARGET POINT =========
        // _targetPoint = transform.TransformPoint(_reticle.FakeReticle.localPosition);
        // _targetPoint.y = 0;

        // Rotation of model
        var dir = (_targetPoint - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            Animation_LookTowards(dir);
        }
    }



    #endregion

    #region ACTIVE_GRAPPLINGHOOK_FIRE_DELAY
    ///<Summary>Update method called when the grappling hook is delaying before firing the grappling hook </Summary>
    private void ACTIVE_GRAPPLING_FIREDELAY_Update()
    {
        if (TickTimer())
        {
            //--------- Change state to ACTIVE_GRAPPLING_OUT ----------------
            SetTimer(Info.GrapplingOutDuration);
            _currentState = MultiUseState.ACTIVE_GRAPPLING_OUT;
            _grapplerAnimator.SetTrigger(Constants.For_PlayerStations.MULTIPURPOSE_ANIMATION_PARAM_GRAPPLEOUT);
            return;
        }
    }
    #endregion

    #region ACTIVE_GRAPPLING_OUT
    ///<Summary>Update method called when the grappling hook has been fired and it flying outwards to the destinated point </Summary>
    void ACTIVE_GRAPPLING_OUT_Update()
    {
        bool timerDoneThisFrame = TickTimer();

        switch (_firedTrigger)
        {
            //============ GRAB HAS NOT BEEN TRIGGERED ==========
            case false:
                //Play grapple animation x seconds before hitting the object
                if (_timer <= Info.GrabTimeStamp)
                {
                    _firedTrigger = true;
                    _grapplerAnimator.SetTrigger(Constants.For_PlayerStations.MULTIPURPOSE_ANIMATION_PARAM_GRAB);
                }
                break;

            //============ GRAB HAS BEEN TRIGGERED ==========
            case true:

                //Hand has reached the targetpoint
                if (timerDoneThisFrame)
                {
                    _firedTrigger = false;
                    //Once point is reached, enter into grapping in state
                    SetTimer(Info.ReelingInDuration);
                    _currentState = MultiUseState.ACTIVE_GRAPPLING_IN;

                    if (hasGrappleable)
                    {
                        //Trigger grappleable's interact to set their states
                        _grappleable.RescueGrappleableInteraction();
                        //Record prev parent
                        _cargoPrevParent = _grappleable.Transform.parent;
                        _grappleable.Transform.SetParent(_grapplerCargoHolder);

                        IGrowableCollider growable = _grappleable.Transform.GetComponent<IGrowableCollider>();
                        Vector3 pos = Vector3.zero;
                        pos.y = -growable.Size.y;
                        _grappleable.Transform.localPosition = pos;
                    }

                    return;
                }
                break;
        }

        //Move your hook very quickly towards the target point every frame
        float lerpPercentage = _timer / Info.GrapplingOutDuration;
        grapplerTransform.position = Vector3.Lerp(_targetPoint, _firePoint.position, lerpPercentage);
        grapplerTransform.LookAt(_targetPoint);
        Grappling_RenderRope();
    }
    #endregion

    #region ACTIVE_GRAPPLING_IN
    ///<Summary>Update method called when the grappling hook been fired, reached it destinated point and is reeling back in </Summary>
    void ACTIVE_GRAPPLING_IN_Update()
    {
        bool doneThisFrame = TickTimer();

        //Move your hook very quickly towards the target point every frame
        float lerpPercentage = _timer / Info.ReelingInDuration;

        switch (_firedTrigger)
        {
            //============ TOSS HAS NOT BEEN TRIGGERED ==========
            case false:
                if (_timer <= Info.TossTimeStamp)
                {
                    _grapplerAnimator.SetTrigger(Constants.For_PlayerStations.MULTIPURPOSE_ANIMATION_PARAM_TOSS);
                    _firedTrigger = true;
                    //Only when there is grappleable do we start the bezier timer and do the toss animation
                    if (hasGrappleable)
                    {
                        //Record the position inwhich the toss is triggered and use that as the bezier curve's targetpoint
                        //Set the bezier timer to start the bezier update
                        Grappler_Bezier_TriggerUpdate(_grappleable.Transform.position);
                    }

                }
                break;


            //============ TOSS HAS BEEN TRIGGERED ==========
            case true:
                //Once point is reached, enter into inactive grapping state
                if (doneThisFrame)
                {
                    //ReSet values
                    ResetTimer();
                    _firedTrigger = false;

                    // If force update is active and bezier update has finished before reeling back in has finished updating
                    if (_forceUpdate && !bezierNeedUpdate)
                    {
                        _forceUpdate = false;
                    }

                    Trajectory_ToggleActiveReticle(true);
                    Grappling_GrapplingHook_PrepareForFiring(false);
                    _currentState = MultiUseState.INACTIVE_GRAPPLINGHOOK;
                    return;
                }

                break;

        }


        //Update grappler hook's pos
        grapplerTransform.position = Vector3.Lerp(_firePoint.position, _targetPoint, lerpPercentage);
        Grappling_RenderRope();

    }
    #endregion


    #region Support Methods
    ///<Summary>Do some code to prepare the grappling hook for firing the grappling hook outwards or after reeling in </Summary>
    void Grappling_GrapplingHook_PrepareForFiring(bool outwards)
    {
        switch (outwards)
        {
            case true:
                //Set the lr to have 2 position
                _lr.positionCount = 2;
                //Set the hand to no longer be parented to the cannon model so that the grappler hand doesnt get subjected to cannon fire animation
                grapplerTransform.SetParent(transform);
                break;

            case false:
                //Reset linerender point count
                _lr.positionCount = 0;
                grapplerTransform.SetParent(_firePoint);
                break;

        }
    }

    void Grappling_RenderRope()
    {
        //Update line renderer
        _lr.SetPosition(0, _firePoint.position);
        _lr.SetPosition(1, grapplerTransform.position);
    }

    #endregion

}
