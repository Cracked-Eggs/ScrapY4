using UnityEngine;

public class Magnet : MonoBehaviour
{
    public enum Polarity { Positive, Negative }

    [Header("Magnet Polarity")]
    public Polarity polarity;  // Polarity of this magnet

    public float attractionForce = 10f; // Magnetic force strength
    public float maxVelocity = 5f;  // Max velocity to limit the speed of the magnet

    protected Rigidbody rb;
    public  SphereCollider field;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        field = GetComponent<SphereCollider>();
        //rb.mass = CalculateMass(transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    void FixedUpdate()
    {
        // Clamp velocity
        ClampVelocity(rb, maxVelocity);
    }

    void OnTriggerStay(Collider other)
    {
        // Check if the other object is a magnet
        Magnet otherMagnet = other.GetComponent<Magnet>();
        if (otherMagnet != null)
        {
            // Interaction between magnets based on their polarity
            if (polarity == Polarity.Positive && otherMagnet.polarity == Polarity.Positive)
                Repel(other);
            else if (polarity == Polarity.Negative && otherMagnet.polarity == Polarity.Negative)
                Repel(other);
            else
                Attract(other);
        }
    }

    protected virtual void Attract(Collider other)
    {
        // Attraction logic (towards opposite polarity)
        Vector3 direction = (other.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, other.transform.position);
        float forceMagnitude = attractionForce / Mathf.Pow(distance, 2);  // Inverse square law
        rb.AddForce(direction * forceMagnitude);
    }

    protected virtual void Repel(Collider other)
    {
        // Repulsion logic (away from same polarity)
        Vector3 direction = (transform.position - other.transform.position).normalized;
        float distance = Vector3.Distance(transform.position, other.transform.position);
        float forceMagnitude = attractionForce / Mathf.Pow(distance, 2);  // Inverse square law
        rb.AddForce(direction * forceMagnitude);
    }

    void ClampVelocity(Rigidbody rb, float maxSpeed)
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    float CalculateMass(float x, float y, float z)
    {
        return x * y * z * 0.1f;  // Mass formula
    }
}
