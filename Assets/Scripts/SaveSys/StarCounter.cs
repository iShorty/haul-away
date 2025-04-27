using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarCounter : MonoBehaviour
{
    [SerializeField]
    Text counter;
    int starCount;
    int tutorialCount, level1Count, level2Count;

    public GameObject tut, map1, map2;

    public void Load()
    {
        var mapdat = Game._Game._GameData;
        starCount = Mathf.Clamp(mapdat.TotalStarCount, 0, 64);
        MapCounter();
    }

    public void MapCounter()
    {
        counter.text = " " + starCount.ToString() + "/64";
    }
}
