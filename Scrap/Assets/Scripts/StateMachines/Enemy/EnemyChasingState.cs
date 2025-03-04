using UnityEngine;

public class EnemyChasingState : EnemyBaseState
{
    int LocomotionHash = Animator.StringToHash("Locomotion");
    int SpeedHash = Animator.StringToHash("Speed"); 
    const float CrossFadeDuration = 0.1f;
    const float AnimatorDampTime = 0.1f;
    
    float blockGraceTimer = 2f; 
    float elapsedTime = 0f;

    public EnemyChasingState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(LocomotionHash, CrossFadeDuration);
        elapsedTime = 0f;
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
        else if (elapsedTime >= blockGraceTimer && Random.value < stateMachine.BlockChance && stateMachine.CanBlock)
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
        stateMachine.Agent.ResetPath();
        stateMachine.Agent.velocity = Vector3.zero;
    }

    void MoveToPlayer(float deltaTime)
    {
        if (stateMachine.Agent.isOnNavMesh)
        {
            stateMachine.Agent.destination = stateMachine.Player.transform.position;
            Move(stateMachine.Agent.desiredVelocity.normalized * stateMachine.MovementSpeed, deltaTime);
        }
        stateMachine.Agent.velocity = stateMachine.Controller.velocity;
    }

    bool IsInAttackRange()
    {
        if (stateMachine.Player.IsDead) { return false; }
        
        float playerDistanceSqr = (stateMachine.Player.transform.position - stateMachine.transform.position).sqrMagnitude;
        return playerDistanceSqr <= stateMachine.AttackRange * stateMachine.AttackRange;
    }
}
