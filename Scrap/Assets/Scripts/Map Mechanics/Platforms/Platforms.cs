using System;
using System.Collections;
using UnityEngine;

public class Platforms : MonoBehaviour
{
    AudioManager audioManager;
    [SerializeField] float delay = 1f;
    
    private float lastPlayTime;
    public float soundCooldown = 0.5f;
    
    Animator platformAnimator;
    bool isOn = false;

    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Start() => platformAnimator = GetComponent<Animator>();

    public void RaisePlatform()
    {
        isOn = true;
        StartCoroutine(ActivatePlatformWithDelay());
    }

    public void LowerPlatform()
    {
        isOn = false;
        StartCoroutine(ActivatePlatformWithDelay());
    } 
    
    IEnumerator ActivatePlatformWithDelay()
    {
        yield return new WaitForSeconds(delay);

        bool currentState = platformAnimator.GetBool("IsOn");

        // Only play sound if the state is changing
        if (isOn != currentState)
        {
            platformAnimator.SetBool("IsOn", isOn);
        }
    }

    public void Play()
    {
        if (Time.time > lastPlayTime + soundCooldown)
        {
            audioManager.Play("Plate");
            lastPlayTime = Time.time;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.transform.parent = transform;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            other.transform.SetParent(null);
    }
}