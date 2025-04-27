///<Summary>Each enum value represents an audio clip which ought to be placed in a Resources folder and named as the enum's value name. The enums cannot have a value lesser than zero and the value difference between consecutive enum names can only be one</Summary>
public enum AudioClipType
{
    #region ============= BGM ================

    #region  ----------- Gameplay Music -------------
    BGM_Tutorial_1
    ,
    BGM_Tutorial_2
    ,
    BGM_Tutorial_3
    ,
    BGM_CombatMusic
    ,
    #endregion
    BGM_Menu
    ,
    BGM_LoadingScreen
    ,
    BGM_LevelComplete
    ,

    #endregion
    #region ============ SFX =================
    #region ------------- Player & Boat --------------
    SFX_Footstep
   ,
    ///<Summary>Called when fuel has been collected by player</Summary>
    SFX_FuelSpawn
   ,
    ///<Summary>Called when fuel has fully regenerated</Summary>
    SFX_FuelIncrease
   ,
    SFX_ItemPickUp
   ,
    SFX_BoatImpact
   ,
    ///<Summary>Called when player toggles the lever</Summary>
    SFX_SpeedLever
   ,
    SFX_CannotDropItem
,
    SFX_ItemDropped
,
    SFX_PlayerSpawnPoof
,
    SFX_Toss
   ,
    #endregion
    #region -------------- Firing ---------------
    SFX_GrappleGunMode
   ,
    SFX_CannonGunMode
   ,
    SFX_CannonFire
   ,
    #endregion
    #region ------------- Sea SFX ---------------
    SFX_Sea1
   ,
    SFX_Sea2
    ,
    SFX_Sea3
     ,
    SFX_Sea4
     ,
    SFX_Sea5
    ,
    SFX_WaterSplash
,
    #endregion
    #region ----------- Explosions ----------
    SFX_SeaMineExplosion
    ,
    SFX_BombExplosion
    ,
    #endregion
    #region ------------- Impacts ----------
    SFX_WhaleImpact
    ,
    #endregion
    #region ------------ Level/UI -----------------
    SFX_TimeUp
    ,
    SFX_ThirtySecondsLeft
    ,
    SFX_ObjectiveCompleted
    ,
    SFX_Pause
    ,
    SFX_ButtonClick
    ,
    SFX_ButtonHighlight
    ,
    SFX_LevelSelected
    ,
    #endregion
    #endregion
}