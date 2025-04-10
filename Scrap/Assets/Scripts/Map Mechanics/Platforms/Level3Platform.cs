using UnityEngine;

public class Level3Platform : MonoBehaviour
{
    [Header("Positions")]
    [SerializeField] Vector3 raisedPosition; 
    [SerializeField] Vector3 loweredPosition;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] bool startRaised;

    Vector3 targetPosition;
    bool isMoving;

    void Start()
    {
        transform.position = startRaised ? raisedPosition : loweredPosition;
        targetPosition = transform.position;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    public void Raise()
    {
        targetPosition = raisedPosition;
        isMoving = true;
    }

    public void Lower()
    {
        targetPosition = loweredPosition;
        isMoving = true;
    }

    public void Toggle()
    {
        if (transform.position == raisedPosition)
            Lower();
        else
            Raise();
    }
}