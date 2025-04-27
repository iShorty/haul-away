using System.Linq;
using UnityEngine;
using System.Collections.Generic;



public class DetectionModule : MonoBehaviour
{
    [Tooltip("The point representing the source of target-detection raycasts for the enemy AI")]
    public Transform detectionSourcePoint;
    public SphereCollider detCollider;
    [Tooltip("The max distance at which the enemy can see targets")]
    public float detectionRange = 20f;
    [Tooltip("The max distance at which the enemy can attack its target")]
    public float attackRange = 10f;
    public float SqredAttackRange = default;
    [Tooltip("Time before an enemy abandons a known target that it can't see anymore")]
    public float knownTargetTimeout = 4f;
    [Tooltip("Optional animator for OnShoot animations")]
    public Animator animator;

    public event System.Action onDetectedTarget = null;
    public event System.Action onLostTarget = null;

    // [SerializeField]
    public GameObject knownDetectedTarget = default;
    public Vector3 lastKnownDetectedTargetPos = Vector3.negativeInfinity;
    [Tooltip("Update the lastKnownDetectedTargetPos when the target has moved a sufficient distance")]
    [SerializeField] float minUpdateDist = 1f;
    [HideInInspector] public float SqredMinUpdateDist = default;
    public bool isTargetInAttackRange { get; private set; }
    public bool isSeeingTarget { get; private set; }
    public bool hadKnownTarget { get; private set; }

    protected float _timeLastSeenTarget = Mathf.NegativeInfinity;

    const string _animAttackParameter = "Attack";
    const string _animOnDamagedParameter = "OnDamaged";

    public DetectionTargetConditions detTargetInfo;
    List<GameObject> _targetsDetected = new List<GameObject>();



    public void Initialise() {
        detCollider.radius = detectionRange;
        SqredMinUpdateDist = minUpdateDist * minUpdateDist;
        SqredAttackRange = attackRange * attackRange;
    }

    // Check to see if the target is valid using the detTargetInfo SO
    bool CheckTargetConditions(GameObject GO) {
        if(detTargetInfo.layerMask.Contains(GO.layer)) {
            return true;
        }
        if(detTargetInfo.tags.Contains(GO.tag)) {
            return true;
        }
        return false;
    }

    public virtual void PostMovement() {
        if(knownDetectedTarget != null) {
            if(lastKnownDetectedTargetPos.x == Mathf.Infinity ||
            (lastKnownDetectedTargetPos - knownDetectedTarget.transform.position).sqrMagnitude > SqredMinUpdateDist) {
                lastKnownDetectedTargetPos = knownDetectedTarget.transform.position;
            }
        }
    }
    
    public virtual void HandleTargetDetection() {
        // Handle known target detection timeout
        if (knownDetectedTarget && !isSeeingTarget && (Time.time - _timeLastSeenTarget) > knownTargetTimeout) {
            knownDetectedTarget = null;
        }

        isSeeingTarget = false;

        // For each thing in range that meets the criteria,
        // Raycast check if its in view.
        float closestDist = Mathf.Infinity;
        float sqredDistToTarget = 0;
        foreach (GameObject target in _targetsDetected) {
            // Dist check for closest target
            sqredDistToTarget = (target.transform.position - detectionSourcePoint.position).sqrMagnitude;
            if(sqredDistToTarget > closestDist) continue;
            // If I shoot a raycast at it and it hits- if I have LOS to it
            if(Physics.Raycast(detectionSourcePoint.position, 
            (target.transform.position - detectionSourcePoint.position).normalized, 
            out RaycastHit hit, 
            Vector3.Distance(target.transform.position, detectionSourcePoint.position), 
            LayerMask.GetMask(LayerMask.LayerToName(target.layer)))) { // Layermask or just layer? check pls braain is fried
                if(hit.collider.attachedRigidbody.gameObject == target) {
                    closestDist = sqredDistToTarget;

                    isSeeingTarget = true;
                    _timeLastSeenTarget = Time.time;

                    knownDetectedTarget = hit.collider.attachedRigidbody?.gameObject;

                    if(knownDetectedTarget == null) {
                        Debug.LogWarning("KnownDetectedTarget set to null? Hit: " + hit.collider.gameObject.name, hit.collider.gameObject);
                    }
                }
            }
        }

        isTargetInAttackRange = knownDetectedTarget != null && 
        (transform.position - knownDetectedTarget.transform.position).sqrMagnitude <= SqredAttackRange;

        // Detection events
        if (!hadKnownTarget && knownDetectedTarget != null) {
            OnDetect();
        }

        if (hadKnownTarget && knownDetectedTarget == null) {
            OnLostTarget();
        }

        // Remember if we already knew a target (for next frame)
        hadKnownTarget = knownDetectedTarget != null;
    }

    // Invoke onLostTarget from derived classes of enemy (only script listening atm at least)
    public virtual void OnLostTarget() {
        onLostTarget?.Invoke();
    }

    // Invoke onDetectedTarget from derived classes of enemy (only script listening atm at least)
    public virtual void OnDetect() {
        onDetectedTarget?.Invoke();
    }

    public virtual void OnDamaged(GameObject source) {
        if(source == null) {
            return;
        }
        // If in range and hit by raycast, it gets set as the target.
        if((source.transform.position - transform.position).sqrMagnitude < detectionRange * detectionRange) {
            if(Physics.Raycast(detectionSourcePoint.position, 
            (source.transform.position - detectionSourcePoint.position).normalized, 
            out RaycastHit hit, 
            Vector3.Distance(source.transform.position, detectionSourcePoint.position), 
            LayerMask.GetMask(LayerMask.LayerToName(source.layer)))) { // Layermask or just layer? check pls braain is fried
                if(hit.collider.attachedRigidbody.gameObject == source) {
                    isSeeingTarget = true;
                    _timeLastSeenTarget = Time.time;
                    // updateLastKnownLate = true;

                    knownDetectedTarget = source;

                    if(knownDetectedTarget == null) {
                        Debug.LogWarning("KnownDetectedTarget set to null? Hit: " + hit.collider.gameObject.name, hit.collider.gameObject);
                    }
                }
            }
        }

        if (animator) {
            animator.SetTrigger(_animOnDamagedParameter);
        }
    }

    // Trigger attack anim
    public virtual void OnAttack() {
        if (animator) {
            animator.SetTrigger(_animAttackParameter);
        }
    }

    // Uses a sphere collider to detect possible targets
    private void OnTriggerEnter(Collider other) {
        // Targets will have rigidbodies- i can ignore the rest
        if(other.attachedRigidbody == false) {
            return;
        }
        if(_targetsDetected.Contains(other.attachedRigidbody.gameObject) || 
        CheckTargetConditions(other.attachedRigidbody.gameObject) == false) return;
        
        _targetsDetected.Add(other.attachedRigidbody.gameObject);
    }

    // Remove target from list
    private void OnTriggerExit(Collider other)
    {
        _targetsDetected.Remove(other.attachedRigidbody?.gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        // Gizmos.DrawWireSphere(transform.position, BoatManager.GetMultiUseStation(0).Info.AttackRectSize.x);
    }


}

