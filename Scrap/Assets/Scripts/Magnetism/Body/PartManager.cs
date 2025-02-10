using System.Collections;
using System.Collections.Generic;
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
            Destroy(partRb);
        }

        part.transform.SetParent(parent.transform);

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
            Destroy(meshRenderer);
        }

        MeshFilter meshFilter = part.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Destroy(meshFilter);
        }

        // Clean up MeshCollider if present
        MeshCollider meshCollider = part.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            Destroy(meshCollider);
        }

        // Add SkinnedMeshRenderer
        SkinnedMeshRenderer skinnedMeshRenderer = part.AddComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null && originalMeshes.ContainsKey(part))
        {
            skinnedMeshRenderer.sharedMesh = originalMeshes[part];
            skinnedMeshRenderer.bones = originalBones[part];
            skinnedMeshRenderer.rootBone = originalRootBones[part];
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
        // Prevent detaching if reattaching is in progress
        if (isReattaching)
        {
            return;  // Exit early if reattaching
        }

        if (part == null || attach.IsBodyPartDetached(part)) return; // Skip if already detached
        secondaryRadiusChecker.UpdateBodyPartCount(-1);

        part.transform.SetParent(null, true);
        part.transform.localScale = originalScales[part];

        // Slightly adjust position to avoid physics overlap issues
        part.transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(0.05f, 0.15f), Random.Range(-0.1f, 0.1f));

        // Remove any existing collider
        Collider existingCollider = part.GetComponent<BoxCollider>();
        if (existingCollider != null)
        {
            Destroy(existingCollider);
        }

        // Ensure there is a MeshFilter before adding a MeshCollider
        MeshFilter meshFilter = part.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            SkinnedMeshRenderer skinnedMesh = part.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMesh != null)
            {
                meshFilter = part.AddComponent<MeshFilter>();
                Mesh bakedMesh = new Mesh();
                skinnedMesh.BakeMesh(bakedMesh);
                meshFilter.mesh = bakedMesh;

                // Also add a MeshRenderer since SkinnedMeshRenderer is being removed
                MeshRenderer meshRenderer = part.AddComponent<MeshRenderer>();
                meshRenderer.materials = skinnedMesh.materials;

                Destroy(skinnedMesh); // Remove SkinnedMeshRenderer to avoid conflicts
            }
            else
            {
                Debug.LogWarning("No SkinnedMeshRenderer or MeshFilter found on " + part.name);
            }
        }

        // Add a new MeshCollider and assign the baked/shared mesh
        MeshCollider newCollider = part.AddComponent<MeshCollider>();
        if (meshFilter != null && meshFilter.mesh != null)
        {
            newCollider.sharedMesh = meshFilter.mesh;
            newCollider.convex = true;  // Must be convex for physics interactions
        }
        else
        {
            Debug.LogWarning("MeshFilter missing or has no mesh on " + part.name + ", can't assign MeshCollider.");
        }

        // Temporarily disable collider to prevent immediate physics conflicts
       
       

        // Ensure Rigidbody is present and enabled
        Rigidbody partRb = part.GetComponent<Rigidbody>();
        if (partRb == null)
        {
            partRb = part.AddComponent<Rigidbody>();
            partRb.mass = 1f;
        }
        partRb.isKinematic = false;
     
      
    }

}