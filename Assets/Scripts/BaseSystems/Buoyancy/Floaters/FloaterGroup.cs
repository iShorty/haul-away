using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class uses unity fixedupdte and hence is good for prototyping
[RequireComponent(typeof(Rigidbody))]
public class FloaterGroup : BaseFloaterGroup
{
    private void FixedUpdate()
    {
        GameFixedUpdate();
    }

    private void Update()
    {
        GameUpdate();
    }
}
