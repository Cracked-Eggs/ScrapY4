using System;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] Image healthbarSprite;
    [SerializeField] float reduceSpeed = 2f;
    float target = 1f;

    public void UpdateHeathBar(float maxHealth, float currentHealth)
    {
        target = currentHealth / maxHealth;
    }

    void Update()
    {
        healthbarSprite.fillAmount =
            Mathf.MoveTowards(healthbarSprite.fillAmount, target, reduceSpeed * Time.deltaTime);
    }
}
