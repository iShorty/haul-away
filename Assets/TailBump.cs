using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailBump : MonoBehaviour
{
    public SphereCollider _TailBumpTrigger = default;

    [SerializeField]
    RammerController _rammerController = default;



    protected void OnEnable()
    {
        _TailBumpTrigger.enabled = false;
    }

    ///<Summary>Handles the triggering of the attack animation for the rammer</Summary>
    private void OnTriggerEnter(Collider other)
    {
        if (_rammerController.aiState != RammerState.ATTACK) return;

        if (!BoatManager.IsPartOfBoat(collider: other)) return;

#if UNITY_EDITOR
        Debug.Log("tailbump doin da bump");
#endif
        if(_TailBumpTrigger.enabled != false) {
            _TailBumpTrigger.enabled = false;

            Vector3 dir = -transform.up;
            other.attachedRigidbody.AddForceAtPosition(dir * _rammerController.bumpForce, transform.position, ForceMode.Impulse);
            // collision.rigidbody?.AddForceAtPosition(dir * value, collision.GetContact(0).point, ForceMode.Impulse);

        }
        // _animController.SetTrigger(Constants.For_Enemy.ORCA_ANIMATION_PARAM_ATTACK);
        // _animationTrigger.enabled = false;
    }

}
