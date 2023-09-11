using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    IEnumerator TransitionToNextLevel()
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        // Go to next scene using LevelManager

        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.LevelComplete();
        }
        else
        {
            Debug.LogError("LevelManager not found.");
        }
    }
}
