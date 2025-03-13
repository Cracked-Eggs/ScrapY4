using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;
    public int level;

    void Awake() => instance = this;
    
    void Start()
    {
        SaveData theSave = SaveSystem.instance.activeSave;
        level = theSave.level;
    }
}
