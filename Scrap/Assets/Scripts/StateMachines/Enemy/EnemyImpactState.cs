using UnityEngine;

public class EnemyImpactState : EnemyBaseState
{
    const float CrossFadeDuration = 0.1f;
    int ImpactHash = Animator.StringToHash("Impact");
    float duration = 1f;

    public EnemyImpactState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter() => stateMachine.Animator.CrossFadeInFixedTime(ImpactHash, CrossFadeDuration);

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);
        duration -= deltaTime;

        if(duration <= 0f)
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
    }

    public override void Exit() { }
}