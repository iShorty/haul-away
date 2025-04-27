using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkTimerIndicator : ImageIndicator
{
    [SerializeField] UnityEngine.UI.Image _fillBg = default;
    ///<Summary>Sets the image fill amount of the image. Should be called by cargo or player's gameupdate while the UIIndicatorPool will handle the positioning update</Summary>
    public void SetImageFill(float fillPercentage)
    {
        image.fillAmount = fillPercentage;
    }

    public override void Initialize(Transform target)
    {
        base.Initialize(target);
        _fillBg.rectTransform.sizeDelta = Info.ImageDimension;
    }


    protected override void HandleEnteringOnScreen()
    {
        //Set image active

        _fillBg.enabled = image.enabled = true;
    }

    protected override void HandleEnteringOutSideScreen()
    {
        //Set image inactive
     _fillBg.enabled =    image.enabled = false;
    }
}
