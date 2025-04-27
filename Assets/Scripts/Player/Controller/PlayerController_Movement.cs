using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManagement;


public partial class PlayerController
{
    [Header("===== PLAYER MOVEMENT =====")]

    [Tooltip("Amount of footstep sounds played when moving one meter")]
    [SerializeField] float footstepSFXFrequency = 1f;

    #region Hidden Fields
    // ======== CONTROLS INPUT/DIRECTION =========
    Vector3 _projectedForwardAxis = default
    , _projectedRightAxis = default
    ;
    // bool _desiresJump = false;


    // ========== RIGIDBODIES ==========
    Rigidbody _parentRb = default
    , _prevParentRb = default
    ;
    Vector3 _velocity = default
    , _desiredVelocity = default
    , _parentVelocity = default
    , _playerPrevWorldPosition = default
    , _playerPrevLocalPosition = default
    ;


    Vector3 _contactNormalAverage = default
    ;

    int _groundContactCount = default
    , _physicsStepsSinceGrounded = 0
    , _physicsStepsSinceJumped = 0
    ;
    float _footstepDistanceCounter;

    Quaternion _rotation = default;

    #region Properties
    //=========== INPUT ==============
    ///<Summary>Movement input which is updated by the player input system</Summary>
    public Vector2 MovementInput { get; private set; }
    //=========== GROUND INTERACTION =============
    bool isGrounded => _groundContactCount > 0;
    bool isSwimming => _currentPropState == PropState.FLOATING;
    bool isInAir => !isGrounded && !isSwimming;
    #endregion
    #endregion


    #region Enable Disable
    void Movement_Awake(Transform referenceTransform)
    {
        _referenceTransform = referenceTransform;
    }

    void Movement_OnEnable()
    {
        PropRigidBody.isKinematic = false;
        _playerPrevLocalPosition = _parentVelocity = _contactNormalAverage = _velocity = _desiredVelocity = _projectedForwardAxis = _projectedRightAxis = Vector3.zero;
        _groundContactCount = _physicsStepsSinceGrounded = _physicsStepsSinceJumped = 0;
        _parentRb = null;
    }


    #endregion

    #region Update

    //Gather movement inputs 
    void Movement_GameUpdate_Default()
    {
        Movement_GatherMoveInput();
        Movement_UpdateProjectAxis();
        UpdateFootstepAudio();
    }

    void Movement_GameUpdate_RAGDOLLED()
    {
        //no input gathering but still updating projected axis
        Movement_UpdateProjectAxis();

        //timer count down
        if (_playerTimer > 0)
        {
            _playerTimer -= Time.deltaTime;
            return;
        }

        //once timer is up, restore state to NONE
        PlayerStates_SetPlayerState(PlayerState.NONE);
        _anim.SetTrigger(Constants.For_Player.ANIMATOR_PARAM_EXITDAZED);
    }
    #endregion


    #region Input Methods

    void Movement_GatherMoveInput()
    {
        //======= ASSIGNING DESIRED VELOCITY ========
        _desiredVelocity.x = MovementInput.x;
        _desiredVelocity.y = 0f;
        _desiredVelocity.z = MovementInput.y;
        _desiredVelocity *= isSwimming ? StatsInfo.MaxWaterSpeed : isGrounded ? StatsInfo.MaxLandSpeed : StatsInfo.MaxAirSpeed;
    }

    void Movement_ClearMoveInput()
    {
        MovementInput = Vector2.zero;
        _desiredVelocity = Vector2.zero;
    }

    void Movement_UpdateProjectAxis()
    {
        //========== PLAYER PLANE ADJUSTED AXIS ===========
        if (_referenceTransform)
        {
            if (!isSwimming)
            {
                _projectedForwardAxis = Movement_ProjectVectorOntoPlane(_referenceTransform.forward, _contactNormalAverage).normalized;
                _projectedRightAxis = Movement_ProjectVectorOntoPlane(_referenceTransform.right, _contactNormalAverage).normalized;
            }
            else
            {
                _projectedForwardAxis = Movement_ProjectVectorOntoPlane(_referenceTransform.forward, Vector3.up).normalized;
                _projectedRightAxis = Movement_ProjectVectorOntoPlane(_referenceTransform.right, Vector3.up).normalized;
            }

        }
        else
        {
            _projectedForwardAxis = Movement_ProjectVectorOntoPlane(Vector3.forward, _contactNormalAverage).normalized;
            _projectedRightAxis = Movement_ProjectVectorOntoPlane(Vector3.right, _contactNormalAverage).normalized;
        }
    }

    #endregion

    #region Fixed Update
    void FixedUpdate_Movement()
    {
        // =========== VELOCITY UPDATE ============
        _velocity = PropRigidBody.velocity;

        Movement_UpdatePhysicsState();
        Movement_AddMovementVelocity();
        Movement_AddRotationVelocity();

        PropRigidBody.velocity = _velocity;
        Movement_ClearPhysicsState();
    }


    #region Update Physics State
    void Movement_UpdatePhysicsState()
    {
        Movement_UpdateGroundPhysicsState();

        switch (_playerState)
        {
            case PlayerState.ENDRESPAWN:
                {
                    _prevParentRb = BoatManager.Controller._rigidBody;
                    _parentRb = BoatManager.Controller._rigidBody;
                }
                break;
        }

        //======= RIGIDBODY PARENTING ==========
        Movement_UpdateRbParentState();

    }

    #region Ground Physics State

    void Movement_UpdateGroundPhysicsState()
    {
        // =========== GROUND UPDATE ============
        _physicsStepsSinceJumped++;
        _physicsStepsSinceGrounded++;
        if (isGrounded || Movement_TrySnapToGround())
        {
            _physicsStepsSinceGrounded = 0;

            //Only normalize contact normal when there is more than 1 collision contacts
            if (_groundContactCount > 1)
            {
                _contactNormalAverage.Normalize();
            }
        }
        else
        {
            _contactNormalAverage = Vector3.up;
        }
    }

    bool Movement_TrySnapToGround()
    {
        //We shuld only try to snap once after we are not grounded (else we consider the player to be in the air already)
        //we shuld also try not to snap to ground immediately after we do a jump
        if (_physicsStepsSinceGrounded > 1 || _physicsStepsSinceJumped <= 2)
        {
            return false;
        }

        //Check if current speed exceeds that of the max snap speed threshold
        float currentSqrSpeed = Vector3.SqrMagnitude(_velocity);

        if (currentSqrSpeed > StatsInfo.SnapSpeedSqrThreshold)
        {
            return false;
        }

        //Check for groud below to snap to
        if (!Physics.Raycast(PropRigidBody.position, Vector3.down, out RaycastHit hit, StatsInfo.SnapRayDistance, StatsInfo.GroundLayerMask))
        {
            return false;
        }

        //Check if the ground is considered as a slope to our player
        if (hit.normal.y < StatsInfo.MinimumSlopeDot)
        {
            return false;
        }

        //=======  SNAPPING TO GROUND ===========
        _groundContactCount = 1;
        _contactNormalAverage = hit.normal;
        _parentRb = hit.rigidbody;

        //====== ALIGN CURRENT VELOCITY TO GROUND ========
        float dot = Vector3.Dot(PropRigidBody.velocity, _contactNormalAverage);

        if (dot > 0)
        {
            _velocity = (_velocity - _contactNormalAverage * dot).normalized * currentSqrSpeed;
        }

        return true;
    }
    #endregion

    #region Rigidbody Parent State

    void Movement_UpdateRbParentState()
    {
        if (!_parentRb) return;

        if (_parentRb.isKinematic || _parentRb.mass >= PropRigidBody.mass)
        {

            if (_parentRb == _prevParentRb)
            {
                Vector3 playerDisplacement = _parentRb.transform.TransformPoint(_playerPrevLocalPosition) - _playerPrevWorldPosition;
                _parentVelocity = playerDisplacement / Time.fixedDeltaTime;
            }


            _playerPrevWorldPosition = PropRigidBody.position;
            _playerPrevLocalPosition = _parentRb.transform.InverseTransformPoint(_playerPrevWorldPosition);
        }

    }
    #endregion

    #endregion

    #region Apply Physics Movement
    void Movement_AddMovementVelocity()
    {
        switch (_playerState)
        {
            case PlayerState.ENDRESPAWN:
                {
                    _velocity.x = _parentVelocity.x;
                    _velocity.z = _parentVelocity.z;
                }
                break;

            default:
                {
                    Vector3 relativeVelocity = _velocity - _parentVelocity;

                    //Project relative velocity onto projected right axis
                    float currentX = Vector3.Dot(relativeVelocity, _projectedRightAxis);
                    float currentZ = Vector3.Dot(relativeVelocity, _projectedForwardAxis);

                    float maxSpeedChange = isSwimming ? StatsInfo.MaxWaterAcceleration : isGrounded ? StatsInfo.MaxLandAcceleration : StatsInfo.MaxAirAcceleration;
                    maxSpeedChange *= Time.fixedDeltaTime;
                    float newX = Mathf.MoveTowards(currentX, _desiredVelocity.x, maxSpeedChange);
                    float newZ = Mathf.MoveTowards(currentZ, _desiredVelocity.z, maxSpeedChange);


                    _velocity += _projectedForwardAxis * (newZ - currentZ) + _projectedRightAxis * (newX - currentX);
                    _anim.SetFloat(Constants.For_Player.ANIMATOR_PARAM_VELOCITY_SQRMAG, _desiredVelocity.sqrMagnitude);
                }
                break;
        }

    }

    void Movement_AddRotationVelocity()
    {
        if (isInAir) return;

        _rotation = transform.localRotation;

        //====== APPLY PLAYER INPUT ROTATIONS =========
        if (MovementInput.sqrMagnitude >= Constants.For_Player.MOVEMENTINPUTSQR_LEEWAY)
        {
            //This is the player's desired direction in a unit sphere 
            Vector3 playerProjectedDirection = _projectedRightAxis * MovementInput.x + _projectedForwardAxis * MovementInput.y;

            Quaternion desiredRotation = Quaternion.LookRotation(playerProjectedDirection, transform.up);

#if UNITY_EDITOR
            Debug.DrawRay(transform.position, playerProjectedDirection, Color.white, Time.deltaTime);
#endif

            _rotation = Quaternion.Slerp(_rotation, desiredRotation, Time.fixedDeltaTime * StatsInfo.RotationSpeed);
        }


        //====== APPLY PARENT ROTATION CHANGES ==========
        if (_parentRb && _parentRb == _prevParentRb)
        {
            _rotation = Quaternion.Euler(
                _parentRb.angularVelocity * (Mathf.Rad2Deg * Time.deltaTime)
            ) * _rotation;
        }


        transform.localRotation = _rotation;
    }


    #endregion

    #region Clear Physics State
    void Movement_ClearPhysicsState()
    {
        _parentVelocity = _contactNormalAverage = Vector3.zero;
        //Because OnCollision events are called after fixedupdate, we will always be able to get the previous physic step's isGrounded value at the start of FixedUpdate
        _groundContactCount = 0;
        _prevParentRb = _parentRb;
        _parentRb = null;
    }

    #endregion

    #region Vector Methods
    Vector3 Movement_ProjectVectorOntoPlane(Vector3 direction, Vector3 normal)
    {
        return direction - normal * Vector3.Dot(direction, normal);
    }

    #endregion


    #endregion

    #region Collision
    void Movement_OnCollisionEnter(Collision collision)
    {
        Movement_EvaluateCollision(collision);
    }

    void Movement_OnCollisionStay(Collision collision)
    {
        Movement_EvaluateCollision(collision);
    }

    void Movement_EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;

            //Dot product of 2 vectors = |A| |B| cos(theta)
            //Since in a unit circle vectors have a magnitude of 1, Dot Product = cos(theta)
            //cos(theta) = adj/hypothenus 
            //since the normal's hypothenus is always 1 in a unity circle,
            //cos(theta) = adj (which is the y component of the normal vector)
            if (normal.y >= StatsInfo.MinimumSlopeDot)
            {
                if (_groundContactCount == 0)
                {
                    switch (_playerState)
                    {
                        case PlayerState.ENDRESPAWN:
                            PlayerStates_SetPlayerState(PlayerState.NONE);
                            break;
                    }
                    _parentRb = collision.rigidbody;
                }

                _groundContactCount++;
                _contactNormalAverage += normal;

            }

        }
    }
    #endregion

    void UpdateFootstepAudio()
    {
        // footsteps sound
        if (_footstepDistanceCounter >= 1f / footstepSFXFrequency)
        {
            _footstepDistanceCounter = 0f;
            // AudioManager.theAM.PlaySFX("Footstep");
            AudioEvents.RaiseOnPlay3DFollow(AudioClipType.SFX_Footstep, transform, true, true);
        }
        _footstepDistanceCounter += _desiredVelocity.magnitude * Time.deltaTime;
    }

}
