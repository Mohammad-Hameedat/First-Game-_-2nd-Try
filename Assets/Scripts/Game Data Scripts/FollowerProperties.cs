using UnityEngine;

[CreateAssetMenu(fileName = "Follower Properties", menuName = "Follower Data/Follower Properties")]
public class FollowerProperties : ScriptableObject
{
    public GameObject collectablePrefab; // The prefab of the collectable object that the follower will spawn

    [Header("Money configurations")]
    public CollectableProperties[] collectableConfigs; // The money types that can be spawned by the follower


    [Header("Detection Range Configurations")]
    public float nearestDistanceToEatATarget; // Set a threshold for the distance between the follower and the target object before eating it


    [Header("Movement Configurations")]
    public float minRandomDesiredVelocity; // The minimum desired velocity for the follower
    public float maxRandomDesiredVelocity; // The maximum desired velocity for the follower

    public float maxFollowingDesiredVelocity; // The maximum desired velocity for the follower when following a target object

    public float minAccelerationDuration; // The minimum acceleration duration for the follower
    public float maxAccelerationDuration; // The maximum acceleration duration for the follower


    [Header("Hunger Situation Configurations")]
    public float hungerStartingTime; // The time before the object gets hungry after eating a target object
    public float destructionTime; // The time before the object gets destroyed after getting hungry

    public float idleDuration; // The time before the object gets hungry after the hunger cycle starts
    public float hungerDuration; // The time allowed for the object to eat a target object before 


    [Header("Collectable Objects Configurations")]
    public float minCollectableSpwanTime; // The minimum time before the next collectable object spawn
    public float maxCollectableSpwanTime; // The maximum time before the next collectable object spawn


    [Header("Random Target Configurations")]
    public float minDistanceTowardsRandomTarget; // The minimum distance towards the random target position
}
