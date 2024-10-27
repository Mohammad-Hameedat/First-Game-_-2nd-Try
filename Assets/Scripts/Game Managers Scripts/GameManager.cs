using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region GameObjects Lists
    [Header("Game Objects Lists")]
    public static List<GameObject> currentActiveMainFishObjectsList = new();
    public static List<GameObject> currentActiveFoodTargetObjectsList = new();
    public static List<GameObject> currentActiveEnemyObjectsList = new();
    //public static List<GameObject> collectablesObjectsList = new();

    public List<GameObject> enemyObjectsList = new();
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
    [SerializeField] int inSceneMoney = 300;
    Vector3 clampedSpawnPosition;

    // This is the delay value between clicks or touches, <<<<<<<< and will be replaced later with a value from a scriptable object >>>>>>>>
    const float spawnDelay = 0.001f;
    #endregion



    #region Upgradables
    [Header("Upgradable Objects")]
    public FoodProperties[] foodTypes;
    public WeaponProperties[] weaponTypes = new WeaponProperties[10];



    #region Upgrades and Costs managers
    const int followerInstantiateCost = 100;
    const int foodUpgradeCost = 300;
    int eggUpgradeCost = 1000;

    int currentFoodIndex = 0;
    int currentWeaponIndex = 0;
    int currentEggIndex = 0;
    #endregion
    #endregion

    private bool isTypeOfFoodEater = false; // New flag to track whether the current enemy is a Food Eater
    private int maxEggLevel = 2; // Maximum level of the egg

    void Start()
    {
        ClearTheStaticLists();

        inSceneMoney = 99999999;
        GameEvents.EventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);
        GameEvents.EventsChannelInstance.RefreshEggCost(eggUpgradeCost);


        positioningManager = GetComponent<BoundsAndPositioningManager>();

        StartCoroutine(HandleClicksAndTouches());

        // Enemy Spawner
        StartCoroutine(SpawnEnemy());

        SpawnObject(1);
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
                if (EventSystem.current.IsPointerOverGameObject() || (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))
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
                    HandleCollectableClick(hit);
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
                        ChildEnemyController enemyController = hit.collider.gameObject.GetComponent<ChildEnemyController>();
                        enemyController?.TakeDamage(weaponTypes[currentWeaponIndex].weaponDamage);
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


    private void HandleCollectableClick(RaycastHit hit)
    {
        inSceneMoney += hit.collider.gameObject.GetComponent<Collectable>().collectableConfig.collectableValue;
        GameEvents.EventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);
        Destroy(hit.collider.gameObject);
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
                if (inSceneMoney >= followerInstantiateCost)
                {
                    // Get a random position depending on the camera viewport
                    clampedSpawnPosition = positioningManager.GetNewRandomPosition();
                    _spawnedObject = Instantiate(followerPrefab, clampedSpawnPosition, Quaternion.identity);

                    currentActiveMainFishObjectsList.Add(_spawnedObject);
                    inSceneMoney -= followerInstantiateCost;
                    GameEvents.EventsChannelInstance.RefresheMainFishesNumber(currentActiveMainFishObjectsList.Count);
                }
                break;
            // Spawn an instance of Food prefab
            case 2:
                if (inSceneMoney >= foodTypes[currentFoodIndex].foodCost || currentActiveEnemyObjectsList.Count > 0)
                {
                    _spawnedObject = Instantiate(foodPrefab, clampedSpawnPosition, Quaternion.identity);
                    _spawnedObject.GetComponent<Food>().foodConfig = foodTypes[currentFoodIndex];

                    currentActiveFoodTargetObjectsList.Add(_spawnedObject);
                    if (currentActiveEnemyObjectsList.Count == 0)
                    {
                        inSceneMoney -= foodTypes[currentFoodIndex].foodCost;
                        break;
                    }
                }
                break;
        }
        if (_spawnedObject != null) // If the object is spawned, update the in-scene money
        {
            GameEvents.EventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);
        }
    }


    // A function that will be called to spawn an enemy automatically
    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            if (currentActiveEnemyObjectsList.Count == 0)
            {
                yield return new WaitForSeconds(3f);
                clampedSpawnPosition = positioningManager.GetNewRandomPosition();

                GameObject tempEnemyInstance = enemyObjectsList[Random.Range(0, enemyObjectsList.Count)];


                GameObject enemyInstance = Instantiate(tempEnemyInstance, clampedSpawnPosition, Quaternion.identity);
                //GameObject enemyInstance = Instantiate(enemy_FoodEaterPrefab, clampedSpawnPosition, Quaternion.identity);



                currentActiveEnemyObjectsList.Add(enemyInstance);

                CheckEnemyType(enemyInstance);
            }
            yield return null;
        }
    }
    #endregion


    #region Utility Functions
    // A function that checks the type of the spawned enemy object
    private void CheckEnemyType(GameObject enemy)
    {
        ChildEnemyController tempEnemyInstance = enemy.GetComponent<ChildEnemyController>();

        if (tempEnemyInstance.GetComponent<ChildEnemyController>() is ChildEnemyFoodEaterController)
        {
            isTypeOfFoodEater = true;
        }
        else
        {
            isTypeOfFoodEater = false;
        }
    }


    // A function that upgrades the food type on request
    void UpgradeFood()
    {
        if (inSceneMoney >= foodUpgradeCost && currentFoodIndex < foodTypes.Length - 1)
        {
            // Deduct the cost of the food upgrade from the in-scene money
            currentFoodIndex = (currentFoodIndex + 1) % foodTypes.Length;
            inSceneMoney -= foodUpgradeCost;
            GameEvents.EventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);
        }
    }


    // A function that upgrades the weapon type on request
    void UpgradeWeapon()
    {
        if (inSceneMoney >= weaponTypes[currentWeaponIndex].weaponCost && currentWeaponIndex < weaponTypes.Length - 1)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weaponTypes.Length;
            inSceneMoney -= weaponTypes[currentWeaponIndex].weaponCost;
            Debug.Log("Current Weapon Index: " + currentWeaponIndex + " Weapon Cost: " + weaponTypes[currentWeaponIndex].weaponCost + " " + weaponTypes[currentWeaponIndex].name);
            GameEvents.EventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);
        }
    }


    // A function that upgrades the egg type on request
    void UpgradeEgg()
    {
        if (currentEggIndex < maxEggLevel && inSceneMoney >= eggUpgradeCost)
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
        inSceneMoney -= eggUpgradeCost;
        eggUpgradeCost *= 2; // Double the cost of the next upgrade
        currentEggIndex++; // Increase the level of the egg
        GameEvents.EventsChannelInstance.RefreshEggCost(eggUpgradeCost);
        GameEvents.EventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);
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
        ClearTheStaticLists();


        string nextScene = gameSaveDataContainer.unlockedLevelsList.Count > 3 ?
                           "SpecialAnimalsSelectionScene" :
                           "Level " + transferableGameData.selectedLevel;
        SceneManager.LoadScene(nextScene);
    }

    private void ClearTheStaticLists()
    {
        // Clear the lists of the current active objects in a current scene
        currentActiveMainFishObjectsList.Clear();
        currentActiveFoodTargetObjectsList.Clear();
        currentActiveEnemyObjectsList.Clear();
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
