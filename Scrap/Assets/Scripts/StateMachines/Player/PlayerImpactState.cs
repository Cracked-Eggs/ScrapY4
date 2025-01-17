using UnityEngine;

public class PlayerImpactState : PlayerBaseState
{
    int ImpactHash = Animator.StringToHash("Impact");
    float duration = 1f;

    const float CrossFadeDuration = 0.1f;

    public PlayerImpactState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter() => stateMachine.Animator.CrossFadeInFixedTime(ImpactHash, CrossFadeDuration);

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);
        duration -= deltaTime;

        if(duration <= 0f)
            ReturnToLocomotion();
    }

    public override void Exit() { }
}