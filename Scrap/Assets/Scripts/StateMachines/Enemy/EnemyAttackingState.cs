using UnityEngine;

public class EnemyAttackingState : EnemyBaseState
{
    int AttackHash = Animator.StringToHash("Attack");
    const float TransitionDuration = 0.1f;

    public EnemyAttackingState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.WeaponL.SetAttack(stateMachine.AttackDamage, stateMachine.AttackKnockback);
        stateMachine.WeaponR.SetAttack(stateMachine.AttackDamage, stateMachine.AttackKnockback);
        stateMachine.Animator.CrossFadeInFixedTime(AttackHash, TransitionDuration);
        stateMachine.LastAttackTime = Time.time;
    }

    public override void Tick(float deltaTime)
    {
        if (GetNormalizedTime(stateMachine.Animator) >= 1)
            stateMachine.SwitchState(new EnemyChasingState(stateMachine));
        
        FacePlayer();
    }

    public override void Exit() { }
}