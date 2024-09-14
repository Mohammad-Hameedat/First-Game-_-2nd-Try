using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    #region GameObjects Lists
    public static List<GameObject> mainFishiesObjectsList = new List<GameObject>();
    public static List<GameObject> foodTargetObjectsList = new List<GameObject>();
    public static List<GameObject> enemiesTargetObjectsList = new List<GameObject>();
    public static List<GameObject> collectablesObjectsList = new List<GameObject>();
    #endregion

    #region GameObjects' References
    [Header("Prefabs")]


    public GameObject followerPrefab;
    public GameObject targetPrefab_Food;
    public GameObject enemy_FoodEaterPrefab;

    #endregion


    #region Spawn Objects Managers
    [Header("Spawn Object")]
    [SerializeField] int inSceneMoney = 300;

    Vector3 clampedSpawnPosition;
    float spawnDelay = 0.25f;
    #endregion


    #region Upgradables
    #region Food Properties

    [Header("Food Properties")]
    public FoodProperties[] foodTypes;
    int currentFoodIndex = 0;


    #endregion

    #region Upgrade Costs
    int followerUpgradeCost = 100;
    int foodUpgradeCost = 300;

    #endregion

    #endregion


    #region Scripts References
    private BoundsAndPositioningManager positioningManager;

    #endregion



    void Start()
    {
        inSceneMoney = 99999999;
        GameEvents.eventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);

        positioningManager = GetComponent<BoundsAndPositioningManager>();

        StartCoroutine(HandleClicksAndTouches());

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


                if (hit.collider != null && hit.collider.gameObject.layer == 8)
                {
                    inSceneMoney += hit.collider.gameObject.GetComponent<Collectable>().moneyConfig.moneyValue;
                    GameEvents.eventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);


                    Destroy(hit.collider.gameObject);

                    yield return new WaitForSeconds(.15f);
                }
                else
                {
                    SpawnObject(2);
                    yield return new WaitForSeconds(spawnDelay);
                }


            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                // test case to spawn a test object 
                // <<<<<<Will be deleted later>>>>>
                // <<<<<<Will be deleted later>>>>>
                // <<<<<<Will be deleted later>>>>>
                SpawnObject(3);

                yield return new WaitForSeconds(spawnDelay);
            }
            yield return null;
        }
    }
    #endregion


    #region Spawning Mnagers
    // A function that will be called to spawn an object
    public void SpawnObject(int objectType)
    {
        GameObject _spawnedObject = null;

        // Switch statement to determine which object to spawn
        switch (objectType)
        {
            // If the object type is 1, spawn an instance from follower prefab
            case 1:
                if (inSceneMoney >= followerUpgradeCost)
                {
                    // Get a random position depending on the camera viewport
                    clampedSpawnPosition = positioningManager.GetNewRandomPosition();
                    _spawnedObject = Instantiate(followerPrefab, clampedSpawnPosition, Quaternion.identity);

                    mainFishiesObjectsList.Add(_spawnedObject);

                    inSceneMoney -= followerUpgradeCost;
                }
                break;
            // If the object type is 2, spawn an instance from target prefab
            case 2:
                if (inSceneMoney >= foodTypes[currentFoodIndex].foodCost)
                {
                    _spawnedObject = Instantiate(targetPrefab_Food, clampedSpawnPosition, Quaternion.identity);
                    _spawnedObject.GetComponent<Food>().foodConfig = foodTypes[currentFoodIndex];


                    foodTargetObjectsList.Add(_spawnedObject);


                    inSceneMoney -= foodTypes[currentFoodIndex].foodCost;
                }
                break;
            // If the object type is 3, spawn an instance from enemy prefab
            case 3:
                clampedSpawnPosition = positioningManager.GetNewRandomPosition();
                _spawnedObject = Instantiate(enemy_FoodEaterPrefab, clampedSpawnPosition, Quaternion.identity);

                enemiesTargetObjectsList.Add(_spawnedObject);
                break;
        }
        GameEvents.eventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);
    }

    void UpgradeFood()
    {
        if (inSceneMoney >= foodUpgradeCost && currentFoodIndex < foodTypes.Length - 1)
        {
            // Deduct the cost of the food upgrade from the in-scene money
            currentFoodIndex = (currentFoodIndex + 1) % foodTypes.Length;
            inSceneMoney -= foodUpgradeCost;
            GameEvents.eventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);
        }
    }
    #endregion


    #region Event Subscriptions
    private void OnEnable()
    {
        GameEvents.eventsChannelInstance.onSpawnObject += SpawnObject;
        GameEvents.eventsChannelInstance.onUpgradeFood += UpgradeFood;
    }

    private void OnDisable()
    {
        GameEvents.eventsChannelInstance.onSpawnObject -= SpawnObject;
        GameEvents.eventsChannelInstance.onUpgradeFood -= UpgradeFood;
    }
    #endregion

}
