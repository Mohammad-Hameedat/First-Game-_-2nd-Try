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
    Vector3 spawnPosition;


    void Start()
    {
        positioningManager = GetComponent<BoundsAndPositioningManager>();

        StartCoroutine(MouseClicksHandler());

        // A function that subscribed to the event, when the event is invoked.. the function will be called to do whatever is inside it.
        //GameEvents.eventsChannelInstance.onSpawnObject += SpawnObject;
    }


    IEnumerator MouseClicksHandler()
    {
        while (true)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePosition = Input.mousePosition;
                Vector3 mousePositionToWorld = Camera.main.ScreenToWorldPoint(mousePosition);
                Vector3 mousePositionClamped = positioningManager.ClampPositionWithInView(mousePositionToWorld);


                #region Mouse Clicks
                // Check if the ((MOUSE CLICK)) is over a UI element
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    yield return null;
                }
                else
                {
                    //Spawn a target object instance in the position of the ((MOUSE CLICK))
                    spawnPosition = mousePositionClamped;
                    SpawnObject(2);

                    yield return new WaitForSeconds(0.25f);
                }
                #endregion


                #region Touches
                // Check if the ((TOUCH)) is over a UI element
                foreach (Touch touch in Input.touches)
                {
                    int id = touch.fingerId;
                    // If the ((TOUCH)) is over a UI element, skip spawning an object and wait for the next frame
                    if (EventSystem.current.IsPointerOverGameObject(id))
                    {
                        yield return null;
                    }

                    else
                    {
                        //Spawn a target object instance in the position of the ((TOUCH))
                        spawnPosition = mousePositionClamped;
                        SpawnObject(2);

                        yield return new WaitForSeconds(0.25f);
                    }
                }
                #endregion
            }
            yield return null;
        }
    }

    // A function that will be called to spawn an object
    public void SpawnObject(int objectType)
    {


        // Switch statement to determine which object to spawn
        switch (objectType)
        {
            // If the object type is 1, spawn the follower prefab
            case 1:
                // Get a random position depending on the camera viewport
                spawnPosition = positioningManager.GetNewRandomPosition();
                _spawnedObject = Instantiate(followerPrefab, spawnPosition, Quaternion.identity);
                break;
            // If the object type is 2, spawn the target prefab
            case 2:
                _spawnedObject = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
                FollowerController.AddTargetObjectToList(_spawnedObject);
                break;
        }
    }
}
