using UnityEngine;

public class PlayerInteractState : PlayerBaseState
{
    int InteractHash = Animator.StringToHash("Interact");
    float duration = 0.6f;

    const float CrossFadeDuration = 0.1f;
    Interactable interactable;

    public PlayerInteractState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(InteractHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);
        duration -= deltaTime;

        if(duration <= 0f)
            ReturnToLocomotion();
    }

    public override void Exit()
    {
    }
}
