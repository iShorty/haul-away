using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioManagement;

///<Summary>The station where you collect fuel by pulling a lever</Summary>
public class FuelStorageStation : BaseStation
{
    [Header("===== FUEL STORE STATION =====")]
    [Header("----- Fuel Values -----")]
    [SerializeField, Min(0)]
    float _fuelRespawnTime = 5; // The cooldown time for this station

    [field: Header("----- Info -----")]
    [field: SerializeField, RenameField(nameof(Info))]
    public FuelInfo Info { get; private set; } = null;  // The object to spawn

    // [Header("----- SpawnPoint -----")]
    // [SerializeField]
    // Transform _fuelSpawnPoint;  // The empty gameobject's transform

    [Header("----- Mesh Reference -----")]
    // [SerializeField]
    // Text _cooldownText;  // Text in the world to state how long before player can withdraw fuel

    [SerializeField]
    Transform _coalMesh = default;


    #region Properties
    ///<Summary>Determines whether the fuel storage station is currently cooling down or not</Summary>
    public override bool IsPlayerInteractable => _timer <= 0;
    public override PlayerInteractableType PlayerInteractableType => PlayerInteractableType.NONEOVERRIDESTATION;


    #endregion

    #region  Runtime vars
    float _timer = 0;
    Vector3 _localPos = Vector3.zero;
    bool _needUpdate = false;
    #endregion

    #region Events
    public static event System.Action OnFuelCollected = null;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        _localPos = _coalMesh.transform.localPosition;
    }
    // protected override void OnEnable()
    // {
    //     base.OnEnable();
    //     _cooldownText.enabled = false;
    // }

    // public override void UsePlayerInteraction(int playerIndex)
    // {
    //     base.UsePlayerInteraction(playerIndex);
    // }

    public override void LeavePlayerInteraction(bool forcefully)
    {
        // AudioManager.theAM.PlaySFX("Fuel Spawned");
        AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_FuelSpawn, transform.position, true, true);
        SpawnFuel();

        //Base behaviour but without the setting of growcollider enable to false
        playerUsingStation = null;
        // _notificationUI.gameObject.SetActive(false);
        // _notificationUI.text = UI_UNINTERACTED;
    }

    public override bool UpdateInteract() { return true; }

    public override void FixedUpdateInteract() { }

    public void GameUpdate()
    {
        if (!_needUpdate) return;

        if (_timer <= 0)
        {
            _needUpdate = false;
            UpdateCoalMeshHeight(Constants.For_PlayerStations.FUELSTORAGESTATION_COALMESH_STARTHEIGHT);
            AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_FuelIncrease, transform.position, true, true);
            // _cooldownText.enabled = false;
            return;
        }

        _timer -= Time.deltaTime;
        float newHeight = Mathf.Lerp(-0.2f, Constants.For_PlayerStations.FUELSTORAGESTATION_COALMESH_STARTHEIGHT, (1 - _timer / _fuelRespawnTime));
        UpdateCoalMeshHeight(newHeight);
    }

    void SpawnFuel()
    {
        if (_timer > 0) return;

        OnFuelCollected?.Invoke();
        FuelItem fuel = FuelPool.GetInstanceOf(Info);
        playerUsingStation.Interaction_PickUpItem(fuel);

        // FuelPool.GetInstanceOf(Info, _fuelSpawnPoint.position);
        // Instantiate(_fuelPrefab, _fuelSpawnPoint.position, Quaternion.identity);
        // _cooldownText.enabled = true;
        _needUpdate = true;
        _timer = _fuelRespawnTime;
    }

    void UpdateCoalMeshHeight(float newHeight)  // The function to display cooldown time
    {
        // _cooldownText.text = _timer.ToString("F0");
        _localPos.y = newHeight;
        _coalMesh.localPosition = _localPos;
    }


}
