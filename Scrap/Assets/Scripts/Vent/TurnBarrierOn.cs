using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnBarrierOn : MonoBehaviour
{
    public GameObject Barrier;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Barrier.SetActive(true);
        }
    }
}
