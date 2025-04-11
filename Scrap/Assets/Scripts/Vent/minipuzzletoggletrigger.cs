using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minipuzzletoggletrigger : MonoBehaviour
{
    public GameObject trigger;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            {
            trigger.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            trigger.SetActive(false);
        }
    }
}
