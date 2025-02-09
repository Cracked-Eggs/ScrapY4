using UnityEngine;

public class PlayerAimingState : PlayerBaseState
{
    int AimingBlendTreeHash = Animator.StringToHash("AimingBlendTree");
    int TargetingForwardHash = Animator.StringToHash("AimingForward");
    int TargetingRightHash = Animator.StringToHash("AimingRight");
    
    const float CrossFadeDuration = 0.1f;
    
    public PlayerAimingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(AimingBlendTreeHash, CrossFadeDuration);
        stateMachine.Crosshair.SetActive(true);
        Debug.Log("Enter Aiming");
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.InputReader.IsAiming == false)
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        
        Vector3 movement = CalculateMovement(deltaTime);
        
        Move(movement * stateMachine.TargetingMovementSpeed, deltaTime);
        UpdateAnimator(deltaTime);
        RotateTowardsCamera();
    }

    public override void Exit()
    {
        stateMachine.Crosshair.SetActive(false);
        Debug.Log("exit aiming");
    }
    
    Vector3 CalculateMovement(float deltaTime)
    {
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.right;
    
        forward.y = 0f; // Keep movement horizontal
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return forward * stateMachine.InputReader.MovementValue.y +
               right * stateMachine.InputReader.MovementValue.x;
    }
    
    void RotateTowardsCamera()
    {
        Quaternion targetRotation = Quaternion.Euler(0f, stateMachine.MainCameraTransform.eulerAngles.y, 0f);
        stateMachine.transform.rotation = Quaternion.Lerp(stateMachine.transform.rotation, targetRotation, Time.deltaTime * stateMachine.RotationDamping);
    }
    
    void UpdateAnimator(float deltaTime)
    {
        if (stateMachine.InputReader.MovementValue.y == 0)
            stateMachine.Animator.SetFloat(TargetingForwardHash, 0, 0.1f, deltaTime);
        else
        {
            float value = stateMachine.InputReader.MovementValue.y > 0 ? 1f : -1f;
            stateMachine.Animator.SetFloat(TargetingForwardHash, value, 0.1f, deltaTime);
        }

        if (stateMachine.InputReader.MovementValue.x == 0)
            stateMachine.Animator.SetFloat(TargetingRightHash, 0, 0.1f, deltaTime);
        else
        {
            float value = stateMachine.InputReader.MovementValue.x > 0 ? 1f : -1f;
            stateMachine.Animator.SetFloat(TargetingRightHash, value, 0.1f, deltaTime);
        }
    }
}
