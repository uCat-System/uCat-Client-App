using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int CurrentLevel;
    public ScoreManager _scoreManager;

    public void LevelComplete()
    {
        Debug.Log("Ended level " + CurrentLevel);
        _scoreManager.ShowScoreAtEndOfLevel();
        // Switch scenes based on score

        switch (CurrentLevel)
        {
            case 1:
                if (_scoreManager.Level1CurrentScore == _scoreManager.Level1MaxScore)
                {
                    // Change to level 2
                    SceneManager.LoadScene("Level2");
                }

                else
                {
                    // Repeat Level 1
                    SceneManager.LoadScene("Level1");
                }
                break;
            case 2:
                //
                break;
            case 3:
                //
                break;

        }

        CurrentLevel++;
    }
    // Start is called before the first frame update
    void Start()
    {
        CurrentLevel = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
