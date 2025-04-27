using UnityEngine;

///<Summary>Handles updating the player's cargo score ui and time ui </Summary>
//This part of the script handles in game ui
public partial class GameUI : LevelSingleton<GameUI>
{

    #region Lifetime
    protected override void GameAwake()
    {
        InGame_Awake();
        NotInGame_Awake();
    }
    private void OnEnable()
    {
        GlobalEvents.OnGameReset += HandleOnGameReset;
        GlobalEvents.OnGameStart += HandleOnGameStart;
        InGame_OnEnable();
        NotInGame_OnEnable();
    }

    private void OnDisable()
    {
        GlobalEvents.OnGameReset -= HandleOnGameReset;
        GlobalEvents.OnGameStart -= HandleOnGameStart;
        InGame_OnDisable();
        NotInGame_OnDisable();
    }

    #endregion

    #region Handlers

    #region One-Frame Events
    private void HandleOnGameReset()
    {
        InGame_HandleOnGameReset();
        NotInGame_HandleOnGameReset();
    }

    private void HandleOnGameStart()
    {
        InGame_HandleGameStart();
    }




    #endregion


    #endregion



}
