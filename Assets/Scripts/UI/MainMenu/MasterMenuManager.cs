using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterMenuManager : BaseMasterManager<MasterMenuManager>
{
    protected override void FixedUpdate()
    {
        GlobalEvents.SendGameFixedUpdate_DURINGGAME();
    }

    protected override void Update()
    {
        GlobalEvents.SendGameUpdate_DURINGGAME();
    }

    protected override void LateUpdate()
    {
        GlobalEvents.SendGameLateUpdate_DURINGGAME();
    }

    protected override void OnDestroy()
    {

    }
}
