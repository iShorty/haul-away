using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = nameof(LevelInfo), menuName = Constants.ASSETMENU_CATEGORY_LEVELINFO + "/" + nameof(LevelInfo))]
public partial class LevelInfo : ScriptableObject
{
    public string SceneName = "SceneName";

    [Header("===== Level Information =====")]
    [TextArea(0, 10)]
    public string LevelName = "Level Name";
    public Sprite LevelScreenShot = default;

    [Range(0, 100)]
    public int StarUnlock = 0;

    [Header("----- Audio -----")]
    public AudioClipType BGM = default;


    [Header("===== Timer Values =====")]
    [Min(0)]
    public float LevelDuration = 120;

    [Header("===== Objectives =====")]
    [SerializeField]
    [Min(0)]
    public BaseLevelObjectiveInfo[] ObjectiveConditionInfos = new BaseLevelObjectiveInfo[0];

}
