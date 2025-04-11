using UnityEngine;

public class EnemyBlockingState : EnemyBaseState
{
    int BlockHash = Animator.StringToHash("Block");
    const float CrossFadeDuration = 0.1f;

    float blockDuration = 10f; 
    float movementSpeedWhileBlocking = 1f; 
    float RotationSpeed = 5f;
    float backUpTimer = 0f;
    const float BackUpInterval = 1.5f;

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
        
        backUpTimer += deltaTime;
        
        if (backUpTimer >= BackUpInterval)
        {
            Vector3 direction = (stateMachine.transform.position - stateMachine.Player.transform.position).normalized;
            stateMachine.Controller.Move(direction * movementSpeedWhileBlocking * 0.3f * deltaTime);
            backUpTimer = 0f;
        }
        else
        {
            Vector3 direction = (stateMachine.Player.transform.position - stateMachine.transform.position).normalized;
            stateMachine.Controller.Move(direction * movementSpeedWhileBlocking * 0.1f * deltaTime);
        }

        stateMachine.Animator.SetFloat("Speed", movementSpeedWhileBlocking * 0.5f);
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
        Vector3 direction = (playerTransform.position - stateMachine.transform.position).normalized;

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