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
    float lastDamageTime;
    float damageCooldown = 0.5f;
    AudioManager audioManager;

    public event Action OnTakeDamage;
    public event Action OnDie;
    public bool IsDead => health == 0;

    private Attach attachScript;

    void Start()
    {
        health = maxHealth;
        attachScript = GetComponent<Attach>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void SetInvulnerable(bool isInvulnerable) => this.isInvulnerable = isInvulnerable;

    public void DealDamage(int damage, bool ignoreInvulnerability = false)
    {
        if (Time.time < lastDamageTime + damageCooldown) return;

        if (!ignoreInvulnerability && (health == 0 || isInvulnerable)) return;

        health = Mathf.Max(health - damage, 0);
        lastDamageTime = Time.time;
        audioManager.Play("Hit");

        if (damagePrefab != null)
        {
            GameObject damageInstance = Instantiate(damagePrefab, transform.position, Quaternion.identity);
            Destroy(damageInstance, 2f);
        }

        OnTakeDamage?.Invoke();
        healthBar.UpdateHeathBar(maxHealth, health);

        if (health == 0 && !Player)
        {
            OnDie?.Invoke();
            StartCoroutine(EnemyDie());
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
        if (attachScript != null)
            attachScript.DetachAll();

        yield return new WaitForSeconds(1f);
        UIController.instance.StartFadeToBlack();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SaveSystem.instance.activeSave.currentLevel);
    }
    
    IEnumerator MainMenu()
    {
        yield return new WaitForSeconds(1f);
        UIController.instance.StartFadeToBlack();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
    }
    
    
    IEnumerator EnemyDie()
    {
        if (attachScript != null)
            attachScript.DetachAll();
        yield return new WaitForSeconds(1f);
    }

    public void Restart() => StartCoroutine(Die());
    public void Menu() => StartCoroutine(MainMenu());
}