using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBombable 
{
    ///<Summary>Is used to properly exit affected units from their states when they get affected by a bomb blast</Summary>
    void BombBlast(float force,Vector3 bombPosition, float blastRadius, float upwardsModifier);
}
