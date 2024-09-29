using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Enemy - Configs", menuName = "Follower Data/Enemy Properties")]
public class EnemyProperties : ScriptableObject
{
    [Header("Health Configurations")]
    public int health; // The health of the follower object (Enemy Follower)


    [Header("Hunger Situation Configurations")]
    public float minRandomHungerTime; // The minimum random time before the object gets hungry
    public float maxRandomHungerTime; // The maximum random time before the object gets hungry


    [Header("Difficulty Configurations")]
    public int numberOfObjectsToEat;
    public int nextNumberOfObjectsToEat;

}
