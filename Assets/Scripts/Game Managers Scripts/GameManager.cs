using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{

    #region References
    private BoundsAndPositioningManager positioningManager;

    [Header("Prefabs")]
    public GameObject followerPrefab;
    public GameObject targetPrefab;

    #endregion

    [Header("Spawn Object")]
    [SerializeField] GameObject _spawnedObject = null;
    Vector3 clampedSpawnPosition;
    float spawnDelay = 0.1f;


    float inSceneMoney;


    void Start()
    {
        inSceneMoney = 300f;
        GameEvents.eventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);


        positioningManager = GetComponent<BoundsAndPositioningManager>();

        StartCoroutine(HandleClicksAndTouches());

        // A function that subscribed to the event, when the event is invoked.. the function will be called to do whatever is inside it.
        //GameEvents.eventsChannelInstance.onSpawnObject += SpawnObject;
    }

    #region Handle Clicks and Touches
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
                // Get the first touch onlt if there are multiple touches and get the touch position
                Touch touch = Input.GetTouch(0);
                inputPosition = touch.position;
                isInputActive = true;
            }

            // quick check to see if the input is active
            if (isInputActive)
            {
                // Convert the input position to a world position
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);
                // Clamp the world position to be within the camera view
                clampedSpawnPosition = positioningManager.ClampPositionWithInView(worldPosition);

                // Check if the mouse click or touch is over a UI element
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    //Spawn a target object instance in the position of the mouse click or touch
                    SpawnObject(2);
                    yield return new WaitForSeconds(spawnDelay);
                }
            }

            yield return null;
        }
    }
    #endregion


    // A function that will be called to spawn an object
    public void SpawnObject(int objectType)
    {
        // Switch statement to determine which object to spawn
        switch (objectType)
        {
            // If the object type is 1, spawn the follower prefab
            case 1:
                if (inSceneMoney >= 200f)
                {
                    // Get a random position depending on the camera viewport
                    clampedSpawnPosition = positioningManager.GetNewRandomPosition();
                    _spawnedObject = Instantiate(followerPrefab, clampedSpawnPosition, Quaternion.identity);
                    inSceneMoney -= 200f;
                }
                break;
            // If the object type is 2, spawn the target prefab
            case 2:
                if (inSceneMoney >= 5f)
                {
                    _spawnedObject = Instantiate(targetPrefab, clampedSpawnPosition, Quaternion.identity);
                    FollowerController.AddTargetObjectToList(_spawnedObject);
                    inSceneMoney -= 5f;
                }
                break;
        }
        GameEvents.eventsChannelInstance.UpdateInGameSceneMoney(inSceneMoney);
    }


    #region Event Subscriptions
    private void OnEnable()
    {
        GameEvents.eventsChannelInstance.onSpawnObject += SpawnObject;
    }

    private void OnDisable()
    {
        GameEvents.eventsChannelInstance.onSpawnObject -= SpawnObject;
    }
    #endregion

}
