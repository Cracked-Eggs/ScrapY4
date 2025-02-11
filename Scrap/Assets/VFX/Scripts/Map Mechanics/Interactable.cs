using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] UnityEvent onUp;
    [SerializeField] UnityEvent onDown;
    Outline outline;
    Animator animator;
    bool leverDown;
    bool playerInRange;

    void Start()
    {
        outline = GetComponent<Outline>();
        animator = GetComponent<Animator>();
        outline.enabled = false;
        leverDown = false;
        playerInRange = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            outline.enabled = true;
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            outline.enabled = false;
            playerInRange = false;
        }
    }

    public void TryInteract()
    {
        if (playerInRange)
            ToggleLever();
    }

    void ToggleLever()
    {
        if (leverDown)
        {
            Debug.Log("lever up" + this.gameObject.name);
            onUp.Invoke();
        }
        else
        {
            Debug.Log("lever down" + this.gameObject.name);
            onDown.Invoke();
        }
        leverDown = !leverDown;
    }
}
