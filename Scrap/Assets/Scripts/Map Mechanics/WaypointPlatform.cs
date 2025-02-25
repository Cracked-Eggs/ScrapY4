using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPlatform : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] float changeDirectionDelay;
    [SerializeField] float speed;

    private int currentWaypointIndex = 0;
    private bool isWaiting;
    private bool isReversed = false;
    private const float waypointThreshold = 0.05f;

    private Vector3 lastPosition;
    private Vector3 movementDelta;
    public Attach attach;
    public CharacterController playerController = null;
    [SerializeField] private List<Rigidbody> bodyParts = new List<Rigidbody>();
    private Dictionary<Rigidbody, Vector3> bodyPartOriginalPositions = new Dictionary<Rigidbody, Vector3>(); // Store original positions of body parts

    public bool isPlayerGrappled = false;

    void Start()
    {
        if (waypoints.Length < 2)
        {
            Debug.LogError("Waypoints must have at least two points!");
            enabled = false;
            return;
        }
        lastPosition = transform.position;
        SaveBodyPartOriginalPositions(); // Save the original positions of body parts
    }

    void FixedUpdate()
    {
        if (attach.isPlayerGrappled)
        {
            playerController = null;
        }
        Move();
        CalculateMovementDelta();
        MovePlayerWithPlatform();
        MoveBodyPartsWithPlatform();
    }

    void Move()
    {
        if (!isWaiting)
        {
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * speed * Time.fixedDeltaTime;

            if (Vector3.Distance(transform.position, targetPosition) <= waypointThreshold)
            {
                transform.position = targetPosition;
                isWaiting = true;
                StartCoroutine(ChangeDelay());
            }
        }
    }

    void ChangeDestination()
    {
        currentWaypointIndex = isReversed
            ? (currentWaypointIndex - 1 + waypoints.Length) % waypoints.Length
            : (currentWaypointIndex + 1) % waypoints.Length;
    }

    public void SetReversed(bool reversed)
    {
        if (isReversed != reversed)
        {
            isReversed = reversed;
            currentWaypointIndex = (currentWaypointIndex - 1 + waypoints.Length) % waypoints.Length;
        }
    }

    IEnumerator ChangeDelay()
    {
        yield return new WaitForSeconds(changeDirectionDelay);
        ChangeDestination();
        isWaiting = false;
    }

    void CalculateMovementDelta()
    {
        movementDelta = transform.position - lastPosition;
        lastPosition = transform.position;
    }

    void MovePlayerWithPlatform()
    {
        if (playerController != null)
        {
            playerController.Move(movementDelta); // Apply platform movement only if not grappled
        }
    }

    void MoveBodyPartsWithPlatform()
    {
        foreach (Rigidbody rb in bodyParts)
        {
            if (rb != null)
            {
                rb.position += movementDelta; // Move rigidbody with platform
                // Prevent unwanted physics forces
            }
        }
    }

    void SaveBodyPartOriginalPositions()
    {
        // Save the original positions of all body parts
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
        }
        else if (other.CompareTag("R_Arm") || other.CompareTag("L_Arm") || other.CompareTag("R_Leg") || other.CompareTag("L_Leg") || other.CompareTag("Torso")) // Check the body part tags
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null && !bodyParts.Contains(rb))
            {
                bodyParts.Add(rb);
                // Restore the Rigidbody position if it's been detached and reattached
                if (bodyPartOriginalPositions.ContainsKey(rb))
                {
                    rb.position = bodyPartOriginalPositions[rb]; // Reset the position to original
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
                // Optional: Clear the Rigidbody reference when it exits the plate
                // bodyPartOriginalPositions.Remove(rb); // If you want to clear it
            }
        }
    }
}
