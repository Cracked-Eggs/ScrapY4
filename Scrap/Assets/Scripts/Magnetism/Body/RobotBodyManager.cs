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
    [SerializeField] public VisualEffect rightArmVFX;
    [SerializeField] public VisualEffect HeadVFX;
    [SerializeField] public VisualEffect LeftLegVFX;
    [SerializeField] public VisualEffect RightLegVFX;
    [SerializeField] public GameObject leftArmARCVFX;
    [SerializeField] public GameObject rightArmARCVFX;
    [SerializeField] public GameObject headARCVFX;
    [SerializeField] public GameObject LeftLegARCVFX;
    [SerializeField] public GameObject RightLegARCVFX;
    [SerializeField] public GameObject rightArmBurstVFX; // Assign in Inspector
    [SerializeField] public GameObject leftArmBurstVFX;


    private Vector3 resetPos1Original;
    private Vector3 resetPos2Original;


    [SerializeField] public float customGravity = -9.81f;
    [SerializeField] AudioClip magnetRepel;
    [SerializeField] public float shootingForce = 500f;
    AudioSource _audioSource;
    Animator _animator;

    public float detachLeftArmCooldown = 2.0f;
    public float detachRightArmCooldown = 2.0f;
    public float detachAllCooldown = 2.0f;
    private float lastDetachLeftArmTime = -2.0f;
    private float lastDetachRightArmTime = -2.0f;
    private float lastDetachAllTime = -2.0f;
  

    public bool isDetached = false;
    public bool _isL_ArmDetached = false;
    public bool _isR_ArmDetached = false;
    public bool _isL_LegDetached = false;
    public bool _isR_LegDetached = false;
    public bool _isTorsoDetached = false;

    public bool _isBothLegsDetached = false;
    public bool _isEverythingDetached = false;

    //public Magnet leftArmMagnetScript;
    //public Magnet rightArmMagnetScript;
    //public SphereCollider leftArmSphereColl;
    //public SphereCollider rightArmSphereColl;
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
    private IEnumerator PlayAndDisableVFX(GameObject vfx, float duration)
    {
        if (vfx != null)
        {
            vfx.SetActive(true);
            yield return new WaitForSeconds(duration);
            vfx.SetActive(false);
        }
    }

    private void Start()
    {
        //if (leftArmMagnetScript != null && _isL_ArmDetached == false) leftArmMagnetScript.enabled = false;
        //if (rightArmMagnetScript != null && _isR_ArmDetached == false) rightArmMagnetScript.enabled = false;
        leftArmVFX.Stop();
        rightArmVFX.Stop();
        RightLegVFX.Stop();
        LeftLegVFX.Stop();
        HeadVFX.Stop();
     
        rightArmARCVFX.SetActive(false);
        leftArmARCVFX.SetActive(false);
        headARCVFX.SetActive(false);
        LeftLegARCVFX.SetActive(false);
        RightLegARCVFX.SetActive(false);
     


    }
    
    public void ToggleDetachReattach(InputAction.CallbackContext context)
    {
        if (partManager.isReattaching) return;

        if (isDetached)
        {
            AttemptReattach();
        }
        else
        {
            if (CanDetach()) // Check if detaching is allowed
            {
                DetachAll();
            }
            else
            {
                Debug.Log("Cannot detach: Some parts are missing and not permanently lost.");
            }
        }
    }

    private void AttemptReattach()
    {
        if (secondaryRadiusChecker.currentBodyParts >= secondaryRadiusChecker.totalBodyParts) return;
        if (Time.time < lastDetachAllTime + detachAllCooldown) return;

        lastDetachAllTime = Time.time;

        List<(GameObject part, Action resetFlag, bool isDetached)> bodyParts = new()
    {
        (partManager.r_Arm, () => _isR_ArmDetached = false, _isR_ArmDetached),
        (partManager.l_Arm, () => _isL_ArmDetached = false, _isL_ArmDetached),
        (partManager.r_Leg, () => _isR_LegDetached = false, _isR_LegDetached),
        (partManager.l_Leg, () => _isL_LegDetached = false, _isL_LegDetached),
        (partManager.torso, () => _isTorsoDetached = false, _isTorsoDetached)
    };

        bool bodyPartInRange = false;
        secondaryRadiusChecker.targetBodyParts.Clear();

        foreach (var (bodyPart, resetFlag, isDetached) in bodyParts)
        {
            if (isDetached && IsBodyPartInSecondaryRadius(bodyPart))
            {
                secondaryRadiusChecker.targetBodyParts.Add(bodyPart);
                resetFlag();
                bodyPartInRange = true;

                // Play VFX for reattaching
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
                    HeadVFX.Play();
                    headARCVFX.SetActive(true);
                }
                else if (bodyPart.CompareTag("R_Leg"))
                {
                    RightLegVFX.Play();
                    RightLegARCVFX.SetActive(true);
                }
                else if (bodyPart.CompareTag("L_Leg"))
                {
                    LeftLegVFX.Play();
                    LeftLegARCVFX.SetActive(true);
                }
            }
        }

        if (!bodyPartInRange)
        {
            Debug.Log("No body parts in range to reattach. Canceling reattachment.");
            return; // Exit early if no parts are available
        }

        // Smooth rise and controller adjustments happen only if at least one part was reattached
        CharacterController controller = GetComponent<CharacterController>();
        controller.center = new Vector3(0, -2.46f, 0);
        controller.height = 5.61f;
        StartCoroutine(SmoothRise(playerCollider.bounds.extents.y));

        secondaryRadiusChecker.isRetracting = true;

        foreach (GameObject bodyPart in secondaryRadiusChecker.targetBodyParts)
        {
            StartCoroutine(WaitForRetractComplete(bodyPart));
        }

        //leftArmMagnetScript.enabled = !_isL_ArmDetached;
        //rightArmMagnetScript.enabled = !_isR_ArmDetached;
        //leftArmSphereColl.enabled = false;
        //rightArmSphereColl.enabled = false;

        CheckIfFullyReattached();
    }

    
    private void CheckIfFullyReattached()
    {
        isDetached = _isL_ArmDetached || _isR_ArmDetached || _isL_LegDetached || _isR_LegDetached || _isTorsoDetached;
    }

    private void DetachAll()
    {
        if (Time.time < lastDetachAllTime + detachAllCooldown) return;

        // Ensure detaching is only possible when all parts are retrieved OR if permanently lost
        if (!CanDetach())
        {
            Debug.Log("Cannot detach: Some parts are missing and not permanently lost.");
            return;
        }

        lastDetachAllTime = Time.time;

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

        //leftArmMagnetScript.enabled = true;
        //rightArmMagnetScript.enabled = true;
        //leftArmSphereColl.enabled = true;
        //rightArmSphereColl.enabled = true;
    }
    private bool CanDetach()
    {
        int missingParts = 0;
        if (_isL_ArmDetached) missingParts++;
        if (_isR_ArmDetached) missingParts++;
        if (_isL_LegDetached) missingParts++;
        if (_isR_LegDetached) missingParts++;
        if (_isTorsoDetached) missingParts++;

        // Allow detaching if no parts are missing OR if too many parts are lost
        return missingParts == 0 || missingParts >= 2;
    }
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
        if (Time.time < lastDetachLeftArmTime + detachLeftArmCooldown) return;

        lastDetachLeftArmTime = Time.time;

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
        if (Time.time < lastDetachLeftArmTime + detachLeftArmCooldown) return;

        lastDetachLeftArmTime = Time.time;

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
        StartCoroutine(PlayAndDisableVFX(rightArmBurstVFX, 0.6f));

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
            //rightArmMagnetScript.enabled = true;
            //rightArmSphereColl.enabled = true;
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
                //rightArmMagnetScript.enabled = false;
                //rightArmSphereColl.enabled = false;
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
        StartCoroutine(PlayAndDisableVFX(leftArmBurstVFX, 0.6f));
        if (!_isL_ArmDetached)
        {
            // Detach and shoot left arm
            partManager.DetachPart(partManager.l_Arm);
            Rigidbody rb = partManager.l_Arm.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log("Skipped it!");
                Vector3 repulsionDirection = transform.forward; // Push the arm forward
                rb.AddForce(repulsionDirection * shootingForce, ForceMode.Impulse);
            }

            _isL_ArmDetached = true;
            //leftArmMagnetScript.enabled = true;
            //leftArmSphereColl.enabled = true;
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
                //leftArmMagnetScript.enabled = false;
                //leftArmSphereColl.enabled = false;
            }
            else
            {
                Debug.Log("Left arm is not in range for reattachment.");
            }
        }
    }
    public void DropLeftArm(InputAction.CallbackContext context)
    {
        if (Time.time < lastDetachLeftArmTime + detachLeftArmCooldown) return;

        lastDetachLeftArmTime = Time.time;

        if (_isL_ArmDetached)
        {
            
            RecallLeftArm();
        }
        else
        {
            
            DroppingLeftArm();
        }
    }
    public void DroppingLeftArm()
    {
        if (partManager.isReattaching) return;
        if (!_isL_ArmDetached)
        {
            lastDetachLeftArmTime = Time.time;
            partManager.DetachPart(partManager.l_Arm);
            _isL_ArmDetached = true;
            //leftArmMagnetScript.enabled = true;
            //leftArmSphereColl.enabled = true;
        }
    }

    public void DropRightArm(InputAction.CallbackContext context)
    {
        if (Time.time < lastDetachRightArmTime + detachRightArmCooldown) return;

        lastDetachRightArmTime = Time.time;

        if (_isR_ArmDetached)
        {
            // The arm is out, recall it
            RecallRightArm();
        }
        else
        {
            // The arm is not out, drop it
            DroppingRightArm();
        }
    }
    public void DroppingRightArm()
    {
        if (partManager.isReattaching) return;
        if (!_isR_ArmDetached)
        {
            lastDetachRightArmTime = Time.time;
            partManager.DetachPart(partManager.r_Arm);
            _isR_ArmDetached = true;
            //rightArmMagnetScript.enabled = true;
            //rightArmSphereColl.enabled = true;
        }
    }

    public void RecallBothArms(InputAction.CallbackContext context)
    {
        if (Time.time >= lastDetachRightArmTime + detachRightArmCooldown)
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
                    //leftArmMagnetScript.enabled = false;
                    //rightArmMagnetScript.enabled = false;
                    //leftArmSphereColl.enabled = false;
                    //rightArmSphereColl.enabled = false;
                    secondaryRadiusChecker.isRightArmInRange = false;
                    secondaryRadiusChecker.isLeftArmInRange = false;
                }
                else
                {
                    Debug.Log("One or both arms are not in range for reattachment.");
                }
                lastDetachRightArmTime = Time.time;
            }



        }
    }
    private IEnumerator WaitForRetractComplete(GameObject bodyPart)
    {
        float timeout = 100f; // Adjust based on your needs
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
            HeadVFX.Play();
            headARCVFX.SetActive(true);
        }
        else if(bodyPart.CompareTag("L_Leg"))
        {
            LeftLegVFX.Play();
            LeftLegARCVFX.SetActive(true) ;
        }
        else if (bodyPart.CompareTag("R_Leg"))
        {
            RightLegVFX.Play();
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

}