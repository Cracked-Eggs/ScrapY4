using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
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
    public bool _isTorsoDetached = false;

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
        if (secondaryRadiusChecker.currentBodyParts < secondaryRadiusChecker.totalBodyParts)
        {
            if (Time.time < lastDetachTime + detachCooldown) return; // Prevent rapid reattachment

            lastDetachTime = Time.time;

            // Clear previous body parts before adding new ones
            secondaryRadiusChecker.targetBodyParts.Clear();
            bool bodyPartInRange = false;

            // List of body parts to check (with lambdas to update their respective detached flags)
            List<(GameObject part, Action resetFlag)> bodyParts = new()
            {
                (partManager.r_Arm, () => _isR_ArmDetached = false),
                (partManager.l_Arm, () => _isL_ArmDetached = false),
                (partManager.r_Leg, () => _isR_LegDetached = false),
                (partManager.l_Leg, () => _isL_LegDetached = false),
                (partManager.torso, () => _isTorsoDetached = false)
            };

            // Add body parts in range to the list
            foreach (var (bodyPart, resetFlag) in bodyParts)
            {
                if (IsBodyPartInSecondaryRadius(bodyPart))
                {
                    secondaryRadiusChecker.targetBodyParts.Add(bodyPart);
                    resetFlag(); // Call the lambda function to reset the flag
                    bodyPartInRange = true;
                }
            }

            // Prevent reattachment if no body parts are in range
            if (!bodyPartInRange)
            {
                Debug.Log("No body parts in range to reattach.");
                return;
            }

            // Start retraction process
            secondaryRadiusChecker.isRetracting = true;
            foreach (GameObject bodyPart in secondaryRadiusChecker.targetBodyParts)
            {
                StartCoroutine(WaitForRetractComplete(bodyPart));
            }

            // Adjust height if necessary

            StartCoroutine(SmoothRise(playerCollider.bounds.extents.y));


            // Update CharacterController if available
            if (TryGetComponent(out CharacterController characterController))
            {
                characterController.center = new Vector3(characterController.center.x, -2.46f, characterController.center.z);
                characterController.height = 5.61f;
                Debug.Log("CharacterController updated.");
            }

            // Reset states
            canRetach = false;
            _isBothLegsDetached = false;
            _isEverythingDetached = false;
            isDetached = false;

            // Restore physics constraints
            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            playerCollider.enabled = false;

            Debug.Log("Reattachment process completed.");

        }

    }
    private IEnumerator SmoothRise(float riseAmount)
    {
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.up * riseAmount;
        float duration = 0.5f; // Adjust for speed
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end; // Ensure exact final position
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
            return distance <= secondaryRadiusChecker.radius;
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
            controller.center = new Vector3(0, 0, 0);

            isDetached = true;
            _isL_ArmDetached = true;
            _isR_ArmDetached = true;
            _isL_LegDetached = true;
            _isR_LegDetached = true;
            _isTorsoDetached = true;
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
            if (secondaryRadiusChecker.currentBodyParts < secondaryRadiusChecker.totalBodyParts)
            {
                if (IsBodyPartInSecondaryRadius(partManager.l_Arm) && IsBodyPartInSecondaryRadius(partManager.r_Arm) && _isR_ArmDetached && _isL_ArmDetached)
                {
                    Debug.Log("Both arms are in range, starting retraction.");

                    // Ensure the list is cleared before adding new body parts
                    secondaryRadiusChecker.targetBodyParts.Clear();

                    // Add both arms to the list
                    secondaryRadiusChecker.targetBodyParts.Add(partManager.r_Arm);
                    secondaryRadiusChecker.targetBodyParts.Add(partManager.l_Arm);

                    secondaryRadiusChecker.isRetracting = true; // Start retraction
                    StartCoroutine(WaitForRetractComplete(partManager.r_Arm));
                    StartCoroutine(WaitForRetractComplete(partManager.l_Arm));

                    _isR_ArmDetached = false;
                    _isL_ArmDetached = false;
                }
                else
                {
                    Debug.Log("One or both arms are not in range for reattachment.");
                }
                lastDetachTime = Time.time;
            }
               

            
        }
    }


    public void RecallRightArm(InputAction.CallbackContext context)
    {
        if (secondaryRadiusChecker.currentBodyParts < secondaryRadiusChecker.totalBodyParts)
        {

            if (Time.time >= lastDetachTime + detachCooldown)
            {
                lastDetachTime = Time.time;

                // Check if the right arm is in range before reattaching
                if (secondaryRadiusChecker.isRightArmInRange && _isR_ArmDetached)
                {
                    Debug.Log("Right arm is in range, starting retraction.");

                    // Add the right arm to the secondaryRadiusChecker's targetBodyParts list
                    secondaryRadiusChecker.targetBodyParts.Clear(); // Clear previous body parts
                    secondaryRadiusChecker.targetBodyParts.Add(partManager.r_Arm);

                    // Retract the right arm
                    foreach (GameObject bodyPart in secondaryRadiusChecker.targetBodyParts)
                    {
                        secondaryRadiusChecker.isRetracting = true;  // Retract the right arm
                        StartCoroutine(WaitForRetractComplete(bodyPart));

                        _isR_ArmDetached = false;
                    }
                }
                else
                {
                    Debug.Log("Right arm is not in range for reattachment.");
                }
            }
        }
    }

    public void RecallLeftArm(InputAction.CallbackContext context)
    {
        if (secondaryRadiusChecker.currentBodyParts < secondaryRadiusChecker.totalBodyParts)
        {
            if (Time.time >= lastDetachTime + detachCooldown)
            {
                lastDetachTime = Time.time;

                // Check if the left arm is in range before reattaching
                if (secondaryRadiusChecker.isLeftArmInRange && _isL_ArmDetached)
                {
                    Debug.Log("Left arm is in range, starting retraction.");

                    // Add the left arm to the secondaryRadiusChecker's targetBodyParts list
                    secondaryRadiusChecker.targetBodyParts.Clear(); // Clear previous body parts
                    secondaryRadiusChecker.targetBodyParts.Add(partManager.l_Arm);

                    // Retract the left arm
                    foreach (GameObject bodyPart in secondaryRadiusChecker.targetBodyParts)
                    {
                        secondaryRadiusChecker.isRetracting = true;  // Retract the left arm
                        StartCoroutine(WaitForRetractComplete(bodyPart));

                        _isL_ArmDetached = false;
                    }
                }
                else
                {
                    Debug.Log("Left arm is not in range for reattachment.");
                }
            }
        }
            
    }


    private IEnumerator WaitForRetractComplete(GameObject bodyPart)
    {
        // Wait until the retraction process completes (can check for some condition, like distance)
        while (Vector3.Distance(bodyPart.transform.position, transform.position) > 2f)  // Customize distance threshold
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
                Debug.Log("Both legs are in range, starting retraction.");

                // Add both legs to the secondaryRadiusChecker's targetBodyParts list
                secondaryRadiusChecker.targetBodyParts.Clear(); // Clear previous body parts
                secondaryRadiusChecker.targetBodyParts.Add(partManager.r_Leg);
                secondaryRadiusChecker.targetBodyParts.Add(partManager.l_Leg);

                // Retract all the body parts in the list
                foreach (GameObject bodyPart in secondaryRadiusChecker.targetBodyParts)
                {
                    secondaryRadiusChecker.isRetracting = true;  // Retract each part
                    StartCoroutine(WaitForRetractComplete(bodyPart));

                    // Update the detached flags based on the body part
                    if (bodyPart == partManager.r_Leg) _isR_LegDetached = false;
                    else if (bodyPart == partManager.l_Leg) _isL_LegDetached = false;
                }

                _isBothLegsDetached = false;

                // Reset the character's height (if needed)
                CharacterController controller = GetComponent<CharacterController>();
                if (controller != null)
                {
                    controller.height = 2.0f;  // Adjust height as required
                }
            }
            else
            {
                Debug.Log("One or both legs are not in range for reattachment.");
            }
        }
    }

    public void RecallRightLeg(InputAction.CallbackContext context)
    {
        if (Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            // Check if the right leg is in range before reattaching
            if (secondaryRadiusChecker.isRightLegInRange)
            {
                Debug.Log("Right leg is in range, starting retraction.");

                // Add the right leg to the secondaryRadiusChecker's targetBodyParts list
                secondaryRadiusChecker.targetBodyParts.Clear(); // Clear previous body parts
                secondaryRadiusChecker.targetBodyParts.Add(partManager.r_Leg);

                // Retract the right leg
                foreach (GameObject bodyPart in secondaryRadiusChecker.targetBodyParts)
                {
                    secondaryRadiusChecker.isRetracting = true;  // Retract the right leg
                    StartCoroutine(WaitForRetractComplete(bodyPart));

                    _isR_LegDetached = false;
                }
            }
            else
            {
                Debug.Log("Right leg is not in range for reattachment.");
            }
        }
    }

    public void RecallLeftLeg(InputAction.CallbackContext context)
    {
        if (Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            // Check if the left leg is in range before reattaching
            if (secondaryRadiusChecker.isLeftLegInRange)
            {
                Debug.Log("Left leg is in range, starting retraction.");

                // Add the left leg to the secondaryRadiusChecker's targetBodyParts list
                secondaryRadiusChecker.targetBodyParts.Clear(); // Clear previous body parts
                secondaryRadiusChecker.targetBodyParts.Add(partManager.l_Leg);

                // Retract the left leg
                foreach (GameObject bodyPart in secondaryRadiusChecker.targetBodyParts)
                {
                    secondaryRadiusChecker.isRetracting = true;  // Retract the left leg
                    StartCoroutine(WaitForRetractComplete(bodyPart));

                    _isL_LegDetached = false;
                }
            }
            else
            {
                Debug.Log("Left leg is not in range for reattachment.");
            }
        }
    }


}
