using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] Healthbar healthBar;
    [SerializeField] GameObject damagePrefab;
    [SerializeField] UnityEvent DieEvent;

    int health;
    bool isInvulnerable;
    float lastDamageTime; // Track the last time damage was dealt
    float damageCooldown = 0.5f; // Cooldown time in seconds

    public event Action OnTakeDamage;
    public event Action OnDie;
    public bool IsDead => health == 0;

    void Start() => health = maxHealth;

    public void SetInvulnerable(bool isInvulnerable) => this.isInvulnerable = isInvulnerable;

    public void DealDamage(int damage)
    {
        // Check if enough time has passed since the last damage
        if (Time.time < lastDamageTime + damageCooldown)
        {
            return;
        }

        if (health == 0 || isInvulnerable)
        {
            return;
        }

        health = Mathf.Max(health - damage, 0);
        lastDamageTime = Time.time; // Update the last damage time

        if (damagePrefab != null)
        {
            Instantiate(damagePrefab, transform.position, Quaternion.identity);
        }

        OnTakeDamage?.Invoke();
        healthBar.UpdateHeathBar(maxHealth, health);

        if (health == 0)
        {
            OnDie?.Invoke();
            DieEvent.Invoke();

        }

        Debug.Log(health);
    }
}