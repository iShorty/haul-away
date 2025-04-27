using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCargoPool : GenericPools<BaseCargo, BaseCargoPool>
{
    
    #region Setup

    public static void ReturnInstanceOf(BaseCargo o)
    {
        ReturnInstanceOf(o.CargoInfo.Prefab, o);
    }
    #endregion


    #region Get
    public static BaseCargo GetInstanceOf(CargoInfo info, BaseCargo.PropState stateToSetTo)
    {
        BaseCargo o = GetInstanceOf(info.Prefab);
        o.PubSetPropState(stateToSetTo);
        return o;
    }

    public static BaseCargo GetInstanceOf(CargoInfo info, BaseCargo.PropState stateToSetTo, Vector3 worldPosition)
    {
        BaseCargo o = GetInstanceOf(info.Prefab, worldPosition);
        o.PubSetPropState(stateToSetTo);
        return o;
    }

    public static BaseCargo GetInstanceOf(CargoInfo info, BaseCargo.PropState stateToSetTo, Transform parent)
    {
        BaseCargo o = GetInstanceOf(info.Prefab, parent);
        o.PubSetPropState(stateToSetTo);
        return o;
    }

    public static BaseCargo GetInstanceOf(CargoInfo info, BaseCargo.PropState stateToSetTo, Transform parent, Vector3 localPosition)
    {
        BaseCargo o = GetInstanceOf(info.Prefab, parent, localPosition);
        o.PubSetPropState(stateToSetTo);
        return o;
    }


    #endregion

}
