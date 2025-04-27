using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint : MonoBehaviour
{
    [SerializeField]
    MovementPath path = default;
    [SerializeField]
    PathPoint[] neighbours = default;
    [SerializeField]
    PathPoint pathTo = default;


    private void OnTriggerEnter(Collider other) {
        
    }

    private void OnTriggerExit(Collider other) {
        
    }
}
