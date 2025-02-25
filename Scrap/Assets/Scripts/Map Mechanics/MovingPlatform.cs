using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform startPoint, endPoint;
    [SerializeField] float changeDirectionDelay;
    [SerializeField] float speed;

    Transform destinationTarget, departTarget;
    float startTime;
    float journeyLength;
    bool isWaiting;
    public GameObject Parent;
    void Start()
    {
        departTarget = startPoint;
        destinationTarget = endPoint;
        

        startTime = Time.time;
        journeyLength = Vector3.Distance(departTarget.position, destinationTarget.position);
    }

    void FixedUpdate() => Move();

    void Move()
    {
        if (!isWaiting)
        {
            if (Vector3.Distance(transform.position, destinationTarget.position) > 0.01f)
            {
                float distCovered = (Time.time - startTime) * speed;
                float fractionOfJourney = distCovered / journeyLength;
                transform.position = Vector3.Lerp(departTarget.position, destinationTarget.position, fractionOfJourney);
            }
            else
            {
                isWaiting = true;
                StartCoroutine(ChangeDelay());
            }
        }
    }

    void ChangeDestination()
    {
        if (departTarget == endPoint && destinationTarget == startPoint)
        {
            departTarget = startPoint;
            destinationTarget = endPoint;
        }
        else
        {
            departTarget = endPoint;
            destinationTarget = startPoint;
        }
    }

    IEnumerator ChangeDelay()
    {
        yield return new WaitForSeconds(changeDirectionDelay);
        ChangeDestination();
        startTime = Time.time;
        journeyLength = Vector3.Distance(departTarget.position, destinationTarget.position);
        isWaiting = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Parts"))
        {
            other.transform.parent = transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Parts"))
        {
            other.transform.SetParent(null);
        }
    }

   

    
}
