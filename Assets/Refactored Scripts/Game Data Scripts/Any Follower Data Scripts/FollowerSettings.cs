using UnityEngine;

[CreateAssetMenu(fileName = "Follower Settings", menuName = "Follower Data/Follower Settings")]
public class FollowerSettings : ScriptableObject
{
    [Tooltip("Assign the movement properties (Scriptable Object) to the follower")]
    public MovementProperties movementProperties;
    [Tooltip("Assign the hunger properties (Scriptable Object) to the follower")]
    public HungerProperties hungerProperties;
    [Tooltip("Assign the spawn properties (Scriptable Object) to the follower")]
    public SpawnProperties spawnProperties;
}