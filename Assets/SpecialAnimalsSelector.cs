using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpecialAnimalsSelector : MonoBehaviour
{
    private int maxSelectedSpecialAnimalsNumber = 3;  // Maximum number of special animals that can be selected

    [Header("Assign Game Data ScriptableObject")]
    public GameData transferableGameData;  // Reference to the GameData ScriptableObject

    [Header("Assign All Toggles")]
    public List<Toggle> animalToggles;  // List of all toggles associated with the animals

    //private List<GameObject> selectedSpecialAnimals = new List<GameObject>();

    private List<int> selectedSpecialAnimalsIList = new List<int>(); // Store IDs instead of GameObjects

    private GameSaveDataContainer savedDataContainer;  // Reference to the GameSaveData class



    private void Awake()
    {
        // Load the saved data
        savedDataContainer = SaveLoadManager.LoadData();

        UpdateToggleInteractivity();
    }




    public void OnSpecialAnimalToggleSelected(Toggle toggle)
    {
        // Get the custom component attached to the toggle
        SpecialAnimalToggle specialAnimalToggle = toggle.GetComponent<SpecialAnimalToggle>();
        // Get the special animal from the custom component
        int specialAnimalID = specialAnimalToggle.specialAnimalID;

        if (toggle.isOn && selectedSpecialAnimalsIList.Count < maxSelectedSpecialAnimalsNumber && !selectedSpecialAnimalsIList.Contains(specialAnimalID))
        {
            selectedSpecialAnimalsIList.Add(specialAnimalID);  // Select Animal

            //Debug.Log("Selected Special Animal: " + specialAnimal.name);
        }
        else
        {
            selectedSpecialAnimalsIList.Remove(specialAnimalID);  // Deselect Animal
            //Debug.Log("Deselected Special Animal: " + specialAnimal.name);
        }

        UpdateToggleInteractivity();
    }

    private void UpdateToggleInteractivity()
    {
        // Limit the number of selected animals to 3 {which is the maximum number of special animals that can be selected = maxSpecialAnimals}
        bool limitReached = selectedSpecialAnimalsIList.Count >= maxSelectedSpecialAnimalsNumber;

        foreach (Toggle toggle in animalToggles)
        {
            // Get the special animal from the toggle's custom component
            SpecialAnimalToggle specialAnimalToggle = toggle.GetComponent<SpecialAnimalToggle>();

            // // // // // // GameObject specialAnimalID = specialAnimalToggle.specialAnimal;
            int specialAnimalID = specialAnimalToggle.specialAnimalID;

            if (savedDataContainer.unlockedLevelsList.Contains(specialAnimalID))
            {
                // If the animal is unlocked, only disable interaction if the selection limit is reached and the toggle is not already selected
                toggle.interactable = !limitReached || toggle.isOn;
            }
            else
            {
                // If the animal is locked, make it non-interactable and ensure it is unchecked
                toggle.SetIsOnWithoutNotify(false);
                toggle.interactable = false;
            }
        }
    }

    public void StartGame()
    {
        transferableGameData.selectedSpecialAnimalsIDs = new List<int>(selectedSpecialAnimalsIList);  // Store selected animals
        SceneManager.LoadScene("Level " + transferableGameData.selectedLevel);  // Load the selected level
    }
}
