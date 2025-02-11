using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    int ImpactHash = Animator.StringToHash("Dead");
    const float CrossFadeDuration = 0.1f;

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(ImpactHash, CrossFadeDuration);
        stateMachine.Weapon.gameObject.SetActive(false);
        stateMachine.Weapon2.gameObject.SetActive(false);
    }

    public override void Tick(float deltaTime) { }
    public override void Exit() { }
}