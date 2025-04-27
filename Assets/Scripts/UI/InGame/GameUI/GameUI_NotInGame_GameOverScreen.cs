using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class GameUI
{
    [Header("===== Game Over Screen =====")]
    [Header("===== NOT IN GAME UI =====")]
    [SerializeField]
    GameObject _gameOverScreen = default;

    [SerializeField]
    [Header("----- Star UI -----")]
    Transform _starHolder = default;
    [SerializeField]
    private Image _starPrefab = default;


    [SerializeField]
    Sprite _activeStar = default
    , _inactiveStar = default
    ;


    #region Hidden Field
    Image[] _starImages = default;
    #endregion


    [SerializeField]
    Button _gameoverRestartButton = default;
    [SerializeField]
    Button _gameOverContinueButton = default;

    void NotInGame_GameOverScreen_GameAwake()
    {
        NotInGame_GameOverScreen_SpawnStars();

        _gameoverRestartButton.onClick.RemoveAllListeners();
        _gameoverRestartButton.onClick.AddListener(Onclick_RestartButton);

        _gameOverContinueButton.onClick.RemoveAllListeners();
        _gameOverContinueButton.onClick.AddListener(Onclick_ContinueButton);
    }

    void NotInGame_GameOverScreen_GameReset()
    {
        _gameOverScreen.SetActive(false);

    }

    void NotInGame_GameOverScreen_GameEnd()
    {
        LevelInfo currLvlInfo = MasterGameManager.CurrentLevelInfo;
        int finalScore = MasterGameManager.CurrentCargoScore;

        //Get final score and check it against lvl info's star per score
        for (int i = 0; i < currLvlInfo.ObjectiveConditionInfos.Length; i++)
        {
            Image starImg = _starImages[i];
            //Reset the colour first incase you didnt hit it
            starImg.sprite = _inactiveStar;
            TextMeshProUGUI inGameObjectiveText = InGameUI_GetObjectiveUI(i);
            TextMeshProUGUI starImgObjText = starImg.GetComponentInChildren<TextMeshProUGUI>();

            starImgObjText.text = inGameObjectiveText.text;
            starImgObjText.fontStyle = FontStyles.Normal;

            //Using the ingameui's objective texts' fontstyle, we can determine if an objective is met or not
            if (inGameObjectiveText.fontStyle == FontStyles.Normal)
            {
                //Normal means this objective hasnt been fulfilled yet
                continue;
            }

            //Set the star to active colour
            starImgObjText.fontStyle = FontStyles.Strikethrough;
            starImg.sprite = _activeStar;
        }

        //Set gameover screen to true only when star allocation is done
        _gameOverScreen.SetActive(true);
        _gameOverContinueButton.Select();
    }



    void NotInGame_GameOverScreen_SpawnStars()
    {
        //Instantiate stars inside of star holder according to the number of score checks in the level info array
        LevelInfo currLvlInfo = MasterGameManager.CurrentLevelInfo;


        _starImages = new Image[currLvlInfo.ObjectiveConditionInfos.Length];
        for (int i = 0; i < currLvlInfo.ObjectiveConditionInfos.Length; i++)
        {
            _starImages[i] = Instantiate(_starPrefab);
            _starImages[i].transform.SetParent(_starHolder);
            _starImages[i].transform.localScale = Vector3.one;
            _starImages[i].sprite = _inactiveStar;

            //Asign score value for acheiving that star
            // TextMeshProUGUI tmp = _starImages[i].transform.GetComponentInChildren<TextMeshProUGUI>();
            // tmp.text = currLvlInfo.ObjectiveConditionInfos[i].GetLevelSelectText();
        }
    }


}
