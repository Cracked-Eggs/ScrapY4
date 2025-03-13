using UnityEngine;
using System.IO;
using System.Xml.Serialization;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;
    public SaveData activeSave;
    public string sceneToNotSave;
    public string saveName;
    public bool dontSave;
    
    void Awake() => SetupInstance();

    public void SetupInstance()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);

            //Load();
        }
        else if(instance != this)
            Destroy(gameObject);
    }

    public void Save()
    {
#if UNITY_EDITOR
        if (dontSave == false)
        {
#endif
            Debug.Log("Saving Data");

            string dataPath = Application.persistentDataPath;

            var serializer = new XmlSerializer(typeof(SaveData));
            var stream = new FileStream(dataPath + "/" + saveName + ".save", FileMode.Create);
            serializer.Serialize(stream, activeSave);
            stream.Close();
#if UNITY_EDITOR
        }
#endif
    }

    public void Load()
    {
        string dataPath = Application.persistentDataPath;

        if (File.Exists(dataPath + "/" + saveName + ".save"))
        {
            Debug.Log("Loading Data");

            var serializer = new XmlSerializer(typeof(SaveData));
            var stream = new FileStream(dataPath + "/" + saveName + ".save", FileMode.Open);
            activeSave = serializer.Deserialize(stream) as SaveData;
            stream.Close();
        }
        else
        {
            Debug.LogWarning("Couldn't find data to load!");
        }
    }

    public void DestroySaveSystem()
    {
        instance = null;
        Destroy(gameObject);
    }
}