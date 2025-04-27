using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int TotalStarCount = 0;
    // public int TotalStarCount;
    public float MasterVol = 1f;
    public float SFXVol = 1f;
    public float BGMVol = 1f;
    public List<LevelData> levelDats = new List<LevelData>();

#if UNITY_EDITOR
    public void LogData()
    {
        string log = $"TotalStarCount: {TotalStarCount} \n MasterVol: {MasterVol} \n SFXVol: {SFXVol} \n BGMVol: {BGMVol}";
        Debug.Log(log);

        log = "";

        for (int i = 0; i < levelDats.Count; i++)
        {
            log += $"========== LevelData Index{i} ========== \n {levelDats[i].LogValues()}";
        }
    }
#endif
}
