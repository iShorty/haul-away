using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatManager : GenericManager<BoatManager>
{
    [Header("----- Controller -----")]
    [Header("===== BOAT COMPONENTS =====")]
    [SerializeField]
    BoatController _controller = default;

    [Header("----- Inventory -----")]
    [SerializeField]
    BoatInventory _inventory = default;

    [Header("----- Deck -----")]
    [SerializeField]
    BoatDeck _deck = default;

    [Header("----- Detection Triggers -----")]
    [SerializeField]
    PlayerBoatWarningDetection _warningDetection = default;

    [SerializeField]
    PlayerBoatDangerDetection _dangerDetection = default;

    [SerializeField]
    PlayerBoatCargoDetection _cargoDetection = default;

    [Header("----- Stations -----")]
    [SerializeField]
    MultiUseStation[] _multiUseStations = default;

    [SerializeField]
    SteeringStation _steeringStation = default;

    [SerializeField]
    FuelBurnerStation _depositStation = default;

    [SerializeField]
    FuelStorageStation _storeStation = default;

    [Header("===== MAIN CAMERA =====")]
    [SerializeField]
    BoatCamera _boatCamera = default;

    // [SerializeField]
    // BoatRespawnHands _respawnHands = default;

    #region Public Statics 
    public static BoatController Controller => instance._controller;
    public static BoatInventory BoatInventory => instance._inventory;
    public static BoatDeck BoatDeck => instance._deck;
    public static BoatCamera BoatCamera => instance._boatCamera;
    public static Camera MainCamera => instance._boatCamera.Camera;
    public static SteeringStation SteeringStation => instance._steeringStation;
    public static FuelBurnerStation FuelDepositStation => instance._depositStation;
    public static FuelStorageStation FuelStoreStation => instance._storeStation;
    public static Transform GetPlayerRespawnPosition(int playerIndex) { return instance._controller.PlayerRespawnPositions[playerIndex]; }
    // public static BoatRespawnHands RespawnHands => instance._respawnHands;

    #region Is Method
    ///<Summary>Checks if collider is under the rigidbody belonging to the BoatController. Colliders belonging to that rigidbody could be: Stations and Boat model colliders</Summary>
    public static bool IsPartOfBoat(Collider collider)
    {
        if (collider.attachedRigidbody == null)
        {
            return false;
        }

        // if (collider.gameObject == instance._warningDetection) return false;
        // if (collider.gameObject == instance._dangerDetection) return false;
        // if (collider.gameObject == instance._cargoDetection) return false;

        return collider.attachedRigidbody.gameObject.layer == Constants.For_Layer_and_Tags.LAYERINDEX_PLAYERBOATMODEL;
    }

    public static bool IsStation(Collider c)
    {
        return (c.CompareTag(Constants.For_Layer_and_Tags.TAG_STATION));
    }

    public static bool IsPropOnBoat(Rigidbody rb)
    {
        return instance._inventory.IsFloatablePropOnBoat(rb);
    }
    #endregion


    #region Get Method
    public static MultiUseStation GetMultiUseStation(int index)
    {
        return instance._multiUseStations[index];
    }

    public static bool TryGetCargoOnBoat(CargoInfo data, out List<BaseCargo> list)
    {
        return instance._inventory.TryGetCargos(data, out list);
    }
    #endregion



    #endregion



    #region Handle Manager Methods

    protected override void OnGameAwake()
    {
        GlobalEvents.OnGameUpdate_DURINGGAME += DuringGameUpdate;
        GlobalEvents.OnGameFixedUpdate_DURINGGAME += DuringGameFixedUpdate;
        GlobalEvents.OnGameStart += OnGameStart;
        GlobalEvents.OnGamePause += OnGamePause;
        GlobalEvents.OnGameResume += OnGameResume;
        GlobalEvents.OnGameEnd += OnGameEnd;
        GlobalEvents.OnGameReset += OnGameReset;


        for (int i = 0; i < _multiUseStations.Length; i++)
        {
            _multiUseStations[i].GameAwake(i);
        }

        // BoatCamera.GameAwake();
        // _respawnHands.GameAwake();
        OnGameReset();
    }


    public override void OnDestroy()
    {
        GlobalEvents.OnGameUpdate_DURINGGAME -= DuringGameUpdate;
        GlobalEvents.OnGameFixedUpdate_DURINGGAME -= DuringGameFixedUpdate;
        GlobalEvents.OnGameStart -= OnGameStart;
        GlobalEvents.OnGamePause -= OnGamePause;
        GlobalEvents.OnGameResume -= OnGameResume;
        GlobalEvents.OnGameEnd -= OnGamePause;
        GlobalEvents.OnGameEnd -= OnGameEnd;
        GlobalEvents.OnGameReset -= OnGameReset;

    }

    #region One-Frame Events
    void OnGameStart()
    {
        _warningDetection.gameObject.SetActive(true);
        _dangerDetection.gameObject.SetActive(true);
    }

    void OnGamePause()
    {
        _warningDetection.gameObject.SetActive(false);
        _dangerDetection.gameObject.SetActive(false);
        _controller.GamePause();
    }

    void OnGameResume()
    {
        _warningDetection.gameObject.SetActive(true);
        _dangerDetection.gameObject.SetActive(true);
        _controller.GameResume();

    }

    private void OnGameReset()
    {
        Controller.transform.localPosition = Vector3.zero;
        Controller.transform.localRotation = Quaternion.identity;
    }

    private void OnGameEnd()
    {
        OnGamePause();
    }

    #endregion

    #region  Updates
    void DuringGameUpdate()
    {
        Controller.GameUpdate();
        for (int i = 0; i < _multiUseStations.Length; i++)
        {
            _multiUseStations[i].GameUpdate();
        }
        _storeStation.GameUpdate();

        BoatCamera.GameUpdate();
        // _respawnHands.GameUpdate();
    }

    void DuringGameFixedUpdate()
    {
        Controller.GameFixedUpdate();
        BoatCamera.GameFixedUpdate();
    }



    #endregion


    #endregion


}
