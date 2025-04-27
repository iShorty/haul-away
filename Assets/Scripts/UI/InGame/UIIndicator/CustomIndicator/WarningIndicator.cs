using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningIndicator : ImageIndicator
{


    float _timer = default;

    public override void Initialize(Transform target)
    {
        base.Initialize(target);
        //Set timer for delay before disappearing from screen
        _timer = Constants.For_InGameUI.WARNING_INDICATOR_TIMEDELAY;
    }

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

    public override bool GameUpdate()
    {
        //Updte base first, if target is null or inactive in hierahcy, then 
        if (base.GameUpdate())
        {
            return true;
        }

        //timer countdown, if reaches 0, disppear
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            return false;
        }

        _timer = 0;
        //Remove indicator
        UIIndicatorPool.ReturnInstanceOf(this.Info.Prefab, this);
        return true;
    }

}
