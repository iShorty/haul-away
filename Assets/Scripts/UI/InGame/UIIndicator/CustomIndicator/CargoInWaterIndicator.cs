using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoInWaterIndicator : ImageIndicator
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    #region Handles
    protected override void HandleEnteringOnScreen()
    {
        image.enabled = true;
    }

    protected override void HandleEnteringOutSideScreen()
    {
        image.enabled = false;
    }
    #endregion
}
