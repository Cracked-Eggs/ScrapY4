using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] GameObject objectToActivate; //door
    [SerializeField] GameObject pressurePlate;
    [SerializeField] private float delay = 1.5f;

    private Animator plateAnimator;
    private Animator doorAnimator;
    
    private bool isPressed = false;
    private bool isOpen = false;

    private void Start()
    {
        if (pressurePlate != null)
            plateAnimator = pressurePlate.GetComponent<Animator>();
        
        if (objectToActivate != null)
            doorAnimator = objectToActivate.GetComponent<Animator>();
        
        if (plateAnimator != null)
            plateAnimator.SetBool("IsPressed", false);
        
        if (doorAnimator != null)
            doorAnimator.SetBool("IsOpen", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        isPressed = true;
        UpdatePlateState();
    }

    private void OnTriggerExit(Collider other)
    {
        isPressed = false;
        UpdatePlateState();
    }

    private void UpdatePlateState()
    {
        if (isPressed)
        {
            ActivatePlate();
        }
        else if(isPressed == false)
        {
            DeactivatePlate();
        }
    }

    private void ActivatePlate()
    {
        Debug.Log("PressurePlate Activated");

        if (plateAnimator != null)
            plateAnimator.SetBool("IsPressed", true);

        if (doorAnimator != null)
        {
            isOpen = true;
            StartCoroutine(ActivateDoorWithDelay());
        }
            
    }
    
    private void DeactivatePlate()
    {
        Debug.Log("PressurePlate Deactivated");
        
        if (plateAnimator != null)
            plateAnimator.SetBool("IsPressed", false);

        if (doorAnimator != null)
        {
            isOpen = false;
            StartCoroutine(ActivateDoorWithDelay());
        }
    }

    private IEnumerator ActivateDoorWithDelay()
    {
        yield return new WaitForSeconds(delay);

        if (isOpen)
        {
            doorAnimator.SetBool("IsOpen", true);
        }
        else if (isOpen == false)
        {
            doorAnimator.SetBool("IsOpen", false);
        }
        
    }
}
