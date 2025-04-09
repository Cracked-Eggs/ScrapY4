using Unity.VisualScripting;
using UnityEngine;

public class VentEntrance : MonoBehaviour
{
    public VentPathData ventPath; // Assign in Inspector
    public float attractionForce = 5f; // Pull parts into the vent

    
    void OnTriggerEnter(Collider other)
    {
        

        // 2. Check if it has a Rigidbody (indicates it's a physics-based part)
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return;

        // 3. OPTIONAL: Verify it's detached (if your system tracks this)
        // Example: Check if it has a FlowField component (meaning it's retracting)
        FlowField flowField = other.GetComponent<FlowField>();
        if (flowField == null) return; // Skip if not retracting

        // 4. Start vent path following
        VentPathFollower ventFollower = other.GetComponent<VentPathFollower>();
        if (ventFollower == null)
        {
            ventFollower = other.AddComponent<VentPathFollower>();
            Debug.Log("adding path follower");
        }
        ventFollower.StartFollowing(ventPath);

        // 5. Disable flow field to avoid interference
        if (flowField != null)
        {
            flowField.enabled = false;
        }
    }
}