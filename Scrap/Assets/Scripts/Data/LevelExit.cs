using UnityEngine;

public class LevelExit : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            LevelManager.instance.LeaveLevel();
    }
}
