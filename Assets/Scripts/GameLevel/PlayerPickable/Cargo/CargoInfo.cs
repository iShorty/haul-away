using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CargoInfo", menuName = Constants.ASSETMENU_CATEGORY_PLAYERPICKABLE + "/CargoInfo")]
public class CargoInfo : PlayerPickableInfo
{
    [field: Header("===== CARGO INFO =====")]
    [field: SerializeField, RenameField(nameof(Value)), Min(0)]
    public int Value { get; protected set; } = default;

    // [field: SerializeField, RenameField(nameof(CargoColors)), Min(0)]
    // ///<Summary>The colors for the cargo when they are being detected/not detected by the grappling hook</Summary>
    // public CargoSilhouetteColors CargoColors { get; protected set; } = default;

    [field: Header("----- UI Indicator -----"), SerializeField, RenameField(nameof(SinkIndicatorInfo))]
    public IndicatorInfo SinkIndicatorInfo { get; protected set; } = null;

    public Sprite CargoSprite = default;

}
