using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class miniroomventtrigger : MonoBehaviour
{
    public GameObject trigger;

    private void OnTriggerEnter(Collider other)
    {
        trigger.SetActive(true);
    }
}
