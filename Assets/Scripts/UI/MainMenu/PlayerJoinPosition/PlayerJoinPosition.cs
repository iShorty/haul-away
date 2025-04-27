using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJoinPosition : MonoBehaviour
{

    GameObject _model = default;

    public void ChangeModel(int playerIndex,  PlayerModelInfo info)
    {
        if (_model != null)
        {
            Destroy(_model);
        }

        _model = Instantiate(info.PlayerModels[playerIndex]);
        Transform t = _model.transform;
        t.SetParent(transform);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
    }

}
