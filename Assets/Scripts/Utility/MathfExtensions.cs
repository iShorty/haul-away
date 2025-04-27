using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathfExtensions
{
    ///<Summary>
    ///Loops the value t, so that it is never larger than length and never smaller than 0.
    ///</Summary>
    public static int Repeat(int t, int length)
    {
        if (t < 0)
            return length;
        else if (t > length)
            return 0;
        else
            return t;
    }
}
