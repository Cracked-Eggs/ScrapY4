using UnityEngine;

public class MagneticField : MonoBehaviour
{
    public bool isPositivePolarity = true;  // True for positive, false for negative
    public float weight = 10f;              // Custom weight (NOT Rigidbody mass)
    public float maxForce = 50f;            // Maximum force applied
    public float fieldRadius = 10f;         // Magnetic field range
    public float stickThreshold = 0.5f;     // Distance at which objects stick together
    public float slowDownFactor = 5f;       // Controls how much attraction force decelerates
    private SphereCollider sphereCollider;

    void Start()
    {
        // Set up the Sphere Collider as a trigger
        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = fieldRadius;  // Adjust the field radius
    }

    void OnTriggerStay(Collider other)
    {
        // If the other object has a Rigidbody and a MagneticField component
        MagneticField otherMagneticField = other.GetComponent<MagneticField>();
        Rigidbody otherRb = other.attachedRigidbody;

        if (otherRb && otherMagneticField)
        {
            Vector3 direction = transform.position - other.transform.position;
            float distance = direction.magnitude;

            // Ensure the object is within the field radius and has a valid distance
            if (distance < sphereCollider.radius && distance > 0.01f) // Avoid division by zero
            {
                direction.Normalize();  // Normalize the direction vector

                // Determine if they should attract or repel
                bool shouldAttract = isPositivePolarity != otherMagneticField.isPositivePolarity;

                // Calculate force based on weight difference
                float totalWeight = weight + otherMagneticField.weight;
                float baseForce = Mathf.Clamp(weight / (distance * slowDownFactor), 0, maxForce); // Decrease force as distance decreases

                // The heavier object resists movement more
                float thisWeightFactor = otherMagneticField.weight / totalWeight;
                float otherWeightFactor = weight / totalWeight;

                // Apply forces
                if (shouldAttract)
                {
                    // If they are very close, make them stick together
                   
                    {
                        // Opposite polarities: Attract (slowing down as they get closer)
                        otherRb.AddForce(direction * baseForce * otherWeightFactor);
                        this.GetComponent<Rigidbody>().AddForce(-direction * baseForce * thisWeightFactor);
                    }
                }
                else
                {
                    // Same polarities: Repel (normal repulsion behavior)
                    otherRb.AddForce(-direction * baseForce * otherWeightFactor);
                    this.GetComponent<Rigidbody>().AddForce(direction * baseForce * thisWeightFactor);
                }
            }
        }
    }

  
}
