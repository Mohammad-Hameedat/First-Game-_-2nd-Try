using UnityEngine;

[CreateAssetMenu(fileName = "Follower Settings", menuName = "Follower Data/Follower Settings")]
public class FollowerSettings : ScriptableObject
{
    [Header("Follower Configurations")]
    #region Follower Configurations
    [Tooltip("Assign the movement properties (Scriptable Object) here.")]
    public MovementProperties movementProperties;

    [Tooltip("Assign the hunger properties (Scriptable Object) here.")]
    public HungerProperties hungerProperties;

    [Tooltip("Assign the spawn properties (Scriptable Object) here.")]
    public CollectibleSpawnProperties spawnProperties;

    [Tooltip("Assign the attack properties (Scriptable Object) here.")]
    public AttackProperties attackProperties;
    #endregion

}