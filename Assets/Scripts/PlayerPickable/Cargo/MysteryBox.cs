using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBox : BaseCargo
{
    // While this is not on the ship, treat as normal cargo.
    // Land on ship, recycle to objpool
    // Then replace with enemy boarder or the cached cargo it's supposed to be.

    [Header("===== MYSTERY BOX INFO =====")]
    [Range(0.5f, 5f)] public float _OpenDelay = 2f;
    float _openTime = Mathf.Infinity;


    [Header("----- Possible Cargos -----")]
    [SerializeField]
    CargoInfo[] _cargoInfo = default;

    #region Runtime
    ///<Summary>Has the mystery box been opened yet?</Summary>
    bool _opened;
    ///<Summary>The cargo which will be spawned</Summary>
    CargoInfo _randomedCargo = default;

    #endregion

    private void Update() {
        if(_openTime + _OpenDelay < Time.time) {
            _openTime = Mathf.Infinity;
            OpenBox(FloatableProp.PropState.ONLAND);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _opened = false;
        //Random cargo here
        int rand = UnityEngine.Random.Range(0, _cargoInfo.Length);
        _randomedCargo = _cargoInfo[rand];
    }

    protected void OpenBox(PropState state)
    {
        // Ensures it only opens once
        if (_opened == true) return;
        _opened = true;

        // Spawn a random type of cargo!
        var c = BaseCargoPool.GetInstanceOf(_randomedCargo, state, transform.position);

        // Return the mystery box to its pool
        ReturnToPool();
    }

    #region Public OnTrigger Events

    // Start the box timer when it enters the deck of the boat
    public virtual void Box_OnEvaluateBoatDeckTrigger()
    {
        _openTime = Time.time;
    }
    #endregion


}
