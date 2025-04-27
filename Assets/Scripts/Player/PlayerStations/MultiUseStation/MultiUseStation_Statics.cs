using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class MultiUseStation : OverridePlayerMovementStation
{
    ///<Summary>Called when a player enters the multiuse station. The int is the multiuse index</Summary>
    public static event Action<int> OnPlayerEnter = null;
   
    ///<Summary>Called when a multiuse fires at a cargo successfully</Summary>
   public static event Action<IGrappleable> OnGrappleFireSuccess = null;
}
