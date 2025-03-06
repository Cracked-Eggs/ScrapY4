using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TogglePressurePlate : MonoBehaviour
{
    Animator animator;
    const string PressedHash = "isPressed";

    [SerializeField] public UnityEvent magnetEvent;
    private bool isActivated = false; // Tracks if the plate has been pressed

    void Awake() => animator = GetComponentInChildren<Animator>();

    void OnTriggerEnter(Collider other)
    {
        if (!isActivated && IsValidObject(other))
        {
            isActivated = true; // Lock the plate in the pressed state
            animator.SetBool(PressedHash, true);
            magnetEvent.Invoke();
            Debug.Log("Pressure plate activated!");
        }
    }
    public void ResetPlate()
    {
        isActivated = false; // Unlock the plate
        animator.SetBool(PressedHash, false); // Reset animation
    }


    bool IsValidObject(Collider other)
    {
        return other.CompareTag("Player") || other.CompareTag("R_Arm") || other.CompareTag("L_Arm") || other.CompareTag("Head");
    }
}