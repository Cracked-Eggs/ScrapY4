using JetBrains.Annotations;
using UnityEngine;

public class PlayerFreeLookState : PlayerBaseState
{
    int FreeLookBlendTreeHash = Animator.StringToHash("FreeLookBlendTree");
    int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    bool hasInteracted;
    float footstepTimer = 0f;

    const float AnimatorDampTime = 0.1f;
    const float CrossFadeDuration = 0.1f;

    public PlayerFreeLookState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.InputReader.JumpEvent += OnJump;
        stateMachine.InputReader.TargetEvent += OnTarget;
        stateMachine.InputReader.InteractEvent += OnInteract;
        stateMachine.InputReader.PauseEvent += OnPause;
        stateMachine.Animator.CrossFadeInFixedTime(FreeLookBlendTreeHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.InputReader.IsAiming)
        {
            stateMachine.SwitchState(new PlayerAimingState(stateMachine));
        }

        if (stateMachine.InputReader.IsLAiming)
        {
            stateMachine.SwitchState(new PlayerLeftAimingState(stateMachine));
        }

        Vector3 movement = CalculateMovement();
        Move(movement * stateMachine.FreeLookMovementSpeed, deltaTime);

        if (stateMachine.InputReader.MovementValue == Vector2.zero)
        {
            stateMachine.Animator.SetFloat(FreeLookSpeedHash, 0, AnimatorDampTime, deltaTime);
            return;
        }

        stateMachine.Animator.SetFloat(FreeLookSpeedHash, 1, AnimatorDampTime, deltaTime);
        FaceMovementDirection(movement, deltaTime);
        Footsteps(deltaTime);
    }

    public override void Exit()
    {
        stateMachine.InputReader.JumpEvent -= OnJump;
        stateMachine.InputReader.TargetEvent -= OnTarget;
        stateMachine.InputReader.InteractEvent -= OnInteract;
    }
    
    private void OnTarget()
    {
        if (!stateMachine.Targeter.SelectTarget()) { return; }

        stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
    }

    
    void Footsteps(float deltaTime)
    {
        footstepTimer -= deltaTime;

        // Check if the player is moving before playing footsteps
        if (footstepTimer <= 0 && stateMachine.InputReader.MovementValue != Vector2.zero)
        {
            stateMachine.AudioManager.PlayFootsteps();
            footstepTimer = 0.5f; // Reset timer after playing a footstep
        }
    }

    
    void OnJump() => stateMachine.SwitchState(new PlayerJumpingState(stateMachine));
    void OnPause() => stateMachine.SwitchState(new PlayerPausedState(stateMachine));

    void OnInteract()
    {
        Collider[] hitColliders = Physics.OverlapSphere(stateMachine.transform.position, 1.5f);
    
        foreach (Collider hitCollider in hitColliders)
        {
            Interactable interactable = hitCollider.GetComponent<Interactable>();

            if (interactable != null)
            {
                interactable.TryInteract();
                stateMachine.SwitchState(new PlayerInteractState(stateMachine)); // Enter interaction state
                return;
            }
        }
    }

    Vector3 CalculateMovement()
    {
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        return forward * stateMachine.InputReader.MovementValue.y +
            right * stateMachine.InputReader.MovementValue.x;
    }

    void FaceMovementDirection(Vector3 movement, float deltaTime)
    {
        stateMachine.transform.rotation = Quaternion.Lerp(
            stateMachine.transform.rotation,
            Quaternion.LookRotation(movement),
            deltaTime * stateMachine.RotationDamping);
    }
}
