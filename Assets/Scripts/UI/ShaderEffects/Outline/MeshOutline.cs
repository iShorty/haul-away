using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public partial class MeshOutline : BaseMeshOutline
{
    private void Awake()
    {
        GameAwake();
    }

}
