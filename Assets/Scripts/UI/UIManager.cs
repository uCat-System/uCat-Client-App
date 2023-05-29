using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
     private static UIManager instance;

     public Animator animator;

     List<string> acceptableWakeWords = new List<string>()
    {
        "activate menu",
        "hey you cat",
        "hey you kat",
        "hey, you cat",
        "hey, you kat"
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public GameObject menu;

    public void CheckIfUICommandsWereSpoken(string text) {
        // text is input as lower case from FreeSpeechManager
        Debug.Log("Listening for menu commands: " + text);

        // Listen for any of the wake phrases
        if (!menu.activeInHierarchy && acceptableWakeWords.Any(text.Contains))
        {
            menu.SetActive(true);
        }

        if (menu.activeInHierarchy)
        {
            switch (text)
            {
                case "repeat level":
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    break;
                case "next level":
                int nextBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
                    if (nextBuildIndex < SceneManager.sceneCountInBuildSettings)
                    {
                        SceneManager.LoadScene(nextBuildIndex);
                    }
                    else
                    {
                        // Handle failure case when there is no next scene
                        Debug.Log("No next scene available.");
                    }
                    break;
                case "quit":
                    Application.Quit();
                    break;
                case "restart":
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    break;
                case "resume": 
                    animator.Play("CloseClip");
                    StartCoroutine(WaitForAnimationToEnd());
                    break;
                case ("level one"):
                    SceneManager.LoadScene("Level1"); // Replace "Level1" with the name of your scene
                    break;
                case "level two":
                    SceneManager.LoadScene("Level2"); // Replace "Level2" with the name of your scene
                    break;
                case "level three":
                    SceneManager.LoadScene("Level3"); // Replace "Level3" with the name of your scene
                    break;
                default:
                    break;
            }
        }
        
        
    }

     private System.Collections.IEnumerator WaitForAnimationToEnd()
    {
        Debug.Log("Waiting for animation to end");
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            Debug.Log("Animation playing");
            yield return null;
        }
        Debug.Log("Animation ended");
        menu.SetActive(false);
    }

    public void ActivateMenu() 
    {
        menu.SetActive(true);
    }
    void Start()
    {
        menu.SetActive(false);
    }
}
