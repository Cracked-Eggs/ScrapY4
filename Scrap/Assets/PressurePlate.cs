using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    Animator animator;
    const string PressedHash = "isPressed";

    [SerializeField] UnityEvent magnetEvent;
    [SerializeField] UnityEvent offMagnetEvent;

    public HashSet<GameObject> objectsOnPlate = new HashSet<GameObject>();
    private int previousObjectCount = 0; // Track previous frame's count

    void Awake() => animator = GetComponentInChildren<Animator>();

    void OnTriggerEnter(Collider other)
    {
        if (IsValidObject(other))
        {
            objectsOnPlate.Add(other.gameObject);
            PrintObjectsOnPlate();
            animator.SetBool(PressedHash, true);
            magnetEvent.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsValidObject(other))
        {
            objectsOnPlate.Remove(other.gameObject);
            PrintObjectsOnPlate();
        }
    }

    void FixedUpdate()
    {
        // Store the previous object count before updating the set
        int currentObjectCount = objectsOnPlate.Count;

        // Remove objects that are no longer valid (destroyed or moved)
        objectsOnPlate.RemoveWhere(obj => obj == null || !obj.activeInHierarchy || !IsStillInTrigger(obj));

        // Check for transition from occupied to empty
        if (previousObjectCount > 0 && objectsOnPlate.Count == 0)
        {
            animator.SetBool(PressedHash, false);
            offMagnetEvent.Invoke();
        }

        // Update previous count for the next frame
        previousObjectCount = objectsOnPlate.Count;
    }

    bool IsValidObject(Collider other)
    {
        return other.CompareTag("Player") || other.CompareTag("R_Arm") || other.CompareTag("L_Arm") || other.CompareTag("Head");
    }

    bool IsStillInTrigger(GameObject obj)
    {
        Collider objCollider = obj.GetComponent<Collider>();
        if (objCollider == null) return false;

        return GetComponent<Collider>().bounds.Intersects(objCollider.bounds);
    }

    void PrintObjectsOnPlate()
    {
        if (objectsOnPlate.Count > 0)
        {
            Debug.Log("Objects on plate: " + string.Join(", ", objectsOnPlate));
        }
        else
        {
            Debug.Log("Pressure plate is empty.");
        }
    }
}