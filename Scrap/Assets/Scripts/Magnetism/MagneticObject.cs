using UnityEngine;

public class MagneticObject : MonoBehaviour
{
    [SerializeField] Transform magnet;
    [SerializeField] float magnetForce = 10f;
    [SerializeField] float maxDistance = 10f;

    Rigidbody rb;
    bool isActive;

    void Awake() => rb = GetComponent<Rigidbody>();
    
    public void ActivateMagnet() => isActive = true;

    void FixedUpdate()
    {
        if (isActive == true)
        {
            Vector3 direction = magnet.position - transform.position;
            float distance = direction.magnitude;

            if (distance <= maxDistance)
            {
                direction.Normalize();
                rb.useGravity = false;
                rb.AddForce(direction * magnetForce * (1 - distance / maxDistance), ForceMode.Acceleration);
            }
            else
                rb.useGravity = true;
        }
    }
}