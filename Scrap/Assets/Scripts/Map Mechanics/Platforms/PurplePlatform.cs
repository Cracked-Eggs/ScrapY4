using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurplePlatform : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] float speed;
    [SerializeField] float changeDirectionDelay;

    private int currentWaypointIndex = 0;
    private bool isTriggered = false;
    private bool isWaiting = false;
    private Vector3 startPosition;
    private const float waypointThreshold = 0.05f;
    private Vector3 lastPosition;
    private Vector3 movementDelta;

    public Attach attach;
    public CharacterController playerController = null;
    [SerializeField] private List<Rigidbody> bodyParts = new List<Rigidbody>();
    private Dictionary<Rigidbody, Vector3> bodyPartOriginalPositions = new Dictionary<Rigidbody, Vector3>();
    public bool isPlayerGrappled = false;

    void Start()
    {
        if (waypoints.Length < 2)
        {
            Debug.LogError("Waypoints must have at least two points!");
            enabled = false;
            return;
        }
        startPosition = transform.position;
        SaveBodyPartOriginalPositions();
    }

    void FixedUpdate()
    {
        if (attach.isPlayerGrappled)
        {
            playerController = null;
        }

        if (isTriggered)
        {
            MoveTowards(waypoints[0].position); // Move to waypoint while held
        }
        else
        {
            MoveTowards(startPosition); // Return when released
        }

        CalculateMovementDelta();
        MovePlayerWithPlatform();
        MoveBodyPartsWithPlatform();
    }

    void MoveTowards(Vector3 targetPosition)
    {
        if (Vector3.Distance(transform.position, targetPosition) > waypointThreshold)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * speed * Time.fixedDeltaTime;

            // Snap to final position if close enough
            if (Vector3.Distance(transform.position, targetPosition) <= waypointThreshold)
            {
                transform.position = targetPosition;
            }
        }
    }


    IEnumerator ChangeDelay()
    {
        yield return new WaitForSeconds(changeDirectionDelay);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        isWaiting = false;
    }

    void CalculateMovementDelta()
    {
        movementDelta = transform.position - lastPosition;
        lastPosition = transform.position;
    }

    public void TriggerMovement()
    {
        isTriggered = true;
    }

    public void StopMovement()
    {
        isTriggered = false;
    }

    void MovePlayerWithPlatform()
    {
        if (playerController != null)
        {
            playerController.Move(movementDelta);
        }
    }

    void MoveBodyPartsWithPlatform()
    {
        foreach (Rigidbody rb in bodyParts)
        {
            if (rb != null)
            {
                rb.position += movementDelta;
            }
        }
    }

    void SaveBodyPartOriginalPositions()
    {
        foreach (Rigidbody rb in bodyParts)
        {
            if (rb != null && !bodyPartOriginalPositions.ContainsKey(rb))
            {
                bodyPartOriginalPositions[rb] = rb.position;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerController = other.GetComponent<CharacterController>();
            isTriggered = true;
        }
        else if (other.CompareTag("R_Arm") || other.CompareTag("L_Arm") || other.CompareTag("R_Leg") || other.CompareTag("L_Leg") || other.CompareTag("Torso"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null && !bodyParts.Contains(rb))
            {
                bodyParts.Add(rb);
                if (bodyPartOriginalPositions.ContainsKey(rb))
                {
                    rb.position = bodyPartOriginalPositions[rb];
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerController = null;
        }
        if (other.CompareTag("R_Arm") || other.CompareTag("L_Arm") || other.CompareTag("R_Leg") || other.CompareTag("L_Leg") || other.CompareTag("Torso"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                bodyParts.Remove(rb);
            }
        }
    }
}
