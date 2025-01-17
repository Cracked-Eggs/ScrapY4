using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    int LocomotionHash = Animator.StringToHash("Locomotion");
    int SpeedHash = Animator.StringToHash("Speed");

    const float CrossFadeDuration = 0.1f;
    const float AnimatorDampTime = 0.1f;

    public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter() => stateMachine.Animator.CrossFadeInFixedTime(LocomotionHash, CrossFadeDuration);

    public override void Tick(float deltaTime)
    {
        Move(deltaTime);

        if(IsInChaseRange())
        {
            stateMachine.SwitchState(new EnemyChasingState(stateMachine));
            return;
        }

        FacePlayer();
        stateMachine.Animator.SetFloat(SpeedHash, 0f, AnimatorDampTime, deltaTime);
    }

    public override void Exit() { }
}