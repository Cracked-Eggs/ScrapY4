using System;
using System.Collections;
using System.Collections.Generic;
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
        //UIController.instance.StartFadeToBlack();

        if (nextLevel != SaveSystem.instance.sceneToNotSave)
        {
            UpdateSaveSystem();

            SaveSystem.instance.Save();
        }

        yield return new WaitForSeconds(.5f);

        if (nextLevel != "")
        {
            SceneManager.LoadScene(nextLevel);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    
    void UpdateSaveSystem()
    {
        PlayerStats stats = PlayerStats.instance;

        SaveSystem.instance.activeSave.level = stats.level;
        SaveSystem.instance.activeSave.currentLevel = nextLevel;
    }
}
