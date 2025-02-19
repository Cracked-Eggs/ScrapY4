using UnityEngine;

public class PlayerRollingHeadState : PlayerBaseState
{
    private Rigidbody rb;

    [SerializeField] public float rollSpeed = 4f;
    [SerializeField] public float rotationSpeed = 0.000001f;


    
    [SerializeField] private float jetpackRotationSpeed =1000f; 

    public PlayerRollingHeadState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        rb = stateMachine.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on Player!");
            return;
        }

        rb.isKinematic = false; 
    }

    public override void Tick(float deltaTime)
    {
        Vector3 movement = CalculateMovement();
        Roll(movement, deltaTime);
        HandleJetpack(deltaTime);
    }

    public override void Exit()
    {
        rb.velocity = Vector3.zero; 
    }

    private Vector3 CalculateMovement()
    {
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        return (forward * stateMachine.InputReader.MovementValue.y + right * stateMachine.InputReader.MovementValue.x).normalized;
    }

    private void Roll(Vector3 movement, float deltaTime)
    {
        if (movement.sqrMagnitude > 0.01f)
        {
            rb.AddForce(movement * rollSpeed, ForceMode.Acceleration);
            rb.AddTorque(stateMachine.transform.right * -movement.magnitude * rotationSpeed);
        }
    }

    private void HandleJetpack(float deltaTime)
    {
        if (stateMachine.isHovering && stateMachine.curFuel > 0f)
        {
            Debug.Log("Hovering: Applying hover force!");

            // Apply hover force, but limit vertical speed
           
                rb.AddForce(rb.transform.up * stateMachine.thrustForce, ForceMode.Impulse);
                stateMachine.curFuel -= Time.deltaTime;
          
            // Call the rotation function here
            RotateHeadTowardsUpward();
        }
        else if (Physics.Raycast(stateMachine.groundedTransform.position, Vector3.down, 1f, LayerMask.GetMask("Ground")) && stateMachine.curFuel < stateMachine.maxFuel)
        {
            Debug.Log("Refueling jetpack...");
            stateMachine.curFuel += Time.deltaTime;
        }
    }


    private void RotateHeadTowardsUpward()
    {
       
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

        
        stateMachine.transform.rotation = Quaternion.RotateTowards(stateMachine.transform.rotation, targetRotation, jetpackRotationSpeed * Time.deltaTime);
    }
}
