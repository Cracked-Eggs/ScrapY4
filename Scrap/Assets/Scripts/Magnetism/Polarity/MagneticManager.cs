using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MagneticManager : MonoBehaviour
{
    public static MagneticManager Instance; // Singleton for easy access

    private List<MagneticField> activeMagneticObjects = new List<MagneticField>();
    
    private VFXManager vFXManager;

    // Inspector exposed fields
    public GameObject leftArm;  // Reference to the left arm (drag in the Inspector)
    public GameObject rightArm; // Reference to the right arm (drag in the Inspector)

    
    public bool canGrapple = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Assign the VFXManager (Ensure it's in the scene)
        vFXManager = FindObjectOfType<VFXManager>();
        if (vFXManager == null)
        {
            Debug.LogError("VFXManager not found in the scene! Assign it manually.");
        }
    }

    void Update()
    {
       
        bool newCanGrapple = false; // Temporary variable to track this frame's grapple status

        // Check interactions between all active magnetic objects
        for (int i = 0; i < activeMagneticObjects.Count; i++)
        {
            for (int j = i + 1; j < activeMagneticObjects.Count; j++)
            {
                MagneticField objA = activeMagneticObjects[i];
                MagneticField objB = activeMagneticObjects[j];

                if (objA == null || objB == null) continue;

                float distance = Vector3.Distance(objA.transform.position, objB.transform.position);

                // Check if they are close enough to interact
                if (distance <= 2f)
                {
                    // Determine if the objects are attracting or repelling
                    bool isAttracting = objA.isPositivePolarity != objB.isPositivePolarity; // Opposite polarity attracts
                    string interactionType = isAttracting ? "Attracting" : "Repelling";


                    // Check if attraction is happening between either arm and a MagneticWall
                    if (isAttracting && (objB.gameObject == leftArm || objB.gameObject == rightArm) && objA.gameObject.CompareTag("MagneticWall"))
                    {
                        newCanGrapple = true; // Update temporary flag
                  

                    }
                }
            }
        }

   

       
     
        canGrapple = newCanGrapple; 
    }

    public void RegisterMagneticObject(MagneticField obj)
    {
        if (!activeMagneticObjects.Contains(obj))
            activeMagneticObjects.Add(obj);
    }

    public void UnregisterMagneticObject(MagneticField obj)
    {
        if (activeMagneticObjects.Contains(obj))
            activeMagneticObjects.Remove(obj);
    }

    
    public bool CanGrapple()
    {
        return canGrapple;
    }
}
