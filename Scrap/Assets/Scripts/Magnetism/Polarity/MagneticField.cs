using UnityEngine;

public class MagneticField : MonoBehaviour
{
    public float magneticFieldRadius = 5f; // Radius of the magnetic field
    private SphereCollider fieldCollider;

    void Start()
    {
        // Add a sphere collider to represent the magnetic field
        fieldCollider = gameObject.AddComponent<SphereCollider>();
        fieldCollider.radius = magneticFieldRadius;
        fieldCollider.isTrigger = true; // Make it a trigger for detection
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is a MagneticObject and within the range
        MagneticObject magneticObject = other.GetComponent<MagneticObject>();
        if (magneticObject != null)
        {
            // Change the color of the object when it enters the magnetic field
            magneticObject.OnEnterMagneticField(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object is a MagneticObject and has left the field
        MagneticObject magneticObject = other.GetComponent<MagneticObject>();
        if (magneticObject != null)
        {
            // Revert the color when the object exits the magnetic field
            magneticObject.OnEnterMagneticField(false);
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize the magnetic field radius with a wire sphere
        Gizmos.color = Color.green; // Color for the field radius
        Gizmos.DrawWireSphere(transform.position, magneticFieldRadius);
    }
}
