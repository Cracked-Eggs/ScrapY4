using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PartManager : MonoBehaviour
{
    public GameObject head, head2, head3, head4, head5, torso, r_Leg, l_Leg, r_Arm, l_Arm, parent;
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> originalRotations = new Dictionary<GameObject, Quaternion>();
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Transform[]> originalBones = new Dictionary<GameObject, Transform[]>();
    private Dictionary<GameObject, Transform> originalRootBones = new Dictionary<GameObject, Transform>();
    private Dictionary<GameObject, Mesh> originalMeshes = new Dictionary<GameObject, Mesh>();
    private Dictionary<GameObject, ColliderData> originalCollidersData = new Dictionary<GameObject, ColliderData>();
    private Dictionary<GameObject, Mesh> preBakedMeshes = new Dictionary<GameObject, Mesh>();
    [SerializeField] private RadiusChecker secondaryRadiusChecker;
    [SerializeField] private Attach attach;
    public float reattachSpeed = 10f;
    public float rotationSpeed = 2f;
    public float reattachDistanceThreshold = 0.1f;

    [System.Serializable]
    public struct ColliderData
    {
        public Vector3 size;
        public Vector3 center;
        public bool isTrigger;

        public ColliderData(BoxCollider boxCollider)
        {
            size = boxCollider.size;
            center = boxCollider.center;
            isTrigger = boxCollider.isTrigger;
        }
    }

    private void Start()
    {
        StoreOriginalTransforms(head);
        StoreOriginalTransforms(head2);
        StoreOriginalTransforms(head3);
        StoreOriginalTransforms(head4);
        StoreOriginalTransforms(head5);
        StoreOriginalTransforms(torso);
        StoreOriginalTransforms(r_Leg);
        StoreOriginalTransforms(l_Leg);
        StoreOriginalTransforms(r_Arm);
        StoreOriginalTransforms(l_Arm);
        GameObject[] bodyParts = { head, head2, head3, head4, head5, torso, r_Leg, l_Leg, r_Arm, l_Arm };
        foreach (GameObject part in bodyParts)
        {
            SkinnedMeshRenderer skinnedMesh = part.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMesh)
            {
                Mesh bakedMesh = new Mesh();
                skinnedMesh.BakeMesh(bakedMesh);
                preBakedMeshes[part] = bakedMesh;

                // 🔹 Pre-add and disable MeshCollider
                MeshCollider meshCollider = part.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = bakedMesh;
                meshCollider.convex = true;
                meshCollider.enabled = false; // Disabled until detachment

                // 🔹 Pre-add MeshFilter and MeshRenderer but disable them
                MeshFilter meshFilter = part.GetComponent<MeshFilter>();
                if (meshFilter == null)
                {
                    meshFilter = part.AddComponent<MeshFilter>();
                }
                meshFilter.mesh = bakedMesh;

                MeshRenderer meshRenderer = part.GetComponent<MeshRenderer>();
                if (meshRenderer == null)
                {
                    meshRenderer = part.AddComponent<MeshRenderer>();
                }
                meshRenderer.materials = skinnedMesh.materials;
                meshRenderer.enabled = false; // Keep it disabled until detachment


              
            }
        }
    
    }
    private void Update()
    {
        
    }

    private void StoreOriginalTransforms(GameObject part)
    {
        originalPositions[part] = part.transform.localPosition;
        originalRotations[part] = part.transform.localRotation;
        originalScales[part] = part.transform.localScale;

        BoxCollider collider = part.GetComponent<BoxCollider>();
        if (collider != null)
        {
            originalCollidersData[part] = new ColliderData(collider);
        }

        SkinnedMeshRenderer renderer = part.GetComponent<SkinnedMeshRenderer>();
        if (renderer != null)
        {
            originalBones[part] = renderer.bones;
            originalRootBones[part] = renderer.rootBone;
            originalMeshes[part] = renderer.sharedMesh;
        }
    }


    public bool isReattaching = false;

    public IEnumerator ShakeAndReattach(GameObject part)
    {
        isReattaching = true;

        secondaryRadiusChecker.UpdateBodyPartCount(1);

        Rigidbody partRb = part.GetComponent<Rigidbody>();
        if (partRb != null)
        {
            partRb.isKinematic = true;
        }

        part.transform.SetParent(parent.transform); 
        part.transform.position = part.transform.position;

        while (Vector3.Distance(part.transform.localPosition, originalPositions[part]) > reattachDistanceThreshold)
        {
            part.transform.localPosition = Vector3.Lerp(part.transform.localPosition, originalPositions[part], reattachSpeed * Time.deltaTime);
            part.transform.localRotation = Quaternion.Slerp(part.transform.localRotation, originalRotations[part], rotationSpeed * Time.deltaTime);
            yield return null;
        }

        part.transform.localPosition = originalPositions[part];
        part.transform.localRotation = originalRotations[part];
        part.transform.localScale = originalScales[part];

        // Reattach BoxCollider if it exists
        BoxCollider partCollider = part.GetComponent<BoxCollider>();
        if (partCollider != null)
        {
            // Restore original collider size and center
            partCollider.size = originalCollidersData[part].size;
            partCollider.center = originalCollidersData[part].center;
            partCollider.enabled = true;  // Make sure it's enabled
        }
        else
        {
            // If no BoxCollider exists, you can add one back
            partCollider = part.AddComponent<BoxCollider>();
            partCollider.size = originalCollidersData[part].size;
            partCollider.center = originalCollidersData[part].center;
            partCollider.enabled = true; // Ensure it's enabled
        }

        // Clean up the MeshRenderer and MeshFilter if necessary
        MeshRenderer meshRenderer = part.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        // Clean up MeshCollider if present
        MeshCollider meshCollider = part.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.enabled = false;
        }

        // Add SkinnedMeshRenderer
        SkinnedMeshRenderer skinnedMesh = part.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMesh != null)
        {
            skinnedMesh.enabled = enabled;
            skinnedMesh.sharedMesh = originalMeshes[part];
            skinnedMesh.bones = originalBones[part];
            skinnedMesh.rootBone = originalRootBones[part];
        }
          
        

        isReattaching = false;  // Reattachment is complete
        attach.leftArmVFX.Stop();
        attach.rightArmVFX.Stop();
        attach.RightLegVFX.Stop();
        attach.LeftLegVFX.Stop();
        attach.HeadVFX.Stop();
        attach.rightArmARCVFX.SetActive(false);
        attach.leftArmARCVFX.SetActive(false);
        attach.headARCVFX.SetActive(false);
        attach.LeftLegARCVFX.SetActive(false);
        attach.RightLegARCVFX .SetActive(false);
       
    }

    public void DetachPart(GameObject part)
    {
        if (isReattaching || part == null || attach.IsBodyPartDetached(part)) return;

        secondaryRadiusChecker.UpdateBodyPartCount(-1);
        part.transform.SetParent(null, true);
        part.transform.localScale = originalScales[part];

        SkinnedMeshRenderer skinnedMesh = part.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMesh != null)
        {
            skinnedMesh.enabled = false;
        }

        Collider existingCollider = part.GetComponent<BoxCollider>();
        if (existingCollider != null)
        {
            existingCollider.enabled = false;
        }

        // Enable mesh collider
        MeshCollider meshCollider = part.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.enabled = true;
        }

        // Assign the baked mesh to MeshFilter
        MeshFilter meshFilter = part.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh = preBakedMeshes[part];
        }

        // Enable MeshRenderer
        MeshRenderer meshRenderer = part.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }

        
        Rigidbody partRb = part.GetComponent<Rigidbody>();
        if (partRb != null)
        {
            
            partRb.isKinematic = false; Debug.Log("Rigidbody mass before detachment: " + partRb.mass);
            partRb.mass = 1f;
            Debug.Log("Rigidbody mass after detachment: " + partRb.mass);

            partRb.WakeUp(); // Ensure the Rigidbody is awake


            Physics.SyncTransforms();


            Debug.Log("yo");
        }
       

    }







}