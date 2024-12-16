using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int currentSceneCoins = 300;


    #region GameObjects Lists
    [Header("Game Objects Lists")]
    public static List<GameObject> currentActiveMainFishObjectsList = new();
    public static List<GameObject> currentActiveFoodTargetObjectsList = new();
    public static List<GameObject> currentActiveEnemyObjectsList = new();
    public static List<GameObject> activeCollectablesList = new();
    private List<GameObject> selectedSpecialPets = new(3);

    public List<GameObject> spawnableEnemyPrefabsList = new();
    #endregion


    #region Data types References

    #region Scriptable Objects References
    public GameData transferableGameData;
    #endregion

    #region Game Objects References
    [Header("Prefabs")]
    public GameObject followerPrefab;
    public GameObject foodPrefab;
    public GameObject enemy_FoodEaterPrefab;
    public GameObject enemy_ClorpPrefab;
    #endregion

    #region Scripts References
    private BoundsAndPositioningManager positioningManager;
    #endregion
    #endregion


    #region Spawn Objects Managers
    [Header("Spawn Object")]
    [SerializeField]
    private Vector3 clampedSpawnPosition;

    private float enemySpawnDelay = 2f;

    // This is the delay value between clicks or touches, <<<<<<<< and will be replaced later with a value from a scriptable object >>>>>>>>
    const float spawnDelay = 0.001f;
    #endregion


    #region Upgradables
    [Header("Upgradable Objects")]
    public FoodProperties[] foodTypes;
    public WeaponProperties[] weaponTypes = new WeaponProperties[10];



    #region Upgrades and Costs managers
    private const int followerInstantiateCost = 100;
    private const int foodUpgradeCost = 300;
    private int eggUpgradeCost = 1000;

    private int currentFoodIndex = 0;
    private int currentWeaponIndex = 0;
    private int currentEggIndex = 0;
    #endregion
    #endregion


    private bool isTypeOfFoodEater = false; // New flag to track whether the current enemy is a Food Eater
    private int maxEggLevel = 2; // Maximum level of the egg


    void Start()
    {
        ClearStaticLists();

        currentSceneCoins = 999999990;
        GameEvents.EventsChannelInstance.UpdateCurrentSceneCoins(currentSceneCoins);
        GameEvents.EventsChannelInstance.RefreshEggCost(eggUpgradeCost);


        positioningManager = GetComponent<BoundsAndPositioningManager>();

        StartCoroutine(HandleClicksAndTouches());

        // Enemy Spawner
        //StartCoroutine(SpawnEnemy());

        SpawnObject(1);
        SpawnPets();

    }


    #region Input Handling
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
                    RefreshInSceneMoney(hit);
                    yield return new WaitForSeconds(.05f);
                }
                // Or is an enemy object
                else if (currentActiveEnemyObjectsList.Count > 0)
                {
                    // Allow spawning food anywhere
                    if (isTypeOfFoodEater)
                    {
                        SpawnObject(2);
                        yield return new WaitForSeconds(spawnDelay);
                    }
                    // Or damage the enemy
                    else
                    {
                        Health decreaseHealth = hit.collider?.gameObject.GetComponent<Health>();
                        decreaseHealth?.TakeDamage(weaponTypes[currentWeaponIndex].weaponDamage);
                        yield return new WaitForSeconds(weaponTypes[currentWeaponIndex].fireDelay);
                    }
                }
                // OR Spawn a food object
                else
                {
                    SpawnObject(2);
                    yield return new WaitForSeconds(spawnDelay);
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
                if (currentSceneCoins >= followerInstantiateCost)
                {
                    // Get a random position depending on the camera viewport
                    clampedSpawnPosition = positioningManager.GetNewRandomPosition();
                    _spawnedObject = Instantiate(followerPrefab, clampedSpawnPosition, Quaternion.identity);

                    currentActiveMainFishObjectsList.Add(_spawnedObject);
                    currentSceneCoins -= followerInstantiateCost;
                    GameEvents.EventsChannelInstance.RefresheMainFishesNumber(currentActiveMainFishObjectsList.Count);
                }
                break;
            // Spawn an instance of Food prefab
            case 2:
                if (currentSceneCoins >= foodTypes[currentFoodIndex].foodCost || currentActiveEnemyObjectsList.Count > 0)
                {
                    _spawnedObject = Instantiate(foodPrefab, clampedSpawnPosition, Quaternion.identity);
                    _spawnedObject.GetComponent<Food>().foodConfig = foodTypes[currentFoodIndex];

                    currentActiveFoodTargetObjectsList.Add(_spawnedObject);
                    if (currentActiveEnemyObjectsList.Count == 0)
                    {
                        currentSceneCoins -= foodTypes[currentFoodIndex].foodCost;
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
                yield return new WaitForSeconds(enemySpawnDelay);
                clampedSpawnPosition = positioningManager.GetNewRandomPosition();

                GameObject selectRandomEnemyPrefab = spawnableEnemyPrefabsList[Random.Range(
                        0,
                        spawnableEnemyPrefabsList.Count
                        )];
                //GameObject tempEnemyInstance = enemyObjectsList[1];


                GameObject enemyInstance = Instantiate(
                    selectRandomEnemyPrefab,
                    clampedSpawnPosition,
                    Quaternion.identity
                    );


                currentActiveEnemyObjectsList.Add(enemyInstance);

                CheckEnemyType(enemyInstance);
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
                clampedSpawnPosition = positioningManager.GetNewRandomPosition();
                clampedSpawnPosition.y = 0.5f;

                GameObject petInstance = Instantiate(
                    transferableGameData.selectedSpecialPetsList[i],
                    clampedSpawnPosition,
                    Quaternion.identity
                    );

                Debug.Log($"Pet {transferableGameData.selectedSpecialPetsList[i].name} has been spawned");
            }
        }
    }
    #endregion


    #region Utility Functions
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


    // A function that refreshes the in-scene money after collecting a collectable object
    private void RefreshInSceneMoney(RaycastHit hit)
    {
        currentSceneCoins += hit.collider.gameObject.GetComponent<Collectable>().collectableConfig.collectableValue;
        GameEvents.EventsChannelInstance.UpdateCurrentSceneCoins(currentSceneCoins);
        Destroy(hit.collider.gameObject);
    }

    // A function that adds coins to the in-scene money
    public static void AddCoins(int coins)
    {
        currentSceneCoins += coins;
        GameEvents.EventsChannelInstance.UpdateCurrentSceneCoins(currentSceneCoins);
    }

    // A function that upgrades the food type on request
    private void UpgradeFood()
    {
        if (currentSceneCoins >= foodUpgradeCost && currentFoodIndex < foodTypes.Length - 1)
        {
            // Deduct the cost of the food upgrade from the in-scene money
            currentFoodIndex = ( currentFoodIndex + 1 ) % foodTypes.Length;
            currentSceneCoins -= foodUpgradeCost;
            GameEvents.EventsChannelInstance.UpdateCurrentSceneCoins(currentSceneCoins);
        }
    }

    // A function that upgrades the weapon type on request
    private void UpgradeWeapon()
    {
        if (currentSceneCoins - weaponTypes[currentWeaponIndex].weaponCost >= 0 && currentWeaponIndex < weaponTypes.Length - 1)
        {
            currentWeaponIndex = ( currentWeaponIndex + 1 ) % weaponTypes.Length;
            currentSceneCoins -= weaponTypes[currentWeaponIndex].weaponCost;
            Debug.Log("Current Weapon Index: " + currentWeaponIndex + " Weapon Cost: " + weaponTypes[currentWeaponIndex].weaponCost + " " + weaponTypes[currentWeaponIndex].name);
            GameEvents.EventsChannelInstance.UpdateCurrentSceneCoins(currentSceneCoins);
        }
    }

    // A function that upgrades the egg type on request
    private void UpgradeEgg()
    {
        if (currentEggIndex < maxEggLevel && currentSceneCoins >= eggUpgradeCost)
        {
            PerformEggUpgrade();
        }
        else if (currentEggIndex == maxEggLevel)
        {
            AdvanceToNextLevel();
        }
    }


    private void PerformEggUpgrade()
    {
        currentSceneCoins -= eggUpgradeCost;
        eggUpgradeCost *= 2; // Double the cost of the next upgrade
        currentEggIndex++; // Increase the level of the egg
        GameEvents.EventsChannelInstance.RefreshEggCost(eggUpgradeCost);
        GameEvents.EventsChannelInstance.UpdateCurrentSceneCoins(currentSceneCoins);
    }

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

    private void LoadNextScene(GameSaveDataContainer gameSaveDataContainer)
    {
        ClearStaticLists();


        string selectNextScene = gameSaveDataContainer.unlockedLevelsList.Count > 3 ?
                           "SpecialAnimalsSelectionScene" :
                           "Level " + transferableGameData.selectedLevel;
        SceneManager.LoadScene(selectNextScene);
    }

    private void ClearStaticLists()
    {
        // Clear the lists of the current active objects in a current scene
        currentActiveMainFishObjectsList.Clear();
        currentActiveFoodTargetObjectsList.Clear();
        currentActiveEnemyObjectsList.Clear();
        activeCollectablesList.Clear();
    }

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
