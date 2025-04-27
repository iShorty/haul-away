using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///<Summary>The station where you toss all the fuel into to fuel the ship. (Does not inherit from basestation)</Summary>
public class FuelBurnerStation : MonoBehaviour
{
    // public override PlayerInteractableType PlayerInteractableType => PlayerInteractableType.NONEOVERRIDESTATION;

#if UNITY_EDITOR
    void Awake()
    {
        Collider c = GetComponent<Collider>();
        Debug.Assert(c != null && c.isTrigger, $"{name} station does not have a collider with is trgger set to true!", c);
        Debug.Assert(gameObject.layer != Constants.For_Layer_and_Tags.LAYERINDEX_PLAYERBOATMODEL, "FuelCollider collider must not be on the playerboat layer!", this);
    }
#endif

    public static event System.Action OnFuelBurnt = null;

    // protected override void OnEnable()
    // {
    //     //Base behaviour but without the player position being set and growing the collider
    //     playerUsingStation = null;
    //     _notificationUI.gameObject.SetActive(false);
    // }

    // public override bool UpdateInteract() { return true; }

    // public override void FixedUpdateInteract() { }

    // public override void UsePlayerInteraction(int playerIndex)
    // {
    //     // Debug.Log("fueldepo UsePlayerInteraction");
    //     //Base behaviour but without the player position being set and growing the collider
    //     playerUsingStation = PlayerManager.GetPlayer(playerIndex);
    //     _notificationUI.text = UI_INTERACTED;

    //     BoatManager.Controller._EngineActive = !BoatManager.Controller._EngineActive;
    // }

    // public override void LeavePlayerInteraction(bool forcefully)
    // {
    //     //Base behaviour but without the setting of growcollider enable to false
    //     playerUsingStation = null;
    //     _notificationUI.gameObject.SetActive(false);
    //     _notificationUI.text = UI_UNINTERACTED;
    // }

    private void OnTriggerEnter(Collider other)
    {
        // If fuel collides with this, destroy it and add fuel level
        if (!PlayerPickableManager.IsFuel(other)) return;

        FuelItem fi = other.attachedRigidbody.GetComponent<FuelItem>();

        // if (!fi) return;

        //Will only collect if the player drops the fuel
        if (fi.IsPlayerInteractable)
        {
            BoatManager.Controller.AddFuel(fi.FuelInfo.FuelValue);
            fi.transform.position = Constants.POOLEDOBJECT_HIDE_POSITION;
            FuelPool.ReturnInstanceOf(fi);
            OnFuelBurnt?.Invoke();
            // Destroy(other.gameObject);
        }
    }
}
