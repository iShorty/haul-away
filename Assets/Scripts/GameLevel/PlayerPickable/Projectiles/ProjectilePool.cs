using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>Handles spawning of projectiles and registering the projectile instance to the PlayerPickableManager's update loop whenever an instance is "Get"</Summary>
public class ProjectilePool : GenericPools<Projectile, ProjectilePool>
{



    public static void ReturnInstanceOf(Projectile o)
    {
        ReturnInstanceOf(o.PickableInfo.Prefab, o);
    }

    #region Setup
    // public override int[] GetPoolCounts()
    // {
    //     Info[] projInfo = MasterGameManager.CurrentLevelInfo.ProjectilePoolInfos;

    //     int[] counts = new int[projInfo.Length];

    //     for (int i = 0; i < counts.Length; i++)
    //     {
    //         counts[i] = projInfo[i].Count;
    //     }

    //     return counts;
    // }

    // public override GameObject[] GetPoolPrefabs()
    // {
    //     Info[] projInfo = MasterGameManager.CurrentLevelInfo.ProjectilePoolInfos;

    //     GameObject[] prefabs = new GameObject[projInfo.Length];

    //     for (int i = 0; i < prefabs.Length; i++)
    //     {
    //         prefabs[i] = projInfo[i].PrefabInfo.Prefab;
    //     }

    //     return prefabs;
    // }

    #endregion

    #region Get
    // public static Projectile GetInstanceOf(ProjectileInfo info)
    // {
    //     Projectile p = GetInstanceOf(info.Prefab);
    //     PlayerPickableManager.RegisterPlayerPickable(p);
    //     return p;
    // }

    public static Projectile GetInstanceOf(ProjectileInfo info, Vector3 worldPosition, Vector3 velocity)
    {
        Projectile p = GetInstanceOf(info.Prefab, worldPosition);
        p.Initialize(velocity);
        return p;
    }


    // public static Projectile GetInstanceOf(ProjectileInfo info, Transform parent)
    // {
    //     Projectile p = GetInstanceOf(info.Prefab, parent);
    //     return p;
    // }

    // public static Projectile GetInstanceOf(ProjectileInfo info, Transform parent, Vector3 localPosition)
    // {
    //     Projectile p = GetInstanceOf(info.Prefab, parent, localPosition);
    //     return p;
    // }
    #endregion
}
