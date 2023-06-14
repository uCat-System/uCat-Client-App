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
     public GameObject textElements;

     public WitListeningStateManager _witListeningStateManager;
    public WordReciteManager _wordReciteManager;

     List<string> acceptableWakeWords = new List<string>()
    {
        "menu",
        "activate menu",
        "hey cat",
        "hey kat",
        "hey cap",
        "hey you cap",
        "hey you can't",
        "hey you cat",
        "hey you kat",
        "hey, you cat",
        "hey, you kat"
    };

    private void Awake()
    {
        textElements = GameObject.FindWithTag("TextElements");
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
    Debug.Log("Checking for UI commands: " + text);
        // Listen for any of the wake phrases
        if (!menu.activeInHierarchy && acceptableWakeWords.Any(text.Contains))
        {
            textElements.SetActive(false);
            // _wordReciteManager.StopAllCoroutines();
            menu.SetActive(true);
            _witListeningStateManager.ChangeState("ListeningForTaskMenuCommandsOnly");
        }

        if (menu.activeInHierarchy && _witListeningStateManager.currentListeningState == "ListeningForTaskMenuCommandsOnly")
        {
            Debug.Log("Menu is active and listening for navigation commands only: " + text);
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
                    string scene = SceneManager.GetActiveScene().name;
                    if (scene == "Level3") {
                        _witListeningStateManager.ChangeState("ListeningForEverything");
                    } else {
                        _witListeningStateManager.ChangeState("ListeningForMenuActivationCommandsOnly");
                        _wordReciteManager.RepeatSameWord();
                    }
                    textElements.SetActive(true);
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
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }

        menu.SetActive(false);
    }

    void Start()
    {
        menu.SetActive(false);
    }
}
