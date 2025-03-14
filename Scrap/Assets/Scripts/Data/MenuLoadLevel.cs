using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoadLevel : MonoBehaviour
{
    [SerializeField] MenuData _menuData;
    
    public void LoadLevel1()
    {
        StartCoroutine(LoadScene("Level 1"));
        _menuData.StartFadeToBlack();
        SaveSystem.instance.activeSave.currentLevel = "Level 1";
    }
    
    public void LoadLevel2()
    {
        StartCoroutine(LoadScene("Level 2"));
        _menuData.StartFadeToBlack();
        SaveSystem.instance.activeSave.currentLevel = "Level 2";
    }
    
    public void LoadLevel3()
    {
        StartCoroutine(LoadScene("Level 3"));
        _menuData.StartFadeToBlack();
        SaveSystem.instance.activeSave.currentLevel = "Level 3";
    }
    
    public void LoadLevel4()
    {
        StartCoroutine(LoadScene("Level 4"));
        _menuData.StartFadeToBlack();
        SaveSystem.instance.activeSave.currentLevel = "Level 4";
    }
    
    public void LoadLevel5()
    {
        StartCoroutine(LoadScene("Level 5"));
        _menuData.StartFadeToBlack();
        SaveSystem.instance.activeSave.currentLevel = "Level 5";
    }
    
    public void LoadLevel6()
    {
        StartCoroutine(LoadScene("Level 6"));
        _menuData.StartFadeToBlack();
        SaveSystem.instance.activeSave.currentLevel = "Level 6";
    }
    
    public void LoadLevel7()
    {
        StartCoroutine(LoadScene("Level 7"));
        _menuData.StartFadeToBlack();
        SaveSystem.instance.activeSave.currentLevel = "Level 7";
    }

    IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneName);
    }
}
