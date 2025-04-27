using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class PlayerManager
{

    #region Get Methods
    public static PlayerController GetPlayer(int playerIndex) { return instance._players[playerIndex]; }

    public static bool IsPlayer(Collider c)
    {
        if (c.attachedRigidbody == null) return false;
        // Debug.Log(c.attachedRigidbody.name,c.attachedRigidbody);
        return c.attachedRigidbody.CompareTag(Constants.For_Layer_and_Tags.TAG_PLAYER);
    }
    #endregion

    public static void StartRespawnPlayer(int index)
    {
        instance._StartRespawnPlayer(index);
    }

    public static void EndRespawnPlayer(int index)
    {
        instance._EndRespawnPlayer(index);
    }


    #region ---------- Events --------------
    public static event Action<Transform> OnPlayerRespawn = null;
    static void RaiseOnPlayerRespawn(Transform playerTransform)
    {
        OnPlayerRespawn?.Invoke(playerTransform);
    }

    #endregion


}
