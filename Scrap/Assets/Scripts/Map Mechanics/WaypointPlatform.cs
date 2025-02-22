using System.Collections;
using UnityEngine;

public class WaypointPlatform : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] float changeDirectionDelay;
    [SerializeField] float speed;
    
    int currentWaypointIndex = 0;
    bool isWaiting;

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
            Vector3 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;
            transform.position += direction * speed * Time.fixedDeltaTime;

            if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) <= 0.01f)
            {
                isWaiting = true;
                StartCoroutine(ChangeDelay());
            }
        }
    }

    void ChangeDestination()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
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
            other.transform.parent = null;
        }
    }
}