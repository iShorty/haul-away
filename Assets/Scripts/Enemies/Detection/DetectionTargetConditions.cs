using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Detection Target Conditions", menuName = Constants.ASSETMENU_CATEGORY_DETECTIONTARGETCONDITIONS + "/DetectionTargetConditions")]
public class DetectionTargetConditions : ScriptableObject
{
    public LayerMask layerMask;
    [TagSelector] public string[] tags;
    public GameObject[] objects;
    // public int[] ranges;

    // [TagSelector] public string da;
    // Specify classes/interfaces?
}
