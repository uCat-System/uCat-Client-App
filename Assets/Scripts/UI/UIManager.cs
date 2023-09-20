using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using EListeningState = WitListeningStateManager.ListeningState;
using EMenuNavigationResponseType = UICommandHandler.MenuNavigationResponseType;
using EDialogueState = DialogueManager.DialogueState;
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
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public GameObject menu;

    public void ActivateMenu() {
        _witListeningStateManager.TransitionToRelevantMenuNavigationStateBasedOnLevel();
        _dialogueManager.ChangeDialogueState(EDialogueState.IsInMenu);
        StartCoroutine(StartMenuOpenAnimation());
    }

    IEnumerator StartMenuOpenAnimation() {
        if (!menu.activeInHierarchy)
        {
            textElements.SetActive(false);
            menu.SetActive(true);
            boardAnimator.SetTrigger("Open");
            yield return new WaitForSeconds(1.5f);
        }
    }
    public void DeactivateMenu() {
        StartCoroutine(StartMenuCloseAnimation());
    }


    IEnumerator StartMenuCloseAnimation() {
        boardAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(1.25f);
        menu.SetActive(false);
    }

    public void Resume() {
        // Repeat task if it was in progress, otherwise continue dialogue

        Debug.Log("Resuming, prev state was " + _dialogueManager.previousDialogueState.ToString() );

        // Set the dialogue and Wit state back to what they were before menu activation
         _dialogueManager.ChangeDialogueState(_dialogueManager.previousDialogueState);
        _witListeningStateManager.TransitionToState(EListeningState.ListeningForMenuActivationCommandsOnly);

        if (_dialogueManager.currentDialogueState == EDialogueState.IsPerformingATask) {
            Debug.Log("Resuming task");
            _wordReciteManager.enabled = true;
            _wordReciteManager.RepeatSameWord();
            textElements.SetActive(true);
        } else if (_dialogueManager.currentDialogueState == EDialogueState.IsPlayingDialogueOnly) {
            Debug.Log("Resuming dialogue");
             _dialogueManager.StartDialogue();
        }

        DeactivateMenu();
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
                Resume();
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
