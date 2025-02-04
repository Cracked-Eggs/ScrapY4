using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] UnityEvent onUp;
    [SerializeField] UnityEvent onDown;
    Outline outline;
    Animator animator;
    bool leverDown;

    void Start()
    {
        outline = GetComponent<Outline>();
        animator = GetComponent<Animator>();
        outline.enabled = false;
        leverDown = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            outline.enabled = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            outline.enabled = false;
    }

    public void ToggleLever()
    {
        if (leverDown)
            Debug.Log("lever up");
        else 
            Debug.Log("lever down");
        leverDown = !leverDown;
    }
}
