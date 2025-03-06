using UnityEngine;

public class MagneticDamage : MonoBehaviour
{
    public float damage = 10f;
    public float magneticRadius = 5f; 
    public LayerMask enemyLayer;
    public Attach attach;
    public bool RArm;


    void OnTriggerEnter(Collider other)
    {
            if (other.TryGetComponent<Health>(out Health enemyHealth))
            {
                enemyHealth.DealDamage((int)damage, true);
                attach.magneticHit = false;
                if (RArm)
                    attach.RecallRightArm();
                else
                    attach.RecallLeftArm();
        }
    }
}