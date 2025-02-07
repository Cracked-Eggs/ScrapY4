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

        inputSystem.Player.ReattachPart.performed += attachScript.On_Reattach;
        inputSystem.Player.ReattachPart.Enable();

        inputSystem.Player.ShootR.performed += attachScript.ShootRightArm;
        inputSystem.Player.ShootR.Enable();

        inputSystem.Player.ShootL.performed += attachScript.ShootLeftArm;
        inputSystem.Player.ShootL.Enable();

        inputSystem.Player.RecallBothArms.performed += attachScript.RecallBothArms;
        inputSystem.Player.RecallBothArms.Enable();
        inputSystem.Player.RecallLeftArm.performed += attachScript.RecallLeftArm;
        inputSystem.Player.RecallLeftArm.Enable();
        inputSystem.Player.RecallRightArm.performed += attachScript.RecallRightArm;
        inputSystem.Player.RecallRightArm.Enable();

        inputSystem.Player.DropEverything.performed += attachScript.DropEverything;
        inputSystem.Player.DropEverything.Enable();

        inputSystem.Player.DropLeftArm.performed += attachScript.DropLeftArm;
        inputSystem.Player.DropLeftArm.Enable();

        inputSystem.Player.DropRightArm.performed += attachScript.DropRightArm;
        inputSystem.Player.DropRightArm.Enable();

        inputSystem.Player.DropLeftLeg.performed += attachScript.DropLeftLeg;
        inputSystem.Player.DropLeftLeg.Enable();

        inputSystem.Player.DropRightLeg.performed += attachScript.DropRightLeg;
        inputSystem.Player.DropRightLeg.Enable();

        inputSystem.Player.DropBothLegs.performed += attachScript.DropBothLegs;
        inputSystem.Player.DropBothLegs.Enable();
    }

    private void OnDisable()
    {
        inputSystem.Player.DetachPart.Disable();
        inputSystem.Player.ReattachPart.Disable();

        inputSystem.Player.ShootR.Disable();
        inputSystem.Player.ShootL.Disable();

        inputSystem.Player.RecallBothArms.Disable();
        inputSystem.Player.RecallLeftArm.Disable();
        inputSystem.Player.RecallRightArm.Disable();

        inputSystem.Player.DropEverything.Disable();
        inputSystem.Player.DropLeftArm.Disable();
        inputSystem.Player.DropRightArm.Disable();
        inputSystem.Player.DropLeftLeg.Disable();
        inputSystem.Player.DropRightLeg.Disable();
        inputSystem.Player.DropBothLegs.Disable();
    }
}