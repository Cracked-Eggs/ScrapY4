using UnityEngine;

public class PlayerAimingState : PlayerBaseState
{
    int AimingBlendTreeHash = Animator.StringToHash("AimingBlendTree");
    
    const float CrossFadeDuration = 0.1f;
    
    public PlayerAimingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(AimingBlendTreeHash, CrossFadeDuration);
        Debug.Log("Enter aiming");
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.InputReader.IsAiming == false)
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
        }
    }

    public override void Exit()
    {
    }
}
