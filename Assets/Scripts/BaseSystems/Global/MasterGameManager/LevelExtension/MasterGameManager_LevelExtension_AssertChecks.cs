#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MasterGameManager
{
    private void Start()
    {
        //After everything has been initialized, check for stuff you wanna check here
        Debug.Assert(WaterManager.instance != null, $"{nameof(WaterManager)} is not present in the scene! Please add it to the ocean object", this);
        Debug.Assert(PlayerManager.SceneObject != null, $"{nameof(PlayerManager)} is not present in the scene! Please add!", this);
    }
}

#endif