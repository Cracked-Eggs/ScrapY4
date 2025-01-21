using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] GameObject pressurePlate;
    [SerializeField] private float delay = 1.5f;

    private Animator plateAnimator;

    private PlatePuzzle puzzle;
    
    private bool isPressed = false;
    private bool isOpen = false;

    public bool IsPressed => isPressed;
    
    private void Start()
    {
        if (pressurePlate != null)
            plateAnimator = pressurePlate.GetComponent<Animator>();
        
        if (plateAnimator != null)
            plateAnimator.SetBool("IsPressed", false);
    }

    //Set the puzzle this plate belongs to
    public void SetPuzzle(PlatePuzzle assignedPuzzle)
    {
        puzzle = assignedPuzzle;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        isPressed = true;
        UpdatePlateState();

        if (puzzle != null)
            puzzle.PlateActivated();
    }

    private void OnTriggerExit(Collider other)
    {
        isPressed = false;
        UpdatePlateState();
        
        if (puzzle != null)
            puzzle.PlateDeactivated();
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
    }
    
    private void DeactivatePlate()
    {
        Debug.Log("PressurePlate Deactivated");
        
        if (plateAnimator != null)
            plateAnimator.SetBool("IsPressed", false);
    }
}
