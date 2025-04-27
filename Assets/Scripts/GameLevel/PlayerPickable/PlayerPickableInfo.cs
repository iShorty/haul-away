using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerPickableInfo", menuName = Constants.ASSETMENU_CATEGORY_PLAYERPICKABLE + "/PlayerPickableInfo")]
public class PlayerPickableInfo : ScriptableObject
{
    [field: Header("===== PLAYER PICKABLE INFO =====")]
    [field: SerializeField, RenameField(nameof(BoundingBox))]
    [field: Tooltip("Imagine putting whatever collider the pickable object has into a the most minimumly lengthed box you could think of. That box is the BoundingBox")]
    public Vector3 BoundingBox { get; protected set; } = default;

    [field: SerializeField, RenameField(nameof(Prefab)), Min(0)]
    public GameObject Prefab { get; protected set; } = default;


}
