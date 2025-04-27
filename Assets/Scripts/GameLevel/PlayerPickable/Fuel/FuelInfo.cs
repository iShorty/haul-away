using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FuelInfo", menuName = Constants.ASSETMENU_CATEGORY_PLAYERPICKABLE + "/FuelInfo")]
public class FuelInfo : PlayerPickableInfo
{
    [field:Header("===== FUEL INFO =====")]
    [field: SerializeField, RenameField(nameof(FuelValue))]
    public int FuelValue { get; protected set; }
}
