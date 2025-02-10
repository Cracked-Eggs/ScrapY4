using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnetic : MonoBehaviour
{
    public float magneticForce = 10f; // Force to attract objects
    public float magneticRadius = 5f; // Radius of magnetic influence

    private void OnTriggerStay(Collider other)
    {
        // Check if the object is a detached arm
        if (other.CompareTag("DetachedArm"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calculate direction to the magnetic object
                Vector3 direction = (transform.position - other.transform.position).normalized;

                // Apply force to move the arm toward the magnetic object
                rb.AddForce(direction * magneticForce, ForceMode.Force);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the magnetic radius in the editor
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, magneticRadius);
    }
}
