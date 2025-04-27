using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LinearEffects;
using LinearEffects.DefaultEffects;

public partial class TransitionManager
{
    public enum TransitionTrigger
    {
        FADE_1_TO_0
        ,
        FADE_0_TO_1_TO_0
        ,
        FADE_0_TO_1
    }


    UnityEvent_Executor.MyEffect _unityEventEffect;

    void FlowChart_GameAwake()
    {
        _unityEventEffect = GlobalFlowChart.GetEffect<UnityEvent_Executor, UnityEvent_Executor.MyEffect>(TransitionTrigger.FADE_0_TO_1_TO_0.ToString(), 2);
    }


    public static void PlayGlobalFlowChart(TransitionTrigger trigger, bool forcePlay)
    {

        string triggerName = trigger.ToString();
        if (forcePlay)
        {
            GlobalFlowChart.TryStopBlock(triggerName);
        }
        GlobalFlowChart.PlayBlock(triggerName);
    }

    public static void SubscribeToOnFade_1_To_0_end(UnityAction action)
    {
        instance._unityEventEffect.EffectEvent.AddListener(action);
    }

    public static void UnSubscribeToOnFade_1_To_0_end(UnityAction action)
    {
        instance._unityEventEffect.EffectEvent.RemoveListener(action);
    }

    public static bool IsTransitionPlaying(TransitionTrigger trigger)
    {
        return instance._flowChart.CheckBlockIsPlaying(trigger.ToString());
    }


}
