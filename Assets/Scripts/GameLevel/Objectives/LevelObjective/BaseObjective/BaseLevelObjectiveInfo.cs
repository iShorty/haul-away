
using UnityEngine;

[CreateAssetMenu(fileName = nameof(BaseLevelObjectiveInfo), menuName = Constants.ASSETMENU_CATEGORY_LEVELOBJECTIVE + "/" + nameof(BaseLevelObjectiveInfo))]
///<Summary>Holds information about how to earn a star </Summary>
public class BaseLevelObjectiveInfo : ScriptableObject
{
    // [TextArea(1, 10)]
    // ///<Summary>The string value which will appear on the  level select panel</Summary>
    // [Tooltip("The string value which will appear on the top right corner of the screen for the player ingame ui")]
    // public string ObjectiveDescription = "ObjectiveDescription";

    [TextArea(1, 10)]
    ///<Summary>The string value which will appear on the level select panel. Format it using curly brackets and a number inside of it. This represents a text format parameter. </Summary>
    [Tooltip("The string value which will appear on the level select panel. Format it using curly brackets and a number inside of it. This represents a text format parameter. ")]
    public string LevelSelectTextFormat = "Deliver to all destinations in the level";

    ///<Summary>Returns the formatted text version of the objective (if there is any format) </Summary>
    public virtual string GetLevelSelectText() { return LevelSelectTextFormat; }

    [TextArea(1, 10)]
    ///<Summary>The string value which will appear on the top left corner of the ingame ui and gameover screen. Format it using curly brackets and a number inside of it. This represents a text format parameter. </Summary>
    [Tooltip("The string value which will appear on the level select panel. Format it using curly brackets and a number inside of it. This represents a text format parameter. ")]
    public string InGameTextFormat = "Deliver to {1}/{0} destinations in the level";

    // public virtual string GetInGameText() { return InGameTextFormat; }

    ///<Summary>Instantiate this to initialize this objective info. This prefab ought to be handling the subscription of events in order to check if condition is completed</Summary>
    public GameObject SubscriberPrefab = default;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (SubscriberPrefab == null) return;

        Debug.Assert(SubscriberPrefab.GetComponent<BaseObjectiveEventHook>() != null, $"The subscriber prefab {SubscriberPrefab.name} must have {nameof(BaseObjectiveEventHook)} attached to it!", SubscriberPrefab);
    }
#endif

}