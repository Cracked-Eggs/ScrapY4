using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public Image fadeScreen;
    public float fadeSpeed = 5f;
    private bool fadeToBlack, fadeFromBlack;
    public string mainMenuScene;


    void Awake()
    {
        instance = this;
        fadeScreen.gameObject.SetActive(true);
    }

    void Start()
    {
        StartFadeFromBlack();
    }

    void Update()
    {
        if (fadeFromBlack)
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b,
                Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime));

        if (fadeToBlack)
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b,
                Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
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
    
    public void GoToMainMenu()
    {
        SaveSystem.instance.DestroySaveSystem();

        SceneManager.LoadScene(mainMenuScene);
        Time.timeScale = 1f;
    }
}
