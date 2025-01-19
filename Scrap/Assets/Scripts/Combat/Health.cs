using System;
using UnityEngine;
public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    
    int health;
    bool isInvulnerable;
    
    public event Action OnTakeDamage;
    public event Action OnDie;
    public bool IsDead => health == 0;
    
    void Start() => health = maxHealth;

    public void SetInvulnerable(bool isInvulnerable) =>  this.isInvulnerable = isInvulnerable;

    public void DealDamage(int damage)
    {
        if (health == 0) { return; }
        
        if (isInvulnerable) { return; }
        health = Mathf.Max(health - damage, 0);
        
        OnTakeDamage?.Invoke();
        
        if (health == 0)
            OnDie?.Invoke();
        
        Debug.Log(health);
    }
}