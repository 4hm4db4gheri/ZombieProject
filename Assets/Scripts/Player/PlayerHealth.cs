using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Player health system that implements IDamageable interface
/// </summary>
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Events")]
    [SerializeField] private UnityEvent<float> OnHealthChanged;
    [SerializeField] private UnityEvent<float> OnDamageTaken;
    [SerializeField] private UnityEvent OnDeath;
    [SerializeField] private UnityEvent OnHealthRestored;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Apply damage to the player
    /// </summary>
    /// <param name="damage">Amount of damage to apply</param>
    /// <param name="damageSource">Source of the damage</param>
    public void TakeDamage(float damage, GameObject damageSource = null)
    {
        if (isDead) return;

        // Clamp damage to ensure it's not negative
        damage = Mathf.Max(0, damage);

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (debugMode)
        {
            Debug.Log($"{gameObject.name} took {damage} damage from {(damageSource != null ? damageSource.name : "unknown source")}. Health: {currentHealth}/{maxHealth}");
        }

        // Invoke events
        OnDamageTaken?.Invoke(damage);
        OnHealthChanged?.Invoke(currentHealth);

        // Check for death
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    /// <summary>
    /// Restore health to the player
    /// </summary>
    /// <param name="healAmount">Amount of health to restore</param>
    public void Heal(float healAmount)
    {
        if (isDead) return;

        healAmount = Mathf.Max(0, healAmount);
        float previousHealth = currentHealth;

        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth > previousHealth)
        {
            OnHealthRestored?.Invoke();
            OnHealthChanged?.Invoke(currentHealth);

            if (debugMode)
            {
                Debug.Log($"{gameObject.name} healed for {healAmount}. Health: {currentHealth}/{maxHealth}");
            }
        }
    }

    /// <summary>
    /// Get the current health
    /// </summary>
    /// <returns>Current health value</returns>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Get the maximum health
    /// </summary>
    /// <returns>Maximum health value</returns>
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// Check if the player is alive
    /// </summary>
    /// <returns>True if alive, false if dead</returns>
    public bool IsAlive()
    {
        return !isDead && currentHealth > 0;
    }

    /// <summary>
    /// Handle player death
    /// </summary>
    private void Die()
    {
        isDead = true;
        DialogueManager.Instance.PlayDialogue(DialogueType.Death);
        OnDeath?.Invoke();

        if (debugMode)
        {
            Debug.Log($"{gameObject.name} has died!");
        }

        // You can add death logic here (disable controls, play death animation, etc.)
        // For example:
        // GetComponent<PlayerMovementController>().enabled = false;
        // GetComponent<PlayerShooting>().enabled = false;
    }

    /// <summary>
    /// Reset health to maximum (useful for respawning)
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        OnHealthChanged?.Invoke(currentHealth);

        if (debugMode)
        {
            Debug.Log($"{gameObject.name} health reset to {maxHealth}");
        }
    }

    /// <summary>
    /// Get health percentage (0-1)
    /// </summary>
    /// <returns>Health percentage</returns>
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    // Debug methods for testing
    [ContextMenu("Take 10 Damage")]
    private void DebugTakeDamage()
    {
        TakeDamage(10f);
    }

    [ContextMenu("Heal 20 Health")]
    private void DebugHeal()
    {
        Heal(20f);
    }

    [ContextMenu("Reset Health")]
    private void DebugResetHealth()
    {
        ResetHealth();
    }
}

