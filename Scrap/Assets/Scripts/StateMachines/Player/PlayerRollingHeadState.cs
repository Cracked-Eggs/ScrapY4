using UnityEngine;

public class PlayerRollingHeadState : PlayerBaseState
{
    private Rigidbody rb;
    [SerializeField] public float rollSpeed = 4f;
    [SerializeField] public float rotationSpeed = 0.000001f;

    
    public PlayerRollingHeadState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    
    public override void Enter()
    {
        rb = stateMachine.GetComponent<Rigidbody>(); 
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on Player!");
            return;
        }

        rb.isKinematic = false; // Enable physics
       
    }

    public override void Tick(float deltaTime)
    {
        Vector3 movement = CalculateMovement();
        Roll(movement, deltaTime);
    }

    public override void Exit()
    {
        rb.velocity = Vector3.zero; // Stop movement when exiting the state
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
}
