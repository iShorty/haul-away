


[System.Serializable]
public class LevelData
{
    public string levelID;
    // Each one is unique, can't just load 1 2 or 3 stars like before
    public bool[] stars = new bool[3];
    public int starCount => StarCount();

    int StarCount()
    {
        int count = 0;

        foreach (bool star in stars)
        {
            if (star)
                count++;
        }
        return count;
    }

    public LevelData(bool[] stars)
    {
        this.stars = stars;
        levelID = TransitionManager.CurrentSceneName;
    }

    public LevelData(bool[] stars,string sceneName)
    {
        this.stars = stars;
        levelID = sceneName;
    }

#if UNITY_EDITOR
    public string LogValues()
    {
        string log = "";
        log = $"LevelID: {levelID} \n";
        for (int i = 0; i < stars.Length; i++)
        {
            log+= $"\n stars[${i}]: {stars[i]}";
        }
        return log;
    }
#endif
}