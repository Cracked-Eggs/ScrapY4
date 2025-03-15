using UnityEngine;

public class VentTrigger : MonoBehaviour
{
    public Attach attach;

    void Start() { 
        Attach attach = GetComponent<Attach>();
    }// Flag to track if player is inside

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Head")) // Make sure your player has the "Player" tag
        {
            attach.inVent = true;
            Debug.Log("Player entered the vent");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Head"))
        {
            attach.inVent = false;
            Debug.Log("Player exited the vent");
        }
    }
}
