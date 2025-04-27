using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;


public partial class MenuCanvas
{
    #region COnst
    const string BLOCKNAME_CREDITS_SHOW_UIELEMENTS = "Credits_Show_UIElements";
    const string BLOCKNAME_CREDITS_HIDE_UIELEMENTS = "Credits_Hide_UIElements";
    #endregion

    void Credits_GameAwake()
    {

    }



    private void Credits_EnterState(MenuState state)
    {
        //Credits can only be accessed via menu for now
        if (state != MenuState.MENU) return;

        _flowchart.PlayBlock(BLOCKNAME_MENU_HIDE_UIELEMENTS);

        //Play block to slide the credits up
        TransitionManager.SubscribeToOnFade_1_To_0_end(Credits_TransitionToCredits);
        TransitionManager.PlayGlobalFlowChart(TransitionManager.TransitionTrigger.FADE_0_TO_1_TO_0,true);

    }

    #region --------------------- Enter State Method -------------------------
    private void Credits_TransitionToCredits()
    {
        _flowchart.PlayBlock(BLOCKNAME_CREDITS_SHOW_UIELEMENTS);
        _masterControls.MainMenu.Cancel.performed += Credits_OnCancel;
        TransitionManager.UnSubscribeToOnFade_1_To_0_end(Credits_TransitionToCredits);
    }
    private void Credits_OnCancel(CallbackContext obj)
    {
        if (TransitionManager.IsTransitionPlaying(TransitionManager.TransitionTrigger.FADE_0_TO_1_TO_0)) return;

        MenuState_SetState(MenuState.MENU);
        _masterControls.MainMenu.Cancel.performed -= Credits_OnCancel;
    }

    #endregion


    private void Credits_LeaveState()
    {
        TransitionManager.SubscribeToOnFade_1_To_0_end(Credits_TransitionToMenu);
        TransitionManager.PlayGlobalFlowChart(TransitionManager.TransitionTrigger.FADE_0_TO_1_TO_0,true);
    }

    private void Credits_TransitionToMenu()
    {
        _flowchart.PlayBlock(BLOCKNAME_CREDITS_HIDE_UIELEMENTS);
        _flowchart.PlayBlock(BLOCKNAME_MENU_SHOW_UIELEMENTS);
        TransitionManager.UnSubscribeToOnFade_1_To_0_end(Credits_TransitionToMenu);
    }


}
