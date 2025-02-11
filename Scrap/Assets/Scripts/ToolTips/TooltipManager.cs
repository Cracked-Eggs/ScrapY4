using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    public List<Tooltips> tooltips; // List of all tooltips

    private GameObject currentTooltip; // Currently active tooltip

    public void StartTooltip(string id)
    {
        Tooltips tooltip = tooltips.Find(t => t.id == id);
        if (tooltip != null && !tooltip.hasBeenShown)
        {
            // Activate the tooltip
            tooltip.gameObject.SetActive(true);
            tooltip.Animator.SetBool("CanShow", true);

            // Mark the tooltip as shown
            tooltip.hasBeenShown = true;

            // Start a coroutine to hide the tooltip after 1 second
            StartCoroutine(HideStart(tooltip, tooltip.duration));
        }
    }
    
    public void ShowTooltipCoroutine(string id)
    {
        Tooltips tooltip = tooltips.Find(t => t.id == id);
        if (tooltip != null && !tooltip.hasBeenShown)
        {
            // Activate the tooltip
            tooltip.gameObject.SetActive(true);
            tooltip.Animator.SetBool("CanShow", true);

            // Mark the tooltip as shown
            tooltip.hasBeenShown = true;

            // Start a coroutine to hide the tooltip after 1 second
            StartCoroutine(HideTooltipAfterDelay(tooltip, tooltip.duration));
        }
    }
    
    public void ShowTooltip(string id)
    {
        Tooltips tooltip = tooltips.Find(t => t.id == id);
        if (tooltip != null && !tooltip.hasBeenShown)
        {
            // Activate the tooltip
            tooltip.gameObject.SetActive(true);
            tooltip.Animator.SetBool("CanShow", true);

            // Mark the tooltip as shown
            tooltip.hasBeenShown = true;
        }
    }
    
    
    public void RemoveTooltip(string id)
    {
        Tooltips tooltip = tooltips.Find(t => t.id == id);
        if (tooltip != null && tooltip.hasBeenShown)
        {
            // Activate the tooltip
            tooltip.Animator.SetBool("CanShow", false);
            tooltip.hasBeenShown = false;
        }
    }

    // Coroutine to hide the tooltip after a delay
    private IEnumerator HideStart(Tooltips tooltip, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified duration
        tooltip.Animator.SetBool("CanShow", false);
    }
    
    private IEnumerator HideTooltipAfterDelay(Tooltips tooltip, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified duration
        tooltip.Animator.SetBool("CanShow", false);
        ShowTooltipCoroutine("Recall");
    }
}