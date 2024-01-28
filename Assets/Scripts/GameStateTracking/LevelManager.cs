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

    // Private field to store the current level
    private string _currentLevel;

    // Public property to access the current level (read-only)
    public string CurrentLevel
    {
        get { return _currentLevel; }
    }

    // Awake method to set the current level when the script starts
    public void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();
        _currentLevel = scene.name;
    }

    // Method to repeat the current level
    public void RepeatLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // Method to go to the next level sequentially
    public void GoToNextLevelSequentially()
    {
        // Switch scenes based on the current level
        switch (_currentLevel)
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
                SceneManager.LoadScene("ConvoMode");
                break;
            case "ConvoMode":
                break;
            default:
                break;
        }
    }
}
