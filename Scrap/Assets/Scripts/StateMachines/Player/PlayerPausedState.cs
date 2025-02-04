using UnityEngine;

public class PlayerPausedState : PlayerBaseState
{
    
    public PlayerPausedState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.Animator.SetFloat("FreeLookSpeed", 0);
        Cursor.lockState = CursorLockMode.None;
        stateMachine.FreeLookInput.enabled = false;
        stateMachine.PauseMenu.SetActive(true);
        stateMachine.Resume = false;
        stateMachine.InputReader.PauseEvent += OnPause;
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.Resume == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            stateMachine.FreeLookInput.enabled = true;
            ReturnToLocomotion();
        }
    }

    public override void Exit()
    {
    }

    void OnPause()
    {
    }
}
