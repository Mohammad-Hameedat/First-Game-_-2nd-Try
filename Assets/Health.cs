using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // This method is called when the gameobject gets hit by player's click
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    // This method is called when the controller wants to know the current health of the gameobject
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
