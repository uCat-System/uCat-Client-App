using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MText;



public class ScoreManager : MonoBehaviour
{
    public Modular3DText partialText3D;
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
        }
    }

    public int Level1CurrentScore
    {
        get { return level1CurrentScore; }
        set
        {
            level1CurrentScore = value;

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
            level2CurrentScore = value;
        }
    }

    public void SetMaxScoreBasedOnWordListCount(int count) {
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

    // Mange the score globally accross all scenes for the current player.
    // Eventually include database storage as well.
    void Start()
    {
        partialText3D = GameObject.FindWithTag("PartialText3D").GetComponent<Modular3DText>();
    }

    public void DisplayScoreInPartialTextSection()
    {
        Scene scene = SceneManager.GetActiveScene();
        switch (scene.name)
        {
            case "Level1":
                partialText3D.UpdateText("Level 1 Score: " + level1CurrentScore.ToString() + " / " + Level1MaxScore.ToString());
                break;
            case "Level2":
                partialText3D.UpdateText("Level 2 Score: " + level2CurrentScore.ToString() + " / " + Level2MaxScore.ToString());

                 break;
            default:
                break;
        }
    }
}
