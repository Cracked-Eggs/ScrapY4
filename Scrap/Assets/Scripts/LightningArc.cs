using UnityEngine;

public class LightningArcUpdater : MonoBehaviour
{
    public Transform P1; // Torso (Start Point)
    public Transform P2; // Torso Control Point (To be adjusted)
    public Transform P3; // Detached Part Control Point (To be adjusted)
    public Transform P4; // Detached Part (End Point)

    public float arcHeight = 1.5f; // Controls how high the arc bends

    void Update()
    {
        if (P1 == null || P2 == null || P3 == null || P4 == null) return;

        // Find midpoints along the straight line
        Vector3 midPoint1 = Vector3.Lerp(P1.position, P4.position, 0.4f); // Closer to P1
        Vector3 midPoint2 = Vector3.Lerp(P1.position, P4.position, 0.6f); // Closer to P4

        // Calculate direction and perpendicular vector
        Vector3 direction = (P4.position - P1.position).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up); // Get perpendicular direction

        // Move control points for a nice arc
        P2.position = midPoint1 + perpendicular * arcHeight; // Push outward
        P3.position = midPoint2 - perpendicular * arcHeight; // Push opposite side
    }
}
