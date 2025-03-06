using UnityEngine;

public class MagneticDamage : MonoBehaviour
{
    public float damage = 10f;
    public float magneticRadius = 5f; 
    public LayerMask enemyLayer;
    public Attach attach;


    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0 && attach.magneticHit)
        {
            if (other.TryGetComponent<Health>(out Health enemyHealth))
            {
                enemyHealth.DealDamage((int)damage, true);
                attach.magneticHit = false;
                attach.RecallRightArm();
            }
        }
    }
}