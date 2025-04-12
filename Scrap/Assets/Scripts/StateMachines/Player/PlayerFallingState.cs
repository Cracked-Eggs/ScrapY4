using UnityEngine;

public class PlayerFallingState : PlayerBaseState
{
    int FallHash = Animator.StringToHash("Fall");
    int LandHash = Animator.StringToHash("Land");
    
    Vector3 momentum;
    bool hasPlayedLanding = false;

    const float CrossFadeDuration = 0.1f;
    const float LandAnimDuration = 0.3f; // Adjust based on your landing animation length

    public PlayerFallingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        hasPlayedLanding = false;
        momentum = stateMachine.Controller.velocity;
        momentum.y = 0f;

        // Immediately play falling animation
        stateMachine.Animator.CrossFadeInFixedTime(FallHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        Vector3 movement = CalculateMovement();

        if (stateMachine.Controller.isGrounded)
        {
            if (!hasPlayedLanding)
            {
                stateMachine.Animator.CrossFadeInFixedTime(LandHash, CrossFadeDuration);
                hasPlayedLanding = true;
                stateMachine.StartCoroutine(DelayedTransitionToLocomotion(LandAnimDuration));
            
                // Apply friction when landing
                momentum *= 0.5f; // Reduce momentum by half immediately
            }
            else
            {
                // Continue slowing down
                momentum = Vector3.Lerp(momentum, Vector3.zero, deltaTime * 10f);
            }
            Move(momentum, deltaTime);
            return;
        }

        // Regular falling movement
        if (movement != Vector3.zero)
        {
            momentum = movement * stateMachine.FreeLookMovementSpeed;
        }

        Move(momentum, deltaTime);

        if (movement != Vector3.zero)
        {
            FaceMovementDirection(movement, deltaTime);
        }
    }

    public override void Exit() 
    {
        // Reset any landing flags when exiting
        hasPlayedLanding = false;
    }

    private System.Collections.IEnumerator DelayedTransitionToLocomotion(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToLocomotion();
    }

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