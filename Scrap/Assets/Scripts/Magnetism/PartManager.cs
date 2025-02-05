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

    public void DetachPart(GameObject part)
    {
        if (part == null || attach.IsBodyPartDetached(part)) return; // Skip if already detached
        secondaryRadiusChecker.UpdateBodyPartCount(-1);

        part.transform.SetParent(null, true);
        part.transform.localScale = originalScales[part];

        // Slightly adjust position to avoid physics overlap issues
        part.transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(0.05f, 0.15f), Random.Range(-0.1f, 0.1f));

        BoxCollider partCollider = part.GetComponent<BoxCollider>();
        if (partCollider != null)
        {
            Vector3 worldScale = part.transform.lossyScale;
            partCollider.size = Vector3.Scale(originalCollidersData[part].size, worldScale) * 0.25f;
            partCollider.center = Vector3.Scale(originalCollidersData[part].center, worldScale) * 0.25f;

            // Temporarily disable collider to prevent immediate physics conflicts
            partCollider.enabled = false;
            StartCoroutine(ReEnableCollider(partCollider, 0.1f));
        }

        Rigidbody partRb = part.GetComponent<Rigidbody>();
        if (partRb == null)
        {
            partRb = part.AddComponent<Rigidbody>();
            partRb.mass = 1f;
        }
        partRb.isKinematic = false;

        SkinnedMeshRenderer skinnedMesh = part.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMesh != null)
        {
            Mesh bakedMesh = new Mesh();
            skinnedMesh.BakeMesh(bakedMesh);
            MeshFilter meshFilter = part.AddComponent<MeshFilter>();
            meshFilter.mesh = bakedMesh;
            MeshRenderer meshRenderer = part.AddComponent<MeshRenderer>();
            meshRenderer.materials = skinnedMesh.materials;
            Destroy(skinnedMesh);
        }
    }

    // Coroutine to re-enable collider after delay
    private IEnumerator ReEnableCollider(BoxCollider collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (collider != null)
        {
            collider.enabled = true;
        }
    }


    public IEnumerator ShakeAndReattach(GameObject part)
    {
        secondaryRadiusChecker.UpdateBodyPartCount(1);


        Rigidbody partRb = part.GetComponent<Rigidbody>();
        if (partRb != null)
        {
            Destroy(partRb);
        }

        part.transform.SetParent(parent.transform);
        float reattachSpeed = 5f;
        float rotationSpeed = 5f;
        float reattachDistanceThreshold = 0.1f;

        while (Vector3.Distance(part.transform.localPosition, originalPositions[part]) > reattachDistanceThreshold)
        {
            part.transform.localPosition = Vector3.Lerp(part.transform.localPosition, originalPositions[part], reattachSpeed * Time.deltaTime);
            part.transform.localRotation = Quaternion.Slerp(part.transform.localRotation, originalRotations[part], rotationSpeed * Time.deltaTime);
            yield return null;
        }

        part.transform.localPosition = originalPositions[part];
        part.transform.localRotation = originalRotations[part];
        part.transform.localScale = originalScales[part];

        BoxCollider partCollider = part.GetComponent<BoxCollider>();
        if (partCollider != null)
        {
            partCollider.size = originalCollidersData[part].size;
            partCollider.center = originalCollidersData[part].center;
            partCollider.enabled = false;
        }

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

        SkinnedMeshRenderer skinnedMeshRenderer = part.AddComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null && originalMeshes.ContainsKey(part))
        {
            skinnedMeshRenderer.sharedMesh = originalMeshes[part];
            skinnedMeshRenderer.bones = originalBones[part];
            skinnedMeshRenderer.rootBone = originalRootBones[part];
        }
    }
}