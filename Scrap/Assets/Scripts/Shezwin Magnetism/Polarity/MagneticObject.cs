using UnityEngine;

public class MagneticObject : MonoBehaviour
{
    public bool polarity; // Polarity of the object, true for positive, false for negative
    public Material defaultMaterial;
    public Material proximityMaterial; // Material to indicate proximity

    private Renderer objectRenderer;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            defaultMaterial = objectRenderer.material; // Store the default material
        }
    }

    public void OnEnterMagneticField(bool isInProximity)
    {
        if (isInProximity)
        {
            // Change the material of the object to indicate it's in proximity
            objectRenderer.material = proximityMaterial;
        }
        else
        {
            // Revert to the original material when it's out of proximity
            objectRenderer.material = defaultMaterial;
        }
    }

    private void OnDrawGizmos()
    {
        // Optional: You can also draw a small circle to indicate object position in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}
