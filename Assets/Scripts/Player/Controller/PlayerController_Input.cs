using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.PlayerInput;
public partial class PlayerController
{

    #region Awake and Destroy
    void Input_GameAwake()
    {
        Interaction_SubscribeActionHandlers();
        PlayerInput.ActivateInput();
    }

    void Input_GameDestroy()
    {
        Interaction_UnSubscribeActionHandlers();
        PlayerInput.DeactivateInput();
    }
    #endregion

    #region Interaction Update Handlers
    #region Subscribe and Unsubscribe wrappers

    void Interaction_SubscribeActionHandlers()
    {
        InputAction temp;
        #region  --------------- Subscription for Movement Action -----------------------
        temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_MOVEMENT);
        temp.performed += Interaction_HandleMovementPerformed;
        temp.canceled += Interaction_HandleMovementCancelled;
        #endregion

        #region  --------------- Subscription for ToggleLeft and ToggleRight Action -----------------------
        temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_TOGGLELEFT);
        temp.performed += Interaction_HandleToggleLeftPerformed;
        temp.canceled += Interaction_HandleToggleLeftCancelled;

        temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_TOGGLERIGHT);
        temp.performed += Interaction_HandleToggleRightPerformed;
        temp.canceled += Interaction_HandleToggleRighttCancelled;
        #endregion

        #region  --------------- Subscription for Interact Action -----------------------
        temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_INTERACT);
        temp.performed += Interaction_HandleInteractPerformed;
        temp.canceled += Interaction_HandleInteractCancelled;

        #endregion

        #region  --------------- Subscription for Use Action -----------------------
        temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_USE);
        temp.performed += Interaction_HandleUsePerformed;
        temp.canceled += Interaction_HandleUseCancelled;

        #endregion

        #region  --------------- Subscription for Use Action -----------------------
        temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_LEAVE);
        temp.performed += Interaction_HandleLeavePerformed;
        temp.canceled += Interaction_HandleLeaveCancelled;

        #endregion

        // #region  --------------- Subscription for ToggleCamera Action -----------------------
        // temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_TOGGLECAMERA);
        // temp.performed += Interaction_HandleToggleCameraPerformed;
        // temp.canceled += Interaction_HandleToggleCameraCancelled;

        // #endregion

        // #region  --------------- Subscription for Pause Action -----------------------
        // temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_PAUSE);
        // temp.performed += Interaction_HandlePause;

        // #endregion

    }


    void Interaction_UnSubscribeActionHandlers()
    {
        InputAction temp;
        #region  --------------- Subscription for Movement Action -----------------------
        temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_MOVEMENT);
        temp.performed -= Interaction_HandleMovementPerformed;
        temp.canceled -= Interaction_HandleMovementCancelled;
        #endregion

        #region  --------------- Subscription for ToggleLeft and ToggleRight Action -----------------------
        temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_TOGGLELEFT);
        temp.performed -= Interaction_HandleToggleLeftPerformed;
        temp.canceled -= Interaction_HandleToggleLeftCancelled;

        temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_TOGGLERIGHT);
        temp.performed -= Interaction_HandleToggleRightPerformed;
        temp.canceled -= Interaction_HandleToggleRighttCancelled;
        #endregion

        #region  --------------- Subscription for Interact Action -----------------------
        temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_INTERACT);
        temp.performed -= Interaction_HandleInteractPerformed;
        temp.canceled -= Interaction_HandleInteractCancelled;

        #endregion

        #region  --------------- Subscription for Use Action -----------------------
        temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_USE);
        temp.performed -= Interaction_HandleUsePerformed;
        temp.canceled -= Interaction_HandleUseCancelled;

        #endregion

        #region  --------------- Subscription for Leave Action -----------------------
        temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_LEAVE);
        temp.performed -= Interaction_HandleLeavePerformed;
        temp.canceled -= Interaction_HandleLeaveCancelled;

        #endregion

        // #region  --------------- Subscription for ToggleCamera Action -----------------------
        // temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_TOGGLECAMERA);
        // temp.performed -= Interaction_HandleToggleCameraPerformed;
        // temp.canceled -= Interaction_HandleToggleCameraCancelled;

        // #endregion

        // #region  --------------- Subscription for Pause Action -----------------------
        // temp = PlayerInput.currentActionMap.FindAction(Constants.For_Player.ACTION_NAME_PAUSE);
        // temp.performed -= Interaction_HandlePause;

        // #endregion

    }

    #endregion

    #region Movement
    private void Interaction_HandleMovementCancelled(InputAction.CallbackContext obj) { MovementInput = Vector2.zero; }

    private void Interaction_HandleMovementPerformed(InputAction.CallbackContext obj) { MovementInput = obj.ReadValue<Vector2>(); }
    #endregion

    #region Toggle Left and Right
    private void Interaction_HandleToggleLeftCancelled(InputAction.CallbackContext obj) { _desireToggleLeft = false; }

    private void Interaction_HandleToggleLeftPerformed(InputAction.CallbackContext obj) { _desireToggleLeft = true; }

    private void Interaction_HandleToggleRightPerformed(InputAction.CallbackContext obj) { _desireToggleRight = true; }

    private void Interaction_HandleToggleRighttCancelled(InputAction.CallbackContext obj) { _desireToggleRight = false; }
    #endregion

    #region Interact
    private void Interaction_HandleInteractCancelled(InputAction.CallbackContext obj) { _desireInteract = false; }

    private void Interaction_HandleInteractPerformed(InputAction.CallbackContext obj) { _desireInteract = true; }
    #endregion

    #region Use
    private void Interaction_HandleUsePerformed(InputAction.CallbackContext obj) { _desireUse = true; }

    private void Interaction_HandleUseCancelled(InputAction.CallbackContext obj) { _desireUse = false; }
    #endregion

    #region Leave
    private void Interaction_HandleLeaveCancelled(InputAction.CallbackContext obj) { _desireLeave = false; }

    private void Interaction_HandleLeavePerformed(InputAction.CallbackContext obj) { _desireLeave = true; }
    #endregion

    // #region Toggle Camera
    // private void Interaction_HandleToggleCameraPerformed(InputAction.CallbackContext obj) { _desireToggleCamera = true; }

    // private void Interaction_HandleToggleCameraCancelled(InputAction.CallbackContext obj) { _desireToggleCamera = false; }
    // #endregion

    // #region Pause
    // private void Interaction_HandlePause(InputAction.CallbackContext obj) { GameUI.RaiseOnPause(); }
    // #endregion
    #endregion

}
