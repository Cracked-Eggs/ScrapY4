using System.Collections;
using UnityEngine;

public class Platforms : MonoBehaviour
{
    [SerializeField] float delay = 1f;
    
    Animator platformAnimator;
    bool isOn = false;
    
    void Start() => platformAnimator = GetComponent<Animator>();

    public void RaisePlatform()
    {
        isOn = true;
        StartCoroutine(ActivatePlatformWithDelay());
    }

    public void LowerPlatform()
    {
        isOn = false;
        StartCoroutine(ActivatePlatformWithDelay());
    } 
    
    IEnumerator ActivatePlatformWithDelay()
    {
        yield return new WaitForSeconds(delay);

        if (isOn)
            platformAnimator.SetBool("IsOn", true);
        else if (isOn == false)
            platformAnimator.SetBool("IsOn", false);
    }
}