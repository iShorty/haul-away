using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class EnemyManager
{
    
    #region GenericPool Methods

    public void ReturnInstanceOf(Enemy o)
    {
        ReturnInstanceOf(o.EnemyStats.Prefab, o);
    }

    public static Enemy GetInstanceOf(GameObject originalPrefab, Enemy.PropState stateToSetTo)
    {
        Enemy o = GetInstanceOf(originalPrefab);
        o.PubSetPropState(stateToSetTo);
        // o.SetState(stateToSetTo);
        return o;
    }

    public static Enemy GetInstanceOf(GameObject originalPrefab, Enemy.PropState stateToSetTo, Vector3 worldPosition)
    {
        Enemy o = GetInstanceOf(originalPrefab, worldPosition);
        o.PubSetPropState(stateToSetTo);
        //   o.SetState(stateToSetTo);
        return o;
    }

    public static Enemy GetInstanceOf(GameObject originalPrefab, Enemy.PropState stateToSetTo, Transform parent)
    {
        Enemy o = GetInstanceOf(originalPrefab, parent);
        o.PubSetPropState(stateToSetTo);
        //   o.SetState(stateToSetTo);
        return o;
    }

    public static Enemy GetInstanceOf(GameObject originalPrefab, Enemy.PropState stateToSetTo, Transform parent, Vector3 localPosition)
    {
        Enemy o = GetInstanceOf(originalPrefab, parent, localPosition);
        o.PubSetPropState(stateToSetTo);
        //   o.SetState(stateToSetTo);
        return o;
    }

    #endregion

}
