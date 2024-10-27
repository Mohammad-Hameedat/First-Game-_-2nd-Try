using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelSelector : MonoBehaviour
{
    [Header("Assign Game Data ScriptableObject")]
    public GameData transferableGameData;  // Reference to the GameData ScriptableObject

    [Header("Assign All Toggles")]
    public List<Toggle> levelToggles;  // List of all toggles associated with the animals

    private GameSaveDataContainer savedDataContainer;  // Reference to the GameSaveData class


    private void Awake()
    {
        transferableGameData.selectedLevel = 1;

        if (SaveLoadManager.LoadData() != null)
        {
            savedDataContainer = SaveLoadManager.LoadData();
        }
        else
        {
            savedDataContainer = new GameSaveDataContainer();
            savedDataContainer.unlockedLevelsList.Add(1);

            SaveLoadManager.SaveData(savedDataContainer.unlockedLevelsList);
        }

        UpdateToggleInteractivity();
    }

    #region Toggle Selection Functionality
    public void OnLevelToggleSelected(Toggle toggle)
    {
        if (toggle.isOn)
        {
            LevelToggle levelToggle = toggle.GetComponent<LevelToggle>();

            // Store the selected level in the GameData ScriptableObject
            transferableGameData.selectedLevel = levelToggle.levelNumber;


            if (savedDataContainer.unlockedLevelsList.Count > 3)
            {
                SceneManager.LoadScene("SpecialAnimalsSelectionScene");
            }
            else
            {
                SceneManager.LoadScene("Level " + transferableGameData.selectedLevel);
            }

        }
    }
    #endregion


    #region Update Toggle Interactivity
    private void UpdateToggleInteractivity()
    {
        foreach (Toggle toggle in levelToggles)
        {
            // Get the custom component attached to the toggle TO get the level number associated with the toggle
            LevelToggle levelToggle = toggle.GetComponent<LevelToggle>();

            if (savedDataContainer.unlockedLevelsList.Contains(levelToggle.levelNumber))
            {
                toggle.interactable = true;
            }
            else
            {
                toggle.interactable = false;
            }
        }
    }
    #endregion

}
