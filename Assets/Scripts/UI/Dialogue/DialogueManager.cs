using UnityEngine;
using MText;
using System.Collections.Generic;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public int dialogueStartingDelayInSeconds = 2;
    private Modular3DText dialogueText;

    private AnimationDriver catAnimationDriver;

    private LevelManager _levelManager;

    private WordReciteManager _wordReciteManager;

    private LevelTransition _levelTransition;

    private UIManager _uiManager;

    private AudioSource catAudioSource;

    // Variables exposing UI elements which uCat will show/hide during the Intro dialogue
    private GameObject micIcon;

    public int currentDialogueOptionIndex;

    // The line of dialogue (index) when various events should occur
    public int micActivationDialogueIndex; // Mic icon

    public int boardActivationDialogueIndex; // Recite board

    public int taskActivationDialogueIndex; // Actual task begins

    public int introTaskActivationIndex;
    public int level1TaskActivationIndex;
    public int level2TaskActivationIndex;
    public int level3TaskActivationIndex;
    public bool dialogueIsPaused;

    public enum DialogueState {
        IsPlayingDialogueOnly, // Eg during intro (before screen appears)
        IsPerformingATask, // Eg during a word countdown
        IsInMenu // When menu is open
    }

    public DialogueState currentDialogueState;

    public DialogueState previousDialogueState;

    void Start()
    {
        // uCat begins idle so that the first anim can play properly
        dialogueText = GameObject.FindWithTag("DialogueText3D").GetComponent<Modular3DText>();
        catAnimationDriver = GameObject.FindWithTag("uCat").GetComponent<AnimationDriver>();
        _levelManager = FindObjectOfType<LevelManager>();
        _wordReciteManager = FindObjectOfType<WordReciteManager>();
        _uiManager = FindObjectOfType<UIManager>();
        catAudioSource = GameObject.FindWithTag("uCat").GetComponent<AudioSource>();
        _levelTransition = FindObjectOfType<LevelTransition>();
        micIcon = GameObject.FindWithTag("MicIcon");
        catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;

        // Start dialogue
        SetDialogueTaskIndexes();
        StartCoroutine(WaitABitAndThenStartDialogue());
        dialogueIsPaused = false;

        if (_levelManager.currentLevel == "Intro") {
            // Hide the board and mic icon
            _uiManager.ShowOrHideReciteMesh(false);
            micIcon.SetActive(false);
        }
    }

    public IEnumerator WaitABitAndThenStartDialogue() {
        yield return new WaitForSeconds(dialogueStartingDelayInSeconds);
        StartDialogue();
    }

    public void StartDialogueFromPreviousLine() {
        ChangeDialogueState(DialogueState.IsPlayingDialogueOnly);
        if (DialogueHandler.currentDialogueOptionIndex > 0) {
            DialogueHandler.currentDialogueOptionIndex--;
        }
        StopAllCoroutines();
        StartCoroutine(CycleThroughDialogue());
    }
    public void StartDialogue() {
        ChangeDialogueState(DialogueState.IsPlayingDialogueOnly);
        _wordReciteManager.StopAllCoroutines();
        StopAllCoroutines();
        StartCoroutine(CycleThroughDialogue());
    }

    public void ChangeDialogueState(DialogueState newDialogueState) {
        previousDialogueState = currentDialogueState;
        currentDialogueState = newDialogueState;
    }
    
    void ActivateTaskAndPauseDialogue() {
        _wordReciteManager.BeginReciteTask();
        ChangeDialogueState(DialogueState.IsPerformingATask);
    }

    public void SkipDialogueAndGoStraightToTask() {
        ChangeDialogueState(DialogueState.IsPlayingDialogueOnly);
        DialogueHandler.currentDialogueOptionIndex = taskActivationDialogueIndex-1;
        StartCoroutine(CycleThroughDialogue());
    }

    void SetDialogueTaskIndexes() {
        switch (_levelManager.currentLevel) {
            case "Intro":
                taskActivationDialogueIndex = introTaskActivationIndex;
                break;
            case "Level1":
                taskActivationDialogueIndex = level1TaskActivationIndex;
                break;
            case "Level2":
                taskActivationDialogueIndex = level2TaskActivationIndex;
                break;
            case "Level3":
                taskActivationDialogueIndex = level3TaskActivationIndex;
                break;
            default:
                Debug.LogError("No dialogue task indexes set for level " + _levelManager.currentLevel);
                break;
        }
    }

   public void SetDialogueTextAnimationAndSound(Dictionary<int, string> dialogueList, Dictionary<int, AnimationDriver.CatAnimations> dialogueAnimations, Dictionary<int, AudioClip> dialogueAudio)
    {
        // Local variable to track current index
        currentDialogueOptionIndex = DialogueHandler.currentDialogueOptionIndex;

        if (dialogueList.TryGetValue(DialogueHandler.currentDialogueOptionIndex, out string currentDialogueOption))
        {
            // Update dialogue
            dialogueText.UpdateText("uCat: " + currentDialogueOption);

            // Play the relevant animation
            catAnimationDriver.catAnimation = dialogueAnimations[DialogueHandler.currentDialogueOptionIndex];

            PlayDialogueAudio(dialogueAudio[DialogueHandler.currentDialogueOptionIndex]);

            //in the Intro, uCat wants to show the user what icon would be displayed when she is listening to them
            var micState = (DialogueHandler.currentDialogueOptionIndex == micActivationDialogueIndex) ? true : false;
            micIcon.SetActive(micState);

            //in the Intro, uCat wants to show the user the board from an appropriate time (not immediately)
            if(DialogueHandler.currentDialogueOptionIndex >= boardActivationDialogueIndex) ActivateBoard();
        }
        else
        {
            Debug.LogError("Invalid current dialogue option index: " + DialogueHandler.currentDialogueOptionIndex);
        }
    }

    void PlayDialogueAudio(AudioClip currentClip) {
        catAudioSource.clip = currentClip;
        catAudioSource.Play();
    }

    public void PauseDialogueAudio() {
        catAudioSource.Stop();
    }

    public void SkipToNextLine() {
        catAudioSource.Stop();
        DialogueHandler.IncrementDialogueOption();
        StopAllCoroutines();
        StartCoroutine(CycleThroughDialogue());
    }

    private void ActivateBoard() {
        _uiManager.ShowOrHideReciteMesh(true);
        if (_levelManager.currentLevel != "Level3") {
            GameObject.FindWithTag("ReciteText3D").GetComponent<Modular3DText>().UpdateText("...Hello...");
        }
    }

    private IEnumerator CycleThroughDialogue() {
        // TODO move this out of ienumerator, only need to do it once
        Dictionary<int, string> currentDialogueList;
        Dictionary<int, AnimationDriver.CatAnimations> currentAnimationList;
        Dictionary<int, AudioClip> currentAudioList;
        
        // We pass in different dictionaries based on the scene
        switch (_levelManager.currentLevel) {
            case "Intro":
                currentDialogueList = DialogueHandler.uCatIntroDialogue;
                currentAnimationList = DialogueHandler.uCatIntroDialogueAnimations;
                currentAudioList = DialogueHandler.uCatIntroDialogueAudio;
                break;
            case "Level1":
                currentDialogueList = DialogueHandler.uCatLevel1Dialogue;
                currentAnimationList = DialogueHandler.uCatLevel1DialogueAnimations;
                currentAudioList = DialogueHandler.uCatLevel1DialogueAudio;
                break;
            case "Level2":
                currentDialogueList = DialogueHandler.uCatLevel2Dialogue;
                currentAnimationList = DialogueHandler.uCatLevel2DialogueAnimations;
                currentAudioList = DialogueHandler.uCatLevel2DialogueAudio;
                break;
            case "Level3":
                currentDialogueList = DialogueHandler.uCatLevel3Dialogue;
                currentAnimationList = DialogueHandler.uCatLevel3DialogueAnimations;
                currentAudioList = DialogueHandler.uCatLevel3DialogueAudio;
                break;
            default:
                currentDialogueList = null;
                currentAnimationList = null;
                currentAudioList = null;
                Debug.LogError("Dictionary not setup for: " + _levelManager.currentLevel);
                break;
        }

        // Catching error states

        if (currentDialogueList.Count == 0)
        {
            yield break;
        }

        if (currentDialogueState != DialogueState.IsPlayingDialogueOnly) {
            yield break;
        }


        // Trigger the task to begin, and pause dialogue
        if (DialogueHandler.currentDialogueOptionIndex == taskActivationDialogueIndex) {
            ActivateTaskAndPauseDialogue();
            // Increment so that when we return from task we are on the next line
            DialogueHandler.IncrementDialogueOption();
            yield break;
        }

        SetDialogueTextAnimationAndSound(currentDialogueList, currentAnimationList, currentAudioList);
        yield return new WaitWhile(() => catAudioSource.isPlaying);
        yield return new WaitForSeconds(DialogueHandler.timeBetweenLinesInSeconds);
    
        bool noMoreDialogue = DialogueHandler.currentDialogueOptionIndex >= currentDialogueList.Count-1 || currentDialogueList == null || currentAnimationList == null;

        if (noMoreDialogue) {
            // Do not continue incrementing if we are at the end
            EndOfDialogue();
            yield break;
        } else {
            DialogueHandler.IncrementDialogueOption();
            // Otherwise, start the next line as long as user is not performing a task
            if (currentDialogueState == DialogueState.IsPlayingDialogueOnly) {
                StartCoroutine(CycleThroughDialogue());
            }
        }

    }

    void EndOfDialogue() {
        DialogueHandler.currentDialogueOptionIndex = 0;
        catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;
        dialogueText.UpdateText("");
        _levelTransition.BeginLevelTransition();
    }

}
