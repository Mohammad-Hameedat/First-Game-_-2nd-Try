using System.Collections.Generic;


[System.Serializable]
public class GameSaveDataContainer
{
    // Store unlocked levels by level number
    public List<int> unlockedLevelsList;


    public GameSaveDataContainer()
    {
        unlockedLevelsList = new List<int>();
    }
}
