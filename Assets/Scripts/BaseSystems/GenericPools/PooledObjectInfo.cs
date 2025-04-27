using UnityEngine;

[CreateAssetMenu(fileName = nameof(PooledObjectInfo), menuName = BaseGenericPool.ASSETMENU_SETTINGS + "/" + nameof(PooledObjectInfo))]
public class PooledObjectInfo : BetterScriptableObject
{
    [Header("===== PooledObjectInfo =====")]
    public GameObject Prefab = default;

    [Range(0, 1000)]
    public int Count = 5;
}