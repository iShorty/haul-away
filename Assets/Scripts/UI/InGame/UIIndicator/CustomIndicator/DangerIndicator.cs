using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerIndicator : ImageIndicator
{

    #region Handles
    protected override void HandleEnteringOnScreen()
    {
        image.enabled = false;
    }

    protected override void HandleEnteringOutSideScreen()
    {
        image.enabled = true;
    }
    #endregion


}
