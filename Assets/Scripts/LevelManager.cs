using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public ScoreManager _scoreManager;

    public void LevelComplete()
    {
        _scoreManager.ShowScoreAtEndOfLevel();
        // Switch scenes based on score
        Scene scene = SceneManager.GetActiveScene();
        Debug.Log("Ended level " + scene.name);

        switch (scene.name)
        {
            case "Level1":
                if (_scoreManager.Level1CurrentScore == _scoreManager.Level1MaxScore)
                {
                    // Change to level 2 if 100% score
                    SceneManager.LoadScene("Level2");
                }

                else
                {
                    // Repeat Level 1
                    SceneManager.LoadScene("Level1");
                }
                break;
            case "Level2":
                    // Change to level 3, no matter the score
                    SceneManager.LoadScene("Level3");
                break;

        }

    }
}
