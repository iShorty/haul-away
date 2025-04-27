using System.Collections;
using System;
using UnityEngine;
using AudioManagement;

public partial class MasterGameManager
{
    #region Definition
    public delegate void OneSecondLoopCallback(float totalGameTimeLeft, int minutes, float seconds);
    public const int TIMELEFT_BEFORE_TICKING = 30;
    #endregion

    #region Exposed Field
    #region Runtime
#if UNITY_EDITOR
    [SerializeField]
    [ReadOnly]
    [Header("----- Runtime -----")]
    [Header("===== Level Timer =====")]
#endif
    float _timer = default;
    #endregion
    #endregion


    IEnumerator _oneSecondCo = null;


    #region Events
    public static event OneSecondLoopCallback OnOneSecondLoop = null;
    #endregion


    #region Event Method
    //called before 
    void LevelExtension_Timer_OnGameAwake()
    {
        _timer = Constants.For_MasterGameManager.GAMESTART_DELAY;
        GlobalEvents.OnGameUpdate_BEFOREGAMESTART += LevelExtension_Timer_BeforeGameStartUpdate;
    }

    private void LevelExtension_Timer_OnDestroy()
    {
        GlobalEvents.OnGameUpdate_BEFOREGAMESTART -= LevelExtension_Timer_BeforeGameStartUpdate;

    }

    private void LevelExtension_Timer_GameStart()
    {
        //Set your how much time you want the game to last here
        //Comment this out if you dont want a duration
        _timer = Info.LevelDuration;

        _oneSecondCo = LevelExtension_Timer_OneSecondLoopCo();
        StartCoroutine(_oneSecondCo);
    }


    private void LevelExtension_Timer_GameEnd()
    {
        StopCoroutine(_oneSecondCo);
    }

    private void LevelExtension_Timer_GamePause()
    {
        StopCoroutine(_oneSecondCo);
    }

    private void LevelExtension_Timer_GameResume()
    {
        StartCoroutine(_oneSecondCo);
    }

    private void LevelExtension_Timer_GameReset()
    {
        StopCoroutine(_oneSecondCo);
        _timer = Constants.For_MasterGameManager.GAMESTART_DELAY;
    }

    #region Update
    //Cant use coroutine for before gmae starts because we will use start coroutine which will mean that timer will hv to wait at least 1 sec inorder to proceed playtesting
    private void LevelExtension_Timer_BeforeGameStartUpdate()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            return;
        }

        SendGameStart();
    }
    #endregion



    IEnumerator LevelExtension_Timer_OneSecondLoopCo()
    {
        //Run only when timer is more than 0
        while (_timer > 0)
        {
            //======== CALCULATE TIMER VALUES ============
            _timer -= 1f;

            //Round down the value of divided val to get min
            int minutes = Mathf.FloorToInt(_timer / 60);
            //Get the remainder for seconds
            float seconds = _timer - (minutes * 60);


            //============ SEND ONESECONDLOOP EVENT =============
            OnOneSecondLoop?.Invoke(_timer, minutes, seconds);

            if (minutes == 0 && Mathf.RoundToInt(seconds) == TIMELEFT_BEFORE_TICKING)
            {
                AudioEvents.RaiseOnPlay2DSFX(AudioClipType.SFX_ThirtySecondsLeft, true);
            }

            yield return Constants.For_MasterGameManager.UPDATEINTERVAL_ONESECOND;
        }

        //========= GAME HAS ENDED =============
        // AudioManager.theAM.PlaySFX("Time's Up");
        AudioEvents.RaiseOnPlay2DSFX(AudioClipType.SFX_TimeUp, true);
        AudioEvents.RaiseOnPlayBGM(AudioClipType.BGM_LevelComplete, BGMAudioPlayer.BGM_PlayType.LOOP);
        SendGameEnd();
    }
    #endregion

    #region Utility
    public static void SetGameTimer(float timeValue)
    {
        instance._timer = timeValue;
    }

    public static void ToggleGameTimer(bool toggle)
    {
        switch (toggle)
        {
            case true:
                instance.StartCoroutine(instance._oneSecondCo);
                break;

            case false:
                instance.StopCoroutine(instance._oneSecondCo);
                break;
        }
    }

    // public static void EndGame()
    // {
    //     instance._timer = 0;
    // }
    #endregion
}
