using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New " + nameof(BombProjectileInfo), menuName = Constants.ASSETMENU_CATEGORY_PLAYERPICKABLE + "/" + nameof(BombProjectileInfo))]
public class BombProjectileInfo : ProjectileInfo
{
    [field: Header("===== BOMB INFO =====")]
    [field: SerializeField, RenameField(nameof(Delay))]
    public float Delay { get; protected set; } = default;

    [Range(0, 1000f)]
    public float SpinMin = 1f, SpinMax = 10f;
    public GameObject ExplosionPrefab;

    [Range(1, 100)]
    public float FlashInterval = 1f;
    ///<Summary>When the bomb's current interval timer has reached 0, the next timer will be the current interval timer's duration minus this value </Summary>
    [Range(0, 1)]
    public float IntervalMultiplier = 0.5f;
}
