using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TutorialColliderTrigger : MonoBehaviour
{
    ///<Summary>Called when player boat enters the detection trigger</Summary>
    public static event Action OnBoatEnterDetectionTrigger = null;

#if UNITY_EDITOR
    private void Awake()
    {
        Collider c = GetComponent<Collider>();
        Debug.Assert(c.isTrigger, $"Collider on {name} does not have its isTrigger set to true!", this);
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        if (BoatManager.IsPartOfBoat(other))
        {
            OnBoatEnterDetectionTrigger?.Invoke();
        }
    }
}
