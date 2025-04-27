using UnityEngine;

[CreateAssetMenu(fileName = nameof(PlayerModelInfo), menuName = Constants.ASSETMENU_CATEGORY_UI + "/" + nameof(PlayerModelInfo))]
///<Summary>Holds all of the prefab models for the different types of player characters</Summary>
public class PlayerModelInfo : ScriptableObject
{
    public GameObject[] PlayerModels = new GameObject[0];
}