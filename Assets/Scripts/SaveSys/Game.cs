using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game _Game;
    public GameData _GameData;

    public KeyCode DeleteKey = KeyCode.D;
    public KeyCode SkipKey = KeyCode.K;

    void Update()
    {
        if (Input.GetKeyDown(DeleteKey))
        {
            SaveSystem.DeleteAllSaveFiles();
        }
        if (Input.GetKeyDown(SkipKey))
        {
            MasterLevelUnlock();
        }
    }

    public void InitGameMap()
    {
        _Game = this;
        Load();
    }

    public void Save(GameData data)
    {
        _GameData.levelDats = data.levelDats;
        _GameData.TotalStarCount = data.TotalStarCount;
        SaveSystem.SaveGame(_GameData);
    }
    
    public void Load()
    {
        GameData data = SaveSystem.LoadGame();
        if (data == null)
            InitSave();
        else
        {
            _GameData.levelDats = data.levelDats;
            _GameData.TotalStarCount = data.TotalStarCount;
        }
    }

    public void MasterLevelUnlock()
    {
        _GameData.TotalStarCount = 9001;
        SaveSystem.SaveGame(_GameData);
    }

    public void InitSave()
    {
        _GameData.TotalStarCount = 0;
        SaveSystem.SaveGame(_GameData);
    }

    public void DisableIntro()
    {
        SaveSystem.SaveGame(_GameData);
    }
}
