using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

///<Summary>Displays in the level select screen the levels and their current progress (number of stars) and required number of stars to unlock</Summary>
public class LevelSelectUIIndicator : ImageIndicator
{

    [Header("----- UI References -----")]
    [SerializeField]
    TextMeshProUGUI _levelNameUI = default;
    [SerializeField]
    Image _lvlScreenshot = default;

    [Header("----- Asset References -----")]
    [SerializeField]
    Sprite _unfilledStar = default;
    [SerializeField]
    Sprite _filledStar = default;

    [SerializeField]
    Image[] _starProgess = new Image[0];

    [field: SerializeField, RenameField(nameof(LevelInfo))]
    ///<Summary>The objectives that will be in the level which players have to fulfill to earn stars</Summary>
    [field: Tooltip("The objectives that will be in the level which players have to fulfill to earn stars")]
    public LevelInfo LevelInfo { get; protected set; } = default;
    // public BaseLevelObjectiveInfo[] ObjectiveInfos { get; protected set; } = new BaseLevelObjectiveInfo[0];


    public override void Initialize(Transform target)
    {
        // base.Initialize(target);
#if UNITY_EDITOR
        int starLength = _starProgess.Length;
        int objLength = LevelInfo.ObjectiveConditionInfos.Length;
        Debug.Assert(starLength > 0, $"This menu level indicator needs its starprogress aray assigned!", this);
        Debug.Assert(objLength > 0, $"This menu level indicator needs its ObjectiveInfo aray assigned!", this);
        // Debug.Assert(starLength == objLength, $"This menu level indicator needs to have equal number of star images and objective infos! assigned!", this);
#endif
        for (int i = 0; i < _starProgess.Length; i++)
        {
            _starProgess[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < LevelInfo.ObjectiveConditionInfos.Length; i++)
        {
            _starProgess[i].gameObject.SetActive(true);
        }

        MenuCanvas.OnResetGameProgress += LoadStarProgress;
        LoadStarProgress();
    }

    ///<Summary>Reads from the savesystem the current star progress for this level then fill in the correct number of star images in the _starProgress array </Summary>
    void LoadStarProgress()
    {
        LevelData data = MenuCanvas.GameDataFile.levelDats.Find(levelData => levelData.levelID == LevelInfo.SceneName);
        if (ReferenceEquals(data,null))
        {
            data = AddLevelIntoGameData();
        }

        bool[] readArray = data.stars;
        for (int i = 0; i < readArray.Length; i++)
        {
            var objectiveMet = readArray[i];
            //fill in star ui with the correct one
            _starProgess[i].sprite = objectiveMet ? _filledStar : _unfilledStar;
        }

        _lvlScreenshot.sprite = LevelInfo.LevelScreenShot;
        _levelNameUI.text = LevelInfo.LevelName;
    }

    protected override void ExitUpdateLoop() { }

    LevelData AddLevelIntoGameData()
    {
        //Get game file from menucanvas
        LevelData ld = new LevelData(new bool[LevelInfo.ObjectiveConditionInfos.Length], LevelInfo.SceneName);
        MenuCanvas.GameDataFile.levelDats.Add(ld);
        return ld;
    }

    public override bool GameUpdate()
    {
        return false;
    }

}
