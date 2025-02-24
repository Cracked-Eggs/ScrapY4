using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;
    Vector3 respawnPosition;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        respawnPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

    // Set the respawn position
    public void SetRespawnPosition(Vector3 newPosition) => respawnPosition = newPosition;

    public void RespawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = respawnPosition;
            Debug.Log("Player respawned at checkpoint.");
        }
    }
}
