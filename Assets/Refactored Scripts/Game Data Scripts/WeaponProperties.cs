using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Properties", menuName = "Weapon Data/Weapon Properties")]
public class WeaponProperties : ScriptableObject
{
    [Header("Weapon Configurations")]
    [Tooltip("The damage that the weapon can deal to the target object")]
    public int weaponDamage;
    [Tooltip("The cost of upgrading to the next weapon")]
    public int weaponCost;
    [Tooltip("The time before firing the taget object again")]
    public float fireDelay;
}
