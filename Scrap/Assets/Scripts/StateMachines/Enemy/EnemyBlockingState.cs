using UnityEngine;

public class EnemyBlockingState : EnemyBaseState
{
    int BlockHash = Animator.StringToHash("Block");
    const float CrossFadeDuration = 0.1f;

    float blockDuration = 10f; // Adjust this value as needed
    float movementSpeedWhileBlocking = 1f; // Adjust this to control how fast the enemy moves while blocking
    float RotationSpeed = 5f;

    // Cache the player transform to avoid repeated access
    private Transform playerTransform;

    public EnemyBlockingState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
        playerTransform = stateMachine.Player.transform;
    }

    public override void Enter()
    {
        stateMachine.Health.SetInvulnerable(true);
        stateMachine.Animator.CrossFadeInFixedTime(BlockHash, CrossFadeDuration);
        stateMachine.CanBlock = false;
    }

    public override void Tick(float deltaTime)
    {
        FacePlayer();

        if (IsInAttackRange())
        {
            stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
        }
        else
        {
            movementSpeedWhileBlocking = 1f; // Reset to default speed
        }

        MoveTowardPlayer(deltaTime);

        stateMachine.Animator.SetFloat("Speed", movementSpeedWhileBlocking);
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

    void MoveTowardPlayer(float deltaTime)
    {
        // Calculate the direction to the player
        Vector3 direction = (playerTransform.position - stateMachine.transform.position).normalized;

        // Move the enemy toward the player at the current speed while blocking
        stateMachine.Controller.Move(direction * movementSpeedWhileBlocking * deltaTime);
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

        // Smoothly interpolate between the current rotation and the target rotation
        stateMachine.transform.rotation = Quaternion.Slerp(
            stateMachine.transform.rotation,
            targetRotation,
            RotationSpeed * deltaTime
        );
    }

    new bool IsInAttackRange()
    {
        if (stateMachine.Player.IsDead) { return false; }

        float playerDistanceSqr = (playerTransform.position - stateMachine.transform.position).sqrMagnitude;
        return playerDistanceSqr <= stateMachine.AttackRange * stateMachine.AttackRange;
    }
}