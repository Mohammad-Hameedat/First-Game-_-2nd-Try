using UnityEngine;

[CreateAssetMenu(fileName = "Spawn Properties", menuName = "Follower Data/Spawn Properties")]
public class SpawnProperties : ScriptableObject
{
    [Header("Collectable Object Prefab")]
    public GameObject collectablePrefab; // The prefab of the collectable object that the follower will spawn


    [Header("Spawnable Collectables Configurations")]
    public CollectableProperties[] collectableConfigs; // The money types that can be spawned by the follower

    public float minCollectableSpwanTime; // The minimum time before the next collectable object spawn
    public float maxCollectableSpwanTime; // The maximum time before the next collectable object spawn
}