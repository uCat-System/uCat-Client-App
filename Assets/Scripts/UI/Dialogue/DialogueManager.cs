using UnityEngine;
using MText;
using System.Collections.Generic;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public Modular3DText dialogueText;

    public AnimationDriver catAnimationDriver;

    public LevelManager _levelManager;

    public WordReciteManager _wordReciteManager;

    public LevelTransition _levelTransition;

    public AudioSource catAudioSource;

    // Variables exposing UI elements which uCat will show/hide during the Intro dialogue
    public GameObject micIcon;
    public GameObject boardComponent;

    public int currentDialogueOptionIndex;

    // The line of dialogue (index) when various events should occur
    public int micActivationDialogueIndex; // Mic icon

    public int boardActivationDialogueIndex; // Recite board

    public int taskActivationDialogueIndex; // Actual task begins

    // public DialogueState currentDialogueState;

    public enum DialogueState {
        IsPlayingDialogueOnly, // Eg during intro (before screen appears)
        IsPerformingATask, // Eg during a word countdown
        // IsPlayingDialogueDuringTask, // Eg during a word countdown, but the dialogue is still playing (eg Good job, try again, etc)
    }

    public DialogueState currentDialogueState;

    void Start()
    {
        // uCat begins idle so that the first anim can play properly
        _levelTransition = FindObjectOfType<LevelTransition>();
        catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;
        StartDialogue();
    }

    public void StartDialogue() {
        Debug.Log("Starting dialogue. Index is " + DialogueHandler.currentDialogueOptionIndex);
        currentDialogueState = DialogueState.IsPlayingDialogueOnly;
        StartCoroutine(CycleThroughDialogue());
    }

    void ActivateTaskAndPauseDialogue() {
        _wordReciteManager.enabled = true;
        currentDialogueState = DialogueState.IsPerformingATask;
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

            // Play the dialogue audio
            // catAudioSource.clip = dialogueAudio[UcatDialogueHandler.currentDialogueOptionIndex];
            catAudioSource.PlayOneShot(dialogueAudio[DialogueHandler.currentDialogueOptionIndex]);

            //in the Intro, uCat wants to show the user what icon would be displayed when she is listening to them
            var micState = (DialogueHandler.currentDialogueOptionIndex == micActivationDialogueIndex) ? true : false;
            micIcon.SetActive(micState);

            //in the Intro, uCat wants to show the user the board from an appropriate time (not immediately)
            boardComponent.SetActive(false);
            if(DialogueHandler.currentDialogueOptionIndex >= boardActivationDialogueIndex) boardComponent.SetActive(true);
        }
        else
        {
            Debug.LogError("Invalid current dialogue option index: " + DialogueHandler.currentDialogueOptionIndex);
        }
    }

    private IEnumerator CycleThroughDialogue() {
        // TODO move this out of ienumerator, only need to do it once
        Debug.Log("Cycling through with level " + _levelManager.currentLevel);
        Debug.Log("Current index is " + DialogueHandler.currentDialogueOptionIndex);

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

        if (currentDialogueList.Count == 0)
        {
            Debug.LogError("No dialogue lines found");
            EndOfDialogue();
            yield break;
        }


        // Trigger the task to begin, and pause dialogue
        if (DialogueHandler.currentDialogueOptionIndex == taskActivationDialogueIndex) {
            //  if its currently '10'
            Debug.Log("Activating task");
            ActivateTaskAndPauseDialogue();
            // Increment so that when we return from task we are on the next line
            DialogueHandler.IncrementDialogueOption();
            Debug.Log("Breaking out");
            yield break;
        }

        SetDialogueTextAnimationAndSound(currentDialogueList, currentAnimationList, currentAudioList);
        yield return new WaitWhile(() => catAudioSource.isPlaying);
        yield return new WaitForSeconds(DialogueHandler.timeBetweenLinesInSeconds);
    

        if (DialogueHandler.currentDialogueOptionIndex >= currentDialogueList.Count-1 || currentDialogueList == null || currentAnimationList == null) {
            // Do not continue incrementing if we are at the end
            Debug.Log("End of dialogue");
            EndOfDialogue();
            yield break;
        } else {
            Debug.Log("Not end of dialogue");
            DialogueHandler.IncrementDialogueOption();
            // Otherwise, start the next line as long as user is not performing a task
            if (currentDialogueState != DialogueState.IsPerformingATask) {
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
