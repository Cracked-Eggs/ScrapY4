using System.Collections;
using UnityEngine;

public class WaypointPlatform : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] float changeDirectionDelay;
    [SerializeField] float speed;
    public GameObject Parent;
    public GameObject DetachHolder;
    int currentWaypointIndex = 0;
    bool isWaiting;
    bool isReversed = false;
    const float waypointThreshold = 0.05f; // Increased threshold to prevent tweaking

    void Start()
    {
        if (waypoints.Length < 2)
        {
            Debug.LogError("Waypoints must have at least two points!");
            enabled = false;
            return;
        }
    }

    void FixedUpdate() => Move();

    void Move()
    {
        if (!isWaiting)
        {
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * speed * Time.fixedDeltaTime;

            if (Vector3.Distance(transform.position, targetPosition) <= waypointThreshold)
            {
                transform.position = targetPosition; // Snap to exact position
                isWaiting = true;
                StartCoroutine(ChangeDelay());
            }
        }
    }

    void ChangeDestination()
    {
        if (isReversed)
        {
            currentWaypointIndex = (currentWaypointIndex - 1 + waypoints.Length) % waypoints.Length;
        }
        else
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    public void SetReversed(bool reversed)
    {
        if (isReversed != reversed) // Only change if needed
        {
            isReversed = reversed;
            currentWaypointIndex = (currentWaypointIndex - 1 + waypoints.Length) % waypoints.Length; // Move one step back
        }
    }

    IEnumerator ChangeDelay()
    {
        yield return new WaitForSeconds(changeDirectionDelay);
        ChangeDestination();
        isWaiting = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.parent = transform;
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.parent = Parent.transform;
        }
           
    }
}
