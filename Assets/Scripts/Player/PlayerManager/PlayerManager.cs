using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.PlayerInput;

public partial class PlayerManager : GenericManager<PlayerManager>
{
    #region Exposed Fields

    [Header("----- Scene References -----")]
    [SerializeField]
    Canvas _playerCanvas = default;

    [Header("----- Asset References -----")]
    [SerializeField]
    GameObject _playerPrefab = default;

    [Header("----- Respawn -----")]
    [SerializeField]
    ///<Summary>The respawn indicators for the player respawn position. The array elements must be arranged in the player index order from 0 to 3</Summary>
    RespawnIndicator[] _respawnIndicators = new RespawnIndicator[0];

    [Range(0, 100)]
    float _playerRespawnTime = 5f;


    #endregion

    #region Hidden Field
    List<PlayerController> _players = default;
    MasterControls _masterControls = default;
    #endregion

    #region Properties
    public static GameObject SceneObject => instance.gameObject;

    #endregion

    #region static 
    public static Canvas PlayerCanvas => instance._playerCanvas;
    #endregion



    #region Handle Global Events

    #region Initialization & Destruction
    protected override void OnGameAwake()
    {
        _masterControls = GlobalPlayerInputManager.MasterControls;

        _players = new List<PlayerController>();
        GlobalEvents.OnGameUpdate_DURINGGAME += DuringGameUpdate;
        GlobalEvents.OnGameFixedUpdate_DURINGGAME += DuringGameFixedUpdate;
        GlobalEvents.OnGamePause += OnGamePause;
        GlobalEvents.OnGameResume += OnGameResume;
        GlobalEvents.OnGameReset += OnGameReset;

        _masterControls.Gameplay.Pause.performed += RaisePause;

        RespawnChecker_GameAwake();
        Spawning_GameAwake();

#if UNITY_EDITOR

        EditorChecks();
#endif

        // //Set all the player indices
        // for (int i = 0; i < _players.Length; i++)
        // {
        //     _players[i].GameAwake(i);
        // }
    }


    public override void OnDestroy()
    {
        GlobalEvents.OnGameUpdate_DURINGGAME -= DuringGameUpdate;
        GlobalEvents.OnGameFixedUpdate_DURINGGAME -= DuringGameFixedUpdate;
        GlobalEvents.OnGamePause -= OnGamePause;
        GlobalEvents.OnGameResume -= OnGameResume;
        GlobalEvents.OnGameReset -= OnGameReset;


        _masterControls.Gameplay.Pause.performed -= RaisePause;
        RespawnChecker_GameDestroy();
    }



    private void OnGameReset()
    {
        //Respawn all of the players at the respawn points

        //Hide all respawn indciator
        foreach (var indicator in _respawnIndicators)
        {
            indicator.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Update
    void DuringGameUpdate()
    {
#if UNITY_EDITOR
        Spawning_EditorGameUpdate();
#endif

        for (int i = 0; i < _players.Count; i++)
        {
            _players[i].GameUpdate();
        }

        RespawnChecker_GameUpdate();

    }


    void DuringGameFixedUpdate()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            _players[i].FixedGameUpdate();
        }
    }
    #endregion

    void OnGamePause()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            _players[i].GamePause();
        }
    }

    void OnGameResume()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            _players[i].GameResume();
        }
    }

    #endregion


    private void RaisePause(InputAction.CallbackContext obj)
    {
        GameUI.RaiseOnPause();
    }



}
