using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelSelector : MonoBehaviour
{
    private GameSaveDataContainer savedDataContainer;  // Reference to the GameSaveData class


    [Header("Assign Game Data ScriptableObject")]
    public GameData transferableGameData;  // Reference to the GameData ScriptableObject

    [Header("Assign All Toggles")]
    public List<Toggle> levelToggles;  // List of all toggles associated with the animals


    private void Awake()
    {
        transferableGameData.selectedLevel = 1;

        // Load the saved data
        if (SaveLoadManager.LoadData() != null)
        {
            savedDataContainer = SaveLoadManager.LoadData();
        }
        // If there is no saved data, create a new one
        else
        {
            // Create a new saved data container
            savedDataContainer = new GameSaveDataContainer();
            // Add the first level to the unlocked levels list
            savedDataContainer.unlockedLevelsList.Add(transferableGameData.selectedLevel);

            // Save the data
            SaveLoadManager.SaveData(savedDataContainer.unlockedLevelsList);
        }

        UpdateToggleInteractivity();
    }


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


    #region Toggle Selection Functionality
    // This function is called when a toggle is selected in the UI
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


    [System.Obsolete]
    void SelectRefreshRate()
    {
        int refreshRate = Screen.currentResolution.refreshRate;

        if (refreshRate >= 120)
        {
            Application.targetFrameRate = 120;
        }
        else if (refreshRate >= 90)
        {
            Application.targetFrameRate = 90;
        }
        else
        {
            Application.targetFrameRate = 60;
        }
    }
}
