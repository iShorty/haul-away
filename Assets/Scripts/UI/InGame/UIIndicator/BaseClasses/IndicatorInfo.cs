using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(IndicatorInfo), menuName = Constants.ASSETMENU_CATEGORY_UI + "/" + nameof(IndicatorInfo))]
public class IndicatorInfo : ScriptableObject
{
    [Header("Display Settings - References")]
    public Sprite IndicatorSprite = default;

    public Vector2 ImageDimension = default;

    public GameObject Prefab = default;

}
