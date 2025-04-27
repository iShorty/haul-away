using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinearEffects;
using System;
using static UnityEngine.InputSystem.InputAction;

///<Summary>Will hold a flowchart to faciliate tutorials. This monobehaviour will hold useful methods to be called by the FlowChart during tutorial!</Summary>
[RequireComponent(typeof(BaseFlowChart))]
public class TutorialManager : GenericManager<TutorialManager>
{
    #region ------- Constants ------------
    const int BLOCKINDEX_AFTER_ENTER_MULTIUSE_STATION = 8;
    const int BLOCKINDEX_AFTER_SUCCESSFUL_GRAPPLE = 9;
    const int BLOCKINDEX_AFTER_FUELCOLLECTED = 10;
    const int BLOCKINDEX_AFTER_FUELBURNT = 11;
    const int BLOCKINDEX_AFTER_ENTERDESTINATION_TRIGGER = 14;

    const string BLOCKNAME_ASSERTOBJECTIVEUI = "AssertObjectiveUI";
    const string BLOCKNAME_SETBLOCKINPUT_FALSE = "SetNextBlockInput_False";
    const string BLOCKNAME_SETBLOCKINPUT_TRUE = "SetNextBlockInput_True";
    const string BLOCKNAME_ASSERT_TUTORIAL_INDICATOR = "AssertTutorialIndicator";
    #endregion

    #region ---------- Exposed Fields --------------
    [Header("----- Pause Timer -----")]
    ///<Summary>Stops the timer from starting when the game starts (as well as when game unpauses) so long as this bool is true</Summary>
    [SerializeField]
    bool _autoPauseTimer = default;

    [Header("----- Block Names -----")]
    [SerializeField]
    string[] _blockNames = new string[0];

    #endregion

    #region ----------- Runtime -----------
#if UNITY_EDITOR
    [SerializeField, ReadOnly, Header("----- Runtime -----")]
#endif
    int _currentBlockNameIndex = default;

    #endregion

    MasterControls _masterControls = default;

    BaseFlowChart _flow = default;


    string nextBlockName => _blockNames[_currentBlockNameIndex];

    protected override void OnGameAwake()
    {
        _flow = GetComponent<BaseFlowChart>();
        _flow.GameAwake();
        GlobalEvents.OnGameUpdate_DURINGGAME += HandleUpdate;
        GlobalEvents.OnGameStart += HandleGameStart;
        GlobalEvents.OnGameResume += HandleResume;

        //Doesnt matter who presses the button, the next text will load
        _masterControls = GlobalPlayerInputManager.MasterControls;

#if UNITY_EDITOR
        Debug.Assert(_flow, $"{name} should have flow chart on it!", this);
#endif
    }

    public override void OnDestroy()
    {
        GlobalEvents.OnGameUpdate_DURINGGAME -= HandleUpdate;
        GlobalEvents.OnGameStart -= HandleGameStart;
        GlobalEvents.OnGameResume -= HandleResume;

        #region -------- Incase ----------
        _masterControls.Gameplay.ContinueText.performed -= TryPlayNextBlockName;
        ToggleSubMultiUseGrappleSuccessfully(false);
        ToggleSubToBoatEnterDestinationTrigger(false);
        ToggleSubToEndOnDelivering(false);
        ToggleSubToOnFuelBurnt(false);
        ToggleSubToOnFuelCollected(false);
        ToggleSubToPlayerEnterMultiStation(false);
        #endregion
    }

    private void HandleGameStart()
    {
        _currentBlockNameIndex = 0;
        _flow.PlayBlock(nextBlockName);
    }


    private void HandleResume()
    {
        switch (_autoPauseTimer)
        {
            case true:
                //Stop and reset the game timer
                ToggleGameTimer(false);
                ResetGameTimer();
                break;

            case false:
                break;
        }
    }


    private void HandleUpdate()
    {
        _flow.GameUpdate();

    }

    #region ================ Public Methods ============================
    ///<Summary>Used when tutorial has a trigger event which will play the next block</Summary>
    public void TryPlayNextBlockName(CallbackContext obj)
    {
        //If current index reached the end of the blocks to play,
        if (_currentBlockNameIndex == _blockNames.Length - 1)
        {
            _masterControls.Gameplay.Interact.performed -= TryPlayNextBlockName;
            return;
        }

        _currentBlockNameIndex++;
        _flow.PlayBlock(nextBlockName);
    }

    ///<Summary>Used when tutorial has a trigger event which will play the next block</Summary>
    public void PlayBlockName(int index)
    {
        _currentBlockNameIndex = index;
        _flow.PlayBlock(nextBlockName);
    }

    ///<Summary>Toggles the players' ability to press "Confirm" in game (Triangle button as of post beta)</Summary>
    public void ToggleNextBlockInput(bool state)
    {
        switch (state)
        {
            case true:
                _masterControls.Gameplay.ContinueText.performed += TryPlayNextBlockName;
                break;
            case false:
                _masterControls.Gameplay.ContinueText.performed -= TryPlayNextBlockName;
                break;
        }
    }

    public void ToggleAutoPauseTimer(bool state)
    {
        _autoPauseTimer = state;
    }

    #region ----------------- Master Game Manager ------------------
    ///<Summary>Toggles the game timer, starting or pausing it depending on the boolean passed in</Summary>
    public void ToggleGameTimer(bool state)
    {
        MasterGameManager.ToggleGameTimer(state);
    }

    ///<Summary>Resets the game timer</Summary>
    public void ResetGameTimer()
    {
        MasterGameManager.SetGameTimer(MasterGameManager.CurrentLevelInfo.LevelDuration);
    }
    #endregion

    #region --------------- (1) SubToPlayerEnterMultiStation() -------------------

    ///<Summary>Subscribe or Unsubscribe to the MultiUseStation.OnPlayerEnter event for tutorial</Summary>
    public void ToggleSubToPlayerEnterMultiStation(bool sub)
    {
        if (sub)
            MultiUseStation.OnPlayerEnter += HandleEnterMultiStation;
        else
            MultiUseStation.OnPlayerEnter -= HandleEnterMultiStation;
    }

    ///<Summary>Handles what happen when player enters the LEFT handside multistation</Summary>
    private void HandleEnterMultiStation(int multiUseStationIndex)
    {
        //Enter the right multiuse station
        if (multiUseStationIndex == 0)
        {
            MultiUseStation.OnPlayerEnter -= HandleEnterMultiStation;
            //Play the next block
            PlayBlockName(BLOCKINDEX_AFTER_ENTER_MULTIUSE_STATION);
        }
    }
    ///<Summary>Subscribe or Unsubscribe to the MultiUseStation.OnGrappleFireSuccess event for tutorial</Summary>
    public void ToggleSubMultiUseGrappleSuccessfully(bool sub)
    {
        if (sub)
            MultiUseStation.OnGrappleFireSuccess += HandleSuccessfullyGrapple;
        else
            MultiUseStation.OnGrappleFireSuccess -= HandleSuccessfullyGrapple;
    }

    ///<Summary>Handles what happen when player successfully grapples an object using the MultiUseStation</Summary>
    private void HandleSuccessfullyGrapple(IGrappleable grappled)
    {
        MultiUseStation.OnGrappleFireSuccess -= HandleSuccessfullyGrapple;
        //Play the next block
        // TryPlayNextBlockName(default);
        PlayBlockName(BLOCKINDEX_AFTER_SUCCESSFUL_GRAPPLE);
    }
    #endregion

    #region  -------------- (2) SubToFuelStations --------------------
    public void ToggleSubToOnFuelCollected(bool sub)
    {
        if (sub)
            FuelStorageStation.OnFuelCollected += HandleFuelCollected;
        else
            FuelStorageStation.OnFuelCollected -= HandleFuelCollected;
    }

    private void HandleFuelCollected()
    {
        //Remove the previous events to prevent looping back in the tutorial order
        MultiUseStation.OnPlayerEnter -= HandleEnterMultiStation;
        MultiUseStation.OnGrappleFireSuccess -= HandleSuccessfullyGrapple;

        _flow.PlayBlock(BLOCKNAME_ASSERTOBJECTIVEUI);
        //Assert tutorial indicator ui is active
        _flow.PlayBlock(BLOCKNAME_ASSERT_TUTORIAL_INDICATOR);
        //Assert playerinput next tutorial text is disabled
        _flow.PlayBlock(BLOCKNAME_SETBLOCKINPUT_FALSE);

        FuelStorageStation.OnFuelCollected -= HandleFuelCollected;
        PlayBlockName(BLOCKINDEX_AFTER_FUELCOLLECTED);
        // TryPlayNextBlockName(default);
    }

    public void ToggleSubToOnFuelBurnt(bool sub)
    {
        if (sub)
            FuelBurnerStation.OnFuelBurnt += HandleFuelBurnt;
        else
            FuelBurnerStation.OnFuelBurnt -= HandleFuelBurnt;
    }

    private void HandleFuelBurnt()
    {
        FuelBurnerStation.OnFuelBurnt -= HandleFuelBurnt;
        // TryPlayNextBlockName(default);
        PlayBlockName(BLOCKINDEX_AFTER_FUELBURNT);
    }


    #endregion

    #region ---------------- (3) SubToDestination --------------
    public void ToggleSubToBoatEnterDestinationTrigger(bool sub)
    {
        if (sub)
            TutorialColliderTrigger.OnBoatEnterDetectionTrigger += HandleBoatEnterDestinationTrigger;
        else
            TutorialColliderTrigger.OnBoatEnterDetectionTrigger -= HandleBoatEnterDestinationTrigger;
    }

    private void HandleBoatEnterDestinationTrigger()
    {
        TutorialColliderTrigger.OnBoatEnterDetectionTrigger -= HandleBoatEnterDestinationTrigger;
        PlayBlockName(BLOCKINDEX_AFTER_ENTERDESTINATION_TRIGGER);
        // TryPlayNextBlockName(default);
    }

    public void ToggleSubToEndOnDelivering(bool sub)
    {
        if (sub)
            Destination.OnDeliverCargoToDestination += HandleDeliverCargoToDestination;
        else
            Destination.OnDeliverCargoToDestination -= HandleDeliverCargoToDestination;
    }

    private void HandleDeliverCargoToDestination(Destination obj)
    {
        //End lvl
        Destination.OnDeliverCargoToDestination -= HandleDeliverCargoToDestination;
        // MasterGameManager.SetGameTimer(0);
    }

    #endregion

    #endregion

}
