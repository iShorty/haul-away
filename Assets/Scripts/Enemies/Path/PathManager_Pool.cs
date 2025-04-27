using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class PathManager
{

    // #region Definition

    // [System.Serializable]
    // public class Info : GenericPoolSettings<PathInfo> { }

    // #endregion

    #region GenericPool Methods
    
    #region Setup

    // public override int[] GetPoolCounts()
    // {
    //     Info[] pathInfo = MasterGameManager.CurrentLevelInfo.PathPoolInfos;

    //     int[] counts = new int[pathInfo.Length];

    //     for (int i = 0; i < counts.Length; i++)
    //     {
    //         counts[i] = pathInfo[i].Count;
    //     }

    //     return counts;
    // }

    // public override GameObject[] GetPoolPrefabs()
    // {
    //     Info[] pathInfo = MasterGameManager.CurrentLevelInfo.PathPoolInfos;

    //     GameObject[] prefabs = new GameObject[pathInfo.Length];

    //     for (int i = 0; i < prefabs.Length; i++)
    //     {
    //         prefabs[i] = pathInfo[i].PrefabInfo.Prefab;
    //     }

    //     return prefabs;
    // }

    #endregion


    // public static void ReturnInstanceOf(MovementPath o)
    // {
    //     ReturnInstanceOf(o.pathInfo.Prefab, o);
    // }

    public static MovementPath GetInstanceOf(PathInfo info, PathType pathType)
    {
        MovementPath o = GetInstanceOf(info.Prefab);
        o._PathType = pathType;
        return o;
    }

    public static MovementPath GetInstanceOf(PathInfo info, PathType pathType, Vector3[] nodePos)
    {
        MovementPath o = GetInstanceOf(info.Prefab);
        o._PathType = pathType;
        foreach (Vector3 pos in nodePos)
        {
            GameObject node = new GameObject(); // Set up a pool for nodes? Just empty GOs?
            node.transform.position = pos;
            node.transform.SetParent(o.transform);
            o.pathNodes.Add(node.transform);
        }
        return o;
    }

    public static MovementPath GetInstanceOf(PathInfo info, Vector3 worldPosition, PathType pathType)
    {
        MovementPath o = GetInstanceOf(info.Prefab, worldPosition);
        o._PathType = pathType;
        return o;
    }

    public static MovementPath GetInstanceOf(PathInfo info, Vector3 worldPosition, PathType pathType, Vector3[] nodePos)
    {
        MovementPath o = GetInstanceOf(info.Prefab, worldPosition);
        o._PathType = pathType;
        foreach (Vector3 pos in nodePos)
        {
            GameObject node = new GameObject(); // Set up a pool for nodes? Just empty GOs?
            node.transform.position = pos;
            node.transform.SetParent(o.transform);
            o.pathNodes.Add(node.transform);
        }
        return o;
    }

    public static MovementPath GetInstanceOf(PathInfo info, Transform parent, PathType pathType)
    {
        MovementPath o = GetInstanceOf(info.Prefab, parent);
        o._PathType = pathType;
        return o;
    }

    public static MovementPath GetInstanceOf(PathInfo info, Transform parent, PathType pathType, Vector3 localPosition)
    {
        MovementPath o = GetInstanceOf(info.Prefab, parent, localPosition);
        o._PathType = pathType;
        return o;
    }

    #endregion

}
