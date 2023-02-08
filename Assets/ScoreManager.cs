using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TMPro.TextMeshPro scoreText;
    private int level1CurrentScore = 0;
    private int level2CurrentScore = 0;
    
    public int Level1MaxScore = 6;
    public int Level2MaxScore = 10;

    public int Level1CurrentScore
    {
        get { return level1CurrentScore; }
        set
        {
            level1CurrentScore = value;
            UpdateScoreBasedOnCurrentLevel();
        }
    }


    public int Level2CurrentScore
    {
        get { return level2CurrentScore; }
        set
        {
            level1CurrentScore = value;
            UpdateScoreBasedOnCurrentLevel();
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
        UpdateScoreBasedOnCurrentLevel();
    }

    void UpdateScoreBasedOnCurrentLevel()
    {
        switch (LevelManager.CurrentLevel)
        {
            case 1:
                scoreText.text = "Score: " + level1CurrentScore.ToString() + " / " + Level1MaxScore.ToString();
                break;
            case 2:
                 scoreText.text = "Score: " + Level2CurrentScore.ToString() + "/ " + Level2MaxScore.ToString();
                 break;
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
