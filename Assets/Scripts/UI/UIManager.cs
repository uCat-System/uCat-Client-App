using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MText;
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

    private GameObject textElements;

    private GameObject freeStyleTextElements;
    private Modular3DText subtitleText3D;

    private WitListeningStateManager _witListeningStateManager;
    private WordReciteManager _wordReciteManager;

    public GameObject menu;

    private void Awake()
    {
        subtitleText3D = GameObject.FindWithTag("SubtitleText3D").GetComponent<Modular3DText>();
        textElements = GameObject.FindWithTag("TextElements");
        freeStyleTextElements = GameObject.FindWithTag("FreestyleTextElements");
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

    void Start() {
        SelectReciteBoardBasedOnLevel();
    }

    void SelectReciteBoardBasedOnLevel() {
         if (_levelManager.currentLevel == "Level3")
            {
                textElements.SetActive(false);
                freeStyleTextElements.SetActive(true);
            } else {
                textElements.SetActive(true);
                freeStyleTextElements.SetActive(false);
        }
    }

    public void ShowOrHideReciteMesh (bool shouldBeEnabled) {
        foreach (Transform child in reciteBoard.transform) {
            child.GetComponent<MeshRenderer>().enabled = shouldBeEnabled;
        }
    }

    public void ActivateMenu() {
        _dialogueManager.PauseDialogueAudio();
        _witListeningStateManager.TransitionToRelevantMenuNavigationStateBasedOnLevel();
        _dialogueManager.ChangeDialogueState(EDialogueState.IsInMenu);
        StartCoroutine(StartMenuOpenAnimation());
    }

    IEnumerator StartMenuOpenAnimation() {
        if (!menu.activeInHierarchy)
        {
            EnableOrDiableReciteBoard(false);
            menu.SetActive(true);
            menuBoardAnimator.SetTrigger("Open");
            yield return new WaitForSeconds(1.5f);
        }
    }
    public void DeactivateMenu() {
        subtitleText3D.UpdateText("");
        StartCoroutine(StartMenuCloseAnimation());
    }


    IEnumerator StartMenuCloseAnimation() {
        menuBoardAnimator.SetTrigger("Close");
        yield return new WaitForSeconds(1.25f);
        menu.SetActive(false);
    }

    private void EnableOrDiableReciteBoard(bool shouldBeEnabled) {
        GameObject reciteBoard = GameObject.FindWithTag("ReciteBoard");
        if (reciteBoard != null) {
            GameObject.FindWithTag("ReciteBoard").GetComponent<MeshRenderer>().enabled = shouldBeEnabled; 
        }
    }

    public void Resume() {
        // Repeat task if it was in progress, otherwise continue dialogue
        // Set the dialogue and Wit state back to what they were before menu activation
         _dialogueManager.ChangeDialogueState(_dialogueManager.previousDialogueState);
        _witListeningStateManager.TransitionToState(EListeningState.ListeningForMenuActivationCommandsOnly);
        EnableOrDiableReciteBoard(true);

        if (_dialogueManager.currentDialogueState == EDialogueState.IsPerformingATask) {
            _wordReciteManager.enabled = true;
            _wordReciteManager.RepeatSameWord();
        } else if (_dialogueManager.currentDialogueState == EDialogueState.IsPlayingDialogueOnly) {
            _dialogueManager.StartDialogueFromPreviousLine();
        }

        DeactivateMenu();
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
                Resume();
                break;
            case EMenuNavigationResponseType.RECITE_WORDS_RESPONSE:
                _levelTransition.BeginSpecificLevelTransition("Level1");
                break;
            case EMenuNavigationResponseType.RECITE_SENTENCES_RESPONSE:
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
