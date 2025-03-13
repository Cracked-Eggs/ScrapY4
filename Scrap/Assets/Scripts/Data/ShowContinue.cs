using UnityEngine;

public class ShowContinue : MonoBehaviour
{
    void Start()
    {
        if(System.IO.File.Exists(Application.persistentDataPath + "/" + SaveSystem.instance.saveName + ".save") == false)
        {
            gameObject.SetActive(false);
        }
    }
}