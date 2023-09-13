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
     public Animator boardAnimator;
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

        StartCoroutine(StartMenuOpenAnimation());
    }

    public void DeactivateMenu() {
        StartCoroutine(StartMenuCloseAnimation());
    }

    IEnumerator StartMenuOpenAnimation() {
        if (!menu.activeInHierarchy)
        {
            textElements.SetActive(false);
            menu.SetActive(true);
            boardAnimator.SetTrigger("Open");
            yield return new WaitForSeconds(1);
            _witListeningStateManager.TransitionToRelevantMenuNavigationStateBasedOnLevel();
        }
    }

    IEnumerator StartMenuCloseAnimation() {
        boardAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(1);
        menu.SetActive(false);
        textElements.SetActive(true);
    }

    public void ActivateMenuNavigationCommandsBasedOnResponse(EMenuNavigationResponseType navigationCommand) {

        LevelTransition _levelTransition = FindObjectOfType<LevelTransition>();

        switch (navigationCommand) {
            case EMenuNavigationResponseType.NURSE_RESPONSE:
                // TODO implement later
                // SceneManager.LoadScene("Nurse");
                break;
            case EMenuNavigationResponseType.RESTART_LEVEL_RESPONSE:
                _levelManager.RepeatLevel();
                break;
            case EMenuNavigationResponseType.RESUME_RESPONSE:
                // TODO clean this up, maybe fire an event handler / put logic in witlisteningstatemanager
                DeactivateMenu();
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
                _levelTransition.BeginSpecificLevelTransition("Level1");
                break;
            case EMenuNavigationResponseType.RECITE_SENTENCES_RESPONSE:
                Debug.Log("Sentences called");
                _levelTransition.BeginSpecificLevelTransition("Level2");
                break;
            case EMenuNavigationResponseType.RECITE_OPEN_QUESTIONS_RESPONSE:
                _levelTransition.BeginSpecificLevelTransition("Level3");
                break;
            case EMenuNavigationResponseType.LOBBY_RESPONSE:
                _levelTransition.BeginSpecificLevelTransition("Lobby");
                break;
            case EMenuNavigationResponseType.WRITING_RESPONSE:
                Debug.Log("Writing not implemented yet");
                break;
            case EMenuNavigationResponseType.CONVERSATION_RESPONSE:
                Debug.Log("Conversations not implemented yet");
                break;
            case EMenuNavigationResponseType.SETTINGS_RESPONSE:
                Debug.Log("Settings not implemented yet");
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
