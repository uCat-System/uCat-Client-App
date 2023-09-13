using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public GameObject crossFadeImage;

    // Update is called once per frame

    void Start() 
    {
        crossFadeImage.SetActive(true);
    }
    public void BeginLevelTransition()
    {
        StartCoroutine(TransitionToNextLevel());
    }

    public void BeginSpecificLevelTransition(string sceneName)
    {
        StartCoroutine(TransitionToSpecificLevel(sceneName));
    }

    public IEnumerator TransitionToNextLevel()
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        // Go to next scene using LevelManager

        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.GoToNextLevelSequentially();
        }
        else
        {
            Debug.LogError("LevelManager not found.");
        }
    }

    public IEnumerator TransitionToSpecificLevel(string sceneName)
    {
                        Debug.Log("Sentences scene: " + sceneName);

        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
}
