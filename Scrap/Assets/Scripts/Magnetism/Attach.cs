using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Attach : MonoBehaviour
{

    private Controls inputSystem;

    private Rigidbody _rb;
    private Collider playerCollider;

    public bool canRetach;

    [SerializeField] public float customGravity = -9.81f;
    [SerializeField] AudioClip magnetRepel;
    [SerializeField] public float shootingForce = 500f;
    AudioSource _audioSource;
    Animator _animator;

    public GameObject head, head2, head3, head4, head5, torso, r_Leg, l_Leg, r_Arm, l_Arm, parent;
    public float detachCooldown = 2.0f; // Time in seconds between detachments
    private float lastDetachTime = -2.0f; // Last time a detach was allowed

    public bool isDetached = false;
    public bool _isL_ArmDetached = false;
    public bool _isR_ArmDetached = false;
    public bool _isL_LegDetached = false;
    public bool _isR_LegDetached = false;

    public bool _isBothLegsDetached = false;
    public bool _isEverythingDetached = false;

    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> originalRotations = new Dictionary<GameObject, Quaternion>();
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Transform[]> originalBones = new Dictionary<GameObject, Transform[]>();
    private Dictionary<GameObject, Transform> originalRootBones = new Dictionary<GameObject, Transform>();
    private Dictionary<GameObject, Mesh> originalMeshes = new Dictionary<GameObject, Mesh>();
    private Dictionary<GameObject, ColliderData> originalCollidersData = new Dictionary<GameObject, ColliderData>();

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

    private void Awake()
    {
        inputSystem = new Controls();

        _rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);

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
    private void Update()
    {
        if (_isR_LegDetached && _isL_LegDetached)
        {
            _isBothLegsDetached = false;
            CharacterController controller = GetComponent<CharacterController>();
            controller.height = 0f;
        }
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

    public void OnEnable()
    {
        inputSystem.Player.DetachPart.performed += On_Detach;
        inputSystem.Player.DetachPart.Enable();

        inputSystem.Player.ReattachPart.performed += On_Reattach;
        inputSystem.Player.ReattachPart.Enable();

        inputSystem.Player.ShootR.performed += ShootRightArm;
        inputSystem.Player.ShootR.Enable();

        inputSystem.Player.ShootL.performed += ShootLeftArm;
        inputSystem.Player.ShootL.Enable();

        inputSystem.Player.RecallBothArms.performed += RecallBothArms;
        inputSystem.Player.RecallBothArms.Enable();
        inputSystem.Player.RecallLeftArm.performed += RecallLeftArm;
        inputSystem.Player.RecallLeftArm.Enable();
        inputSystem.Player.RecallRightArm.performed += RecallRightArm;
        inputSystem.Player.RecallRightArm.Enable();

        // Bind new D-pad controls
        inputSystem.Player.DropEverything.performed += DropEverything;
        inputSystem.Player.DropEverything.Enable();

        inputSystem.Player.DropLeftArm.performed += DropLeftArm;
        inputSystem.Player.DropLeftArm.Enable();

        inputSystem.Player.DropRightArm.performed += DropRightArm;
        inputSystem.Player.DropRightArm.Enable();

        inputSystem.Player.DropLeftLeg.performed += DropLeftLeg;
        inputSystem.Player.DropLeftLeg.Enable();

        inputSystem.Player.DropRightLeg.performed += DropRightLeg;
        inputSystem.Player.DropRightLeg.Enable();

        inputSystem.Player.DropBothLegs.performed += DropBothLegs;
        inputSystem.Player.DropBothLegs.Enable();
    }

    public void OnDisable()
    {
        inputSystem.Player.DetachPart.Disable();
        inputSystem.Player.ReattachPart.Disable();

        inputSystem.Player.ShootR.Disable();
        inputSystem.Player.ShootL.Disable();

        inputSystem.Player.RecallBothArms.Disable();
        inputSystem.Player.RecallLeftArm.Disable();
        inputSystem.Player.RecallRightArm.Disable();


        // Unbind new D-pad controls
        inputSystem.Player.DropEverything.Disable();
        inputSystem.Player.DropLeftArm.Disable();
        inputSystem.Player.DropRightArm.Disable();
        inputSystem.Player.DropLeftLeg.Disable();
        inputSystem.Player.DropRightLeg.Disable();
        inputSystem.Player.DropBothLegs.Disable();
    }


    public void On_Detach(InputAction.CallbackContext context)
    {
        if (!isDetached && Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            DetachPart(head);
            DetachPart(head2);
            DetachPart(head3);
            DetachPart(head4);
            DetachPart(head5);
            DetachPart(torso);
            DetachPart(r_Leg);
            DetachPart(l_Leg);
            DetachPart(r_Arm);
            DetachPart(l_Arm);

            playerCollider.enabled = true;
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            isDetached = true;
        }
    }

    private void DetachPart(GameObject part)
    {
        if (part == null) return;

        canRetach = true;

        // Detach while retaining world transformation
        part.transform.SetParent(null, true);

        // Ensure the part maintains its original scale
        part.transform.localScale = originalScales[part];

        // Enable BoxCollider for detached parts and adjust size to world space
        BoxCollider partCollider = part.GetComponent<BoxCollider>();
        if (partCollider != null)
        {
            Vector3 worldScale = part.transform.lossyScale;
            partCollider.size = Vector3.Scale(originalCollidersData[part].size, worldScale) * 0.25f;
            partCollider.center = Vector3.Scale(originalCollidersData[part].center, worldScale) * 0.25f;
            partCollider.enabled = true;
        }

        Rigidbody partRb = part.GetComponent<Rigidbody>();
        if (partRb != null)
        {
            partRb.isKinematic = false; // Disable physics for detachment
        }

        // Track specific parts
        if (part == l_Arm) _isL_ArmDetached = true;
        if (part == r_Arm) _isR_ArmDetached = true;

        // Handle SkinnedMeshRenderer baking
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

        // Add Rigidbody if not already present
        if (partRb == null)
        {
            partRb = part.AddComponent<Rigidbody>();
            partRb.mass = 1f;
        }
    }


    public void On_Reattach(InputAction.CallbackContext context)
    {
        if (isDetached || canRetach)
        {
            // Move the player slightly above the ground
            if(_isBothLegsDetached || _isEverythingDetached)
            {
                Vector3 groundPosition = transform.position;
                float safeHeight = playerCollider.bounds.extents.y + 0.5f; // Adjust offset as needed
                transform.position = new Vector3(groundPosition.x, groundPosition.y + safeHeight, groundPosition.z);
            }
            // Proceed with reattaching parts
            StartCoroutine(ShakeAndReattach(head));
            StartCoroutine(ShakeAndReattach(head2));
            StartCoroutine(ShakeAndReattach(head3));
            StartCoroutine(ShakeAndReattach(head4));
            StartCoroutine(ShakeAndReattach(head5));
            StartCoroutine(ShakeAndReattach(torso));
            StartCoroutine(ShakeAndReattach(r_Leg));
            StartCoroutine(ShakeAndReattach(l_Leg));
            StartCoroutine(ShakeAndReattach(r_Arm));
            StartCoroutine(ShakeAndReattach(l_Arm));

            canRetach = false;
            ResetController();
            _isL_LegDetached = false;
            _isR_LegDetached = false;
            _isBothLegsDetached = false;
            _isEverythingDetached = false;
            isDetached = false;
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            playerCollider.enabled = false;
        }
    }


    private IEnumerator ShakeAndReattach(GameObject part)
    {
        Debug.Log($"Reattaching {part.name}");
        _isL_ArmDetached = false;
        _isR_ArmDetached = false;
        // Destroy any temporary Rigidbody
        Rigidbody partRb = part.GetComponent<Rigidbody>();
        if (partRb != null)
        {
            Destroy(partRb);
        }

        // Reset parent and restore scale
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
        part.transform.localScale = originalScales[part]; // Restore original scale
        

        // Restore collider and SkinnedMeshRenderer
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

    public void ShootRightArm(InputAction.CallbackContext context)
    {
        if (!_isR_ArmDetached && Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;
            DetachPart(r_Arm);
            Rigidbody rb = r_Arm.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(transform.forward * shootingForce);  // Adjust force as needed
            }

            _isR_ArmDetached = true;
            _animator.SetTrigger("shootingR");
        }
    }

    public void ShootLeftArm(InputAction.CallbackContext context)
    {
        if (!_isL_ArmDetached && Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;
            DetachPart(l_Arm);
            Rigidbody rb = l_Arm.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(transform.forward * shootingForce);  // Adjust force as needed
            }

            _isL_ArmDetached = true;
            _animator.SetTrigger("shootingL");
        }
    }
    public void DropEverything(InputAction.CallbackContext context)
    {
        if (!isDetached && Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            // Detach all parts

            DetachPart(torso);
            DetachPart(r_Leg);
            DetachPart(l_Leg);
            DetachPart(r_Arm);
            DetachPart(l_Arm);
            CharacterController controller = GetComponent<CharacterController>();
            controller.center = new Vector3(0, 2f, 0);

            isDetached = true;
            _isEverythingDetached = true;
        }

    }

    public void DropLeftArm(InputAction.CallbackContext context)
    {
        if (!_isL_ArmDetached && Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            DetachPart(l_Arm);
            _isL_ArmDetached = true;
        }
    }

    public void DropRightArm(InputAction.CallbackContext context)
    {
        if (!_isR_ArmDetached && Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            DetachPart(r_Arm);
            _isR_ArmDetached = true;
        }
    }

    public void DropLeftLeg(InputAction.CallbackContext context)
    {
        if (Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            // Detach either leg (priority: right leg)
            if (r_Leg != null) DetachPart(r_Leg);
            else if (l_Leg != null) DetachPart(l_Leg);
            _isL_LegDetached = true;
        }
    }

    public void DropRightLeg(InputAction.CallbackContext context)
    {
        if (Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            // Detach the other leg (priority: left leg)
            if (l_Leg != null) DetachPart(l_Leg);
            else if (r_Leg != null) DetachPart(r_Leg);
            _isR_LegDetached = true;
        }
    }

    public void DropBothLegs(InputAction.CallbackContext context)
    {
        if (Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            // Detach both legs
            DetachPart(r_Leg);
            DetachPart(l_Leg);
            CharacterController controller = GetComponent<CharacterController>();
            controller.height = 0f;
            _isBothLegsDetached = true;

        }
   
    }

    public void RecallBothArms(InputAction.CallbackContext context)
    {
        Debug.Log("botharms");

        if (Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;
            
                Debug.Log("Recalling Left Arm");
                StartCoroutine(ShakeAndReattach(l_Arm));
                _isL_ArmDetached = false;
            
           
                Debug.Log("Recalling Right Arm");
                StartCoroutine(ShakeAndReattach(r_Arm));
                _isR_ArmDetached = false;
            
        }
    }


    public void RecallRightArm(InputAction.CallbackContext context)
    {
        Debug.Log("rarms");
        if (Time.time >= lastDetachTime + detachCooldown)
        {
            
                Debug.Log("Recalling Right Arm");
                lastDetachTime = Time.time;
                StartCoroutine(ShakeAndReattach(r_Arm));
                _isR_ArmDetached = false;
            
        }
    }

    public void RecallLeftArm(InputAction.CallbackContext context)
    {
        Debug.Log("larms");
        if (Time.time >= lastDetachTime + detachCooldown)
        {
            
                Debug.Log("Recalling Left Arm");
                lastDetachTime = Time.time;
                StartCoroutine(ShakeAndReattach(l_Arm));
                _isL_ArmDetached = false;
            
        }
    }


    public void ResetController()
    {
        CharacterController controller = GetComponent<CharacterController>();
        // Default settings for the full body
        controller.height = 5.61f;
        controller.center = new Vector3(0, -2.46f, 0);
        controller.radius = 1.5f;
    }


}
