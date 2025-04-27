
using UnityEngine;

[CreateAssetMenu(fileName = nameof(FloaterSinkInfo), menuName = Constants.ASSETMENU_CATEGORY_WATERPHYSICS + "/" + nameof(FloaterSinkInfo))]
public class FloaterSinkInfo : ScriptableObject
{
    [Header("---- Sinking Values -----")]
    [Min(0)]
    public Vector2 SinkSpeedRange = default;
    [Range(0, 100)]
    public float FloatDuration = default;
    [Range(0, 100)]
    public float SinkDuration = default;

}