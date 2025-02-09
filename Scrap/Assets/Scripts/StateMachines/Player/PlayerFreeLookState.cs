using UnityEngine;

public class PlayerFreeLookState : PlayerBaseState
{
    int FreeLookBlendTreeHash = Animator.StringToHash("FreeLookBlendTree");
    int FreeLookSpeedHash = Animator.StringToHash("FreeLookSpeed");
    float footstepTimer = 0f;
    bool hasInteracted;

    const float AnimatorDampTime = 0.1f;
    const float CrossFadeDuration = 0.1f;

    public PlayerFreeLookState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.InputReader.TargetEvent += OnTarget;
        stateMachine.InputReader.JumpEvent += OnJump;
        stateMachine.InputReader.InteractEvent += OnInteract;
        stateMachine.InputReader.PauseEvent += OnPause;
        stateMachine.Animator.CrossFadeInFixedTime(FreeLookBlendTreeHash, CrossFadeDuration);
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.InputReader.IsAttacking)
        {
            stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
            return;
        }

        if (stateMachine.InputReader.IsAiming)
        {
            stateMachine.SwitchState(new PlayerAimingState(stateMachine));
            return;
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
        Footsteps();
    }

    public override void Exit()
    {
        stateMachine.InputReader.TargetEvent -= OnTarget;
        stateMachine.InputReader.JumpEvent -= OnJump;
        stateMachine.InputReader.InteractEvent -= OnInteract;
        stateMachine.InputReader.PauseEvent -= OnPause;
    }
    
    void OnTarget()
    {
        if (!stateMachine.Targeter.SelectTarget()) { return; }
        stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
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
    
    void Footsteps()
    {
        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            if (stateMachine.TargetingMovementSpeed != 0)
            {
                stateMachine.AudioManager.PlayFootsteps();
                footstepTimer = 0.5f;
            }
        }
    }
}
