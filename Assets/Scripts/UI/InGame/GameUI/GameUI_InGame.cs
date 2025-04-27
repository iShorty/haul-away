using System.Text;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using AudioManagement;

public partial class GameUI
{

    [Header("===== IN GAME UI =====")]
    [SerializeField]
    TextMeshProUGUI _timerText = default;
    // [SerializeField]
    // TextMeshProUGUI _cargoScoreText = default;
    [SerializeField]
    Transform _objectiveTextHolder = default;
    // [SerializeField]
    // TextMeshProUGUI[] _objectiveTexts = new TextMeshProUGUI[0];

    #region Hidden
    StringBuilder _timerTextBuilder = default;

    #endregion



    private void InGame_Awake()
    {
#if UNITY_EDITOR
        InGame_DebugAssertValues();
#endif
        _timerTextBuilder = new StringBuilder();
        InGame_HandleOnGameReset();
        for (int i = 0; i < _objectiveTextHolder.childCount; i++)
        {
            TextMeshProUGUI objectiveUI = InGameUI_GetObjectiveUI(i);
            objectiveUI.richText = true;
            objectiveUI.fontMaterial.SetFloat("_UnderlayOffsetX", 1);
            objectiveUI.fontMaterial.SetFloat("_UnderlayOffsetY", -1);
        }
    }



    private void InGame_OnEnable()
    {
        MasterGameManager.OnOneSecondLoop += InGame_HandleOneSecondLoop;
        // MasterGameManager.OnIncrementScore += InGame_HandleIncrementScore;
        BaseObjectiveEventHook.OnStarConditionFulFilled += InGame_HandleOnStarConditionFulFilled;
    }


    private void InGame_OnDisable()
    {
        MasterGameManager.OnOneSecondLoop -= InGame_HandleOneSecondLoop;
        // MasterGameManager.OnIncrementScore -= InGame_HandleIncrementScore;
        BaseObjectiveEventHook.OnStarConditionFulFilled -= InGame_HandleOnStarConditionFulFilled;
    }

    #region -------------- Event Handlers -------------------
    private void InGame_HandleOnStarConditionFulFilled(int i)
    {
        //Strike through the objective text array using the integer
        InGameUI_GetObjectiveUI(i).fontStyle = FontStyles.Strikethrough;

        AudioEvents.RaiseOnPlay2DSFX(AudioClipType.SFX_ObjectiveCompleted, true);
        //Play strike through block animation here or msth
        for (int index = 0; index < MasterGameManager.CurrentLevelInfo.ObjectiveConditionInfos.Length; index++)
        {
            #if UNITY_EDITOR
            Debug.Log($"Star condition fulfilled at {i}",this);
            #endif
            //If there is even one objective not completed, dont even think about ending the game
            if (InGameUI_GetObjectiveUI(index).fontStyle != FontStyles.Strikethrough)
            {
                  #if UNITY_EDITOR
            Debug.Log($"Objective ui fontstyle is not strikethrough!",this);
            #endif
                return;
            }

        }

        MasterGameManager.SendGameEnd();
    }

    private void InGame_HandleOneSecondLoop(float totalGameTimeLeft, int minutes, float seconds)
    {
        //======== UPDATE TIMER UI ============
        _timerTextBuilder.Clear();
        _timerTextBuilder.AppendFormat(Constants.For_InGameUI.LEVELTIMER_STRINGFORMAT, minutes, seconds);
        //Handle like countdown ui or smth here
        _timerText.text = _timerTextBuilder.ToString();
    }


    // private void InGame_HandleIncrementScore(int newScore)
    // {
    //     _cargoScoreText.text = newScore.ToString();
    // }


    private void InGame_HandleOnGameReset()
    {
        _timerText.gameObject.SetActive(false);
        _objectiveTextHolder.gameObject.SetActive(false);
        for (int i = 0; i < _objectiveTextHolder.childCount; i++)
        {
            InGameUI_GetObjectiveUI(i).fontStyle = FontStyles.Normal;
        }
        // _cargoScoreText.gameObject.SetActive(false);
    }

    private void InGame_HandleGameStart()
    {
        _timerText.gameObject.SetActive(true);
        _objectiveTextHolder.gameObject.SetActive(true);
        // _cargoScoreText.gameObject.SetActive(true);
    }
    #endregion

    #region -------------------- Statics ------------------------
    public static void InGameUI_UpdateObjectiveText(int objectiveIndex, string text)
    {
        instance._InGameUI_UpdateObjectiveText(objectiveIndex, text);
    }

    public void _InGameUI_UpdateObjectiveText(int objectiveIndex, string text)
    {
        TextMeshProUGUI textUI = InGameUI_GetObjectiveUI(objectiveIndex);
        textUI.text = text;
    }

    #endregion


    #region -------------------- Helper Methods ------------------------
    TextMeshProUGUI InGameUI_GetObjectiveUI(int index)
    {
        TextMeshProUGUI textUI = _objectiveTextHolder.GetComponentOfChild<TextMeshProUGUI>(index);
        return textUI;
    }


    #endregion



#if UNITY_EDITOR
    #region -------------- Debug Assertions ------------------
    private void InGame_DebugAssertValues()
    {
        Debug.Assert(_objectiveTextHolder, $"{nameof(_objectiveTextHolder)} is not assigned!", this);
        Debug.Assert(_objectiveTextHolder.childCount > 0, $"There is no objective texts assigned to {nameof(GameUI)}!", this);
        Debug.Assert(_timerText, $"{nameof(_timerText)} is not assigned!", this);
    }
    #endregion
#endif

}
