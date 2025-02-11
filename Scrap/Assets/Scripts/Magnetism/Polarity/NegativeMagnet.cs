using UnityEngine;

public class NegativeMagnet : MonoBehaviour
{
    // Reference to other magnets (no player involved)
    [SerializeField] private float attractionForce = 10f; // Strength of magnetic attraction
    [SerializeField] private float maxVelocity = 5f;  // Max velocity to prevent uncontrolled movement
    private Rigidbody rb;
    private SphereCollider field;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        field = GetComponent<SphereCollider>();
        rb.mass = CalculateMass(transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void FixedUpdate()
    {
        // Clamp Rigidbody velocity to prevent excessive speed
        ClampVelocity(rb, maxVelocity);
    }

    void OnTriggerStay(Collider other)
    {
        // Check if the other object is a magnet (can be both positive or negative)
        Magnet otherMagnet = other.GetComponent<Magnet>();
        if (otherMagnet != null)
        {
            // If other magnet is negative, repel; if positive, attract
            if (otherMagnet.polarity == Magnet.Polarity.Negative)
            {
                // Repel (same polarity)
                Repel(other);
            }
            else if (otherMagnet.polarity == Magnet.Polarity.Positive)
            {
                // Attract (opposite polarity)
                Attract(other);
            }
        }
    }

    void Attract(Collider other)
    {
        // Attraction logic (towards opposite polarity)
        Vector3 direction = (other.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, other.transform.position);

        // Apply force towards the other magnet
        float forceMagnitude = attractionForce / Mathf.Pow(distance, 2);  // Use inverse square law
        rb.AddForce(direction * forceMagnitude);
    }

    void Repel(Collider other)
    {
        // Repulsion logic (away from same polarity)
        Vector3 direction = (transform.position - other.transform.position).normalized;
        float distance = Vector3.Distance(transform.position, other.transform.position);

        // Apply force away from the other magnet
        float forceMagnitude = attractionForce / Mathf.Pow(distance, 2);  // Use inverse square law
        rb.AddForce(direction * forceMagnitude);
    }

    // Helper function to clamp the Rigidbody velocity
    void ClampVelocity(Rigidbody rb, float maxSpeed)
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    // Helper function to calculate the mass based on the scale of the object
    float CalculateMass(float x, float y, float z)
    {
        return x * y * z * 0.1f;  // Example formula for mass based on scale
    }
}
