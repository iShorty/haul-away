using UnityEngine;

[System.Serializable]
public class MoveReticle
{
    public Transform Reticle;

    ///<summary>true for left, false for right</summary>
    [SerializeField]
    bool _flipTargetingSide = default;

    #region Hidden Fields
    Rect _allowedArea;
    MultiUseStationInfo _info = default;
    #endregion

#if UNITY_EDITOR
    public void GameAwake(Transform parent, MultiUseStationInfo info, System.Action<Component> deleteFunction)
    {
        GameAwake(info);

        if (!info.DrawRectCorners) return;

        //Visualisation plz
        GameObject[] markers = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            GameObject rectMarkers = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rectMarkers.transform.SetParent(parent);
            Collider c = rectMarkers.GetComponent<Collider>();
            deleteFunction?.Invoke(c);

            markers[i] = rectMarkers;
        }

        markers[0].transform.localPosition = new Vector3(_allowedArea.xMin, 0, _allowedArea.yMin);
        markers[1].transform.localPosition = new Vector3(_allowedArea.xMin, 0, _allowedArea.yMax);
        markers[2].transform.localPosition = new Vector3(_allowedArea.xMax, 0, _allowedArea.yMin);
        markers[3].transform.localPosition = new Vector3(_allowedArea.xMax, 0, _allowedArea.yMax);
    }
#endif

    public void GameAwake(MultiUseStationInfo info)
    {
        // #if UNITY_EDITOR
        //         Debug.Assert(_mr.material.FindPass(Constants.MATERIAL_URPDECAL_PASSNAME) != -1, $"The material renderer of the station.", _mr);
        // #endif

        _info = info;
        float x = _flipTargetingSide ? -(_info.AttackRectSize.x + _info.AttackRectOffset) : _info.AttackRectOffset;
        //Rect starts at the top left
        _allowedArea = new Rect(x, -_info.AttackRectSize.y * 0.5f, _info.AttackRectSize.x, _info.AttackRectSize.y);

        //Set the reticleholder's y level to sea level
        Vector3 reticlePos = Reticle.transform.position;
        reticlePos.y = Constants.For_PlayerStations.MULTIPURPOSE_WATERLEVEL;
        Reticle.transform.position = reticlePos;
    }

    public void GameEnable()
    {
        Reticle.gameObject.SetActive(false);
        // ChangeReticleMaterial(false);
    }

    public void UpdateReticle(Vector2 playerInput)
    {
        Vector3 velocity = new Vector3(playerInput.x, 0, playerInput.y) * _info.ReticleSpeed;
        Vector3 displacement = velocity * Time.deltaTime;
        Vector3 newPosition = Reticle.localPosition + displacement;


        newPosition.x = Mathf.Clamp(newPosition.x, _allowedArea.xMin, _allowedArea.xMax);
        newPosition.z = Mathf.Clamp(newPosition.z, _allowedArea.yMin, _allowedArea.yMax);

        Reticle.localPosition = newPosition;
    }

    ///<Summary>Called when the player uses the station this reticle is at</Summary>
    public void PlayerUseStation()
    {
        Reticle.gameObject.SetActive(true);
    }

    ///<Summary>Called when the player leaves the station this reticle is at</Summary>
    public void PlayerLeaveStation()
    {
        Reticle.gameObject.SetActive(false);
    }

    // public void ChangeReticleMaterial(bool targetFound)
    // {
    //     // Texture t = targetFound ? _info.TargetFoundReticle : _info.TargetNotFoundReticle;
    //     // _mr.material.SetTexture(Constants.MATERIAL_URPDECAL_PROPERTYNAME_TEXTURE, t);

    //     // ValidTargetReticle.SetActive(targetFound);
    //     // InvalidTargetReticle.SetActive(!targetFound);
    // }
}