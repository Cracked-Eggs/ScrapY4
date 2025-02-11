using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] UnityEvent onUp;
    [SerializeField] UnityEvent onDown;
    Outline[] outlines; // Renamed to plural for clarity
    Animator animator;
    bool leverDown;
    bool playerInRange;

    void Start()
    {
        outlines = GetComponentsInChildren<Outline>(); // Get all Outline components in children
        animator = GetComponent<Animator>();
        SetOutlinesEnabled(false); // Disable all outlines at start
        leverDown = false;
        playerInRange = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SetOutlinesEnabled(true); // Enable all outlines
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SetOutlinesEnabled(false); // Disable all outlines
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
            Debug.Log("lever up " + this.gameObject.name);
            onUp.Invoke();
            animator.SetTrigger("Up");
        }
        else
        {
            Debug.Log("lever down " + this.gameObject.name);
            onDown.Invoke();
            animator.SetTrigger("Down");
        }
        leverDown = !leverDown;
    }

    // Helper method to enable/disable all outlines
    void SetOutlinesEnabled(bool enabled)
    {
        if (outlines != null)
        {
            foreach (Outline outline in outlines)
            {
                if (outline != null)
                {
                    outline.enabled = enabled;
                }
            }
        }
    }
}