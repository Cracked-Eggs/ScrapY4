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
        Move(momentum, deltaTime);

        if(stateMachine.Controller.isGrounded)
            ReturnToLocomotion();

        FaceTarget();
    }

    public override void Exit() { }
}