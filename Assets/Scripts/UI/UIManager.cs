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

     public LevelManager _levelManager;
     public GameObject textElements;

     public WitListeningStateManager _witListeningStateManager;
    public WordReciteManager _wordReciteManager;

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
        
    }

    public void ActivateMenu() {

        if (!menu.activeInHierarchy)
        {
            textElements.SetActive(false);
            menu.SetActive(true);
            _witListeningStateManager.TransitionToRelevantMenuNavigationStateBasedOnLevel();
        }
    }

    public void ActivateMenuNavigationCommandsBasedOnResponse(EMenuNavigationResponseType navigationCommand) {
        switch (navigationCommand) {
            case EMenuNavigationResponseType.REPEAT_LEVEL_RESPONSE:
                // TODO move to levelmanager
                _levelManager.RepeatLevel();
                break;
            case EMenuNavigationResponseType.NURSE_RESPONSE:
                // TODO implement later
                // SceneManager.LoadScene("Nurse");
                break;
            case EMenuNavigationResponseType.RESTART_LEVEL_RESPONSE:
                _levelManager.RepeatLevel();
                break;
            case EMenuNavigationResponseType.RESUME_RESPONSE:
                // TODO clean this up, maybe fire an event handler / put logic in witlisteningstatemanager
                menu.SetActive(false);
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

    void Start()
    {
        menu.SetActive(false);
    }
}
