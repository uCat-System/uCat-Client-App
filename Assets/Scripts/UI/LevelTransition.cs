using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    public GameObject crossFadeImage;

    private static LevelTransition instance;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This method is called whenever a new scene is loaded.
        // You can put your logic here to handle the changes needed when the scene changes.
        Debug.Log("OnSceneLoaded: " + scene.name);
    }

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
        DialogueHandler.ResetDialogueIndex();
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        // Go to the next scene using LevelManager

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
        Debug.Log("Transitioning to " + sceneName);
        DialogueHandler.ResetDialogueIndex();

        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
}
