using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrowableCollider 
{
    ///<Summary>The size of the collider which will be used to grow another collider </Summary>
    Vector3 Size { get; }
  
}
