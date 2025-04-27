using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageIndicator : BaseUIIndicator
{


    #region Hidden Properties
    [field: SerializeField, Header("===== IMAGE INDICATOR ====="), RenameField(nameof(image))]
    public Image image { get; protected set; }


    #endregion

    public override void Initialize(Transform target)
    {
        base.Initialize(target);
        image.sprite = Info.IndicatorSprite;
        image.rectTransform.sizeDelta = Info.ImageDimension;
    }

}
