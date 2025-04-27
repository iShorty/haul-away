using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New " + nameof(ProjectileInfo), menuName = Constants.ASSETMENU_CATEGORY_PLAYERPICKABLE + "/" + nameof(ProjectileInfo))]
public class ProjectileInfo : PlayerPickableInfo
{
    [field: Header("===== PROJECTILE INFO =====")]
    // [field: SerializeField, RenameField(nameof(Delay))]
    // public float Delay { get; protected set; } = default;

    ///<Summary>The amount of force applied to the player when projectile interacts with the IBombable entity</Summary>
    [field: SerializeField, RenameField(nameof(Force))]
    public float Force { get; protected set; } = default;

    [field: SerializeField, RenameField(nameof(Radius))]
    public float Radius { get; protected set; } = default;


}
