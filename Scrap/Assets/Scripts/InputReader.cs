using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, Controls.IPlayerActions
{
    public bool IsAttacking { get; private set; }
    public bool IsBlocking { get; private set; }
    public bool IsAiming { get; private set; }
    public bool IsArm = true;
    public bool IsInCombat;
    public Vector2 MovementValue { get; private set; }

    public event Action JumpEvent;
    public event Action DodgeEvent;
    public event Action TargetEvent;
    public event Action DetachPartEvent;
    public event Action ReattachPartEvent;
    public event Action ShootRightEvent;
    public event Action ShootLeftEvent;

    public event Action DropEverythingEvent;
    public event Action DropLeftArmEvent;
    public event Action DropRightArmEvent;
    public event Action DropLeftLegEvent;
    public event Action DropRightLegEvent;
    public event Action DropBothLegsEvent;

    public event Action RecallBothArmsEvent;
    public event Action RecallLeftArmEvent;
    public event Action RecallRightArmEvent;
    public event Action InteractEvent;
    public event Action AimEvent;

    Controls controls;
    Attach attach;

    void Start()
    {
        controls = new Controls();
        attach = GetComponent<Attach>();
        controls.Player.SetCallbacks(this);
        controls.Player.Enable();
    }

    void OnDestroy() => controls.Player.Disable();

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        JumpEvent?.Invoke();
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        DodgeEvent?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context) => MovementValue = context.ReadValue<Vector2>();

    public void OnLook(InputAction.CallbackContext context) { }

    public void OnTarget(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        TargetEvent?.Invoke();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            IsAttacking = true;
        else if (context.canceled)
            IsAttacking = false;
    }

    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.performed)
            IsBlocking = true;
        else if (context.canceled)
            IsBlocking = false;
    }

    public void OnShootR(InputAction.CallbackContext context) { }

    public void OnShootL(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        ShootLeftEvent?.Invoke();
    }

    // Missing methods for new D-pad actions
    public void OnDropEverything(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        DropEverythingEvent?.Invoke();
    }

    public void OnDropLeftArm(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        DropLeftArmEvent?.Invoke();
    }

    public void OnDropRightArm(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        DropRightArmEvent?.Invoke();
    }

    public void OnDropLeftLeg(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        DropLeftLegEvent?.Invoke();
    }

    public void OnDropRightLeg(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        DropRightLegEvent?.Invoke();
    }

    public void OnDropBothLegs(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        DropBothLegsEvent?.Invoke();
    }

    public void OnRecallBothArms(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        RecallBothArmsEvent?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        InteractEvent?.Invoke();
    }

    public void OnAiming(InputAction.CallbackContext context)
    {
        if (context.performed && !IsInCombat)
        {
            IsAiming = true;
        }
        else if (context.canceled)
        {
            ShootRightEvent?.Invoke();
            IsArm = false;
            IsAiming = false;
        }
        AimEvent?.Invoke();
    }

    IEnumerator AimDelay()
    {
        yield return new WaitForSeconds(2f);
    }
}
