using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public static LoadLevel instance;
    [SerializeField] Animator transitionAim;

    public void NextLevel()
    {
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        transitionAim.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        transitionAim.SetTrigger("Start");
    }
}
