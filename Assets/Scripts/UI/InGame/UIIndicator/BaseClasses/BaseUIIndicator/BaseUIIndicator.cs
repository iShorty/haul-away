using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class BaseUIIndicator : MonoBehaviour
{
    #region  Exposed Properties
    [field: SerializeField, Header("----- Target -----")]
    public Transform followingTarget { get; protected set; }
    [field: SerializeField, Header("----- Camera -----")]
    public Camera screenCamera { get; protected set; }
    // [field: SerializeField, Header("----- Player -----")]
    // public Transform player { get; protected set; }
    #endregion

    [Header("----- Info -----")]
    public IndicatorInfo Info = null;

    [Header("Display Settings - Values")]
    [SerializeField]
    [Range(0, 1000)]
    [Tooltip("What is the distance of this sprite from the edge of the screen?")]
    protected float _border = 100f;

    [field: SerializeField, Header("OffSet from Target")]
    public Vector2 offSet { get; set; } = Vector2.zero;

    public  Vector2 _screenOffset = default;

    [field: SerializeField, Header("Arrow Angle Offset")]
    public float arrowAngleOffset { get; set; } = 180f;

    #region Runtime var
    protected float _maxScreenX = 0;
    protected float _maxScreenY = 0;
    protected bool _curOnScreen = false;
    protected bool _prevOnScreen = false;
    #endregion

    #region Lifetime

    public virtual void GameAwake() { }

    protected virtual void OnEnable()
    {
        RefreshScreenValue();
    }

    protected virtual void OnDisable()
    {

    }


    ///<Summary>Returns true when the target of this indicator is null or inactive in hierahcy and hence should be calling ExitUpdateLoop function</Summary>
    public virtual bool GameUpdate()
    {
        if (followingTarget == null || !followingTarget.gameObject.activeInHierarchy)
        {
            ExitUpdateLoop();
            return true;
        }

        _prevOnScreen = _curOnScreen;
        // Vector3 screenPosition = followingTarget.position;
        // screenPosition.x += offSet.x;
        // screenPosition.y += offSet.y;

        //============= SPRITE POSITIONING ===================
        _curOnScreen = GetTargetScreenPosition(out Vector3 screenPosition);
        // screenPosition = screenCamera.WorldToScreenPoint(screenPosition);

        // _curOnScreen = screenPosition.z > 0 && screenPosition.x > _border && screenPosition.x <= _maxScreenX && screenPosition.y > _border && screenPosition.y <= _maxScreenY;
        UpdateScreenIndicator(screenPosition);

        return false;
    }

    ///<Summary>Exits the baseui indicator from the update loop it is in. By default, it will return to the UI-Indicator pool</Summary>
    protected virtual void ExitUpdateLoop()
    {
        UIIndicatorPool.ReturnInstanceOf(Info.Prefab.gameObject, this);
    }
    #endregion


    #region Update Methods
    ///<Summary>Updates the screenposition of the indicator. It will also check for a change in current state and previous state. If there is a change, then it will call the HandleEnteringOnScreen or HandleEnteringOutSideScreen function.</Summary>
  protected  void UpdateScreenIndicator(Vector3 screenPosition)
    {
        switch (_curOnScreen)
        {
            case true:

                //If current on screen status is not equal to prev's status,
                if (_curOnScreen != _prevOnScreen)
                {
                    //Execute handle On enter onScreen state
                    HandleEnteringOnScreen();
                }

                UpdateOnScreen(screenPosition);

                break;

            case false:

                //If current on screen status is not equal to prev's status,
                if (_curOnScreen != _prevOnScreen)
                {
                    //Execute handle On enter outsidescreen state
                    HandleEnteringOutSideScreen();
                }

                UpdateOffScreen(screenPosition);

                break;

        }


    }

    ///<Summary>
    ///Handles the positioning of the ui indicator when it screenpoint is on screen.
    ///</Summary>
    protected virtual void UpdateOnScreen(Vector3 screenPosition)
    {
        screenPosition.z = 0;
        screenPosition.x +=_screenOffset.x;
        screenPosition.y +=_screenOffset.y;
        transform.position = screenPosition;
    }


    ///<Summary>
    ///Handles the positioning and rotation of the ui indicator when it screenpoint is off screen.
    ///</Summary>
    protected virtual void UpdateOffScreen(Vector3 screenPosition)
    {
        //Check if target is behind the camera
        if (screenPosition.z < 0) screenPosition *= -1;

        //If screen position is within the x boundaries
        if (screenPosition.x > _border && screenPosition.x < _maxScreenX)
        {
            screenPosition.y = screenPosition.y < _maxScreenY * 0.5f ? _border : _maxScreenY;
        }
        else
        {
            screenPosition.x = Mathf.Clamp(screenPosition.x, _border, _maxScreenX);
        }

        //If screen position is within the y boundaries
        if (screenPosition.y > _border && screenPosition.y < _maxScreenY)
        {
            screenPosition.x = screenPosition.x < _maxScreenX * 0.5f ? _border : _maxScreenX;
        }
        else
        {
            screenPosition.y = Mathf.Clamp(screenPosition.y, _border, _maxScreenY);
        }


        screenPosition.z = 0;
        transform.position= screenPosition;

        //=================== SPRITE ROTATION ======================
        RotateIndicator();
    }


    #endregion

    #region Handle Methods
    ///<Summary>
    ///Handles the positioning and rotation of the ui indicator when it screenpoint is on screen.
    ///</Summary>
    protected virtual void HandleEnteringOutSideScreen() { }

    ///<Summary>
    ///Handles the positioning and rotation of the ui indicator when it screenpoint is on screen.
    ///</Summary>
    protected virtual void HandleEnteringOnScreen()
    {
        //=================== SPRITE ROTATION ======================
        transform.localRotation = Quaternion.identity;
    }

    #endregion

    #region Supporting Methods

    ///<Summary>Returns true when the target is in the camera's view frustrum and returns the screen position if true</Summary>
    protected bool GetTargetScreenPosition(out Vector3 screenPosition)
    {
        //Reusuing screenPosition
        screenPosition = followingTarget.position;
        screenPosition.x += offSet.x;
        screenPosition.y += offSet.y;

        screenPosition = screenCamera.WorldToScreenPoint(screenPosition);
        return screenPosition.z > 0 && screenPosition.x > _border && screenPosition.x <= _maxScreenX && screenPosition.y > _border && screenPosition.y <= _maxScreenY;
    }



    ///<Summary>
    ///Rotates this transform's localrotation to point towards the followingTarget
    ///</Summary>
    protected virtual void RotateIndicator()
    {
        // //================== IMAGE ROTATION ==================
        // //Imagine making target's transform into player transform's child. The dir value is the position you see in the inspector in relation to Player's forward axis
        // Vector3 dir = followingTarget.position - player.position;
        // // Vector3 dir = player.InverseTransformPoint(followingTarget.position);
        // //Remove 180f if arrow sprite by default is pointing up
        // float angle = arrowAngleOffset + Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        // //Because we use inversretransformpoint, when the player rotate their forward axis, the dir will change despite the player being in the same position.
        // // transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        // transform.localRotation = Quaternion.Euler(0f, 180f, angle);
    }



    #endregion

}
