using System;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    Animator animator;
    const string PressedHash = "isPressed";
    
    [SerializeField] UnityEvent magnetEvent;
    [SerializeField] UnityEvent offMagnetEvent;
    
    [SerializeField] int objectsOnPlate = 0; // Counter for objects on the plate

    void Awake() => animator = GetComponentInChildren<Animator>();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("R_Arm") || other.CompareTag("L_Arm"))
        {
            objectsOnPlate++; // Increase counter
            animator.SetBool(PressedHash, true);
            magnetEvent.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("R_Arm") || other.CompareTag("L_Arm"))
        {
            objectsOnPlate--; // Decrease counter

            if (objectsOnPlate <= 0) // Only deactivate if no objects remain
            {
                animator.SetBool(PressedHash, false);
                offMagnetEvent.Invoke();
            }
        }
    }
}