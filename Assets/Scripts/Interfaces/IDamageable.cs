using UnityEngine;

/// <summary>
/// Interface for objects that can take damage
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Apply damage to this object
    /// </summary>
    /// <param name="damage">Amount of damage to apply</param>
    /// <param name="damageSource">The source of the damage (optional, for tracking purposes)</param>
    void TakeDamage(float damage, GameObject damageSource = null);

    /// <summary>
    /// Get the current health of this object
    /// </summary>
    /// <returns>Current health value</returns>
    float GetCurrentHealth();

    /// <summary>
    /// Get the maximum health of this object
    /// </summary>
    /// <returns>Maximum health value</returns>
    float GetMaxHealth();

    /// <summary>
    /// Check if this object is currently alive
    /// </summary>
    /// <returns>True if alive, false if dead</returns>
    bool IsAlive();
}

