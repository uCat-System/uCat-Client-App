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

     public DialogueManager _dialogueManager;

     public GameObject reciteBoard;

     public WitListeningStateManager _witListeningStateManager;
    public WordReciteManager _wordReciteManager;

    private void Awake()
    {
        textElements = GameObject.FindWithTag("TextElements");
        reciteBoard = GameObject.FindWithTag("ReciteBoard");
        _levelManager = FindObjectOfType<LevelManager>();
        _dialogueManager = FindObjectOfType<DialogueManager>();
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
            yield return new WaitForSeconds(1.5f);
            _witListeningStateManager.TransitionToRelevantMenuNavigationStateBasedOnLevel();
        }
    }

    IEnumerator StartMenuCloseAnimation() {
        boardAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(1.25f);
        menu.SetActive(false);
        textElements.SetActive(true);
        // if (_dialogueManager.currentDialogueState == DialogueManager.DialogueState.IsPerformingATask) {
        // }
    }

    public void ActivateMenuNavigationCommandsBasedOnResponse(EMenuNavigationResponseType navigationCommand) {

        Debug.Log("MENU NAV COMMAND: " + navigationCommand.ToString());
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
                _witListeningStateManager.TransitionToState(EListeningState.ListeningForMenuActivationCommandsOnly);
                // string scene = SceneManager.GetActiveScene().name;
                // if (scene == "Level3") {
                //     _witListeningStateManager.TransitionToState(EListeningState.ListeningForEverything);
                // // If board is active (& we are in an exercise), reset the word
                // } else if (reciteBoard.activeInHierarchy && _wordReciteManager.isCurrentlyCountingTowardsTimeout) {
                //     Debug.Log("Board is active");
                //     _witListeningStateManager.TransitionToState(EListeningState.ListeningForMenuActivationCommandsOnly);
                //     _wordReciteManager.RepeatSameWord();
                // }
                // textElements.SetActive(true);
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
