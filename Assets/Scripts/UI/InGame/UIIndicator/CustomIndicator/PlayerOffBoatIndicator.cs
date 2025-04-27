using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerOffBoatIndicator : ImageIndicator
{
    #region Constants
    static readonly Color OUTLINE_COLOR = new Color(0.0377f, 0.0377f, 0.0377f);
    #endregion

    [Header("===== PLAYER OFFBOAT INDICATOR =====")]
    [SerializeField]
    Image _fillBg = default;

    TextMeshProUGUI _playerText = default;

    private void Awake()
    {
        GameAwake();
    }

    public override void GameAwake()
    {
        _playerText = GetComponentInChildren<TextMeshProUGUI>();
#if UNITY_EDITOR
        Debug.Assert(_playerText, $"The playeroffboatindicator {name} does not have a TextMeshProUGUI on it!", this);
#endif
        // //Set text settings
        // _playerText.outlineWidth = 0.25f;
        // _playerText.outlineColor = OUTLINE_COLOR;
    }


    ///<Summary>Sets the image fill amount of the image. Should be called by cargo or player's gameupdate while the UIIndicatorPool will handle the positioning update</Summary>
    public void SetImageFill(float fillPercentage)
    {
        image.fillAmount = fillPercentage;
    }

    public void SetPlayerIndexUI(int index)
    {
        Color playerColor = Constants.For_Player.GetColor(index);
        image.color = playerColor;

        //Because player indices are zero based indices
        index++;
        _playerText.text = $"P{index}";
    }

    public override void Initialize(Transform target)
    {
        base.Initialize(target);
        _fillBg.rectTransform.sizeDelta = Info.ImageDimension;
    }

}
