using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.VFX;
using System.Linq;
public class Attach : MonoBehaviour
{
    private CharacterController characterController;
    private Collider playerCollider;
    public bool canRetach;
    private VFXManager vfxManager;
    private PlayerStateMachine playerStateMachine;
    InputReader _inputReader;
    public Rigidbody rb_head;
    public SphereCollider playerCollider_head;
    public MagneticManager magneticManager;

    [SerializeField] public float customGravity = -9.81f;
    [SerializeField] AudioClip magnetRepel;
    [SerializeField] public float shootingForce = 500f;
    [SerializeField] LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] Transform debugTransform;
    
    AudioSource _audioSource;
    Animator _animator;

    public float detachLeftArmCooldown = 2.0f;
    public float detachRightArmCooldown = 2.0f;
    public float detachAllCooldown = 2.0f;
    private float lastDetachLeftArmTime = -2.0f;
    private float lastDetachRightArmTime = -2.0f;
    private float lastDetachAllTime = -2.0f;
    Vector3 currentRotation;
    Vector3 mouseWorldPosition;



    public bool isDetached = false;
    public bool _isL_ArmDetached = false;
    public bool _isR_ArmDetached = false;
    public bool _isL_LegDetached = false;
    public bool _isR_LegDetached = false;
    public bool _isTorsoDetached = false;
    public bool canShoot = true;

    public bool _isBothLegsDetached = false;
    public bool _isEverythingDetached = false;

   
    public PartManager partManager;

    // Reference to the second radius checker script
    [SerializeField] private RadiusChecker secondaryRadiusChecker;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCollider = GetComponent<Collider>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        partManager = GetComponent<PartManager>();
        vfxManager = GetComponent<VFXManager>();
        playerStateMachine = GetComponent<PlayerStateMachine>();
        rb_head = GetComponent<Rigidbody>();
        _inputReader = GetComponent<InputReader>();
    }

    private void Update()
    {
        currentRotation = transform.rotation.eulerAngles;
        
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point; // Optional: Visualize the aim point
        }
        mouseWorldPosition = raycastHit.point; 
    }
    private void Start()
    {
      vfxManager.StopAllVFX();
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
    public void AttemptReattach()
    {
        if (secondaryRadiusChecker.currentBodyParts >= secondaryRadiusChecker.totalBodyParts) return;
        if (Time.time < lastDetachAllTime + detachAllCooldown) return;

        lastDetachAllTime = Time.time;
        _animator.enabled = true;

        characterController.enabled = true;
        Vector3 initialEulerAngles = playerStateMachine.initalRotation.eulerAngles;

        transform.rotation = Quaternion.Euler(initialEulerAngles.x, currentRotation.y,initialEulerAngles.z);
      
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
        canShoot = true; // Allow shooting again

        foreach (var (bodyPart, resetFlag, isDetached) in bodyParts)
        {
            if (isDetached && IsBodyPartInSecondaryRadius(bodyPart))
            {
                secondaryRadiusChecker.targetBodyParts.Add(bodyPart);
                resetFlag();
                bodyPartInRange = true;

                // Play VFX for reattaching
               
            }
        }

        if (!bodyPartInRange)
        {
            Debug.Log("No body parts in range to reattach. Canceling reattachment.");
            return; // Exit early if no parts are available
        }

        // Adjust character controller only if reattachment happened
        if (TryGetComponent<CharacterController>(out CharacterController controller))
        {
            controller.center = new Vector3(0, -2.46f, 0);
            controller.height = 5.61f;
            StartCoroutine(SmoothRise(1f));
        }

        if(TryGetComponent<SphereCollider>(out SphereCollider sphereCollider))
        {
            sphereCollider.enabled = false;
        }

        secondaryRadiusChecker.isRetracting = true;

        foreach (GameObject bodyPart in secondaryRadiusChecker.targetBodyParts)
        {
            StartCoroutine(WaitForRetractComplete(bodyPart));
        }

        // Finally, switch back to normal movement state
        playerStateMachine.SwitchState(new PlayerFreeLookState(playerStateMachine));
        if(TryGetComponent<Rigidbody>(out rb_head))
        {
            rb_head.isKinematic = true;
        }

        CheckIfFullyReattached();
    }

    private void CheckIfFullyReattached()
    {
        isDetached = _isL_ArmDetached || _isR_ArmDetached || _isL_LegDetached || _isR_LegDetached || _isTorsoDetached;
    }
    public void DetachAll()
    {
        if (Time.time < lastDetachAllTime + detachAllCooldown) return;
        if (partManager.isReattaching) return;

        // Ensure detaching is only possible when all parts are retrieved OR if permanently lost
        if (!CanDetach())
        {
            Debug.Log("Cannot detach: Some parts are missing and not permanently lost.");
            return;
        }

        lastDetachAllTime = Time.time;
      
        playerStateMachine.HandleLoseBody();
        characterController.enabled = false;
        _animator.enabled = false;

        partManager.DetachPart(partManager.torso);
        partManager.DetachPart(partManager.r_Leg);
        partManager.DetachPart(partManager.l_Leg);
        partManager.DetachPart(partManager.r_Arm);
        partManager.DetachPart(partManager.l_Arm);

        CharacterController controller = GetComponent<CharacterController>();
        controller.center = new Vector3(0, 0.77f, 0);
        controller.height = 0f;
        if (TryGetComponent<SphereCollider>(out SphereCollider sphereCollider))
        {
            sphereCollider.enabled = true;
        }
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
        canShoot = false;
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
        if(canShoot)
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
        
    }
    public void ShootOrRecallLeftArm(InputAction.CallbackContext context)
    {
        if (canShoot) {
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
       
    }

    IEnumerator MovePartToTarget(GameObject part, Vector3 targetPosition, float speed)
    {
        Rigidbody rb = part.GetComponent<Rigidbody>();

        // Ensure Rigidbody is disabled for smooth movement
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Disable physics while moving
        }

        while (Vector3.Distance(part.transform.position, targetPosition) > 0.1f)
        {
            part.transform.position = Vector3.MoveTowards(part.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        // Reactivate Rigidbody after reaching the target
        if (rb) rb.isKinematic = false;
    }

    public void ShootRightArm()
    {
        if (partManager.isReattaching) return;
        if (_inputReader.IsAiming == false) return;
        if (_isR_ArmDetached) return; // Prevent double shooting

        vfxManager.PlayBurstVFX("R_Arm");

        partManager.DetachPart(partManager.r_Arm);

        StopAllCoroutines(); // Stop any ongoing movement
        StartCoroutine(MovePartToTarget(partManager.r_Arm, mouseWorldPosition, shootingForce));

        _isR_ArmDetached = true;
    }

    public void RecallRightArm()
    {
        if (!_isR_ArmDetached) return;

        if (secondaryRadiusChecker.isRightArmInRange)
        {
            secondaryRadiusChecker.targetBodyParts.Add(partManager.r_Arm);
            secondaryRadiusChecker.isRetracting = true;
            StartCoroutine(WaitForRetractComplete(partManager.r_Arm));
            _isR_ArmDetached = false;
        }
        else
        {
            Debug.Log("Right arm is not in range for reattachment.");
        }
    }

    public void ShootLeftArm()
    {
        if (partManager.isReattaching) return;
        if (_inputReader.IsAiming == false) return;
        if (_isL_ArmDetached) return; // Prevent double shooting

        vfxManager.PlayBurstVFX("L_Arm");

        partManager.DetachPart(partManager.l_Arm);

        StopAllCoroutines(); // Stop any ongoing movement
        StartCoroutine(MovePartToTarget(partManager.l_Arm, mouseWorldPosition, shootingForce));

        _isL_ArmDetached = true;
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
        if (!canShoot) return;
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
        if (!canShoot) return;
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
            vfxManager.PlayVFX("L_Arm");
           
        }
        else if (bodyPart.CompareTag("R_Arm"))
        {
            vfxManager.PlayVFX("R_Arm");
            
        }
        else if(bodyPart.CompareTag("Torso"))
        {
            vfxManager.PlayVFX("Torso");
        }
        else if(bodyPart.CompareTag("L_Leg"))
        {
            vfxManager.PlayVFX("L_Leg");
        }
        else if (bodyPart.CompareTag("R_Leg"))
        {
            vfxManager.PlayVFX("R_Leg");
        }


        while (Vector3.Distance(bodyPart.transform.position, transform.position) > secondaryRadiusChecker.secondaryRadius)
        {
            if (Time.time - startTime > timeout)
            {
                Debug.LogWarning(bodyPart.name + " retraction timed out.");
                yield break;
            }
            yield return null;
        }

        yield return StartCoroutine(partManager.ShakeAndReattach(bodyPart));

        Debug.Log(bodyPart.name + " has been reattached.");
        

        // Remove the body part from targetBodyParts after successful reattachment
        if (secondaryRadiusChecker.targetBodyParts.Contains(bodyPart))
        {
            secondaryRadiusChecker.targetBodyParts.Remove(bodyPart);
            Debug.Log(bodyPart.name + " removed from targetBodyParts.");
        }
    }
    public void ActivateGrappleAndReattach(InputAction.CallbackContext context)
    {
        if (partManager.isReattaching) return;
        if (!magneticManager.canGrapple) return;

        GameObject targetArm = null;

        // Check which arm is currently detached and assign the target arm accordingly
        if (_isR_ArmDetached)
        {
            targetArm = partManager.r_Arm;
        }
        else if (_isL_ArmDetached)
        {
            targetArm = partManager.l_Arm;
        }
        else
        {
            Debug.Log("No arm is currently detached.");
            return; // Exit if no arm is detached
        }

        // Get the position of the arm that's activating the grapple
        Vector3 armPosition = targetArm.transform.position;
        Debug.Log("Target arm position: " + armPosition);

        // Call the method to drop all body parts
        DetachAllG();

        // Attempt to reattach everything to the position of the arm
        AttemptReattachToPosition(armPosition);
    }

    public void AttemptReattachToPosition(Vector3 targetPosition)
    {
        // First, teleport the player to the arm's position
        TeleportPlayerToArmPosition(targetPosition);

        // Ensure the cooldowns allow reattachment
        if (Time.time >= lastDetachLeftArmTime + detachLeftArmCooldown && Time.time >= lastDetachRightArmTime + detachRightArmCooldown)
        {
            StartCoroutine(HandleReattachProcess());
        }
        else
        {
            Debug.Log("Cooldown active, reattachment not allowed yet.");
        }

        lastDetachLeftArmTime = Time.time;
        lastDetachRightArmTime = Time.time;
    }

    private IEnumerator HandleReattachProcess()
    {
        // Wait for the teleportation to finish (you could adjust this depending on your teleport duration)
        yield return new WaitForSeconds(1.0f); // Assuming 1 second for teleportation

        // Now that the player has arrived at the arm's position, attempt reattachment
        AttemptReattachG();
    }
    private void TeleportPlayerToArmPosition(Vector3 targetPosition, float teleportDuration = 0.8f)
    {
        StartCoroutine(LerpTeleport(targetPosition, teleportDuration));
    }

    private IEnumerator LerpTeleport(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is exactly the target position
        transform.position = targetPosition;

        // Optionally, rotate the player to face the direction of the arm
        transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);
    }

    public void AttemptReattachG()
    {
        if (secondaryRadiusChecker.currentBodyParts >= secondaryRadiusChecker.totalBodyParts) return;
        

        lastDetachAllTime = Time.time;
        _animator.enabled = true;

        characterController.enabled = true;
        Vector3 initialEulerAngles = playerStateMachine.initalRotation.eulerAngles;
        transform.rotation = Quaternion.Euler(initialEulerAngles.x, currentRotation.y, initialEulerAngles.z);

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
        canShoot = true; // Allow shooting again

        foreach (var (bodyPart, resetFlag, isDetached) in bodyParts)
        {
            if (isDetached && IsBodyPartInSecondaryRadius(bodyPart))
            {
                secondaryRadiusChecker.targetBodyParts.Add(bodyPart);
                resetFlag();
                bodyPartInRange = true;

                // Optionally, play VFX for reattaching
            }
        }

        if (!bodyPartInRange)
        {
            Debug.Log("No body parts in range to reattach. Canceling reattachment.");
            return; // Exit early if no parts are available
        }

        // Adjust character controller only if reattachment happened
        if (TryGetComponent<CharacterController>(out CharacterController controller))
        {
            controller.center = new Vector3(0, -2.46f, 0);
            controller.height = 5.61f;
            StartCoroutine(SmoothRise(1f));
        }

        if (TryGetComponent<SphereCollider>(out SphereCollider sphereCollider))
        {
            sphereCollider.enabled = false;
        }

        secondaryRadiusChecker.isRetracting = true;

        foreach (GameObject bodyPart in secondaryRadiusChecker.targetBodyParts)
        {
            StartCoroutine(WaitForRetractComplete(bodyPart));
        }

        // Finally, switch back to normal movement state
        playerStateMachine.SwitchState(new PlayerFreeLookState(playerStateMachine));
        if (TryGetComponent<Rigidbody>(out Rigidbody rb_head))
        {
            rb_head.isKinematic = true;
        }

        CheckIfFullyReattached();
    }
    public void DetachAllG()
    {
        if (Time.time < lastDetachAllTime + detachAllCooldown) return;
        if (partManager.isReattaching) return;

        lastDetachAllTime = Time.time;

        playerStateMachine.HandleLoseBody();
        characterController.enabled = false;
        _animator.enabled = false;

        partManager.DetachPart(partManager.torso);
        partManager.DetachPart(partManager.r_Leg);
        partManager.DetachPart(partManager.l_Leg);
        partManager.DetachPart(partManager.r_Arm);
        partManager.DetachPart(partManager.l_Arm);

        CharacterController controller = GetComponent<CharacterController>();
        controller.center = new Vector3(0, 0.77f, 0);
        controller.height = 0f;
        if (TryGetComponent<SphereCollider>(out SphereCollider sphereCollider))
        {
            sphereCollider.enabled = true;
        }
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
        canShoot = false;
    }



}