using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] UnityEvent triggerEvent;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            triggerEvent.Invoke();
    }
}