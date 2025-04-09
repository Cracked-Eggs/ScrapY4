using UnityEngine;

public class VentPathFollower : MonoBehaviour
{
    private VentPathData ventPath;
    private int currentPointIndex = 0;
    private Rigidbody rb;
    public float moveSpeed = 15f;
    public float arrivalThreshold = 0.6f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void StartFollowing(VentPathData path)
    {
        ventPath = path;
        currentPointIndex = 0;
    }

    void FixedUpdate()
    {
        if (ventPath == null || ventPath.pathPoints.Count == 0) return;

        Vector3 targetPoint = ventPath.pathPoints[currentPointIndex];
        Vector3 direction = (targetPoint - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        // Check if reached the current point
        if (Vector3.Distance(transform.position, targetPoint) < arrivalThreshold)
        {
            currentPointIndex++;
            // If path ends, resume normal retraction
            if (currentPointIndex >= ventPath.pathPoints.Count)
            {
                ReturnToFlowField();
            }
        }
    }

    void ReturnToFlowField()
    {
        FlowField flowField = GetComponent<FlowField>();
        if (flowField != null) flowField.enabled = true;
        Destroy(this); // Remove this component
    }
}