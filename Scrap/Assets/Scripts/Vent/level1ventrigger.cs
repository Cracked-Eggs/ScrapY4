using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class level1ventrigger : MonoBehaviour
{
    public BoxCollider vent;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            vent.enabled = true;
        }
    }
}
