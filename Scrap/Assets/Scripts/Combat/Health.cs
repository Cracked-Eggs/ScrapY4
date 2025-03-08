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

    int health;
    int damageTakenCounter = 0;
    bool isInvulnerable;
    bool rArmLosing;
    float lastDamageTime; // Track the last time damage was dealt
    float damageCooldown = 0.5f; // Cooldown time in seconds

    public event Action OnTakeDamage;
    public event Action OnDie;
    public bool IsDead => health == 0;

    Attach attachScript;

    void Start()
    {
        health = maxHealth;
        attachScript = GetComponent<Attach>();
    }

    void Update()
    {
        if (damageTakenCounter == 2 && attachScript != null && !attachScript._isL_ArmDetached && !rArmLosing)
        {
            attachScript.l_ArmColl.enabled = false;
            attachScript.DroppingLeftArm();
            damageTakenCounter = 0;
            rArmLosing = true;
        }
        
        if (damageTakenCounter == 2 && attachScript != null && !attachScript._isR_ArmDetached && rArmLosing)
        {
            attachScript.r_ArmColl.enabled = false;
            attachScript.DroppingRightArm();
            damageTakenCounter = 0;
            rArmLosing = false;
        }
    }

    public void SetInvulnerable(bool isInvulnerable) => this.isInvulnerable = isInvulnerable;

    public void DealDamage(int damage, bool ignoreInvulnerability = false)
    {
        if (Time.time < lastDamageTime + damageCooldown)
            return;

        if (!ignoreInvulnerability && (health == 0 || isInvulnerable))
            return;

        health = Mathf.Max(health - damage, 0);
        lastDamageTime = Time.time; // Update the last damage time

        if (damagePrefab != null)
        {
            GameObject damageInstance = Instantiate(damagePrefab, transform.position, Quaternion.identity);
            Destroy(damageInstance, 2f); // Destroy the damage prefab after 2 seconds
        }

        OnTakeDamage?.Invoke();
        healthBar?.UpdateHeathBar(maxHealth, health);
        damageTakenCounter++;

        if (health == 0)
        {
            OnDie?.Invoke();
            DieEvent.Invoke();
        }

        Debug.Log(health);
    }

    public void Die()
    {
        health = Mathf.Max(health - 100, 0);
        healthBar?.UpdateHeathBar(maxHealth, health);
        StartCoroutine(Restart());
    }

    public IEnumerator Restart()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(2);
    }
}