using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseUIIndicator
{
    #region Utility Methods
    ///<Summary>
    ///Changes the distance from the edges of the screen (aka border)
    ///</Summary>
    public void SetBorderValue(float borderValue)
    {
        _border = borderValue;
        RefreshScreenValue();
    }

    ///<Summary>
    ///Updates the internal values of the screen size. It is best to call this function when the screen size changes
    ///</Summary>
    public virtual void RefreshScreenValue()
    {
        _maxScreenX = Screen.width - _border;
        _maxScreenY = Screen.height - _border;
    }
    #endregion

}
