using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.VFX;
using System.Linq;
public class Attach : MonoBehaviour
{
    private Rigidbody _rb;
    private Collider playerCollider;
    public bool canRetach;
    [SerializeField] public VisualEffect leftArmVFX;
    [SerializeField] public GameObject leftArmARCVFX;
    [SerializeField] public VisualEffect rightArmVFX;
    [SerializeField] public GameObject rightArmARCVFX;
    [SerializeField] public GameObject headARCVFX;
    [SerializeField] public GameObject LeftLegARCVFX;
    [SerializeField] public GameObject RightLegARCVFX;

    
    private Vector3 resetPos1Original;
    private Vector3 resetPos2Original;





    [SerializeField] public float customGravity = -9.81f;
    [SerializeField] AudioClip magnetRepel;
    [SerializeField] public float shootingForce = 500f;
    AudioSource _audioSource;
    Animator _animator;

    public float detachCooldown = 2.0f;
    private float lastDetachTime = -2.0f;
    private float lastActionTime = 0f;
    private float actionCooldown = 0.5f;

    public bool isDetached = false;
    public bool _isL_ArmDetached = false;
    public bool _isR_ArmDetached = false;
    public bool _isL_LegDetached = false;
    public bool _isR_LegDetached = false;
    public bool _isTorsoDetached = false;

    public bool _isBothLegsDetached = false;
    public bool _isEverythingDetached = false;

    public Magnet leftArmMagnetScript;
    public Magnet rightArmMagnetScript;
    public SphereCollider leftArmSphereColl;
    public SphereCollider rightArmSphereColl;
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
    private void Start()
    {
        if (leftArmMagnetScript != null && _isL_ArmDetached == false) leftArmMagnetScript.enabled = false;
        if (rightArmMagnetScript != null && _isR_ArmDetached == false) rightArmMagnetScript.enabled = false;
        leftArmVFX.Stop();
        rightArmVFX.Stop();
     
        rightArmARCVFX.SetActive(false);
        leftArmARCVFX.SetActive(false);
        headARCVFX.SetActive(false);
        LeftLegARCVFX.SetActive(false);
        RightLegARCVFX.SetActive(false);
     


    }

    public void On_Detach(InputAction.CallbackContext context)
    {   
        
        if (!isDetached && Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            // Detach the body parts
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

            // Disable the Magnet scripts for the arms when they are detached
            leftArmMagnetScript.enabled = true;
            rightArmMagnetScript.enabled = true;
            leftArmSphereColl.enabled = true;
            rightArmSphereColl.enabled = true;
    

            playerCollider.enabled = true;
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            isDetached = true;
        }
    }
    public void On_Reattach(InputAction.CallbackContext context)
    {
        if (secondaryRadiusChecker.currentBodyParts < secondaryRadiusChecker.totalBodyParts)
        {
            if (Time.time < lastDetachTime + detachCooldown) return;

            lastDetachTime = Time.time;
            CharacterController controller = GetComponent<CharacterController>();
            controller.center = new Vector3(0, -2.46f, 0);
            controller.height = 5.61f;

            // Clear previous body parts before adding new ones
            secondaryRadiusChecker.targetBodyParts.Clear();
            bool bodyPartInRange = false;

            // List of body parts, checking individually if detached
            List<(GameObject part, Action resetFlag, bool isDetached)> bodyParts = new()
        {
            (partManager.r_Arm, () => _isR_ArmDetached = false, _isR_ArmDetached),
            (partManager.l_Arm, () => _isL_ArmDetached = false, _isL_ArmDetached),
            (partManager.r_Leg, () => _isR_LegDetached = false, _isR_LegDetached),
            (partManager.l_Leg, () => _isL_LegDetached = false, _isL_LegDetached),
            (partManager.torso, () => _isTorsoDetached = false, _isTorsoDetached)
        };

            // Add body parts to the list if they are in range and detached
            foreach (var (bodyPart, resetFlag, isDetached) in bodyParts)
            {
                if (isDetached && IsBodyPartInSecondaryRadius(bodyPart))
                {
                    secondaryRadiusChecker.targetBodyParts.Add(bodyPart);
                    resetFlag();
                    bodyPartInRange = true;

                    // Play VFX for each detached part being retracted
                    if (bodyPart.CompareTag("R_Arm"))
                    {
                        rightArmVFX.Play();
                        rightArmARCVFX.SetActive(true);
                    }
                    else if (bodyPart.CompareTag("L_Arm"))
                    {
                        leftArmVFX.Play();
                        leftArmARCVFX.SetActive(true);
                    }
                    else if (bodyPart.CompareTag("Torso"))
                    {
                        headARCVFX.SetActive(true);
                    }
                    else if (bodyPart.CompareTag("R_Leg"))
                    {
                        RightLegARCVFX.SetActive(true);
                    }
                    else if (bodyPart.CompareTag("L_Leg"))
                    {
                        LeftLegARCVFX.SetActive(true);
                    }
                }
            }

            if (!bodyPartInRange)
            {
                Debug.Log("No body parts in range to reattach.");
                return;
            }

            secondaryRadiusChecker.isRetracting = true;

            // Iterate through all target body parts and retract them
            foreach (GameObject bodyPart in secondaryRadiusChecker.targetBodyParts)
            {
                StartCoroutine(WaitForRetractComplete(bodyPart)); // Ensure retraction completes for each part
            }


            StartCoroutine(SmoothRise(playerCollider.bounds.extents.y));



            // Disable Magnet scripts and colliders for reattached arms
            if (leftArmMagnetScript != null && !_isL_ArmDetached) leftArmMagnetScript.enabled = false;
            if (rightArmMagnetScript != null && !_isR_ArmDetached) rightArmMagnetScript.enabled = false;
            leftArmSphereColl.enabled = false;
            rightArmSphereColl.enabled = false;

            // Reset other states
            canRetach = false;
            _isBothLegsDetached = false;
            _isEverythingDetached = false;
            isDetached = false;

            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            playerCollider.enabled = false;
           

            Debug.Log("Reattachment process completed.");
        }
    }

    // New Coroutine that waits until all parts are reattached before calling SmoothRise
  
       
   
    private IEnumerator SmoothRise(float riseAmount)
    {
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.up * riseAmount;
        float duration = 0.05f; // Adjust for speed
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
    public void ShootOrRecallRightArm(InputAction.CallbackContext context)
    {
        if (Time.time < lastDetachTime + detachCooldown) return;

        lastDetachTime = Time.time;

        if (_isR_ArmDetached)
        {
            // The arm is out, recall it
            RecallRightArm();
        }
        else
        {
            // The arm is not out, shoot it
            ShootRightArm();
        }
    }

    public void ShootOrRecallLeftArm(InputAction.CallbackContext context)
    {
        if (Time.time < lastDetachTime + detachCooldown) return;

        lastDetachTime = Time.time;

        if (_isL_ArmDetached)
        {
            // The arm is out, recall it
            RecallLeftArm();
        }
        else
        {
            // The arm is not out, shoot it
            ShootLeftArm();
        }
    }

    public void ShootRightArm()
    {
        if (partManager.isReattaching) return;

        if (!_isR_ArmDetached)
        {

            // Detach and shoot right arm
            partManager.DetachPart(partManager.r_Arm);
            Rigidbody rb = partManager.r_Arm.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 repulsionDirection = transform.forward; // Push the arm forward
                rb.AddForce(repulsionDirection * shootingForce, ForceMode.Impulse);
            }

            _isR_ArmDetached = true;
            rightArmMagnetScript.enabled = true;
            rightArmSphereColl.enabled = true;
        }
    }

    public void RecallRightArm()
    {
        if (_isR_ArmDetached)
        {
            // Handle retraction of right arm
            if (secondaryRadiusChecker.isRightArmInRange)
            {
                secondaryRadiusChecker.targetBodyParts.Add(partManager.r_Arm);
                secondaryRadiusChecker.isRetracting = true;
                StartCoroutine(WaitForRetractComplete(partManager.r_Arm));
                _isR_ArmDetached = false;
                rightArmMagnetScript.enabled = false;
                rightArmSphereColl.enabled = false;
            }
            else
            {
                Debug.Log("Right arm is not in range for reattachment.");
            }
        }
    }

    public void ShootLeftArm()
    {
        if (partManager.isReattaching) return;
        if (!_isL_ArmDetached)
        {
            // Detach and shoot left arm
            partManager.DetachPart(partManager.l_Arm);
            Rigidbody rb = partManager.l_Arm.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 repulsionDirection = transform.forward; // Push the arm forward
                rb.AddForce(repulsionDirection * shootingForce, ForceMode.Impulse);
            }

            _isL_ArmDetached = true;
            leftArmMagnetScript.enabled = true;
            leftArmSphereColl.enabled = true;
        }
    }

    public void RecallLeftArm()
    {
        if(partManager.isReattaching) return;
        if (_isL_ArmDetached)
        {
            // Handle retraction of left arm
            if (secondaryRadiusChecker.isLeftArmInRange)
            {
                secondaryRadiusChecker.targetBodyParts.Add(partManager.l_Arm);
                secondaryRadiusChecker.isRetracting = true;
                StartCoroutine(WaitForRetractComplete(partManager.l_Arm));
                _isL_ArmDetached = false;
                leftArmMagnetScript.enabled = false;
                leftArmSphereColl.enabled = false;
            }
            else
            {
                Debug.Log("Left arm is not in range for reattachment.");
            }
        }
    }

    public void DropEverything(InputAction.CallbackContext context)
    {
        if(partManager.isReattaching) return;
        if (!isDetached && Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;

            partManager.DetachPart(partManager.torso);
            partManager.DetachPart(partManager.r_Leg);
            partManager.DetachPart(partManager.l_Leg);
            partManager.DetachPart(partManager.r_Arm);
            partManager.DetachPart(partManager.l_Arm);
            CharacterController controller = GetComponent<CharacterController>();
            controller.center = new Vector3(0, 0.77f, 0);
            controller.height = 0f;
            isDetached = true;
            _isL_ArmDetached = true;
            _isR_ArmDetached = true;
            _isL_LegDetached = true;
            _isR_LegDetached = true;
            _isTorsoDetached = true;
            leftArmMagnetScript.enabled = true;
            rightArmMagnetScript.enabled = true;
            leftArmSphereColl.enabled = true;
            rightArmSphereColl.enabled = true;
          
        }
    }
    public void DropLeftArm(InputAction.CallbackContext context)
    {
        if (!_isL_ArmDetached && Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;
            partManager.DetachPart(partManager.l_Arm);
            _isL_ArmDetached = true;
            leftArmMagnetScript.enabled = true;
            leftArmSphereColl.enabled = true;
           

        }
    }
    public void DropRightArm(InputAction.CallbackContext context)
    {
        if (!_isR_ArmDetached && Time.time >= lastDetachTime + detachCooldown)
        {
            lastDetachTime = Time.time;
            partManager.DetachPart(partManager.r_Arm);
            _isR_ArmDetached = true;

            rightArmMagnetScript.enabled = true;

            rightArmSphereColl.enabled = true;
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
                    leftArmMagnetScript.enabled = false;
                    rightArmMagnetScript.enabled = false;
                    leftArmSphereColl.enabled = false;
                    rightArmSphereColl.enabled = false;
                    secondaryRadiusChecker.isRightArmInRange = false;
                    secondaryRadiusChecker.isLeftArmInRange = false;
                }
                else
                {
                    Debug.Log("One or both arms are not in range for reattachment.");
                }
                lastDetachTime = Time.time;
            }



        }
    }
    private IEnumerator WaitForRetractComplete(GameObject bodyPart)
    {
        float timeout = 10f; // Adjust based on your needs
        float startTime = Time.time;
        if (bodyPart.CompareTag("L_Arm"))
        {
            
            leftArmVFX.Play();
            leftArmARCVFX.SetActive(true);
           
            
        }
        else if (bodyPart.CompareTag("R_Arm"))
        {
            rightArmVFX.Play();
            rightArmARCVFX.SetActive(true);
            
        }
        else if(bodyPart.CompareTag("Torso"))
        {
         
            headARCVFX.SetActive(true);
           

        }
        else if(bodyPart.CompareTag("L_Leg"))
        {
            LeftLegARCVFX.SetActive(true) ;
        }
        else if (bodyPart.CompareTag("R_Leg"))
        {
            RightLegARCVFX.SetActive(true);
        }


        while (Vector3.Distance(bodyPart.transform.position, transform.position) > 2f)
        {
            if (Time.time - startTime > timeout)
            {
                Debug.LogWarning(bodyPart.name + " retraction timed out.");
                yield break;
            }
            yield return null;
        }

        // Ensure we complete the reattachment process
        yield return StartCoroutine(partManager.ShakeAndReattach(bodyPart));

        Debug.Log(bodyPart.name + " has been reattached.");

        // Remove the body part from targetBodyParts after successful reattachment
        if (secondaryRadiusChecker.targetBodyParts.Contains(bodyPart))
        {
            secondaryRadiusChecker.targetBodyParts.Remove(bodyPart);
            Debug.Log(bodyPart.name + " removed from targetBodyParts.");
        }
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