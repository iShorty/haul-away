using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;

public partial class PlayerManager
{

#if UNITY_EDITOR
    [SerializeField]
    [Header("----- Debug -----")]
    [Header("===== Spawning =====")]
    [Tooltip("Toggle this to true to join and spawn player live during game update")]
    ///<Summary>Toggle this to true to join and spawn player live during game update</Summary>
    bool _debugLiveJoinPlayer = false;


    bool _debugToggledJoining = false;
#endif

    public PlayerModelInfo PlayerModelInfo = default;


    private void Spawning_GameAwake()
    {
        Spawning_SpawnPlayers();
#if UNITY_EDITOR
        if (_debugLiveJoinPlayer)
        {
            GlobalPlayerInputManager.TogglePlayerInputManagerJoining(true);
            _debugToggledJoining = true;
        }
        GlobalPlayerInputManager.OnPlayerJoined += Spawning_EditorGameOnPlayerJoined;
#endif
    }



    ///<Summary>Spawns players as children of the playermanager</Summary>
    void Spawning_SpawnPlayers()
    {
        for (int i = 0; i < GlobalPlayerInputManager.PlayerInputCount; i++)
        {
            Spawning_SpawnPlayer(i);
        }
    }

    void Spawning_SpawnPlayer(int playerIndex)
    {
        PlayerController p = Instantiate(_playerPrefab).GetComponent<PlayerController>();
        PlayerInput input = GlobalPlayerInputManager.GetPlayerInput(playerIndex);
        //Spawn in player animator 
Animator playerModelAnimator = Instantiate(PlayerModelInfo.PlayerModels[playerIndex]).GetComponentInChildren<Animator>();

        p.transform.SetParent(transform);
        p.transform.position = BoatManager.GetPlayerRespawnPosition(playerIndex).position;
        p.transform.localRotation = Quaternion.identity;

        p.Initialize(playerIndex, input,BoatManager.BoatCamera.transform, playerModelAnimator);
        _players.Add(p);
    }


#if UNITY_EDITOR
    private void Spawning_EditorGameUpdate()
    {
        if (_debugLiveJoinPlayer)
        {
            if (!_debugToggledJoining)
            {
                _debugToggledJoining = true;
                GlobalPlayerInputManager.TogglePlayerInputManagerJoining(true);
            }
        }
        else
        {
            if (_debugToggledJoining)
            {
                _debugToggledJoining = false;
                GlobalPlayerInputManager.TogglePlayerInputManagerJoining(false);
            }
        }
    }

    private void Spawning_EditorGameOnPlayerJoined(PlayerInput obj, int newPlayerCount)
    {
        //Spawn player
        Spawning_SpawnPlayer(newPlayerCount-1);
    }
#endif

}
