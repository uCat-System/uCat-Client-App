using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TMPro.TextMeshPro scoreText;
    private int level1CurrentScore = 0;
    public int Level1CurrentScore
    {
        get { return level1CurrentScore; }
        set
        {
            Debug.Log("Adding, word recity " + value);

            level1CurrentScore = value;
            UpdateScoreBasedOnCurrentLevel();
        }
    }

    public void ShowScoreAtEndOfLevel()
    {
        scoreText.gameObject.SetActive(true);
    }

    // int Level2CurrentScore;
    //int Level3CurrentScore;

    public int Level1MaxScore = 6;


    // Mange the score globally accross all scenes for the current player.
    // Eventually include database storage as well.
    void Start()
    {
        UpdateScoreBasedOnCurrentLevel();
    }

    void UpdateScoreBasedOnCurrentLevel()
    {
        switch (LevelManager.CurrentLevel)
        {
            case 1:
                scoreText.text = "Score: " + level1CurrentScore.ToString() + " / " + Level1MaxScore.ToString();
                break;
            //case 2:
            //     scoreText.text = "Score: " + Level2CurrentScore.ToString() + "/ " + Level2MaxScore.ToString();
            //     break;
            //case 3:
            //     scoreText.text = "Score: " + Level1CurrentScore.ToString() + "/ " + Level1MaxScore.ToString();
            //     break;

            default:
                break;
        }
    }

    void Update()
    {
        
    }
}
