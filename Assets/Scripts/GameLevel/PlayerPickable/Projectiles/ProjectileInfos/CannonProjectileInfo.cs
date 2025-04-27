using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New " + nameof(CannonProjectileInfo), menuName = Constants.ASSETMENU_CATEGORY_PLAYERPICKABLE + "/" + nameof(CannonProjectileInfo))]
public class CannonProjectileInfo : ProjectileInfo
{
    [Range(0, 25f)]
    public float PlayerForceScaler = 1f;
    [Range(0, 50f)]
    public float BoatForceScalar = 5f;
}
