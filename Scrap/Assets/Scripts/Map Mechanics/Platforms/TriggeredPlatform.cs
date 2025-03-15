using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredPlatform : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] float speed;
    
    private int currentWaypointIndex = 0;
    private bool isTriggered = false;
    private bool isReversed = false;
    private const float waypointThreshold = 0.05f;
    
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
            Move();
        }
        
        MovePlayerWithPlatform();
        MoveBodyPartsWithPlatform();
    }
    
    void Move()
    {
        if (currentWaypointIndex >= 0 && currentWaypointIndex < waypoints.Length)
        {
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * speed * Time.fixedDeltaTime;
            
            if (Vector3.Distance(transform.position, targetPosition) <= waypointThreshold)
            {
                transform.position = targetPosition;
                if (isReversed)
                {
                    currentWaypointIndex--;
                }
                else
                {
                    currentWaypointIndex++;
                }
            }
        }
    }
    
    public void TriggerMovement()
    {
        isTriggered = true;
        isReversed = false;
        currentWaypointIndex = Mathf.Clamp(currentWaypointIndex, 0, waypoints.Length - 1);
    }
    
    public void ReverseMovement()
    {
        isTriggered = true;
        isReversed = true;
        currentWaypointIndex = Mathf.Clamp(currentWaypointIndex, 0, waypoints.Length - 1);
    }
    
    void MovePlayerWithPlatform()
    {
        if (playerController != null)
        {
            playerController.Move(transform.position - waypoints[Mathf.Clamp(currentWaypointIndex - 1, 0, waypoints.Length - 1)].position);
        }
    }
    
    void MoveBodyPartsWithPlatform()
    {
        foreach (Rigidbody rb in bodyParts)
        {
            if (rb != null)
            {
                rb.position += transform.position - waypoints[Mathf.Clamp(currentWaypointIndex - 1, 0, waypoints.Length - 1)].position;
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