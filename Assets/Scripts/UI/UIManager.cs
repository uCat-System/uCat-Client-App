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

    private LevelManager _levelManager;
    private Animator menuBoardAnimator;
    private DialogueManager _dialogueManager;

    private GameObject reciteBoard;

    private WitListeningStateManager _witListeningStateManager;
    private WordReciteManager _wordReciteManager;

    public GameObject menu;

    private void Awake()
    {
        _wordReciteManager = FindObjectOfType<WordReciteManager>();
        _witListeningStateManager = FindObjectOfType<WitListeningStateManager>();
        reciteBoard = GameObject.FindWithTag("ReciteBoard");
        _levelManager = FindObjectOfType<LevelManager>();
        _dialogueManager = FindObjectOfType<DialogueManager>();
        menuBoardAnimator = menu.GetComponent<Animator>();
        menu.SetActive(false);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ActivateMenu() {
        _witListeningStateManager.TransitionToRelevantMenuNavigationStateBasedOnLevel();
        _dialogueManager.ChangeDialogueState(EDialogueState.IsInMenu);
        StartCoroutine(StartMenuOpenAnimation());
    }

    IEnumerator StartMenuOpenAnimation() {
        if (!menu.activeInHierarchy)
        {
            GameObject.FindWithTag("ReciteText3D").GetComponent<MeshRenderer>().enabled = false; 
            reciteBoard.SetActive(false);
            menu.SetActive(true);
            menuBoardAnimator.SetTrigger("Open");
            yield return new WaitForSeconds(1.5f);
        }
    }
    public void DeactivateMenu() {
        StartCoroutine(StartMenuCloseAnimation());
    }


    IEnumerator StartMenuCloseAnimation() {
        menuBoardAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(1.25f);
        menu.SetActive(false);
    }

    public void Resume() {
        // Repeat task if it was in progress, otherwise continue dialogue

        Debug.Log("Resuming, prev state was " + _dialogueManager.previousDialogueState.ToString() );

        // Set the dialogue and Wit state back to what they were before menu activation
         _dialogueManager.ChangeDialogueState(_dialogueManager.previousDialogueState);
        _witListeningStateManager.TransitionToState(EListeningState.ListeningForMenuActivationCommandsOnly);
        GameObject.FindWithTag("ReciteText3D").GetComponent<MeshRenderer>().enabled = true; 

        if (_dialogueManager.currentDialogueState == EDialogueState.IsPerformingATask) {
            Debug.Log("Resuming task");
            reciteBoard.SetActive(true);
            _wordReciteManager.enabled = true;
            _wordReciteManager.RepeatSameWord();
        } else if (_dialogueManager.currentDialogueState == EDialogueState.IsPlayingDialogueOnly) {
            Debug.Log("Resuming dialogue and activating board");
             _dialogueManager.StartDialogue();
            if (DialogueHandler.currentDialogueOptionIndex >= _dialogueManager.boardActivationDialogueIndex) {
                 reciteBoard.SetActive(true);
            }
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
}
