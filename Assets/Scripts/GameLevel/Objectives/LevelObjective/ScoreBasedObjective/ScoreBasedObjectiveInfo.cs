using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ScoreBasedObjectiveInfo), menuName = Constants.ASSETMENU_CATEGORY_LEVELOBJECTIVE + "/" + nameof(ScoreBasedObjectiveInfo))]
public class ScoreBasedObjectiveInfo : BaseLevelObjectiveInfo
{

    [Header("Format Parameter: {0}")]
    [Header("===== ScoreBasedObjectiveInfo =====")]
    [Range(0, 1000)]
    public int ScoreToAchieve = 10;

    public override string GetLevelSelectText()
    {
        return string.Format(LevelSelectTextFormat, ScoreToAchieve);
    }

}
