using UnityEngine;

[CreateAssetMenu(fileName = nameof(DeviceSpriteInfo), menuName = Constants.ASSETMENU_CATEGORY_UI + "/" + nameof(DeviceSpriteInfo))]
///<Summary>Holds device sprites for the main menu's box ui to show what devices are connected to the game</Summary>
public class DeviceSpriteInfo : ScriptableObject
{
    [SerializeField]
    Sprite Controller = default;
    [SerializeField]
    Sprite Keyboard = default;

    //For unrecognised devices
    [SerializeField]
    Sprite Tick = default;

    
    public Sprite Disconnected = default;

    public Sprite GetDeviceSprite(string nameOfDevice)
    {
        switch (nameOfDevice)
        {
            case "Wireless Controller":
                return Controller;

            case "Keyboard":
                return Keyboard;


            default:
                return Tick;
        }
    }
}