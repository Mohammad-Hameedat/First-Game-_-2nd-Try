using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject followerPrefab;
    public GameObject targetPrefab;

    [Header("Spawn Object")]
    [SerializeField] GameObject _spawnObject = null;


    private void Start()
    {
        // A function that subscribed to the event, when the event is invoked.. the function will be called to do whatever is inside it.
        GameEvents.eventsChannelInstance.onSpawnObject += SpawnObject;
    }


    public void SpawnObject(int objectType)
    {
        // Random spawn position
        Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), Random.Range(1f, 17f), -2);

        // Switch statement to determine which object to spawn
        switch (objectType)
        {
            // If the object type is 1, spawn the follower prefab
            case 1:
                _spawnObject = Instantiate(followerPrefab, spawnPosition, Quaternion.identity);
                break;
            // If the object type is 2, spawn the target prefab
            case 2:
                _spawnObject = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
                FollowerController.AddTargetObjectToList(_spawnObject);
                break;
        }

    }

}
