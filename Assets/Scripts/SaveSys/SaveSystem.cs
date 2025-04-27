using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AudioManagement;

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/hauledData1.data";

    public static void SaveGame(GameData data)
    {
#if UNITY_EDITOR
        Debug.Assert(data != null, "Why is data null!?!?");
#endif
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, data);
        }

#if UNITY_EDITOR
        Debug.Log($"Data has been saved at {path}");
        data.LogData();
#endif
    }

    ///<Summary>Loads a game data by checking if the save path exists. If so, this method returns that data else, it will return a new GameData and create the save path.</Summary>
    public static GameData LoadGame()
    {
        GameData data = default;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                if (stream.Length > 0)
                    data = formatter.Deserialize(stream) as GameData;
            }

#if UNITY_EDITOR
            Debug.Assert(data != null, $"Why is the data which was loaded from {path} null??");
            data.LogData();
#endif
        }
        else
        {
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }

            data = new GameData();
            SaveGame(data);
        }
        return data;
    }

    public static void SaveLevel(LevelData levelData)
    {
        GameData gameData = LoadGame();

        // if (gameData.levelAccess <= SceneMaster.CurrScene)
        //     gameData.levelAccess = SceneMaster.CurrScene + 1;

        #region --------- Saving Level Data --------------

        //If this game data already has level datas
        if (gameData.levelDats.Count > 0)
        {
            LevelData saveFileLvlData = gameData.levelDats.Find(lvlData => lvlData.levelID == levelData.levelID);

            // If we have matching level data,
            if (saveFileLvlData != null)
            {
                // Update gameData so it takes the completed objective
                for (int j = 0; j < levelData.stars.Length; j++)
                {
                    if (saveFileLvlData.stars[j] == false && levelData.stars[j] == true)
                    {
                        saveFileLvlData.stars[j] = true;
                    }
                }
            }
            else
            {
                // If no level data for that level, add it
                gameData.levelDats.Add(levelData);
            }


            // Save total starcount, used as level access value
            int currentLevelAccess = 0;
            for (int i = 0; i < gameData.levelDats.Count; i++)
            {
                currentLevelAccess += gameData.levelDats[i].starCount;
            }
            
            if (gameData.TotalStarCount != 999)
            {
                gameData.TotalStarCount = currentLevelAccess;
            }

        }
        // If no level data, add this
        else
        {
            gameData.levelDats.Add(levelData);
        }
        #endregion

        // #region ------------- Saving Audio Volumes --------------
        // AudioChannelManager.SaveVolumeData(gameData);
        // // // Save volume levels UPDATE ME TO USE PROPER VOLUME SOURCE VALUES
        // // gameData.MasterVol = channelManager.MasterVolume.GetCurrentValue();
        // // gameData.SFXVol = channelManager.SFXVolume.GetCurrentValue();
        // // gameData.BGMVol = channelManager.BGMVolume.GetCurrentValue();
        // #endregion

        SaveGame(gameData);
    }

    public static void DeleteAllSaveFiles()
    {
        PlayerPrefs.DeleteAll();
        File.Delete(path);
    }
}
