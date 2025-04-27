using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(BoatCameraInfo), menuName = Constants.ASSETMENU_CATEGORY_PLAYER_BOAT + "/" + nameof(BoatCameraInfo), order = 0)]
public class BoatCameraInfo : ScriptableObject
{
    [Header("----- Position -----")]
    [Range(0, 100)]
    public float PositionSmoothing = 10;

    public float Y_Offset = default;
    public float Z_Offset = default;

    [Header("----- Rotation -----")]
    [Range(-180, 180)]
    public float X_Rotation = default;

    [Range(0, 100)]
    public float RotationSmoothing = 5;


}
