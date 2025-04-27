using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DestinationIndicator : ImageIndicator
{
    [SerializeField]
    TextMeshProUGUI _distanceText = default;

    [SerializeField]
    Image _preferredCargoUI = default;

    private void Awake()
    {
        GameAwake();
        _distanceText.fontMaterial.SetFloat("_OutlineWidth", 0.5f);
    }
    #region Handles

    //Remove all sprite settings
    // public override void Initialize(Transform target)
    // {
    //     followingTarget = target;
    //     RefreshScreenValue();
    //     //By doing this, this will always ensure that the indicator starts with its handle events called
    //     _curOnScreen = GetTargetScreenPosition(out Vector3 screenPos);
    //     _prevOnScreen = !_curOnScreen;
    //     UpdateScreenIndicator(screenPos);
    // }

    public void SetPreferredCargoSprite(Sprite preferredCargo)
    {
        //In this case, image is desired carog image
        _preferredCargoUI.sprite = preferredCargo;
    }

    protected override void HandleEnteringOnScreen()
    {
        _distanceText.enabled = image.enabled =_preferredCargoUI.enabled= true;
        UpdateDistanceText();
    }

    protected override void HandleEnteringOutSideScreen()
    {
        _distanceText.enabled = image.enabled=_preferredCargoUI.enabled = false;
    }
    #endregion

    protected override void UpdateOnScreen(Vector3 screenPosition)
    {
        base.UpdateOnScreen(screenPosition);
        UpdateDistanceText();
    }

    void UpdateDistanceText()
    {
        float distance = Vector3.Distance(BoatManager.Controller.transform.position, followingTarget.position);
        _distanceText.text = $"{distance.ToString("F0")}m";
    }

}
