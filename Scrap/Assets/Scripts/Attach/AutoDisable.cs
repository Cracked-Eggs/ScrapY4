using UnityEngine;
using UnityEngine.UI;

public class AutoDisable : MonoBehaviour
{
    public Attach attach;
    public enum LimbType { LeftArm, RightArm }
    public LimbType limbToMonitor;

    private Image imageComponent;
    private TargetIndicator targetIndicator; // Reference to indicator script

    private void Start()
    {
        attach = FindObjectOfType<Attach>();

        if (attach == null)
        {
            Debug.LogError("Attach script not found in the scene!", this);
        }

        imageComponent = GetComponent<Image>();

        if (imageComponent == null)
        {
            Debug.LogWarning("No Image component found! Make sure this object has a UI Image component.", this);
        }

        targetIndicator = GetComponent<TargetIndicator>();
    }

    private void Update()
    {
        if (attach == null || imageComponent == null) return;

        bool isDetached = false;

        switch (limbToMonitor)
        {
            case LimbType.LeftArm:
                isDetached = attach._isL_ArmDetached;
                break;
            case LimbType.RightArm:
                isDetached = attach._isR_ArmDetached;
                break;
        }

        // Only enable the image if the limb is detached AND the target is in view
        if (targetIndicator != null)
        {
            imageComponent.enabled = isDetached && targetIndicator.TargetIndicatorImage.enabled;
        }
        else
        {
            imageComponent.enabled = isDetached;
        }
    }
}
