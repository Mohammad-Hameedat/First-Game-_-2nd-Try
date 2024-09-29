using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    #region GameObjects Lists
    public static List<GameObject> mainFishObjectsList = new();
    public static List<GameObject> foodTargetObjectsList = new();
    public static List<GameObject> enemyObjectsList = new();
    public static List<GameObject> collectablesObjectsList = new();
    #endregion

    #region GameObjects' References
    [Header("Prefabs")]
    public GameObject followerPrefab;
    public GameObject foodPrefab;
    public GameObject enemy_FoodEaterPrefab;
    public GameObject enemy_ClorpPrefab;

    #region Scripts References
    private BoundsAndPositioningManager positioningManager;
    #endregion
    #endregion


    #region Spawn Objects Managers
    [Header("Spawn Object")]
    [SerializeField] int inSceneMoney = 300;
    Vector3 clampedSpawnPosition;

    // This is the delay value between clicks or touches
    const float spawnDelay = 0.001f;
    #endregion


    #region Upgradables

    #region Food Properties
    [Header("Food Properties")]
    public FoodProperties[] foodTypes;
    int currentFoodIndex = 0;
    #endregion


    #region Upgrade Costs
    const int followerInstantiateCost = 100;
    const int foodUpgradeCost = 300;
    #endregion
    #endregion


    void Start()
    {
        inSceneMoney = 99999999;
        GameEvents.EventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);

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


                if (hit.collider != null && hit.collider.gameObject.layer == 8)
                {
                    inSceneMoney += hit.collider.gameObject.GetComponent<Collectable>().collectableConfig.collectableValue;
                    GameEvents.EventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);


                    Destroy(hit.collider.gameObject);

                    yield return new WaitForSeconds(.05f);
                }
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
    // A function that will be called to spawn an object
    public void SpawnObject(int objectType)
    {
        GameObject _spawnedObject = null;

        // Switch statement to determine which object to spawn
        switch (objectType)
        {
            // If the object type is 1, spawn an instance from follower prefab
            case 1:
                if (inSceneMoney >= followerInstantiateCost)
                {
                    // Get a random position depending on the camera viewport
                    clampedSpawnPosition = positioningManager.GetNewRandomPosition();
                    _spawnedObject = Instantiate(followerPrefab, clampedSpawnPosition, Quaternion.identity);

                    mainFishObjectsList.Add(_spawnedObject);

                    inSceneMoney -= followerInstantiateCost;
                    GameEvents.EventsChannelInstance.RefresheMainFishesNumber(mainFishObjectsList.Count);
                }
                break;
            // If the object type is 2, spawn an instance from target prefab
            case 2:
                if (inSceneMoney >= foodTypes[currentFoodIndex].foodCost || enemyObjectsList.Count > 0)
                {
                    _spawnedObject = Instantiate(foodPrefab, clampedSpawnPosition, Quaternion.identity);
                    _spawnedObject.GetComponent<Food>().foodConfig = foodTypes[currentFoodIndex];


                    foodTargetObjectsList.Add(_spawnedObject);

                    if (enemyObjectsList.Count > 0)
                    {
                        break;
                    }
                    else
                    {
                        inSceneMoney -= foodTypes[currentFoodIndex].foodCost;
                    }
                }
                break;
        }
        if (_spawnedObject != null) // If the object is spawned, update the in-scene money
        {
            GameEvents.EventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);
        }
    }


    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            if (enemyObjectsList.Count == 0)
            {
                yield return new WaitForSeconds(3f);
                clampedSpawnPosition = positioningManager.GetNewRandomPosition();
                GameObject enemyInstance = Instantiate(enemy_ClorpPrefab, clampedSpawnPosition, Quaternion.identity);

                enemyObjectsList.Add(enemyInstance);
            }
            yield return null;
        }
    }
    #endregion


    // A function that will be called to upgrade the food
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


    #region Event Subscriptions
    private void OnEnable()
    {
        // Invoked by UI buttons
        GameEvents.EventsChannelInstance.OnSpawnObject += SpawnObject;
        GameEvents.EventsChannelInstance.OnUpgradeFood += UpgradeFood;
    }

    private void OnDisable()
    {
        GameEvents.EventsChannelInstance.OnSpawnObject -= SpawnObject;
        GameEvents.EventsChannelInstance.OnUpgradeFood -= UpgradeFood;
    }
    #endregion

}
