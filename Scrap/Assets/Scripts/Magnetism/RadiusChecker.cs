using System.Collections.Generic;
using UnityEngine;

public class RadiusChecker : MonoBehaviour
{
    public List<GameObject> bodyParts; // List of body parts to check
    public float radius = 5f; // Main radius to check around the object
    public float secondaryRadius = 3f; // Secondary radius for checking proximity
    public float forceStrength = 10f; // Strength of the force
    public bool isRepelling = false; // Toggle repelling
    public bool isRetracting = false; // Toggle retracting
    public bool isBodyPartInRange = false; // Flag to check if body part is in secondary radius
    public bool isBodyPartInMainRange = false; // Flag to check if body part is in main radius

    // Public bools for each body part in range
    public bool isHeadInRange = false;
    public bool isTorsoInRange = false;
    public bool isRightLegInRange = false;
    public bool isLeftLegInRange = false;
    public bool isRightArmInRange = false;
    public bool isLeftArmInRange = false;

    // Reference to the Attach script to check the detached status
    public Attach attachScript;

    // Store the current body part being recalled
    public List<GameObject> targetBodyParts = new List<GameObject>();

    // New variables for tracking body parts count
    public int totalBodyParts;  // Total body parts (including detached)
    public int currentBodyParts;  // Currently attached body parts

    void Start()
    {
        // Initialize body parts count
        totalBodyParts = bodyParts.Count; // This is the total number of body parts
        currentBodyParts = totalBodyParts; // Initially, all parts are attached
    }

    void Update()
    {
        if (isRepelling)
        {
            RepelObjects(); // Handle repelling logic
        }

        // Only retract target body parts if isRetracting is true and there are targets
        if (isRetracting && targetBodyParts.Count > 0)
        {
            RetractObjects(); // Retract all target body parts
        }

        // If all body parts are reattached, clear the targets and stop retraction
        if (currentBodyParts == totalBodyParts)
        {
            targetBodyParts.Clear(); // Clear the list of target body parts
            Debug.Log("All body parts reattached!");
            isRetracting = false;
        }

        // Check if any body part is inside the secondary radius
        CheckBodyPartsInSecondaryRadius();

        // Check if any body part is inside the main radius
        CheckBodyPartsInMainRadius();
    }

    void RepelObjects()
    {
        foreach (var bodyPart in bodyParts)
        {
            if (bodyPart != null)
            {
                float distance = Vector3.Distance(transform.position, bodyPart.transform.position);
                if (distance <= radius)
                {
                    Rigidbody rb = bodyPart.GetComponent<Rigidbody>();
                    if (rb != null) // Ensure the object has a Rigidbody
                    {
                        Vector3 direction = bodyPart.transform.position - transform.position; // Get direction away from center
                        direction.Normalize(); // Normalize to maintain consistent force

                        rb.AddForce(direction * forceStrength / distance, ForceMode.Impulse);
                        Debug.Log("Repelled: " + bodyPart.name);
                    }
                }
            }
        }
    }

    // Method to retract the specific body part
    void RetractObjects()
    {
        foreach (GameObject bodyPart in targetBodyParts)
        {
            if (bodyPart != null) // Ensure we have a valid target
            {
                float distance = Vector3.Distance(transform.position, bodyPart.transform.position);
                if (distance <= radius)
                {
                    Rigidbody rb = bodyPart.GetComponent<Rigidbody>();
                    if (rb != null) // Ensure the object has a Rigidbody
                    {
                        Vector3 direction = transform.position - bodyPart.transform.position; // Direction toward center
                        direction.Normalize(); // Normalize force

                        rb.AddForce(direction * forceStrength / distance, ForceMode.Impulse);
                        Debug.Log("Retracted: " + bodyPart.name);
                    }
                }
            }
        }
    }


    // Method to check if any body part is inside the secondary radius
    void CheckBodyPartsInSecondaryRadius()
    {
        foreach (var bodyPart in bodyParts)
        {
            if (bodyPart != null)
            {
                // Skip checking body parts that are not detached
                if (attachScript != null && !attachScript.IsBodyPartDetached(bodyPart))
                    continue;

                float distance = Vector3.Distance(transform.position, bodyPart.transform.position);
                if (distance <= secondaryRadius)
                {
                    // Update the corresponding public bool for each body part
                    if (bodyPart == attachScript.partManager.head) isHeadInRange = true;
                    if (bodyPart == attachScript.partManager.torso) isTorsoInRange = true;
                    if (bodyPart == attachScript.partManager.r_Leg) isRightLegInRange = true;
                    if (bodyPart == attachScript.partManager.l_Leg) isLeftLegInRange = true;
                    if (bodyPart == attachScript.partManager.r_Arm) isRightArmInRange = true;
                    if (bodyPart == attachScript.partManager.l_Arm) isLeftArmInRange = true;


                    Debug.Log("Body part " + bodyPart.name + " is inside the secondary radius.");
                }
                else
                {
                    // Reset the bool if the body part is no longer in range
                    if (bodyPart == attachScript.partManager.head) isHeadInRange = false;
                    if (bodyPart == attachScript.partManager.torso) isTorsoInRange = false;
                    if (bodyPart == attachScript.partManager.r_Leg) isRightLegInRange = false;
                    if (bodyPart == attachScript.partManager.l_Leg) isLeftLegInRange = false;
                    if (bodyPart == attachScript.partManager.r_Arm) isRightArmInRange = false;
                    if (bodyPart == attachScript.partManager.l_Arm) isLeftArmInRange = false;

                }
            }
        }
    }

    // Method to check if any body part is inside the main radius
    void CheckBodyPartsInMainRadius()
    {
        foreach (var bodyPart in bodyParts)
        {
            if (bodyPart != null)
            {
                // Skip checking body parts that are not detached
                if (attachScript != null && !attachScript.IsBodyPartDetached(bodyPart))
                    continue;

                float distance = Vector3.Distance(transform.position, bodyPart.transform.position);
                if (distance <= radius)
                {
                    // Update the corresponding public bool for each body part
                    if (bodyPart == attachScript.partManager.head) isHeadInRange = true;
                    if (bodyPart == attachScript.partManager.torso) isTorsoInRange = true;
                    if (bodyPart == attachScript.partManager.r_Leg) isRightLegInRange = true;
                    if (bodyPart == attachScript.partManager.l_Leg) isLeftLegInRange = true;
                    if (bodyPart == attachScript.partManager.r_Arm) isRightArmInRange = true;
                    if (bodyPart == attachScript.partManager.l_Arm) isLeftArmInRange = true;
                    if (bodyPart == attachScript.partManager.head) isBodyPartInMainRange = true;
                    if (bodyPart == attachScript.partManager.torso) isBodyPartInMainRange = true;
                    if (bodyPart == attachScript.partManager.r_Leg) isBodyPartInMainRange = true;
                    if (bodyPart == attachScript.partManager.l_Leg) isBodyPartInMainRange = true;
                    if (bodyPart == attachScript.partManager.r_Arm) isBodyPartInMainRange = true;
                    if (bodyPart == attachScript.partManager.l_Arm) isBodyPartInMainRange = true;
                    Debug.Log("Body part " + bodyPart.name + " is inside the main radius.");
                }
                else
                {
                    // Reset the bool if the body part is no longer in range
                    if (bodyPart == attachScript.partManager.head) isHeadInRange = false;
                    if (bodyPart == attachScript.partManager.torso) isTorsoInRange = false;
                    if (bodyPart == attachScript.partManager.r_Leg) isRightLegInRange = false;
                    if (bodyPart == attachScript.partManager.l_Leg) isLeftLegInRange = false;
                    if (bodyPart == attachScript.partManager.r_Arm) isRightArmInRange = false;
                    if (bodyPart == attachScript.partManager.l_Arm) isLeftArmInRange = false;
                    if (bodyPart == attachScript.partManager.head) isBodyPartInMainRange = false;
                    if (bodyPart == attachScript.partManager.torso) isBodyPartInMainRange = false;
                    if (bodyPart == attachScript.partManager.r_Leg) isBodyPartInMainRange = false;
                    if (bodyPart == attachScript.partManager.l_Leg) isBodyPartInMainRange = false;
                    if (bodyPart == attachScript.partManager.r_Arm) isBodyPartInMainRange = false;
                    if (bodyPart == attachScript.partManager.l_Arm) isBodyPartInMainRange = false;
                }
            }
        }
    }

    // Method to update body part counts when a part is detached or reattached
    
    // Optional: Visualize both radii in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius); // Main radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, secondaryRadius); // Secondary radius
    }
}
