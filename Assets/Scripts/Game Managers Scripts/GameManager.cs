using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region References
    private BoundsManager boundsManager;

    [Header("Prefabs")]
    public GameObject followerPrefab;
    public GameObject targetPrefab;

    #endregion

    [Header("Spawn Object")]
    [SerializeField] GameObject _spawnedObject = null;

    void Start()
    {
        boundsManager = GetComponent<BoundsManager>();

        // A function that subscribed to the event, when the event is invoked.. the function will be called to do whatever is inside it.
        //GameEvents.eventsChannelInstance.onSpawnObject += SpawnObject;
    }


    public void SpawnObject(int objectType)
    {
        // Get a random position in the camera view
        Vector3 spawnPosition = boundsManager.GetNewRandomPosition();
        spawnPosition.z = -2;

        // Switch statement to determine which object to spawn
        switch (objectType)
        {
            // If the object type is 1, spawn the follower prefab
            case 1:
                _spawnedObject = Instantiate(followerPrefab, spawnPosition, Quaternion.identity);
                break;
            // If the object type is 2, spawn the target prefab
            case 2:

                // <<<<<<<<>>>>>>>>> A random position for the target object to spawn <<<<<<<<>>>>>>>>>
                // <<<<<<<<>>>>>>>>> This will be later deleted                       <<<<<<<<>>>>>>>>>
                spawnPosition.y = Random.Range(10f, 17f);



                _spawnedObject = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
                FollowerController.AddTargetObjectToList(_spawnedObject);
                break;
        }
    }
}
