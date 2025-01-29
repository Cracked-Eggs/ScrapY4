using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
public class Attach : MonoBehaviour
{
    private Rigidbody _rb;
    private Collider playerCollider;
    public bool canRetach;

    [SerializeField] public float customGravity = -9.81f;
    [SerializeField] AudioClip magnetRepel;
    [SerializeField] public float shootingForce = 500f;
    AudioSource _audioSource;
    Animator _animator;

    public float detachCooldown = 2.0f;
    private float lastDetachTime = -2.0f;

    public bool isDetached = false;
    public bool _isL_ArmDetached = false;
    public bool _isR_ArmDetached = false;
    public bool _isL_LegDetached = false;
    public bool _isR_LegDetached = false;

    public bool _isBothLegsDetached = false;
    public bool _isEverythingDetached = false;

    public PartManager partManager;

    // Reference to the second radius checker script
    [SerializeField] private RadiusChecker secondaryRadiusChecker;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        partManager = GetComponent<PartManager>();

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

    public void On_Detach(InputAction.CallbackContext context)
    {
        if (!isDetached && Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            partManager.DetachPart(partManager.head);
            partManager.DetachPart(partManager.head2);
            partManager.DetachPart(partManager.head3);
            partManager.DetachPart(partManager.head4);
            partManager.DetachPart(partManager.head5);
            partManager.DetachPart(partManager.torso);
            partManager.DetachPart(partManager.r_Leg);
            partManager.DetachPart(partManager.l_Leg);
            partManager.DetachPart(partManager.r_Arm);
            partManager.DetachPart(partManager.l_Arm);

            playerCollider.enabled = true;
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            isDetached = true;
        }
    }

    public void On_Reattach(InputAction.CallbackContext context)
    {
        if (isDetached || canRetach)
        {
            bool bodyPartInRange = false;

            // Check if any of the body parts are within the secondary radius
            if (IsBodyPartInSecondaryRadius(partManager.head) ||
                IsBodyPartInSecondaryRadius(partManager.head2) ||
                IsBodyPartInSecondaryRadius(partManager.head3) ||
                IsBodyPartInSecondaryRadius(partManager.head4) ||
                IsBodyPartInSecondaryRadius(partManager.head5) ||
                IsBodyPartInSecondaryRadius(partManager.torso) ||
                IsBodyPartInSecondaryRadius(partManager.r_Leg) ||
                IsBodyPartInSecondaryRadius(partManager.l_Leg) ||
                IsBodyPartInSecondaryRadius(partManager.r_Arm) ||
                IsBodyPartInSecondaryRadius(partManager.l_Arm))
            {
                bodyPartInRange = true;
            }

            // If no body part is within the secondary radius, prevent reattachment
            if (!bodyPartInRange)
            {
                Debug.Log("No body parts in range to reattach.");
                return; // Prevent reattachment if no body parts are in range
            }

            if (_isBothLegsDetached || _isEverythingDetached)
            {
                Vector3 groundPosition = transform.position;
                float safeHeight = playerCollider.bounds.extents.y + 0.5f;
                transform.position = new Vector3(groundPosition.x, groundPosition.y + safeHeight, groundPosition.z);
            }

            // Reattach body parts here if they're in range
            StartCoroutine(partManager.ShakeAndReattach(partManager.head));
            StartCoroutine(partManager.ShakeAndReattach(partManager.head2));
            StartCoroutine(partManager.ShakeAndReattach(partManager.head3));
            StartCoroutine(partManager.ShakeAndReattach(partManager.head4));
            StartCoroutine(partManager.ShakeAndReattach(partManager.head5));
            StartCoroutine(partManager.ShakeAndReattach(partManager.torso));
            StartCoroutine(partManager.ShakeAndReattach(partManager.r_Leg));
            StartCoroutine(partManager.ShakeAndReattach(partManager.l_Leg));
            StartCoroutine(partManager.ShakeAndReattach(partManager.r_Arm));
            StartCoroutine(partManager.ShakeAndReattach(partManager.l_Arm));

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

    public bool IsBodyPartDetached(GameObject bodyPart)
    {
        if (bodyPart == partManager.r_Arm) return _isR_ArmDetached;
        if (bodyPart == partManager.l_Arm) return _isL_ArmDetached;
        if (bodyPart == partManager.r_Leg) return _isR_LegDetached;
        if (bodyPart == partManager.l_Leg) return _isL_LegDetached;
        if (bodyPart == partManager.torso) return false; // Assume torso is always attached
        if (bodyPart == partManager.head) return false; // Assume head is always attached

        // Default case: body part is considered not detached
        return false;
    }

    
   
    private bool IsBodyPartInSecondaryRadius(GameObject bodyPart)
    {
        if (secondaryRadiusChecker != null && bodyPart != null)
        {
            // Check if the body part is inside the secondary radius
            float distance = Vector3.Distance(secondaryRadiusChecker.transform.position, bodyPart.transform.position);
            return distance <= secondaryRadiusChecker.secondaryRadius;
        }
        return false;
    }

    public void ShootRightArm(InputAction.CallbackContext context)
    {
        if (!_isR_ArmDetached && Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;
            partManager.DetachPart(partManager.r_Arm);
            Rigidbody rb = partManager.r_Arm.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(transform.forward * shootingForce);
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
            partManager.DetachPart(partManager.l_Arm);
            Rigidbody rb = partManager.l_Arm.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(transform.forward * shootingForce);
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

            partManager.DetachPart(partManager.torso);
            partManager.DetachPart(partManager.r_Leg);
            partManager.DetachPart(partManager.l_Leg);
            partManager.DetachPart(partManager.r_Arm);
            partManager.DetachPart(partManager.l_Arm);
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
            partManager.DetachPart(partManager.l_Arm);
            _isL_ArmDetached = true;
        }
    }

    public void DropRightArm(InputAction.CallbackContext context)
    {
        if (!_isR_ArmDetached && Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;
            partManager.DetachPart(partManager.r_Arm);
            _isR_ArmDetached = true;
        }
    }

    public void DropLeftLeg(InputAction.CallbackContext context)
    {
        if (Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;
            if (partManager.r_Leg != null) partManager.DetachPart(partManager.r_Leg);
            else if (partManager.l_Leg != null) partManager.DetachPart(partManager.l_Leg);
            _isL_LegDetached = true;
        }
    }

    public void DropRightLeg(InputAction.CallbackContext context)
    {
        if (Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;
            if (partManager.l_Leg != null) partManager.DetachPart(partManager.l_Leg);
            else if (partManager.r_Leg != null) partManager.DetachPart(partManager.r_Leg);
            _isR_LegDetached = true;
        }
    }

    public void DropBothLegs(InputAction.CallbackContext context)
    {
        if (Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;
            partManager.DetachPart(partManager.r_Leg);
            partManager.DetachPart(partManager.l_Leg);
            CharacterController controller = GetComponent<CharacterController>();
            controller.height = 0f;
            _isBothLegsDetached = true;
        }
    }

    public void RecallBothArms(InputAction.CallbackContext context)
    {
        if (Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            // Check if both arms are in range before reattaching
            if (IsBodyPartInSecondaryRadius(partManager.l_Arm) && IsBodyPartInSecondaryRadius(partManager.r_Arm))
            {
                StartCoroutine(partManager.ShakeAndReattach(partManager.l_Arm));
                _isL_ArmDetached = false;

                StartCoroutine(partManager.ShakeAndReattach(partManager.r_Arm));
                _isR_ArmDetached = false;
            }
            else
            {
                Debug.Log("One or both arms are not in range for reattachment.");
            }
        }
    }

    public void RecallRightArm(InputAction.CallbackContext context)
    {
        if (Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            // Check if the right arm is in range before reattaching
            if (secondaryRadiusChecker.isRightArmInRange)  // Check if within the main range
            {
                Debug.Log("Right arm in range, starting retraction.");

                // Assign the target and start retraction
                secondaryRadiusChecker.targetBodyPart = partManager.r_Arm;
                secondaryRadiusChecker.isRetracting = true;
                _isR_ArmDetached = false;
                

                StartCoroutine(WaitForRetractComplete(partManager.r_Arm));
                
            }
            else
            {
                Debug.Log("Right arm is not in range for reattachment.");
            }
        }
    }

    public void RecallLeftArm(InputAction.CallbackContext context)
    {
        if (Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            // Check if the left arm is in range before reattaching
            if (secondaryRadiusChecker.isLeftArmInRange)  // Check if within the main range
            {
                Debug.Log("Left arm in range, starting retraction.");

                // Assign the target and start retraction
                secondaryRadiusChecker.targetBodyPart = partManager.l_Arm;
                secondaryRadiusChecker.isRetracting = true;
                _isL_ArmDetached = false;

                StartCoroutine(WaitForRetractComplete(partManager.l_Arm));
                
            }
            else
            {
                Debug.Log("Left arm is not in range for reattachment.");
            }
        }
    }
    private IEnumerator WaitForRetractComplete(GameObject bodyPart)
    {
        // Wait until the retraction process completes (can check for some condition, like distance)
        while (Vector3.Distance(bodyPart.transform.position, transform.position) > 1f)  // Customize distance threshold
        {
            yield return null;  // Wait one frame
        }

        // Once the body part is close enough, reattach it
        StartCoroutine(partManager.ShakeAndReattach(bodyPart));
        Debug.Log(bodyPart.name + " has been reattached.");
    }
    public void RecallBothLegs(InputAction.CallbackContext context)
    {
        if (Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            // Check if both legs are in range before reattaching
            if (IsBodyPartInSecondaryRadius(partManager.r_Leg) && IsBodyPartInSecondaryRadius(partManager.l_Leg))
            {
                StartCoroutine(partManager.ShakeAndReattach(partManager.r_Leg));
                _isR_LegDetached = false;

                StartCoroutine(partManager.ShakeAndReattach(partManager.l_Leg));
                _isL_LegDetached = false;
                _isBothLegsDetached = false;

                CharacterController controller = GetComponent<CharacterController>();
                controller.height = 2.0f;
            }
            else
            {
                Debug.Log("One or both legs are not in range for reattachment.");
            }
        }
    }
    private void ResetController()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.center = Vector3.zero;
            controller.height = 2.0f;
        }
    }
}
