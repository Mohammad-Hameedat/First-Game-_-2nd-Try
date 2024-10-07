using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Properties", menuName = "Weapon Data/Weapon Properties")]
public class WeaponProperties : ScriptableObject
{
    public int weaponDamage;
    public int weaponCost;
    public float fireDelay;
}
