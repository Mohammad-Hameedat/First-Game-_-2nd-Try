using UnityEngine;

[CreateAssetMenu(fileName = "X Pet Attack Properties", menuName = "Follower Data/Attack Properties")]
public class AttackProperties : ScriptableObject
{
    [Header("Attack Properties")]
    [Tooltip("The damage that a follower OR a pet can deal to an enemy")]
    public int attackDamage;

    [Tooltip("The cooldown time that a follower OR a pet needs to wait before attacking again")]
    public float nextAttackHitTime;
}
