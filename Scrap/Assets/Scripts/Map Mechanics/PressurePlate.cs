using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    // Animation & Audio
    private Animator animator;
    private const string PressedHash = "isPressed";
    private bool wasPressed = false; // Tracks previous state to detect changes

    [SerializeField] private AudioClip pressurePlateSound;
    private AudioSource audioSource;

    // Events
    [SerializeField] public UnityEvent magnetEvent; // Fires when plate is pressed
    [SerializeField] public UnityEvent offMagnetEvent; // Fires when plate is released

    // Body part tracking
    public bool isHeadOnPlate = false;
    public bool isTorsoOnPlate = false;
    public bool isRightArmOnPlate = false;
    public bool isLeftArmOnPlate = false;
    public bool isRightLegOnPlate = false;
    public bool isLeftLegOnPlate = false;

    // Object tracking
    public HashSet<GameObject> objectsOnPlate = new HashSet<GameObject>();
    private int previousObjectCount = 0;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) // Ensure AudioSource exists
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsValidObject(other))
        {
            objectsOnPlate.Add(other.gameObject);
            SetBodyPartOnPlate(other.gameObject, true);
            UpdatePlateState();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsValidObject(other))
        {
            objectsOnPlate.Remove(other.gameObject);
            SetBodyPartOnPlate(other.gameObject, false);
            UpdatePlateState();
        }
    }

    void FixedUpdate()
    {
        // Clean up destroyed objects
        objectsOnPlate.RemoveWhere(obj => obj == null || !obj.activeInHierarchy || !IsStillInTrigger(obj));
        UpdatePlateState();
    }

    void UpdatePlateState()
    {
        bool shouldBePressed = objectsOnPlate.Count > 0;

        // Only update if state changed
        if (shouldBePressed != wasPressed)
        {
            animator.SetBool(PressedHash, shouldBePressed);

            if (shouldBePressed)
            {
                magnetEvent.Invoke();
            }
            else
            {
                offMagnetEvent.Invoke();
            }

            PlayPressurePlateSound();
            wasPressed = shouldBePressed;
        }

        previousObjectCount = objectsOnPlate.Count;
    }

    void PlayPressurePlateSound()
    {
        if (pressurePlateSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pressurePlateSound);
        }
    }

    bool IsValidObject(Collider other)
    {
        return other.CompareTag("Player") || 
               other.CompareTag("R_Arm") || 
               other.CompareTag("L_Arm") || 
               other.CompareTag("Head") || 
               other.CompareTag("Torso") || 
               other.CompareTag("R_Leg") || 
               other.CompareTag("L_Leg");
    }

    bool IsStillInTrigger(GameObject obj)
    {
        Collider objCollider = obj.GetComponent<Collider>();
        return objCollider != null && GetComponent<Collider>().bounds.Intersects(objCollider.bounds);
    }

    void SetBodyPartOnPlate(GameObject bodyPart, bool isOnPlate)
    {
        if (bodyPart.CompareTag("Head")) isHeadOnPlate = isOnPlate;
        else if (bodyPart.CompareTag("Torso")) isTorsoOnPlate = isOnPlate;
        else if (bodyPart.CompareTag("R_Arm")) isRightArmOnPlate = isOnPlate;
        else if (bodyPart.CompareTag("L_Arm")) isLeftArmOnPlate = isOnPlate;
        else if (bodyPart.CompareTag("R_Leg")) isRightLegOnPlate = isOnPlate;
        else if (bodyPart.CompareTag("L_Leg")) isLeftLegOnPlate = isOnPlate;

    }
}