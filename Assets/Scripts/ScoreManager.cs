using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ScoreManager : MonoBehaviour
{
    public TMPro.TextMeshPro scoreText;
    private int level1CurrentScore = 0;
    private int level2CurrentScore = 0;
    
    private int level1MaxScore = 20;
    public int level2MaxScore = 10;

    public int Level1MaxScore
    {
        get { return level1MaxScore; }
        set
        {
            level1MaxScore = value;
            Debug.Log("Just set level 1 max to " + value.ToString());

        }
    }

    public int Level1CurrentScore
    {
        get { return level1CurrentScore; }
        set
        {
                    Debug.Log("Adding score in score manager to " + level1CurrentScore.ToString());

            level1CurrentScore = value;
            UpdateScoreUIBasedOnCurrentLevel();

        }
    }

    public int Level2MaxScore
    {
        get { return level2MaxScore; }
        set
        {
            level2MaxScore = value;
        }
    }


    public int Level2CurrentScore
    {
        get { return level2CurrentScore; }
        set
        {
            level1CurrentScore = value;
            UpdateScoreUIBasedOnCurrentLevel();
        }
    }

    public void SetMaxScoreBasedOnWordListCount(int count) {
        Debug.Log("Setting max score to " + count.ToString());
        Scene scene = SceneManager.GetActiveScene();
        switch (scene.name)
        {
            case "Level1":
                Level1MaxScore = count;
                break;
            case "Level2":
                 Level2MaxScore = count;
                 break;
            default:
                break;
        }
    }


    public void ShowScoreAtEndOfLevel()
    {
        scoreText.gameObject.SetActive(true);
    }

    // Mange the score globally accross all scenes for the current player.
    // Eventually include database storage as well.
    void Start()
    {
        UpdateScoreUIBasedOnCurrentLevel();
    }

    void UpdateScoreUIBasedOnCurrentLevel()
    {
        Debug.Log("Updating score UI");
        Scene scene = SceneManager.GetActiveScene();
        switch (scene.name)
        {
            case "Level1":
                scoreText.text = "Score: " + level1CurrentScore.ToString() + " / " + Level1MaxScore.ToString();
                break;
            case "Level2":
                 scoreText.text = "Score: " + Level2CurrentScore.ToString() + "/ " + Level2MaxScore.ToString();
                 break;
            default:
                break;
        }
    }
}
