using UnityEngine;

public class PlayerFallingState : PlayerBaseState
{
    int FallHash = Animator.StringToHash("Fall");
    Vector3 momentum;

    const float CrossFadeDuration = 0.1f;

    public PlayerFallingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        momentum = stateMachine.Controller.velocity;
        momentum.y = 0f;

        stateMachine.Animator.CrossFadeInFixedTime(FallHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        Vector3 movement = CalculateMovement();
        momentum = movement * stateMachine.FreeLookMovementSpeed;
        Move(momentum, deltaTime);

        if (stateMachine.Controller.isGrounded)
            ReturnToLocomotion();

        FaceMovementDirection(movement, deltaTime);
    }

    public override void Exit() { }

    Vector3 CalculateMovement()
    {
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return forward * stateMachine.InputReader.MovementValue.y +
               right * stateMachine.InputReader.MovementValue.x;
    }

    void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        stateMachine.transform.rotation = Quaternion.Lerp(
            stateMachine.transform.rotation,
            Quaternion.LookRotation(movement),
            deltaTime * stateMachine.RotationDamping);
    }
}