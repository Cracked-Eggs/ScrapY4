using UnityEngine;

public class EnemyBlockingState : EnemyBaseState
{
    int BlockHash = Animator.StringToHash("Block");
    const float CrossFadeDuration = 0.1f;

    float blockDuration = 4f; // Adjust this value as needed

    public EnemyBlockingState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Health.SetInvulnerable(true);

        stateMachine.Animator.CrossFadeInFixedTime(BlockHash, CrossFadeDuration);
        stateMachine.CanBlock = false;
    }

    public override void Tick(float deltaTime)
    {
        FacePlayer();

        blockDuration -= deltaTime;

        if (blockDuration <= 0f)
        {
            stateMachine.SwitchState(new EnemyChasingState(stateMachine));
            return;
        }
    }

    public override void Exit()
    {
        stateMachine.Health.SetInvulnerable(false);
        stateMachine.StartCoroutine(EnableBlockingAfterCooldown());
    }
    
    private System.Collections.IEnumerator EnableBlockingAfterCooldown()
    {
        yield return new WaitForSeconds(3f);
        stateMachine.CanBlock = true;
    }
}