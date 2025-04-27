using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PathInfo", menuName = "PathInfo", order = 0)]
public class PathInfo : ScriptableObject
{
    [field: Header("===== Path INFO =====")]
    [field: SerializeField, RenameField(nameof(Prefab)), Min(0)]
    public GameObject Prefab { get; protected set; } = default;

    [Tooltip("Path Nodes")]
    // public List<BaseCargo> droppableItems = new List<BaseCargo>();
    public List<Transform> nodes = new List<Transform>();

}