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
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.InputReader.IsAiming == false)
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        
        Vector3 movement = CalculateMovement(deltaTime);
        
        Move(movement * stateMachine.TargetingMovementSpeed, deltaTime);
        UpdateAnimator(deltaTime);
    }

    public override void Exit()
    {
    }
    
    Vector3 CalculateMovement(float deltaTime)
    {
        Vector3 movement = new Vector3();

        movement += stateMachine.transform.right * stateMachine.InputReader.MovementValue.x;
        movement += stateMachine.transform.forward * stateMachine.InputReader.MovementValue.y;
        return movement;
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
