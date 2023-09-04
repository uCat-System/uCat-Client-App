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

    public AudioSource catAudioSource;

    // Variables exposing UI elements which uCat will show/hide during the Intro dialogue
    public GameObject micIcon;
    public GameObject boardComponent;

    // The line of dialogue (index) when the mic icon display event should occur
    public int micActivationDialogueIndex;

    void Start()
    {
        // uCat begins idle so that the first anim can play properly
        catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;
        StartCoroutine(CycleThroughDialogue());
    }

   public void SetDialogueTextAnimationAndSound(Dictionary<int, string> dialogueList, 
        Dictionary<int, AnimationDriver.CatAnimations> dialogueAnimations, Dictionary<int, AudioClip> dialogueAudio)
    {
        Debug.Log("SetDialogueTextAnimationAndSound" + DialogueHandler.currentDialogueOptionIndex + " " + dialogueList.Count + " " + dialogueAnimations.Count);
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
            if(DialogueHandler.currentDialogueOptionIndex >= 4) boardComponent.SetActive(true);

        }
        else
        {
            Debug.LogError("Invalid current dialogue option index: " + DialogueHandler.currentDialogueOptionIndex);
        }
    }

    private IEnumerator CycleThroughDialogue() {
        // TODO move this out of ienumerator, only need to do it once
        Debug.Log("Cycling through with level " + _levelManager.currentLevel);

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
                Debug.Log("Level1");
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

        SetDialogueTextAnimationAndSound(currentDialogueList, currentAnimationList, currentAudioList);
        yield return new WaitWhile(() => catAudioSource.isPlaying);
        yield return new WaitForSeconds(DialogueHandler.timeBetweenLinesInSeconds);
        
        Debug.Log("Checking if we should continue" + DialogueHandler.currentDialogueOptionIndex + " " + currentDialogueList.Count + " " + currentAnimationList.Count);
        if (DialogueHandler.currentDialogueOptionIndex >= currentDialogueList.Count-1 || currentDialogueList == null || currentAnimationList == null) {
            // Do not continue incrementing if we are at the end
            Debug.Log("END OF DIALOGUE");
            EndOfDialogue();
            yield break;
        } else {
            // Otherwise, start the next line
            DialogueHandler.IncrementDialogueOption();
            StartCoroutine(CycleThroughDialogue());
        }
    }

    void EndOfDialogue() {
        DialogueHandler.currentDialogueOptionIndex = 0;
        catAnimationDriver.catAnimation = AnimationDriver.CatAnimations.Idle;
        _wordReciteManager.enabled = true;
        dialogueText.UpdateText("");
    }

}
