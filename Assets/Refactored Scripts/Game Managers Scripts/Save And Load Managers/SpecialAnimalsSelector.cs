using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpecialAnimalsSelector : MonoBehaviour
{
    private const int maxSelectedSpecialAnimalsNumber = 3;  // Maximum number of special animals that can be selected

    [Header("Assign Game Data ScriptableObject")]
    public GameData transferableGameData;  // Reference to the GameData ScriptableObject

    [Header("Assign All Toggles")]
    public List<Toggle> petToggles;  // List of all toggles associated with the animals

    private List<GameObject> selectedSpecialPets = new List<GameObject>(3); // Store selected pets as GameObjects

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
        SpecialAnimalToggle specialPetToggle = toggle.GetComponent<SpecialAnimalToggle>();
        // Get the special animal from the custom component
        GameObject specialPet = specialPetToggle.specialPet;

        if (toggle.isOn &&
            selectedSpecialPets.Count < maxSelectedSpecialAnimalsNumber &&
            !selectedSpecialPets.Contains(specialPet))
        {
            // Add the selected animal to the list
            selectedSpecialPets.Add(specialPetToggle.specialPet);
        }
        else
        {
            // Remove the deselected animal from the list
            selectedSpecialPets.Remove(specialPetToggle.specialPet);
        }

        UpdateToggleInteractivity();
    }

    private void UpdateToggleInteractivity()
    {
        // Limit the number of selected animals to 3 {which is the maximum number of special animals that can be selected = maxSpecialAnimals}
        bool limitReached = selectedSpecialPets.Count >= maxSelectedSpecialAnimalsNumber;


        foreach (Toggle toggle in petToggles)
        {
            // Get the special animal from the toggle's custom component
            SpecialAnimalToggle specialPetToggle = toggle.GetComponent<SpecialAnimalToggle>();

            // // GameObject specialAnimalID = specialAnimalToggle.specialAnimal;
            int toggleID = specialPetToggle.toggleID;


            if (savedDataContainer.unlockedLevelsList.Contains(toggleID))
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
        transferableGameData.selectedSpecialPetsList = new(selectedSpecialPets);  // Store selected animals

        SceneManager.LoadScene($"Level {transferableGameData.selectedLevel}");  // Load the selected level
    }
}
