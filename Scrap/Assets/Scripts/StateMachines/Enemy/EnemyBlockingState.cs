using UnityEngine;

public class EnemyBlockingState : EnemyBaseState
{
    int BlockHash = Animator.StringToHash("Block");
    const float CrossFadeDuration = 0.1f;

    float blockDuration = 2f; // Adjust this value as needed
    float timer;

    public EnemyBlockingState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Health.SetInvulnerable(true);

        stateMachine.Animator.CrossFadeInFixedTime(BlockHash, CrossFadeDuration);

        timer = blockDuration;
    }

    public override void Tick(float deltaTime)
    {
        FacePlayer();

        timer -= deltaTime;

        if (timer <= 0f)
        {
            stateMachine.SwitchState(new EnemyChasingState(stateMachine));
            return;
        }
    }

    public override void Exit()
    {
        stateMachine.Health.SetInvulnerable(false);
    }
}