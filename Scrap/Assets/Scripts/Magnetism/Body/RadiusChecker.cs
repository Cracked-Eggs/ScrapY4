using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

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
    public Attach attachScript;
    public VFXManager vfxManager;
    public PartManager partManager;
  

    public bool isHeadInRange = false;
    public bool isTorsoInRange = false;
    public bool isRightLegInRange = false;
    public bool isLeftLegInRange = false;
    public bool isRightArmInRange = false;
    public bool isLeftArmInRange = false;
    // Priority assignment (higher numbers will be retracted first)
    public Dictionary<GameObject, int> bodyPartPriorities = new Dictionary<GameObject, int>();

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

        vfxManager = GetComponent<VFXManager>();
        // Set up body part priorities
        AssignBodyPartPriorities();
    }

    void AssignBodyPartPriorities()
    {
        // Assign priorities to each body part (lower value = higher priority)
        // Example:
        bodyPartPriorities.Add(attachScript.partManager.head, 1); // Highest priority
        bodyPartPriorities.Add(attachScript.partManager.torso, 2);
        bodyPartPriorities.Add(attachScript.partManager.r_Arm, 5);
        bodyPartPriorities.Add(attachScript.partManager.l_Arm, 6);
        bodyPartPriorities.Add(attachScript.partManager.r_Leg, 4);
        bodyPartPriorities.Add(attachScript.partManager.l_Leg, 3); // Lowest priority
    }

    void Update()
    {
        if (targetBodyParts.Count > 0)
        {
            partManager.isReattaching = true;
        }
        else
        {
            partManager.isReattaching = false;
        }
        if (isRepelling)
        {
            RepelObjects(); // Handle repelling logic
        }

        // Only retract target body parts if isRetracting is true and there are targets
        if (isRetracting && targetBodyParts.Count > 0)
        {
            // Sort body parts by priority before retracting
            SortBodyPartsByPriority();

            // Start the coroutine to retract body parts one by one
            StartCoroutine(RetractBodyPartOneByOne());
        }

        // If all body parts are reattached, clear the targets and stop retraction
        if (currentBodyParts == totalBodyParts)
        {
            // Reset in-range flags for all body parts when all parts are reattached
            ResetInRangeFlags();

            targetBodyParts.Clear(); // Clear the list of target body parts
            Debug.Log("All body parts reattached!");
            isRetracting = false;
        }

        // Check if any body part is inside the secondary radius
        CheckBodyPartsInSecondaryRadius();

        // Check if any body part is inside the main radius
        CheckBodyPartsInMainRadius();
    }

    void ResetInRangeFlags()
    {
        // Reset the in-range flags for all parts when they are reattached
        isHeadInRange = false;
        isTorsoInRange = false;
        isRightLegInRange = false;
        isLeftLegInRange = false;
        isRightArmInRange = false;
        isLeftArmInRange = false;

        // Also reset the main radius flag
        isBodyPartInMainRange = false;
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

    // Coroutine to retract body parts one by one
    private IEnumerator RetractBodyPartOneByOne()
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
                        vfxManager.PlayVFX(bodyPart.tag);
                        
                        // Wait for the next frame before continuing (you can adjust this time as needed)
                        yield return new WaitForSeconds(0.3f);
                    }
                }
            }
        }

        // After finishing all retractions, stop the retraction process
        isRetracting = false;
    }

    void SortBodyPartsByPriority()
    {
        // Sort the targetBodyParts list based on the priority defined in bodyPartPriorities
        targetBodyParts.Sort((part1, part2) =>
        {
            // Retrieve the priority for each body part (lower priority number is higher priority)
            int priority1 = bodyPartPriorities.ContainsKey(part1) ? bodyPartPriorities[part1] : int.MaxValue;
            int priority2 = bodyPartPriorities.ContainsKey(part2) ? bodyPartPriorities[part2] : int.MaxValue;

            return priority1.CompareTo(priority2); // Compare priorities
        });

        // Debug log to see the order of body parts
        foreach (var part in targetBodyParts)
        {
            Debug.Log("Body part: " + part.name + " Priority: " + bodyPartPriorities[part]);
        }
    }
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
                    // Update the corresponding public bool for each body part if it's detached
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
                    // Update the corresponding public bool for each body part if it's detached
                    if (bodyPart == attachScript.partManager.head) isHeadInRange = true;
                    if (bodyPart == attachScript.partManager.torso) isTorsoInRange = true;
                    if (bodyPart == attachScript.partManager.r_Leg) isRightLegInRange = true;
                    if (bodyPart == attachScript.partManager.l_Leg) isLeftLegInRange = true;
                    if (bodyPart == attachScript.partManager.r_Arm) isRightArmInRange = true;
                    if (bodyPart == attachScript.partManager.l_Arm) isLeftArmInRange = true;

                    // Update the main radius flag
                    isBodyPartInMainRange = true;

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

                    // Reset the main radius flag
                    isBodyPartInMainRange = false;
                }
            }
        }
    }

    public void UpdateBodyPartCount(int change)
    {
        currentBodyParts += change;
        currentBodyParts = Mathf.Clamp(currentBodyParts, 0, totalBodyParts); // Ensure it stays within bounds
        Debug.Log($"Updated body part count: {currentBodyParts}/{totalBodyParts}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius); // Main radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, secondaryRadius); // Secondary radius
    }

    public void AddStrength()
    {
        forceStrength = 30f;
    }

    public void DecreaseStrength()
    {
        forceStrength = 10f;
    }
}
