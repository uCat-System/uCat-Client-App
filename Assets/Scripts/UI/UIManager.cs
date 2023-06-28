using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using EListeningState = WitListeningStateManager.ListeningState;
using EMenuNavigationResponseType = UICommandHandler.MenuNavigationResponseType;
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
        // TODO move to UIHandler class
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
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public GameObject menu;

    public void CheckIfMenuActivationCommandsWereSpoken(string text) {
        // TODO move the below to external UIHandler class

        // Listen for any of the wake phrases
        if (!menu.activeInHierarchy && acceptableWakeWords.Any(text.Contains))
        {
            textElements.SetActive(false);
            menu.SetActive(true);
            _witListeningStateManager.TransitionToState(EListeningState.ListeningForTaskMenuCommandsOnly);
        }
    }

    void ActivateMenuNavigationCommandsBasedOnResponse(EMenuNavigationResponseType navigationCommand) {

        //  NavigationInputData navigationInputData = Resources.Load<NavigationInputData>("NavigationInputData");
        // menuNavigationActions = new Dictionary<string, MenuNavigationResponseType>
        // {
        //     { navigationInputData.repeatLevelInput, MenuNavigationResponseType.REPEAT_LEVEL_RESPONSE },
        //     { navigationInputData.nextLevelInput, MenuNavigationResponseType.NEXT_LEVEL_RESPONSE },
        //     { navigationInputData.nurseInput, MenuNavigationResponseType.NURSE_RESPONSE },
        //     { navigationInputData.restartLevelInput, MenuNavigationResponseType.RESTART_LEVEL_RESPONSE },
        //     { navigationInputData.resumeInput, MenuNavigationResponseType.RESUME_RESPONSE },
        //     { navigationInputData.reciteWordsInput, MenuNavigationResponseType.RECITE_WORDS_RESPONSE },
        //     { navigationInputData.reciteSentencesInput, MenuNavigationResponseType.RECITE_SENTENCES_RESPONSE },
        //     { navigationInputData.reciteOpenQuestionsInput, MenuNavigationResponseType.RECITE_OPEN_QUESTIONS_RESPONSE },
        //     { navigationInputData.lobbyInput, MenuNavigationResponseType.LOBBY_RESPONSE }
        // };
        switch (navigationCommand) {
            case EMenuNavigationResponseType.REPEAT_LEVEL_RESPONSE:
                // TODO move to levelmanager
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            case EMenuNavigationResponseType.NEXT_LEVEL_RESPONSE:
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
            case EMenuNavigationResponseType.NURSE_RESPONSE:
                // TODO move to levelmanager
                SceneManager.LoadScene("Nurse");
                break;
            case EMenuNavigationResponseType.RESTART_LEVEL_RESPONSE:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            case EMenuNavigationResponseType.RESUME_RESPONSE:
                animator.Play("CloseClip");
                StartCoroutine(WaitForAnimationToEnd());
                string scene = SceneManager.GetActiveScene().name;
                if (scene == "Level3") {
                    _witListeningStateManager.TransitionToState(EListeningState.ListeningForEverything);
                } else {
                    _witListeningStateManager.TransitionToState(EListeningState.ListeningForMenuActivationCommandsOnly);
                    _wordReciteManager.RepeatSameWord();
                }
                textElements.SetActive(true);
                break;
            case EMenuNavigationResponseType.RECITE_WORDS_RESPONSE:
                SceneManager.LoadScene("Level1");
                break;
            case EMenuNavigationResponseType.RECITE_SENTENCES_RESPONSE:
                SceneManager.LoadScene("Level2");
                break;
            case EMenuNavigationResponseType.RECITE_OPEN_QUESTIONS_RESPONSE:
                SceneManager.LoadScene("Level3");
                break;
            case EMenuNavigationResponseType.LOBBY_RESPONSE:
                SceneManager.LoadScene("Lobby");
                break;
            default:
                break;
        }
    }

    public void CheckIfMenuNavigationCommandsWereSpoken(string text) {

            Debug.Log("Menu is active and listening for navigation commands only: " + text);
            EMenuNavigationResponseType navigationCommand = UICommandHandler.CheckIfMenuNavigationCommandsWereSpoken(text);
            ActivateMenuNavigationCommandsBasedOnResponse(navigationCommand);
            // StartCoroutine(ProceedBasedOnConfirmation(confirmation, originallyUtteredText));

            switch (text)
            {
                case "repeat level":
                    // TODO move to levelmanager
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
                        _witListeningStateManager.TransitionToState(EListeningState.ListeningForEverything);
                    } else {
                        _witListeningStateManager.TransitionToState(EListeningState.ListeningForMenuActivationCommandsOnly);
                        _wordReciteManager.RepeatSameWord();
                    }
                    textElements.SetActive(true);
                    break;
                case ("level one"):
                    SceneManager.LoadScene("Level1"); 
                    break;
                case "level two":
                    SceneManager.LoadScene("Level2"); 
                    break;
                case "level three":
                    SceneManager.LoadScene("Level3");
                    break;
                default:
                    break;
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
