using UnityEngine;

[CreateAssetMenu(fileName = "Collectable Properties", menuName = "Collectable Data/Collectable Properties")]
public class CollectibleProperties : ScriptableObject
{
    [Header("Money Configurations")]
    [Tooltip("The value of the collectable.")]
    public int collectableValue;

    [Tooltip("The speed of the collectable.")]
    public float collectableMovementSpeed;


    [Header("Destroying Managers")]
    [Tooltip("The time before the collectable is destroyed.")]
    public float TimeBeforeDestroy;

}
