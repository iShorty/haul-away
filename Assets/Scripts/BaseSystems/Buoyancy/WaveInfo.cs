using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WaveInfo", menuName = Constants.ASSETMENU_CATEGORY_WATERPHYSICS + "/WaveInfo")]
public class WaveInfo : ScriptableObject
{
    // Y = A * Sin(x/f + c)
    [field: SerializeField, RenameField(nameof(Amplitude))]
    public float Amplitude { get; private set; } = default;

    //f
    [field: SerializeField, RenameField(nameof(WaveLength))]
    public float WaveLength { get; private set; } = default;

    //Rate of change in offset
    [field: SerializeField, RenameField(nameof(Speed))]
    public float Speed { get; private set; } = default;



}
