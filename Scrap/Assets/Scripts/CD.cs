using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CD : MonoBehaviour
{
    [Header("UI References")]
    public Image fillImage;
    
    [Header("Settings")]
    public float totalTime = 2f;
    public bool hideOnComplete = true;
    
    private float currentTime;
    private bool isCountingDown = false;

    void Start()
    {
        gameObject.SetActive(false); // Start hidden
    }

    void Update()
    {
        if (isCountingDown)
        {
            currentTime -= Time.deltaTime;
            
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                isCountingDown = false;
                OnCountdownComplete();
            }
            
            UpdateDisplay();
        }
    }

    void UpdateDisplay()
    {
        fillImage.fillAmount = currentTime / totalTime;
    }

    public void StartCountdown(float duration)
    {
        totalTime = duration;
        currentTime = totalTime;
        gameObject.SetActive(true);
        isCountingDown = true;
        UpdateDisplay();
    }

    void OnCountdownComplete()
    {
        if (hideOnComplete)
        {
            gameObject.SetActive(false);
        }
    }
}