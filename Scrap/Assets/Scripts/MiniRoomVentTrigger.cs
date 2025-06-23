using UnityEngine;

public class MiniRoomVentTrigger : MonoBehaviour
{
    public GameObject trigger;

    private void OnTriggerEnter(Collider other)
    {
        trigger.SetActive(true);
    }
}
