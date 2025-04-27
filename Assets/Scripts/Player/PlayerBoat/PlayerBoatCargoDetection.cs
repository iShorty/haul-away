using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>Handles indicators for cargos (on screen only) within a certain range</Summary>
[RequireComponent(typeof(SphereCollider))]
public class PlayerBoatCargoDetection : MonoBehaviour
{

    [SerializeField]
    IndicatorInfo _cargoIndicatorInfo = default;

    private void Awake()
    {
#if UNITY_EDITOR
        Debug.Assert(gameObject.layer != Constants.For_Layer_and_Tags.LAYERINDEX_PLAYERBOATMODEL, "Danger detection sphere collider must not be on the playerboat layer!", this);
#endif
        SphereCollider c = GetComponent<SphereCollider>();
        c.radius = Constants.For_PlayerBoat.DETECTION_RAIDUS_CARGO;
        c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PlayerPickableManager.IsCargo(other)) return;

        if (BoatManager.BoatInventory.IsFloatablePropOnBoat(other.attachedRigidbody)) return;

        BaseCargo c = other.attachedRigidbody.GetComponent<BaseCargo>();
        if (c.CurrentPropState != FloatableProp.PropState.INWATER) return;
        //If basecargo state is just in water (that means no sink timer) 

        UIIndicatorPool.GetIndicator(_cargoIndicatorInfo, PlayerManager.PlayerCanvas.transform, other.attachedRigidbody);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!PlayerPickableManager.IsCargo(other)) return;

        if (BoatManager.BoatInventory.IsFloatablePropOnBoat(other.attachedRigidbody)) return;

        UIIndicatorPool.TryRemoveIndicator(other.attachedRigidbody);
    }

}
