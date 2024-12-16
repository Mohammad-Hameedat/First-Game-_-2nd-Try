using UnityEngine;

[CreateAssetMenu(fileName = "Collectable Properties", menuName = "Collectable Data/Collectable Properties")]
public class CollectableProperties : ScriptableObject
{
    [Header("Money Configurations")]
    public int collectableValue;
    public float collectableMovementSpeed;


    [Header("Destroying Managers")]
    public float TimeBeforeDestroy;

}
