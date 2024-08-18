using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "FoodProperties", menuName = "Game Economy/Food Properties")]
public class FoodProperties : ScriptableObject
{
    public int foodCost;
    public int damage;
    public float staminaTime;
    public float destructionTime;
}
