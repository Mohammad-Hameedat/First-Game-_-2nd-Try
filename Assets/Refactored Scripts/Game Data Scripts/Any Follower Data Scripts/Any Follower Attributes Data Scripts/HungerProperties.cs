using UnityEngine;

[CreateAssetMenu(fileName = "Hunger Properties",menuName = "Follower Data/Hunger Properties")]
public class HungerProperties : ScriptableObject
{
    [Header("Hunger Configurations")]
    public float hungerStartingTime; // The time before the object gets hungry after eating a target object
    public float destructionTime; // The time before the object gets destroyed after getting hungry

    public float idleDuration; // The time before the object gets hungry after the hunger cycle starts
    public float hungerDuration; // The time allowed for the object to eat a target object before 
}