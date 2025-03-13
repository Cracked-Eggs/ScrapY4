using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public string nextLevel;
    bool isEnding;

    void Awake() => instance = this;
    
    public void LeaveLevel()
    {
        if (isEnding == false)
        {
            isEnding = true;
            StartCoroutine(LeaveLevelCo());
        }
    }
    
    IEnumerator LeaveLevelCo()
    {

        if (nextLevel != SaveSystem.instance.sceneToNotSave)
        {
            UpdateSaveSystem();

            SaveSystem.instance.Save();
        }

        yield return new WaitForSeconds(.5f);

        /*if (nextLevel != "")
        {
            SceneManager.LoadScene(nextLevel);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }*/
    }
    
    void UpdateSaveSystem()
    {
        SaveSystem.instance.activeSave.currentLevel = nextLevel;
    }
}
