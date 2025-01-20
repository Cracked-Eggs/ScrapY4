using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatePuzzle : MonoBehaviour
{
    //[SerializeField] private GameObject door;
    [SerializeField] private List<PressurePlate> plates;
    [SerializeField] private float delay = 1f;
    
    private Animator doorAnimator;

    private bool isOpen = false;
    private int activePlates = 0;
    
    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        
        if (doorAnimator != null)
            doorAnimator.SetBool("IsOpen", false);

        foreach (var plate in plates)
        {
            plate.SetPuzzle(this);
        }
    }

    public void PlateActivated()
    {
        //Debug.Log("activated");
        activePlates++;
        CheckPlates();
    }

    public void PlateDeactivated()
    {
        activePlates--;
        CheckPlates();
    }

    private void CheckPlates()
    {
        if (activePlates >= plates.Count)
            OpenDoor();
        else
            CloseDoor();
    }

    private void OpenDoor()
    {
        //Debug.Log("trying to open");
        isOpen = true;
        StartCoroutine(ActivateDoorWithDelay());
    }

    private void CloseDoor()
    {
        isOpen = false;
        StartCoroutine(ActivateDoorWithDelay());
    }
    
    private IEnumerator ActivateDoorWithDelay()
    {
        yield return new WaitForSeconds(delay);

        if (isOpen)
        {
            //Debug.Log("opening door");
            doorAnimator.SetBool("IsOpen", true);
        }
        else if (isOpen == false)
        {
            //Debug.Log("closing door");
            doorAnimator.SetBool("IsOpen", false);
        }
        
    }
}
