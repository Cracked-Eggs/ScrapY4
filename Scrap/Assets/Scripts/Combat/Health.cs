using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    [SerializeField] Healthbar healthBar;
    [SerializeField] GameObject damagePrefab;
    [SerializeField] UnityEvent DieEvent;
    [SerializeField] bool Player;

    int health;
    bool isInvulnerable;
    float lastDamageTime; // Track the last time damage was dealt
    float damageCooldown = 0.5f; // Cooldown time in seconds

    public event Action OnTakeDamage;
    public event Action OnDie;
    public bool IsDead => health == 0;

    void Start() => health = maxHealth;

    public void SetInvulnerable(bool isInvulnerable) => this.isInvulnerable = isInvulnerable;

    public void DealDamage(int damage, bool ignoreInvulnerability = false)
    {
        // Check if enough time has passed since the last damage
        if (Time.time < lastDamageTime + damageCooldown)
        {
            return;
        }

        if (!ignoreInvulnerability && (health == 0 || isInvulnerable))
        {
            return;
        }

        health = Mathf.Max(health - damage, 0);
        lastDamageTime = Time.time; // Update the last damage time

        if (damagePrefab != null)
        {
            GameObject damageInstance = Instantiate(damagePrefab, transform.position, Quaternion.identity);
            Destroy(damageInstance, 2f); // Destroy the damage prefab after 2 seconds
        }

        OnTakeDamage?.Invoke();
        healthBar.UpdateHeathBar(maxHealth, health);

        if (health == 0 && !Player)
        {
            OnDie?.Invoke();
            DieEvent.Invoke();
        }
        else if (health == 0 && Player)
        {
            OnDie?.Invoke();
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(1f);
        UIController.instance.StartFadeToBlack();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SaveSystem.instance.activeSave.currentLevel);

    }

    public void Restart() => StartCoroutine(Die());

}