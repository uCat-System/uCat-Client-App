using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{

    /* 
        <Summary>
        * This class is responsible for tracking the current level and switching between levels.
        * Intro level : User repeats the word "Hello"
        * Level 1: User repeats single words in a list, as well as UI words.
        * Level 2: User repeats sentences in a list, as well as UI sentences.
        * Level 3: User answers open questions in whichever format they like.
        </Summary>
    */


    public ScoreManager _scoreManager;

    // TODO set this to be read-only (private)
    public string currentLevel;

    public void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();
        currentLevel = scene.name;
    }

    public void RepeatLevel() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LevelComplete()
    {
        // Switch scenes based on score
        Scene scene = SceneManager.GetActiveScene();
        switch (scene.name)
        {
            case "Intro":
                SceneManager.LoadScene("Level1");
                break; 
            case "Level1":
                SceneManager.LoadScene("Level2");
                break;
            case "Level2":
                SceneManager.LoadScene("Level3");
                break;

            case "Level3":
                SceneManager.LoadScene("Lobby");
                break;

        }

    }
}
