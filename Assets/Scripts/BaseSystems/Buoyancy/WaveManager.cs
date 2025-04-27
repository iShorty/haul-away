using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : GenericManager<WaveManager>
{
    [Header("===== INFO =====")]
    [SerializeField]
    WaveInfo _waveInfo = default;

    float _offset = default;

    public override void OnDestroy()
    {
        GlobalEvents.OnGameUpdate_DURINGGAME -= DuringGameUpdate;

    }

    protected override void OnGameAwake()
    {
        GlobalEvents.OnGameUpdate_DURINGGAME += DuringGameUpdate;

    }
    #region Update Methods
    void DuringGameUpdate()
    {
        _offset += Time.deltaTime * _waveInfo.Speed;
    }

    #endregion

    #region Get Methods
    public static float GetWaveHeight(float x)
    {
        return instance._waveInfo.Amplitude * Mathf.Sin(x / instance._waveInfo.WaveLength + instance._offset);
        // return instance._waveInfo.Amplitude * Mathf.Sin(x / instance._waveInfo.WaveLength + instance._offset);
    }


    #endregion

}
