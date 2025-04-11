using UnityEngine;

public class EnemyChasingState : EnemyBaseState
{
    int LocomotionHash = Animator.StringToHash("Locomotion");
    int SpeedHash = Animator.StringToHash("Speed"); 
    const float CrossFadeDuration = 0.1f;
    const float AnimatorDampTime = 0.1f;
    
    float elapsedTime = 0f;

    float rotationSpeed = 5f; // Adjust this to control how fast the enemy turns 

    public EnemyChasingState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(LocomotionHash, CrossFadeDuration);
        elapsedTime = 0f;
        stateMachine.CanBlock = true;
    }

    public override void Tick(float deltaTime)
    {
        elapsedTime += deltaTime;

        if (!IsInChaseRange())
        {
            stateMachine.SwitchState(new EnemyIdleState(stateMachine));
            return;
        }
        else if (IsInAttackRange())
        {
            stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
            return;
        }
        else if (elapsedTime >= stateMachine.BlockGrace && Random.value < stateMachine.BlockChance && stateMachine.CanBlock)
        {
            stateMachine.SwitchState(new EnemyBlockingState(stateMachine));
            return;
        }

        MoveToPlayer(deltaTime);
        FacePlayer();

        stateMachine.Animator.SetFloat(SpeedHash, 1f, AnimatorDampTime, deltaTime);
    }

    public override void Exit()
    {
    }

    void MoveToPlayer(float deltaTime)
    {
        Vector3 direction = (stateMachine.Player.transform.position - stateMachine.transform.position).normalized;
        SmoothFaceTarget(direction, deltaTime);

        stateMachine.Controller.Move(direction * stateMachine.MovementSpeed * deltaTime);
    }

    new void FacePlayer()
    {
        Vector3 direction = (stateMachine.Player.transform.position - stateMachine.transform.position).normalized;
        SmoothFaceTarget(direction, Time.deltaTime);
    }

    void SmoothFaceTarget(Vector3 direction, float deltaTime)
    {
        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Smoothly interpolate between the current rotation and the target rotation
        stateMachine.transform.rotation = Quaternion.Slerp(
            stateMachine.transform.rotation,
            targetRotation,
            rotationSpeed * deltaTime
        );
    }

    new bool IsInAttackRange()
    {
        if (stateMachine.Player.IsDead) { return false; }
        
        float playerDistanceSqr = (stateMachine.Player.transform.position - stateMachine.transform.position).sqrMagnitude;
        return playerDistanceSqr <= stateMachine.AttackRange * stateMachine.AttackRange;
    }
}