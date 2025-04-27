using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LinearEffects;
using AudioManagement;
using System;

/// <summary>
/// Handles loading of scene
/// </summary>

public partial class TransitionManager : LevelSingleton<TransitionManager>
{
    #region ---------- Constants --------------
    static WaitForEndOfFrame EndOfFrame = new WaitForEndOfFrame();
    const string BLOCKNAME_OPEN_LOADINGSCREEN = "OpenLoadScreen";
    const string BLOCKNAME_CLOSE_LOADINGSCREEN = "CloseLoadScreen";

    #endregion

    #region ------------- Exposed Fields --------------
    [SerializeField, Range(0, 10)]
    float _minimumLoadingDur = 5f;

    #endregion


    #region --------------- Cached --------------------
    bool m_IsTransitioning = false;
    //This will handle all of the transitions all we need is accompanying blocknames
    BaseFlowChart _flowChart = default;
    ///<Summary>The current level loaded. This field can be null if the current scene is main menu</Summary>
    LevelInfo _currentLevelInfo = default;
    #endregion


    #region ------------ Properties -------------------
    public static BaseFlowChart GlobalFlowChart => instance._flowChart;
    public static string CurrentSceneName { get; private set; }
    #endregion


    protected override void GameAwake()
    {
        CurrentSceneName = SceneManager.GetActiveScene().name;
        _flowChart = GetComponent<BaseFlowChart>();

        FlowChart_GameAwake();

#if UNITY_EDITOR
        EditorAwake();
#endif

        //Check if manager is in coldstart
        if (CurrentSceneName == Constants.For_Scene.SCENENAME_COLDSTART)
        {
            LoadScene(Constants.For_Scene.SCENENAME_MAINMENU);
        }

    }

#if UNITY_EDITOR
    ///<Summary>Handle playing audio here for editor (since there will be cold start in buid)</Summary>
    void EditorAwake()
    {
        switch (CurrentSceneName)
        {
            case Constants.For_Scene.SCENENAME_MAINMENU:
                PlaySceneBGM(CurrentSceneName);
                break;

            case Constants.For_Scene.SCENENAME_COLDSTART:
                PlaySceneBGM(CurrentSceneName);
                break;

            default:
                _currentLevelInfo = MasterGameManager.CurrentLevelInfo;
                PlaySceneBGM(_currentLevelInfo.SceneName);
                break;
        }
        _currentLevelInfo = null;
    }
#endif

    public static void LoadScene(string sceneToLoad)
    {
        //Do everything u need b4 u leave scene
        GlobalEvents.SendExitScene();
        CurrentSceneName = SceneManager.GetActiveScene().name;
        instance.StartCoroutine(instance.LoadSceneCo(sceneToLoad, CurrentSceneName));
    }

    public static void LoadScene(LevelInfo levelToLoad)
    {
        instance._currentLevelInfo = levelToLoad;
        //Do everything u need b4 u leave scene
        GlobalEvents.SendExitScene();
        CurrentSceneName = SceneManager.GetActiveScene().name;
        instance.StartCoroutine(instance.LoadSceneCo(levelToLoad.SceneName, CurrentSceneName));
    }


    IEnumerator LoadSceneCo(string sceneToLoad, string sceneCurrentAt)
    {
        //Play fade animation
        m_IsTransitioning = true;
        _flowChart.PlayBlock(BLOCKNAME_OPEN_LOADINGSCREEN);

        //Fade the BGM into the loading screen bgm
        AudioEvents.RaiseOnPlayBGM(AudioClipType.BGM_LoadingScreen, BGMAudioPlayer.BGM_PlayType.LOOP);


        //Do loading code here
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        operation.allowSceneActivation = false;

        float timer = _minimumLoadingDur;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            //Wait a frame every while loop
            yield return EndOfFrame;
        }

        while (!operation.isDone)
        {
            //Do UI fancy shit here


            //If operation is loaded complete 
            if (operation.progress >= 0.9f)
            {
                //Update the progress ui to 100%

                //Allow scene to load only after unloading previous scene
                operation.allowSceneActivation = true;

            }

            //Wait a frame every while loop
            yield return EndOfFrame;
        }




        m_IsTransitioning = false;
        _flowChart.PlayBlock(BLOCKNAME_CLOSE_LOADINGSCREEN);

        GlobalEvents.SendTransitionScene(sceneCurrentAt, sceneToLoad);
        PlaySceneBGM(sceneToLoad);
        CurrentSceneName = SceneManager.GetActiveScene().name;
        GlobalEvents.SendEnterScene();

        //Must reset after every transition to accomodate menu
        _currentLevelInfo = null;
    }

    private void PlaySceneBGM(string newScene)
    {
        //If currently the scene is an actual game level,
        if (_currentLevelInfo != null)
        {
            AudioEvents.RaiseOnPlayBGM(_currentLevelInfo.BGM, BGMAudioPlayer.BGM_PlayType.LOOP);
            return;
        }

        //Else it means that the scene is in the menu
        AudioEvents.RaiseOnPlayBGM(AudioClipType.BGM_Menu, BGMAudioPlayer.BGM_PlayType.LOOP);

    }
}
