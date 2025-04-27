using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

///<Summary>This is the class will record and keep track of all instantiated Playerinput instances when player is trying to register their controllers</Summary>
public class GlobalPlayerInputManager : LevelSingleton<GlobalPlayerInputManager>
{
    #region Definition
    public delegate void PlayerJoinedCallBack(PlayerInput obj, int newPlayerCount);
    public delegate void PlayerLeftCallBack(PlayerInput obj);
    #endregion

    [SerializeField]
    PlayerInputManager _inputManager = default;

    public static MasterControls MasterControls { get; set; } = default;

    [Header("----- Runtime -----")]
#if UNITY_EDITOR
    [SerializeField, ReadOnly]
#endif
    List<PlayerInput> _playerInputs = new List<PlayerInput>();
    // HashSet<int> _playerDevicesId = new HashSet<int>();

    #region Statics
    public static PlayerInput GetPlayerInput(int playerIndex) { return instance._playerInputs[playerIndex]; }

    public static int PlayerInputCount => instance._playerInputs.Count;

    public static void TogglePlayerInputManagerJoining(bool state)
    {
        instance._TogglePlayerInputManagerJoining(state);
    }

    void _TogglePlayerInputManagerJoining(bool state)
    {
        if (state)
        {
            // _masterControls.Enable();
            // _masterControls.MainMenu.Join.canceled += HandleJoinPerform;
            _inputManager.EnableJoining();
        }
        else
        {
            // _masterControls.Disable();
            // _masterControls.MainMenu.Join.canceled -= HandleJoinPerform;
            _inputManager.DisableJoining();
        }

    }

    // void HandleJoinPerform(CallbackContext obj)
    // {
    //     InputDevice device = obj.control.device;
    //     if (_playerDevicesId.Contains(device.deviceId)) return;

    //     _playerDevicesId.Add(device.deviceId);
    //     PlayerInputManager m = PlayerInputManager.instance;

    //     m.JoinPlayer(instance._playerInputs.Count, -1, null, device);
    // }

    public static event PlayerJoinedCallBack OnPlayerJoined = null;
    public static event PlayerLeftCallBack OnPlayerLeft = null;

    #endregion

    protected override void GameAwake()
    {
#if UNITY_EDITOR
        Debug.Assert(transform == GlobalSingleton.Instance.transform, $"GlobalPlayerInputManager must be on the GlobalSingleton's transform!", this);
#endif
        MasterControls = new MasterControls();
    }

    private void OnEnable()
    {
        MasterControls.Enable();
    }

    private void OnDisable()
    {
        MasterControls.Disable();
    }

    private void Start()
    {
        _inputManager.onPlayerJoined += RegisterPlayerInput;
        _inputManager.onPlayerLeft += HandlePlayerLeft;
    }


    private void OnDestroy()
    {
        // PlayerInputManager manager = PlayerInputManager.instance;
        _inputManager.onPlayerJoined -= RegisterPlayerInput;
        _inputManager.onPlayerLeft -= HandlePlayerLeft;
    }

    private void RegisterPlayerInput(PlayerInput obj)
    {
#if UNITY_EDITOR
        string devicesPairedToPlayer = "";
        foreach (var item in obj.devices)
        {
            devicesPairedToPlayer += item.displayName;
        }
        Debug.Log($"Player has joined! Player index is {obj.playerIndex} and its device is {devicesPairedToPlayer}", obj);
#endif
        obj.transform.SetParent(transform);
        _playerInputs.Add(obj);
        OnPlayerJoined?.Invoke(obj, _playerInputs.Count);
        _inputManager.joinAction = new InputActionProperty(_inputManager.joinAction.action.Clone());
    }

    private void HandlePlayerLeft(PlayerInput obj)
    {
        OnPlayerLeft?.Invoke(obj);
    }
}
