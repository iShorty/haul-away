using UnityEngine;

[CreateAssetMenu(fileName = nameof(PoolerInfo), menuName = BaseGenericPool.ASSETMENU_SETTINGS + "/" + nameof(PoolerInfo))]
public class PoolerInfo : BetterScriptableObject
{
    [Header("===== PoolerInfo =====")]
    public PooledObjectInfo[] PooledObjectInfos = default;
}