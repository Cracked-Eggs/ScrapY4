using UnityEngine;

public class MagneticObject : MonoBehaviour
{
    [SerializeField] Transform magnet;
    [SerializeField] float magnetForce = 10f;
    [SerializeField] float maxDistance = 10f;

    Rigidbody rb;

    void Awake() => rb = GetComponent<Rigidbody>();

    void FixedUpdate()
    {
        if (magnet == null)
            return;

        Vector3 direction = magnet.position - transform.position;
        float distance = direction.magnitude;

        if (distance <= maxDistance)
        {
            direction.Normalize();
            rb.AddForce(direction * magnetForce * (1 - distance / maxDistance), ForceMode.Force);
        }
    }
}