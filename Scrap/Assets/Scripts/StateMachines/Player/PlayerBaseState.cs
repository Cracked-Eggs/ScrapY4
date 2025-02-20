using UnityEngine;

public abstract class PlayerBaseState : State
{
    protected PlayerStateMachine stateMachine;

    public PlayerBaseState(PlayerStateMachine stateMachine) => this.stateMachine = stateMachine;

    protected void Move(float deltaTime) => Move(Vector3.zero, deltaTime);

    protected void Move(Vector3 motion, float deltaTime)
    {
        stateMachine.Controller.Move((motion + stateMachine.ForceReceiver.Movement) * deltaTime);
        stateMachine.SetMomentum(stateMachine.Controller.velocity);
    }
    
    protected void FaceTarget()
    {
        if (stateMachine.Targeter.CurrentTarget == null) { return; }

        Vector3 lookPos = stateMachine.Targeter.CurrentTarget.transform.position - stateMachine.transform.position;
        lookPos.y = 0f;

        stateMachine.transform.rotation = Quaternion.LookRotation(lookPos);
    }
    
    protected void ReturnToLocomotion()
    {
        if (stateMachine.Targeter.CurrentTarget != null)
            stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
        else
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
    }
}
