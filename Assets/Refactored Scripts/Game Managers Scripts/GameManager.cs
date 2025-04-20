using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

#region GameManager's Required Components
[RequireComponent(typeof(BoundsAndPositioningManager))]
#endregion
public class GameManager : MonoBehaviour
{
    /* Note: This region contains static lists, read the note below:
     * 
     * Whenever you implement a new static list in the GameManager,
     * make sure to add it to the ClearStaticLists() function to avoid memory leaks,
     * when the scene is changed or the game is closed.
     */
    #region Current Active Objects Lists

    [Header("Current Active Objects Lists")]
    // This list contains the current active Main Fish objects in the scene 
    public static List<GameObject> currentActiveMainFishObjectsList = new();

    // This list contains all current active and distractible objects in the scene
    public static List<GameObject> currentActiveDistractibleObjectsList = new();

    public static List<GameObject> currentActiveFoodTargetObjectsList = new();
    public static List<GameObject> currentActiveCollectiblesList = new();

    public static List<GameObject> currentActiveEnemyObjectsList = new();


    /* Dictionary to store the protective pets and their types
     * 
     * cAPPetsDictionary => cAP => stands for Current Active Protective Pets
     */
    public static Dictionary<PetType, GameObject> cAPPetsDictionary = new();
    #endregion


    #region Data types References
    [Tooltip("Assign here the level data (Scriptable Object)")]
    public LevelDataSettingsScript levelData;

    [Tooltip("Assign here the transferable game data (Scriptable Object)")]
    public GameData transferableGameData;
    #endregion


    #region Scripts References
    private BoundsAndPositioningManager positioningManager;
    #endregion


    #region Spawn Objects Managers
    private Vector3 clampedSpawnPosition;
    #endregion


    #region Upgrading Indexes
    private int currentFoodIndex = 0;
    private int currentWeaponIndex = 0;
    private int currentEggIndex = 0;
    #endregion


    #region Utility Variables
    private static int currentSceneCoins;

    private bool isTypeOfFoodEater = false; // New flag to track whether the current enemy is a Food Eater

    #region Pets Utility Variables

    #region WTW Pet protection variables
    public static bool canBeProtectedByWTWPet = false; // A flag to check if the Main-Fish objects can be protected by the WTW pet.
    public static float fishesProtectionDuration { get; private set; } // The duration that the WTW pet can protect the Main-Fish objects after an enemy object is spawned.
    #endregion

    #region NTMR Pet Unique Ability
    public static bool NTMRpetUniqueAbility = false; // A flag to check if the NTMR pet is active and has a unique ability.
    #endregion


    #endregion
    #endregion


    void Start()
    {
        ClearStaticLists();

        currentSceneCoins = levelData.initialAvailableCoins;
        fishesProtectionDuration = levelData.fishesProtectionDuration;

        GameEvents.EventsChannelInstance.UpdateCurrentSceneCoins(currentSceneCoins);
        GameEvents.EventsChannelInstance.RefreshEggCost(levelData.eggUpgradeCostList[currentEggIndex]);


        positioningManager = GetComponent<BoundsAndPositioningManager>();

        StartCoroutine(HandleClicksAndTouches());

        // Enemy Spawner
        //StartCoroutine(SpawnEnemy());

        // Spawn 1000 Main Fishes
        //for (int i = 0; i < 1000; i++)
        //{
        //    SpawnObject(1);
        //}


        SpawnPets();

        SpawnObject(1);
    }


    #region Input Handling

    /* Older Method:
    
    IEnumerator HandleClicksAndTouches()
    {
        while (true)
        {
            Vector3 inputPosition = Vector3.zero;
            bool isInputActive = false;

            // Check if the player is using a mouse or a touch
            if (Input.GetMouseButton(0))
            {
                // Get the mouse position
                inputPosition = Input.mousePosition;
                isInputActive = true;
            }
            else if (Input.touchCount > 0)
            {
                // Get the first touch only if there are multiple touches and get the touch position
                Touch touch = Input.GetTouch(0);
                inputPosition = touch.position;
                isInputActive = true;
            }


            // quick check to see if the input is active
            if (isInputActive)
            {
                if (EventSystem.current.IsPointerOverGameObject() || ( Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) ))
                {
                    yield return new WaitForSeconds(.1f);
                    continue; // Skip further processing if over a UI element
                }

                // Convert the input position to a world position
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);
                // Clamp the world position to be within the camera view
                clampedSpawnPosition = positioningManager.ClampPositionWithInView(worldPosition);


                Ray ray = Camera.main.ScreenPointToRay(inputPosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit, 21f);

                // Is a collectable object
                if (hit.collider != null && hit.collider.gameObject.layer == 8)
                {
                    UpdateSceneCoins(hit);
                    yield return new WaitForSeconds(.05f);
                }
                // Or is an enemy object
                else if (currentActiveEnemyObjectsList.Count > 0)
                {
                    // Allow spawning food anywhere
                    if (isTypeOfFoodEater)
                    {
                        SpawnObject(2);
                        yield return new WaitForSeconds(levelData.foodSpawnDelay);
                    }
                    // Or damage the enemy
                    else
                    {
                        Health decreaseHealth = hit.collider?.gameObject.GetComponent<Health>();
                        decreaseHealth?.TakeDamage(levelData.weaponTypes[currentWeaponIndex].weaponDamage);
                        yield return new WaitForSeconds(levelData.weaponTypes[currentWeaponIndex].fireDelay);
                    }
                }
                // OR Spawn a food object
                else
                {
                    SpawnObject(2);
                    yield return new WaitForSeconds(levelData.foodSpawnDelay);
                }
            }
            yield return null;
        }
    }
    */


    // This method is optimized to handle the inputs depending on the platform
    IEnumerator HandleClicksAndTouches()
    {
        while (true)
        {
            Vector3 inputPosition = Vector3.zero;
            bool isInputActive = false;

#if UNITY_STANDALONE || UNITY_EDITOR
            // Handle Mouse Input for Standalone (Windows/Mac/Linux) and Unity Editor
            if (Input.GetMouseButton(0))
            {
                inputPosition = Input.mousePosition;
                isInputActive = true;
            }
#elif UNITY_ANDROID || UNITY_IOS
        // Handle Touch Input for Mobile Devices
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputPosition = touch.position;
            isInputActive = true;
        }
#endif

            // Check if input is active
            if (isInputActive)
            {
                // Prevent interaction if tapping on UI
                if (
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                    EventSystem.current.IsPointerOverGameObject()
#elif UNITY_ANDROID || UNITY_IOS
                (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#endif
                    )
                {
                    yield return new WaitForSeconds(.1f);
                    continue;
                }

                // Convert the input position to world coordinates
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);
                clampedSpawnPosition = positioningManager.ClampPositionWithInView(worldPosition);

                Ray ray = Camera.main.ScreenPointToRay(inputPosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit, 21f);

                // Check what was clicked/tapped
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.tag == "Collectible") // Collectibles
                    {
                        UpdateSceneCoins(hit);
                        yield return new WaitForSeconds(0.05f);
                    }
                    else if (currentActiveEnemyObjectsList.Count > 0) // If there are enemies
                    {
                        if (isTypeOfFoodEater)
                        {
                            SpawnObject(2);
                            yield return new WaitForSeconds(levelData.foodSpawnDelay);
                        }
                        else
                        {
                            Health decreaseHealth = hit.collider?.gameObject.GetComponent<Health>();
                            decreaseHealth?.TakeDamage(levelData.weaponTypes[currentWeaponIndex].weaponDamage);
                            yield return new WaitForSeconds(levelData.weaponTypes[currentWeaponIndex].fireDelay);
                        }
                    }
                    else if (hit.collider.gameObject.tag == "Amp")
                    {
                        hit.collider.gameObject.GetComponent<ATEEPetControllerScript>().CurrentClickIndex++;

                        yield return new WaitForSeconds(1f);
                    }
                    else
                    {
                        // Spawn food normally
                        SpawnObject(2);
                        yield return new WaitForSeconds(levelData.foodSpawnDelay);
                    }
                }
            }
            yield return null;
        }
    }


    #endregion


    #region Spawning Mnagers
    // A function that will be called to spawn an object on request
    public void SpawnObject(int objectType)
    {
        GameObject _spawnedObject = null;

        // Determine which object type to spawn
        switch (objectType)
        {
            // Spawn an instance of Main Fish prefab
            case 1:
                if (currentSceneCoins >= levelData.primaryFishPurchaseCost)
                {
                    // Get a random position depending on the camera viewport
                    clampedSpawnPosition = positioningManager.GenerateRandomClampedPosition();
                    _spawnedObject = Instantiate(levelData.primaryFishPrefab, clampedSpawnPosition, Quaternion.identity);

                    currentSceneCoins -= levelData.primaryFishPurchaseCost;
                    GameEvents.EventsChannelInstance.RefresheMainFishesNumber(currentActiveMainFishObjectsList.Count);
                }
                break;
            // Spawn an instance of Food prefab
            case 2:
                if (currentSceneCoins >= levelData.foodQualityTypes[currentFoodIndex].foodCost || currentActiveEnemyObjectsList.Count > 0)
                {
                    _spawnedObject = Instantiate(levelData.foodPrefab, clampedSpawnPosition, Quaternion.identity);
                    _spawnedObject.GetComponent<Food>().foodConfig = levelData.foodQualityTypes[currentFoodIndex];

                    //currentActiveFoodTargetObjectsList.Add(_spawnedObject);
                    if (currentActiveEnemyObjectsList.Count == 0)
                    {
                        currentSceneCoins -= levelData.foodQualityTypes[currentFoodIndex].foodCost;
                        break;
                    }
                }
                break;
        }
        if (_spawnedObject != null) // If the object is spawned, update the in-scene money
        {
            GameEvents.EventsChannelInstance.UpdateCurrentSceneCoins(currentSceneCoins);
        }
    }


    // A function that will be called to spawn an enemy automatically
    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            if (currentActiveEnemyObjectsList.Count == 0)
            {
                yield return new WaitForSeconds(Random.Range(
                        levelData.minEnemySpawnDelay,
                        levelData.maxEnemySpawnDelay
                        ));

                clampedSpawnPosition = positioningManager.GenerateRandomClampedPosition();

                GameObject selectRandomEnemyPrefab =
                    levelData.spawnableEnemyPrefabsList[Random.Range(
                            0,
                            levelData.spawnableEnemyPrefabsList.Count
                            )];
                //GameObject tempEnemyInstance = enemyObjectsList[1];


                GameObject enemyInstance = Instantiate(
                    selectRandomEnemyPrefab,
                    clampedSpawnPosition,
                    Quaternion.identity
                    );


                currentActiveEnemyObjectsList.Add(enemyInstance);

                CheckEnemyType(enemyInstance);
                canBeProtectedByWTWPet = true;
            }
            yield return null;
        }
    }

    void SpawnPets()
    {
        if (transferableGameData.selectedSpecialPetsList.Count != 0)
        {
            for (int i = 0; i < transferableGameData.selectedSpecialPetsList.Count; i++)
            {
                clampedSpawnPosition = positioningManager.GenerateRandomClampedPosition();
                clampedSpawnPosition.y = 0.5f;

                GameObject petInstance = Instantiate(
                    transferableGameData.selectedSpecialPetsList[i],
                    clampedSpawnPosition,
                    Quaternion.identity
                    );
            }
        }
    }
    #endregion


    #region Utility Functions

    #region Enemy Utility Functions
    // A function that checks the type of the spawned enemy object
    private void CheckEnemyType(GameObject enemy)
    {
        EnemyType enemyType = enemy.GetComponent<EnemyController>().enemyType;

        if (enemyType == EnemyType.FishesAndFoodsEater)
        {
            isTypeOfFoodEater = true;
        }
        else
        {
            isTypeOfFoodEater = false;
        }
    }

    #endregion


    #region Scene Coins Utility Functions
    // A function that refreshes the in-scene money after collecting a collectable object
    private void UpdateSceneCoins(RaycastHit hit)
    {
        currentSceneCoins += hit.collider.gameObject.GetComponent<CollectibleScript>().collectibleProperties.collectableValue;
        GameEvents.EventsChannelInstance.UpdateCurrentSceneCoins(currentSceneCoins);
        Destroy(hit.collider.gameObject);
    }

    #endregion


    #region Upgrading Functions
    // A function that upgrades the food type on request
    private void UpgradeFood()
    {
        if (currentFoodIndex < levelData.foodQualityTypes.Length - 1
            && currentSceneCoins >= levelData.foodQualityUpgradeCostsList[currentFoodIndex])
        {
            // Deduct the cost of the food upgrade from the in-scene money
            currentFoodIndex = ( currentFoodIndex + 1 ) % levelData.foodQualityTypes.Length;
            currentSceneCoins -= levelData.foodQualityUpgradeCostsList[currentFoodIndex];
            GameEvents.EventsChannelInstance.UpdateCurrentSceneCoins(currentSceneCoins);
        }
    }

    // A function that upgrades the weapon type on request
    private void UpgradeWeapon()
    {
        if (currentSceneCoins - levelData.weaponTypes[currentWeaponIndex].weaponCost >= 0 && currentWeaponIndex < levelData.weaponTypes.Length - 1)
        {
            currentWeaponIndex = ( currentWeaponIndex + 1 ) % levelData.weaponTypes.Length;
            currentSceneCoins -= levelData.weaponTypes[currentWeaponIndex].weaponCost;
            Debug.Log("Current Weapon Index: " + currentWeaponIndex + " Weapon Cost: " + levelData.weaponTypes[currentWeaponIndex].weaponCost + " " + levelData.weaponTypes[currentWeaponIndex].name);
            GameEvents.EventsChannelInstance.UpdateCurrentSceneCoins(currentSceneCoins);
        }
    }

    // A function that upgrades the egg type on request
    private void UpgradeEgg()
    {
        if (currentEggIndex == levelData.eggUpgradeCostList.Count - 1
            && currentSceneCoins >= levelData.eggUpgradeCostList[currentEggIndex])
        {
            AdvanceToNextLevel();
            Debug.Log(currentSceneCoins);
        }
        else if (currentSceneCoins >= levelData.eggUpgradeCostList[currentEggIndex])
        {
            PerformEggUpgrade();
        }
    }

    #endregion


    #region Level Utility Functions
    // A function that performs the egg upgrade
    private void PerformEggUpgrade()
    {
        currentSceneCoins -= levelData.eggUpgradeCostList[currentEggIndex];
        currentEggIndex++; // Increase the level of the egg
        GameEvents.EventsChannelInstance.RefreshEggCost(levelData.eggUpgradeCostList[currentEggIndex]);
        GameEvents.EventsChannelInstance.UpdateCurrentSceneCoins(currentSceneCoins);
    }

    // A function that advances to the next level
    private void AdvanceToNextLevel()
    {
        GameSaveDataContainer gameSaveDataContainer = SaveLoadManager.LoadData();
        transferableGameData.selectedLevel++;

        if (!gameSaveDataContainer.unlockedLevelsList.Contains(transferableGameData.selectedLevel))
        {
            gameSaveDataContainer.unlockedLevelsList.Add(transferableGameData.selectedLevel);
            SaveLoadManager.SaveData(gameSaveDataContainer.unlockedLevelsList);
        }

        LoadNextScene(gameSaveDataContainer);
    }

    // A function that loads the next scene
    private void LoadNextScene(GameSaveDataContainer gameSaveDataContainer)
    {
        ClearStaticLists();


        string selectNextScene = gameSaveDataContainer.unlockedLevelsList.Count > 3 ?
                           "SpecialAnimalsSelectionScene" :
                           "Level " + transferableGameData.selectedLevel;
        SceneManager.LoadScene(selectNextScene);
    }

    // A function that clears the static lists of the current active objects in a current scene
    private void ClearStaticLists()
    {
        // Clear the lists of the current active objects in a current scene
        currentActiveMainFishObjectsList.Clear();
        currentActiveFoodTargetObjectsList.Clear();
        currentActiveEnemyObjectsList.Clear();
        currentActiveCollectiblesList.Clear();

        cAPPetsDictionary.Clear();
    }

    #endregion


    #region Pets Utility Functions
    public static PetType IdentifyProtectivePet()
    {
        /* Read the following note when you add new protective pet types
         * 
         * The following random movement strategy is a temporary solution
         * because there is no other protective pet types created yet.
         * 
         * Once you add new protective pet types,
         * you should create or use the appropriate movement strategy.
         */

        PetType protectivePetObject = PetType.None;

        // Find the best protective pet object.
        foreach (KeyValuePair<PetType, GameObject> protectivePet in cAPPetsDictionary)
        {
            if (protectivePet.Value.activeSelf)
            {
                if (cAPPetsDictionary.ContainsKey(PetType.WTWPet) && canBeProtectedByWTWPet == true)
                {
                    protectivePetObject = protectivePet.Key;
                    break;
                }
                else if (protectivePet.Key == PetType.GTAPet)
                {
                    protectivePetObject = protectivePet.Key;
                    break;
                }
                else
                {
                    break;
                }
            }
        }
        return protectivePetObject;
    }

    #endregion


    #region Static Utility Functions
    // A function that adds coins to the in-scene money
    public static void AddCoins(int coins)
    {
        currentSceneCoins += coins;
        GameEvents.EventsChannelInstance.UpdateCurrentSceneCoins(currentSceneCoins);
    }

    #endregion

    #endregion


    #region Event Subscriptions
    private void OnEnable()
    {
        GameEvents.EventsChannelInstance.OnSpawnObject += SpawnObject;

        GameEvents.EventsChannelInstance.OnUpgradeFood += UpgradeFood;
        GameEvents.EventsChannelInstance.OnUpgradeWeapon += UpgradeWeapon;
        GameEvents.EventsChannelInstance.OnUpgradeEgg += UpgradeEgg;
    }

    private void OnDisable()
    {
        GameEvents.EventsChannelInstance.OnSpawnObject -= SpawnObject;

        GameEvents.EventsChannelInstance.OnUpgradeFood -= UpgradeFood;
        GameEvents.EventsChannelInstance.OnUpgradeWeapon -= UpgradeWeapon;
        GameEvents.EventsChannelInstance.OnUpgradeEgg -= UpgradeEgg;
    }
    #endregion
}