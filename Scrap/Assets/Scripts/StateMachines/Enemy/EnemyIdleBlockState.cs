using UnityEngine;

public class EnemyIdleBlockState : EnemyBaseState
{
    int BlockHash = Animator.StringToHash("Block");
    const float CrossFadeDuration = 0.1f;

    float blockDuration = 10f; 
    float RotationSpeed = 5f;

    private Transform playerTransform;

    public EnemyIdleBlockState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
        playerTransform = stateMachine.Player.transform;
    }

    public override void Enter()
    {
        stateMachine.Health.SetInvulnerable(true);
        stateMachine.Animator.CrossFadeInFixedTime(BlockHash, CrossFadeDuration);
        stateMachine.CanBlock = false;
        stateMachine.Animator.SetFloat("Speed", 0f); // Ensure movement speed is 0
    }

    public override void Tick(float deltaTime)
    {
        FacePlayer(deltaTime);
        
        blockDuration -= deltaTime;
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

    void FacePlayer(float deltaTime)
    {
        Vector3 direction = (playerTransform.position - stateMachine.transform.position).normalized;
        SmoothFaceTarget(direction, deltaTime);
    }

    void SmoothFaceTarget(Vector3 direction, float deltaTime)
    {
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        stateMachine.transform.rotation = Quaternion.Slerp(
            stateMachine.transform.rotation,
            targetRotation,
            RotationSpeed * deltaTime
        );
    }
}