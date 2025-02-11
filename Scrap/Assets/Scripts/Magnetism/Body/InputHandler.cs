using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private Controls inputSystem;
    public Attach attachScript;

    private void Awake()
    {
        inputSystem = new Controls();
        attachScript = GetComponent<Attach>();
    }

    private void OnEnable()
    {
        inputSystem.Player.DetachPart.performed += attachScript.On_Detach;
        inputSystem.Player.DetachPart.Enable();

      

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

     
    }

    private void OnDisable()
    {
        inputSystem.Player.DetachPart.Disable();
        

        inputSystem.Player.ShootR.Disable();
        inputSystem.Player.ShootL.Disable();

        inputSystem.Player.RecallBothArms.Disable();
        inputSystem.Player.RecallLeftArm.Disable();
        inputSystem.Player.RecallRightArm.Disable();

        inputSystem.Player.DropEverything.Disable();
        inputSystem.Player.DropLeftArm.Disable();
        inputSystem.Player.DropRightArm.Disable();
     
    }
}