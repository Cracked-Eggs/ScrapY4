using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoadLevel : MonoBehaviour
{
    [SerializeField] private MenuData _menuData;

    public void LoadLevel(string levelName)
    {
        StartCoroutine(LoadScene(levelName));
        _menuData.StartFadeToBlack();
        SaveSystem.instance.activeSave.currentLevel = levelName;
    }

    private IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneName);
    }
}