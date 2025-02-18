using System.Collections.Generic;
using UnityEngine;

public class MagneticManager : MonoBehaviour
{
    public static MagneticManager Instance; // Singleton for easy access

    private List<MagneticField> activeMagneticObjects = new List<MagneticField>();
    private List<string> interactionsLog = new List<string>(); // Track interactions

    // Inspector exposed fields
    public GameObject leftArm; // Reference to the left arm (drag in the Inspector)
    public GameObject rightArm; // Reference to the right arm (drag in the Inspector)
    public GameObject target; // Specific target object (drag in the Inspector)

    // canGrapple flag (will be set to true when attraction happens)
    public bool canGrapple = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Check interactions between all active magnetic objects
        for (int i = 0; i < activeMagneticObjects.Count; i++)
        {
            for (int j = i + 1; j < activeMagneticObjects.Count; j++)
            {
                MagneticField objA = activeMagneticObjects[i];
                MagneticField objB = activeMagneticObjects[j];

                if (objA == null || objB == null) continue;

                float distance = Vector3.Distance(objA.transform.position, objB.transform.position);

                // Check if they are close enough to interact (e.g., 0.1 distance threshold)
                if (distance <= 1f)
                {
                    // Determine if the objects are attracting or repelling
                    bool isAttracting = objA.isPositivePolarity != objB.isPositivePolarity; // Opposite polarity attracts
                    string interactionType = isAttracting ? "Attracting" : "Repelling";

                    // Log or store the interaction
                    interactionsLog.Add($"Interaction between {objA.gameObject.name} and {objB.gameObject.name}: {interactionType}");

                    // Optionally, you can log it to the console for debugging purposes
                    Debug.Log($"Interaction between {objA.gameObject.name} and {objB.gameObject.name}: {interactionType}");

                    // Check if the attraction is happening between either arm and the target object
                    if (isAttracting && (objB.gameObject == leftArm || objB.gameObject == rightArm) && objA.gameObject == target)
                    {
                        canGrapple = true; // Enable canGrapple flag
                        Debug.Log("Can Grapple is now ENABLED!");
                    }
                    else
                    {
                        canGrapple = false; // Disable if attraction does not happen between arms and the target
                    }
                }
            }
        }
    }

    public void RegisterMagneticObject(MagneticField obj)
    {
        if (!activeMagneticObjects.Contains(obj))
            activeMagneticObjects.Add(obj);
    }

    public void UnregisterMagneticObject(MagneticField obj)
    {
        if (activeMagneticObjects.Contains(obj))
            activeMagneticObjects.Remove(obj);
    }

    // Optionally: Provide a way to get the logged interactions
    public List<string> GetInteractionsLog()
    {
        return interactionsLog;
    }

    // Getter for canGrapple flag
    public bool CanGrapple()
    {
        return canGrapple;
    }
}
