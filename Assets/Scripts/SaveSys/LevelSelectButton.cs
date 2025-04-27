using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField]
    GameObject[] stars;
    [SerializeField]
    Button button;
    [SerializeField]
    Image buttonImage;
    [SerializeField]
    int levelAccessValue;
    string levelID => transform.GetChild(0).name;
    [SerializeField]
    Sprite[] backingSprites;

    public void Load()
    {
        var mapdat = Game._Game._GameData;
        // If access is > curr level num, i can use this button to play this level
        if (mapdat.TotalStarCount >= levelAccessValue)
        {
            button.interactable = true;
        }
        else
            button.interactable = false;
        // for all saved level data 
        for (int i = 0; i < mapdat.levelDats.Count; i++)
        {
            // Find matching level data if any
            if (levelID == mapdat.levelDats[i].levelID)
            {
                buttonImage.sprite = backingSprites[1];
                
                // For all the stars cached, iterate through them and activate the correct star sprites
                for (int j = 0; j < mapdat.levelDats[i].stars.Length; j++)
                {
                    // If true, activate/set the right star sprite
                    if(mapdat.levelDats[i].stars[j])
                        stars[j].SetActive(true);
                    // False, no star sprite/grey star sprite
                    else
                        Debug.Log("no star for u");
                }
            }
        }
    }
}
