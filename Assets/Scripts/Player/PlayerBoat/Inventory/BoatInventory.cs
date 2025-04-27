using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
//Inventory only keeps tracks of players n cargos onboard
public partial class BoatInventory : MonoBehaviour
{
    ///<Summary>Contains Players, Cargo and Projectiles</Summary>
    public Dictionary<Rigidbody, FloatableProp> AllPropsOnBoat { get; private set; } = new Dictionary<Rigidbody, FloatableProp>();

    ///<Summary>Given the cargo info, a list of each cargo type on the boat will be kept tracked here</Summary>
    public Dictionary<CargoInfo, List<BaseCargo>> AllCargosOnBoat { get; private set; } = new Dictionary<CargoInfo, List<BaseCargo>>();

    #region Get Methods
    public bool IsFloatablePropOnBoat(Rigidbody rb)
    {
        return AllPropsOnBoat.ContainsKey(rb);
    }

    public bool IsCargoTypeOnBoard(CargoInfo type)
    {
        return AllCargosOnBoat.ContainsKey(type);
    }

    public bool TryGetCargos(CargoInfo type, out List<BaseCargo> list)
    {
        list = null;

        if (!AllCargosOnBoat.ContainsKey(type))
        {
            return false;
        }

        list = AllCargosOnBoat[type];
        return true;
    }


    #endregion

    #region Trigger Methods

    #region Enter & Stay
    private void OnTriggerEnter(Collider other)
    {
        EvaluateTrigger(other);
    }

    private void OnTriggerStay(Collider other)
    {
        EvaluateTrigger(other);
    }

    private void EvaluateTrigger(Collider other)
    {
        //Filter out terrain
        if (other.attachedRigidbody == null) return;

        //Filter out station objects
        if (BoatManager.IsStation(other)) return;

        if (Destination.IsDestination(other)) return;

        //========= MAIN DICTIONARY CHECK ===========
        if (AllPropsOnBoat.ContainsKey(other.attachedRigidbody)) return;

        //Projectiles, Players and BaseCargo are on the interactable layer and therefore subjected to this code
        //Differentiate between cargo from players & projectiles
        if (PlayerPickableManager.IsCargo(other))
        {
            BaseCargo cargo = other.attachedRigidbody.GetComponent<BaseCargo>();
            if (cargo as MysteryBox != null) return;

            AddCargo(cargo);
            return;
        }

        // if(other.attachedRigidbody.gameObject.GetComponent<OctopusController>())
        // {
        //     Debug.Log("octopus passed boat inv considered a player?");
        // }
        AddProp(other);
    }

    void AddCargo(BaseCargo cargo)
    {

#if UNITY_EDITOR
        if (cargo == null)
        {
            // Debug.LogError($"Collider {other.name} should be expected to have a BaseCargo on it!", other);
            Debug.LogError($"Collider {cargo.name} should be expected to have a BaseCargo on it!", cargo);
            return;
        }
#endif

        //Add key if there isnt one initially
        if (!AllCargosOnBoat.ContainsKey(cargo.CargoInfo))
        {
            AllCargosOnBoat.Add(cargo.CargoInfo, new List<BaseCargo>());
        }

        AllCargosOnBoat[cargo.CargoInfo].Add(cargo);
        // AllPropsOnBoat.Add(cargo.attachedRigidbody, cargo);
        AllPropsOnBoat.Add(cargo.PropRigidBody, cargo);

        cargo.Prop_OnEvaluateBoatInventoryTrigger();
    }

    void AddProp(Collider other)
    {
        // if(other.attachedRigidbody.gameObject.GetComponent<OctopusController>())
        // {
        //     Debug.Log("octopus passed boat inv, in AddProp");
        // }
        FloatableProp prop = other.attachedRigidbody.GetComponent<FloatableProp>();

        // if(other.attachedRigidbody.gameObject.GetComponent<OctopusController>())
        // {
        //     Debug.Log("octopus passed boat inv, in AddProp");
        // }
#if UNITY_EDITOR
        if (prop == null)
        {
            Debug.LogError($"Collider {other.name} should be expected to have a FloatableProp on it!", other);
            return;
        }
#endif

        AllPropsOnBoat.Add(other.attachedRigidbody, prop);
        // if(other.attachedRigidbody.gameObject.GetComponent<OctopusController>())
        // {
        //     Debug.Log("octopus added to allpropsonboat list");
        // }
        prop.Prop_OnEvaluateBoatInventoryTrigger();
    }
    #endregion


    #region Exit

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null) return;

        //========= MAIN DICTIONARY CHECK ===========
        if (!AllPropsOnBoat.ContainsKey(other.attachedRigidbody))
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{other.name} just left the BoatInventory Trigger without being registered! Is this normal?", other);
#endif
            return;
        }

        // //Filter out station objects
        // if (BoatManager.IsPartOfBoat(other)) return;

#if UNITY_EDITOR
        // if (other.gameObject.layer != Constants.For_Layer_and_Tags.LAYERINDEX_INTERACTABLE)
        // {
        //     // Debug.LogError($"{other.gameObject.layer} layer should not be colliding with BoatInventory layer!");
        //     return;
        // }
#endif

        //Projectiles, Players and BaseCargo are on the interactable layer and therefore subjected to this code
        //Differentiate between cargo from players & projectiles
        if (PlayerPickableManager.IsCargo(other))
        {
            RemoveCargo(other);
            return;
        }

        RemoveProp(other);
    }

    private void RemoveProp(Collider other)
    {
        FloatableProp prop = AllPropsOnBoat[other.attachedRigidbody];

#if UNITY_EDITOR
        if (prop == null)
        {
            Debug.LogError($"Collider {other.name} should be expected to have a FloatableProp on it!", other);
            return;
        }
#endif

        AllPropsOnBoat.Remove(other.attachedRigidbody);
    }

    private void RemoveCargo(Collider other)
    {
        BaseCargo cargo = other.attachedRigidbody.GetComponent<BaseCargo>();

#if UNITY_EDITOR
        if (cargo == null)
        {
            Debug.LogError($"Collider {other.name} should be expected to have a BaseCargo on it!", other);
            return;
        }
#endif

        AllCargosOnBoat[cargo.CargoInfo].Remove(cargo);
        AllPropsOnBoat.Remove(other.attachedRigidbody);
    }

    #endregion
    #endregion
}
