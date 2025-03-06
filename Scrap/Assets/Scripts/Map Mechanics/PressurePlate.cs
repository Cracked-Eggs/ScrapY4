using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    Animator animator;
    const string PressedHash = "isPressed";
   

    [SerializeField] public UnityEvent magnetEvent;
    [SerializeField] UnityEvent offMagnetEvent;

    // Flags for each body part
    public bool isHeadOnPlate = false;
    public bool isTorsoOnPlate = false;
    public bool isRightArmOnPlate = false;
    public bool isLeftArmOnPlate = false;
    public bool isRightLegOnPlate = false;
    public bool isLeftLegOnPlate = false;

    public HashSet<GameObject> objectsOnPlate = new HashSet<GameObject>();
    private int previousObjectCount = 0; // Track previous frame's count

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsValidObject(other))
        {
            objectsOnPlate.Add(other.gameObject);
            PrintObjectsOnPlate();
            animator.SetBool(PressedHash, true);
            magnetEvent.Invoke();

            // Set flags based on the object that collided
            SetBodyPartOnPlate(other.gameObject, true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsValidObject(other))
        {
            objectsOnPlate.Remove(other.gameObject);
            PrintObjectsOnPlate();

            // Reset flags when the object leaves the plate
            SetBodyPartOnPlate(other.gameObject, false);
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
        return other.CompareTag("Player") || other.CompareTag("R_Arm") || other.CompareTag("L_Arm") || other.CompareTag("Head") || other.CompareTag("Torso") || other.CompareTag("R_Leg") || other.CompareTag("L_Leg");
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

    // Method to set the body part flags
    private void SetBodyPartOnPlate(GameObject bodyPart, bool isOnPlate)
    {
        if (bodyPart.CompareTag("Head"))
        {
            isHeadOnPlate = isOnPlate;
        }
        else if (bodyPart.CompareTag("Torso"))
        {
            isTorsoOnPlate = isOnPlate;
        }
        else if (bodyPart.CompareTag("R_Arm"))
        {
            isRightArmOnPlate = isOnPlate;
        }
        else if (bodyPart.CompareTag("L_Arm"))
        {
            isLeftArmOnPlate = isOnPlate;
        }
        else if (bodyPart.CompareTag("R_Leg"))
        {
            isRightLegOnPlate = isOnPlate;
        }
        else if (bodyPart.CompareTag("L_Leg"))
        {
            isLeftLegOnPlate = isOnPlate;
        }

        
        Debug.Log($"Head: {isHeadOnPlate}, Torso: {isTorsoOnPlate}, Right Arm: {isRightArmOnPlate}, Left Arm: {isLeftArmOnPlate}, Right Leg: {isRightLegOnPlate}, Left Leg: {isLeftLegOnPlate}");
    }
}
