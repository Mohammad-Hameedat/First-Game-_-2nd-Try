using UnityEngine;

[CreateAssetMenu(fileName = "Movement Properties", menuName = "Follower Data/Movement Properties")]
public class MovementProperties : ScriptableObject
{
    [Header("Movement Configurations")]
    [Tooltip("The minimum desired velocity for the follower")]
    public float minRandomDesiredVelocity; // The minimum desired velocity for the follower
    [Tooltip("The maximum desired velocity for the follower")]
    public float maxRandomDesiredVelocity; // The maximum desired velocity for the follower

    [Tooltip("The minimum desired velocity for the follower when following a target object")]
    public float maxFollowingDesiredVelocity; // The maximum desired velocity for the follower when following a target object


    [Header("Movement Smoothness Configurations")]
    [Tooltip("The minimum acceleration duration for the follower")]
    public float minAccelerationDuration; // The minimum acceleration duration for the follower
    [Tooltip("The maximum acceleration duration for the follower")]
    public float maxAccelerationDuration; // The maximum acceleration duration for the follower


    [Header("Target Configurations")]
    [Tooltip("The minimum distance towards the random target position")]
    public float minDistanceTowardsRandomTarget; // The minimum distance towards the random target position

    [Tooltip("The distance threshold for the follower to eat a target object")]
    public float nearestDistanceToEatATarget; // Set a threshold for the distance between the follower and the target object before eating it
}