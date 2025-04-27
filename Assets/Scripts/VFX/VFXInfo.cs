using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New VFX Object Info", menuName = Constants.ASSETMENU_CATEGORY_VFXINFO)]
public class VFXInfo : ScriptableObject
{
    [field:Header("===== VFX OBJECT INFO =====")]
    [field: SerializeField, RenameField(nameof(Prefab)), Min(0)]
    public GameObject Prefab { get; protected set; } = default;

    [field: SerializeField, RenameField(nameof(Duration))]
    public float Duration { get; protected set; }
}
