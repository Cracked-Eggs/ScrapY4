using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    Animator animator;
    const string PressedHash = "isPressed";
    [SerializeField] UnityEvent magnetEvent;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>(); 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            animator.SetBool(PressedHash, true);
            magnetEvent.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            animator.SetBool(PressedHash, false);
    }
}
