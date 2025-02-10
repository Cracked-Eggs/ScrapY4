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

    void Awake() => animator = GetComponentInChildren<Animator>();

    void OnTriggerEnter(Collider other)
    {
        if (IsValidObject(other))
        {
            objectsOnPlate.Add(other.gameObject); // Add object
            PrintObjectsOnPlate();
            animator.SetBool(PressedHash, true);
            magnetEvent.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsValidObject(other))
        {
            objectsOnPlate.Remove(other.gameObject); // Remove object
            PrintObjectsOnPlate();
        }
    }

    void FixedUpdate()
    {
        // Remove objects that are no longer valid (destroyed or moved)
        objectsOnPlate.RemoveWhere(obj => obj == null || !obj.activeInHierarchy || !IsStillInTrigger(obj));

        // Update plate state
        if (objectsOnPlate.Count == 0)
        {
            animator.SetBool(PressedHash, false);
            offMagnetEvent.Invoke();
        }
    }

    bool IsValidObject(Collider other)
    {
        return other.CompareTag("Player") || other.CompareTag("R_Arm") || other.CompareTag("L_Arm");
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