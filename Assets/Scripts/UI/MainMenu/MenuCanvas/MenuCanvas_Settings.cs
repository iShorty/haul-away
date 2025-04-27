using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;
using AudioManagement;
public partial class MenuCanvas
{

    const string BLOCKNAME_SETTINGS_SHOWUI = "Settings_Show_UI_Elements";
    const string BLOCKNAME_SETTINGS_HIDEUI = "Settings_Hide_UI_Elements";

    [Header("===== Settings =====")]
    [SerializeField]
    Slider _masterVolume = default;
    [SerializeField]
    Slider _sfxVolume = default;
    [SerializeField]
    Slider _bgmVolume = default;
    [SerializeField]
    Button _resetProgressButton = default;

    public static event Action OnResetGameProgress = null;
    public static void RaiseOnResetGameProgress()
    {
        OnResetGameProgress?.Invoke();

    }

    void Settings_GameAwake()
    {
        ToggleButton_Navigation(false, _masterVolume, _sfxVolume, _bgmVolume, _resetProgressButton);
        _resetProgressButton.onClick.RemoveAllListeners();
        _resetProgressButton.onClick.AddListener(Settings_OnClick_ResetProgress);
    }

    void Settings_Start()
    {
        //Set slider values
        Settings_LoadSliderValue(_masterVolume, GameDataFile.MasterVol);
        Settings_LoadSliderValue(_sfxVolume, GameDataFile.SFXVol);
        Settings_LoadSliderValue(_bgmVolume, GameDataFile.BGMVol);
    }

    private void Settings_OnClick_ResetProgress()
    {
        float masterVol = GameDataFile.MasterVol, bgmVol = GameDataFile.BGMVol, sfxVol = GameDataFile.SFXVol;
        SaveSystem.DeleteAllSaveFiles();
        GameDataFile = SaveSystem.LoadGame();
        GameDataFile.BGMVol = bgmVol;
        GameDataFile.SFXVol = sfxVol;
        GameDataFile.MasterVol = masterVol;
        SaveSystem.SaveGame(GameDataFile);
        MenuState_SetState(MenuState.MENU);
        RaiseOnResetGameProgress();
    }

    ///<Summary>Loads in the slider value from the gamedata and change the audiomixer parameter</Summary>
    void Settings_LoadSliderValue(Slider slider, float value)
    {
        //Set slider values
        slider.value = value;
        slider.onValueChanged?.Invoke(value);
    }

    private void Settings_EnterState(MenuState prevState)
    {
        //Settings panel can only be accessed via menu
        if (prevState != MenuState.MENU) return;

        //Play block to lerp in the settings
        _flowchart.PlayBlock(BLOCKNAME_SETTINGS_SHOWUI);
        ToggleButton_Navigation(true, _masterVolume, _sfxVolume, _bgmVolume, _resetProgressButton);

        _masterControls.MainMenu.Cancel.performed += Settings_OnCancel;
        _masterVolume.Select();
    }

    private void Settings_OnCancel(CallbackContext obj)
    {
        if (TransitionManager.IsTransitionPlaying(TransitionManager.TransitionTrigger.FADE_0_TO_1_TO_0)) return;

        MenuState_SetState(MenuState.MENU);
        _masterControls.MainMenu.Cancel.performed -= Settings_OnCancel;
    }

    private void Settings_LeaveState()
    {
        AudioChannelManager.SaveVolumeData(GameDataFile);
        SaveSystem.SaveGame(GameDataFile);
        ToggleButton_Navigation(false, _masterVolume, _sfxVolume, _bgmVolume, _resetProgressButton);
        //Play block to lerp out the settings
        _flowchart.PlayBlock(BLOCKNAME_SETTINGS_HIDEUI);
    }
}
