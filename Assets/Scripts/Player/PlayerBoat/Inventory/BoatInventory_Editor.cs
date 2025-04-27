#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BoatInventory : MonoBehaviour
{
    private void Awake()
    {
        Collider c = GetComponent<Collider>();
        Debug.Assert(c!=null, $"The boatinventory class {name} should have a collider attached to it!",this);
        Debug.Assert(c.isTrigger, $"The boatinventory class {name} should have a collider with isTrigger turned on!",this);
        Debug.Assert(gameObject.layer != Constants.For_Layer_and_Tags.LAYERINDEX_PLAYERBOATMODEL, "Boat inventory collider must not be on the playerboat layer!",this);
    }
}


#endif