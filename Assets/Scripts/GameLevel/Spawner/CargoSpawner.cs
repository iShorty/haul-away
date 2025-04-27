using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoSpawner : MonoBehaviour
{
    [SerializeField]
    CargoInfo[] _cargoInfo = default;

    [SerializeField]
    [Range(0, 100)]
    float _respawnDuration = default;

    [SerializeField]
    FloatableProp.PropState _startingPropState = default;

    //Runtime
    float _timer = default;
    BaseCargo _spawnedCargo = default;

    // bool _isPaused = default;

#if UNITY_EDITOR
    [SerializeField]
    Color _cargoSpawnerColor = Color.white;
    private void OnDrawGizmosSelected()
    {
        Color prevColor = Gizmos.color;
        Gizmos.color = _cargoSpawnerColor;
        Gizmos.DrawSphere(transform.position, 5f);
        Gizmos.color = prevColor;
    }
#endif

    private void OnEnable()
    {
        _spawnedCargo = null;
        // MasterGameManager.Update
        GlobalEvents.OnGameUpdate_DURINGGAME += GameUpdate;
        // GlobalEvents.OnGamePause += HandlePause;
        // GlobalEvents.OnGameResume += HandleResume;
    }

    // private void HandleResume()
    // {
    //     _isPaused = false;
    // }

    // private void HandlePause()
    // {
    //     _isPaused = true;
    // }

    private void OnDisable()
    {
        GlobalEvents.OnGameUpdate_DURINGGAME -= GameUpdate;
        // GlobalEvents.OnGamePause -= HandlePause;
        // GlobalEvents.OnGameResume -= HandleResume;
    }

    private void GameUpdate()
    {
        // if (_isPaused) return;

        if (_spawnedCargo != null)
        {
            //That means something happened to the prop
            if (_spawnedCargo.CurrentPropState != FloatableProp.PropState.INWATER)
            {
                //Reset spawned cargo and start spawning agn
                _spawnedCargo = null;
            }

            return;
        }

        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            return;
        }


        _timer = _respawnDuration;
        //Spawn
        SpawnRandomCargo();

    }

    private void SpawnRandomCargo()
    {
        int rand = UnityEngine.Random.Range(0, _cargoInfo.Length);
        BaseCargo c = BaseCargoPool.GetInstanceOf(_cargoInfo[rand], FloatableProp.PropState.INWATER, transform.position);
        _spawnedCargo = c;
    }
}
