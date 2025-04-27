using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverToAllDestinationsOnceEventHook : BaseObjectiveEventHook
{
    Destination[] _destinationsInLevel = null;
    HashSet<Destination> _allDestinationsDeliveredToSoFar = default;

    BaseLevelObjectiveInfo _objectiveInfo = default;

    protected override void Awake()
    {
        base.Awake();
        _allDestinationsDeliveredToSoFar = new HashSet<Destination>();
        Destination.OnDeliverCargoToDestination += Handle_OnDeliverCargo;

        _destinationsInLevel = FindObjectsOfType<Destination>();
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        Destination.OnDeliverCargoToDestination -= Handle_OnDeliverCargo;
    }

    public override void SetObjectiveInfo(BaseLevelObjectiveInfo info)
    {
        _objectiveInfo = info;
    }

    private void Handle_OnDeliverCargo(Destination obj)
    {
        if (FulFilled) return;

        if (_allDestinationsDeliveredToSoFar.Contains(obj)) return;


        _allDestinationsDeliveredToSoFar.Add(obj);
        UpdateObjectiveText();
        //Compare with the environmentmanager's number of destination
        if (_allDestinationsDeliveredToSoFar.Count >= _destinationsInLevel.Length)
        {
            RaiseStarConditionFulFilled(this, _objectiveIndex);
        }
    }

    protected override void ResetEventHook()
    {
        _allDestinationsDeliveredToSoFar.Clear();
        base.ResetEventHook();
    }

    protected override void UpdateObjectiveText()
    {
        //For this, 0 = number of destinations in the lvl, 1 = current progress so far
        string s = string.Format(_objectiveInfo.InGameTextFormat, _destinationsInLevel.Length, _allDestinationsDeliveredToSoFar.Count);
        GameUI.InGameUI_UpdateObjectiveText(_objectiveIndex, s);
    }
}
