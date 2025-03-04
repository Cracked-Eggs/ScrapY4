using UnityEngine;

public class EnemyPatrolState : EnemyBaseState
{
   int currentPatrolIndex = 0;

   int LocomotionHash = Animator.StringToHash("Locomotion");
   int SpeedHash = Animator.StringToHash("Speed");
   const float CrossFadeDuration = 0.1f;
   const float AnimatorDampTime = 0.1f;

    float rotationSpeed = 5f; // Adjust this to control how fast the enemy turns

    public EnemyPatrolState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Animator.CrossFadeInFixedTime(LocomotionHash, CrossFadeDuration);
        currentPatrolIndex = 0; 
    }

    public override void Tick(float deltaTime)
    {
        if (IsInChaseRange())
        {
            stateMachine.SwitchState(new EnemyChasingState(stateMachine));
            return;
        }

        Patrol(deltaTime);

        stateMachine.Animator.SetFloat(SpeedHash, 1f, AnimatorDampTime, deltaTime);
    }

    public override void Exit() { }

    private void Patrol(float deltaTime)
    {
        if (stateMachine.PatrolPoints.Count == 0) { return; }

        Transform currentPatrolPoint = stateMachine.PatrolPoints[currentPatrolIndex];
        Vector3 direction = (currentPatrolPoint.position - stateMachine.transform.position).normalized;

        SmoothFaceTarget(direction, deltaTime);

        stateMachine.Controller.Move(direction * stateMachine.PatrolSpeed * deltaTime);

        if (Vector3.Distance(stateMachine.transform.position, currentPatrolPoint.position) < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % stateMachine.PatrolPoints.Count;
        }
    }

    private void SmoothFaceTarget(Vector3 direction, float deltaTime)
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
}