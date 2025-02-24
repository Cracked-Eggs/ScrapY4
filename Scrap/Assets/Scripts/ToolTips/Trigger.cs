using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] UnityEvent triggerEvent;
    [SerializeField] UnityEvent soundEvent;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            triggerEvent.Invoke();
    }

    public void SoundTrigger()
    {
        soundEvent.Invoke();
    }
}