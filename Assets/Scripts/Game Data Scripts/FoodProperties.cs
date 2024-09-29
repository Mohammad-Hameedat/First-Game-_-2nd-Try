using UnityEngine;


[CreateAssetMenu(fileName = "Food Properties", menuName = "Food Data/Food Properties")]
public class FoodProperties : ScriptableObject
{
    public int foodCost;
    public int damage;
    public float staminaTime;
    public float destructionTime;
}
