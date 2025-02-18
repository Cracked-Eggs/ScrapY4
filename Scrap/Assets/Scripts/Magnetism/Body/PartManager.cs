using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
    public Dictionary<GameObject, Mesh> preBakedMeshes = new Dictionary<GameObject, Mesh>();
    private Dictionary<string, GameObject> gameObjectDictionary = new Dictionary<string, GameObject>();
    public Transform headParent, torsoParent, r_LegParent, l_LegParent, r_ArmParent, l_ArmParent;

    [SerializeField] private RadiusChecker secondaryRadiusChecker;
    [SerializeField] private Attach attach;
    public float reattachSpeed = 10f;
    public float rotationSpeed = 2f;
    public float reattachDistanceThreshold = 0.1f;
    private VFXManager vfxManager;  
    private InputReader inputReader;

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
        vfxManager = GetComponent<VFXManager>();
        inputReader = GetComponent<InputReader>();
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

             
                if (part.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
                {
                    Debug.Log("MeshFilter already added");
                }
                else
                {
                    meshFilter = part.AddComponent<MeshFilter>();
                }
                if (!preBakedMeshes.ContainsKey(part))
                {
                    Debug.LogError($"Pre-baked mesh for {part.name} not found!");
                }
                meshFilter.mesh = bakedMesh;

                
                if (part.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
                {
                    Debug.Log("MeshRenderer already added");
                }
                else
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
        GameObject[] bodyParts = { head, head2, head3, head4, head5, torso, r_Leg, l_Leg, r_Arm, l_Arm };
        foreach (GameObject part in bodyParts)
            if (!preBakedMeshes.ContainsKey(part))
            {
                Debug.LogError($"Pre-baked mesh for {part.name} not found!");
                part.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;
            }

    }
    private Transform GetParentForPart(GameObject part)
    {
        if (part == head) return headParent;
        if (part == torso) return torsoParent;
        if (part == r_Leg) return r_LegParent;
        if (part == l_Leg) return l_LegParent;
        if (part == r_Arm) return r_ArmParent;
        if (part == l_Arm) return l_ArmParent;
        return null; // Fallback if no parent is found
    }

    private void StoreOriginalTransforms(GameObject part)
    {
        originalPositions[part] = part.transform.localPosition;
        originalRotations[part] = part.transform.localRotation;
        originalScales[part] = part.transform.localScale;

        
        if (part.TryGetComponent<BoxCollider>(out BoxCollider collider))
        {
            originalCollidersData[part] = new ColliderData(collider);
        }

      
        if (part.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer renderer))
        {
            originalBones[part] = renderer.bones;
            originalRootBones[part] = renderer.rootBone;
            originalMeshes[part] = renderer.sharedMesh;
        }
    }


    public bool isReattaching = false;
    
    public IEnumerator ShakeAndReattach(GameObject part)
    {
       

        secondaryRadiusChecker.UpdateBodyPartCount(1);

        if(part == torso)
        {
            vfxManager.StopVFX("Torso");
            vfxManager.StopVFX("Head");
        }
        if (part == r_Leg)
        {
            vfxManager.StopVFX("R_Leg");
        }
        if (part == l_Leg)
        {
            vfxManager.StopVFX("L_Leg");
        }
        if (part == r_Arm)
        {
            vfxManager.StopVFX("R_Arm");
        }
        if (part == l_Arm)
        {
            vfxManager.StopVFX("L_Arm");
        }

        if (part.TryGetComponent<Rigidbody>(out Rigidbody partRb))
        {
            partRb.isKinematic = true;
        }


        part.transform.SetParent(GetParentForPart(part));
       
        part.transform.position = part.transform.position;

        // Make sure to preserve the part's scale during reattachment
      

        while (Vector3.Distance(part.transform.localPosition, originalPositions[part]) > reattachDistanceThreshold)
        {
            part.transform.localPosition = Vector3.Lerp(part.transform.localPosition, originalPositions[part], reattachSpeed * Time.deltaTime);
            part.transform.localRotation = Quaternion.Slerp(part.transform.localRotation, originalRotations[part], rotationSpeed * Time.deltaTime);
            yield return null;
        }

        part.transform.localPosition = originalPositions[part];
        part.transform.localRotation = originalRotations[part];
        
        if (part.TryGetComponent<BoxCollider>(out BoxCollider partCollider))
        {
            partCollider.size = originalCollidersData[part].size;
            partCollider.center = originalCollidersData[part].center;
            partCollider.enabled = true;
        }
        else
        {
            partCollider = part.AddComponent<BoxCollider>();
            partCollider.size = originalCollidersData[part].size;
            partCollider.center = originalCollidersData[part].center;
            partCollider.enabled = true;
        }

        // Clean up the MeshRenderer and MeshFilter if necessary
        
        // Clean up MeshCollider if present
        if (part.TryGetComponent<MeshCollider>(out MeshCollider meshCollider))
        {
            meshCollider.enabled = false;
        }


        if (part.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
        {
            meshRenderer.enabled = false;
        }

       
        // Finally, enable the SkinnedMeshRenderer
        if (part.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer skinnedMesh))
        {
            skinnedMesh.enabled = true;
            skinnedMesh.sharedMesh = originalMeshes[part];
            skinnedMesh.bones = originalBones[part];
            skinnedMesh.rootBone = originalRootBones[part];
        }
        part.transform.localScale = originalScales[part];

        
    }


    public void DetachPart(GameObject part)
    {
        if (isReattaching || part == null || attach.IsBodyPartDetached(part)) return;

        secondaryRadiusChecker.UpdateBodyPartCount(-1);
        Vector3 worldPos = part.transform.position;
        Quaternion worldRot = part.transform.rotation;

        // Detach from parent
        part.transform.SetParent(null, true);

        // Apply the world position & rotation back
        part.transform.position = worldPos;
        part.transform.rotation = worldRot;
        part.transform.localScale = Vector3.one;



        if (part.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer skinnedMesh))
        {
            skinnedMesh.enabled = false;
        }

       
        if (part.TryGetComponent<BoxCollider>(out BoxCollider existingCollider))
        {
            existingCollider.enabled = false;
        }

        // Enable mesh collider
       
        if (part.TryGetComponent<MeshCollider>(out MeshCollider meshCollider))
        {
            meshCollider.enabled = true;
        }

     
        if (part.TryGetComponent<MeshFilter>(out MeshFilter meshFilter))
        {
            meshFilter.mesh = null;
            meshFilter.mesh = preBakedMeshes[part];
        }

       
        if (part.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
        {
            meshRenderer.enabled = true;
        }

        if (part.TryGetComponent<Rigidbody>(out Rigidbody partRb))
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