using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStateMachine : StateMachine
{
    [field: SerializeField] public InputReader InputReader { get; private set; }
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public Targeter Targeter { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public WeaponDamage Weapon { get; private set; }
    [field: SerializeField] public WeaponDamage Weapon2 { get; private set; }
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public float FreeLookMovementSpeed { get; private set; }
    [field: SerializeField] public float TargetingMovementSpeed { get; private set; }
    [field: SerializeField] public float RotationDamping { get; private set; }
    [field: SerializeField] public float DodgeDuration { get; private set; }
    [field: SerializeField] public float DodgeLength { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public CinemachineInputProvider FreeLookInput { get; private set; }
    [field: SerializeField] public GameObject PauseMenu { get; private set; }
    [field: SerializeField] public Attack[] Attacks { get; private set; }

    public float PreviousDodgeTime { get; private set; } = Mathf.NegativeInfinity;
    public Transform MainCameraTransform { get; private set; }
    public bool Resume;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        MainCameraTransform = Camera.main.transform;

        SwitchState(new PlayerFreeLookState(this));
    }
    
    void OnEnable()
    {
        Health.OnTakeDamage += HandleTakeDamage;
        Health.OnDie += HandleDie;
    }

    void OnDisable()
    {
        Health.OnTakeDamage -= HandleTakeDamage;
        Health.OnDie -= HandleDie;
    }

    void HandleTakeDamage() => SwitchState(new PlayerImpactState(this));

    void HandleDie() => SwitchState(new PlayerDeadState(this));

    public void Unpause() => Resume = true;
}
