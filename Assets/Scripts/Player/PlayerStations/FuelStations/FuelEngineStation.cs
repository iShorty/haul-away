using System.Collections;
using System.Collections.Generic;
using AudioManagement;
using UnityEngine;
using LinearEffects;
using TMPro;

///<Summary>Allows for toggling of the boat fuel consumption.</Summary>
[RequireComponent(typeof(BaseFlowChart))]
public class FuelEngineStation : OverridePlayerMovementStation
{
    #region ------------- Constants -------------

    public enum ThrottleState
    {
        ///<Summary>The boat is in reverse</Summary>
        REVERSE = 0
        ,
        ///<Summary>The boat does not move</Summary>
        STOP = 1
        ,
        ///<Summary>The boat is moving forwards but slowly</Summary>
        SLOW = 2
        ,
        ///<Summary>The boat is moving forwards but quickly</Summary>
        FAST = 3
    }

    [System.Serializable]
    public struct ThrottleValue
    {
        public const int NUMBER_OF_STATES = 4;
        public float speed;
        public float fuelConsumptionRate;

        public ThrottleValue(float speed, float fuelConsumptionRate)
        {
            this.speed = speed;
            this.fuelConsumptionRate = fuelConsumptionRate;
        }
    }

    public const string BLOCKNAME_LERPTO_KEYWORD = "LerpTo_";
    public const string BLOCKNAME_SHOWWORD = "ShowWord";

    #endregion

    BaseFlowChart _flowChart = default;

    // ///<Summary>There is a slight cooldown for the fuel engine station when someone toggles the engine</Summary>
    // float _timer = default;

    [Header("===== FUEL ENGINE =====")]
    [SerializeField]
    ///<Summary>This array holds all the throttle values for the boat's lever to engage in. The elements follows the ThrottleState enum</Summary>
    ThrottleValue[] _throttleValues = new ThrottleValue[4];

    [SerializeField]
    ThrottleState _currentState = ThrottleState.SLOW;

    [Header("----- UI -----")]
    [SerializeField]
    BaseUIIndicator _indicator = default;
    [SerializeField]
    TextMeshProUGUI _textUI = default;
    #region Public Methods

    protected override void Awake()
    {
        base.Awake();
        _flowChart = GetComponent<BaseFlowChart>();
        _flowChart.GameAwake();
        _indicator.transform.SetParent(PlayerManager.PlayerCanvas.transform);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        int currentState = (int)_currentState;
        //Set the throttle to the correct state's angle
        BoatManager.Controller.SetBoatMovement(_throttleValues[currentState].speed, _throttleValues[currentState].fuelConsumptionRate);
        PlayThrottleBlock();
        // _timer = Constants.For_PlayerStations.FUEL_ENGINESTATION_COOLDOWN;
        GlobalEvents.OnGameUpdate_DURINGGAME += GameUpdate;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GlobalEvents.OnGameUpdate_DURINGGAME -= GameUpdate;
    }

    public void GameUpdate()
    {
        _flowChart.GameUpdate();
        _indicator.GameUpdate();
    }

    #endregion


    #region Player Interactable Methods
    public override bool IsPlayerInteractable => !_flowChart.IsPlaying;

    protected override float stationCollider_HalfExtents_Z => GetComponent<BoxCollider>().size.z * 0.5f;

    public override void FixedUpdateInteract() { }

    public override bool UpdateInteract()
    {
        if (playerUsingStation.DesireToggleLeft)
        {
            int currentState = IncrementThrottleState(-1);
            //Execute toggling of fuel engine here
            BoatManager.Controller.SetBoatMovement(_throttleValues[currentState].speed, _throttleValues[currentState].fuelConsumptionRate);

            PlayThrottleBlock();

        }
        else if (playerUsingStation.DesireToggleRight)
        {
            int currentState = IncrementThrottleState(1);
            BoatManager.Controller.SetBoatMovement(_throttleValues[currentState].speed, _throttleValues[currentState].fuelConsumptionRate);
            PlayThrottleBlock();
        }
        return true;
    }
    #endregion

    int IncrementThrottleState(int value)
    {
        int currentState = (int)_currentState;
        currentState = Mathf.RoundToInt(Mathf.Repeat(currentState + value, ThrottleValue.NUMBER_OF_STATES));
        _currentState = (ThrottleState)currentState;
        return currentState;
    }

    void PlayThrottleBlock()
    {
        //Set text ui to a new text 
        _textUI.text = _currentState.ToString();

        //Play animation    
        _flowChart.PlayBlock($"{BLOCKNAME_LERPTO_KEYWORD}{_currentState.ToString()}");
        _flowChart.TryStopBlock(BLOCKNAME_SHOWWORD);
        _flowChart.PlayBlock(BLOCKNAME_SHOWWORD);

        AudioEvents.RaiseOnPlay3DAtLocation(AudioClipType.SFX_SpeedLever, transform.position, true, true);
    }



}
