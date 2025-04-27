using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Destination
{
    const float PREFERRED_MULTIPLIER = 2.5f;

    public static bool IsDestination(Collider c)
    {
        if (c.attachedRigidbody == null) return false;

        return c.attachedRigidbody.CompareTag(Constants.For_Layer_and_Tags.TAG_DESTINATION);
    }

    public static event Action OnCollectPreferredCargo = null;

    public static event Action<Destination> OnDeliverCargoToDestination = null;

  
}
