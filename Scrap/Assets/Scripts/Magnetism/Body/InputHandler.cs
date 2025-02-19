using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Controls inputSystem;
    public Attach attachScript;
    public PlayerRollingHeadState headState; 
    public PlayerStateMachine stateMachine;

    private void Awake()
    {
        inputSystem = new Controls();
        attachScript = GetComponent<Attach>();
        stateMachine = GetComponent<PlayerStateMachine>();  

    }

    public void OnEnable()
    {
        inputSystem.Player.Aiming.canceled += OnAimingCanceled;
        inputSystem.Player.Aiming.Enable();

        inputSystem.Player.ShootR.performed += attachScript.ShootOrRecallRightArm;
        inputSystem.Player.ShootR.Enable();

        inputSystem.Player.ShootL.performed += attachScript.ShootOrRecallLeftArm;
        inputSystem.Player.ShootL.Enable();

        inputSystem.Player.RecallBothArms.performed += attachScript.RecallBothArms;
        inputSystem.Player.RecallBothArms.Enable();

        inputSystem.Player.DropEverything.performed += attachScript.ToggleDetachReattach;
        inputSystem.Player.DropEverything.Enable();

        inputSystem.Player.DropLeftArm.performed += attachScript.DropLeftArm;
        inputSystem.Player.DropLeftArm.Enable();

        inputSystem.Player.DropRightArm.performed += attachScript.DropRightArm;
        inputSystem.Player.DropRightArm.Enable();

        inputSystem.Player.ActivateGrappleAndReattach.performed += attachScript.ActivateGrappleAndReattach;
        inputSystem.Player.ActivateGrappleAndReattach.Enable();

        inputSystem.Player.Hover.performed += ctx => StartHover();
        inputSystem.Player.Hover.canceled += ctx => StopHover();
        inputSystem.Player.Hover.Enable();
    }
    public void OnDisable()
    {
        inputSystem.Player.Aiming.canceled -= OnAimingCanceled;
        inputSystem.Player.Aiming.Disable();

        inputSystem.Player.ShootR.Disable();
        inputSystem.Player.ShootL.Disable();

        inputSystem.Player.RecallBothArms.Disable();

        inputSystem.Player.DropEverything.Disable();
        inputSystem.Player.DropLeftArm.Disable();
        inputSystem.Player.DropRightArm.Disable();

        inputSystem.Player.ActivateGrappleAndReattach.Disable();
        inputSystem.Player.Hover.Disable();
    }

    private void OnAimingCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("Aiming released. Shooting right arm.");
        attachScript.ShootOrRecallRightArm(context);
    }
    private void StartHover()
    {
        Debug.Log("Hover Started");
        
            stateMachine.isHovering = true;
    }

    private void StopHover()
    {
        Debug.Log("Hover Stopped");
      
            stateMachine.isHovering = false;
    }
}