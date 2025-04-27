using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MultiUseStation
{
    #region Exposed Field
    [Header("----- References -----")]
    [Header("===== GRAPPLING =====")]
    [SerializeField]
    LineRenderer _lr = default;

    [SerializeField]
    Transform _grapplerCargoHolder = default;

    [SerializeField]
    ///<Summary>The Transform which will determine where all rescued cargo will fly towards to and the end of the bezier curve</Summary>
    Transform _grapplingReturnPosition = default;

    [SerializeField]
    QuadraticBezierPath _bezierPath = default;

    #endregion


    #region Hidden Field
    IGrappleable _grappleable = default;
    // Transform _bezierTransform = default;
    ///<Summary>A boolean to determine if certain animation has been triggered during the grappling process. Used in two situations.</Summary>
    ///<Remarks>First scenario: MultiUse Station is grappling outwards. As timer counts down, if timer is lesser than GrabAnimationDelay value from Info, set this to true to trigger grab animation. Second scenario: Multiuse Station is reeling back in. As timer counts down, if timer is lesser than TossAnimationDelay value from Info, set this to true to trigger Toss animation and start bezier update (if there is something grabbed)</Remarks>
    bool _firedTrigger = false;
    #endregion


    #region Properties
    ///<Summary>The linerender's transform which is also the grappling hook holder's transform</Summary>
    Transform grapplerTransform => _lr.transform;


    #endregion

    #region Initialize
#if UNITY_EDITOR
    void Grappling_AwakeChecks()
    {
        Debug.Assert(Info.TossTimeStamp < Info.ReelingInDuration, $"The Multiusestation info {Info.name} should not have its TossTimeStamp more than or equal to the ReelingInDuration!", Info);
        Debug.Assert(Info.GrabTimeStamp < Info.GrapplingOutDuration, $"The Multiusestation info {Info.name} should not have its GrabTimeStamp more than or equal to the GrapplingOutDuration!", Info);
        Grappling_Bezier_AwakeChecks();
    }
#endif

    private void Grappling_OnEnable()
    {
        _lr.positionCount = 0;
        _firedTrigger = false;
        Grappling_Bezier_OnEnable();
    }

    ///<Summary>Called when player uses the station when the station is in any of the GRAPPLING state</Summary>
    private void ANY_GRAPPLINGHOOK_UsePlayerInteraction()
    {
        if (_currentState == MultiUseState.INACTIVE_GRAPPLINGHOOK)
        {
            //Hide reticle only
            Trajectory_ToggleActiveReticle(true);
        }

    }

    ///<Summary>Called when player leaves the station when the station is in any of the GRAPPLING state</Summary>
    private void ANY_GRAPPLINGHOOK_LeavePlayerInteraction(bool forcefully)
    {
        //Hide reticle only
        Trajectory_ToggleActiveReticle(false);
    }

    #endregion

    #region Fixedupdates
    //This basically detects the interactable things found by the raycast
    private void INACTIVE_GRAPPLINGHOOK_FixedUpdate()
    {
        _targetPoint = _reticle.Reticle.transform.position;
        _targetPoint.y = Constants.For_PlayerStations.MULTIPURPOSE_WATERLEVEL;


        //======= DETECT COLLIDERS ===========
        //Get as many interactables in the ocean water 
        if (Physics.OverlapSphereNonAlloc(_targetPoint, Info.DetectionRaidus, _result, Constants.For_Layer_and_Tags.LAYERMASK_INTERACTABLE_FINALMASK, queryTriggerInteraction: QueryTriggerInteraction.Ignore) <= 0)
        {
            //------- Prev frame had detected collider ------------
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

        // Debug.Log("1",hitCollider);

        // =========== COLLIDERS FOUND ===========
        //Only accept hit result if the hitcollider is not a station
        if (BoatManager.IsStation(hitCollider) || !PlayerPickableManager.IsGrappleable(hitCollider))
        {
            _result[0] = default;
            return;
        }

        // Debug.Log("2",hitCollider);


        //============= CHECKING IGRAPPLEABLE ================
        IGrappleable found = hitCollider.attachedRigidbody.GetComponent<IGrappleable>();

#if UNITY_EDITOR
        if (found == null)
        {
            Debug.LogError($"Collider {hitCollider.name} with the attached rb: {hitCollider.attachedRigidbody} should not have been hit by grappling physics detection!", hitCollider);
        }
#endif

        found = found.GetRootGrappleable();

        //--------- Grappleable found is same as curr one ------------
        if (found == _grappleable) return;

        // Debug.Log($"3, {found.IsGrappleableInteractable}" ,found.Transform);

        //-------- Grappleable found is interactable ------------
        if (!found.IsGrappleableInteractable) return;

        // Debug.Log("4",hitCollider);

        _grappleable = found;
        _grappleable.EnterDetection();
        // _reticle.ChangeReticleMaterial(true);

    }
    #endregion

}
