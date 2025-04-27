using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(OutlineInfo), menuName = Constants.ASSETMENU_CATEGORY_UIShaderEffects + "/" + nameof(OutlineInfo))]
public class OutlineInfo : ScriptableObject
{
    [Range(-1, 1)]
    public float Thickness = 0.01f;
    public Color OutlineColor = Color.white;
    public bool PrecalculateNormals = true;
}