using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] Image healthbarSprite;

    public void UpdateHeathBar(float maxHealth, float currentHealth) =>
        healthbarSprite.fillAmount = currentHealth / maxHealth;
}
