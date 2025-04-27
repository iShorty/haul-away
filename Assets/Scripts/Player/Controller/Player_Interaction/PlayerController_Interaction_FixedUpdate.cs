using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This file holds methods for interaction for the fixedupdate loop
public partial class PlayerController
{
    #region NONE STATE
    //Maybe we remove the overlap sphere thing and use ontrigger +layer matrix instead
    void Interaction_FixedUpdate_NONE()
    {
        //====== NO INTERACTION FOUND ===========
        if (!Interaction_TryDetectAccurateInteract(out IPlayerInteractable interactableFound))
        {
            if (_currInteract != null)
            {
                _currInteract?.LeaveDetection();
                _currInteract = null;
            }
            return;
        }

        //============ INTERACTION FOUND =============
        if (_currInteract != interactableFound)
        {
            _currInteract?.LeaveDetection();
            _currInteract = interactableFound;
        }
        _currInteract.EnterDetection();

    }


    #endregion

    //Handle whatever gets called in the PICKEDUPITEM state
    void Interaction_FixedUpdate_PICKEDUPITEM()
    {
        //Do i want to still detect whether there are interactable stuff in the player's range?

        //Maybe so for loading stuff into the canon?

        //or maybe we can use toss to load items into the cannon (toss will make a bool on the object as true then the station ontrigger can catch that)

    }

    void Interaction_FixedUpdate_INSTATION()
    {
        _currInteract.FixedUpdateInteract();
    }

    #region Detecting Interacts
    ///<Summary>Returns true if there is a most accurate interaction found</Summary>
    bool Interaction_TryDetectAccurateInteract(out IPlayerInteractable mostAccurateInteract)
    {
        // mostAccurateInteract = null;
        // bool interactsDetected = Physics.OverlapSphereNonAlloc(DetectionSpherePosition, StatsInfo.CheckRaidus, _detectedInteracts, Constants.For_Layer_and_Tags.LAYERMASK_INTERACTABLE_FINALMASK) > 0;
        // //No colliders found
        // if (!interactsDetected)
        // {
        //     return false;
        // }

        Interaction_ClearRemovedInteracts();
        mostAccurateInteract = Interaction_GetMostAccurateInteractable();
        // Debug.Log($"Best potential detected: {mostAccurateInteract}");

        //No most accurate interact which have Interactable bool set to false found
        return mostAccurateInteract != null;
    }

    private void Interaction_ClearRemovedInteracts()
    {
        foreach (var c in _removeInteractsList)
        {
            _detectedInteractsHashset.Remove(c);
        }

        _removeInteractsList.Clear();
    }

    ///<Summary>
    ///Returns the transform nearest the player's transform.forward in terms of angle in the collider[] cache
    ///</Summary>
    IPlayerInteractable Interaction_GetMostAccurateInteractable()
    {
        IPlayerInteractable closestInteractable = null;
        float closestDot = 0f;
        foreach (var c in _detectedInteractsHashset)
        {
            //Skip players
            if (PlayerManager.IsPlayer(c))
            {
                continue;
            }

            //Because grow collider isenabled and disabled, ontriggerexit doesnt catch the collider when it is disabled hence we need to catch this ourselves
            if (!c.enabled || !c.gameObject.activeInHierarchy)
            {
                _removeInteractsList.Add(c);
                continue;
            }

            // Debug.Log($"Worth your time! {c.name}", c);

            Transform t = c.transform;
            Vector3 dir = (t.position - transform.position).normalized;
            //must set y to 0 in order to properly compare the vectors in the dot product (assumign that there is not much verticality in the ship)
            dir.y = transform.forward.y;
            float dot = Vector3.Dot(dir, transform.forward);


            // Debug.Log($"Comparing {c} with the dot of {dot} versus the closestdot of {closestDot}");
            //if interactable is within the dot range or if the dot is within the range, check if it is less than the curr closest dot
            if (dot <= StatsInfo.MinDetectDot || dot < closestDot)
            {
                continue;
            }

            //Do a last check to see if this interactable is actually interactable
            //colliders that makes it here will be:
            //PlayerPickables :(Cargo, Bomb, Enemy Projectile)
            //Stations

            //             IPlayerInteractable check;
            //             if (BoatManager.IsStation(c))
            //             {
            //                 check = c.GetComponent<IPlayerInteractable>();
            //             }
            //             else
            //             {
            // #if UNITY_EDITOR
            //                 Debug.Log($"Is Attachedrigidbody there on {c.name} of collider type {c}? {c.attachedRigidbody != null}", c);
            // #endif
            //                
            //                 check = c.attachedRigidbody?.GetComponent<IPlayerInteractable>();
            //             }

            //Bug where attachedrigidbody returns null when the collider's root gameobject is set active to false, 
            //thus ? is needed and i shifted check == null to be noneditor
            IPlayerInteractable check = BoatManager.IsStation(c) ? c.GetComponent<IPlayerInteractable>() : c.attachedRigidbody?.GetComponent<IPlayerInteractable>();
            if (check == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"The collider {c} does not have an IPlayerInteractable-inherited component on it. Is this correct?", c);
#endif
                continue;
            }


            if (!check.IsPlayerInteractable)
            {
                continue;
            }


            //Congraz! u are now the most accurate interactable
            closestDot = dot;
            closestInteractable = check;
        }

        return closestInteractable;
    }

    // void Interaction_ClearDetectedInteractablesArray()
    // {
    //     for (int i = 0; i < _detectedInteracts.Length; i++)
    //     {
    //         if (_detectedInteracts[i] == null) break;

    //         _detectedInteracts[i] = null;
    //     }
    // }

    #endregion
}
