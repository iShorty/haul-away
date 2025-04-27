using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.PlayerInput;

[RequireComponent(typeof(CharacterController))]
public class MenuBoatController : MonoBehaviour
{
    #region  Constants

    const float GRAVITY_GROUNDED_VELOCITY = -2;

    #endregion

    #region Exposed Variables
    [Header("Movement - References")]
    [SerializeField]
    protected CharacterController _characterController = default;
    [SerializeField]
    protected Transform _groundCheck = default;

    [SerializeField, Header("Movement - Values"), Range(0, 100)]
    protected float _walkSpeed = 5f;

    [SerializeField, Range(-1000, 0)]
    protected float _characterGravity = -9.81f;

    [Header("Ground Check - Values")]
    [SerializeField]
    protected LayerMask _groundMask = default;

    [SerializeField, Range(0.0001f, 100f)]
    protected float _groundCheckRaidus = 0.15f;

    [Header("Rotation - References")]
    [Space(15f)]
    [SerializeField]
    protected Transform _thirdPersonCamera = default;

    [Header("Rotation - Values")]
    [SerializeField]
    protected float _smoothTime = 0.1f;

    #endregion

    #region Runtime Variables
    protected float _yRotationSmoothDamp = default;
    protected float _yVelocity;
    protected bool _isGrounded;

    protected Vector2 _moveDelta = default;

    // protected PlayerInput _input = default;
    protected MasterControls _masterControls = default;

    protected LevelSelectTrigger _currentLevelTrigger = default;
    #endregion

    ///<Summary>Called when the menu boat controller enters a level trigger</Summary>
    public static event System.Action<LevelSelectTrigger> OnEnterLevelTrigger = null;

    ///<Summary>Called when the menu boat controller leaves a level trigger</Summary>
    public static event System.Action OnExitLevelTrigger = null;

    private void Awake()
    {
        _masterControls = GlobalPlayerInputManager.MasterControls;
#if UNITY_EDITOR
        Debug.Assert(CompareTag(Constants.For_Layer_and_Tags.TAG_PLAYER), "Menuboatcontroller must have player tag!", this);
#endif
    }

    protected virtual void OnEnable()
    {
        _isGrounded = false;
        _yVelocity = GRAVITY_GROUNDED_VELOCITY;
        _moveDelta = Vector2.zero;
        _currentLevelTrigger = null;

        _masterControls.MainMenu.Move.performed += HandleMovePerformed;
        _masterControls.MainMenu.Move.canceled += HandleMoveCancelled;

        MenuCanvas.LevelSelect_OnPanelDown += OnPanelDown_EnableMovement;
        MenuCanvas.LevelSelect_OnPanelUp += OnPanelUp_DisableMovement;
        //Get playerinputs
        // Cursor.lockState = CursorLockMode.Locked;
    }

    protected virtual void OnDisable()
    {
        _masterControls.MainMenu.Move.performed -= HandleMovePerformed;
        _masterControls.MainMenu.Move.canceled -= HandleMoveCancelled;

        MenuCanvas.LevelSelect_OnPanelDown -= OnPanelDown_EnableMovement;
        MenuCanvas.LevelSelect_OnPanelUp -= OnPanelUp_DisableMovement;
        OnPanelUp_DisableMovement();
    }

    private void HandleMoveCancelled(InputAction.CallbackContext obj)
    {
        _moveDelta = Vector2.zero;
    }

    private void HandleMovePerformed(InputAction.CallbackContext obj)
    {
        _moveDelta = obj.ReadValue<Vector2>();
    }

    public virtual void GameUpdate()
    {
        //=============== GRAVITY ============
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundCheckRaidus, _groundMask);

        _yVelocity = _isGrounded ? GRAVITY_GROUNDED_VELOCITY : _yVelocity += _characterGravity * Time.deltaTime;

        Vector3 direction = Vector3.up * _yVelocity;
        _characterController.Move(direction * Time.deltaTime);


        //Reset vector values
        direction.x = _moveDelta.x;
        direction.y = 0;
        direction.z = _moveDelta.y;

        //If there is no input 
        if (direction.magnitude <= 0.1f)
        {
            return;
        }

        //======================== ROTATION ========================
        float targetYRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _thirdPersonCamera.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetYRotation, ref _yRotationSmoothDamp, _smoothTime);

        transform.rotation = Quaternion.Euler(0f, angle, 0f);


        //================= MOVEMENT ========================
        //Convert quaternion rotation to a direction
        direction = Quaternion.Euler(0f, targetYRotation, 0f) * Vector3.forward;


        //Else walking speed is used
        _characterController.Move(direction * _walkSpeed * Time.deltaTime);

    }

    private void OnTriggerEnter(Collider other)
    {
        EvaluateEnterTrigger(other);
    }

    private void OnTriggerStay(Collider other)
    {
        EvaluateEnterTrigger(other);
    }

    private void EvaluateEnterTrigger(Collider other)
    {
        //Return if current lvl trigger is found to prevent calling OnEnterLevelTrigger during ontriggerstay
        if (_currentLevelTrigger) return;

        //If what i hit is level select trigger
        if (other.CompareTag(Constants.For_Layer_and_Tags.TAG_DESTINATION))
        {
            //Sub to pressing confirm button to pull up the lvl panel
            _currentLevelTrigger = other.GetComponent<LevelSelectTrigger>();
#if UNITY_EDITOR
            Debug.Assert(_currentLevelTrigger != null, $"The collider {other.name} does not have {nameof(LevelSelectTrigger)} component on it", other);
#endif
            OnEnterLevelTrigger?.Invoke(_currentLevelTrigger);
            // _masterControls.MainMenu.Confirm.performed += HandleDisableMovement;
            // _masterControls.MainMenu.Cancel.performed += HandleEnableMovement;
        }
    }

    private void OnPanelDown_EnableMovement()
    {
        _masterControls.MainMenu.Move.performed += HandleMovePerformed;
        _masterControls.MainMenu.Move.canceled += HandleMoveCancelled;
    }

    private void OnPanelUp_DisableMovement()
    {
        //Disable movement
        _masterControls.MainMenu.Move.performed -= HandleMovePerformed;
        _masterControls.MainMenu.Move.canceled -= HandleMoveCancelled;
        //Prevents the player from sliding when they open the panel
        _moveDelta = Vector2.zero;
    }

    private void OnTriggerExit(Collider other)
    {
        //Return if current lvl trigger is found
        // if (!_currentLevelTrigger) return;

        //If what i hit is level select trigger
        if (other.CompareTag(Constants.For_Layer_and_Tags.TAG_DESTINATION))
        {
            //UnSub to pressing confirm button to pull up the lvl panel
            _currentLevelTrigger = null;
            OnExitLevelTrigger?.Invoke();
            // _masterControls.MainMenu.Confirm.performed -= HandleDisableMovement;
            // _masterControls.MainMenu.Cancel.performed -= HandleEnableMovement;
        }
    }

}
