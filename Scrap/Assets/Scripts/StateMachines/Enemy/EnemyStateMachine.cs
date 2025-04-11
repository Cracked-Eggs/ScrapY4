using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachine : StateMachine
{
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public CharacterController Controller { get; private set; }
    [field: SerializeField] public ForceReceiver ForceReceiver { get; private set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public WeaponDamage WeaponR { get; private set; }
    [field: SerializeField] public WeaponDamage WeaponL { get; private set; }
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public Target Target { get; private set; }
    [field: SerializeField] public PlayerStateMachine PlayerC { get; private set; }


    [field: SerializeField] public float MovementSpeed { get; private set; }
    [field: SerializeField] public float PlayerChasingRange { get; private set; }
    [field: SerializeField] public float AttackRange { get; private set; }
    [field: SerializeField] public int AttackDamage { get; private set; }
    [field: SerializeField] public int AttackKnockback { get; private set; }
    [field: SerializeField] public List<Transform> PatrolPoints { get; private set; } = new List<Transform>();
    [field: SerializeField] public float PatrolSpeed { get; private set; } = 2f;
    [field: SerializeField] public float BlockChance { get; private set; } = 0.01f;
    [field: SerializeField] public float BlockGrace { get; private set; } = 1.5f;
    [field: SerializeField] public bool CanBlock;
    [field: SerializeField] public float AttackCooldown = 3f;
    [field: SerializeField] public float LastAttackTime { get; set; } = Mathf.NegativeInfinity;
    

    public Health Player;


    void Start()
    {
        
        Agent.updatePosition = false;
        Agent.updateRotation = false;
        
        if (PatrolPoints.Count == 0)
            SwitchState(new EnemyIdleState(this));
        else
            SwitchState(new EnemyPatrolState(this));
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

    void HandleTakeDamage() => SwitchState(new EnemyImpactState(this));

    void HandleDie() => SwitchState(new EnemyDeadState(this));


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, PlayerChasingRange);
    }
}
