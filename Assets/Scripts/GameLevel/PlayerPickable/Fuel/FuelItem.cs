using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>Fuel item acts like a basecargo where it will only get updated when it enters water</Summary>
public class FuelItem : PlayerPickable,IGrappleable
{

    public override PlayerPickableInfo PickableInfo => FuelInfo;

    [field: Header("----- Info -----")]
    [field: SerializeField, RenameField(nameof(FuelInfo))]
    ///<Summary>The fuel info which this fuel instance belongs to. Return to object pooler using this FuelInfo</Summary>
    public FuelInfo FuelInfo { get; private set; } = null;

    public bool IsGrappleableInteractable => _currentPropState != PropState.SINKING;

    public IGrappleable GetRootGrappleable()
    {
       return isSomeOneHoldingMe?PlayerManager.GetPlayer(_holderIndex).GetRootGrappleable() :this;
    }

    public void LeaveGrapplingInteraction()
    {
          LeavePlayerInteraction(false);
    }

    public void RescueGrappleableInteraction()
    {
       SetPropState(PropState.KINEMATIC);
    }

    public void TargetGrappleableInteraction(int stationIndex)
    {
      _holderIndex = stationIndex;

        LeaveDetection();
    }

    protected override void Awake()
    {
        base.Awake();
        //Check if fuel tag is on the fuel script gameobject
#if UNITY_EDITOR
        Debug.Assert(CompareTag(Constants.For_Layer_and_Tags.TAG_FUEL), $"The fuel item {name} doesnt have the Tag {Constants.For_Layer_and_Tags.TAG_FUEL} on it!", this);
#endif

    }

    protected override void OnSinkTimerUp()
    {
        FuelPool.ReturnInstanceOf(this);
    }

    protected override void RegisterToUpdateLoop()
    {
        PlayerPickableManager.RegisterPlayerPickable(this);
    }
}
