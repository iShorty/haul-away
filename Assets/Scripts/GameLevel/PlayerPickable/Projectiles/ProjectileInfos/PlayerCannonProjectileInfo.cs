using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New " + nameof(PlayerCannonProjectileInfo), menuName = Constants.ASSETMENU_CATEGORY_PLAYERPICKABLE + "/" + nameof(PlayerCannonProjectileInfo))]
public class PlayerCannonProjectileInfo : ProjectileInfo
{
    [field: Header("===== PLAYERCANNON INFO =====")]
    [field: SerializeField, RenameField(nameof(Damage))]
    public int Damage { get; protected set; } = 1;


    [Range(0, 1000)]
    ///<Summary>The duration in which a player cannonball will exist from the moment the player cannon station fires and then returning to the Projectile Pool once the duration is up</Summary>
    public float LifeTime = 30f;

    public VFXInfo enemyHitVFXInfo = default;

}
