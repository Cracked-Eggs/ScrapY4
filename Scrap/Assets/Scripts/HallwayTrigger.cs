using UnityEngine;

public class HallwayTrigger : MonoBehaviour
{
    public GameObject trigger;
    private void OnTriggerEnter(Collider other)
    {
        trigger.SetActive(true);
    }
}
