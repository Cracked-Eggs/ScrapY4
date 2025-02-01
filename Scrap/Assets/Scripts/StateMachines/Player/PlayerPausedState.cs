using UnityEngine;

public class PlayerPausedState : PlayerBaseState
{
    
    public PlayerPausedState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.InputReader.PauseEvent += OnPause;
    }

    public override void Tick(float deltaTime)
    {
    }

    public override void Exit()
    {
    }

    void OnPause()
    {
        
    }
}
