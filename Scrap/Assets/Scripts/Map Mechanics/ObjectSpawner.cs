using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject barrelPrefab; // Barrel prefab to spawn
    public Transform spawnPoint; // The exact location to spawn the barrel
    public Transform spawnPoint2; // The exact location to spawn the barrel
    private bool hasSpawned = false; // Prevents multiple spawns at the same position
    private int barrels = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasSpawned && barrels < 2)
        {
            SpawnBarrel();
            barrels++;
            hasSpawned = true; // Prevents multiple spawns until platform moves away
        }
        else
        {
            hasSpawned = false; // Reset spawn flag when platform moves away
        }
    }

    void SpawnBarrel()
    {
        if (barrels >= 1)
        {
            Debug.Log("spawn bruh");
            Instantiate(barrelPrefab, spawnPoint2.position, Quaternion.identity);
        }
        else 
            Instantiate(barrelPrefab, spawnPoint.position, Quaternion.identity);
        
    }
}
