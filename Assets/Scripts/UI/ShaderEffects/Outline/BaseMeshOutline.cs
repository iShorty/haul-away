using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class BaseMeshOutline : MonoBehaviour
{
    #region Constants

    const string SHADERPROPERTIES_OUTLINE_ALPHA = "_Alpha"
    , SHADERPROPERTIES_OUTLINE_USEPRECALCULATEDNORMALS = "_PrecalculateNormals"
    , SHADERPROPERTIES_OUTLINE_ZTEST = "_OutlineZTest"
    , SHADERPROPERTIES_OUTLINE_THICKNESS = "_Thickness"
    , SHADERPROPERTIES_OUTLINE_COLOR = "_Color"
#if UNITY_EDITOR
    , SHADERPROPERTIES_OUTLINE_PASSNAME = "Outlines"
#endif
    ;
    #endregion

    #region Definitions
    public enum OutlineMode
    {
        ///<Summary>Initalized state</Summary>
        NONE = -1
        ,
        ///<Summary>Outline is turned off</Summary>
        OFF = 0
        ,
        ///<Summary>Outline is blocked by opaque objects and is only visible to unblocked surfaces</Summary>
        PARTIALLYHIDDEN = 2
    }

    static readonly int OUTLINE_OFF_ALPHA = 0
    , OUTLINE_ON_ALPHA = 1
    , OUTLINE_PARTIALLYHIDDEN_ZTEST = (int)CompareFunction.LessEqual
    ;
    #endregion

    [Range(0, 3)]
    [SerializeField]
    protected int _materialIndex = 0;

    [SerializeField]
    OutlineInfo _info = default;

    //Runtime
    protected OutlineMode _currentState = OutlineMode.NONE;
    protected Material _outlineMaterial = default;

    public void GameAwake()
    {
        _outlineMaterial = GetComponent<MeshRenderer>().materials[_materialIndex];

#if UNITY_EDITOR
        Debug.Assert(_info != null, $"The meshoutline instance on {name} does not have a outline info!", this);
        Debug.Assert(_outlineMaterial.FindPass(SHADERPROPERTIES_OUTLINE_PASSNAME) != -1, $"The meshrenderer on the object {name} does not have the correct outline material on it!", this);
#endif

        _outlineMaterial.SetFloat(SHADERPROPERTIES_OUTLINE_THICKNESS, _info.Thickness);
        _outlineMaterial.SetColor(SHADERPROPERTIES_OUTLINE_COLOR, _info.OutlineColor);
        if (_info.PrecalculateNormals)
        {
            _outlineMaterial.SetFloat(SHADERPROPERTIES_OUTLINE_USEPRECALCULATEDNORMALS, 1.0f);
        }

        ToggleOutline(OutlineMode.OFF);
    }

    public void ToggleOutline(OutlineMode state)
    {
        if (_currentState == state) return;

        _currentState = state;
        switch (state)
        {
            case OutlineMode.OFF:
                _outlineMaterial.SetFloat(SHADERPROPERTIES_OUTLINE_ALPHA, OUTLINE_OFF_ALPHA);
                break;

            case OutlineMode.PARTIALLYHIDDEN:
                _outlineMaterial.SetFloat(SHADERPROPERTIES_OUTLINE_ALPHA, OUTLINE_ON_ALPHA);
                _outlineMaterial.SetFloat(SHADERPROPERTIES_OUTLINE_ZTEST, OUTLINE_PARTIALLYHIDDEN_ZTEST);
                break;
        }
    }
}
