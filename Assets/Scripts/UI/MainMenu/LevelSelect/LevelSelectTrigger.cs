using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
///<Summary>The physical object in the level select environment which will toggle a MenuLevelUIIndicator on and off to show the information of the level whenever the player low poly boat gets close enough</Summary>
public class LevelSelectTrigger : MonoBehaviour
{
    [field: SerializeField, RenameField(nameof(LevelIndicator))]
    public LevelSelectUIIndicator LevelIndicator { get; private set; } = default;


    private void Start()
    {
        //Set the level panel's screen position
        LevelIndicator.Initialize(MenuCanvas.MenuCamera, LevelIndicator.followingTarget);
#if UNITY_EDITOR
        Collider c = GetComponent<Collider>();
        Debug.Assert(c != null, $"This does not have a collider on it! {name}", this);
        Debug.Assert(c.isTrigger, $"This does not have a collider with istriggered on! {name}", this);
        Debug.Assert(CompareTag(Constants.For_Layer_and_Tags.TAG_DESTINATION), $"This does not have the {Constants.For_Layer_and_Tags.TAG_DESTINATION} tag!", this);
#endif
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        GlobalEvents.OnGameUpdate_DURINGGAME += HandleDebugUpdate;
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        GlobalEvents.OnGameUpdate_DURINGGAME -= HandleDebugUpdate;
#endif
    }

#if UNITY_EDITOR
    private void HandleDebugUpdate()
    {
        LevelIndicator.GameUpdate();
    }
#endif


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(Constants.For_Layer_and_Tags.TAG_PLAYER)) return;
        LevelIndicator.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(Constants.For_Layer_and_Tags.TAG_PLAYER)) return;

        LevelIndicator.gameObject.SetActive(false);
    }

}
