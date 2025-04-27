using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

public class BoatCamera : MonoBehaviour
{
    // #region Constants
    // const float TRANSITION_DURATION = 0.5f;
    // #endregion

    [Header("References")]
    [SerializeField]
    Transform _target = default;

    [field: SerializeField, RenameField(nameof(Camera))]
    public Camera Camera { get; private set; } = null;

    [Header("----- Info -----")]
    // [field: SerializeField, RenameField(nameof(ZoomedInInfo))]
    // public BoatCameraInfo ZoomedInInfo { get; private set; } = default;
    // [field: SerializeField, RenameField(nameof(ZoomedOutInfo))]
    // public BoatCameraInfo ZoomedOutInfo { get; private set; } = default;
    [SerializeField]
    BoatCameraInfo _currentInfo = default;

    // [Header("----- Settings -----")]
    // [SerializeField]
    // bool _isZoomed = default;


    #region Properties
    // bool isTransitioning => _timer > 0;
    #endregion

    #region Runtime 
    Vector3 _desiredPosition;
    Quaternion _desiredRotation;
    // ///<Summary>The state of the camera. True represents being zoomed in and false represents being zoomed out</Summary>
    // CameraState _state = default;
    // float _timer = default;
    #endregion


    // private void OnEnable()
    // {
    //     _timer = 0;
    //     _isZoomed = true;
    //     _currentInfo = _isZoomed ? ZoomedInInfo : ZoomedOutInfo;
    // }

    public void GameUpdate()
    {
        // if (isTransitioning)
        // {
        //     _timer -= Time.deltaTime;
        // }

#if UNITY_EDITOR
        // Keyboard keyboard = InputSystem.GetDevice<Keyboard>();
        // if (keyboard.gKey.wasPressedThisFrame)
        // {
        //     ToggleZoomInfo();
        // }
        // //Bind this to another key for players to actually use
        // if (Input.GetKeyDown(KeyCode.G))
        // {
        //     //Toggle zoom info
        //     ToggleZoomInfo();
        //     return;
        // }
#endif
    }

    public void GameFixedUpdate()
    {
        //Position lerping (we use vector3.up for Y offset because we dont want to get motion sickness)
        _desiredPosition = _target.position + _target.forward * _currentInfo.Z_Offset + Vector3.up * _currentInfo.Y_Offset;
        _desiredPosition = Vector3.Lerp(transform.position, _desiredPosition, _currentInfo.PositionSmoothing * Time.fixedDeltaTime);
        transform.position = _desiredPosition;

        //SLerp rotation
        _desiredRotation = Quaternion.Euler(_currentInfo.X_Rotation, _target.eulerAngles.y, transform.eulerAngles.z);
        _desiredRotation = Quaternion.Slerp(transform.rotation, _desiredRotation, _currentInfo.RotationSmoothing * Time.fixedDeltaTime);
        transform.rotation = _desiredRotation;
    }

    // ///<Summary>Toggles the camera zoom state</Summary>
    // public void ToggleZoomInfo()
    // {
    //     if (isTransitioning) return;

    //     _isZoomed = !_isZoomed;
    //     _timer = TRANSITION_DURATION;
    //     _currentInfo = _isZoomed ? ZoomedInInfo : ZoomedOutInfo;
    // }


}
