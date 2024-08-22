using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManagerInstance;


    #region References
    [Header("Prefabs")]

    private BoundsAndPositioningManager positioningManager;

    public GameObject followerPrefab;
    public GameObject targetPrefab;

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



    private void Awake()
    {
        if (gameManagerInstance == null)
        {
            gameManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        inSceneMoney = 99999999;
        GameEvents.eventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);


        positioningManager = GetComponent<BoundsAndPositioningManager>();

        StartCoroutine(HandleClicksAndTouches());
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

            // quick check to see if the input is active
            if (isInputActive)
            {
                // Convert the input position to a world position
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

                // Clamp the world position to be within the camera view
                clampedSpawnPosition = positioningManager.ClampPositionWithInView(worldPosition);

                Ray ray = Camera.main.ScreenPointToRay(inputPosition);
                RaycastHit hit;



                if (Physics.Raycast(ray, out hit, 21f))
                {
                    switch (hit.collider.gameObject.layer)
                    {
                        case 5:
                            yield return new WaitForSeconds(.1f);
                            isInputActive = false;
                            break;


                        case 8:
                            Destroy(hit.collider.gameObject);
                            inSceneMoney += hit.collider.gameObject.GetComponent<Collectable>().moneyConfig.moneyValue;
                            GameEvents.eventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);
                            yield return new WaitForSeconds(.1f);

                            break;


                        default:
                            SpawnObject(2);
                            yield return new WaitForSeconds(spawnDelay);

                            break;
                    }
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
            // If the object type is 1, spawn the follower prefab
            case 1:
                if (inSceneMoney >= followerUpgradeCost)
                {
                    // Get a random position depending on the camera viewport
                    clampedSpawnPosition = positioningManager.GetNewRandomPosition();
                    _spawnedObject = Instantiate(followerPrefab, clampedSpawnPosition, Quaternion.identity);

                    inSceneMoney -= followerUpgradeCost;
                }
                break;
            // If the object type is 2, spawn the target prefab
            case 2:
                if (inSceneMoney >= foodTypes[currentFoodIndex].foodCost)
                {
                    _spawnedObject = Instantiate(targetPrefab, clampedSpawnPosition, Quaternion.identity);
                    _spawnedObject.GetComponent<Target>().foodConfig = foodTypes[currentFoodIndex];

                    FollowerController.AddTargetObjectToList(_spawnedObject);
                    inSceneMoney -= foodTypes[currentFoodIndex].foodCost;
                }
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

            Debug.Log("Current food index = " + currentFoodIndex + ", And food type is: " + foodTypes[currentFoodIndex].name);
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
