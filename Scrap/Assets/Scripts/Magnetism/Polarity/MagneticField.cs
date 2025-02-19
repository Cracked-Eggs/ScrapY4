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
        MagneticManager.Instance.RegisterMagneticObject(this);
        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = fieldRadius;  // Adjust the field radius
    }
    void OnDestroy()
    {
        MagneticManager.Instance.UnregisterMagneticObject(this);
    }
    void OnTriggerStay(Collider other)
    {
        MagneticField otherMagneticField = other.GetComponent<MagneticField>();
        Rigidbody otherRb = other.attachedRigidbody;
        Rigidbody thisRb = GetComponent<Rigidbody>();

        if (otherRb && otherMagneticField)
        {
            Vector3 direction = transform.position - other.transform.position;
            float distance = direction.magnitude;

            // If objects are within 0.4f, slow down both linear and angular velocity
            if (distance <= 1f)
            {
                thisRb.velocity *= 0.9f; // Gradually slow movement
                otherRb.velocity *= 0.9f;

                thisRb.angularVelocity *= 0.9f; // Gradually slow rotation
                otherRb.angularVelocity *= 0.9f;

                // If they are extremely close and nearly stopped, reduce motion even further
                if (distance <= 0.1f && thisRb.velocity.magnitude < 0.05f && otherRb.velocity.magnitude < 0.05f)
                {
                    thisRb.velocity = Vector3.zero;
                    otherRb.velocity = Vector3.zero;

                    thisRb.angularVelocity = Vector3.zero; // Stop rotation gradually
                    otherRb.angularVelocity = Vector3.zero;
                }

                return; // Stop applying forces once they are close enough
            }

            direction.Normalize();
            bool shouldAttract = isPositivePolarity != otherMagneticField.isPositivePolarity;

            float totalWeight = weight + otherMagneticField.weight;
            float baseForce = Mathf.Clamp(weight / (distance * slowDownFactor), 0, maxForce);

            float thisWeightFactor = otherMagneticField.weight / totalWeight;
            float otherWeightFactor = weight / totalWeight;

            if (shouldAttract)
            {
                otherRb.AddForce(direction * baseForce * otherWeightFactor);
                thisRb.AddForce(-direction * baseForce * thisWeightFactor);
            }
            else
            {
                otherRb.AddForce(-direction * baseForce * otherWeightFactor);
                thisRb.AddForce(direction * baseForce * thisWeightFactor);
            }
        }
    }





}
