using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{
    public EnemyDeadState(EnemyStateMachine stateMachine) : base(stateMachine) { }
    int ImpactHash = Animator.StringToHash("Dead");
    const float CrossFadeDuration = 0.1f;

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(ImpactHash, CrossFadeDuration);
        stateMachine.WeaponL.gameObject.SetActive(false);
        stateMachine.WeaponR.gameObject.SetActive(false);
        GameObject.Destroy(stateMachine.Target);
        GameObject.Destroy(stateMachine.gameObject, 3f);
    }

    public override void Tick(float deltaTime) { }

    public override void Exit() { }
}