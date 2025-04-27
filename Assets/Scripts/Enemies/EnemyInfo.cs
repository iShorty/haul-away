using UnityEngine;
using System.Collections.Generic;

public abstract class EnemyInfo : ScriptableObject
{
    [field: Header("===== ENEMY INFO =====")]
    [field: SerializeField, RenameField(nameof(Prefab)), Min(0)]
    public GameObject Prefab { get; protected set; } = default;

    [Header("Base stats")]
    [Tooltip("Delay after death where the GameObject is destroyed (to allow for animation)")]
    public float deathDuration = 0f;


    [Tooltip("Number of items it can drop on death")]
    [Range(0, 10)] 
    public int dropItemNum = 0;

    [Tooltip("Items it can drop on death")]
    // public List<BaseCargo> droppableItems = new List<BaseCargo>();
    public List<CargoInfo> DroppableCargo = new List<CargoInfo>();

}