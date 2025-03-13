using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuData : MonoBehaviour
{
    public string firstLevel;
    public Image fadeScreen;
    public float fadeSpeed = 5f;

    bool fadeToBlack, fadeFromBlack;

    void Start() => StartFadeFromBlack();

    void Update()
    {
        if (fadeFromBlack)
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime));

        if (fadeToBlack)
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
    }

    public void StartFadeToBlack()
    {
        fadeToBlack = true;
        fadeFromBlack = false;
    }

    public void StartFadeFromBlack()
    {
        fadeToBlack = false;
        fadeFromBlack = true;
    }

    public void StartGame() => StartCoroutine(StartCo());

    IEnumerator StartCo()
    {
        StartFadeToBlack();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(firstLevel);
    }

    public void Continue() => StartCoroutine(ContinueCo());

    IEnumerator ContinueCo()
    {
        SaveSystem.instance.Load();

        StartFadeToBlack();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SaveSystem.instance.activeSave.currentLevel);
    }
}