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

                // Check if they are close enough to interact
                if (distance <= 1f)
                {
                    // Determine if the objects are attracting or repelling
                    bool isAttracting = objA.isPositivePolarity != objB.isPositivePolarity; // Opposite polarity attracts
                    string interactionType = isAttracting ? "Attracting" : "Repelling";

                    // Log interaction
                    interactionsLog.Add($"Interaction between {objA.gameObject.name} and {objB.gameObject.name}: {interactionType}");
                    Debug.Log($"Interaction between {objA.gameObject.name} and {objB.gameObject.name}: {interactionType}");

                    // Check if attraction is happening between either arm and an object with the target tag
                    if (isAttracting && (objB.gameObject == leftArm || objB.gameObject == rightArm) && objA.gameObject.CompareTag("MagneticWall"))
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

    public List<string> GetInteractionsLog()
    {
        return interactionsLog;
    }

    public bool CanGrapple()
    {
        return canGrapple;
    }
}
