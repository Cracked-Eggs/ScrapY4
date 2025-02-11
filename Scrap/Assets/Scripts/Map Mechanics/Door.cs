using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] float delay = 1f;
    
    Animator doorAnimator;
    bool isOpen = false;
    
    void Start() => doorAnimator = GetComponent<Animator>();

    public void OpenDoor()
    {
        isOpen = true;
        StartCoroutine(ActivateDoorWithDelay());
    }

    public void CloseDoor()
    {
        isOpen = false;
        StartCoroutine(ActivateDoorWithDelay());
    } 
    
    IEnumerator ActivateDoorWithDelay()
    {
        yield return new WaitForSeconds(delay);

        if (isOpen)
            doorAnimator.SetBool("IsOpen", true);
        else if (isOpen == false)
            doorAnimator.SetBool("IsOpen", false);
    }
}
