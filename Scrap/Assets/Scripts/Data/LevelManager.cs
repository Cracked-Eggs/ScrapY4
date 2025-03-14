using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    void Awake() => instance = this;

    public void LeaveLevel()
    {
        StartCoroutine(LeaveLevelCo());
    }

    IEnumerator LeaveLevelCo()
    {
        UpdateSaveSystem();

        SaveSystem.instance.Save();

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
        if (SaveSystem.instance.activeSave.currentLevel == "")
            SaveSystem.instance.activeSave.currentLevel = "Level 2";
        else if (SaveSystem.instance.activeSave.currentLevel == "Level 2")
            SaveSystem.instance.activeSave.currentLevel = "Level 3";
    }
}