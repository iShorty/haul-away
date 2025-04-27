using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelPool : GenericPools<FuelItem, FuelPool>
{
    public static void ReturnInstanceOf(FuelItem o)
    {
        ReturnInstanceOf(o.FuelInfo.Prefab, o);
    }

    #region Get
    public static FuelItem GetInstanceOf(FuelInfo info)
    {
        FuelItem o = GetInstanceOf(info.Prefab);
        return o;
    }

    public static FuelItem GetInstanceOf(FuelInfo info, Vector3 worldPosition)
    {
        FuelItem o = GetInstanceOf(info.Prefab, worldPosition);
        return o;
    }

    public static FuelItem GetInstanceOf(FuelInfo info, Transform parent)
    {
        FuelItem o = GetInstanceOf(info.Prefab, parent);
        return o;
    }

    public static FuelItem GetInstanceOf(FuelInfo info, Transform parent, Vector3 localPosition)
    {
        FuelItem o = GetInstanceOf(info.Prefab, parent, localPosition);
        return o;
    }
    #endregion
}
