using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimedPressurePlate : MonoBehaviour
{
    Animator animator;
    const string PressedHash = "isPressed";

    [SerializeField] UnityEvent magnetEvent;     // Event triggered when pressed
    [SerializeField] UnityEvent offMagnetEvent;  // Event triggered when timer ends
    [SerializeField] private float pressDuration = 3f; // Time in seconds before resetting

    private HashSet<GameObject> objectsOnPlate = new HashSet<GameObject>();
    private bool isActivated = false;

    void Awake() => animator = GetComponentInChildren<Animator>();

    void OnTriggerEnter(Collider other)
    {
        if (!isActivated && IsValidObject(other))
        {
            objectsOnPlate.Add(other.gameObject);
            animator.SetBool(PressedHash, true);
            magnetEvent.Invoke();
            isActivated = true;
            StartCoroutine(ResetAfterDelay());
        }
    }

    private System.Collections.IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(pressDuration);
        animator.SetBool(PressedHash, false);
        offMagnetEvent.Invoke();
        isActivated = false;
    }

    bool IsValidObject(Collider other)
    {
        return other.CompareTag("Player") || other.CompareTag("R_Arm") || other.CompareTag("L_Arm") || other.CompareTag("Head");
    }
}
