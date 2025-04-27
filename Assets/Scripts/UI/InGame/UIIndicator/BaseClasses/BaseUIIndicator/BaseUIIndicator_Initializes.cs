using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseUIIndicator
{
    public virtual void SetScreen_XOffset(float x)
    {
        _screenOffset.x = x;
    }

     public virtual void SetScreen_YOffset(float y)
    {
        _screenOffset.y = y;
    }

    public virtual void Initialize(Transform target)
    {
        followingTarget = target;
        RefreshScreenValue();
        //By doing this, this will always ensure that the indicator starts with its handle events called
        _curOnScreen = GetTargetScreenPosition(out Vector3 screenPos);
        _prevOnScreen = !_curOnScreen;
        UpdateScreenIndicator(screenPos);
    }

    public virtual void Initialize(Transform target, float offsetX, float offsetY)
    {
        Vector2 offsetValue = Vector2.zero;
        offsetValue.x = offsetX;
        offsetValue.y = offsetY;
        offSet = offsetValue;

        Initialize(target);
    }

    // public virtual void Initialize(Transform player, Transform target)
    // {
    //     // this.player = player;
    //     Initialize(target);

    // }

    // public virtual void Initialize(Transform player, Transform target, float offsetX, float offsetY)
    // // public virtual void Initialize(Transform player, Transform target, float offsetX, float offsetY)
    // {
    //     Vector2 offsetValue = Vector2.zero;
    //     offsetValue.x = offsetX;
    //     offsetValue.y = offsetY;
    //     offSet = offsetValue;

    //     // this.player = player;
    //     Initialize(target);

    // }


    // public virtual void Initialize(Camera camera, Transform player, Transform target)
    // {
    //     this.screenCamera = camera;
    //     // this.player = player;
    //     Initialize(target);

    // }

    public virtual void Initialize(Camera camera, Transform target, float offsetX, float offsetY)
    {
        Vector2 offsetValue = Vector2.zero;
        offsetValue.x = offsetX;
        offsetValue.y = offsetY;
        offSet = offsetValue;

        this.screenCamera = camera;
        Initialize(target);

    }

    public virtual void Initialize(Camera camera, Transform target)
    {
        this.screenCamera = camera;
        Initialize(target);

    }

    // public virtual void Initialize(Camera camera, Transform player, Transform target, float offsetX, float offsetY)
    // {
    //     Vector2 offsetValue = Vector2.zero;
    //     offsetValue.x = offsetX;
    //     offsetValue.y = offsetY;
    //     offSet = offsetValue;

    //     this.screenCamera = camera;
    //     // this.player = player;
    //     Initialize(target);

    // }

    public virtual void Initialize(Camera camera, Transform target, Vector2 offsetValue)
    // public virtual void Initialize(Camera camera, Transform player, Transform target, Vector2 offsetValue)
    {
        offSet = offsetValue;

        this.screenCamera = camera;
        // this.player = player;
        Initialize(target);

    }
}
