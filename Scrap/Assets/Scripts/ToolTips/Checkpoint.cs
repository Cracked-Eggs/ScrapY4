using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RespawnManager.Instance.SetRespawnPosition(transform.position);
            Debug.Log("Checkpoint reached!");
        }
    }
}